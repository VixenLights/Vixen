namespace VixenModules.Preview.VixenPreview
{
    partial class VixenPreviewSetupElementsDocument
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VixenPreviewSetupElementsDocument));
			this.imageListStatus = new System.Windows.Forms.ImageList(this.components);
			this.treeElements = new Common.Controls.ElementTree();
			this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.AddTemplatePanel = new System.Windows.Forms.FlowLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxNewItemType = new System.Windows.Forms.ComboBox();
			this.buttonAddTemplate = new System.Windows.Forms.Button();
			this.mainTableLayoutPanel.SuspendLayout();
			this.AddTemplatePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageListStatus
			// 
			this.imageListStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStatus.ImageStream")));
			this.imageListStatus.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListStatus.Images.SetKeyName(0, "bullet_green.png");
			this.imageListStatus.Images.SetKeyName(1, "bullet_white.png");
			this.imageListStatus.Images.SetKeyName(2, "bullet_yellow.png");
			// 
			// treeElements
			// 
			this.treeElements.AllowDragging = true;
			this.treeElements.AllowPropertyEdit = true;
			this.treeElements.AllowWireExport = true;
			this.treeElements.AutoSize = true;
			this.treeElements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeElements.ExportDiagram = null;
			this.treeElements.Location = new System.Drawing.Point(3, 40);
			this.treeElements.Name = "treeElements";
			this.treeElements.Size = new System.Drawing.Size(210, 370);
			this.treeElements.TabIndex = 0;
			// 
			// mainTableLayoutPanel
			// 
			this.mainTableLayoutPanel.ColumnCount = 1;
			this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTableLayoutPanel.Controls.Add(this.AddTemplatePanel, 0, 0);
			this.mainTableLayoutPanel.Controls.Add(this.treeElements, 0, 1);
			this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
			this.mainTableLayoutPanel.RowCount = 2;
			this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTableLayoutPanel.Size = new System.Drawing.Size(216, 413);
			this.mainTableLayoutPanel.TabIndex = 1;
			// 
			// AddTemplatePanel
			// 
			this.AddTemplatePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.AddTemplatePanel.Controls.Add(this.label1);
			this.AddTemplatePanel.Controls.Add(this.comboBoxNewItemType);
			this.AddTemplatePanel.Controls.Add(this.buttonAddTemplate);
			this.AddTemplatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AddTemplatePanel.Location = new System.Drawing.Point(0, 0);
			this.AddTemplatePanel.Margin = new System.Windows.Forms.Padding(0);
			this.AddTemplatePanel.Name = "AddTemplatePanel";
			this.AddTemplatePanel.Size = new System.Drawing.Size(216, 37);
			this.AddTemplatePanel.TabIndex = 45;
			this.AddTemplatePanel.WrapContents = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 12);
			this.label1.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 29;
			this.label1.Text = "Add:";
			// 
			// comboBoxNewItemType
			// 
			this.comboBoxNewItemType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxNewItemType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxNewItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNewItemType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxNewItemType.FormattingEnabled = true;
			this.comboBoxNewItemType.Location = new System.Drawing.Point(38, 8);
			this.comboBoxNewItemType.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
			this.comboBoxNewItemType.Name = "comboBoxNewItemType";
			this.comboBoxNewItemType.Size = new System.Drawing.Size(146, 21);
			this.comboBoxNewItemType.TabIndex = 30;
			this.comboBoxNewItemType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxNewItemType_DrawItem);
			this.comboBoxNewItemType.SelectedIndexChanged += new System.EventHandler(this.ComboBoxNewItemType_SelectedIndexChanged);
			// 
			// buttonAddTemplate
			// 
			this.buttonAddTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddTemplate.BackColor = System.Drawing.Color.Transparent;
			this.buttonAddTemplate.Enabled = false;
			this.buttonAddTemplate.FlatAppearance.BorderSize = 0;
			this.buttonAddTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddTemplate.Location = new System.Drawing.Point(190, 6);
			this.buttonAddTemplate.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
			this.buttonAddTemplate.Name = "buttonAddTemplate";
			this.buttonAddTemplate.Size = new System.Drawing.Size(24, 24);
			this.buttonAddTemplate.TabIndex = 31;
			this.buttonAddTemplate.Text = "+";
			this.buttonAddTemplate.UseVisualStyleBackColor = false;
			this.buttonAddTemplate.Click += new System.EventHandler(this.ButtonAddTemplate_Click);
			// 
			// VixenPreviewSetupElementsDocument
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(216, 413);
			this.CloseButton = false;
			this.CloseButtonVisible = false;
			this.Controls.Add(this.mainTableLayoutPanel);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
			this.Name = "VixenPreviewSetupElementsDocument";
			this.Text = "Elements";
			this.Load += new System.EventHandler(this.VixenPreviewSetupElementsDocument_Load);
			this.mainTableLayoutPanel.ResumeLayout(false);
			this.mainTableLayoutPanel.PerformLayout();
			this.AddTemplatePanel.ResumeLayout(false);
			this.AddTemplatePanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageListStatus;
		private Common.Controls.ElementTree treeElements;
		private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
		private System.Windows.Forms.FlowLayoutPanel AddTemplatePanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxNewItemType;
		private System.Windows.Forms.Button buttonAddTemplate;
	}
}