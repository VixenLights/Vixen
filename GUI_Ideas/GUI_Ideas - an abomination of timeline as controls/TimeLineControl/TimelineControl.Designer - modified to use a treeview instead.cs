namespace Timeline
{
	partial class TimelineControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			Timeline.TimelineRowCollection timelineRowCollection1 = new Timeline.TimelineRowCollection();
			Timeline.TimelineRowCollection timelineRowCollection2 = new Timeline.TimelineRowCollection();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.timelineRowTreeView = new System.Windows.Forms.TreeView();
			this.timelineHeader = new Timeline.TimelineHeader();
			this.timelineGrid = new Timeline.TimelineGrid();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.timelineRowTreeView);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.timelineHeader);
			this.splitContainer.Panel2.Controls.Add(this.timelineGrid);
			this.splitContainer.Size = new System.Drawing.Size(853, 476);
			this.splitContainer.SplitterDistance = 283;
			this.splitContainer.TabIndex = 3;
			this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
			// 
			// timelineRowTreeView
			// 
			this.timelineRowTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.timelineRowTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.timelineRowTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
			this.timelineRowTreeView.Location = new System.Drawing.Point(0, 40);
			this.timelineRowTreeView.Name = "timelineRowTreeView";
			this.timelineRowTreeView.Size = new System.Drawing.Size(283, 436);
			this.timelineRowTreeView.TabIndex = 4;
			// 
			// timelineHeader
			// 
			this.timelineHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.timelineHeader.Location = new System.Drawing.Point(0, 0);
			this.timelineHeader.MajorTickInterval = System.TimeSpan.Parse("00:00:01");
			this.timelineHeader.MinorTicksPerMajor = 4;
			this.timelineHeader.Name = "timelineHeader";
			this.timelineHeader.Rows = timelineRowCollection1;
			this.timelineHeader.Size = new System.Drawing.Size(566, 40);
			this.timelineHeader.TabIndex = 3;
			this.timelineHeader.VisibleTimeSpan = System.TimeSpan.Parse("00:00:10");
			this.timelineHeader.VisibleTimeStart = System.TimeSpan.Parse("00:00:00");
			// 
			// timelineGrid
			// 
			this.timelineGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.timelineGrid.AutoScroll = true;
			this.timelineGrid.AutoScrollMinSize = new System.Drawing.Size(6792, 0);
			this.timelineGrid.AutoScrollOffset = new System.Drawing.Point(50, 50);
			this.timelineGrid.AutoSize = true;
			this.timelineGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
			this.timelineGrid.GridlineInterval = System.TimeSpan.Parse("00:00:01");
			this.timelineGrid.Location = new System.Drawing.Point(0, 40);
			this.timelineGrid.MajorGridlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
			this.timelineGrid.Name = "timelineGrid";
			this.timelineGrid.Rows = timelineRowCollection2;
			this.timelineGrid.RowSeparatorColor = System.Drawing.Color.Black;
			this.timelineGrid.Size = new System.Drawing.Size(566, 436);
			this.timelineGrid.TabIndex = 1;
			this.timelineGrid.TotalTime = System.TimeSpan.Parse("00:02:00");
			this.timelineGrid.VerticalOffset = 0;
			this.timelineGrid.VisibleTimeSpan = System.TimeSpan.Parse("00:00:10");
			this.timelineGrid.VisibleTimeStart = System.TimeSpan.Parse("00:00:00");
			this.timelineGrid.Load += new System.EventHandler(this.timelineGrid_Load);
			// 
			// TimelineControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.splitContainer);
			this.Name = "TimelineControl";
			this.Size = new System.Drawing.Size(853, 476);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer;
		private TimelineHeader timelineHeader;
		private TimelineGrid timelineGrid;
		private System.Windows.Forms.TreeView timelineRowTreeView;

	}
}
