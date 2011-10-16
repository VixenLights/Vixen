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
			this.components = new System.ComponentModel.Container();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageAdministration = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.buttonControllers = new System.Windows.Forms.Button();
			this.buttonPatching = new System.Windows.Forms.Button();
			this.buttonChannels = new System.Windows.Forms.Button();
			this.tabPageAppModule = new System.Windows.Forms.TabPage();
			this.moduleListApp = new VixenTestbed.ModuleList();
			this.tabPageEditorModule = new System.Windows.Forms.TabPage();
			this.buttonShowEditor = new System.Windows.Forms.Button();
			this.buttonLoadSequence = new System.Windows.Forms.Button();
			this.moduleListEditor = new VixenTestbed.ModuleList();
			this.tabPageEffectModule = new System.Windows.Forms.TabPage();
			this.pictureBoxEffectImage = new System.Windows.Forms.PictureBox();
			this.numericUpDownEffectRenderTimeSpan = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonRenderEffect = new System.Windows.Forms.Button();
			this.checkedListBoxEffectTargetNodes = new System.Windows.Forms.CheckedListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.moduleListEffect = new VixenTestbed.ModuleList();
			this.tabPageMediaModule = new System.Windows.Forms.TabPage();
			this.labelLoadedMedia = new System.Windows.Forms.Label();
			this.groupBoxMediaExecution = new System.Windows.Forms.GroupBox();
			this.buttonStopMedia = new System.Windows.Forms.Button();
			this.buttonResumeMedia = new System.Windows.Forms.Button();
			this.buttonPauseMedia = new System.Windows.Forms.Button();
			this.buttonPlayMedia = new System.Windows.Forms.Button();
			this.buttonLoadMediaFile = new System.Windows.Forms.Button();
			this.buttonSetupMedia = new System.Windows.Forms.Button();
			this.moduleListMedia = new VixenTestbed.ModuleList();
			this.tabPageTimingModule = new System.Windows.Forms.TabPage();
			this.labelTimingCurrentPosition = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBoxTimingExecution = new System.Windows.Forms.GroupBox();
			this.buttonStopTiming = new System.Windows.Forms.Button();
			this.buttonResumeTiming = new System.Windows.Forms.Button();
			this.buttonPauseTiming = new System.Windows.Forms.Button();
			this.buttonPlayTiming = new System.Windows.Forms.Button();
			this.moduleListTiming = new VixenTestbed.ModuleList();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.timingTimer = new System.Windows.Forms.Timer(this.components);
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.tabControl.SuspendLayout();
			this.tabPageAdministration.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tabPageAppModule.SuspendLayout();
			this.tabPageEditorModule.SuspendLayout();
			this.tabPageEffectModule.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxEffectImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownEffectRenderTimeSpan)).BeginInit();
			this.tabPageMediaModule.SuspendLayout();
			this.groupBoxMediaExecution.SuspendLayout();
			this.tabPageTimingModule.SuspendLayout();
			this.groupBoxTimingExecution.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageAdministration);
			this.tabControl.Controls.Add(this.tabPageAppModule);
			this.tabControl.Controls.Add(this.tabPageEditorModule);
			this.tabControl.Controls.Add(this.tabPageEffectModule);
			this.tabControl.Controls.Add(this.tabPageMediaModule);
			this.tabControl.Controls.Add(this.tabPageTimingModule);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 24);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(658, 346);
			this.tabControl.TabIndex = 0;
			// 
			// tabPageAdministration
			// 
			this.tabPageAdministration.Controls.Add(this.tableLayoutPanel1);
			this.tabPageAdministration.Location = new System.Drawing.Point(4, 22);
			this.tabPageAdministration.Name = "tabPageAdministration";
			this.tabPageAdministration.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAdministration.Size = new System.Drawing.Size(650, 320);
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
			this.tableLayoutPanel1.Size = new System.Drawing.Size(644, 314);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// buttonControllers
			// 
			this.buttonControllers.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonControllers.Location = new System.Drawing.Point(267, 34);
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
			this.buttonPatching.Location = new System.Drawing.Point(267, 243);
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
			this.buttonChannels.Location = new System.Drawing.Point(267, 138);
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
			this.tabPageAppModule.Size = new System.Drawing.Size(650, 320);
			this.tabPageAppModule.TabIndex = 1;
			this.tabPageAppModule.Text = "App Module";
			this.tabPageAppModule.UseVisualStyleBackColor = true;
			// 
			// moduleListApp
			// 
			this.moduleListApp.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListApp.Location = new System.Drawing.Point(3, 3);
			this.moduleListApp.Name = "moduleListApp";
			this.moduleListApp.Size = new System.Drawing.Size(213, 314);
			this.moduleListApp.TabIndex = 1;
			// 
			// tabPageEditorModule
			// 
			this.tabPageEditorModule.Controls.Add(this.buttonShowEditor);
			this.tabPageEditorModule.Controls.Add(this.buttonLoadSequence);
			this.tabPageEditorModule.Controls.Add(this.moduleListEditor);
			this.tabPageEditorModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageEditorModule.Name = "tabPageEditorModule";
			this.tabPageEditorModule.Size = new System.Drawing.Size(650, 320);
			this.tabPageEditorModule.TabIndex = 2;
			this.tabPageEditorModule.Text = "Editor Module";
			this.tabPageEditorModule.UseVisualStyleBackColor = true;
			// 
			// buttonShowEditor
			// 
			this.buttonShowEditor.Enabled = false;
			this.buttonShowEditor.Location = new System.Drawing.Point(256, 68);
			this.buttonShowEditor.Name = "buttonShowEditor";
			this.buttonShowEditor.Size = new System.Drawing.Size(127, 23);
			this.buttonShowEditor.TabIndex = 6;
			this.buttonShowEditor.Text = "Show Editor";
			this.buttonShowEditor.UseVisualStyleBackColor = true;
			this.buttonShowEditor.Click += new System.EventHandler(this.buttonShowEditor_Click);
			// 
			// buttonLoadSequence
			// 
			this.buttonLoadSequence.Enabled = false;
			this.buttonLoadSequence.Location = new System.Drawing.Point(256, 29);
			this.buttonLoadSequence.Name = "buttonLoadSequence";
			this.buttonLoadSequence.Size = new System.Drawing.Size(127, 23);
			this.buttonLoadSequence.TabIndex = 5;
			this.buttonLoadSequence.Text = "Load a Sequence";
			this.buttonLoadSequence.UseVisualStyleBackColor = true;
			this.buttonLoadSequence.Click += new System.EventHandler(this.buttonLoadEditorSequence_Click);
			// 
			// moduleListEditor
			// 
			this.moduleListEditor.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListEditor.Location = new System.Drawing.Point(0, 0);
			this.moduleListEditor.Name = "moduleListEditor";
			this.moduleListEditor.Size = new System.Drawing.Size(213, 320);
			this.moduleListEditor.TabIndex = 2;
			this.moduleListEditor.SelectedModuleChanged += new System.EventHandler(this.moduleListEditor_SelectedModuleChanged);
			// 
			// tabPageEffectModule
			// 
			this.tabPageEffectModule.Controls.Add(this.pictureBoxEffectImage);
			this.tabPageEffectModule.Controls.Add(this.numericUpDownEffectRenderTimeSpan);
			this.tabPageEffectModule.Controls.Add(this.label5);
			this.tabPageEffectModule.Controls.Add(this.buttonRenderEffect);
			this.tabPageEffectModule.Controls.Add(this.checkedListBoxEffectTargetNodes);
			this.tabPageEffectModule.Controls.Add(this.label2);
			this.tabPageEffectModule.Controls.Add(this.moduleListEffect);
			this.tabPageEffectModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageEffectModule.Name = "tabPageEffectModule";
			this.tabPageEffectModule.Size = new System.Drawing.Size(650, 320);
			this.tabPageEffectModule.TabIndex = 3;
			this.tabPageEffectModule.Text = "Effect Module";
			this.tabPageEffectModule.UseVisualStyleBackColor = true;
			// 
			// pictureBoxEffectImage
			// 
			this.pictureBoxEffectImage.Location = new System.Drawing.Point(442, 65);
			this.pictureBoxEffectImage.Name = "pictureBoxEffectImage";
			this.pictureBoxEffectImage.Size = new System.Drawing.Size(48, 48);
			this.pictureBoxEffectImage.TabIndex = 7;
			this.pictureBoxEffectImage.TabStop = false;
			// 
			// numericUpDownEffectRenderTimeSpan
			// 
			this.numericUpDownEffectRenderTimeSpan.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericUpDownEffectRenderTimeSpan.Location = new System.Drawing.Point(569, 32);
			this.numericUpDownEffectRenderTimeSpan.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			this.numericUpDownEffectRenderTimeSpan.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.numericUpDownEffectRenderTimeSpan.Name = "numericUpDownEffectRenderTimeSpan";
			this.numericUpDownEffectRenderTimeSpan.Size = new System.Drawing.Size(66, 20);
			this.numericUpDownEffectRenderTimeSpan.TabIndex = 5;
			this.numericUpDownEffectRenderTimeSpan.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(439, 34);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(124, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Time span (milliseconds):";
			// 
			// buttonRenderEffect
			// 
			this.buttonRenderEffect.Enabled = false;
			this.buttonRenderEffect.Location = new System.Drawing.Point(505, 76);
			this.buttonRenderEffect.Name = "buttonRenderEffect";
			this.buttonRenderEffect.Size = new System.Drawing.Size(75, 23);
			this.buttonRenderEffect.TabIndex = 6;
			this.buttonRenderEffect.Text = "Render";
			this.buttonRenderEffect.UseVisualStyleBackColor = true;
			this.buttonRenderEffect.Click += new System.EventHandler(this.buttonRenderEffect_Click);
			// 
			// checkedListBoxEffectTargetNodes
			// 
			this.checkedListBoxEffectTargetNodes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.checkedListBoxEffectTargetNodes.CheckOnClick = true;
			this.checkedListBoxEffectTargetNodes.FormattingEnabled = true;
			this.checkedListBoxEffectTargetNodes.Location = new System.Drawing.Point(241, 31);
			this.checkedListBoxEffectTargetNodes.Name = "checkedListBoxEffectTargetNodes";
			this.checkedListBoxEffectTargetNodes.Size = new System.Drawing.Size(178, 289);
			this.checkedListBoxEffectTargetNodes.TabIndex = 3;
			this.checkedListBoxEffectTargetNodes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxEffectTargetNodes_ItemCheck);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(238, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Nodes to render to:";
			// 
			// moduleListEffect
			// 
			this.moduleListEffect.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListEffect.Location = new System.Drawing.Point(0, 0);
			this.moduleListEffect.Name = "moduleListEffect";
			this.moduleListEffect.Size = new System.Drawing.Size(213, 320);
			this.moduleListEffect.TabIndex = 0;
			this.moduleListEffect.SelectedModuleChanged += new System.EventHandler(this.moduleListEffect_SelectedModuleChanged);
			// 
			// tabPageMediaModule
			// 
			this.tabPageMediaModule.Controls.Add(this.labelLoadedMedia);
			this.tabPageMediaModule.Controls.Add(this.groupBoxMediaExecution);
			this.tabPageMediaModule.Controls.Add(this.buttonLoadMediaFile);
			this.tabPageMediaModule.Controls.Add(this.buttonSetupMedia);
			this.tabPageMediaModule.Controls.Add(this.moduleListMedia);
			this.tabPageMediaModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageMediaModule.Name = "tabPageMediaModule";
			this.tabPageMediaModule.Size = new System.Drawing.Size(650, 320);
			this.tabPageMediaModule.TabIndex = 5;
			this.tabPageMediaModule.Text = "Media Module";
			this.tabPageMediaModule.UseVisualStyleBackColor = true;
			// 
			// labelLoadedMedia
			// 
			this.labelLoadedMedia.AutoSize = true;
			this.labelLoadedMedia.Location = new System.Drawing.Point(392, 89);
			this.labelLoadedMedia.Name = "labelLoadedMedia";
			this.labelLoadedMedia.Size = new System.Drawing.Size(79, 13);
			this.labelLoadedMedia.TabIndex = 14;
			this.labelLoadedMedia.Text = "Nothing loaded";
			// 
			// groupBoxMediaExecution
			// 
			this.groupBoxMediaExecution.Controls.Add(this.buttonStopMedia);
			this.groupBoxMediaExecution.Controls.Add(this.buttonResumeMedia);
			this.groupBoxMediaExecution.Controls.Add(this.buttonPauseMedia);
			this.groupBoxMediaExecution.Controls.Add(this.buttonPlayMedia);
			this.groupBoxMediaExecution.Enabled = false;
			this.groupBoxMediaExecution.Location = new System.Drawing.Point(256, 112);
			this.groupBoxMediaExecution.Name = "groupBoxMediaExecution";
			this.groupBoxMediaExecution.Size = new System.Drawing.Size(292, 45);
			this.groupBoxMediaExecution.TabIndex = 12;
			this.groupBoxMediaExecution.TabStop = false;
			// 
			// buttonStopMedia
			// 
			this.buttonStopMedia.Location = new System.Drawing.Point(220, 13);
			this.buttonStopMedia.Name = "buttonStopMedia";
			this.buttonStopMedia.Size = new System.Drawing.Size(55, 23);
			this.buttonStopMedia.TabIndex = 11;
			this.buttonStopMedia.Text = "Stop";
			this.buttonStopMedia.UseVisualStyleBackColor = true;
			this.buttonStopMedia.Click += new System.EventHandler(this.buttonStopMedia_Click);
			// 
			// buttonResumeMedia
			// 
			this.buttonResumeMedia.Location = new System.Drawing.Point(149, 13);
			this.buttonResumeMedia.Name = "buttonResumeMedia";
			this.buttonResumeMedia.Size = new System.Drawing.Size(55, 23);
			this.buttonResumeMedia.TabIndex = 10;
			this.buttonResumeMedia.Text = "Resume";
			this.buttonResumeMedia.UseVisualStyleBackColor = true;
			this.buttonResumeMedia.Click += new System.EventHandler(this.buttonResumeMedia_Click);
			// 
			// buttonPauseMedia
			// 
			this.buttonPauseMedia.Location = new System.Drawing.Point(88, 13);
			this.buttonPauseMedia.Name = "buttonPauseMedia";
			this.buttonPauseMedia.Size = new System.Drawing.Size(55, 23);
			this.buttonPauseMedia.TabIndex = 9;
			this.buttonPauseMedia.Text = "Pause";
			this.buttonPauseMedia.UseVisualStyleBackColor = true;
			this.buttonPauseMedia.Click += new System.EventHandler(this.buttonPauseMedia_Click);
			// 
			// buttonPlayMedia
			// 
			this.buttonPlayMedia.Location = new System.Drawing.Point(16, 13);
			this.buttonPlayMedia.Name = "buttonPlayMedia";
			this.buttonPlayMedia.Size = new System.Drawing.Size(55, 23);
			this.buttonPlayMedia.TabIndex = 8;
			this.buttonPlayMedia.Text = "Play";
			this.buttonPlayMedia.UseVisualStyleBackColor = true;
			this.buttonPlayMedia.Click += new System.EventHandler(this.buttonPlayMedia_Click);
			// 
			// buttonLoadMediaFile
			// 
			this.buttonLoadMediaFile.Location = new System.Drawing.Point(256, 83);
			this.buttonLoadMediaFile.Name = "buttonLoadMediaFile";
			this.buttonLoadMediaFile.Size = new System.Drawing.Size(127, 23);
			this.buttonLoadMediaFile.TabIndex = 7;
			this.buttonLoadMediaFile.Text = "Load Media File";
			this.buttonLoadMediaFile.UseVisualStyleBackColor = true;
			this.buttonLoadMediaFile.Click += new System.EventHandler(this.buttonLoadMediaFile_Click);
			// 
			// buttonSetupMedia
			// 
			this.buttonSetupMedia.Enabled = false;
			this.buttonSetupMedia.Location = new System.Drawing.Point(256, 29);
			this.buttonSetupMedia.Name = "buttonSetupMedia";
			this.buttonSetupMedia.Size = new System.Drawing.Size(75, 23);
			this.buttonSetupMedia.TabIndex = 6;
			this.buttonSetupMedia.Text = "Setup";
			this.buttonSetupMedia.UseVisualStyleBackColor = true;
			this.buttonSetupMedia.Click += new System.EventHandler(this.buttonSetupMedia_Click);
			// 
			// moduleListMedia
			// 
			this.moduleListMedia.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListMedia.Location = new System.Drawing.Point(0, 0);
			this.moduleListMedia.Name = "moduleListMedia";
			this.moduleListMedia.Size = new System.Drawing.Size(213, 320);
			this.moduleListMedia.TabIndex = 0;
			this.moduleListMedia.SelectedModuleChanged += new System.EventHandler(this.moduleListMedia_SelectedModuleChanged);
			// 
			// tabPageTimingModule
			// 
			this.tabPageTimingModule.Controls.Add(this.labelTimingCurrentPosition);
			this.tabPageTimingModule.Controls.Add(this.label4);
			this.tabPageTimingModule.Controls.Add(this.groupBoxTimingExecution);
			this.tabPageTimingModule.Controls.Add(this.moduleListTiming);
			this.tabPageTimingModule.Location = new System.Drawing.Point(4, 22);
			this.tabPageTimingModule.Name = "tabPageTimingModule";
			this.tabPageTimingModule.Size = new System.Drawing.Size(650, 320);
			this.tabPageTimingModule.TabIndex = 8;
			this.tabPageTimingModule.Text = "Timing Module";
			this.tabPageTimingModule.UseVisualStyleBackColor = true;
			// 
			// labelTimingCurrentPosition
			// 
			this.labelTimingCurrentPosition.AutoSize = true;
			this.labelTimingCurrentPosition.Location = new System.Drawing.Point(349, 101);
			this.labelTimingCurrentPosition.Name = "labelTimingCurrentPosition";
			this.labelTimingCurrentPosition.Size = new System.Drawing.Size(70, 13);
			this.labelTimingCurrentPosition.TabIndex = 16;
			this.labelTimingCurrentPosition.Text = "00:00:00.000";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(260, 101);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(83, 13);
			this.label4.TabIndex = 15;
			this.label4.Text = "Current position:";
			// 
			// groupBoxTimingExecution
			// 
			this.groupBoxTimingExecution.Controls.Add(this.buttonStopTiming);
			this.groupBoxTimingExecution.Controls.Add(this.buttonResumeTiming);
			this.groupBoxTimingExecution.Controls.Add(this.buttonPauseTiming);
			this.groupBoxTimingExecution.Controls.Add(this.buttonPlayTiming);
			this.groupBoxTimingExecution.Enabled = false;
			this.groupBoxTimingExecution.Location = new System.Drawing.Point(256, 29);
			this.groupBoxTimingExecution.Name = "groupBoxTimingExecution";
			this.groupBoxTimingExecution.Size = new System.Drawing.Size(292, 45);
			this.groupBoxTimingExecution.TabIndex = 14;
			this.groupBoxTimingExecution.TabStop = false;
			// 
			// buttonStopTiming
			// 
			this.buttonStopTiming.Location = new System.Drawing.Point(220, 13);
			this.buttonStopTiming.Name = "buttonStopTiming";
			this.buttonStopTiming.Size = new System.Drawing.Size(55, 23);
			this.buttonStopTiming.TabIndex = 11;
			this.buttonStopTiming.Text = "Stop";
			this.buttonStopTiming.UseVisualStyleBackColor = true;
			this.buttonStopTiming.Click += new System.EventHandler(this.buttonStopTiming_Click);
			// 
			// buttonResumeTiming
			// 
			this.buttonResumeTiming.Location = new System.Drawing.Point(149, 13);
			this.buttonResumeTiming.Name = "buttonResumeTiming";
			this.buttonResumeTiming.Size = new System.Drawing.Size(55, 23);
			this.buttonResumeTiming.TabIndex = 10;
			this.buttonResumeTiming.Text = "Resume";
			this.buttonResumeTiming.UseVisualStyleBackColor = true;
			this.buttonResumeTiming.Click += new System.EventHandler(this.buttonResumeTiming_Click);
			// 
			// buttonPauseTiming
			// 
			this.buttonPauseTiming.Location = new System.Drawing.Point(88, 13);
			this.buttonPauseTiming.Name = "buttonPauseTiming";
			this.buttonPauseTiming.Size = new System.Drawing.Size(55, 23);
			this.buttonPauseTiming.TabIndex = 9;
			this.buttonPauseTiming.Text = "Pause";
			this.buttonPauseTiming.UseVisualStyleBackColor = true;
			this.buttonPauseTiming.Click += new System.EventHandler(this.buttonPauseTiming_Click);
			// 
			// buttonPlayTiming
			// 
			this.buttonPlayTiming.Location = new System.Drawing.Point(16, 13);
			this.buttonPlayTiming.Name = "buttonPlayTiming";
			this.buttonPlayTiming.Size = new System.Drawing.Size(55, 23);
			this.buttonPlayTiming.TabIndex = 8;
			this.buttonPlayTiming.Text = "Play";
			this.buttonPlayTiming.UseVisualStyleBackColor = true;
			this.buttonPlayTiming.Click += new System.EventHandler(this.buttonPlayTiming_Click);
			// 
			// moduleListTiming
			// 
			this.moduleListTiming.Dock = System.Windows.Forms.DockStyle.Left;
			this.moduleListTiming.Location = new System.Drawing.Point(0, 0);
			this.moduleListTiming.Name = "moduleListTiming";
			this.moduleListTiming.Size = new System.Drawing.Size(213, 320);
			this.moduleListTiming.TabIndex = 0;
			this.moduleListTiming.SelectedModuleChanged += new System.EventHandler(this.moduleListTiming_SelectedModuleChanged);
			// 
			// menuStrip
			// 
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(658, 24);
			this.menuStrip.TabIndex = 1;
			// 
			// openFileDialog
			// 
			this.openFileDialog.SupportMultiDottedExtensions = true;
			// 
			// timingTimer
			// 
			this.timingTimer.Interval = 200;
			this.timingTimer.Tick += new System.EventHandler(this.timingTimer_Tick);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(658, 370);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
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
			this.tabPageEffectModule.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxEffectImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownEffectRenderTimeSpan)).EndInit();
			this.tabPageMediaModule.ResumeLayout(false);
			this.tabPageMediaModule.PerformLayout();
			this.groupBoxMediaExecution.ResumeLayout(false);
			this.tabPageTimingModule.ResumeLayout(false);
			this.tabPageTimingModule.PerformLayout();
			this.groupBoxTimingExecution.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

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
		private System.Windows.Forms.TabPage tabPageMediaModule;
		private System.Windows.Forms.TabPage tabPageTimingModule;
		private ModuleList moduleListApp;
		private ModuleList moduleListEditor;
		private ModuleList moduleListEffect;
		private ModuleList moduleListMedia;
		private ModuleList moduleListTiming;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.Button buttonLoadSequence;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button buttonShowEditor;
		private System.Windows.Forms.Button buttonStopMedia;
		private System.Windows.Forms.Button buttonResumeMedia;
		private System.Windows.Forms.Button buttonPauseMedia;
		private System.Windows.Forms.Button buttonPlayMedia;
		private System.Windows.Forms.Button buttonLoadMediaFile;
		private System.Windows.Forms.Button buttonSetupMedia;
		private System.Windows.Forms.GroupBox groupBoxMediaExecution;
		private System.Windows.Forms.GroupBox groupBoxTimingExecution;
		private System.Windows.Forms.Button buttonStopTiming;
		private System.Windows.Forms.Button buttonResumeTiming;
		private System.Windows.Forms.Button buttonPauseTiming;
		private System.Windows.Forms.Button buttonPlayTiming;
		private System.Windows.Forms.Label labelTimingCurrentPosition;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Timer timingTimer;
		private System.Windows.Forms.Button buttonRenderEffect;
		private System.Windows.Forms.CheckedListBox checkedListBoxEffectTargetNodes;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownEffectRenderTimeSpan;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelLoadedMedia;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.PictureBox pictureBoxEffectImage;
	}
}

