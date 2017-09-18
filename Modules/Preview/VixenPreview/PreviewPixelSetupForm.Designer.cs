namespace VixenModules.Preview.VixenPreview
{
	partial class PreviewPixelSetupForm
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lblPrefixName = new System.Windows.Forms.Label();
			this.lblIndex = new System.Windows.Forms.Label();
			this.suffixIndexChooser = new System.Windows.Forms.NumericUpDown();
			this.bulbSizeChooser = new System.Windows.Forms.NumericUpDown();
			this.txtPrefixName = new System.Windows.Forms.TextBox();
			this.lblPixelSize = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.suffixIndexChooser)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bulbSizeChooser)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.lblPrefixName, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblIndex, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.suffixIndexChooser, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.bulbSizeChooser, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.txtPrefixName, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblPixelSize, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.button1, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.button2, 2, 4);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(5);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(312, 145);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// lblPrefixName
			// 
			this.lblPrefixName.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblPrefixName.AutoSize = true;
			this.lblPrefixName.Location = new System.Drawing.Point(3, 6);
			this.lblPrefixName.Name = "lblPrefixName";
			this.lblPrefixName.Size = new System.Drawing.Size(64, 13);
			this.lblPrefixName.TabIndex = 0;
			this.lblPrefixName.Text = "Prefix Name";
			// 
			// lblIndex
			// 
			this.lblIndex.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblIndex.AutoSize = true;
			this.lblIndex.Location = new System.Drawing.Point(3, 32);
			this.lblIndex.Name = "lblIndex";
			this.lblIndex.Size = new System.Drawing.Size(101, 13);
			this.lblIndex.TabIndex = 1;
			this.lblIndex.Text = "Suffix Starting Index";
			// 
			// suffixIndexChooser
			// 
			this.suffixIndexChooser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.suffixIndexChooser.Location = new System.Drawing.Point(110, 29);
			this.suffixIndexChooser.Name = "suffixIndexChooser";
			this.suffixIndexChooser.Size = new System.Drawing.Size(98, 20);
			this.suffixIndexChooser.TabIndex = 1;
			// 
			// bulbSizeChooser
			// 
			this.bulbSizeChooser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.bulbSizeChooser.Location = new System.Drawing.Point(110, 55);
			this.bulbSizeChooser.Name = "bulbSizeChooser";
			this.bulbSizeChooser.Size = new System.Drawing.Size(98, 20);
			this.bulbSizeChooser.TabIndex = 2;
			// 
			// txtPrefixName
			// 
			this.txtPrefixName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.SetColumnSpan(this.txtPrefixName, 2);
			this.txtPrefixName.Location = new System.Drawing.Point(110, 3);
			this.txtPrefixName.Name = "txtPrefixName";
			this.txtPrefixName.Size = new System.Drawing.Size(199, 20);
			this.txtPrefixName.TabIndex = 0;
			// 
			// lblPixelSize
			// 
			this.lblPixelSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblPixelSize.AutoSize = true;
			this.lblPixelSize.Location = new System.Drawing.Point(3, 58);
			this.lblPixelSize.Name = "lblPixelSize";
			this.lblPixelSize.Size = new System.Drawing.Size(51, 13);
			this.lblPixelSize.TabIndex = 5;
			this.lblPixelSize.Text = "Bulb Size";
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.AutoSize = true;
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(115, 113);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(93, 29);
			this.button1.TabIndex = 3;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.button1.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.AutoSize = true;
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(216, 113);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(93, 29);
			this.button2.TabIndex = 4;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// PreviewPixelSetupForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(312, 145);
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "PreviewPixelSetupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Single Bulb Configuration";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.suffixIndexChooser)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bulbSizeChooser)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label lblPrefixName;
		private System.Windows.Forms.Label lblIndex;
		private System.Windows.Forms.NumericUpDown suffixIndexChooser;
		private System.Windows.Forms.NumericUpDown bulbSizeChooser;
		private System.Windows.Forms.TextBox txtPrefixName;
		private System.Windows.Forms.Label lblPixelSize;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}