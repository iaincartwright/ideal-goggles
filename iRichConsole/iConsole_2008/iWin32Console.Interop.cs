using System;
using System.Runtime.InteropServices;
using System.Text;

namespace iConsole
{
  public partial class iWin32Console
  {
#pragma warning disable 0649  //  "blah-blah is never assigned to, and will always have its default value"

    static public readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    const Int32 STD_INPUT_HANDLE = -10;
    const Int32 STD_OUTPUT_HANDLE = -11;
    const Int32 STD_ERROR_HANDLE = -12;

    // attributes
    public const UInt16 FOREGROUND_BLUE = 0x0001;
    public const UInt16 FOREGROUND_GREEN = 0x0002;
    public const UInt16 FOREGROUND_RED = 0x0004;
    public const UInt16 FOREGROUND_INTENSITY = 0x0008;
    public const UInt16 BACKGROUND_BLUE = 0x0010;
    public const UInt16 BACKGROUND_GREEN = 0x0020;
    public const UInt16 BACKGROUND_RED = 0x0040;
    public const UInt16 BACKGROUND_INTENSITY = 0x0080;

    // access modes
    const uint GENERIC_READ = 0x80000000;
    const uint GENERIC_WRITE = 0x40000000;
    const uint GENERIC_ALL = 0x10000000;
    const uint GENERIC_EXECUTE = 0x20000000;

    // share modes
    const uint FILE_SHARE_DELETE = 4;
    const uint FILE_SHARE_WRITE = 2;

    // disposition
    const int OPEN_EXISTING = 3;

    public const int FILE_ATTRIBUTE_NORMAL = 0x80;

    const uint FILE_SHARE_READ = 1;

    // flags
    const uint CONSOLE_TEXTMODE_BUFFER = 1;

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
      public uint nLength;

      public IntPtr lpSecurityDescriptor;

      [MarshalAsAttribute(UnmanagedType.Bool)]
      public bool bInheritHandle;
    }

    [DllImportAttribute("winmm.dll", EntryPoint = "timeBeginPeriod")]
    public static extern uint timeBeginPeriod(uint uPeriod);

