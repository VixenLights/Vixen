namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class Form_Grid
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
			this.timelineControl = new Common.Controls.Timeline.TimelineControl();
			this.SuspendLayout();
			// 
			// timelineControl
			// 
			this.timelineControl.AllowGridResize = true;
			this.timelineControl.Audio = null;
			this.timelineControl.AutoSize = true;
			this.timelineControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.timelineControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.timelineControl.Location = new System.Drawing.Point(0, 0);
			this.timelineControl.Margin = new System.Windows.Forms.Padding(0);
			this.timelineControl.Name = "timelineControl";
			this.timelineControl.SequenceLoading = false;
			this.timelineControl.Size = new System.Drawing.Size(1012, 399);
			this.timelineControl.TabIndex = 3;
			// 
			// Form_Grid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1012, 399);
			this.CloseButton = false;
			this.CloseButtonVisible = false;
			this.Controls.Add(this.timelineControl);
			this.Name = "Form_Grid";
			this.Text = "Timeline";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Common.Controls.Timeline.TimelineControl timelineControl;
	}
}