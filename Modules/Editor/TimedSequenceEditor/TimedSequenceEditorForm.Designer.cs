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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimedSequenceEditorForm));
			this.toolStripOperations = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.sequenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_Save = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_Close = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripEffects = new System.Windows.Forms.ToolStrip();
			this.timelineControl = new CommonElements.Timeline.TimelineControl();
			this.toolStripOperations.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripOperations
			// 
			this.toolStripOperations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
			this.toolStripOperations.Location = new System.Drawing.Point(0, 24);
			this.toolStripOperations.Name = "toolStripOperations";
			this.toolStripOperations.Size = new System.Drawing.Size(1088, 25);
			this.toolStripOperations.TabIndex = 1;
			this.toolStripOperations.Text = "Operations";
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton1.Text = "toolStripButton1";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sequenceToolStripMenuItem,
            this.editToolStripMenuItem,
            this.addEffectToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(1088, 24);
			this.menuStrip.TabIndex = 2;
			this.menuStrip.Text = "Menu";
			// 
			// sequenceToolStripMenuItem
			// 
			this.sequenceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Save,
            this.toolStripMenuItem_SaveAs,
            this.toolStripMenuItem_Close});
			this.sequenceToolStripMenuItem.Name = "sequenceToolStripMenuItem";
			this.sequenceToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
			this.sequenceToolStripMenuItem.Text = "Sequence";
			// 
			// toolStripMenuItem_Save
			// 
			this.toolStripMenuItem_Save.Name = "toolStripMenuItem_Save";
			this.toolStripMenuItem_Save.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem_Save.Text = "Save";
			this.toolStripMenuItem_Save.Click += new System.EventHandler(this.toolStripMenuItem_Save_Click);
			// 
			// toolStripMenuItem_SaveAs
			// 
			this.toolStripMenuItem_SaveAs.Name = "toolStripMenuItem_SaveAs";
			this.toolStripMenuItem_SaveAs.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem_SaveAs.Text = "Save As...";
			this.toolStripMenuItem_SaveAs.Click += new System.EventHandler(this.toolStripMenuItem_SaveAs_Click);
			// 
			// toolStripMenuItem_Close
			// 
			this.toolStripMenuItem_Close.Name = "toolStripMenuItem_Close";
			this.toolStripMenuItem_Close.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem_Close.Text = "Close";
			this.toolStripMenuItem_Close.Click += new System.EventHandler(this.toolStripMenuItem_Close_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// addEffectToolStripMenuItem
			// 
			this.addEffectToolStripMenuItem.Name = "addEffectToolStripMenuItem";
			this.addEffectToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
			this.addEffectToolStripMenuItem.Text = "Add Effect";
			// 
			// toolStripEffects
			// 
			this.toolStripEffects.Location = new System.Drawing.Point(0, 49);
			this.toolStripEffects.Name = "toolStripEffects";
			this.toolStripEffects.Size = new System.Drawing.Size(1088, 25);
			this.toolStripEffects.TabIndex = 3;
			this.toolStripEffects.Text = "Effects";
			// 
			// timelineControl
			// 
			this.timelineControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.timelineControl.AutoSize = true;
			this.timelineControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.timelineControl.Location = new System.Drawing.Point(12, 77);
			this.timelineControl.Name = "timelineControl";
			this.timelineControl.Size = new System.Drawing.Size(1064, 661);
			this.timelineControl.TabIndex = 0;
			this.timelineControl.TimePerPixel = System.TimeSpan.Parse("00:00:00.0100000");
			this.timelineControl.TotalTime = System.TimeSpan.Parse("00:02:00");
			this.timelineControl.VerticalOffset = 0;
			this.timelineControl.VisibleTimeEnd = System.TimeSpan.Parse("00:00:07.7300000");
			this.timelineControl.VisibleTimeStart = System.TimeSpan.Parse("00:00:00");
			// 
			// TimedSequenceEditorForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1088, 750);
			this.Controls.Add(this.toolStripEffects);
			this.Controls.Add(this.toolStripOperations);
			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.timelineControl);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.menuStrip;
			this.Name = "TimedSequenceEditorForm";
			this.Text = "Timed Sequence Editor";
			this.toolStripOperations.ResumeLayout(false);
			this.toolStripOperations.PerformLayout();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CommonElements.Timeline.TimelineControl timelineControl;
		private System.Windows.Forms.ToolStrip toolStripOperations;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem sequenceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Save;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStrip toolStripEffects;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Close;
		private System.Windows.Forms.ToolStripMenuItem addEffectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SaveAs;
	}
}