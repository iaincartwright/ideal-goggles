namespace iControls
{
    partial class iEditableDetails
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
            this.components = new System.ComponentModel.Container();
            this._lv = new System.Windows.Forms.ListView();
            this.editPopup = new iControls.iEditPopup(this.components);
            this.SuspendLayout();
            // 
            // _lv
            // 
            this._lv.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lv.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lv.FullRowSelect = true;
            this._lv.Location = new System.Drawing.Point(0, 0);
            this._lv.Name = "_lv";
            this._lv.Size = new System.Drawing.Size(241, 176);
            this._lv.TabIndex = 0;
            this._lv.UseCompatibleStateImageBehavior = false;
            this._lv.View = System.Windows.Forms.View.Details;
            this._lv.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lv_MouseDoubleClick);
            this._lv.SelectedIndexChanged += new System.EventHandler(this._lv_SelectedIndexChanged);
            this._lv.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this._lv_ColumnClick);
            // 
            // editPopup
            // 
            this.editPopup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editPopup.ForeColor = System.Drawing.SystemColors.InfoText;
            this.editPopup.Location = new System.Drawing.Point(80, 68);
            this.editPopup.Margin = new System.Windows.Forms.Padding(0);
            this.editPopup.Name = "editPopup";
            this.editPopup.Size = new System.Drawing.Size(100, 20);
            this.editPopup.TabIndex = 1;
            // 
            // iEditableDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.editPopup);
            this.Controls.Add(this._lv);
            this.Name = "iEditableDetails";
            this.Size = new System.Drawing.Size(241, 176);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView _lv;
        private iEditPopup editPopup;
    }
}
