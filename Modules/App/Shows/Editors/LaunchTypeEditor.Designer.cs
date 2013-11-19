namespace VixenModules.App.Shows
{
	partial class LaunchTypeEditor
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
			this.buttonSelectProgram = new System.Windows.Forms.Button();
			this.textBoxProgram = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.textBoxCommandLine = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonTest = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.checkBoxShowCommandWindow = new System.Windows.Forms.CheckBox();
			this.checkBoxWaitForExit = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// buttonSelectProgram
			// 
			this.buttonSelectProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSelectProgram.Location = new System.Drawing.Point(267, 2);
			this.buttonSelectProgram.Name = "buttonSelectProgram";
			this.buttonSelectProgram.Size = new System.Drawing.Size(20, 20);
			this.buttonSelectProgram.TabIndex = 12;
			this.buttonSelectProgram.Text = "S";
			this.toolTip1.SetToolTip(this.buttonSelectProgram, "Select a File");
			this.buttonSelectProgram.UseVisualStyleBackColor = true;
			this.buttonSelectProgram.Click += new System.EventHandler(this.buttonSelectProgram_Click);
			// 
			// textBoxProgram
			// 
			this.textBoxProgram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxProgram.Location = new System.Drawing.Point(90, 2);
			this.textBoxProgram.Name = "textBoxProgram";
			this.textBoxProgram.Size = new System.Drawing.Size(176, 20);
			this.textBoxProgram.TabIndex = 11;
			this.textBoxProgram.TextChanged += new System.EventHandler(this.textBoxProgram_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Program to Run:";
			// 
			// textBoxCommandLine
			// 
			this.textBoxCommandLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxCommandLine.Location = new System.Drawing.Point(90, 28);
			this.textBoxCommandLine.Name = "textBoxCommandLine";
			this.textBoxCommandLine.Size = new System.Drawing.Size(176, 20);
			this.textBoxCommandLine.TabIndex = 14;
			this.textBoxCommandLine.TextChanged += new System.EventHandler(this.textBoxCommandLine_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(0, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 13);
			this.label2.TabIndex = 13;
			this.label2.Text = "Command Line:";
			// 
			// buttonTest
			// 
			this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonTest.Location = new System.Drawing.Point(267, 28);
			this.buttonTest.Name = "buttonTest";
			this.buttonTest.Size = new System.Drawing.Size(20, 20);
			this.buttonTest.TabIndex = 15;
			this.buttonTest.Text = "T";
			this.toolTip1.SetToolTip(this.buttonTest, "Test");
			this.buttonTest.UseVisualStyleBackColor = true;
			this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
			// 
			// checkBoxShowCommandWindow
			// 
			this.checkBoxShowCommandWindow.AutoSize = true;
			this.checkBoxShowCommandWindow.Location = new System.Drawing.Point(90, 54);
			this.checkBoxShowCommandWindow.Name = "checkBoxShowCommandWindow";
			this.checkBoxShowCommandWindow.Size = new System.Drawing.Size(145, 17);
			this.checkBoxShowCommandWindow.TabIndex = 16;
			this.checkBoxShowCommandWindow.Text = "Show Command Window";
			this.checkBoxShowCommandWindow.UseVisualStyleBackColor = true;
			this.checkBoxShowCommandWindow.CheckedChanged += new System.EventHandler(this.checkBoxShowCommandWindow_CheckedChanged);
			// 
			// checkBoxWaitForExit
			// 
			this.checkBoxWaitForExit.AutoSize = true;
			this.checkBoxWaitForExit.Location = new System.Drawing.Point(90, 77);
			this.checkBoxWaitForExit.Name = "checkBoxWaitForExit";
			this.checkBoxWaitForExit.Size = new System.Drawing.Size(83, 17);
			this.checkBoxWaitForExit.TabIndex = 17;
			this.checkBoxWaitForExit.Text = "Wait for Exit";
			this.checkBoxWaitForExit.UseVisualStyleBackColor = true;
			this.checkBoxWaitForExit.CheckedChanged += new System.EventHandler(this.checkBoxWaitForExit_CheckedChanged);
			// 
			// LaunchTypeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.checkBoxWaitForExit);
			this.Controls.Add(this.checkBoxShowCommandWindow);
			this.Controls.Add(this.buttonTest);
			this.Controls.Add(this.textBoxCommandLine);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonSelectProgram);
			this.Controls.Add(this.textBoxProgram);
			this.Controls.Add(this.label1);
			this.Name = "LaunchTypeEditor";
			this.Size = new System.Drawing.Size(290, 150);
			this.Load += new System.EventHandler(this.LaunchTypeEditor_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonSelectProgram;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TextBox textBoxProgram;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.TextBox textBoxCommandLine;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonTest;
		private System.Windows.Forms.CheckBox checkBoxShowCommandWindow;
		private System.Windows.Forms.CheckBox checkBoxWaitForExit;
	}
}
