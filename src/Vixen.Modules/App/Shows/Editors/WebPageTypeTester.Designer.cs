namespace VixenModules.App.Shows
{
	partial class WebPageTypeTester
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
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.buttonClose = new System.Windows.Forms.Button();
			this.labelURL = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// webBrowser
			// 
			this.webBrowser.Location = new System.Drawing.Point(12, 12);
			this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new System.Drawing.Size(643, 391);
			this.webBrowser.TabIndex = 0;
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(580, 415);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			// 
			// labelURL
			// 
			this.labelURL.AutoSize = true;
			this.labelURL.Location = new System.Drawing.Point(9, 420);
			this.labelURL.Name = "labelURL";
			this.labelURL.Size = new System.Drawing.Size(29, 13);
			this.labelURL.TabIndex = 2;
			this.labelURL.Text = "URL";
			// 
			// WebPageTypeTester
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(667, 447);
			this.Controls.Add(this.labelURL);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.webBrowser);
			this.Name = "WebPageTypeTester";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Web Page Test";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.WebBrowser webBrowser;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Label labelURL;
	}
}