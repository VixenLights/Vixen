namespace VixenModules.Output.LauncherController
{
	partial class SetupForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.chkHideLaunchedWindows = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(172, 77);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 36);
            this.button1.TabIndex = 4;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
            this.button1.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
            // 
            // chkHideLaunchedWindows
            // 
            this.chkHideLaunchedWindows.AutoSize = true;
            this.chkHideLaunchedWindows.Location = new System.Drawing.Point(123, 31);
            this.chkHideLaunchedWindows.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkHideLaunchedWindows.Name = "chkHideLaunchedWindows";
            this.chkHideLaunchedWindows.Size = new System.Drawing.Size(195, 24);
            this.chkHideLaunchedWindows.TabIndex = 0;
            this.chkHideLaunchedWindows.Text = "Hide Launched Windows";
            this.chkHideLaunchedWindows.UseVisualStyleBackColor = true;
            this.chkHideLaunchedWindows.CheckedChanged += new System.EventHandler(this.chkHideLaunchedWindows_CheckedChanged);
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 138);
            this.Controls.Add(this.chkHideLaunchedWindows);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SetupForm";
            this.Text = "Launcher Controller Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkHideLaunchedWindows;
	}
}