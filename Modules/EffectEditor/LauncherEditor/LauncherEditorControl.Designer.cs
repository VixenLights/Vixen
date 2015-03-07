namespace LauncherEditor
{
	partial class LauncherEditorControl
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
			if (disposing && (components != null)) {
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtFileName = new System.Windows.Forms.TextBox();
			this.txtArguments = new System.Windows.Forms.TextBox();
			this.btnOpenFile = new System.Windows.Forms.Button();
			this.txtDescription = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 66);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Executable";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 155);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "Arguments";
			// 
			// txtFileName
			// 
			this.txtFileName.Location = new System.Drawing.Point(9, 92);
			this.txtFileName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.txtFileName.Multiline = true;
			this.txtFileName.Name = "txtFileName";
			this.txtFileName.Size = new System.Drawing.Size(614, 56);
			this.txtFileName.TabIndex = 1;
			// 
			// txtArguments
			// 
			this.txtArguments.Location = new System.Drawing.Point(9, 180);
			this.txtArguments.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.txtArguments.Multiline = true;
			this.txtArguments.Name = "txtArguments";
			this.txtArguments.Size = new System.Drawing.Size(672, 72);
			this.txtArguments.TabIndex = 3;
			// 
			// btnOpenFile
			// 
			this.btnOpenFile.Location = new System.Drawing.Point(634, 92);
			this.btnOpenFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnOpenFile.Name = "btnOpenFile";
			this.btnOpenFile.Size = new System.Drawing.Size(48, 58);
			this.btnOpenFile.TabIndex = 2;
			this.btnOpenFile.Text = "...";
			this.btnOpenFile.UseVisualStyleBackColor = true;
			this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
			// 
			// txtDescription
			// 
			this.txtDescription.Location = new System.Drawing.Point(9, 26);
			this.txtDescription.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.txtDescription.MaxLength = 30;
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.Size = new System.Drawing.Size(672, 26);
			this.txtDescription.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 0);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(89, 20);
			this.label3.TabIndex = 5;
			this.label3.Text = "Description";
			// 
			// LauncherEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.txtDescription);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnOpenFile);
			this.Controls.Add(this.txtArguments);
			this.Controls.Add(this.txtFileName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "LauncherEditorControl";
			this.Size = new System.Drawing.Size(688, 274);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtFileName;
		private System.Windows.Forms.TextBox txtArguments;
		private System.Windows.Forms.Button btnOpenFile;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.Label label3;
	}
}
