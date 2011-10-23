namespace CommonElements.Timeline
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
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.timelineRowList = new CommonElements.Timeline.RowList();
			this.panelCorner = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
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
			this.splitContainer.Panel1.Controls.Add(this.panelCorner);
			this.splitContainer.Panel1.Controls.Add(this.timelineRowList);
			this.splitContainer.Size = new System.Drawing.Size(853, 476);
			this.splitContainer.SplitterDistance = 283;
			this.splitContainer.TabIndex = 3;
			// 
			// timelineRowList
			// 
			this.timelineRowList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.timelineRowList.DottedLineColor = System.Drawing.Color.Black;
			this.timelineRowList.Location = new System.Drawing.Point(0, 40);
			this.timelineRowList.Name = "timelineRowList";
			this.timelineRowList.Size = new System.Drawing.Size(283, 436);
			this.timelineRowList.TabIndex = 2;
			this.timelineRowList.VerticalOffset = 0;
			// 
			// panelCorner
			// 
			this.panelCorner.Location = new System.Drawing.Point(0, 0);
			this.panelCorner.Name = "panelCorner";
			this.panelCorner.Size = new System.Drawing.Size(283, 40);
			this.panelCorner.TabIndex = 3;
			this.panelCorner.BackColor = System.Drawing.Color.FromArgb(200, 200, 200);
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
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer;
		private RowList timelineRowList;
		private System.Windows.Forms.Panel panelCorner;
		

	}
}
