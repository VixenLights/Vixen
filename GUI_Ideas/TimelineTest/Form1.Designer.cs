namespace Timeline
{
    partial class Form1
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
            this.buttonElemAt = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonElemAt
            // 
            this.buttonElemAt.Location = new System.Drawing.Point(13, 597);
            this.buttonElemAt.Name = "buttonElemAt";
            this.buttonElemAt.Size = new System.Drawing.Size(115, 23);
            this.buttonElemAt.TabIndex = 0;
            this.buttonElemAt.Text = "Elements at 0:02";
            this.buttonElemAt.UseVisualStyleBackColor = true;
            this.buttonElemAt.Click += new System.EventHandler(this.buttonElemAt_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(145, 596);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 1;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(931, 641);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.buttonElemAt);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonElemAt;
        private System.Windows.Forms.Button buttonReset;
    }
}

