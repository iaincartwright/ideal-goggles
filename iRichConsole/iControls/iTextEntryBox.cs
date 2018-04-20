using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace iControls
{
    public partial class iTextEntryBox : Form
    {
        public iTextEntryBox()
        {
            InitializeComponent();
        }

        public string TextEntry
        {
            get { return _textB.Text; }
        }

        public bool MatchCase
        {
            get { return _caseCB.Checked; }
        }

        public bool WholeWord
        {
            get { return _wholeWordCB.Checked; }
        }

        public bool SearchUp
        {
            get { return _reverseCB.Checked; }
        }
    }
}