namespace TestHarness
{
	partial class Form1
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
			this.iEditableDetails1 = new iControls.iEditableDetails();
			this.iMemoryView1 = new iControls.iMemoryView();
			this.SuspendLayout();
			// 
			// iEditableDetails1
			// 
			this.iEditableDetails1.Dock = System.Windows.Forms.DockStyle.Left;
			this.iEditableDetails1.Location = new System.Drawing.Point(0, 0);
			this.iEditableDetails1.Name = "iEditableDetails1";
			this.iEditableDetails1.ReadOnly = false;
			this.iEditableDetails1.ShowGroups = true;
			this.iEditableDetails1.Size = new System.Drawing.Size(215, 511);
			this.iEditableDetails1.TabIndex = 0;
			this.iEditableDetails1.Load += new System.EventHandler(this.iEditableDetails1_Load);
			// 
			// iMemoryView1
			// 
			this.iMemoryView1.BackgroundColor = System.Drawing.Color.PaleGoldenrod;
			this.iMemoryView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.iMemoryView1.BytesPerSquare = 8;
			this.iMemoryView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.iMemoryView1.HeapSize = 1048576;
			this.iMemoryView1.Location = new System.Drawing.Point(215, 0);
			this.iMemoryView1.Name = "iMemoryView1";
			this.iMemoryView1.Size = new System.Drawing.Size(606, 511);
			this.iMemoryView1.SquareSize = 6;
			this.iMemoryView1.TabIndex = 1;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(821, 511);
			this.Controls.Add(this.iMemoryView1);
			this.Controls.Add(this.iEditableDetails1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private iControls.iEditableDetails iEditableDetails1;
		private iControls.iMemoryView iMemoryView1;
	}
}

