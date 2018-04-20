using System;
using System.Drawing;
using System.IO;

namespace iUtils
{
  public enum LogType
  {
    None,
    Console,
    Messagebox,
    Callback
  }

  public delegate void LogDelegate(string a_message, Color a_colour);

  public static class iLog
  {
    private static string s_logPrefix = "    ";
    private static LogType s_logType = LogType.None;
    private static LogDelegate s_logFunc = null;
    private static StreamWriter s_logStream = null;

    private static bool s_appendLog = false;

    static iLog()
    {
      // grabs a console for console logging
      Win32.AllocConsole();
    }

    public static LogDelegate LogFunc
    {
      set => s_logFunc = value;
    }

    public static LogType LogTo
    {
      get => s_logType;
	    set { s_logType = value; }
    }

    public static string LogPrefix
    {
      get => s_logPrefix;
	    set { SetLogPrefix(value); }
    }

    public static bool AppendLog
    {
      get => s_appendLog;
	    set { s_appendLog = value; }
    }

    public static void CaptureLog(string a_fileName)
    {
      if (s_logStream != null)
        EndCapture();

      if (AppendLog && File.Exists(a_fileName))
        s_logStream = File.AppendText(a_fileName);
      else
        s_logStream = File.CreateText(a_fileName);
    }

    public static void EndCapture()
    {
      if (s_logStream != null)
        s_logStream.Close();

      s_logStream = null;
    }

    public static void SetLogPrefix(string a_prefix)
    {
      s_logPrefix = a_prefix;
    }

    public static void LogMessage(string a_format, params object[] a_parameters)
    {
      LogMessage(Color.Black, a_format, a_parameters);
    }

    public static void LogMessage(Color a_colour, string a_format, params object[] a_parameters)
    {
      if (String.IsNullOrEmpty(s_logPrefix))
        LogFilter(a_colour, "" + s_logPrefix + "] :" + a_format, a_parameters);
      else
        LogFilter(a_colour, a_format, a_parameters);
    }

    public static void LogError(string a_format, params object[] a_parameters)
    {
      LogFilter(Color.Red, a_format, a_parameters);
    }

    private static void LogFilter(Color a_colour, string a_format, object[] a_parameters)
    {
	    LogInternal(a_colour, a_parameters.Length > 0 ? String.Format(a_format, a_parameters) : a_format);
    }

    private static void LogInternal(Color a_colour, string a_format)
    {
      if (s_logType == LogType.Console)
      {
        Console.WriteLine(a_format);
      }
      else if (s_logType == LogType.Messagebox)
      {
        System.Windows.Forms.MessageBox.Show(a_format, "Log Message");
      }
      else if (s_logType == LogType.Callback)
      {
	      s_logFunc?.Invoke(a_format, a_colour);
      }

	    s_logStream?.WriteLine(a_format);
    }
  }
}