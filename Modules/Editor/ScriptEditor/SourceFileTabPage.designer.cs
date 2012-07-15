namespace VixenModules.Editor.ScriptEditor {
	partial class SourceFileTabPage {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
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
			this.scintilla = new ScintillaNET.Scintilla();
			((System.ComponentModel.ISupportInitialize)(this.scintilla)).BeginInit();
			this.SuspendLayout();
			// 
			// scintilla
			// 
			this.scintilla.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scintilla.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.scintilla.Location = new System.Drawing.Point(0, 0);
			this.scintilla.Margins.Margin0.Width = 50;
			this.scintilla.Name = "scintilla";
			this.scintilla.Size = new System.Drawing.Size(471, 274);
			this.scintilla.Styles.BraceBad.Size = 10F;
			this.scintilla.Styles.BraceLight.Size = 10F;
			this.scintilla.Styles.ControlChar.Size = 10F;
			this.scintilla.Styles.Default.BackColor = System.Drawing.SystemColors.Window;
			this.scintilla.Styles.Default.Size = 10F;
			this.scintilla.Styles.IndentGuide.Size = 10F;
			this.scintilla.Styles.LastPredefined.Size = 10F;
			this.scintilla.Styles.LineNumber.Size = 9F;
			this.scintilla.Styles.Max.Size = 10F;
			this.scintilla.TabIndex = 1;
			this.scintilla.SelectionChanged += new System.EventHandler(this.scintilla_SelectionChanged);
			// 
			// SourceFileTabPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.scintilla);
			this.Name = "SourceFileTabPage";
			this.Size = new System.Drawing.Size(471, 274);
			((System.ComponentModel.ISupportInitialize)(this.scintilla)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private ScintillaNET.Scintilla scintilla;
	}
}
