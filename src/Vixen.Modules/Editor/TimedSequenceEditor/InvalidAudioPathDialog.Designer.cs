namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class InvalidAudioPathDialog
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
			this.buttonLocateAudio = new System.Windows.Forms.Button();
			this.buttonRemoveAudio = new System.Windows.Forms.Button();
			this.buttonKeepAudio = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.labelAudioPath = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonLocateAudio
			// 
			this.buttonLocateAudio.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.buttonLocateAudio.Location = new System.Drawing.Point(174, 135);
			this.buttonLocateAudio.Name = "buttonLocateAudio";
			this.buttonLocateAudio.Size = new System.Drawing.Size(143, 27);
			this.buttonLocateAudio.TabIndex = 0;
			this.buttonLocateAudio.Text = "Select New Audio";
			this.buttonLocateAudio.UseVisualStyleBackColor = true;
			this.buttonLocateAudio.Click += new System.EventHandler(this.buttonLocateAudio_Click);
			this.buttonLocateAudio.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonLocateAudio.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonRemoveAudio
			// 
			this.buttonRemoveAudio.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.buttonRemoveAudio.Location = new System.Drawing.Point(324, 135);
			this.buttonRemoveAudio.Name = "buttonRemoveAudio";
			this.buttonRemoveAudio.Size = new System.Drawing.Size(143, 27);
			this.buttonRemoveAudio.TabIndex = 0;
			this.buttonRemoveAudio.Text = "Forget This Audio";
			this.buttonRemoveAudio.UseVisualStyleBackColor = true;
			this.buttonRemoveAudio.Click += new System.EventHandler(this.buttonRemoveAudio_Click);
			this.buttonRemoveAudio.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonRemoveAudio.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonKeepAudio
			// 
			this.buttonKeepAudio.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			this.buttonKeepAudio.Location = new System.Drawing.Point(475, 135);
			this.buttonKeepAudio.Name = "buttonKeepAudio";
			this.buttonKeepAudio.Size = new System.Drawing.Size(143, 27);
			this.buttonKeepAudio.TabIndex = 0;
			this.buttonKeepAudio.Text = "Proceed As Is";
			this.buttonKeepAudio.UseVisualStyleBackColor = true;
			this.buttonKeepAudio.Click += new System.EventHandler(this.buttonKeepAudio_Click);
			this.buttonKeepAudio.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonKeepAudio.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(177, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Audio file not found at the path:";
			// 
			// labelAudioPath
			// 
			this.labelAudioPath.AutoSize = true;
			this.labelAudioPath.Location = new System.Drawing.Point(15, 42);
			this.labelAudioPath.Name = "labelAudioPath";
			this.labelAudioPath.Size = new System.Drawing.Size(0, 15);
			this.labelAudioPath.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(170, 117);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(199, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Selected the action you wish to take:";
			// 
			// InvalidAudioPathDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(632, 172);
			this.ControlBox = false;
			this.Controls.Add(this.labelAudioPath);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonKeepAudio);
			this.Controls.Add(this.buttonRemoveAudio);
			this.Controls.Add(this.buttonLocateAudio);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "InvalidAudioPathDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Missing Audio File";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonLocateAudio;
		private System.Windows.Forms.Button buttonRemoveAudio;
		private System.Windows.Forms.Button buttonKeepAudio;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelAudioPath;
		private System.Windows.Forms.Label label2;
	}
}