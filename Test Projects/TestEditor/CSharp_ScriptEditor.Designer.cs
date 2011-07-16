namespace TestEditor {
	partial class CSharp_ScriptEditor {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.tabControl = new System.Windows.Forms.TabControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonCompile = new System.Windows.Forms.Button();
			this.buttonStop = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.tabControlErrors = new System.Windows.Forms.TabControl();
			this.tabPageCompileErrors = new System.Windows.Forms.TabPage();
			this.listBoxCompileErrors = new System.Windows.Forms.ListBox();
			this.tabPageRuntimeErrors = new System.Windows.Forms.TabPage();
			this.listBoxRuntimeErrors = new System.Windows.Forms.ListBox();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.tabControlErrors.SuspendLayout();
			this.tabPageCompileErrors.SuspendLayout();
			this.tabPageRuntimeErrors.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(629, 311);
			this.tabControl.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonCompile);
			this.panel1.Controls.Add(this.buttonStop);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(629, 38);
			this.panel1.TabIndex = 1;
			// 
			// buttonCompile
			// 
			this.buttonCompile.Location = new System.Drawing.Point(240, 9);
			this.buttonCompile.Name = "buttonCompile";
			this.buttonCompile.Size = new System.Drawing.Size(92, 23);
			this.buttonCompile.TabIndex = 2;
			this.buttonCompile.Text = "Compile only";
			this.buttonCompile.UseVisualStyleBackColor = true;
			this.buttonCompile.Click += new System.EventHandler(this.buttonCompile_Click);
			// 
			// buttonStop
			// 
			this.buttonStop.Location = new System.Drawing.Point(123, 9);
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.Size = new System.Drawing.Size(75, 23);
			this.buttonStop.TabIndex = 1;
			this.buttonStop.Text = "Stop";
			this.buttonStop.UseVisualStyleBackColor = true;
			this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 9);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(105, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Build and Run";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 38);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.tabControl);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.tabControlErrors);
			this.splitContainer.Size = new System.Drawing.Size(629, 467);
			this.splitContainer.SplitterDistance = 311;
			this.splitContainer.TabIndex = 2;
			// 
			// tabControlErrors
			// 
			this.tabControlErrors.Controls.Add(this.tabPageCompileErrors);
			this.tabControlErrors.Controls.Add(this.tabPageRuntimeErrors);
			this.tabControlErrors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlErrors.Location = new System.Drawing.Point(0, 0);
			this.tabControlErrors.Name = "tabControlErrors";
			this.tabControlErrors.SelectedIndex = 0;
			this.tabControlErrors.Size = new System.Drawing.Size(629, 152);
			this.tabControlErrors.TabIndex = 1;
			// 
			// tabPageCompileErrors
			// 
			this.tabPageCompileErrors.Controls.Add(this.listBoxCompileErrors);
			this.tabPageCompileErrors.Location = new System.Drawing.Point(4, 22);
			this.tabPageCompileErrors.Name = "tabPageCompileErrors";
			this.tabPageCompileErrors.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCompileErrors.Size = new System.Drawing.Size(621, 126);
			this.tabPageCompileErrors.TabIndex = 0;
			this.tabPageCompileErrors.Text = "Compile";
			this.tabPageCompileErrors.UseVisualStyleBackColor = true;
			// 
			// listBoxCompileErrors
			// 
			this.listBoxCompileErrors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxCompileErrors.FormattingEnabled = true;
			this.listBoxCompileErrors.HorizontalScrollbar = true;
			this.listBoxCompileErrors.Location = new System.Drawing.Point(3, 3);
			this.listBoxCompileErrors.Name = "listBoxCompileErrors";
			this.listBoxCompileErrors.Size = new System.Drawing.Size(615, 120);
			this.listBoxCompileErrors.TabIndex = 0;
			// 
			// tabPageRuntimeErrors
			// 
			this.tabPageRuntimeErrors.Controls.Add(this.listBoxRuntimeErrors);
			this.tabPageRuntimeErrors.Location = new System.Drawing.Point(4, 22);
			this.tabPageRuntimeErrors.Name = "tabPageRuntimeErrors";
			this.tabPageRuntimeErrors.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageRuntimeErrors.Size = new System.Drawing.Size(621, 126);
			this.tabPageRuntimeErrors.TabIndex = 1;
			this.tabPageRuntimeErrors.Text = "Runtime";
			this.tabPageRuntimeErrors.UseVisualStyleBackColor = true;
			// 
			// listBoxRuntimeErrors
			// 
			this.listBoxRuntimeErrors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxRuntimeErrors.FormattingEnabled = true;
			this.listBoxRuntimeErrors.Location = new System.Drawing.Point(3, 3);
			this.listBoxRuntimeErrors.Name = "listBoxRuntimeErrors";
			this.listBoxRuntimeErrors.Size = new System.Drawing.Size(615, 120);
			this.listBoxRuntimeErrors.TabIndex = 1;
			// 
			// ScriptEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(629, 505);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.panel1);
			this.Name = "ScriptEditor";
			this.Text = "ScriptEditor";
			this.panel1.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.tabControlErrors.ResumeLayout(false);
			this.tabPageCompileErrors.ResumeLayout(false);
			this.tabPageRuntimeErrors.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.ListBox listBoxCompileErrors;
		private System.Windows.Forms.TabControl tabControlErrors;
		private System.Windows.Forms.TabPage tabPageCompileErrors;
		private System.Windows.Forms.TabPage tabPageRuntimeErrors;
		private System.Windows.Forms.ListBox listBoxRuntimeErrors;
		private System.Windows.Forms.Button buttonStop;
		private System.Windows.Forms.Button buttonCompile;
	}
}