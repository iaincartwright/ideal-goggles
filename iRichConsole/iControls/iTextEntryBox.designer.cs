namespace iControls
{
    partial class iTextEntryBox
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._textB = new System.Windows.Forms.TextBox();
            this._findB = new System.Windows.Forms.Button();
            this._exitB = new System.Windows.Forms.Button();
            this._caseCB = new System.Windows.Forms.CheckBox();
            this._wholeWordCB = new System.Windows.Forms.CheckBox();
            this._reverseCB = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _textB
            // 
            this._textB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textB.Location = new System.Drawing.Point(12, 12);
            this._textB.Name = "_textB";
            this._textB.Size = new System.Drawing.Size(424, 20);
            this._textB.TabIndex = 0;
            // 
            // _findB
            // 
            this._findB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._findB.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._findB.Location = new System.Drawing.Point(280, 50);
            this._findB.Name = "_findB";
            this._findB.Size = new System.Drawing.Size(75, 23);
            this._findB.TabIndex = 1;
            this._findB.Text = "Find";
            this._findB.UseVisualStyleBackColor = true;
            // 
            // _exitB
            // 
            this._exitB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._exitB.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._exitB.Location = new System.Drawing.Point(361, 51);
            this._exitB.Name = "_exitB";
            this._exitB.Size = new System.Drawing.Size(75, 23);
            this._exitB.TabIndex = 2;
            this._exitB.Text = "Close";
            this._exitB.UseVisualStyleBackColor = true;
            // 
            // _caseCB
            // 
            this._caseCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._caseCB.AutoSize = true;
            this._caseCB.Location = new System.Drawing.Point(12, 55);
            this._caseCB.MinimumSize = new System.Drawing.Size(83, 17);
            this._caseCB.Name = "_caseCB";
            this._caseCB.Size = new System.Drawing.Size(83, 17);
            this._caseCB.TabIndex = 3;
            this._caseCB.Text = "Match Case";
            this._caseCB.UseVisualStyleBackColor = true;
            // 
            // _wholeWordCB
            // 
            this._wholeWordCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._wholeWordCB.AutoSize = true;
            this._wholeWordCB.Location = new System.Drawing.Point(100, 55);
            this._wholeWordCB.Name = "_wholeWordCB";
            this._wholeWordCB.Size = new System.Drawing.Size(86, 17);
            this._wholeWordCB.TabIndex = 4;
            this._wholeWordCB.Text = "Whole Word";
            this._wholeWordCB.UseVisualStyleBackColor = true;
            // 
            // _reverseCB
            // 
            this._reverseCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._reverseCB.AutoSize = true;
            this._reverseCB.Location = new System.Drawing.Point(188, 55);
            this._reverseCB.Name = "_reverseCB";
            this._reverseCB.Size = new System.Drawing.Size(77, 17);
            this._reverseCB.TabIndex = 5;
            this._reverseCB.Text = "Search Up";
            this._reverseCB.UseVisualStyleBackColor = true;
            // 
            // iTextEntryBox
            // 
            this.AcceptButton = this._findB;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._exitB;
            this.ClientSize = new System.Drawing.Size(448, 86);
            this.Controls.Add(this._reverseCB);
            this.Controls.Add(this._wholeWordCB);
            this.Controls.Add(this._caseCB);
            this.Controls.Add(this._exitB);
            this.Controls.Add(this._findB);
            this.Controls.Add(this._textB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximumSize = new System.Drawing.Size(1024, 112);
            this.MinimumSize = new System.Drawing.Size(456, 112);
            this.Name = "TextEntryBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TextEntryBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _textB;
        private System.Windows.Forms.Button _findB;
        private System.Windows.Forms.Button _exitB;
        private System.Windows.Forms.CheckBox _caseCB;
        private System.Windows.Forms.CheckBox _wholeWordCB;
        private System.Windows.Forms.CheckBox _reverseCB;
    }
}