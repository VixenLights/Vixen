namespace VixenModules.EffectEditor.RDSEditor {
	partial class RDSEditorControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
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
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.textRDSTitle = new System.Windows.Forms.TextBox();
			this.textRDSArtist = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Title";
			// 
			// textRDSTitle
			// 
			this.textRDSTitle.Location = new System.Drawing.Point(90, 4);
			this.textRDSTitle.Name = "textRDSTitle";
			this.textRDSTitle.Size = new System.Drawing.Size(295, 20);
			this.textRDSTitle.TabIndex = 1;
			// 
			// textRDSArtist
			// 
			this.textRDSArtist.Location = new System.Drawing.Point(90, 30);
			this.textRDSArtist.Name = "textRDSArtist";
			this.textRDSArtist.Size = new System.Drawing.Size(295, 20);
			this.textRDSArtist.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(30, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Artist";
			// 
			// RDSEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textRDSArtist);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textRDSTitle);
			this.Controls.Add(this.label1);
			this.Name = "RDSEditorControl";
			this.Size = new System.Drawing.Size(396, 57);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textRDSTitle;
		private System.Windows.Forms.TextBox textRDSArtist;
		private System.Windows.Forms.Label label2;
	}
}
