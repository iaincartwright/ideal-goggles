using System.Threading;

namespace iConsole
{
	class iConsoleThreadInput : iConsoleThread
	{
		public iConsoleThreadInput(iConsoleHandle handle, string parentPipe)
			: base()
		{
			Start(this, handle, parentPipe);
		}

		protected override void ThreadProcedure()
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;

			WaitForKeystroke();
		}

		private void WaitForKeystroke()
		{
			iWin32Console.WaitForSingleObject(Handle, -1);

			return;
		}
	}
}