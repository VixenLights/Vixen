namespace VixenModules.App.WebServer
{
	partial class Settings
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
			if (disposing && (components != null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.lblDisclaimer = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(152, 139);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            64000,
            0,
            0,
            0});
			this.numericUpDown1.Minimum = new decimal(new int[] {
            80,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(53, 20);
			this.numericUpDown1.TabIndex = 0;
			this.numericUpDown1.Value = new decimal(new int[] {
            8000,
            0,
            0,
            0});
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(94, 141);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Http Port:";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox1.Location = new System.Drawing.Point(97, 165);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(73, 17);
			this.checkBox1.TabIndex = 3;
			this.checkBox1.Text = "IsEnabled";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(168, 89);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(125, 25);
			this.label2.TabIndex = 5;
			this.label2.Text = "Web Server";
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(12, 210);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(55, 13);
			this.linkLabel1.TabIndex = 6;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "linkLabel1";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(227, 281);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(146, 281);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 9;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 197);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(121, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Point your browser here:";
			// 
			// lblDisclaimer
			// 
			this.lblDisclaimer.AutoSize = true;
			this.lblDisclaimer.Location = new System.Drawing.Point(12, 180);
			this.lblDisclaimer.Name = "lblDisclaimer";
			this.lblDisclaimer.Size = new System.Drawing.Size(0, 13);
			this.lblDisclaimer.TabIndex = 11;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(4, 2);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(307, 129);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 15;
			this.pictureBox1.TabStop = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 243);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(290, 35);
			this.label4.TabIndex = 16;
			this.label4.Text = "Turn off the web server when not in use to conserve resources.";
			// 
			// Settings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(314, 316);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblDisclaimer);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.pictureBox1);
			this.MaximumSize = new System.Drawing.Size(330, 355);
			this.MinimumSize = new System.Drawing.Size(330, 355);
			this.Name = "Settings";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Configuration";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblDisclaimer;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label4;
	}
}