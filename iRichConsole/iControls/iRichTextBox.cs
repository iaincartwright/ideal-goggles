using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace iControls
{
    public partial class iRichTextBox : iRichTextBase
    {
        public delegate bool ProcessKeystrokeEventHandler(char a_typed);
        public delegate void WriteLineDelegate(string a_message, Color a_colour);

        public event ProcessKeystrokeEventHandler ProcessKeystroke;

        public iRichTextBox()
        {
            InitializeComponent();
        }

        public void WriteLine(string a_message)
        {
            WriteLine(a_message, Color.Black);
        }

        public void WriteLine(string a_message, Color a_colour)
        {
            Write(a_message + Environment.NewLine, a_colour);
        }

        public void Write(string a_message)
        {
            Write(a_message, Color.Black);
        }

        public void Write(string a_message, Color a_colour)
        {
            if (InvokeRequired)
            {
                Invoke(new WriteLineDelegate(WriteLine), a_message, a_colour);
            }
            else
            {
                BeginUpdate();
                var oldColour = SelectionColor;
                SelectionLength = 0;
                SelectionColor = a_colour;
                AppendText("<" + Thread.CurrentThread.ManagedThreadId + ">" + a_message);
                SelectionLength = 0;
                SelectionColor = oldColour;
                EndUpdate();
            }
        }

        public void WriteRtf(string a_message)
        {
            BeginUpdate();
            SelectionLength = 0;
            SelectedRtf = a_message;
            SelectionLength = 0;
            EndUpdate();
        }

        private void OnProcessKeystroke(char a_typed)
        {
            ProcessKeystroke?.Invoke(a_typed);
        }

        protected override void OnLostFocus(EventArgs a_e)
        {
            CtrlDown = false;

            base.OnLostFocus(a_e);
        }

        public bool CtrlDown { get; set; }

        protected override bool ProcessKeyMessage(ref Message a_m)
        {
            switch (a_m.Msg)
            {
                case WmKeydown:
                    if ((int)a_m.WParam == (int)Keys.ControlKey)
                        CtrlDown = true;
                    break;

                case WmKeyup:
                    if ((int)a_m.WParam == (int)Keys.ControlKey)
                        CtrlDown = false;
                    break;

                case WmChar:
                    OnProcessKeystroke((char)a_m.WParam);
                    break;
            }

            return base.ProcessKeyMessage(ref a_m);
        }
    }
}
