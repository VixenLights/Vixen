namespace VixenModules.Preview.VixenPreview.Shapes
{
    partial class PreviewCustomSetupControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewCustomSetupControl));
			this.buttonHelp = new System.Windows.Forms.Button();
			this.btnSyncLightSize = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBoxStringToEdit = new System.Windows.Forms.ComboBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panelProperties = new System.Windows.Forms.Panel();
			this.lblSync = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonHelp
			// 
			this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonHelp.AutoSize = true;
			this.buttonHelp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(387, 3);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(22, 22);
			this.buttonHelp.TabIndex = 57;
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			// 
			// btnSyncLightSize
			// 
			this.btnSyncLightSize.AutoSize = true;
			this.btnSyncLightSize.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnSyncLightSize.Location = new System.Drawing.Point(3, 31);
			this.btnSyncLightSize.Name = "btnSyncLightSize";
			this.btnSyncLightSize.Size = new System.Drawing.Size(41, 23);
			this.btnSyncLightSize.TabIndex = 56;
			this.btnSyncLightSize.Text = "Sync";
			this.btnSyncLightSize.UseVisualStyleBackColor = true;
			this.btnSyncLightSize.Click += new System.EventHandler(this.btnSyncBulbSize_Click);
			// 
			// label6
			// 
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 7);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(34, 13);
			this.label6.TabIndex = 54;
			this.label6.Text = "String";
			// 
			// comboBoxStringToEdit
			// 
			this.comboBoxStringToEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxStringToEdit.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxStringToEdit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxStringToEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxStringToEdit.FormattingEnabled = true;
			this.comboBoxStringToEdit.Items.AddRange(new object[] {
            "Standard",
            "Pixel"});
			this.comboBoxStringToEdit.Location = new System.Drawing.Point(50, 3);
			this.comboBoxStringToEdit.Name = "comboBoxStringToEdit";
			this.comboBoxStringToEdit.Size = new System.Drawing.Size(331, 21);
			this.comboBoxStringToEdit.TabIndex = 55;
			this.comboBoxStringToEdit.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.comboBoxStringToEdit.SelectedIndexChanged += new System.EventHandler(this.comboBoxStringToEdit_SelectedIndexChanged);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.buttonHelp, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.comboBoxStringToEdit, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.panelProperties, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.btnSyncLightSize, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.lblSync, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(412, 468);
			this.tableLayoutPanel1.TabIndex = 56;
			// 
			// panelProperties
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.panelProperties, 3);
			this.panelProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelProperties.Location = new System.Drawing.Point(3, 60);
			this.panelProperties.Name = "panelProperties";
			this.panelProperties.Size = new System.Drawing.Size(406, 405);
			this.panelProperties.TabIndex = 58;
			// 
			// lblSync
			// 
			this.lblSync.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblSync.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.lblSync, 2);
			this.lblSync.Location = new System.Drawing.Point(50, 36);
			this.lblSync.Name = "lblSync";
			this.lblSync.Size = new System.Drawing.Size(173, 13);
			this.lblSync.TabIndex = 59;
			this.lblSync.Text = "Syncronize all light sizes with string.";
			// 
			// PreviewCustomSetupControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "PreviewCustomSetupControl";
			this.Size = new System.Drawing.Size(412, 468);
			this.Title = "Custom Prop Properties";
			this.Load += new System.EventHandler(this.PreviewCustomSetupControl_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxStringToEdit;
		private System.Windows.Forms.Button btnSyncLightSize;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panelProperties;
		private System.Windows.Forms.Label lblSync;
	}
}
