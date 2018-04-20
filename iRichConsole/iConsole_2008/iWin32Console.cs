using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace iConsole
{
  public class iConsoleHandle : WaitHandle
  {
    public iConsoleHandle(IntPtr handle)
    {
      SafeWaitHandle = new SafeWaitHandle(handle, true);
    }

    public static implicit operator IntPtr(iConsoleHandle h)
    {
      return h.SafeWaitHandle.DangerousGetHandle();
    }
  }

  public partial class iWin32Console
  {
    iConsoleHandle _inputBuffer;
    iConsoleHandle _outputBuffer;

    private List<iConsoleHandle> _hScreenBuffers = new List<iConsoleHandle>();

    IntPtr _consoleHwnd = IntPtr.Zero;

    public iWin32Console()
    {
      // this will fail if we already have a console
      // we could test for console existence but
      // it's easier to just try and create
      AllocConsole();

      _consoleHwnd = GetConsoleWindow();

      _outputBuffer = new iConsoleHandle(OpenOutputBuffer());

      _inputBuffer = new iConsoleHandle(OpenInputBuffer());
    }

    private static IntPtr OpenOutputBuffer()
    {
      IntPtr conOutPtr = CreateFileA("CONOUT$", GENERIC_READ | GENERIC_WRITE, FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

      if (conOutPtr == INVALID_HANDLE_VALUE)
      {
        int lastErrOut = Marshal.GetLastWin32Error();

        Console.WriteLine("OpenOutputBuffer() failed : Win32 error {0}", lastErrOut);
      }

      return conOutPtr;
    }

    private static IntPtr OpenInputBuffer()
    {
      IntPtr conInPtr = CreateFileA("CONIN$", GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

      if (conInPtr == INVALID_HANDLE_VALUE)
      {
        int lastErrOut = Marshal.GetLastWin32Error();

        Console.WriteLine("OpenInputBuffer() failed : Win32 error {0}", lastErrOut);
      }

      return conInPtr;
    }

    uint _access = GENERIC_READ | GENERIC_WRITE;
    uint _share = FILE_SHARE_READ | FILE_SHARE_WRITE;

    public void AddScreenBuffer()
    {
      _hScreenBuffers.Add(new iConsoleHandle(CreateConsoleScreenBuffer(_access, _share, IntPtr.Zero, CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero)));
    }

    public void TestWaitInput()
    {
      iWin32Console.FlushConsoleInputBuffer(_inputBuffer);

      new iConsoleThreadScreen(_outputBuffer, "iConsoleTest");

      //new iConsoleInputThread(_inputBuffer, "parent");

      //Thread.Sleep(1000);

      //iWin32Console.SetConsoleActiveScreenBuffer(new iConsoleHandle(CreateConsoleScreenBuffer(_access, _share, IntPtr.Zero, CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero)));

      iConsoleThread.JoinAll();

      Console.WriteLine("Done waiting...To sleep, perchance to dream");
    }

    CONSOLE_FONT_INFO _currentFont;

    public void TestWrite(string testOut, UInt16 attribs, short top, short left)
    {
      uint[] procIDs = new uint[64];

      GetConsoleProcessList(procIDs, 64);

      bool retval = GetCurrentConsoleFont(_outputBuffer, false, out _currentFont);

      if (retval == false)
      {
        int err = Marshal.GetLastWin32Error();
      }

      List<CHAR_INFO> charInfo = new List<CHAR_INFO>();

      foreach (char letter in testOut)
      {
        charInfo.Add(new CHAR_INFO(letter, attribs));
      }

      SMALL_RECT coordScreen;
      coordScreen.Top = top;
      coordScreen.Left = left;
      coordScreen.Right = 79;
      coordScreen.Bottom = 80;

      COORD coordBufferPos;
      coordBufferPos.X = 0;
      coordBufferPos.Y = 0;

      COORD coordBufferSize;
      coordBufferSize.X = (Int16)(testOut.Length / 2);
      coordBufferSize.Y = 2;

      WriteConsoleOutput(_outputBuffer, charInfo.ToArray(), coordBufferSize, coordBufferPos, ref coordScreen);
    }
  }
}