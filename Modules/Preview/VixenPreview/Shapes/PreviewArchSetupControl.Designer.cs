namespace VixenModules.Preview.VixenPreview.Shapes
{
    partial class PreviewArchSetupControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewArchSetupControl));
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this.propertyGrid.BackColor = System.Drawing.SystemColors.Control;
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(150, 150);
			this.propertyGrid.TabIndex = 2;
			// 
			// buttonHelp
			// 
			this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(134, 2);
			this.buttonHelp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(90, 35);
			this.buttonHelp.TabIndex = 66;
			this.buttonHelp.Text = "Help";
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			this.buttonHelp.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonHelp.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// PreviewArchSetupControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.propertyGrid);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "PreviewArchSetupControl";
			this.Title = "Arch Properties";
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button buttonHelp;
    }
}