    [DllImport("kernel32", EntryPoint = "WaitForSingleObject", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint WaitForSingleObject(uint hHandle, uint dwMilliseconds);

    [DllImport("kernel32", EntryPoint = "WaitForSingleObject", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

    [DllImport("kernel32", EntryPoint = "ResetEvent", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool ResetEvent(IntPtr hHandle);

    [DllImportAttribute("kernel32.dll", EntryPoint = "CreateFileA", SetLastError = true)]
    public static extern System.IntPtr CreateFileA([In] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpFileName,
                        uint dwDesiredAccess,
                        uint dwShareMode,
                        [In] System.IntPtr lpSecurityAttributes,
                        uint dwCreationDisposition,
                        uint dwFlagsAndAttributes,
                        [In] System.IntPtr hTemplateFile);

    [DllImportAttribute("kernel32.dll", EntryPoint = "CreateFileA", SetLastError = true)]
    public static extern System.IntPtr CreateFileA([In] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpFileName,
                        uint dwDesiredAccess,
                        uint dwShareMode,
                        ref SECURITY_ATTRIBUTES lpSecurityAttributes,
                        uint dwCreationDisposition,
                        uint dwFlagsAndAttributes,
                        [In] System.IntPtr hTemplateFile);

    //EntryPoint = "CreateFileW",
    [DllImportAttribute("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true)]
    public static extern IntPtr CreateFile([In] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpFileName,
                        uint dwDesiredAccess,
                        uint dwShareMode,
                        [In] IntPtr lpSecurityAttributes,
                        uint dwCreationDisposition,
                        uint dwFlagsAndAttributes,
                        [In] IntPtr hTemplateFile);

    // http://pinvoke.net/default.aspx/kernel32/AddConsoleAlias.html
    [DllImport("kernel32", SetLastError = true)]
    static extern bool AddConsoleAlias(
        string Source,
        string Target,
        string ExeName
        );

    // http://pinvoke.net/default.aspx/kernel32/AllocConsole.html
    [DllImport("kernel32", SetLastError = true)]
    static extern bool AllocConsole();

    // http://pinvoke.net/default.aspx/kernel32/AttachConsole.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AttachConsole(
        uint dwProcessId
        );

    // http://pinvoke.net/default.aspx/kernel32/CreateConsoleScreenBuffer.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr CreateConsoleScreenBuffer(
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwFlags,
        IntPtr lpScreenBufferData
        );

    // http://pinvoke.net/default.aspx/kernel32/FillConsoleOutputAttribute.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FillConsoleOutputAttribute(
        IntPtr hConsoleOutput,
        ushort wAttribute,
        uint nLength,
        COORD dwWriteCoord,
        out uint lpNumberOfAttrsWritten
        );

    // http://pinvoke.net/default.aspx/kernel32/FillConsoleOutputCharacter.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FillConsoleOutputCharacter(
        IntPtr hConsoleOutput,
        char cCharacter,
        uint nLength,
        COORD dwWriteCoord,
        out uint lpNumberOfCharsWritten
        );

    // http://pinvoke.net/default.aspx/kernel32/FlushConsoleInputBuffer.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FlushConsoleInputBuffer(
        IntPtr hConsoleInput
        );

    // http://pinvoke.net/default.aspx/kernel32/FreeConsole.html
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern bool FreeConsole();

    // http://pinvoke.net/default.aspx/kernel32/GenerateConsoleCtrlEvent.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GenerateConsoleCtrlEvent(
        uint dwCtrlEvent,
        uint dwProcessGroupId
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAlias.html
    [DllImport("kernel32", SetLastError = true)]
    static extern bool GetConsoleAlias(
        string Source,
        out StringBuilder TargetBuffer,
        uint TargetBufferLength,
        string ExeName
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliases.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint GetConsoleAliases(
        StringBuilder[] lpTargetBuffer,
        uint targetBufferLength,
        string lpExeName
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliasesLength.html
    [DllImport("kernel32", SetLastError = true)]
    static extern uint GetConsoleAliasesLength(
        string ExeName
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliasExes.html
    [DllImport("kernel32", SetLastError = true)]
    static extern uint GetConsoleAliasExes(
        out StringBuilder ExeNameBuffer,
        uint ExeNameBufferLength
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliasExesLength.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint GetConsoleAliasExesLength();

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleCP.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint GetConsoleCP();

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleCursorInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleCursorInfo(
        IntPtr hConsoleOutput,
        out CONSOLE_CURSOR_INFO lpConsoleCursorInfo
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleDisplayMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleDisplayMode(
        out uint ModeFlags
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleFontSize.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern COORD GetConsoleFontSize(
        IntPtr hConsoleOutput,
        Int32 nFont
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleHistoryInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleHistoryInfo(
        out CONSOLE_HISTORY_INFO ConsoleHistoryInfo
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleMode(
        IntPtr hConsoleHandle,
        out uint lpMode
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleOriginalTitle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint GetConsoleOriginalTitle(
        out StringBuilder ConsoleTitle,
        uint Size
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleOutputCP.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint GetConsoleOutputCP();

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleProcessList.html
    //    uint[] procs = new uint[64];
    //    GetConsoleProcessList(ref procs[0], (uint)procs.Length);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint GetConsoleProcessList(uint[] ProcessList, uint ProcessCount);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleScreenBufferInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleScreenBufferInfo(
        IntPtr hConsoleOutput,
        ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleScreenBufferInfoEx.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleScreenBufferInfoEx(
        IntPtr hConsoleOutput,
        ref CONSOLE_SCREEN_BUFFER_INFO_EX ConsoleScreenBufferInfo
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleSelectionInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleSelectionInfo(
        CONSOLE_SELECTION_INFO ConsoleSelectionInfo
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleTitle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint GetConsoleTitle(
        [Out] StringBuilder lpConsoleTitle,
        uint nSize
        );

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleWindow.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetConsoleWindow();

    // http://pinvoke.net/default.aspx/kernel32/GetCurrentConsoleFont.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetCurrentConsoleFont(
        IntPtr hConsoleOutput,
        bool bMaximumWindow,
        out CONSOLE_FONT_INFO lpConsoleCurrentFont
        );

    // http://pinvoke.net/default.aspx/kernel32/GetCurrentConsoleFontEx.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetCurrentConsoleFontEx(
        IntPtr ConsoleOutput,
        bool MaximumWindow,
        out CONSOLE_FONT_INFO_EX ConsoleCurrentFont
        );

    // http://pinvoke.net/default.aspx/kernel32/GetLargestConsoleWindowSize.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern COORD GetLargestConsoleWindowSize(
        IntPtr hConsoleOutput
        );

    // http://pinvoke.net/default.aspx/kernel32/GetNumberOfConsoleInputEvents.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetNumberOfConsoleInputEvents(
        IntPtr hConsoleInput,
        out uint lpcNumberOfEvents
        );

    // http://pinvoke.net/default.aspx/kernel32/GetNumberOfConsoleMouseButtons.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetNumberOfConsoleMouseButtons(
        ref uint lpNumberOfMouseButtons
        );

    // http://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(
        int nStdHandle
        );

    // http://pinvoke.net/default.aspx/kernel32/HandlerRoutine.html
    // Delegate type to be used as the Handler Routine for SCCH
    delegate bool ConsoleCtrlDelegate(CtrlTypes CtrlType);

    // http://pinvoke.net/default.aspx/kernel32/PeekConsoleInput.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool PeekConsoleInput(
        IntPtr hConsoleInput,
        ref INPUT_RECORD lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead
        );

    // http://pinvoke.net/default.aspx/kernel32/ReadConsole.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ReadConsole(
        IntPtr hConsoleInput,
        [Out] StringBuilder lpBuffer,
        uint nNumberOfCharsToRead,
        out uint lpNumberOfCharsRead,
        IntPtr lpReserved
        );

    // http://pinvoke.net/default.aspx/kernel32/ReadConsoleInput.html
    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
    public static extern bool ReadConsoleInput(
        IntPtr hConsoleInput,
        ref INPUT_RECORD lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead
        );

    [DllImportAttribute("kernel32.dll", SetLastError = true, EntryPoint = "ReadConsoleOutputW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadConsoleOutputW(
            IntPtr hConsoleOutput,
            ref CHAR_INFO lpBuffer,
            COORD dwBufferSize,
            COORD dwBufferCoord,
            ref SMALL_RECT lpReadRegion);

    [DllImportAttribute("kernel32.dll", SetLastError = true, EntryPoint = "ReadConsoleOutputW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadConsoleOutput(
            IntPtr hConsoleOutput,
            IntPtr pBuffer,
            COORD dwBufferSize,
            COORD dwBufferCoord,
            ref SMALL_RECT lpReadRegion);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadConsoleOutputAttribute(
        IntPtr hConsoleOutput,
        ushort[] lpAttribute,
        uint nLength,
        COORD dwReadCoord,
        ref uint lpNumberOfAttrsRead
        );

    // http://pinvoke.net/default.aspx/kernel32/ReadConsoleOutputCharacter.html
    //[DllImport("kernel32.dll", SetLastError = true)]
    //public static extern bool ReadConsoleOutputCharacter(
    //    IntPtr hConsoleOutput,
    //    [Out] StringBuilder lpCharacter,
    //    uint nLength,
    //    COORD dwReadCoord,
    //    out uint lpNumberOfCharsRead
    //    );

    [DllImportAttribute("kernel32.dll", SetLastError = true, EntryPoint = "ReadConsoleOutputCharacterW")]
    [return: MarshalAsAttribute(UnmanagedType.Bool)]
    public static extern bool ReadConsoleOutputCharacter(
        IntPtr hConsoleOutput,
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpCharacter,
        uint nLength,
        COORD dwReadCoord,
        ref uint lpNumberOfCharsRead
        );

    [DllImportAttribute("kernel32.dll", SetLastError = true, EntryPoint = "ReadConsoleOutputCharacterW")]
    [return: MarshalAsAttribute(UnmanagedType.Bool)]
    public static extern bool ReadConsoleOutputCharacter(
        IntPtr hConsoleOutput,
        [In] ref char lpCharacter,
        uint nLength,
        COORD dwReadCoord,
        ref uint lpNumberOfCharsRead
        );

    // http://pinvoke.net/default.aspx/kernel32/ScrollConsoleScreenBuffer.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ScrollConsoleScreenBuffer(
        IntPtr hConsoleOutput,
       [In] ref SMALL_RECT lpScrollRectangle,
        IntPtr lpClipRectangle,
       COORD dwDestinationOrigin,
        [In] ref CHAR_INFO lpFill
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleActiveScreenBuffer.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleActiveScreenBuffer(
        IntPtr hConsoleOutput
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleCP.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleCP(
        uint wCodePageID
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleCtrlHandler.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleCtrlHandler(
        ConsoleCtrlDelegate HandlerRoutine,
        bool Add
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleCursorInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleCursorInfo(
        IntPtr hConsoleOutput,
        [In] ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleCursorPosition.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleCursorPosition(
        IntPtr hConsoleOutput,
       COORD dwCursorPosition
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleDisplayMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleDisplayMode(
        IntPtr ConsoleOutput,
        uint Flags,
        out COORD NewScreenBufferDimensions
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleHistoryInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleHistoryInfo(
        CONSOLE_HISTORY_INFO ConsoleHistoryInfo
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleMode(
        IntPtr hConsoleHandle,
        uint dwMode
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleOutputCP.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleOutputCP(
        uint wCodePageID
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleScreenBufferInfoEx.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleScreenBufferInfoEx(
        IntPtr ConsoleOutput,
        CONSOLE_SCREEN_BUFFER_INFO_EX ConsoleScreenBufferInfoEx
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleScreenBufferSize.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleScreenBufferSize(
        IntPtr hConsoleOutput,
        COORD dwSize
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleTextAttribute.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleTextAttribute(
        IntPtr hConsoleOutput,
       ushort wAttributes
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleTitle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleTitle(
        string lpConsoleTitle
        );

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleWindowInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleWindowInfo(
        IntPtr hConsoleOutput,
        bool bAbsolute,
        [In] ref SMALL_RECT lpConsoleWindow
        );

    // http://pinvoke.net/default.aspx/kernel32/SetCurrentConsoleFontEx.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetCurrentConsoleFontEx(
        IntPtr ConsoleOutput,
        bool MaximumWindow,
        CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx
        );

    // http://pinvoke.net/default.aspx/kernel32/SetStdHandle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetStdHandle(
        uint nStdHandle,
        IntPtr hHandle
        );

    // http://pinvoke.net/default.aspx/kernel32/WriteConsole.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteConsole(
        IntPtr hConsoleOutput,
        string lpBuffer,
        uint nNumberOfCharsToWrite,
        out uint lpNumberOfCharsWritten,
        IntPtr lpReserved
        );

    // http://pinvoke.net/default.aspx/kernel32/WriteConsoleInput.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static public extern bool WriteConsoleInput(
        IntPtr hConsoleInput,
        INPUT_RECORD[] lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsWritten
        );

    // http://pinvoke.net/default.aspx/kernel32/WriteConsoleOutput.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteConsoleOutput(
        IntPtr hConsoleOutput,
        CHAR_INFO[] lpBuffer,
        COORD dwBufferSize,
        COORD dwBufferCoord,
        ref SMALL_RECT lpWriteRegion
        );

    // http://pinvoke.net/default.aspx/kernel32/WriteConsoleOutputAttribute.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteConsoleOutputAttribute(
        IntPtr hConsoleOutput,
        ushort[] lpAttribute,
        uint nLength,
        COORD dwWriteCoord,
        out uint lpNumberOfAttrsWritten
        );

    // http://pinvoke.net/default.aspx/kernel32/WriteConsoleOutputCharacter.html
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteConsoleOutputCharacter(
        IntPtr hConsoleOutput,
        string lpCharacter,
        uint nLength,
        COORD dwWriteCoord,
        out uint lpNumberOfCharsWritten
        );

    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
      public short X;
      public short Y;
    }

    public struct SMALL_RECT
    {
      public short Left;
      public short Top;
      public short Right;
      public short Bottom;
    }

    public struct CONSOLE_SCREEN_BUFFER_INFO
    {
      public COORD dwSize;
      public COORD dwCursorPosition;
      public short wAttributes;
      public SMALL_RECT srWindow;
      public COORD dwMaximumWindowSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CONSOLE_SCREEN_BUFFER_INFO_EX
    {
      public uint cbSize;
      public COORD dwSize;
      public COORD dwCursorPosition;
      public short wAttributes;
      public SMALL_RECT srWindow;
      public COORD dwMaximumWindowSize;

      public ushort wPopupAttributes;
      public bool bFullscreenSupported;

      // Hack Hack Hack
      // Too lazy to figure out the array at the moment...
      //public COLORREF[16] ColorTable;
      public COLORREF color0;
      public COLORREF color1;
      public COLORREF color2;
      public COLORREF color3;

      public COLORREF color4;
      public COLORREF color5;
      public COLORREF color6;
      public COLORREF color7;

      public COLORREF color8;
      public COLORREF color9;
      public COLORREF colorA;
      public COLORREF colorB;

      public COLORREF colorC;
      public COLORREF colorD;
      public COLORREF colorE;
      public COLORREF colorF;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COLORREF
    {
      public uint ColorDWORD;

      public COLORREF(System.Drawing.Color color)
      {
        ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
      }

      public System.Drawing.Color GetColor()
      {
        return System.Drawing.Color.FromArgb((int)(0x000000FFU & ColorDWORD),
           (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
      }

      public void SetColor(System.Drawing.Color color)
      {
        ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CONSOLE_FONT_INFO
    {
      public int nFont;
      public COORD dwFontSize;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CONSOLE_FONT_INFO_EX
    {
      public uint cbSize;
      public uint nFont;
      public COORD dwFontSize;
      public ushort FontFamily;
      public ushort FontWeight;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
      string FaceName;

      const int LF_FACESIZE = 32;
    }

    /// Return Type: DWORD->unsigned int
    ///lpdwProcessList: LPDWORD->DWORD*
    ///dwProcessCount: DWORD->unsigned int
    [DllImportAttribute("kernel32.dll", EntryPoint = "GetConsoleProcessList")]
    public static extern uint GetConsoleProcessList(ref uint lpdwProcessList, uint dwProcessCount);

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT_RECORD
    {
      [FieldOffset(0)]
      public ushort EventType;
      [FieldOffset(4)]
      public KEY_EVENT_RECORD KeyEvent;
      [FieldOffset(4)]
      public MOUSE_EVENT_RECORD MouseEvent;
      [FieldOffset(4)]
      public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
      [FieldOffset(4)]
      public MENU_EVENT_RECORD MenuEvent;
      [FieldOffset(4)]
      public FOCUS_EVENT_RECORD FocusEvent;
    };

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct KEY_EVENT_RECORD
    {
      [FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
      public bool bKeyDown;
      [FieldOffset(4), MarshalAs(UnmanagedType.U2)]
      public ushort wRepeatCount;
      [FieldOffset(6), MarshalAs(UnmanagedType.U2)]
      //public VirtualKeys wVirtualKeyCode;
      public ushort wVirtualKeyCode;
      [FieldOffset(8), MarshalAs(UnmanagedType.U2)]
      public ushort wVirtualScanCode;
      [FieldOffset(10)]
      public char UnicodeChar;
      [FieldOffset(12), MarshalAs(UnmanagedType.U4)]
      //public ControlKeyState dwControlKeyState;
      public uint dwControlKeyState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSE_EVENT_RECORD
    {
      public COORD dwMousePosition;
      public uint dwButtonState;
      public uint dwControlKeyState;
      public uint dwEventFlags;
    }

    public struct WINDOW_BUFFER_SIZE_RECORD
    {
      public COORD dwSize;

      public WINDOW_BUFFER_SIZE_RECORD(short x, short y)
      {
        dwSize = new COORD();
        dwSize.X = x;
        dwSize.Y = y;
      }
    }

    /// <summary>
    /// dont use this
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MENU_EVENT_RECORD
    {
      public uint dwCommandId;
    }

    /// <summary>
    /// dont use this
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FOCUS_EVENT_RECORD
    {
      public uint bSetFocus;
    }

    //CHAR_INFO struct, which was a union in the old days
    // so we want to use LayoutKind.Explicit to mimic it as closely
    // as we can
    [StructLayout(LayoutKind.Explicit)]
    public struct CHAR_INFO
    {
      public CHAR_INFO(char theChar, UInt16 attribs)
      {
        AsciiChar = UnicodeChar = theChar;
        Attributes = attribs;
      }

      [FieldOffset(0)]
      public char UnicodeChar;
      [FieldOffset(0)]
      public char AsciiChar;
      [FieldOffset(2)] //2 bytes seems to work properly
      public UInt16 Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CONSOLE_CURSOR_INFO
    {
      uint Size;
      bool Visible;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CONSOLE_HISTORY_INFO
    {
      ushort cbSize;
      ushort HistoryBufferSize;
      ushort NumberOfHistoryBuffers;
      uint dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CONSOLE_SELECTION_INFO
    {
      uint Flags;
      COORD SelectionAnchor;
      SMALL_RECT Selection;

      // Flags values:
      const uint CONSOLE_MOUSE_DOWN = 0x0008; // Mouse is down
      const uint CONSOLE_MOUSE_SELECTION = 0x0004; //Selecting with the mouse
      const uint CONSOLE_NO_SELECTION = 0x0000; //No selection
      const uint CONSOLE_SELECTION_IN_PROGRESS = 0x0001; //Selection has begun
      const uint CONSOLE_SELECTION_NOT_EMPTY = 0x0002; //Selection rectangle is not empty
    }

    // Enumerated type for the control messages sent to the handler routine
    enum CtrlTypes : uint
    {
      CTRL_C_EVENT = 0,
      CTRL_BREAK_EVENT,
      CTRL_CLOSE_EVENT,
      CTRL_LOGOFF_EVENT = 5,
      CTRL_SHUTDOWN_EVENT
    }
  }
}