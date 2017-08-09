namespace VixenModules.Preview.VixenPreview
{
    partial class LocationOffsetForm
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
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.HorizontalOffset = new System.Windows.Forms.TextBox();
			this.VerticalOffset = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(43, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(62, 15);
			this.label3.TabIndex = 0;
			this.label3.Text = "Horizontal";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(43, 119);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(46, 15);
			this.label4.TabIndex = 1;
			this.label4.Text = "Vertical";
			// 
			// HorizontalOffset
			// 
			this.HorizontalOffset.Location = new System.Drawing.Point(113, 65);
			this.HorizontalOffset.Name = "HorizontalOffset";
			this.HorizontalOffset.Size = new System.Drawing.Size(116, 23);
			this.HorizontalOffset.TabIndex = 2;
			// 
			// VerticalOffset
			// 
			this.VerticalOffset.Location = new System.Drawing.Point(113, 115);
			this.VerticalOffset.Name = "VerticalOffset";
			this.VerticalOffset.Size = new System.Drawing.Size(116, 23);
			this.VerticalOffset.TabIndex = 2;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(113, 223);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(87, 27);
			this.button1.TabIndex = 3;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// LocationOffsetForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(309, 263);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.VerticalOffset);
			this.Controls.Add(this.HorizontalOffset);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Name = "LocationOffsetForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "LocationOffsetForm";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox HorizontalOffset;
        private System.Windows.Forms.TextBox VerticalOffset;
        private System.Windows.Forms.Button button1;
    }
}