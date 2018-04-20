namespace iControls
{
    partial class iMemoryView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this.vScroll = new System.Windows.Forms.VScrollBar();
            this.hScroll = new System.Windows.Forms.HScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _pictureBox
            // 
            this._pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pictureBox.Location = new System.Drawing.Point(0, 0);
            this._pictureBox.MinimumSize = new System.Drawing.Size(32, 32);
            this._pictureBox.Name = "_pictureBox";
            this._pictureBox.Size = new System.Drawing.Size(176, 129);
            this._pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pictureBox.TabIndex = 0;
            this._pictureBox.TabStop = false;
            this._pictureBox.Resize += new System.EventHandler(this._pictureBox_Resize);
            // 
            // vScroll
            // 
            this.vScroll.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScroll.Location = new System.Drawing.Point(176, 0);
            this.vScroll.Maximum = 1000;
            this.vScroll.Name = "vScroll";
            this.vScroll.Size = new System.Drawing.Size(17, 146);
            this.vScroll.TabIndex = 1;
            this.vScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScroll_Scroll);
            // 
            // hScroll
            // 
            this.hScroll.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScroll.Location = new System.Drawing.Point(0, 129);
            this.hScroll.Maximum = 256;
            this.hScroll.Name = "hScroll";
            this.hScroll.Size = new System.Drawing.Size(176, 17);
            this.hScroll.TabIndex = 2;
            this.hScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScroll_Scroll);
            // 
            // iMemoryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this._pictureBox);
            this.Controls.Add(this.hScroll);
            this.Controls.Add(this.vScroll);
            this.Name = "iMemoryView";
            this.Size = new System.Drawing.Size(193, 146);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.VScrollBar vScroll;
        private System.Windows.Forms.HScrollBar hScroll;
    }
}
