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
			this.label1.Location = new System.Drawing.Point(3, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Executable";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 101);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Arguments";
			// 
			// txtFileName
			// 
			this.txtFileName.Location = new System.Drawing.Point(6, 60);
			this.txtFileName.Multiline = true;
			this.txtFileName.Name = "txtFileName";
			this.txtFileName.Size = new System.Drawing.Size(411, 38);
			this.txtFileName.TabIndex = 2;
			// 
			// txtArguments
			// 
			this.txtArguments.Location = new System.Drawing.Point(6, 117);
			this.txtArguments.Multiline = true;
			this.txtArguments.Name = "txtArguments";
			this.txtArguments.Size = new System.Drawing.Size(449, 48);
			this.txtArguments.TabIndex = 3;
			// 
			// btnOpenFile
			// 
			this.btnOpenFile.Location = new System.Drawing.Point(423, 60);
			this.btnOpenFile.Name = "btnOpenFile";
			this.btnOpenFile.Size = new System.Drawing.Size(32, 38);
			this.btnOpenFile.TabIndex = 4;
			this.btnOpenFile.Text = "...";
			this.btnOpenFile.UseVisualStyleBackColor = true;
			this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
			// 
			// txtDescription
			// 
			this.txtDescription.Location = new System.Drawing.Point(6, 17);
			this.txtDescription.MaxLength = 30;
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.Size = new System.Drawing.Size(449, 20);
			this.txtDescription.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Description";
			// 
			// LauncherEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.txtDescription);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnOpenFile);
			this.Controls.Add(this.txtArguments);
			this.Controls.Add(this.txtFileName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "LauncherEditorControl";
			this.Size = new System.Drawing.Size(459, 178);
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
