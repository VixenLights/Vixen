namespace Common.Controls
{
    partial class UndoDropDownControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listbox = new System.Windows.Forms.ListBox();
            this.bottomtext = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listbox
            // 
            this.listbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listbox.FormattingEnabled = true;
            this.listbox.Location = new System.Drawing.Point(1, 1);
            this.listbox.Name = "listbox";
            this.listbox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listbox.Size = new System.Drawing.Size(146, 195);
            this.listbox.TabIndex = 0;
            // 
            // bottomtext
            // 
            this.bottomtext.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomtext.Location = new System.Drawing.Point(1, 183);
            this.bottomtext.Margin = new System.Windows.Forms.Padding(3, 4, 3, 1);
            this.bottomtext.Name = "bottomtext";
            this.bottomtext.Size = new System.Drawing.Size(146, 13);
            this.bottomtext.TabIndex = 1;
            this.bottomtext.Text = "bottomtext";
            this.bottomtext.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // UndoDropDownControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.bottomtext);
            this.Controls.Add(this.listbox);
            this.Name = "UndoDropDownControl";
            this.Padding = new System.Windows.Forms.Padding(1, 1, 1, 4);
            this.Size = new System.Drawing.Size(148, 200);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listbox;
        private System.Windows.Forms.Label bottomtext;
    }
}
