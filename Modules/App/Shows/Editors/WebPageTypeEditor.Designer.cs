namespace VixenModules.App.Shows
{
	partial class WebPageTypeEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebPageTypeEditor));
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxWebsite = new System.Windows.Forms.TextBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.buttonTest = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Web Page:";
			// 
			// textBoxWebsite
			// 
			this.textBoxWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxWebsite.Location = new System.Drawing.Point(66, 3);
			this.textBoxWebsite.Name = "textBoxWebsite";
			this.textBoxWebsite.Size = new System.Drawing.Size(193, 20);
			this.textBoxWebsite.TabIndex = 1;
			this.textBoxWebsite.TextChanged += new System.EventHandler(this.textBoxWebsite_TextChanged);
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog";
			this.openFileDialog.Multiselect = true;
			// 
			// buttonTest
			// 
			this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonTest.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonTest.BackgroundImage")));
			this.buttonTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonTest.Location = new System.Drawing.Point(260, 3);
			this.buttonTest.Name = "buttonTest";
			this.buttonTest.Size = new System.Drawing.Size(20, 20);
			this.buttonTest.TabIndex = 16;
			this.toolTip1.SetToolTip(this.buttonTest, "Test - Load Website");
			this.buttonTest.UseVisualStyleBackColor = true;
			this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
			// 
			// WebPageTypeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonTest);
			this.Controls.Add(this.textBoxWebsite);
			this.Controls.Add(this.label1);
			this.Name = "WebPageTypeEditor";
			this.Size = new System.Drawing.Size(285, 79);
			this.Load += new System.EventHandler(this.SequenceTypeEditor_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxWebsite;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button buttonTest;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
