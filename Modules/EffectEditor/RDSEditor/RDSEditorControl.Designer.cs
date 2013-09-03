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
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Text";
			// 
			// textRDSTitle
			// 
			this.textRDSTitle.Location = new System.Drawing.Point(45, 4);
			this.textRDSTitle.Name = "textRDSTitle";
			this.textRDSTitle.Size = new System.Drawing.Size(348, 20);
			this.textRDSTitle.TabIndex = 1;
			// 
			// textRDSArtist
			// 
			this.textRDSArtist.Location = new System.Drawing.Point(45, 30);
			this.textRDSArtist.Name = "textRDSArtist";
			this.textRDSArtist.Size = new System.Drawing.Size(348, 20);
			this.textRDSArtist.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(3, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(36, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Artist";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(42, 53);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(300, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Note:  Some RDS Implementations will not show the Artist Tag";
			// 
			// RDSEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textRDSArtist);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textRDSTitle);
			this.Controls.Add(this.label1);
			this.Name = "RDSEditorControl";
			this.Size = new System.Drawing.Size(396, 79);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textRDSTitle;
		private System.Windows.Forms.TextBox textRDSArtist;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
	}
}
