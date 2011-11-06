namespace SampleEditor {
	partial class TheEditor {
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
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.labelLength = new System.Windows.Forms.Label();
			this.buttonPlay = new System.Windows.Forms.Button();
			this.buttonPause = new System.Windows.Forms.Button();
			this.buttonResume = new System.Windows.Forms.Button();
			this.buttonStop = new System.Windows.Forms.Button();
			this.labelExecution = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Length:";
			// 
			// labelLength
			// 
			this.labelLength.AutoSize = true;
			this.labelLength.Location = new System.Drawing.Point(76, 13);
			this.labelLength.Name = "labelLength";
			this.labelLength.Size = new System.Drawing.Size(13, 13);
			this.labelLength.TabIndex = 1;
			this.labelLength.Text = "0";
			// 
			// buttonPlay
			// 
			this.buttonPlay.Enabled = false;
			this.buttonPlay.Location = new System.Drawing.Point(16, 65);
			this.buttonPlay.Name = "buttonPlay";
			this.buttonPlay.Size = new System.Drawing.Size(75, 23);
			this.buttonPlay.TabIndex = 2;
			this.buttonPlay.Text = "Play";
			this.buttonPlay.UseVisualStyleBackColor = true;
			this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
			// 
			// buttonPause
			// 
			this.buttonPause.Enabled = false;
			this.buttonPause.Location = new System.Drawing.Point(145, 65);
			this.buttonPause.Name = "buttonPause";
			this.buttonPause.Size = new System.Drawing.Size(75, 23);
			this.buttonPause.TabIndex = 3;
			this.buttonPause.Text = "Pause";
			this.buttonPause.UseVisualStyleBackColor = true;
			this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
			// 
			// buttonResume
			// 
			this.buttonResume.Enabled = false;
			this.buttonResume.Location = new System.Drawing.Point(227, 65);
			this.buttonResume.Name = "buttonResume";
			this.buttonResume.Size = new System.Drawing.Size(75, 23);
			this.buttonResume.TabIndex = 4;
			this.buttonResume.Text = "Resume";
			this.buttonResume.UseVisualStyleBackColor = true;
			this.buttonResume.Click += new System.EventHandler(this.buttonResume_Click);
			// 
			// buttonStop
			// 
			this.buttonStop.Enabled = false;
			this.buttonStop.Location = new System.Drawing.Point(351, 65);
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.Size = new System.Drawing.Size(75, 23);
			this.buttonStop.TabIndex = 5;
			this.buttonStop.Text = "Stop";
			this.buttonStop.UseVisualStyleBackColor = true;
			this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// labelExecution
			// 
			this.labelExecution.AutoSize = true;
			this.labelExecution.Location = new System.Drawing.Point(76, 30);
			this.labelExecution.Name = "labelExecution";
			this.labelExecution.Size = new System.Drawing.Size(13, 13);
			this.labelExecution.TabIndex = 7;
			this.labelExecution.Text = "0";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 30);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Execution:";
			// 
			// timer
			// 
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// TheEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(438, 140);
			this.Controls.Add(this.labelExecution);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonStop);
			this.Controls.Add(this.buttonResume);
			this.Controls.Add(this.buttonPause);
			this.Controls.Add(this.buttonPlay);
			this.Controls.Add(this.labelLength);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "TheEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "(No Sequence)";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TheEditor_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelLength;
		private System.Windows.Forms.Button buttonPlay;
		private System.Windows.Forms.Button buttonPause;
		private System.Windows.Forms.Button buttonResume;
		private System.Windows.Forms.Button buttonStop;
		private System.Windows.Forms.Label labelExecution;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Timer timer;
	}
}