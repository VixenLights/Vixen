namespace Common.Controls
{
	partial class DiscreteColorPicker
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.labelSelectPrompt = new System.Windows.Forms.Label();
			this.tableLayoutPanelColors = new System.Windows.Forms.TableLayoutPanel();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(253, 285);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 27);
			this.buttonCancel.TabIndex = 0;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonOK_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonOK_MouseHover);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(159, 285);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(87, 27);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonOK_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonOK_MouseHover);
			// 
			// labelSelectPrompt
			// 
			this.labelSelectPrompt.AutoSize = true;
			this.labelSelectPrompt.Location = new System.Drawing.Point(31, 27);
			this.labelSelectPrompt.Name = "labelSelectPrompt";
			this.labelSelectPrompt.Size = new System.Drawing.Size(188, 15);
			this.labelSelectPrompt.TabIndex = 2;
			this.labelSelectPrompt.Text = "Select one or more discrete colors:";
			// 
			// tableLayoutPanelColors
			// 
			this.tableLayoutPanelColors.ColumnCount = 3;
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanelColors.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanelColors.Location = new System.Drawing.Point(33, 60);
			this.tableLayoutPanelColors.Name = "tableLayoutPanelColors";
			this.tableLayoutPanelColors.RowCount = 3;
			this.tableLayoutPanelColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33233F));
			this.tableLayoutPanelColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33533F));
			this.tableLayoutPanelColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33233F));
			this.tableLayoutPanelColors.Size = new System.Drawing.Size(294, 187);
			this.tableLayoutPanelColors.TabIndex = 3;
			// 
			// DiscreteColorPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(355, 325);
			this.Controls.Add(this.tableLayoutPanelColors);
			this.Controls.Add(this.labelSelectPrompt);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DiscreteColorPicker";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Color(s)";
			this.Load += new System.EventHandler(this.DiscreteColorPicker_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label labelSelectPrompt;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelColors;
	}
}