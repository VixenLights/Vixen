namespace VixenApplication {
	partial class CreateAndNameChannels {
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxChannelCount = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxNamingMethod = new System.Windows.Forms.ComboBox();
			this.buttonPreview = new System.Windows.Forms.Button();
			this.buttonConfigureNamingRule = new System.Windows.Forms.Button();
			this.listBoxPreview = new System.Windows.Forms.ListBox();
			this.buttonCommit = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(26, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Create";
			// 
			// textBoxChannelCount
			// 
			this.textBoxChannelCount.Location = new System.Drawing.Point(70, 21);
			this.textBoxChannelCount.Name = "textBoxChannelCount";
			this.textBoxChannelCount.Size = new System.Drawing.Size(48, 20);
			this.textBoxChannelCount.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(124, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "channels";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(26, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(92, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Name them using:";
			// 
			// comboBoxNamingMethod
			// 
			this.comboBoxNamingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNamingMethod.FormattingEnabled = true;
			this.comboBoxNamingMethod.Location = new System.Drawing.Point(127, 58);
			this.comboBoxNamingMethod.Name = "comboBoxNamingMethod";
			this.comboBoxNamingMethod.Size = new System.Drawing.Size(179, 21);
			this.comboBoxNamingMethod.TabIndex = 4;
			// 
			// buttonPreview
			// 
			this.buttonPreview.Location = new System.Drawing.Point(29, 106);
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.Size = new System.Drawing.Size(75, 23);
			this.buttonPreview.TabIndex = 6;
			this.buttonPreview.Text = "Preview";
			this.buttonPreview.UseVisualStyleBackColor = true;
			this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
			// 
			// buttonConfigureNamingRule
			// 
			this.buttonConfigureNamingRule.Location = new System.Drawing.Point(312, 56);
			this.buttonConfigureNamingRule.Name = "buttonConfigureNamingRule";
			this.buttonConfigureNamingRule.Size = new System.Drawing.Size(75, 23);
			this.buttonConfigureNamingRule.TabIndex = 5;
			this.buttonConfigureNamingRule.Text = "Configure";
			this.buttonConfigureNamingRule.UseVisualStyleBackColor = true;
			this.buttonConfigureNamingRule.Click += new System.EventHandler(this.buttonConfigureNamingRule_Click);
			// 
			// listBoxPreview
			// 
			this.listBoxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxPreview.FormattingEnabled = true;
			this.listBoxPreview.Location = new System.Drawing.Point(29, 135);
			this.listBoxPreview.Name = "listBoxPreview";
			this.listBoxPreview.Size = new System.Drawing.Size(196, 186);
			this.listBoxPreview.TabIndex = 7;
			// 
			// buttonCommit
			// 
			this.buttonCommit.Location = new System.Drawing.Point(312, 298);
			this.buttonCommit.Name = "buttonCommit";
			this.buttonCommit.Size = new System.Drawing.Size(75, 23);
			this.buttonCommit.TabIndex = 8;
			this.buttonCommit.Text = "Commit";
			this.buttonCommit.UseVisualStyleBackColor = true;
			this.buttonCommit.Click += new System.EventHandler(this.buttonCommit_Click);
			// 
			// CreateAndNameChannels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(429, 341);
			this.Controls.Add(this.buttonCommit);
			this.Controls.Add(this.listBoxPreview);
			this.Controls.Add(this.buttonConfigureNamingRule);
			this.Controls.Add(this.buttonPreview);
			this.Controls.Add(this.comboBoxNamingMethod);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxChannelCount);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CreateAndNameChannels";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Create and Name Channels [TEST]";
			this.Load += new System.EventHandler(this.CreateAndNameChannels_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxChannelCount;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxNamingMethod;
		private System.Windows.Forms.Button buttonPreview;
		private System.Windows.Forms.Button buttonConfigureNamingRule;
		private System.Windows.Forms.ListBox listBoxPreview;
		private System.Windows.Forms.Button buttonCommit;
	}
}