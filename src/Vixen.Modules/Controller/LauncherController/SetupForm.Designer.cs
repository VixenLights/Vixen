namespace VixenModules.Output.LauncherController
{
	partial class SetupForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
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
			button1 = new System.Windows.Forms.Button();
			chkHideLaunchedWindows = new System.Windows.Forms.CheckBox();
			SuspendLayout();
			// 
			// button1
			// 
			button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
			button1.Location = new System.Drawing.Point(150, 58);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(83, 27);
			button1.TabIndex = 4;
			button1.Text = "Save";
			button1.UseVisualStyleBackColor = true;
			// 
			// chkHideLaunchedWindows
			// 
			chkHideLaunchedWindows.AutoSize = true;
			chkHideLaunchedWindows.Location = new System.Drawing.Point(108, 23);
			chkHideLaunchedWindows.Name = "chkHideLaunchedWindows";
			chkHideLaunchedWindows.Size = new System.Drawing.Size(158, 19);
			chkHideLaunchedWindows.TabIndex = 0;
			chkHideLaunchedWindows.Text = "Hide Launched Windows";
			chkHideLaunchedWindows.UseVisualStyleBackColor = true;
			chkHideLaunchedWindows.CheckedChanged += chkHideLaunchedWindows_CheckedChanged;
			// 
			// SetupForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(382, 105);
			Controls.Add(chkHideLaunchedWindows);
			Controls.Add(button1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Text = "Launcher Controller Configuration";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkHideLaunchedWindows;
	}
}