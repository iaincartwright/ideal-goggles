namespace iConsoleFrame
{
    partial class iConsoles
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form Designer generated code

		private iControls.iRichTextBox _rtb_debug;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private iControls.iRichTextConsole _rtc_console;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._rtb_debug = new iControls.iRichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._rtc_console = new iControls.iRichTextConsole();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(791, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.testToolStripMenuItem.Text = "Test";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // _rtb_debug
            // 
            this._rtb_debug.CaretCol = 0;
            this._rtb_debug.CaretLine = 0;
            this._rtb_debug.CtrlDown = false;
            this._rtb_debug.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rtb_debug.Font = new System.Drawing.Font("Consolas", 12F);
            this._rtb_debug.Location = new System.Drawing.Point(0, 0);
            this._rtb_debug.Name = "_rtb_debug";
            this._rtb_debug.Size = new System.Drawing.Size(334, 463);
            this._rtb_debug.TabIndex = 0;
            this._rtb_debug.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._rtb_debug);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._rtc_console);
            this.splitContainer1.Size = new System.Drawing.Size(791, 463);
            this.splitContainer1.SplitterDistance = 334;
            this.splitContainer1.TabIndex = 2;
            // 
            // _rtc_console
            // 
            this._rtc_console.CaretCol = 0;
            this._rtc_console.CaretLine = 0;
            this._rtc_console.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rtc_console.Font = new System.Drawing.Font("Consolas", 12F);
            this._rtc_console.Location = new System.Drawing.Point(0, 0);
            this._rtc_console.Name = "_rtc_console";
            this._rtc_console.Size = new System.Drawing.Size(453, 463);
            this._rtc_console.TabIndex = 0;
            this._rtc_console.Text = "";
            // 
            // iConsoles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 487);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "iConsoles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "iConsoles";
            this.Load += new System.EventHandler(this.iConsoles_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}

