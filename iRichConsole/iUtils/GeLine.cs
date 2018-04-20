//
// getline.cs: A command line editor
//
// Authors:
//   Miguel de Icaza (miguel@novell.com)
//
// Copyright 2008 Novell, Inc.
//
// Dual-licensed under the terms of the MIT X11 license or the
// Apache License 2.0
//
// USE -define:DEMO to build this as a standalone file and test it
//
// TODO:
//    Enter an error (a = 1);  Notice how the prompt is in the wrong line
//		This is caused by Stderr not being tracked by System.Console.
//    Completion support
//    Why is Thread.Interrupt not working?   Currently I resort to Abort which is too much.
//
// Limitations in System.Console:
//    Console needs SIGWINCH support of some sort
//    Console needs a way of updating its position after things have been written
//    behind its back (P/Invoke puts for example).
//    System.Console needs to get the DELETE character, and report accordingly.
//    Typing before the program start causes the cursor position to be wrong
//              This is caused by Console not reading all the available data
//              before sending the report-position sequence and reading it back.
//

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace iUtils
{
  public class LineEditor
  {
    public delegate string[] AutoCompleteHandler(string a_text, int a_pos);

    //static StreamWriter log;

    // The text being edited.
    StringBuilder _text;

    // The text as it is rendered (replaces (char)1 with ^A on display for example).
    StringBuilder _renderedText;

    // The prompt specified, and the prompt shown to the user.
    string _prompt;
    string _shownPrompt;

    // The current cursor position, indexes into "text", for an index
    // into rendered_text, use TextToRenderPos
    int _cursor;

    // The row where we started displaying data.
    int _homeRow;

    // The maximum length that has been displayed on the screen
    int _maxRendered;

    // If we are done editing, this breaks the interactive loop
    bool _done = false;

    // The thread where the Editing started taking place
    Thread _editThread;

    // Our object that tracks history
    History _history;

    // The contents of the kill buffer (cut/paste in Emacs parlance)
    string _killBuffer = "";

    // The string being searched for
    string _search;
    string _lastSearch;

    // whether we are searching (-1= reverse; 0 = no; 1 = forward)
    int _searching;

    // The position where we found the match.
    int _matchAt;

    // Used to implement the Kill semantics (multiple Alt-Ds accumulate)
    KeyHandler _lastHandler;

    delegate void KeyHandler();

    struct Handler
    {
      public ConsoleKeyInfo Cki;
      public KeyHandler KeyHandler;

      public Handler(ConsoleKey a_key, KeyHandler a_h)
      {
        Cki = new ConsoleKeyInfo((char)0, a_key, false, false, false);
        KeyHandler = a_h;
      }

      public Handler(char a_c, KeyHandler a_h)
      {
        KeyHandler = a_h;
        // Use the "Zoom" as a flag that we only have a character.
        Cki = new ConsoleKeyInfo(a_c, ConsoleKey.Zoom, false, false, false);
      }

      public Handler(ConsoleKeyInfo a_cki, KeyHandler a_h)
      {
        Cki = a_cki;
        KeyHandler = a_h;
      }

      public static Handler Control(char a_c, KeyHandler a_h)
      {
        return new Handler((char)(a_c - 'A' + 1), a_h);
      }

      public static Handler Alt(char a_c, ConsoleKey a_k, KeyHandler a_h)
      {
        ConsoleKeyInfo cki = new ConsoleKeyInfo((char)a_c, a_k, false, true, false);
        return new Handler(cki, a_h);
      }
    }

    /// <summary>
    ///   Invoked when the user requests auto-completion using the tab character
    /// </summary>
    /// <remarks>
    ///    The succeed is null for no values found, an array with a single
    ///    string, in that case the string should be the text to be inserted
    ///    for example if the word at pos is "T", the succeed for a completion
    ///    of "ToString" should be "oString", not "ToString".
    ///
    ///    When there are multiple results, the succeed should be the full
    ///    text
    /// </remarks>
    public AutoCompleteHandler AutoCompleteEvent;

    static Handler[] s_handlers;

    public LineEditor(string a_name) : this(a_name, 10) { }

    public LineEditor(string a_name, int a_histsize)
    {
      s_handlers = new Handler[] {
				new Handler (ConsoleKey.Home,       CmdHome),
				new Handler (ConsoleKey.End,        CmdEnd),
				new Handler (ConsoleKey.LeftArrow,  CmdLeft),
				new Handler (ConsoleKey.RightArrow, CmdRight),
				new Handler (ConsoleKey.UpArrow,    CmdHistoryPrev),
				new Handler (ConsoleKey.DownArrow,  CmdHistoryNext),
				new Handler (ConsoleKey.Enter,      CmdDone),
				new Handler (ConsoleKey.Backspace,  CmdBackspace),
				new Handler (ConsoleKey.Delete,     CmdDeleteChar),
				new Handler (ConsoleKey.Tab,        CmdTabOrComplete),

				// Emacs keys
				Handler.Control ('A', CmdHome),
				Handler.Control ('E', CmdEnd),
				Handler.Control ('B', CmdLeft),
				Handler.Control ('F', CmdRight),
				Handler.Control ('P', CmdHistoryPrev),
				Handler.Control ('N', CmdHistoryNext),
				Handler.Control ('K', CmdKillToEof),
				Handler.Control ('Y', CmdYank),
				Handler.Control ('D', CmdDeleteChar),
				Handler.Control ('L', CmdRefresh),
				Handler.Control ('R', CmdReverseSearch),
				Handler.Control ('G', delegate {} ),
				Handler.Alt ('B', ConsoleKey.B, CmdBackwardWord),
				Handler.Alt ('F', ConsoleKey.F, CmdForwardWord),

				Handler.Alt ('D', ConsoleKey.D, CmdDeleteWord),
				Handler.Alt ((char) 8, ConsoleKey.Backspace, CmdDeleteBackword),

				// DEBUG
				Handler.Control ('T', CmdDebug),

				// quote
				Handler.Control ('Q', delegate { HandleChar (Console.ReadKey (true).KeyChar); })
			};

      _renderedText = new StringBuilder();
      _text = new StringBuilder();

      _history = new History(a_name, a_histsize);

      //if (File.Exists ("log"))File.Delete ("log");
      //log = File.CreateText ("log");
    }

    void CmdDebug()
    {
      _history.Dump();
      Console.WriteLine();
      Render();
    }

    void Render()
    {
      Console.Write(_shownPrompt);
      Console.Write(_renderedText);

      int max = System.Math.Max(_renderedText.Length + _shownPrompt.Length, _maxRendered);

      for (int i = _renderedText.Length + _shownPrompt.Length; i < _maxRendered; i++)
        Console.Write(' ');
      _maxRendered = _shownPrompt.Length + _renderedText.Length;

      // Write one more to ensure that we always wrap around properly if we are at the
      // end of a line.
      Console.Write(' ');

      UpdateHomeRow(max);
    }

    void UpdateHomeRow(int a_screenpos)
    {
      int lines = 1 + (a_screenpos / Console.WindowWidth);

      _homeRow = Console.CursorTop - (lines - 1);
      if (_homeRow < 0)
        _homeRow = 0;
    }

    void RenderFrom(int a_pos)
    {
      int rpos = TextToRenderPos(a_pos);
      int i;

      for (i = rpos; i < _renderedText.Length; i++)
        Console.Write(_renderedText[i]);

      if ((_shownPrompt.Length + _renderedText.Length) > _maxRendered)
        _maxRendered = _shownPrompt.Length + _renderedText.Length;
      else
      {
        int maxExtra = _maxRendered - _shownPrompt.Length;
        for (; i < maxExtra; i++)
          Console.Write(' ');
      }
    }

    void ComputeRendered()
    {
      _renderedText.Length = 0;

      for (int i = 0; i < _text.Length; i++)
      {
        int c = (int)_text[i];
        if (c < 26)
        {
          if (c == '\t')
            _renderedText.Append("    ");
          else
          {
            _renderedText.Append('^');
            _renderedText.Append((char)(c + (int)'A' - 1));
          }
        }
        else
          _renderedText.Append((char)c);
      }
    }

    int TextToRenderPos(int a_pos)
    {
      int p = 0;

      for (int i = 0; i < a_pos; i++)
      {
        int c;

        c = (int)_text[i];

        if (c < 26)
        {
          if (c == 9)
            p += 4;
          else
            p += 2;
        }
        else
          p++;
      }

      return p;
    }

    int TextToScreenPos(int a_pos)
    {
      return _shownPrompt.Length + TextToRenderPos(a_pos);
    }

    string Prompt
    {
      get => _prompt;
	    set { _prompt = value; }
    }

    int LineCount => (_shownPrompt.Length + _renderedText.Length) / Console.WindowWidth;

	  void ForceCursor(int a_newpos)
    {
      _cursor = a_newpos;

      int actualPos = _shownPrompt.Length + TextToRenderPos(_cursor);
      int row = _homeRow + (actualPos / Console.WindowWidth);
      int col = actualPos % Console.WindowWidth;

      if (row >= Console.BufferHeight)
        row = Console.BufferHeight - 1;
      Console.SetCursorPosition(col, row);

      //log.WriteLine ("Going to cursor={0} row={1} _col={2} actual={3} prompt={4} ttr={5} old={6}", newpos, row, _col, actual_pos, prompt.Length, TextToRenderPos (cursor), cursor);
      //log.Flush ();
    }

    void UpdateCursor(int a_newpos)
    {
      if (_cursor == a_newpos)
        return;

      ForceCursor(a_newpos);
    }

    void InsertChar(char a_c)
    {
      int prevLines = LineCount;
      _text = _text.Insert(_cursor, a_c);
      ComputeRendered();
      if (prevLines != LineCount)
      {
        Console.SetCursorPosition(0, _homeRow);
        Render();
        ForceCursor(++_cursor);
      }
      else
      {
        RenderFrom(_cursor);
        ForceCursor(++_cursor);
        UpdateHomeRow(TextToScreenPos(_cursor));
      }
    }

    //
    // Commands
    //
    void CmdDone()
    {
      _done = true;
    }

    void CmdTabOrComplete()
    {
      bool complete = false;

      if (AutoCompleteEvent != null)
      {
        for (int i = 0; i < _cursor; i++)
        {
          if (!Char.IsWhiteSpace(_text[i]))
          {
            complete = true;
            break;
          }
        }
        if (complete)
        {
          string[] completions = AutoCompleteEvent(_text.ToString(), _cursor);
          if (completions == null || completions.Length == 0)
            return;

          if (completions.Length == 1)
          {
            InsertTextAtCursor(completions[0]);
          }
          else
          {
            Console.WriteLine();
            foreach (string s in completions)
            {
              Console.Write(s);
              Console.Write(' ');
            }
            Console.WriteLine();
            Render();
            ForceCursor(_cursor);
          }
        }
        else
          HandleChar('\t');
      }
      else
        HandleChar('t');
    }

    void CmdHome()
    {
      UpdateCursor(0);
    }

    void CmdEnd()
    {
      UpdateCursor(_text.Length);
    }

    void CmdLeft()
    {
      if (_cursor == 0)
        return;

      UpdateCursor(_cursor - 1);
    }

    void CmdBackwardWord()
    {
      int p = WordBackward(_cursor);
      if (p == -1)
        return;
      UpdateCursor(p);
    }

    void CmdForwardWord()
    {
      int p = WordForward(_cursor);
      if (p == -1)
        return;
      UpdateCursor(p);
    }

    void CmdRight()
    {
      if (_cursor == _text.Length)
        return;

      UpdateCursor(_cursor + 1);
    }

    void RenderAfter(int a_p)
    {
      ForceCursor(a_p);
      RenderFrom(a_p);
      ForceCursor(_cursor);
    }

    void CmdBackspace()
    {
      if (_cursor == 0)
        return;

      _text.Remove(--_cursor, 1);
      ComputeRendered();
      RenderAfter(_cursor);
    }

    void CmdDeleteChar()
    {
      // If there is no input, this behaves like EOF
      if (_text.Length == 0)
      {
        _done = true;
        _text = null;
        Console.WriteLine();
        return;
      }

      if (_cursor == _text.Length)
        return;
      _text.Remove(_cursor, 1);
      ComputeRendered();
      RenderAfter(_cursor);
    }

    int WordForward(int a_p)
    {
      if (a_p >= _text.Length)
        return -1;

      int i = a_p;
      if (Char.IsPunctuation(_text[a_p]) || Char.IsWhiteSpace(_text[a_p]))
      {
        for (; i < _text.Length; i++)
        {
          if (Char.IsLetterOrDigit(_text[i]))
            break;
        }
        for (; i < _text.Length; i++)
        {
          if (!Char.IsLetterOrDigit(_text[i]))
            break;
        }
      }
      else
      {
        for (; i < _text.Length; i++)
        {
          if (!Char.IsLetterOrDigit(_text[i]))
            break;
        }
      }
      if (i != a_p)
        return i;
      return -1;
    }

    int WordBackward(int a_p)
    {
      if (a_p == 0)
        return -1;

      int i = a_p - 1;
      if (i == 0)
        return 0;

      if (Char.IsPunctuation(_text[i]) || Char.IsSymbol(_text[i]) || Char.IsWhiteSpace(_text[i]))
      {
        for (; i >= 0; i--)
        {
          if (Char.IsLetterOrDigit(_text[i]))
            break;
        }
        for (; i >= 0; i--)
        {
          if (!Char.IsLetterOrDigit(_text[i]))
            break;
        }
      }
      else
      {
        for (; i >= 0; i--)
        {
          if (!Char.IsLetterOrDigit(_text[i]))
            break;
        }
      }
      i++;

      if (i != a_p)
        return i;

      return -1;
    }

    void CmdDeleteWord()
    {
      int pos = WordForward(_cursor);

      if (pos == -1)
        return;

      string k = _text.ToString(_cursor, pos - _cursor);

      if (_lastHandler == CmdDeleteWord)
        _killBuffer = _killBuffer + k;
      else
        _killBuffer = k;

      _text.Remove(_cursor, pos - _cursor);
      ComputeRendered();
      RenderAfter(_cursor);
    }

    void CmdDeleteBackword()
    {
      int pos = WordBackward(_cursor);
      if (pos == -1)
        return;

      string k = _text.ToString(pos, _cursor - pos);

      if (_lastHandler == CmdDeleteBackword)
        _killBuffer = k + _killBuffer;
      else
        _killBuffer = k;

      _text.Remove(pos, _cursor - pos);
      ComputeRendered();
      RenderAfter(pos);
    }

    //
    // Adds the current line to the history if needed
    //
    void HistoryUpdateLine()
    {
      _history.Update(_text.ToString());
    }

    void CmdHistoryPrev()
    {
      if (!_history.PreviousAvailable())
        return;

      HistoryUpdateLine();

      SetText(_history.Previous());
    }

    void CmdHistoryNext()
    {
      if (!_history.NextAvailable())
        return;

      _history.Update(_text.ToString());
      SetText(_history.Next());
    }

    void CmdKillToEof()
    {
      _killBuffer = _text.ToString(_cursor, _text.Length - _cursor);
      _text.Length = _cursor;
      ComputeRendered();
      RenderAfter(_cursor);
    }

    void CmdYank()
    {
      InsertTextAtCursor(_killBuffer);
    }

    void InsertTextAtCursor(string a_str)
    {
      int prevLines = LineCount;
      _text.Insert(_cursor, a_str);
      ComputeRendered();
      if (prevLines != LineCount)
      {
        Console.SetCursorPosition(0, _homeRow);
        Render();
        _cursor += a_str.Length;
        ForceCursor(_cursor);
      }
      else
      {
        RenderFrom(_cursor);
        _cursor += a_str.Length;
        ForceCursor(_cursor);
        UpdateHomeRow(TextToScreenPos(_cursor));
      }
    }

    void SetSearchPrompt(string a_s)
    {
      SetPrompt("(reverse-i-search)`" + a_s + "': ");
    }

    void ReverseSearch()
    {
      int p;

      if (_cursor == _text.Length)
      {
        // The cursor is at the end of the string

        p = _text.ToString().LastIndexOf(_search);
        if (p != -1)
        {
          _matchAt = p;
          _cursor = p;
          ForceCursor(_cursor);
          return;
        }
      }
      else
      {
        // The cursor is somewhere in the middle of the string
        int start = (_cursor == _matchAt) ? _cursor - 1 : _cursor;
        if (start != -1)
        {
          p = _text.ToString().LastIndexOf(_search, start);
          if (p != -1)
          {
            _matchAt = p;
            _cursor = p;
            ForceCursor(_cursor);
            return;
          }
        }
      }

      // Need to search backwards in history
      HistoryUpdateLine();
      string s = _history.SearchBackward(_search);
      if (s != null)
      {
        _matchAt = -1;
        SetText(s);
        ReverseSearch();
      }
    }

    void CmdReverseSearch()
    {
      if (_searching == 0)
      {
        _matchAt = -1;
        _lastSearch = _search;
        _searching = -1;
        _search = "";
        SetSearchPrompt("");
      }
      else
      {
        if (_search == "")
        {
          if (_lastSearch != "" && _lastSearch != null)
          {
            _search = _lastSearch;
            SetSearchPrompt(_search);

            ReverseSearch();
          }
          return;
        }
        ReverseSearch();
      }
    }

    void SearchAppend(char a_c)
    {
      _search = _search + a_c;
      SetSearchPrompt(_search);

      //
      // If the new typed data still matches the current text, stay here
      //
      if (_cursor < _text.Length)
      {
        string r = _text.ToString(_cursor, _text.Length - _cursor);
        if (r.StartsWith(_search))
          return;
      }

      ReverseSearch();
    }

    void CmdRefresh()
    {
      Console.Clear();
      _maxRendered = 0;
      Render();
      ForceCursor(_cursor);
    }

    void InterruptEdit(object a_sender, ConsoleCancelEventArgs a_a)
    {
      // Do not abort our program:
      a_a.Cancel = true;

      // Interrupt the editor
      _editThread.Abort();
    }

    void HandleChar(char a_c)
    {
      if (_searching != 0)
        SearchAppend(a_c);
      else
        InsertChar(a_c);
    }

    void EditLoop()
    {
      ConsoleKeyInfo cki;

      while (!_done)
      {
        cki = Console.ReadKey(true);

        bool handled = false;
        foreach (Handler handler in s_handlers)
        {
          ConsoleKeyInfo t = handler.Cki;

          if (t.Key == cki.Key && t.Modifiers == cki.Modifiers)
          {
            handled = true;
            handler.KeyHandler();
            _lastHandler = handler.KeyHandler;
            break;
          }
          else if (t.KeyChar == cki.KeyChar && t.Key == ConsoleKey.Zoom)
          {
            handled = true;
            handler.KeyHandler();
            _lastHandler = handler.KeyHandler;
            break;
          }
        }
        if (handled)
        {
          if (_searching != 0)
          {
            if (_lastHandler != CmdReverseSearch)
            {
              _searching = 0;
              SetPrompt(_prompt);
            }
          }
          continue;
        }

        if (cki.KeyChar != (char)0)
          HandleChar(cki.KeyChar);
      }
    }

    void InitText(string a_initial)
    {
      _text = new StringBuilder(a_initial);
      ComputeRendered();
      _cursor = _text.Length;
      Render();
      ForceCursor(_cursor);
    }

    void SetText(string a_newtext)
    {
      Console.SetCursorPosition(0, _homeRow);
      InitText(a_newtext);
    }

    void SetPrompt(string a_newprompt)
    {
      _shownPrompt = a_newprompt;
      Console.SetCursorPosition(0, _homeRow);
      Render();
      ForceCursor(_cursor);
    }

    public string Edit(string a_prompt, string a_initial)
    {
      _editThread = Thread.CurrentThread;
      _searching = 0;
      Console.CancelKeyPress += InterruptEdit;

      _done = false;
      _history.CursorToEnd();
      _maxRendered = 0;

      Prompt = a_prompt;
      _shownPrompt = a_prompt;
      InitText(a_initial);
      _history.Append(a_initial);

      do
      {
        try
        {
          EditLoop();
        }
        catch (ThreadAbortException)
        {
          _searching = 0;
          Thread.ResetAbort();
          Console.WriteLine();
          SetPrompt(a_prompt);
          SetText("");
        }
      } while (!_done);
      Console.WriteLine();

      Console.CancelKeyPress -= InterruptEdit;

      if (_text == null)
      {
        _history.Close();
        return null;
      }

      string result = _text.ToString();
      if (result != "")
        _history.Accept(result);
      else
        _history.RemoveLast();

      return result;
    }

    //
    // Emulates the bash-like behavior, where edits done to the
    // history are recorded
    //
    class History
    {
      string[] _history;
      int _head, _tail;
      int _cursor, _count;
      string _histfile;

      public History(string a_app, int a_size)
      {
        if (a_size < 1)
          throw new ArgumentException("size");

        if (a_app != null)
        {
          string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
          //Console.WriteLine (dir);
          if (!Directory.Exists(dir))
          {
            try
            {
              Directory.CreateDirectory(dir);
            }
            catch
            {
              a_app = null;
            }
          }
          if (a_app != null)
            _histfile = Path.Combine(dir, a_app) + ".history";
        }

        _history = new string[a_size];
        _head = _tail = _cursor = 0;

        if (File.Exists(_histfile))
        {
          using (StreamReader sr = File.OpenText(_histfile))
          {
            string line;

            while ((line = sr.ReadLine()) != null)
            {
              if (line != "")
                Append(line);
            }
          }
        }
      }

      public void Close()
      {
        if (_histfile == null)
          return;

        try
        {
          using (StreamWriter sw = File.CreateText(_histfile))
          {
            int start = (_count == _history.Length) ? _head : _tail;
            for (int i = start; i < start + _count; i++)
            {
              int p = i % _history.Length;
              sw.WriteLine(_history[p]);
            }
          }
        }
        catch
        {
          // ignore
        }
      }

      //
      // Appends a value to the history
      //
      public void Append(string a_s)
      {
        //Console.WriteLine ("APPENDING {0} {1}", s, Environment.StackTrace);
        _history[_head] = a_s;
        _head = (_head + 1) % _history.Length;
        if (_head == _tail)
          _tail = (_tail + 1 % _history.Length);
        if (_count != _history.Length)
          _count++;
      }

      //
      // Updates the current cursor location with the string,
      // to support editing of history items.   For the current
      // line to participate, an Append must be done before.
      //
      public void Update(string a_s)
      {
        _history[_cursor] = a_s;
      }

      public void RemoveLast()
      {
        _head = _head - 1;
        if (_head < 0)
          _head = _history.Length - 1;
      }

      public void Accept(string a_s)
      {
        int t = _head - 1;
        if (t < 0)
          t = _history.Length - 1;

        _history[t] = a_s;
      }

      public bool PreviousAvailable()
      {
        //Console.WriteLine ("h={0} t={1} cursor={2}", head, tail, cursor);
        if (_count == 0 || _cursor == _tail)
          return false;

        return true;
      }

      public bool NextAvailable()
      {
        int next = (_cursor + 1) % _history.Length;
        if (_count == 0 || next > _head)
          return false;

        return true;
      }

      //
      // Returns: a string with the previous line contents, or
      // nul if there is no data in the history to move to.
      //
      public string Previous()
      {
        if (!PreviousAvailable())
          return null;

        _cursor--;
        if (_cursor < 0)
          _cursor = _history.Length - 1;

        return _history[_cursor];
      }

      public string Next()
      {
        if (!NextAvailable())
          return null;

        _cursor = (_cursor + 1) % _history.Length;
        return _history[_cursor];
      }

      public void CursorToEnd()
      {
        if (_head == _tail)
          return;

        _cursor = _head;
      }

      public void Dump()
      {
        Console.WriteLine("Head={0} Tail={1} Cursor={2}", _head, _tail, _cursor);
        for (int i = 0; i < _history.Length; i++)
        {
          Console.WriteLine(" {0} {1}: {2}", i == _cursor ? "==>" : "   ", i, _history[i]);
        }
        //log.Flush ();
      }

      public string SearchBackward(string a_term)
      {
        for (int i = 1; i < _count; i++)
        {
          int slot = _cursor - i;
          if (slot < 0)
            slot = _history.Length - 1;
          if (_history[slot] != null && _history[slot].IndexOf(a_term) != -1)
          {
            _cursor = slot;
            return _history[slot];
          }

          // Will the next hit tail?
          slot--;
          if (slot < 0)
            slot = _history.Length - 1;
          if (slot == _tail)
            break;
        }

        return null;
      }
    }
  }

#if DEMO
	class Demo {
		static void Main ()
		{
			LineEditor le = new LineEditor (null);
			string s;

			while ((s = le.Edit ("shell> ", "")) != null){
				Console.WriteLine ("----> [{0}]", s);
			}
		}
	}
#endif
}