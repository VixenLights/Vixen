namespace VixenTestbed {
	partial class Form1 {
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
			this.tabPageAdministration = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.buttonControllers = new System.Windows.Forms.Button();
			this.buttonPatching = new System.Windows.Forms.Button();
			this.buttonChannels = new System.Windows.Forms.Button();
			this.tabPageAppModule = new System.Windows.Forms.TabPage();
			this.moduleListApp = new VixenTestbed.ModuleList();
			this.tabPageEditorModule = new System.Windows.Forms.TabPage();
			this.moduleListEditor = new VixenTestbed.ModuleList();
			this.tabPageEffectModule = new System.Windows.Forms.TabPage();
			this.moduleListEffect = new VixenTestbed.ModuleList();
			this.tabPageEffectEditorModule = new System.Windows.Forms.TabPage();
			this.moduleListEffectEditor = new VixenTestbed.ModuleList();
			this.tabPageMediaModule = new System.Windows.Forms.TabPage();
			this.moduleListMedia = new VixenTestbed.ModuleList();
			this.tabPageOutputModule = new System.Windows.Forms.TabPage();
			this.moduleListOutput = new VixenTestbed.ModuleList();
			this.tabPagePropertyModule = new System.Windows.Forms.TabPage();
			this.moduleListProperty = new VixenTestbed.ModuleList();
			this.tabPageTimingModule = new System.Windows.Forms.TabPage();
			this.moduleListTiming = new VixenTestbed.ModuleList();
			this.tabControl.SuspendLayout();
			this.tabPageAdministration.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tabPageAppModule.SuspendLayout();
			this.tabPageEditorModule.SuspendLayout();
			this.tabPageEffectModule.SuspendLayout();
			this.tabPageEffectEditorModule.SuspendLayout();
			this.tabPageMediaModule.SuspendLayout();
			this.tabPageOutputModule.SuspendLayout();
			this.tabPagePropertyModule.SuspendLayout();
			this.tabPageTimingModule.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageAdministration);
			this.tabControl.Controls.Add(this.tabPageAppModule);
			this.tabControl.Controls.Add(this.tabPageEditorModule);
			this.tabControl.Controls.Add(this.tabPageEffectModule);
			this.tabControl.Controls.Add(this.tabPageEffectEditorModule);
			this.tabControl.Controls.Add(this.tabPageMediaModule);
			this.tabControl.Controls.Add(this.tabPageOutputModule);
			this.tabControl.Controls.Add(this.tabPagePropertyModule);
			this.tabControl.Controls.Add(this.tabPageTimingModule);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(658, 370);
			this.tabControl.TabIndex = 0;
			// 
			// tabPageAdministration
			// 
			this.tabPageAdministration.Controls.Add(this.tableLayoutPanel1);
			this.tabPageAdministration.Location = new System.Drawing.Point(4, 22);
			this.tabPageAdministration.Name = "tabPageAdministration";
			this.tabPageAdministration.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAdministration.Size = new System.Drawing.Size(650, 344);
			this.tabPageAdministration.TabIndex = 0;
			this.tabPageAdministration.Text = "Administration";
			this.tabPageAdministration.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.buttonControllers, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonPatching, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.buttonChannels, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(644, 338);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// buttonControllers
			// 
			this.buttonControllers.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonControllers.Location = new System.Drawing.Point(267, 38);
			this.buttonControllers.Name = "buttonControllers";
			this.buttonControllers.Size = new System.Drawing.Size(110, 35);
			this.buttonControllers.TabIndex = 0;
			this.buttonControllers.Text = "Controllers";
			this.buttonControllers.UseVisualStyleBackColor = true;
			this.buttonControllers.Click += new System.EventHandler(this.buttonControllers_Click);
			// 
			// buttonPatching
			// 
			this.buttonPatching.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonPatching.Location = new System.Drawing.Point(267, 263);
			this.buttonPatching.Name = "buttonPatching";
			this.buttonPatching.Size = new System.Drawing.Size(110, 35);
			this.buttonPatching.TabIndex = 2;
			this.buttonPatching.Text = "Patching";
			this.buttonPatching.UseVisualStyleBackColor = true;
			this.buttonPatching.Click += new System.EventHandler(this.buttonPatching_Click);
			// 
			// buttonChannels
			// 
			this.buttonChannels.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonChannels.Location = new System.Drawing.Point(267, 150);
			this.buttonChannels.Name = "buttonChannels";
			this.buttonChannels.Size = new System.Drawing.Size(110, 35);
			this.buttonChannels.TabIndex = 1;
			this.buttonChannels.Text = "Channels";
			this.buttonChannels.UseVisualStyleBackColor = true;
			this.buttonChannels.Click += new System.EventHandler(this.buttonChannels_Click);
			// 
			// tabPageAppModule
			// 
			this.tabPageAppModule.Controls.Add(this.moduleListApp);
			this.tabPageAppModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageAppModule.Name = "tabPageAppModule";
			this.tabPageAppModule.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAppModule.Size = new System.Drawing.Size(650, 344);
			this.tabPageAppModule.TabIndex = 1;
			this.tabPageAppModule.Text = "App Module";
			this.tabPageAppModule.UseVisualStyleBackColor = true;
			// 
			// moduleListApp
			// 
			this.moduleListApp.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListApp.Location = new System.Drawing.Point(3, 3);
			this.moduleListApp.Name = "moduleListApp";
			this.moduleListApp.Size = new System.Drawing.Size(186, 338);
			this.moduleListApp.TabIndex = 1;
			// 
			// tabPageEditorModule
			// 
			this.tabPageEditorModule.Controls.Add(this.moduleListEditor);
			this.tabPageEditorModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageEditorModule.Name = "tabPageEditorModule";
			this.tabPageEditorModule.Size = new System.Drawing.Size(650, 344);
			this.tabPageEditorModule.TabIndex = 2;
			this.tabPageEditorModule.Text = "Editor Module";
			this.tabPageEditorModule.UseVisualStyleBackColor = true;
			// 
			// moduleListEditor
			// 
			this.moduleListEditor.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListEditor.Location = new System.Drawing.Point(0, 0);
			this.moduleListEditor.Name = "moduleListEditor";
			this.moduleListEditor.Size = new System.Drawing.Size(186, 344);
			this.moduleListEditor.TabIndex = 2;
			// 
			// tabPageEffectModule
			// 
			this.tabPageEffectModule.Controls.Add(this.moduleListEffect);
			this.tabPageEffectModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageEffectModule.Name = "tabPageEffectModule";
			this.tabPageEffectModule.Size = new System.Drawing.Size(650, 344);
			this.tabPageEffectModule.TabIndex = 3;
			this.tabPageEffectModule.Text = "Effect Module";
			this.tabPageEffectModule.UseVisualStyleBackColor = true;
			// 
			// moduleListEffect
			// 
			this.moduleListEffect.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListEffect.Location = new System.Drawing.Point(0, 0);
			this.moduleListEffect.Name = "moduleListEffect";
			this.moduleListEffect.Size = new System.Drawing.Size(213, 344);
			this.moduleListEffect.TabIndex = 0;
			// 
			// tabPageEffectEditorModule
			// 
			this.tabPageEffectEditorModule.Controls.Add(this.moduleListEffectEditor);
			this.tabPageEffectEditorModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageEffectEditorModule.Name = "tabPageEffectEditorModule";
			this.tabPageEffectEditorModule.Size = new System.Drawing.Size(650, 344);
			this.tabPageEffectEditorModule.TabIndex = 4;
			this.tabPageEffectEditorModule.Text = "Effect Editor Module";
			this.tabPageEffectEditorModule.UseVisualStyleBackColor = true;
			// 
			// moduleListEffectEditor
			// 
			this.moduleListEffectEditor.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListEffectEditor.Location = new System.Drawing.Point(0, 0);
			this.moduleListEffectEditor.Name = "moduleListEffectEditor";
			this.moduleListEffectEditor.Size = new System.Drawing.Size(213, 344);
			this.moduleListEffectEditor.TabIndex = 0;
			// 
			// tabPageMediaModule
			// 
			this.tabPageMediaModule.Controls.Add(this.moduleListMedia);
			this.tabPageMediaModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageMediaModule.Name = "tabPageMediaModule";
			this.tabPageMediaModule.Size = new System.Drawing.Size(650, 344);
			this.tabPageMediaModule.TabIndex = 5;
			this.tabPageMediaModule.Text = "Media Module";
			this.tabPageMediaModule.UseVisualStyleBackColor = true;
			// 
			// moduleListMedia
			// 
			this.moduleListMedia.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListMedia.Location = new System.Drawing.Point(0, 0);
			this.moduleListMedia.Name = "moduleListMedia";
			this.moduleListMedia.Size = new System.Drawing.Size(213, 344);
			this.moduleListMedia.TabIndex = 0;
			// 
			// tabPageOutputModule
			// 
			this.tabPageOutputModule.Controls.Add(this.moduleListOutput);
			this.tabPageOutputModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageOutputModule.Name = "tabPageOutputModule";
			this.tabPageOutputModule.Size = new System.Drawing.Size(650, 344);
			this.tabPageOutputModule.TabIndex = 6;
			this.tabPageOutputModule.Text = "Output Module";
			this.tabPageOutputModule.UseVisualStyleBackColor = true;
			// 
			// moduleListOutput
			// 
			this.moduleListOutput.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListOutput.Location = new System.Drawing.Point(0, 0);
			this.moduleListOutput.Name = "moduleListOutput";
			this.moduleListOutput.Size = new System.Drawing.Size(213, 344);
			this.moduleListOutput.TabIndex = 0;
			// 
			// tabPagePropertyModule
			// 
			this.tabPagePropertyModule.Controls.Add(this.moduleListProperty);
			this.tabPagePropertyModule.Location = new System.Drawing.Point(4, 22);
			this.tabPagePropertyModule.Name = "tabPagePropertyModule";
			this.tabPagePropertyModule.Size = new System.Drawing.Size(650, 344);
			this.tabPagePropertyModule.TabIndex = 7;
			this.tabPagePropertyModule.Text = "Property Module";
			this.tabPagePropertyModule.UseVisualStyleBackColor = true;
			// 
			// moduleListProperty
			// 
			this.moduleListProperty.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListProperty.Location = new System.Drawing.Point(0, 0);
			this.moduleListProperty.Name = "moduleListProperty";
			this.moduleListProperty.Size = new System.Drawing.Size(213, 344);
			this.moduleListProperty.TabIndex = 0;
			// 
			// tabPageTimingModule
			// 
			this.tabPageTimingModule.Controls.Add(this.moduleListTiming);
			this.tabPageTimingModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageTimingModule.Name = "tabPageTimingModule";
			this.tabPageTimingModule.Size = new System.Drawing.Size(650, 344);
			this.tabPageTimingModule.TabIndex = 8;
			this.tabPageTimingModule.Text = "Timing Module";
			this.tabPageTimingModule.UseVisualStyleBackColor = true;
			// 
			// moduleListTiming
			// 
			this.moduleListTiming.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListTiming.Location = new System.Drawing.Point(0, 0);
			this.moduleListTiming.Name = "moduleListTiming";
			this.moduleListTiming.Size = new System.Drawing.Size(213, 344);
			this.moduleListTiming.TabIndex = 0;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(658, 370);
			this.Controls.Add(this.tabControl);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Vixen 3.0 Developer Testbed";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.tabControl.ResumeLayout(false);
			this.tabPageAdministration.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tabPageAppModule.ResumeLayout(false);
			this.tabPageEditorModule.ResumeLayout(false);
			this.tabPageEffectModule.ResumeLayout(false);
			this.tabPageEffectEditorModule.ResumeLayout(false);
			this.tabPageMediaModule.ResumeLayout(false);
			this.tabPageOutputModule.ResumeLayout(false);
			this.tabPagePropertyModule.ResumeLayout(false);
			this.tabPageTimingModule.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageAdministration;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button buttonControllers;
		private System.Windows.Forms.Button buttonPatching;
		private System.Windows.Forms.Button buttonChannels;
		private System.Windows.Forms.TabPage tabPageAppModule;
		private System.Windows.Forms.TabPage tabPageEditorModule;
		private System.Windows.Forms.TabPage tabPageEffectModule;
		private System.Windows.Forms.TabPage tabPageEffectEditorModule;
		private System.Windows.Forms.TabPage tabPageMediaModule;
		private System.Windows.Forms.TabPage tabPageOutputModule;
		private System.Windows.Forms.TabPage tabPagePropertyModule;
		private System.Windows.Forms.TabPage tabPageTimingModule;
		private ModuleList moduleListApp;
		private ModuleList moduleListEditor;
		private ModuleList moduleListEffect;
		private ModuleList moduleListEffectEditor;
		private ModuleList moduleListMedia;
		private ModuleList moduleListOutput;
		private ModuleList moduleListProperty;
		private ModuleList moduleListTiming;
	}
}

