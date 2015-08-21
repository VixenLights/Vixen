namespace VixenModules.OutputFilter.ColorBreakdown
{
	partial class ColorBreakdownSetup
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxTemplates = new System.Windows.Forms.ComboBox();
			this.buttonApplyTemplate = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonAddColor = new System.Windows.Forms.Button();
			this.tableLayoutPanelControls = new System.Windows.Forms.TableLayoutPanel();
			this.checkBoxMixColors = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(323, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select a pre-configured template from the drop-down box, or build a";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(23, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(179, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "custom color breakdown filter below.";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(31, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Template:";
			// 
			// comboBoxTemplates
			// 
			this.comboBoxTemplates.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTemplates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxTemplates.Location = new System.Drawing.Point(96, 75);
			this.comboBoxTemplates.Name = "comboBoxTemplates";
			this.comboBoxTemplates.Size = new System.Drawing.Size(121, 21);
			this.comboBoxTemplates.TabIndex = 3;
			this.comboBoxTemplates.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			// 
			// buttonApplyTemplate
			// 
			this.buttonApplyTemplate.Location = new System.Drawing.Point(231, 72);
			this.buttonApplyTemplate.Name = "buttonApplyTemplate";
			this.buttonApplyTemplate.Size = new System.Drawing.Size(111, 25);
			this.buttonApplyTemplate.TabIndex = 5;
			this.buttonApplyTemplate.Text = "Apply Template";
			this.buttonApplyTemplate.UseVisualStyleBackColor = true;
			this.buttonApplyTemplate.Click += new System.EventHandler(this.buttonApplyTemplate_Click);
			this.buttonApplyTemplate.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonApplyTemplate.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(292, 425);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 25);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(206, 425);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 25);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonAddColor
			// 
			this.buttonAddColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAddColor.Location = new System.Drawing.Point(26, 400);
			this.buttonAddColor.Name = "buttonAddColor";
			this.buttonAddColor.Size = new System.Drawing.Size(100, 25);
			this.buttonAddColor.TabIndex = 8;
			this.buttonAddColor.Text = "Add Color";
			this.buttonAddColor.UseVisualStyleBackColor = true;
			this.buttonAddColor.Click += new System.EventHandler(this.buttonAddColor_Click);
			this.buttonAddColor.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonAddColor.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// tableLayoutPanelControls
			// 
			this.tableLayoutPanelControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanelControls.AutoScroll = true;
			this.tableLayoutPanelControls.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanelControls.ColumnCount = 1;
			this.tableLayoutPanelControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelControls.Location = new System.Drawing.Point(26, 117);
			this.tableLayoutPanelControls.Name = "tableLayoutPanelControls";
			this.tableLayoutPanelControls.RowCount = 1;
			this.tableLayoutPanelControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelControls.Size = new System.Drawing.Size(330, 237);
			this.tableLayoutPanelControls.TabIndex = 9;
			// 
			// checkBoxMixColors
			// 
			this.checkBoxMixColors.AutoSize = true;
			this.checkBoxMixColors.Location = new System.Drawing.Point(26, 369);
			this.checkBoxMixColors.Name = "checkBoxMixColors";
			this.checkBoxMixColors.Size = new System.Drawing.Size(290, 17);
			this.checkBoxMixColors.TabIndex = 10;
			this.checkBoxMixColors.Text = "These colors should be mixed to produce the input color";
			this.checkBoxMixColors.UseVisualStyleBackColor = true;
			this.checkBoxMixColors.CheckedChanged += new System.EventHandler(this.checkBoxMixColors_CheckedChanged);
			// 
			// ColorBreakdownSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 462);
			this.Controls.Add(this.checkBoxMixColors);
			this.Controls.Add(this.tableLayoutPanelControls);
			this.Controls.Add(this.buttonAddColor);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonApplyTemplate);
			this.Controls.Add(this.comboBoxTemplates);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 480);
			this.Name = "ColorBreakdownSetup";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Breakdown Filter Setup";
			this.Load += new System.EventHandler(this.ColorBreakdownSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxTemplates;
		private System.Windows.Forms.Button buttonApplyTemplate;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonAddColor;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelControls;
		private System.Windows.Forms.CheckBox checkBoxMixColors;
	}
}