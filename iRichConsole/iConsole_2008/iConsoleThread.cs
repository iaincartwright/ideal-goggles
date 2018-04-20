using System.Collections.Generic;
using System.Threading;
using iUtils;

namespace iConsole
{
  abstract class iConsoleThread
  {
    //
    // static stuff
    //
    private static List<Thread> s_threadList;
    private static iConsoleProcess s_childProcess;

    static iConsoleThread()
    {
      s_threadList = new List<Thread>();
    }

    public static iConsoleProcess ChildProcess
    {
      get { return s_childProcess; }
      set { s_childProcess = value; }
    }

    protected static void Start(iConsoleThread consoleThread, iConsoleHandle handle, string parentPipe)
    {
      consoleThread._handle = new iConsoleHandle(handle);

      consoleThread.PipeName = parentPipe;

      consoleThread._theThread = new Thread(consoleThread.ThreadProcedureInternal);

      consoleThread._theThread.Start();

      s_threadList.Add(consoleThread._theThread);
    }

    public static void JoinAll()
    {
      while (s_threadList.Count > 0)
      {
        if (s_threadList[0].IsAlive)
        {
          s_threadList[0].Join();
        }

        s_threadList.RemoveAt(0);
      }
    }

    //
    // instance stuff
    //
    protected iConsoleThread()
    {
      _pipe = new iPipe();

      _pipe.PipeReadBufferSizeKB = 256;
      _pipe.PipeWritBufferSizeKB = 256;
    }

    private Thread _theThread;
    private iConsoleHandle _handle;
    private string _pipeName = "iConsoleTest";
    private bool _exitThread;

    private int _refreshTimeMS;
    private long _updateCount;

    public long UpdateCount
    {
      get { return _updateCount; }
      protected set { _updateCount = value; }
    }

    public int RefreshTimeMS
    {
      get { return _refreshTimeMS; }
      set { _refreshTimeMS = value; }
    }

    private iPipe _pipe;

    protected iPipe Pipe
    {
      get { return _pipe; }
    }

    public bool ExitThread
    {
      get { return _exitThread; }
      set { _exitThread = value; }
    }

    protected Thread TheThread
    {
      get { return _theThread; }
    }

    protected string PipeName
    {
      get { return _pipeName; }
      set { _pipeName = value; }
    }

    protected iConsoleHandle Handle
    {
      get { return _handle; }
    }

    protected abstract void ThreadProcedure();

    // start-up in the thread context
    private void ThreadProcedureInternal()
    {
      UpdateCount = 0;

      ExitThread = false;

      RefreshTimeMS = 15;

      ThreadProcedure();
    }
  }
}