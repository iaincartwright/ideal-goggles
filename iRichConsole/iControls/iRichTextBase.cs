using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace iControls
{
    public partial class iRichTextBase : RichTextBox, IDisposable
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef a_hWnd, int a_msg, int a_wParam, int a_lParam);

        // windows message constants
        protected const int WmSetredraw = 11;
        protected const int EmLineindex = 187;
        protected const int WmKeydown = 256;
        protected const int WmKeyup = 257;
        protected const int WmChar = 258;
        protected const int EmSeteventmask = 1073;

        private int _updating = 0;
        private int _oldEventMask = 0;

        private readonly List<string> _monospaceFontNames;

        iRichTextBoxHooks _hooks;

        protected iRichTextBase()
        {
            InitializeComponent();

            _monospaceFontNames = new List<string>();

            foreach (var ff in FontFamily.Families)
            {
                try
                {
                    using (var temp = new Font(ff, 12))
                    {
                        var eyes = TextRenderer.MeasureText("i", temp);
                        var ems = TextRenderer.MeasureText("M", temp);

                        if (ems.Width != eyes.Width)
                            continue;

                        _monospaceFontNames.Add(ff.Name);
                    }
                }
                catch { }
            }

            if (_monospaceFontNames.Contains("Consolas"))
                base.Font = new Font("Consolas", 12);
            else
				base.Font = new Font(FontFamily.GenericMonospace, 12);

            _hooks = new iRichTextBoxHooks(this);
        }

        public IEnumerable<string> MonospaceFontNames => _monospaceFontNames;

        public override Font Font
        {
            get { return base.Font; }
            set
            {
                if ((_monospaceFontNames.Contains(value.FontFamily.Name)))
                    base.Font = value;
            }
        }

        public void DrawRuler()
        {
            var tens = "0         1         2         3         4         5         6         7         8         9        10        11";
            var ones = "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";

            SelectionStart = 0;
            SelectionColor = Color.DarkGreen;
            SelectedText = tens + "\n" + ones;
        }

        public void BeginUpdate()
        {
            // manage nested calls
            _updating++;

            if (_updating > 1)
                return;

            // Prevent the control from raising any events.
            _oldEventMask = SendMessage(new HandleRef(this, Handle), EmSeteventmask, 0, 0);

            // Prevent the control from redrawing itself.
            SendMessage(new HandleRef(this, Handle), WmSetredraw, 0, 0);
        }

        public void EndUpdate()
        {
            // dont do anything until 
            // we have un nested the updates
            _updating--;

            if (_updating > 0)
                return;

            // Allow the control to redraw itself.
            SendMessage(new HandleRef(this, Handle), WmSetredraw, 1, 0);

            // Allow the control to raise event messages.
            SendMessage(new HandleRef(this, Handle), EmSeteventmask, 0, _oldEventMask);

            // now do the update
            Invalidate(true);
        }

        public int CaretLine
        {
            get { return GetLineFromCharIndex(SelectionStart); }
            set
            {
                var charIndex = GetFirstCharIndexFromLine(value);

                if (charIndex <= 0)
                    charIndex = Text.Length;

                SelectionStart = charIndex;
            }
        }

        public int CaretCol
        {
            get { return SelectionStart - GetFirstCharIndexFromLine(CaretLine); }
            set
            {
                var colNumber = value;

                if (colNumber >= CaretLineLength)
                    colNumber = Math.Max(CaretLineLength, 0);

                SelectionStart = GetFirstCharIndexFromLine(CaretLine) + colNumber;
            }
        }

        public int CaretLineLength
        {
            get
            {
                var thisLineStart = GetFirstCharIndexFromLine(CaretLine);
                var nextLineStart = GetFirstCharIndexFromLine(CaretLine + 1);

                if (nextLineStart <= 0)
                    nextLineStart = Text.Length;

                return nextLineStart - thisLineStart;
            }
        }

        public void CaretHome()
        {
            SelectionStart = 0;
            SelectionLength = 0;
        }

        private void contextMenu_Opening(object a_sender, System.ComponentModel.CancelEventArgs a_e)
        {
            var dataObject = Clipboard.GetDataObject();

            _clearMI.Enabled = (TextLength > 0);
            _selectAllMI.Enabled = (TextLength > 0);
            _copyMI.Enabled = (SelectionLength > 0);
            _pasteMI.Enabled = !ReadOnly && dataObject != null && dataObject.GetDataPresent(DataFormats.Text);
            _cutMI.Enabled = !ReadOnly && (SelectionLength > 0);

            _wordWrap.Checked = WordWrap;
            _readOnlyMI.Checked = ReadOnly;
        }

        private void _clearMI_Click(object a_sender, EventArgs a_e)
        {
            Clear();
        }

        private void _copyMI_Click(object a_sender, EventArgs a_e)
        {
            Copy();
        }

        private void _selectAllMI_Click(object a_sender, EventArgs a_e)
        {
            SelectAll();
        }

        private void _pasteMI_Click(object a_sender, EventArgs a_e)
        {
            Paste();
        }

        private void _cutMI_Click(object a_sender, EventArgs a_e)
        {
            Cut();
        }

        void _wordWrap_Click(object a_sender, EventArgs a_e)
        {
            WordWrap = _wordWrap.Checked;
        }

        private void _readOnlyMI_Click(object a_sender, EventArgs a_e)
        {
            ReadOnly = _readOnlyMI.Checked;
        }

        private void _backGroundColour_Click(object a_sender, EventArgs a_e)
        {
            if (_colorDialog.ShowDialog() == DialogResult.OK)
                SelectionBackColor = _colorDialog.Color;
        }

        private void iRichTextBox_LinkClicked(object a_sender, LinkClickedEventArgs a_e)
        {
            Process.Start(a_e.LinkText);
        }

        private void _saveAs_Click(object a_sender, EventArgs a_e)
        {
            var sfd = new SaveFileDialog
            {
                CheckFileExists = false,
                CreatePrompt = true,
                OverwritePrompt = true,
                DefaultExt = "txt",
                Filter = @"Text (*.txt)|*.txt|Rich Text Format(*.rtf)|*.rtf|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveFile(sfd.FileName,
                    Path.GetExtension(sfd.FileName)?.ToLower() == ".rtf"
                        ? RichTextBoxStreamType.RichText
                        : RichTextBoxStreamType.PlainText);
            }
        }

        private void _findMI_Click(object a_sender, EventArgs a_e)
        {
            var teb = new iTextEntryBox();

            if (teb.ShowDialog() == DialogResult.OK)
            {
                var findType = teb.MatchCase ? RichTextBoxFinds.MatchCase : RichTextBoxFinds.None;
                findType |= teb.WholeWord ? RichTextBoxFinds.WholeWord : RichTextBoxFinds.None;
                findType |= teb.SearchUp ? RichTextBoxFinds.Reverse : RichTextBoxFinds.None;

                Find(teb.TextEntry, findType);
            }
        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct Scrollbarinfo
        {
            public uint cbSize;
            public Rect rcScrollBar;
            public int dxyLineButton;
            public int xyThumbTop;
            public int xyThumbBottom;
            public int reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.U4)]
            public uint[] rgstate;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", EntryPoint = "GetScrollBarInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollBarInfo([In] IntPtr a_hwnd, int a_idObject, ref Scrollbarinfo a_psbi);
    }
}
