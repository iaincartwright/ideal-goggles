using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace iControls
{
    internal partial class iEditPopup : TextBox
    {
        public iEditPopup()
        {
            InitializeComponent();
        }

        public iEditPopup(IContainer a_container)
        {
            a_container.Add(this);

            InitializeComponent();
        }

        protected override void OnKeyDown(KeyEventArgs a_e)
        {
            if (a_e.KeyCode == Keys.Escape)
            {
                FinishEdit(false);
            }
            else if (a_e.KeyCode == Keys.Enter)
            {
                FinishEdit(true);
            }

            base.OnKeyDown(a_e);
        }

        private bool _isFocusEnabled = true;

        protected override void OnLostFocus(EventArgs a_e)
        {
            if (_isFocusEnabled && Visible)
            {
                FinishEdit(false);
            }

            base.OnLostFocus(a_e);
        }

        public override string Text
        {
            get { return base.Text.Trim(); }
            set { base.Text = value.Trim(); }
        }

        private void FinishEdit(bool a_updateCell)
        {
            try
            {
                _isFocusEnabled = false;

                if (Visible)
                {
                    Visible = false;

                    var sub = (ListViewItem.ListViewSubItem)Tag;

                    if (a_updateCell)
                    {
                        sub.Text = Text;
                    }

                    var lvi = (ListViewItem)sub.Tag;

                    if (lvi != null && lvi.ListView != null && lvi.SubItems[0].Text == "")
                    {
                        lvi.ListView.Items.Remove(lvi);
                    }
                }
            }
            finally
            {
                _isFocusEnabled = true;
            }
        }
    }
}
