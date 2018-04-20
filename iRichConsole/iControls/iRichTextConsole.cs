using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using iCppLib;

namespace iControls
{
    public partial class iRichTextConsole : iRichTextBase
    {
        const string RtfHeader = @"\rtf1\ansi\ansicpg1252\deff0\deflang3081";

        const string RtfColourForeRed = @"\cf1 ";
        public void Print(int a_offset, string a_data, Color a_foreColour, Color a_backColor)
        {
        }

        iCONSOLE_SCREEN_BUFFER_INFO _screenBuffer = new iCONSOLE_SCREEN_BUFFER_INFO();

        public void GetSize(iConsoleWindowReader reader)
        {
            _screenBuffer = reader.iScreenBufferInfo;
        }

		public void SetConsoleSize(int a_rows, int a_cols)
        {
            int totalCells = a_rows * a_cols;

            Point p0 = GetPositionFromCharIndex(0);
            Point p1 = GetPositionFromCharIndex(a_cols - 1);

            int cellWidth = GetPositionFromCharIndex(1).X - GetPositionFromCharIndex(0).X;

            int cellHeight = GetPositionFromCharIndex(GetFirstCharIndexFromLine(1)).Y - GetPositionFromCharIndex(0).Y;

            //if ((Size.Width != cell_width * cols) || (Size.Height != cell_height * rows))
                //Size = new Size(cell_width * cols, cell_height * rows);
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct CharInfoEx
    {
        public int Offset;
        public char UnicodeChar;
        public CharAttribs Attributes;
    };

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct ConsoleScreenBufferInfo
    {
        public Coord dwSize;
        public Coord dwCursorPosition;
        public ushort wAttributes;
        public SmallRect srWindow;
        public Coord dwMaximumWindowSize;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct CharInfo
    {
        public char UnicodeChar;
        public CharAttribs Attributes;
    };

    [Flags]
    public enum CharAttribs : ushort
    {
        ForegroundBlue = 0x0001,   //Text color contains blue.
        ForegroundGreen = 0x0002,  //Text color contains green.
        ForegroundRed = 0x0004,    //Text color contains red.
        ForegroundIntensity = 0x0008,  //Text color is intensified.
        BackgroundBlue = 0x0010,   //Background color contains blue.
        BackgroundGreen = 0x0020,  //Background color contains green.
        BackgroundRed = 0x0040,    //Background color contains red.
        BackgroundIntensity = 0x0080,  //Background color is intensified.
        CommonLvbLeadingByte = 0x0100,   //Leading byte.
        CommonLvbTrailingByte = 0x0200,  //Trailing byte.
        CommonLvbGridHorizontal = 0x0400,    //Top horizontal
        CommonLvbGridLvertical = 0x0800, //Left vertical.
        CommonLvbGridRvertical = 0x1000, //Right vertical.
        CommonLvbReverseVideo = 0x4000,  //Reverse foreground and background attribute.
        CommonLvbUnderscore = 0x8000, //Underscore.
    }


}
