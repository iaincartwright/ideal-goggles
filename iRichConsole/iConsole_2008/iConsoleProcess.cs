using System;
using System.Diagnostics;
using System.IO;

namespace iConsole
{
  public class iConsoleProcess
  {
    private bool isStarted;
    private bool isExited;

    public bool IsExited
    {
      get { return isExited; }
      set { isExited = value; }
    }

    private string _appName;
    private string _args;
    private string _workingDir;

    private Process _theProcess;
    private ProcessStartInfo _startInfo;

    public ProcessStartInfo StartInfo
    {
      get { return _startInfo; }
      set { _startInfo = value; }
    }

    private iConsoleProcess(string appName, string args)
    {
      _appName = appName;
      _args = args;
      _workingDir = "";

      _theProcess = new Process();

      _startInfo = new ProcessStartInfo(_appName, _args);
    }

    public static iConsoleProcess Create(string appName, string args)
    {
      return new iConsoleProcess(appName, args);
    }

    public static iConsoleProcess Create(string appName)
    {
      return new iConsoleProcess(appName, "");
    }

    public static iConsoleProcess Create()
    {
      return new iConsoleProcess("", "");
    }

    public string AppName
    {
      get { return _appName; }
      set { _appName = value; }
    }

    public string Args
    {
      get { return _args; }
      set { _args = value; }
    }

    public string WorkingDir
    {
      get { return _workingDir; }
      set { _workingDir = value; }
    }

    public StreamReader StdOut
    {
      get { return _theProcess.StandardOutput; }
    }

    public bool Start()
    {
      FillDefaults();

      ConvertIfBatch();

      iConsoleThread.ChildProcess = this;

      _theProcess.StartInfo = _startInfo;

      _theProcess.StartInfo.FileName = _appName;
      _theProcess.StartInfo.Arguments = _args;
      _theProcess.StartInfo.WorkingDirectory = _workingDir;

      _theProcess.StartInfo.CreateNoWindow = false;   // use the parent's window...
      _theProcess.StartInfo.UseShellExecute = false;  // ...if this is false
      _theProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

      //_theProcess.StartInfo.RedirectStandardOutput = true;
      //_theProcess.StartInfo.RedirectStandardError = true;

      _theProcess.EnableRaisingEvents = true;
      _theProcess.Exited += new EventHandler(_theProcess_Exited);

      return isStarted = _theProcess.Start();
    }

    void _theProcess_Exited(object sender, EventArgs e)
    {
      isExited = true;
    }

    public bool Kill()
    {
      try
      {
        if (_theProcess.HasExited == false)
          _theProcess.Kill();
      }
      catch { }

      return true;
    }

    private void FillDefaults()
    {
      if (String.IsNullOrEmpty(_appName))
      {
        _appName = Environment.GetEnvironmentVariable("ComSpec");
        _args = "/K";
      }
    }

    private void ConvertIfBatch()
    {
      // instead of this switch statement
      // we could read a config file and
      // do the conversion based on that
      switch (Path.GetExtension(_appName.ToLowerInvariant()))
      {
        case null:
        case "":
        case ".":
          // it has no extension so we can't id
          // it as a script
          // TODO: read first line for #/bash/bin header
          break;

        // no processing required
        case ".exe":
          return;

        case ".bat":
        case ".cmd":
          _args = String.Format("/C \"{0}\" {1}", _appName, _args);

          _appName = Environment.GetEnvironmentVariable("ComSpec");

          break;

        case ".pyw":
          goto case ".py";

        case ".py":
          if (SearchPathFor("python.exe"))
          {
          }
          break;

        default:
          // unrecognised extension - should we assert, warn
          // the user and/or just try and run it anyway?
          return;
      }

      return;
    }

    private bool SearchPathFor(string fileName)
    {
      char[] pathDelims = { ';' };

      string[] paths = Environment.GetEnvironmentVariable("PATH").Split(pathDelims, StringSplitOptions.RemoveEmptyEntries);

      foreach (string path in paths)
      {
        if (File.Exists(path + (path.EndsWith("\\") ? "" : "\\") + fileName))
          return true;
      }

      return false;
    }
  }
}