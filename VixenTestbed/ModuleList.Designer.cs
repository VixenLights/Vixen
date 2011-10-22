namespace VixenTestbed {
	partial class ModuleList {
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonReloadModule = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxModules = new System.Windows.Forms.ListBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonReloadModule);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(213, 30);
			this.panel1.TabIndex = 0;
			// 
			// buttonReloadModule
			// 
			this.buttonReloadModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonReloadModule.Enabled = false;
			this.buttonReloadModule.Location = new System.Drawing.Point(143, 7);
			this.buttonReloadModule.Name = "buttonReloadModule";
			this.buttonReloadModule.Size = new System.Drawing.Size(67, 20);
			this.buttonReloadModule.TabIndex = 1;
			this.buttonReloadModule.Text = "Reload";
			this.buttonReloadModule.UseVisualStyleBackColor = true;
			this.buttonReloadModule.Visible = false;
			this.buttonReloadModule.Click += new System.EventHandler(this.buttonReloadModule_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Loaded Modules";
			// 
			// listBoxModules
			// 
			this.listBoxModules.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxModules.FormattingEnabled = true;
			this.listBoxModules.Location = new System.Drawing.Point(0, 30);
			this.listBoxModules.Name = "listBoxModules";
			this.listBoxModules.Size = new System.Drawing.Size(213, 297);
			this.listBoxModules.TabIndex = 1;
			this.listBoxModules.SelectedIndexChanged += new System.EventHandler(this.listBoxModules_SelectedIndexChanged);
			// 
			// ModuleList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listBoxModules);
			this.Controls.Add(this.panel1);
			this.Name = "ModuleList";
			this.Size = new System.Drawing.Size(213, 327);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox listBoxModules;
		private System.Windows.Forms.Button buttonReloadModule;
	}
}
