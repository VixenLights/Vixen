namespace Common.Controls.Wizard
{
	partial class WizardForm
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
			this.panelContent = new System.Windows.Forms.Panel();
			this.buttonNext = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonPrevious = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelContent
			// 
			this.panelContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelContent.Location = new System.Drawing.Point(14, 14);
			this.panelContent.Name = "panelContent";
			this.panelContent.Size = new System.Drawing.Size(583, 462);
			this.panelContent.TabIndex = 0;
			// 
			// buttonNext
			// 
			this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonNext.Location = new System.Drawing.Point(364, 19);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(105, 29);
			this.buttonNext.TabIndex = 11;
			this.buttonNext.Text = "Next >>";
			this.buttonNext.UseVisualStyleBackColor = true;
			this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(475, 19);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 10;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonPrevious
			// 
			this.buttonPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPrevious.Location = new System.Drawing.Point(253, 19);
			this.buttonPrevious.Name = "buttonPrevious";
			this.buttonPrevious.Size = new System.Drawing.Size(105, 29);
			this.buttonPrevious.TabIndex = 12;
			this.buttonPrevious.Text = "<< Previous";
			this.buttonPrevious.UseVisualStyleBackColor = true;
			this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.buttonPrevious, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonNext, 2, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(14, 482);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(583, 51);
			this.tableLayoutPanel1.TabIndex = 13;
			// 
			// WizardForm
			// 
			this.AcceptButton = this.buttonNext;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(611, 537);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.panelContent);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(627, 575);
			this.Name = "WizardForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Wizard";
			this.Load += new System.EventHandler(this.WizardForm_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelContent;
		private System.Windows.Forms.Button buttonNext;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonPrevious;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}