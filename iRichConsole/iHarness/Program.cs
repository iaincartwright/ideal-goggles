using System;
using System.IO;
using System.Linq;
using System.Threading;
using iConsole;

namespace iHarness
{
  class Program
  {
    static FileStream fs;

    static void Main(string[] args)
    {
      if (false)
      {
        int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
        int oddNumbers = numbers.Count(n => n % 2 == 1);

        Func<int, bool> fred;

        fred = x => x > 0;

        numbers.Count(fred);
      }
      else
      {
        iWin32Console console = new iWin32Console();

        console.TestWrite("Hello, My name is Iain Cartwright and I am here to cause difficulties in your life",
                            iWin32Console.FOREGROUND_BLUE | iWin32Console.FOREGROUND_INTENSITY,
                            5, 35);

        console.TestWrite("This string contains the unicode character 3/4 (\u00f3) in the non-last position",
                            iWin32Console.FOREGROUND_RED | iWin32Console.FOREGROUND_INTENSITY | iWin32Console.BACKGROUND_BLUE | iWin32Console.BACKGROUND_GREEN | iWin32Console.BACKGROUND_RED,
                            6, 15);

        iConsoleProcess proc = iConsoleProcess.Create("cmd.exe", "/K");

        proc.Start();

        console.TestWaitInput();

        Console.WriteLine("Parent process will now sleep...");

        //Thread.Sleep(1000);

        Console.ReadKey();

        proc.Kill();
      }
    }

    static void MainThreadTest(string[] args)
    {
      Console.WriteLine("ID = " + Thread.CurrentThread.ManagedThreadId);
      fs = File.Open("test.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

      fs.BeginRead(new byte[512000], 0, 512000, new AsyncCallback(AsyncReader), "read");

      Console.WriteLine("Done Read - waiting");
      Console.ReadKey(true);

      fs.BeginWrite(new byte[512000], 0, 512000, new AsyncCallback(AsyncReader), "write");

      Console.WriteLine("Done Write - waiting");
      Console.ReadKey(true);

      Console.WriteLine("done - ID = " + Thread.CurrentThread.ManagedThreadId);

      Console.ReadKey(true);
    }

    static void AsyncReader(IAsyncResult result)
    {
      string state = result.AsyncState as string;

      if (state == "read")
        fs.EndRead(result);
      else
        fs.EndWrite(result);

      Console.WriteLine("End of " + state + " - ID = " + Thread.CurrentThread.ManagedThreadId);
    }
  }
}