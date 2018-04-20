using System;
using System.Runtime.InteropServices;
using System.Threading;
using iUtils;
using iCppLib;

namespace iConsole
{
  [StructLayoutAttribute(LayoutKind.Sequential)]
  public struct CHAR_INFO_EX
  {
    public int Offset;
    public char UnicodeChar;
    public CHAR_ATTRIBS Attributes;
  };

  [StructLayoutAttribute(LayoutKind.Sequential)]
  struct CONSOLE_SCREEN_BUFFER_INFO
  {
    public iWin32Console.COORD dwSize;
    public iWin32Console.COORD dwCursorPosition;
    public ushort wAttributes;
    public iWin32Console.SMALL_RECT srWindow;
    public iWin32Console.COORD dwMaximumWindowSize;
  }

  [StructLayoutAttribute(LayoutKind.Sequential)]
  public struct CHAR_INFO
  {
    public char UnicodeChar;
    public CHAR_ATTRIBS Attributes;
  };

  [Flags]
  public enum CHAR_ATTRIBS : ushort
  {
    FOREGROUND_BLUE = 0x0001,   //Text color contains blue.
    FOREGROUND_GREEN = 0x0002,  //Text color contains green.
    FOREGROUND_RED = 0x0004,    //Text color contains red.
    FOREGROUND_INTENSITY = 0x0008,  //Text color is intensified.
    BACKGROUND_BLUE = 0x0010,   //Background color contains blue.
    BACKGROUND_GREEN = 0x0020,  //Background color contains green.
    BACKGROUND_RED = 0x0040,    //Background color contains red.
    BACKGROUND_INTENSITY = 0x0080,  //Background color is intensified.
    COMMON_LVB_LEADING_BYTE = 0x0100,   //Leading byte.
    COMMON_LVB_TRAILING_BYTE = 0x0200,  //Trailing byte.
    COMMON_LVB_GRID_HORIZONTAL = 0x0400,    //Top horizontal
    COMMON_LVB_GRID_LVERTICAL = 0x0800, //Left vertical.
    COMMON_LVB_GRID_RVERTICAL = 0x1000, //Right vertical.
    COMMON_LVB_REVERSE_VIDEO = 0x4000,  //Reverse foreground and background attribute.
    COMMON_LVB_UNDERSCORE = 0x8000, //Underscore.
  }

  class iConsoleThreadScreen : iConsoleThread
  {
    private iConsoleWindowReader _windowReader;

    public iConsoleThreadScreen(iConsoleHandle handle, string parentPipe)
      : base()
    {
      _windowReader = new iConsoleWindowReader();

      Start(this, handle, parentPipe);
    }

    protected override void ThreadProcedure()
    {
      TheThread.Name = "iConsoleThreadScreen";

      TheThread.Priority = ThreadPriority.Highest;

      ExtractBufferEx();
    }

    private void ExtractBufferEx()
    {
      Pipe.Disconnect();
      Pipe.PipeReadBufferSizeKB = 0;
      Pipe.PipeWritBufferSizeKB = 256;
      Pipe.Connect(PipeName, iPipe.PipeType.Client);

      bool sendSuccesful = true;

      int bytesToWrite = 0;

      while (!ExitThread)
      {
        if (sendSuccesful)
        {// last send was successful
          bytesToWrite = 0;

          _windowReader.Update();

          int numUpdates = _windowReader.UpdateDeltas();

          if (numUpdates > 0)
          {
            bytesToWrite = _windowReader.Encode(Pipe.PipeWriteBuffer);

            if (bytesToWrite > 0)
            {
              sendSuccesful = Pipe.SendData(bytesToWrite, iPipe.PipeIO.Asynchronous);
            }
          }
        }
        else
        {// last send failed try a resend
          sendSuccesful = Pipe.SendData(bytesToWrite, iPipe.PipeIO.Asynchronous);
        }

        UpdateCount++;

        Thread.Sleep(RefreshTimeMS);
      }

      Pipe.Flush();
      Pipe.Disconnect();

      _windowReader.Dispose();
    }
  }
}