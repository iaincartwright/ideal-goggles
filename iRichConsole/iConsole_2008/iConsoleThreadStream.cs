using System.Threading;

namespace iConsole
{
  class iConsoleThreadStream : iConsoleThread
  {
    public iConsoleThreadStream(iConsoleHandle handle, string parentPipe)
      : base()
    {
      Start(this, handle, parentPipe);
    }

    protected override void ThreadProcedure()
    {
      TheThread.Name = "iConsoleThreadScreen";

      TheThread.Priority = ThreadPriority.Highest;

      //ChildProcess.
    }
  }
}