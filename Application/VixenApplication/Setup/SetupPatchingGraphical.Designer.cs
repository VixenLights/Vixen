namespace VixenApplication.Setup
{
	partial class SetupPatchingGraphical
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupPatchingGraphical));
			Dataweb.NShape.RoleBasedSecurityManager roleBasedSecurityManager4 = new Dataweb.NShape.RoleBasedSecurityManager();
			this.diagramDisplay = new Dataweb.NShape.WinFormsUI.Display();
			this.diagramSetController = new Dataweb.NShape.Controllers.DiagramSetController();
			this.project = new Dataweb.NShape.Project(this.components);
			this.cachedRepository = new Dataweb.NShape.Advanced.CachedRepository();
			this.copyFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteFilterMultipleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.diagramContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.buttonDeleteFilter = new System.Windows.Forms.Button();
			this.buttonAddFilter = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.buttonZoomIn = new System.Windows.Forms.Button();
			this.buttonZoomOut = new System.Windows.Forms.Button();
			this.buttonZoomFit = new System.Windows.Forms.Button();
			this.diagramContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// diagramDisplay
			// 
			this.diagramDisplay.AllowDrop = true;
			this.diagramDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.diagramDisplay.BackColor = System.Drawing.Color.Transparent;
			this.diagramDisplay.BackColorGradient = System.Drawing.SystemColors.Control;
			this.diagramDisplay.BackgroundGradientAngle = 90;
			this.diagramDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.diagramDisplay.DiagramSetController = this.diagramSetController;
			this.diagramDisplay.GridColor = System.Drawing.Color.Gainsboro;
			this.diagramDisplay.GridSize = 19;
			this.diagramDisplay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.diagramDisplay.Location = new System.Drawing.Point(2, 2);
			this.diagramDisplay.Name = "diagramDisplay";
			this.diagramDisplay.PropertyController = null;
			this.diagramDisplay.RenderingQualityLowQuality = Dataweb.NShape.Advanced.RenderingQuality.LowQuality;
			this.diagramDisplay.SelectionHilightColor = System.Drawing.Color.Firebrick;
			this.diagramDisplay.SelectionInactiveColor = System.Drawing.Color.Gray;
			this.diagramDisplay.SelectionInteriorColor = System.Drawing.Color.WhiteSmoke;
			this.diagramDisplay.SelectionNormalColor = System.Drawing.Color.DarkGreen;
			this.diagramDisplay.ShowDefaultContextMenu = false;
			this.diagramDisplay.Size = new System.Drawing.Size(446, 511);
			this.diagramDisplay.SnapToGrid = false;
			this.diagramDisplay.TabIndex = 1;
			this.diagramDisplay.ToolPreviewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
			this.diagramDisplay.ToolPreviewColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
			this.diagramDisplay.ShapesSelected += new System.EventHandler(this.diagramDisplay_ShapesSelected);
			this.diagramDisplay.ShapeDoubleClick += new System.EventHandler<Dataweb.NShape.Controllers.DiagramPresenterShapeClickEventArgs>(this.displayDiagram_ShapeDoubleClick);
			this.diagramDisplay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.diagramDisplay_KeyDown);
			// 
			// diagramSetController
			// 
			this.diagramSetController.ActiveTool = null;
			this.diagramSetController.Project = this.project;
			// 
			// project
			// 
			this.project.LibrarySearchPaths = ((System.Collections.Generic.IList<string>)(resources.GetObject("project.LibrarySearchPaths")));
			this.project.Name = null;
			this.project.Repository = this.cachedRepository;
			roleBasedSecurityManager4.CurrentRole = Dataweb.NShape.StandardRole.Administrator;
			roleBasedSecurityManager4.CurrentRoleName = "Administrator";
			this.project.SecurityManager = roleBasedSecurityManager4;
			// 
			// cachedRepository
			// 
			this.cachedRepository.ProjectName = null;
			this.cachedRepository.Store = null;
			this.cachedRepository.Version = 0;
			// 
			// copyFilterToolStripMenuItem
			// 
			this.copyFilterToolStripMenuItem.Name = "copyFilterToolStripMenuItem";
			this.copyFilterToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.copyFilterToolStripMenuItem.Text = "Copy Filter";
			this.copyFilterToolStripMenuItem.Click += new System.EventHandler(this.copyFilterToolStripMenuItem_Click);
			// 
			// pasteFilterToolStripMenuItem
			// 
			this.pasteFilterToolStripMenuItem.Name = "pasteFilterToolStripMenuItem";
			this.pasteFilterToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.pasteFilterToolStripMenuItem.Text = "Paste Filter";
			this.pasteFilterToolStripMenuItem.Click += new System.EventHandler(this.pasteFilterToolStripMenuItem_Click);
			// 
			// pasteFilterMultipleToolStripMenuItem
			// 
			this.pasteFilterMultipleToolStripMenuItem.Name = "pasteFilterMultipleToolStripMenuItem";
			this.pasteFilterMultipleToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.pasteFilterMultipleToolStripMenuItem.Text = "Paste Filter (Multiple)";
			this.pasteFilterMultipleToolStripMenuItem.Click += new System.EventHandler(this.pasteFilterMultipleToolStripMenuItem_Click);
			// 
			// diagramContextMenuStrip
			// 
			this.diagramContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyFilterToolStripMenuItem,
            this.pasteFilterToolStripMenuItem,
            this.pasteFilterMultipleToolStripMenuItem});
			this.diagramContextMenuStrip.Name = "diagramContextMenuStrip";
			this.diagramContextMenuStrip.Size = new System.Drawing.Size(187, 70);
			this.diagramContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.diagramContextMenuStrip_Opening);
			// 
			// buttonDeleteFilter
			// 
			this.buttonDeleteFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDeleteFilter.BackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteFilter.FlatAppearance.BorderSize = 0;
			this.buttonDeleteFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDeleteFilter.Location = new System.Drawing.Point(33, 519);
			this.buttonDeleteFilter.Name = "buttonDeleteFilter";
			this.buttonDeleteFilter.Size = new System.Drawing.Size(24, 24);
			this.buttonDeleteFilter.TabIndex = 43;
			this.buttonDeleteFilter.Text = "-";
			this.toolTip1.SetToolTip(this.buttonDeleteFilter, "Delete Filter");
			this.buttonDeleteFilter.UseVisualStyleBackColor = false;
			this.buttonDeleteFilter.Click += new System.EventHandler(this.buttonDeleteFilter_Click);
			// 
			// buttonAddFilter
			// 
			this.buttonAddFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAddFilter.BackColor = System.Drawing.Color.Transparent;
			this.buttonAddFilter.FlatAppearance.BorderSize = 0;
			this.buttonAddFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddFilter.Location = new System.Drawing.Point(3, 519);
			this.buttonAddFilter.Name = "buttonAddFilter";
			this.buttonAddFilter.Size = new System.Drawing.Size(24, 24);
			this.buttonAddFilter.TabIndex = 44;
			this.buttonAddFilter.Text = "+";
			this.toolTip1.SetToolTip(this.buttonAddFilter, "Add Filter");
			this.buttonAddFilter.UseVisualStyleBackColor = false;
			this.buttonAddFilter.Click += new System.EventHandler(this.buttonAddFilter_Click);
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 200;
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 200;
			this.toolTip1.ReshowDelay = 40;
			// 
			// buttonZoomIn
			// 
			this.buttonZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonZoomIn.BackColor = System.Drawing.Color.Transparent;
			this.buttonZoomIn.FlatAppearance.BorderSize = 0;
			this.buttonZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonZoomIn.Location = new System.Drawing.Point(87, 519);
			this.buttonZoomIn.Name = "buttonZoomIn";
			this.buttonZoomIn.Size = new System.Drawing.Size(24, 24);
			this.buttonZoomIn.TabIndex = 45;
			this.buttonZoomIn.Text = "Z";
			this.toolTip1.SetToolTip(this.buttonZoomIn, "Zoom In");
			this.buttonZoomIn.UseVisualStyleBackColor = false;
			this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
			// 
			// buttonZoomOut
			// 
			this.buttonZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonZoomOut.BackColor = System.Drawing.Color.Transparent;
			this.buttonZoomOut.FlatAppearance.BorderSize = 0;
			this.buttonZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonZoomOut.Location = new System.Drawing.Point(117, 519);
			this.buttonZoomOut.Name = "buttonZoomOut";
			this.buttonZoomOut.Size = new System.Drawing.Size(24, 24);
			this.buttonZoomOut.TabIndex = 46;
			this.buttonZoomOut.Text = "Z";
			this.toolTip1.SetToolTip(this.buttonZoomOut, "Zoom Out");
			this.buttonZoomOut.UseVisualStyleBackColor = false;
			this.buttonZoomOut.Click += new System.EventHandler(this.buttonZoomOut_Click);
			// 
			// buttonZoomFit
			// 
			this.buttonZoomFit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonZoomFit.BackColor = System.Drawing.Color.Transparent;
			this.buttonZoomFit.FlatAppearance.BorderSize = 0;
			this.buttonZoomFit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonZoomFit.Location = new System.Drawing.Point(147, 519);
			this.buttonZoomFit.Name = "buttonZoomFit";
			this.buttonZoomFit.Size = new System.Drawing.Size(24, 24);
			this.buttonZoomFit.TabIndex = 47;
			this.buttonZoomFit.Text = "Z";
			this.toolTip1.SetToolTip(this.buttonZoomFit, "Zoom to Fit");
			this.buttonZoomFit.UseVisualStyleBackColor = false;
			this.buttonZoomFit.Click += new System.EventHandler(this.buttonZoomFit_Click);
			// 
			// SetupPatchingGraphical
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonZoomFit);
			this.Controls.Add(this.buttonZoomOut);
			this.Controls.Add(this.buttonZoomIn);
			this.Controls.Add(this.buttonAddFilter);
			this.Controls.Add(this.buttonDeleteFilter);
			this.Controls.Add(this.diagramDisplay);
			this.DoubleBuffered = true;
			this.Name = "SetupPatchingGraphical";
			this.Size = new System.Drawing.Size(450, 550);
			this.Load += new System.EventHandler(this.SetupPatchingGraphical_Load);
			this.Resize += new System.EventHandler(this.SetupPatchingGraphical_Resize);
			this.diagramContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Dataweb.NShape.WinFormsUI.Display diagramDisplay;
		private System.Windows.Forms.ContextMenuStrip diagramContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem copyFilterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteFilterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteFilterMultipleToolStripMenuItem;
		private Dataweb.NShape.Project project;
		private Dataweb.NShape.Advanced.CachedRepository cachedRepository;
		private Dataweb.NShape.Controllers.DiagramSetController diagramSetController;
		private System.Windows.Forms.Button buttonDeleteFilter;
		private System.Windows.Forms.Button buttonAddFilter;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonZoomIn;
		private System.Windows.Forms.Button buttonZoomOut;
		private System.Windows.Forms.Button buttonZoomFit;
	}
}
