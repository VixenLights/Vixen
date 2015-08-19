namespace VixenApplication
{
    partial class SelectProfile
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
			this.listBoxProfiles = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.buttonEditor = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBoxProfiles
			// 
			this.listBoxProfiles.FormattingEnabled = true;
			this.listBoxProfiles.ItemHeight = 20;
			this.listBoxProfiles.Location = new System.Drawing.Point(18, 54);
			this.listBoxProfiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.listBoxProfiles.Name = "listBoxProfiles";
			this.listBoxProfiles.Size = new System.Drawing.Size(289, 144);
			this.listBoxProfiles.TabIndex = 0;
			this.listBoxProfiles.DoubleClick += new System.EventHandler(this.listBoxProfiles_DoubleClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 20);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(177, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "Select a Profile to Load:";
			// 
			// buttonLoad
			// 
			this.buttonLoad.Location = new System.Drawing.Point(196, 209);
			this.buttonLoad.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(112, 35);
			this.buttonLoad.TabIndex = 2;
			this.buttonLoad.Text = "Load";
			this.buttonLoad.UseVisualStyleBackColor = true;
			this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
			this.buttonLoad.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.buttonLoad.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonLoad.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonEditor
			// 
			this.buttonEditor.Location = new System.Drawing.Point(18, 209);
			this.buttonEditor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.buttonEditor.Name = "buttonEditor";
			this.buttonEditor.Size = new System.Drawing.Size(112, 35);
			this.buttonEditor.TabIndex = 3;
			this.buttonEditor.Text = "Profile Editor";
			this.buttonEditor.UseVisualStyleBackColor = true;
			this.buttonEditor.Click += new System.EventHandler(this.buttonEditor_Click);
			this.buttonEditor.Paint += new System.Windows.Forms.PaintEventHandler(this.button_Paint);
			this.buttonEditor.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonEditor.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// SelectProfile
			// 
			this.AcceptButton = this.buttonLoad;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(328, 260);
			this.Controls.Add(this.buttonEditor);
			this.Controls.Add(this.buttonLoad);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBoxProfiles);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(350, 516);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(350, 316);
			this.Name = "SelectProfile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select a Profile";
			this.Load += new System.EventHandler(this.SelectProfile_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxProfiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonEditor;
    }
}