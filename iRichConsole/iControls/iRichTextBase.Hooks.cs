using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;

namespace iControls
{
    internal class iRichTextBoxHooks
    {
        delegate int HookProc(int a_nCode, int a_wParam, IntPtr a_lParam);

        static HookProc s_mouseHookProcedure;
        static HookProc s_keyboardHookProcedure;

        static int s_hMouseHook = 0;
        static int s_hKeyboardHook = 0;

        static RichTextBox s_hookTarget;

        internal iRichTextBoxHooks(RichTextBox a_hookTarget)
        {
            s_hookTarget = a_hookTarget;
        }

        ~iRichTextBoxHooks()
        {
            Unhook();
        }

        internal static void Hook()
        {
            HookMouse();
            HookKeyboard();
        }

        internal static void Unhook()
        {
            UnHookKeyboard();
            UnHookMouse();
        }

        internal static void UnHookKeyboard()
        {
            if (s_hKeyboardHook != 0)
            {
                bool ret = UnhookWindowsHookEx(s_hKeyboardHook);

                if (ret == false)
                {
                    int error = Marshal.GetLastWin32Error();

                    if (error != 2) // cannot find file - means already canned
                    {
                        throw new Win32Exception(error);
                    }
                }

                s_hKeyboardHook = 0;
            }
        }

        internal static void UnHookMouse()
        {
            if (s_hMouseHook != 0)
            {
                bool ret = UnhookWindowsHookEx(s_hMouseHook);

                if (ret == false)
                {
                    int error = Marshal.GetLastWin32Error();

                    if (error != 2) // cannot find file - means already canned
                    {
                        throw new Win32Exception(error);
                    }
                }

                s_hMouseHook = 0;
            }
        }

        internal static void HookKeyboard()
        {
            if (s_hKeyboardHook == 0)
            {
                s_keyboardHookProcedure = new HookProc(iRichTextBoxHooks.KeyboardHookProc);

                Module[] modules = Assembly.GetExecutingAssembly().GetModules();

                s_hKeyboardHook = SetWindowsHookEx(WhKeyboardLl, s_keyboardHookProcedure, Marshal.GetHINSTANCE(modules[0]), 0);

                if (s_hKeyboardHook == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        internal static void HookMouse()
        {
            if (s_hMouseHook == 0)
            {
                s_mouseHookProcedure = new HookProc(iRichTextBoxHooks.MouseHookProc);

                Module[] modules = Assembly.GetExecutingAssembly().GetModules();

                s_hMouseHook = SetWindowsHookEx(WhMouseLl, s_mouseHookProcedure, Marshal.GetHINSTANCE(modules[0]), 0);

                if (s_hMouseHook == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        internal static RichTextBox HookTarget
        {
            get { return s_hookTarget; }
            set { s_hookTarget = value; }
        }

        private static int MouseHookProc(int a_nCode, int a_wParam, IntPtr a_lParam)
        {
            if (a_nCode >= 0 && s_hookTarget.IsDisposed == false)
            {
                MouseHookStruct mouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(a_lParam, typeof(MouseHookStruct));

                switch (a_wParam)
                {
                    case WmMousemove:
                        break;
                    case WmLbuttondown:
                        break;
                    case WmLbuttonup:
                        break;
                    case WmLbuttondblclk:
                        break;
                    case WmRbuttondown:
                        break;
                    case WmRbuttonup:
                        break;
                    case WmRbuttondblclk:
                        break;
                    case WmMousewheel:
                        break;
                    case WmMousehwheel:
                        break;
                    default:
                        return CallNextHookEx(s_hMouseHook, a_nCode, a_wParam, a_lParam);
                }
            }

            return CallNextHookEx(s_hMouseHook, a_nCode, a_wParam, a_lParam);
        }

        private static int KeyboardHookProc(int a_nCode, int a_wParam, IntPtr a_lParam)
        {
            if (a_nCode >= 0 && s_hookTarget.IsDisposed == false)
            {
                KeyboardHookStruct keyHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(a_lParam, typeof(KeyboardHookStruct));

                switch (a_wParam)
                {
                    case WmKeydown:
                        break;
                    case WmKeyup:
                        break;
                    case WmSyskeydown:
                        break;
                    case WmSyskeyup:
                        break;
                    default:
                        return CallNextHookEx(s_hKeyboardHook, a_nCode, a_wParam, a_lParam);
                }
            }

            return CallNextHookEx(s_hKeyboardHook, a_nCode, a_wParam, a_lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int SetWindowsHookEx(int a_idHook, HookProc a_lpfn, IntPtr a_hInstance, int a_threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern bool UnhookWindowsHookEx(int a_idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int CallNextHookEx(int a_idHook, int a_nCode, int a_wParam, IntPtr a_lParam);

        [DllImport("user32", EntryPoint = "ToAscii")]
        static extern int ToAscii(int a_uVirtKey, int a_uScanCode, byte[] a_lpbKeyState, byte[] a_lpwTransKey, int a_fuState);

        [DllImport("user32", EntryPoint = "GetKeyboardState")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] a_keyState);

        const int WhMouseLl = 14;
        const int WhKeyboardLl = 13;

        const int WhMouse = 7;
        const int WhKeyboard = 2;

        const int WmMousemove = 0x200;
        const int WmLbuttondown = 0x201;
        const int WmRbuttondown = 0x204;
        const int WmMbuttondown = 0x207;
        const int WmLbuttonup = 0x202;
        const int WmRbuttonup = 0x205;
        const int WmMbuttonup = 0x208;
        const int WmLbuttondblclk = 0x203;
        const int WmRbuttondblclk = 0x206;
        const int WmMbuttondblclk = 0x209;
        const int WmMousewheel = 0x020A;
        const int WmMousehwheel = 0x020E;

        const int WmXbuttondown = 0x020B;
        const int WmXbuttonup = 0x020C;
        const int WmXbuttondblclk = 0x020D;

        const int WmKeydown = 0x100;
        const int WmKeyup = 0x101;
        const int WmSyskeydown = 0x104;
        const int WmSyskeyup = 0x105;

        const byte VkShift = 0x10;
        const byte VkCapital = 0x14;
        const byte VkNumlock = 0x90;

        const int LlkhfExtended = 1 << 0;
        const int LlkhfAltdown = 1 << 5;
        const int LlkhfUp = 1 << 7;

        [StructLayout(LayoutKind.Sequential)]
        class POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseHookStruct
        {
            public Point Point;
            public int MouseData;   // wheel delta or xbutton number
            public int Flags;       // unused
            public int Time;        // unused
            public int ExtraInfo;   // unused
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardHookStruct
        {
            public int VirtualKeyCode;
            public int ScanCode;
            public int Flags;       // LLKHF_*
            public int Time;        // unused
            public int ExtraInfo;   // unused
        }
    }
}