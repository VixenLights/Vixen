namespace VixenModules.App.Curves
{
	partial class CurveLibrarySelector
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.listViewCurves = new System.Windows.Forms.ListView();
			this.buttonEditCurve = new System.Windows.Forms.Button();
			this.buttonDeleteCurve = new System.Windows.Forms.Button();
			this.buttonNewCurve = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(449, 390);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(93, 29);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(349, 390);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(93, 29);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = false;
			// 
			// listViewCurves
			// 
			this.listViewCurves.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewCurves.Location = new System.Drawing.Point(12, 12);
			this.listViewCurves.Name = "listViewCurves";
			this.listViewCurves.Size = new System.Drawing.Size(532, 364);
			this.listViewCurves.TabIndex = 6;
			this.listViewCurves.UseCompatibleStateImageBehavior = false;
			this.listViewCurves.SelectedIndexChanged += new System.EventHandler(this.listViewCurves_SelectedIndexChanged);
			this.listViewCurves.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewCurves_MouseDoubleClick);
			// 
			// buttonEditCurve
			// 
			this.buttonEditCurve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonEditCurve.Enabled = false;
			this.buttonEditCurve.Location = new System.Drawing.Point(114, 390);
			this.buttonEditCurve.Name = "buttonEditCurve";
			this.buttonEditCurve.Size = new System.Drawing.Size(93, 29);
			this.buttonEditCurve.TabIndex = 7;
			this.buttonEditCurve.Text = "Edit Curve";
			this.buttonEditCurve.UseVisualStyleBackColor = false;
			this.buttonEditCurve.Click += new System.EventHandler(this.buttonEditCurve_Click);
			// 
			// buttonDeleteCurve
			// 
			this.buttonDeleteCurve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDeleteCurve.Enabled = false;
			this.buttonDeleteCurve.Location = new System.Drawing.Point(215, 390);
			this.buttonDeleteCurve.Name = "buttonDeleteCurve";
			this.buttonDeleteCurve.Size = new System.Drawing.Size(93, 29);
			this.buttonDeleteCurve.TabIndex = 8;
			this.buttonDeleteCurve.Text = "Delete Curve";
			this.buttonDeleteCurve.UseVisualStyleBackColor = false;
			this.buttonDeleteCurve.Click += new System.EventHandler(this.buttonDeleteCurve_Click);
			// 
			// buttonNewCurve
			// 
			this.buttonNewCurve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonNewCurve.Location = new System.Drawing.Point(14, 390);
			this.buttonNewCurve.Name = "buttonNewCurve";
			this.buttonNewCurve.Size = new System.Drawing.Size(93, 29);
			this.buttonNewCurve.TabIndex = 9;
			this.buttonNewCurve.Text = "New Curve";
			this.buttonNewCurve.UseVisualStyleBackColor = false;
			this.buttonNewCurve.Click += new System.EventHandler(this.buttonNewCurve_Click);
			// 
			// CurveLibrarySelector
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(556, 434);
			this.Controls.Add(this.buttonNewCurve);
			this.Controls.Add(this.buttonDeleteCurve);
			this.Controls.Add(this.buttonEditCurve);
			this.Controls.Add(this.listViewCurves);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(572, 472);
			this.Name = "CurveLibrarySelector";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Curve Library";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CurveLibrarySelector_FormClosing);
			this.Load += new System.EventHandler(this.CurveLibrarySelector_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CurveLibrarySelector_KeyDown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ListView listViewCurves;
		private System.Windows.Forms.Button buttonEditCurve;
		private System.Windows.Forms.Button buttonDeleteCurve;
		private System.Windows.Forms.Button buttonNewCurve;
	}
}