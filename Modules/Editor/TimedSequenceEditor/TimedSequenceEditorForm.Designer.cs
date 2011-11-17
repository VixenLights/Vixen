namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class TimedSequenceEditorForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.toolStripOperations = new CommonElements.ToolStripEx();
			this.toolStripButton_Start = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_Play = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_Stop = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_Pause = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_End = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.splitButton_Undo = new System.Windows.Forms.ToolStripSplitButton();
			this.splitButton_Redo = new System.Windows.Forms.ToolStripSplitButton();
			this.menuStrip = new CommonElements.MenuStripEx();
			this.sequenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_Save = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_Close = new System.Windows.Forms.ToolStripMenuItem();
			this.playbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_EditEffect = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_Cut = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_Copy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_Paste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllElementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_deleteElements = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_zoomTimeIn = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_zoomTimeOut = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_zoomRowsIn = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_zoomRowsOut = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_associateAudio = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MarkManager = new System.Windows.Forms.ToolStripMenuItem();
			this.addEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripEffects = new CommonElements.ToolStripEx();
			this.timerPlaying = new System.Windows.Forms.Timer(this.components);
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel_currentTime = new System.Windows.Forms.ToolStripStatusLabel();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.timelineControl = new CommonElements.Timeline.TimelineControl();
			this.toolStripOperations.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripOperations
			// 
			this.toolStripOperations.ClickThrough = true;
			this.toolStripOperations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Start,
            this.toolStripButton_Play,
            this.toolStripButton_Stop,
            this.toolStripButton_Pause,
            this.toolStripButton_End,
            this.toolStripSeparator4,
            this.splitButton_Undo,
            this.splitButton_Redo});
			this.toolStripOperations.Location = new System.Drawing.Point(0, 24);
			this.toolStripOperations.Name = "toolStripOperations";
			this.toolStripOperations.Size = new System.Drawing.Size(886, 25);
			this.toolStripOperations.TabIndex = 1;
			this.toolStripOperations.Text = "Operations";
			// 
			// toolStripButton_Start
			// 
			this.toolStripButton_Start.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_Start.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.MoveFirstHS;
			this.toolStripButton_Start.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_Start.Name = "toolStripButton_Start";
			this.toolStripButton_Start.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_Start.Text = "Start";
			this.toolStripButton_Start.Click += new System.EventHandler(this.toolStripButton_Start_Click);
			// 
			// toolStripButton_Play
			// 
			this.toolStripButton_Play.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_Play.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.PlayHS;
			this.toolStripButton_Play.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_Play.Name = "toolStripButton_Play";
			this.toolStripButton_Play.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_Play.Text = "Play";
			this.toolStripButton_Play.Click += new System.EventHandler(this.toolStripButton_Play_Click);
			// 
			// toolStripButton_Stop
			// 
			this.toolStripButton_Stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_Stop.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.StopHS;
			this.toolStripButton_Stop.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_Stop.Name = "toolStripButton_Stop";
			this.toolStripButton_Stop.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_Stop.Text = "Stop";
			this.toolStripButton_Stop.Click += new System.EventHandler(this.toolStripButton_Stop_Click);
			// 
			// toolStripButton_Pause
			// 
			this.toolStripButton_Pause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_Pause.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.PauseHS;
			this.toolStripButton_Pause.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_Pause.Name = "toolStripButton_Pause";
			this.toolStripButton_Pause.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_Pause.Text = "Pause";
			this.toolStripButton_Pause.Click += new System.EventHandler(this.toolStripButton_Pause_Click);
			// 
			// toolStripButton_End
			// 
			this.toolStripButton_End.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_End.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.MoveLastHS;
			this.toolStripButton_End.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton_End.Name = "toolStripButton_End";
			this.toolStripButton_End.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_End.Text = "End";
			this.toolStripButton_End.Click += new System.EventHandler(this.toolStripButton_End_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// splitButton_Undo
			// 
			this.splitButton_Undo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.splitButton_Undo.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.Edit_UndoHS;
			this.splitButton_Undo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.splitButton_Undo.Name = "splitButton_Undo";
			this.splitButton_Undo.Size = new System.Drawing.Size(32, 22);
			this.splitButton_Undo.Text = "Undo";
			this.splitButton_Undo.ButtonClick += new System.EventHandler(this.splitButton_Undo_ButtonClick);
			// 
			// splitButton_Redo
			// 
			this.splitButton_Redo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.splitButton_Redo.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.Edit_RedoHS;
			this.splitButton_Redo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.splitButton_Redo.Name = "splitButton_Redo";
			this.splitButton_Redo.Size = new System.Drawing.Size(32, 22);
			this.splitButton_Redo.Text = "Redo";
			this.splitButton_Redo.ButtonClick += new System.EventHandler(this.splitButton_Redo_ButtonClick);
			// 
			// menuStrip
			// 
			this.menuStrip.ClickThrough = true;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sequenceToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.addEffectToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(886, 24);
			this.menuStrip.TabIndex = 2;
			this.menuStrip.Text = "Menu";
			// 
			// sequenceToolStripMenuItem
			// 
			this.sequenceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Save,
            this.toolStripMenuItem_SaveAs,
            this.toolStripSeparator1,
            this.toolStripMenuItem_Close,
            this.playbackToolStripMenuItem});
			this.sequenceToolStripMenuItem.Name = "sequenceToolStripMenuItem";
			this.sequenceToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
			this.sequenceToolStripMenuItem.Text = "Sequence";
			// 
			// toolStripMenuItem_Save
			// 
			this.toolStripMenuItem_Save.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.saveHS;
			this.toolStripMenuItem_Save.Name = "toolStripMenuItem_Save";
			this.toolStripMenuItem_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.toolStripMenuItem_Save.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem_Save.Text = "Save";
			this.toolStripMenuItem_Save.Click += new System.EventHandler(this.toolStripMenuItem_Save_Click);
			// 
			// toolStripMenuItem_SaveAs
			// 
			this.toolStripMenuItem_SaveAs.Name = "toolStripMenuItem_SaveAs";
			this.toolStripMenuItem_SaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
						| System.Windows.Forms.Keys.S)));
			this.toolStripMenuItem_SaveAs.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem_SaveAs.Text = "Save As...";
			this.toolStripMenuItem_SaveAs.Click += new System.EventHandler(this.toolStripMenuItem_SaveAs_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
			// 
			// toolStripMenuItem_Close
			// 
			this.toolStripMenuItem_Close.Name = "toolStripMenuItem_Close";
			this.toolStripMenuItem_Close.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
			this.toolStripMenuItem_Close.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem_Close.Text = "Close";
			this.toolStripMenuItem_Close.Click += new System.EventHandler(this.toolStripMenuItem_Close_Click);
			// 
			// playbackToolStripMenuItem
			// 
			this.playbackToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.stopToolStripMenuItem});
			this.playbackToolStripMenuItem.Name = "playbackToolStripMenuItem";
			this.playbackToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.playbackToolStripMenuItem.Text = "Playback";
			// 
			// playToolStripMenuItem
			// 
			this.playToolStripMenuItem.Name = "playToolStripMenuItem";
			this.playToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.playToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.playToolStripMenuItem.Text = "Play";
			this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
			// 
			// pauseToolStripMenuItem
			// 
			this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
			this.pauseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
			this.pauseToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.pauseToolStripMenuItem.Text = "Pause";
			this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
			// 
			// stopToolStripMenuItem
			// 
			this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
			this.stopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.stopToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.stopToolStripMenuItem.Text = "Stop";
			this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_EditEffect,
            this.toolStripSeparator2,
            this.toolStripMenuItem_Cut,
            this.toolStripMenuItem_Copy,
            this.toolStripMenuItem_Paste,
            this.toolStripSeparator3,
            this.selectAllElementsToolStripMenuItem,
            this.toolStripMenuItem_deleteElements});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// toolStripMenuItem_EditEffect
			// 
			this.toolStripMenuItem_EditEffect.Name = "toolStripMenuItem_EditEffect";
			this.toolStripMenuItem_EditEffect.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.toolStripMenuItem_EditEffect.Size = new System.Drawing.Size(215, 22);
			this.toolStripMenuItem_EditEffect.Text = "Edit Effect...";
			this.toolStripMenuItem_EditEffect.Click += new System.EventHandler(this.toolStripMenuItem_EditEffect_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(212, 6);
			// 
			// toolStripMenuItem_Cut
			// 
			this.toolStripMenuItem_Cut.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.CutHS;
			this.toolStripMenuItem_Cut.Name = "toolStripMenuItem_Cut";
			this.toolStripMenuItem_Cut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.toolStripMenuItem_Cut.Size = new System.Drawing.Size(215, 22);
			this.toolStripMenuItem_Cut.Text = "Cut";
			this.toolStripMenuItem_Cut.Click += new System.EventHandler(this.toolStripMenuItem_Cut_Click);
			// 
			// toolStripMenuItem_Copy
			// 
			this.toolStripMenuItem_Copy.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.CopyHS;
			this.toolStripMenuItem_Copy.Name = "toolStripMenuItem_Copy";
			this.toolStripMenuItem_Copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItem_Copy.Size = new System.Drawing.Size(215, 22);
			this.toolStripMenuItem_Copy.Text = "Copy";
			this.toolStripMenuItem_Copy.Click += new System.EventHandler(this.toolStripMenuItem_Copy_Click);
			// 
			// toolStripMenuItem_Paste
			// 
			this.toolStripMenuItem_Paste.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.PasteHS;
			this.toolStripMenuItem_Paste.Name = "toolStripMenuItem_Paste";
			this.toolStripMenuItem_Paste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.toolStripMenuItem_Paste.Size = new System.Drawing.Size(215, 22);
			this.toolStripMenuItem_Paste.Text = "Paste";
			this.toolStripMenuItem_Paste.Click += new System.EventHandler(this.toolStripMenuItem_Paste_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(212, 6);
			// 
			// selectAllElementsToolStripMenuItem
			// 
			this.selectAllElementsToolStripMenuItem.Name = "selectAllElementsToolStripMenuItem";
			this.selectAllElementsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.selectAllElementsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
			this.selectAllElementsToolStripMenuItem.Text = "Select All Elements";
			this.selectAllElementsToolStripMenuItem.Click += new System.EventHandler(this.selectAllElementsToolStripMenuItem_Click);
			// 
			// toolStripMenuItem_deleteElements
			// 
			this.toolStripMenuItem_deleteElements.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.DeleteHS;
			this.toolStripMenuItem_deleteElements.Name = "toolStripMenuItem_deleteElements";
			this.toolStripMenuItem_deleteElements.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.toolStripMenuItem_deleteElements.Size = new System.Drawing.Size(215, 22);
			this.toolStripMenuItem_deleteElements.Text = "Delete Element(s)";
			this.toolStripMenuItem_deleteElements.Click += new System.EventHandler(this.toolStripMenuItem_deleteElements_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_zoomTimeIn,
            this.toolStripMenuItem_zoomTimeOut,
            this.toolStripMenuItem_zoomRowsIn,
            this.toolStripMenuItem_zoomRowsOut,
            this.toolStripSeparator5});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// toolStripMenuItem_zoomTimeIn
			// 
			this.toolStripMenuItem_zoomTimeIn.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.Zoom_In;
			this.toolStripMenuItem_zoomTimeIn.Name = "toolStripMenuItem_zoomTimeIn";
			this.toolStripMenuItem_zoomTimeIn.ShortcutKeyDisplayString = "Ctrl+ +";
			this.toolStripMenuItem_zoomTimeIn.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Add)));
			this.toolStripMenuItem_zoomTimeIn.Size = new System.Drawing.Size(234, 22);
			this.toolStripMenuItem_zoomTimeIn.Text = "Zoom Time In";
			this.toolStripMenuItem_zoomTimeIn.Click += new System.EventHandler(this.toolStripMenuItem_zoomTimeIn_Click);
			// 
			// toolStripMenuItem_zoomTimeOut
			// 
			this.toolStripMenuItem_zoomTimeOut.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.Zoom_Out;
			this.toolStripMenuItem_zoomTimeOut.Name = "toolStripMenuItem_zoomTimeOut";
			this.toolStripMenuItem_zoomTimeOut.ShortcutKeyDisplayString = "Ctrl+ -";
			this.toolStripMenuItem_zoomTimeOut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Subtract)));
			this.toolStripMenuItem_zoomTimeOut.Size = new System.Drawing.Size(234, 22);
			this.toolStripMenuItem_zoomTimeOut.Text = "Zoom Time Out";
			this.toolStripMenuItem_zoomTimeOut.Click += new System.EventHandler(this.toolStripMenuItem_zoomTimeOut_Click);
			// 
			// toolStripMenuItem_zoomRowsIn
			// 
			this.toolStripMenuItem_zoomRowsIn.Name = "toolStripMenuItem_zoomRowsIn";
			this.toolStripMenuItem_zoomRowsIn.ShortcutKeyDisplayString = "Ctrl+Shift+ +";
			this.toolStripMenuItem_zoomRowsIn.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.Add)));
			this.toolStripMenuItem_zoomRowsIn.Size = new System.Drawing.Size(234, 22);
			this.toolStripMenuItem_zoomRowsIn.Text = "Zoom Rows In";
			this.toolStripMenuItem_zoomRowsIn.Click += new System.EventHandler(this.toolStripMenuItem_zoomRowsIn_Click);
			// 
			// toolStripMenuItem_zoomRowsOut
			// 
			this.toolStripMenuItem_zoomRowsOut.Name = "toolStripMenuItem_zoomRowsOut";
			this.toolStripMenuItem_zoomRowsOut.ShortcutKeyDisplayString = "Ctrl+Shift+ -";
			this.toolStripMenuItem_zoomRowsOut.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.Subtract)));
			this.toolStripMenuItem_zoomRowsOut.Size = new System.Drawing.Size(234, 22);
			this.toolStripMenuItem_zoomRowsOut.Text = "Zoom Rows Out";
			this.toolStripMenuItem_zoomRowsOut.Click += new System.EventHandler(this.toolStripMenuItem_zoomRowsOut_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(231, 6);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_associateAudio,
            this.toolStripMenuItem_MarkManager});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// toolStripMenuItem_associateAudio
			// 
			this.toolStripMenuItem_associateAudio.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.base_speaker_32;
			this.toolStripMenuItem_associateAudio.Name = "toolStripMenuItem_associateAudio";
			this.toolStripMenuItem_associateAudio.Size = new System.Drawing.Size(168, 22);
			this.toolStripMenuItem_associateAudio.Text = "Associate Audio...";
			this.toolStripMenuItem_associateAudio.Click += new System.EventHandler(this.toolStripMenuItem_associateAudio_Click);
			// 
			// toolStripMenuItem_MarkManager
			// 
			this.toolStripMenuItem_MarkManager.Image = global::VixenModules.Editor.TimedSequenceEditor.TimedSequenceEditorResources.pencil_32;
			this.toolStripMenuItem_MarkManager.Name = "toolStripMenuItem_MarkManager";
			this.toolStripMenuItem_MarkManager.Size = new System.Drawing.Size(168, 22);
			this.toolStripMenuItem_MarkManager.Text = "Mark Manager...";
			this.toolStripMenuItem_MarkManager.Click += new System.EventHandler(this.toolStripMenuItem_MarkManager_Click);
			// 
			// addEffectToolStripMenuItem
			// 
			this.addEffectToolStripMenuItem.Name = "addEffectToolStripMenuItem";
			this.addEffectToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
			this.addEffectToolStripMenuItem.Text = "Add Effect";
			// 
			// toolStripEffects
			// 
			this.toolStripEffects.ClickThrough = true;
			this.toolStripEffects.Location = new System.Drawing.Point(0, 49);
			this.toolStripEffects.Name = "toolStripEffects";
			this.toolStripEffects.Size = new System.Drawing.Size(886, 25);
			this.toolStripEffects.TabIndex = 3;
			this.toolStripEffects.Text = "Effects";
			// 
			// timerPlaying
			// 
			this.timerPlaying.Interval = 40;
			this.timerPlaying.Tick += new System.EventHandler(this.timerPlaying_Tick);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_currentTime});
			this.statusStrip.Location = new System.Drawing.Point(0, 616);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(886, 26);
			this.statusStrip.TabIndex = 4;
			this.statusStrip.Text = "statusStrip1";
			// 
			// toolStripStatusLabel_currentTime
			// 
			this.toolStripStatusLabel_currentTime.AutoSize = false;
			this.toolStripStatusLabel_currentTime.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.toolStripStatusLabel_currentTime.BorderStyle = System.Windows.Forms.Border3DStyle.Bump;
			this.toolStripStatusLabel_currentTime.Name = "toolStripStatusLabel_currentTime";
			this.toolStripStatusLabel_currentTime.Size = new System.Drawing.Size(60, 21);
			this.toolStripStatusLabel_currentTime.Text = "0:00.00";
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "All files (*.*)|*.*";
			// 
			// timelineControl
			// 
			this.timelineControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.timelineControl.AutoSize = true;
			this.timelineControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.timelineControl.Location = new System.Drawing.Point(0, 77);
			this.timelineControl.Name = "timelineControl";
			this.timelineControl.SelectedRow = null;
			this.timelineControl.Size = new System.Drawing.Size(887, 541);
			this.timelineControl.TabIndex = 0;
			// 
			// TimedSequenceEditorForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(886, 642);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.toolStripEffects);
			this.Controls.Add(this.toolStripOperations);
			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.timelineControl);
			this.DoubleBuffered = true;
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip;
			this.Name = "TimedSequenceEditorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Timed Sequence Editor";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TimedSequenceEditorForm_FormClosed);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TimedSequenceEditorForm_KeyDown);
			this.Resize += new System.EventHandler(this.TimedSequenceEditorForm_Resize);
			this.toolStripOperations.ResumeLayout(false);
			this.toolStripOperations.PerformLayout();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CommonElements.Timeline.TimelineControl timelineControl;
		private CommonElements.ToolStripEx toolStripOperations;
		private CommonElements.MenuStripEx menuStrip;
		private System.Windows.Forms.ToolStripMenuItem sequenceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Save;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private CommonElements.ToolStripEx toolStripEffects;
		private System.Windows.Forms.ToolStripMenuItem addEffectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SaveAs;
		private System.Windows.Forms.ToolStripButton toolStripButton_Play;
		private System.Windows.Forms.ToolStripButton toolStripButton_Stop;
		private System.Windows.Forms.ToolStripButton toolStripButton_Pause;
		private System.Windows.Forms.Timer timerPlaying;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_currentTime;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Close;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_associateAudio;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MarkManager;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_EditEffect;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Cut;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Copy;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Paste;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_zoomTimeIn;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_zoomTimeOut;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_zoomRowsIn;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_zoomRowsOut;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_deleteElements;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSplitButton splitButton_Undo;
        private System.Windows.Forms.ToolStripSplitButton splitButton_Redo;
		private System.Windows.Forms.ToolStripButton toolStripButton_Start;
		private System.Windows.Forms.ToolStripButton toolStripButton_End;
		private System.Windows.Forms.ToolStripMenuItem selectAllElementsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem playbackToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
	}
}