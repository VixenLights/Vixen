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
			this.panel1 = new System.Windows.Forms.Panel();
			this.panelProperties = new System.Windows.Forms.Panel();
			this.panel5 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.panel3 = new System.Windows.Forms.Panel();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBoxStringToEdit = new System.Windows.Forms.ComboBox();
			this.panel1.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.panelProperties);
			this.panel1.Controls.Add(this.panel5);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(467, 468);
			this.panel1.TabIndex = 55;
			// 
			// panelProperties
			// 
			this.panelProperties.AutoScroll = true;
			this.panelProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelProperties.Location = new System.Drawing.Point(0, 72);
			this.panelProperties.Name = "panelProperties";
			this.panelProperties.Size = new System.Drawing.Size(467, 396);
			this.panelProperties.TabIndex = 61;
			// 
			// panel5
			// 
			this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel5.Controls.Add(this.label1);
			this.panel5.Controls.Add(this.buttonHelp);
			this.panel5.Controls.Add(this.panel3);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel5.Location = new System.Drawing.Point(0, 0);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(467, 72);
			this.panel5.TabIndex = 60;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 13);
			this.label1.TabIndex = 59;
			this.label1.Text = "Custom Prop";
			// 
			// buttonHelp
			// 
			this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(401, 4);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(60, 23);
			this.buttonHelp.TabIndex = 57;
			this.buttonHelp.Text = "Help";
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			this.buttonHelp.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonHelp.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.panel3.Controls.Add(this.label6);
			this.panel3.Controls.Add(this.comboBoxStringToEdit);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel3.Location = new System.Drawing.Point(0, 34);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(465, 36);
			this.panel3.TabIndex = 58;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 9);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(37, 13);
			this.label6.TabIndex = 54;
			this.label6.Text = "String:";
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
			this.comboBoxStringToEdit.Location = new System.Drawing.Point(51, 6);
			this.comboBoxStringToEdit.Name = "comboBoxStringToEdit";
			this.comboBoxStringToEdit.Size = new System.Drawing.Size(409, 21);
			this.comboBoxStringToEdit.TabIndex = 55;
			this.comboBoxStringToEdit.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			this.comboBoxStringToEdit.SelectedIndexChanged += new System.EventHandler(this.comboBoxStringToEdit_SelectedIndexChanged);
			// 
			// PreviewCustomSetupControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "PreviewCustomSetupControl";
			this.Size = new System.Drawing.Size(467, 468);
			this.Title = "Custom Properties";
			this.Load += new System.EventHandler(this.PreviewCustomSetupControl_Load);
			this.panel1.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxStringToEdit;
        private System.Windows.Forms.Panel panelProperties;
        private System.Windows.Forms.Label label1;

    }
}
