namespace VixenModules.Analysis.BeatsAndBars
{
	partial class BeatsAndBarsSettings
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
			this.vampParamCtrl1 = new QMLibrary.VampParamCtrl();
			this.SuspendLayout();
			// 
			// vampParamCtrl1
			// 
			this.vampParamCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.vampParamCtrl1.Location = new System.Drawing.Point(13, 12);
			this.vampParamCtrl1.Name = "vampParamCtrl1";
			this.vampParamCtrl1.Size = new System.Drawing.Size(418, 67);
			this.vampParamCtrl1.TabIndex = 0;
			// 
			// BeatsAndBarsSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(443, 261);
			this.Controls.Add(this.vampParamCtrl1);
			this.Name = "BeatsAndBarsSettings";
			this.Text = "BeatsAndBarsSettings";
			this.ResumeLayout(false);

		}

		#endregion

		private QMLibrary.VampParamCtrl vampParamCtrl1;
	}
}