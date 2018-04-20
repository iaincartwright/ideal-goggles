namespace iControls
{
    public partial class iRichTextBase 
    {
        private System.ComponentModel.IContainer components;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._clearMI = new System.Windows.Forms.ToolStripMenuItem();
            this._selectAllMI = new System.Windows.Forms.ToolStripMenuItem();
            this._copyMI = new System.Windows.Forms.ToolStripMenuItem();
            this._pasteMI = new System.Windows.Forms.ToolStripMenuItem();
            this._cutMI = new System.Windows.Forms.ToolStripMenuItem();
            this._readOnlyMI = new System.Windows.Forms.ToolStripMenuItem();
            this._wordWrap = new System.Windows.Forms.ToolStripMenuItem();
            this._backGroundColour = new System.Windows.Forms.ToolStripMenuItem();
            this._saveAs = new System.Windows.Forms.ToolStripMenuItem();
            this._findMI = new System.Windows.Forms.ToolStripMenuItem();
            this._colorDialog = new System.Windows.Forms.ColorDialog();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(224, 6);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(224, 6);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(224, 6);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._clearMI,
            this._selectAllMI,
            toolStripSeparator1,
            this._copyMI,
            this._pasteMI,
            this._cutMI,
            toolStripSeparator2,
            this._readOnlyMI,
            this._wordWrap,
            this._backGroundColour,
            toolStripSeparator3,
            this._saveAs,
            this._findMI});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(228, 242);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // _clearMI
            // 
            this._clearMI.Name = "_clearMI";
            this._clearMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this._clearMI.Size = new System.Drawing.Size(227, 22);
            this._clearMI.Text = "Clear";
            this._clearMI.Click += new System.EventHandler(this._clearMI_Click);
            // 
            // _selectAllMI
            // 
            this._selectAllMI.Name = "_selectAllMI";
            this._selectAllMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this._selectAllMI.Size = new System.Drawing.Size(227, 22);
            this._selectAllMI.Text = "Select All";
            this._selectAllMI.Click += new System.EventHandler(this._selectAllMI_Click);
            // 
            // _copyMI
            // 
            this._copyMI.Name = "_copyMI";
            this._copyMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this._copyMI.Size = new System.Drawing.Size(227, 22);
            this._copyMI.Text = "Copy";
            this._copyMI.Click += new System.EventHandler(this._copyMI_Click);
            // 
            // _pasteMI
            // 
            this._pasteMI.Name = "_pasteMI";
            this._pasteMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this._pasteMI.Size = new System.Drawing.Size(227, 22);
            this._pasteMI.Text = "Paste";
            this._pasteMI.Click += new System.EventHandler(this._pasteMI_Click);
            // 
            // _cutMI
            // 
            this._cutMI.Name = "_cutMI";
            this._cutMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this._cutMI.Size = new System.Drawing.Size(227, 22);
            this._cutMI.Text = "Cut";
            this._cutMI.Click += new System.EventHandler(this._cutMI_Click);
            // 
            // _readOnlyMI
            // 
            this._readOnlyMI.CheckOnClick = true;
            this._readOnlyMI.Name = "_readOnlyMI";
            this._readOnlyMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this._readOnlyMI.Size = new System.Drawing.Size(227, 22);
            this._readOnlyMI.Text = "Read Only";
            this._readOnlyMI.Click += new System.EventHandler(this._readOnlyMI_Click);
            // 
            // _wordWrap
            // 
            this._wordWrap.CheckOnClick = true;
            this._wordWrap.Name = "_wordWrap";
            this._wordWrap.Size = new System.Drawing.Size(227, 22);
            this._wordWrap.Text = "Word wrap";
            this._wordWrap.Click += new System.EventHandler(this._wordWrap_Click);
            // 
            // _backGroundColour
            // 
            this._backGroundColour.Name = "_backGroundColour";
            this._backGroundColour.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this._backGroundColour.Size = new System.Drawing.Size(227, 22);
            this._backGroundColour.Text = "Background Colour...";
            this._backGroundColour.Click += new System.EventHandler(this._backGroundColour_Click);
            // 
            // _saveAs
            // 
            this._saveAs.Name = "_saveAs";
            this._saveAs.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this._saveAs.Size = new System.Drawing.Size(227, 22);
            this._saveAs.Text = "Save As...";
            this._saveAs.Click += new System.EventHandler(this._saveAs_Click);
            // 
            // _findMI
            // 
            this._findMI.Name = "_findMI";
            this._findMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this._findMI.Size = new System.Drawing.Size(227, 22);
            this._findMI.Text = "Find...";
            this._findMI.Click += new System.EventHandler(this._findMI_Click);
            // 
            // iRichTextBase
            // 
            this.ContextMenuStrip = this.contextMenu;
            this.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.iRichTextBox_LinkClicked);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem _clearMI;
        private System.Windows.Forms.ToolStripMenuItem _copyMI;
        private System.Windows.Forms.ToolStripMenuItem _selectAllMI;
        private System.Windows.Forms.ToolStripMenuItem _pasteMI;
        private System.Windows.Forms.ToolStripMenuItem _cutMI;
        private System.Windows.Forms.ToolStripMenuItem _readOnlyMI;
        private System.Windows.Forms.ToolStripMenuItem _backGroundColour;
        private System.Windows.Forms.ColorDialog _colorDialog;
        private System.Windows.Forms.ToolStripMenuItem _saveAs;
        private System.Windows.Forms.ToolStripMenuItem _findMI;
        private System.Windows.Forms.ToolStripMenuItem _wordWrap;
    }
}
