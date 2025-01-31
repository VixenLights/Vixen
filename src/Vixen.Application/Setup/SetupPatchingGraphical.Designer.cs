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
			if (disposing && (components != null))
			{
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupPatchingGraphical));
			Dataweb.NShape.RoleBasedSecurityManager roleBasedSecurityManager1 = new Dataweb.NShape.RoleBasedSecurityManager();
			diagramDisplay = new Dataweb.NShape.WinFormsUI.Display();
			diagramSetController = new Dataweb.NShape.Controllers.DiagramSetController();
			project = new Dataweb.NShape.Project(components);
			cachedRepository = new Dataweb.NShape.Advanced.CachedRepository();
			copyFilterToolStripMenuItem = new ToolStripMenuItem();
			pasteFilterToolStripMenuItem = new ToolStripMenuItem();
			pasteFilterMultipleToolStripMenuItem = new ToolStripMenuItem();
			diagramContextMenuStrip = new ContextMenuStrip(components);
			buttonDeleteFilter = new Button();
			buttonAddFilter = new Button();
			toolTip1 = new ToolTip(components);
			buttonZoomIn = new Button();
			buttonZoomOut = new Button();
			buttonZoomFit = new Button();
			tableLayoutPanel1 = new TableLayoutPanel();
			flowLayoutPanelButton = new FlowLayoutPanel();
			diagramContextMenuStrip.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			flowLayoutPanelButton.SuspendLayout();
			SuspendLayout();
			// 
			// diagramDisplay
			// 
			diagramDisplay.AllowDrop = true;
			diagramDisplay.AutoSize = true;
			diagramDisplay.BackColor = Color.Transparent;
			diagramDisplay.BackColorGradient = SystemColors.Control;
			diagramDisplay.BackgroundGradientAngle = 90;
			diagramDisplay.BorderStyle = BorderStyle.Fixed3D;
			diagramDisplay.DiagramSetController = diagramSetController;
			diagramDisplay.Dock = DockStyle.Fill;
			diagramDisplay.GridColor = Color.Gainsboro;
			diagramDisplay.GridSize = 19;
			diagramDisplay.ImeMode = ImeMode.NoControl;
			diagramDisplay.Location = new Point(4, 3);
			diagramDisplay.Margin = new Padding(4, 3, 4, 3);
			diagramDisplay.Name = "diagramDisplay";
			diagramDisplay.PropertyController = null;
			diagramDisplay.RenderingQualityLowQuality = Dataweb.NShape.Advanced.RenderingQuality.LowQuality;
			diagramDisplay.SelectionHilightColor = Color.Firebrick;
			diagramDisplay.SelectionInactiveColor = Color.Gray;
			diagramDisplay.SelectionInteriorColor = Color.WhiteSmoke;
			diagramDisplay.SelectionNormalColor = Color.DarkGreen;
			diagramDisplay.ShowDefaultContextMenu = false;
			diagramDisplay.Size = new Size(517, 589);
			diagramDisplay.SnapToGrid = false;
			diagramDisplay.TabIndex = 1;
			diagramDisplay.ToolPreviewBackColor = Color.FromArgb(64, 119, 136, 153);
			diagramDisplay.ToolPreviewColor = Color.FromArgb(96, 70, 130, 180);
			diagramDisplay.ShapesSelected += diagramDisplay_ShapesSelected;
			diagramDisplay.ShapeDoubleClick += displayDiagram_ShapeDoubleClick;
			diagramDisplay.KeyDown += diagramDisplay_KeyDown;
			// 
			// diagramSetController
			// 
			diagramSetController.ActiveTool = null;
			diagramSetController.Project = project;
			// 
			// project
			// 
			project.LibrarySearchPaths = (IList<string>)resources.GetObject("project.LibrarySearchPaths");
			project.Name = null;
			project.Repository = cachedRepository;
			roleBasedSecurityManager1.CurrentRole = Dataweb.NShape.StandardRole.Administrator;
			roleBasedSecurityManager1.CurrentRoleName = "Administrator";
			project.SecurityManager = roleBasedSecurityManager1;
			// 
			// cachedRepository
			// 
			cachedRepository.ProjectName = null;
			cachedRepository.Store = null;
			cachedRepository.Version = 0;
			// 
			// copyFilterToolStripMenuItem
			// 
			copyFilterToolStripMenuItem.Name = "copyFilterToolStripMenuItem";
			copyFilterToolStripMenuItem.Size = new Size(186, 22);
			copyFilterToolStripMenuItem.Text = "Copy Filter";
			copyFilterToolStripMenuItem.Click += copyFilterToolStripMenuItem_Click;
			// 
			// pasteFilterToolStripMenuItem
			// 
			pasteFilterToolStripMenuItem.Name = "pasteFilterToolStripMenuItem";
			pasteFilterToolStripMenuItem.Size = new Size(186, 22);
			pasteFilterToolStripMenuItem.Text = "Paste Filter";
			pasteFilterToolStripMenuItem.Click += pasteFilterToolStripMenuItem_Click;
			// 
			// pasteFilterMultipleToolStripMenuItem
			// 
			pasteFilterMultipleToolStripMenuItem.Name = "pasteFilterMultipleToolStripMenuItem";
			pasteFilterMultipleToolStripMenuItem.Size = new Size(186, 22);
			pasteFilterMultipleToolStripMenuItem.Text = "Paste Filter (Multiple)";
			pasteFilterMultipleToolStripMenuItem.Click += pasteFilterMultipleToolStripMenuItem_Click;
			// 
			// diagramContextMenuStrip
			// 
			diagramContextMenuStrip.Items.AddRange(new ToolStripItem[] { copyFilterToolStripMenuItem, pasteFilterToolStripMenuItem, pasteFilterMultipleToolStripMenuItem });
			diagramContextMenuStrip.Name = "diagramContextMenuStrip";
			diagramContextMenuStrip.Size = new Size(187, 70);
			diagramContextMenuStrip.Opening += diagramContextMenuStrip_Opening;
			// 
			// buttonDeleteFilter
			// 
			buttonDeleteFilter.AutoSize = true;
			buttonDeleteFilter.Location = new Point(40, 3);
			buttonDeleteFilter.Margin = new Padding(4, 3, 10, 3);
			buttonDeleteFilter.Name = "buttonDeleteFilter";
			buttonDeleteFilter.Size = new Size(28, 28);
			buttonDeleteFilter.TabIndex = 43;
			toolTip1.SetToolTip(buttonDeleteFilter, "Delete Filter");
			buttonDeleteFilter.UseVisualStyleBackColor = false;
			buttonDeleteFilter.Click += buttonDeleteFilter_Click;
			// 
			// buttonAddFilter
			// 
			buttonAddFilter.AutoSize = true;
			buttonAddFilter.Location = new Point(4, 3);
			buttonAddFilter.Margin = new Padding(4, 3, 4, 3);
			buttonAddFilter.Name = "buttonAddFilter";
			buttonAddFilter.Size = new Size(28, 28);
			buttonAddFilter.TabIndex = 44;
			toolTip1.SetToolTip(buttonAddFilter, "Add Filter");
			buttonAddFilter.UseVisualStyleBackColor = false;
			buttonAddFilter.Click += buttonAddFilter_Click;
			// 
			// toolTip1
			// 
			toolTip1.AutomaticDelay = 200;
			toolTip1.AutoPopDelay = 5000;
			toolTip1.InitialDelay = 200;
			toolTip1.ReshowDelay = 40;
			// 
			// buttonZoomIn
			// 
			buttonZoomIn.AutoSize = true;
			buttonZoomIn.Location = new Point(82, 3);
			buttonZoomIn.Margin = new Padding(4, 3, 4, 3);
			buttonZoomIn.Name = "buttonZoomIn";
			buttonZoomIn.Size = new Size(28, 28);
			buttonZoomIn.TabIndex = 45;
			toolTip1.SetToolTip(buttonZoomIn, "Zoom In");
			buttonZoomIn.UseVisualStyleBackColor = false;
			buttonZoomIn.Click += buttonZoomIn_Click;
			// 
			// buttonZoomOut
			// 
			buttonZoomOut.AutoSize = true;
			buttonZoomOut.Location = new Point(118, 3);
			buttonZoomOut.Margin = new Padding(4, 3, 4, 3);
			buttonZoomOut.Name = "buttonZoomOut";
			buttonZoomOut.Size = new Size(28, 28);
			buttonZoomOut.TabIndex = 46;
			toolTip1.SetToolTip(buttonZoomOut, "Zoom Out");
			buttonZoomOut.UseVisualStyleBackColor = false;
			buttonZoomOut.Click += buttonZoomOut_Click;
			// 
			// buttonZoomFit
			// 
			buttonZoomFit.AutoSize = true;
			buttonZoomFit.Location = new Point(154, 3);
			buttonZoomFit.Margin = new Padding(4, 3, 4, 3);
			buttonZoomFit.Name = "buttonZoomFit";
			buttonZoomFit.Size = new Size(28, 28);
			buttonZoomFit.TabIndex = 47;
			toolTip1.SetToolTip(buttonZoomFit, "Zoom to Fit");
			buttonZoomFit.UseVisualStyleBackColor = false;
			buttonZoomFit.Click += buttonZoomFit_Click;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(diagramDisplay, 0, 0);
			tableLayoutPanel1.Controls.Add(flowLayoutPanelButton, 0, 1);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 2;
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.Size = new Size(525, 635);
			tableLayoutPanel1.TabIndex = 48;
			// 
			// flowLayoutPanelButton
			// 
			flowLayoutPanelButton.AutoSize = true;
			flowLayoutPanelButton.Controls.Add(buttonAddFilter);
			flowLayoutPanelButton.Controls.Add(buttonDeleteFilter);
			flowLayoutPanelButton.Controls.Add(buttonZoomIn);
			flowLayoutPanelButton.Controls.Add(buttonZoomOut);
			flowLayoutPanelButton.Controls.Add(buttonZoomFit);
			flowLayoutPanelButton.Dock = DockStyle.Fill;
			flowLayoutPanelButton.Location = new Point(3, 598);
			flowLayoutPanelButton.Name = "flowLayoutPanelButton";
			flowLayoutPanelButton.Size = new Size(519, 34);
			flowLayoutPanelButton.TabIndex = 2;
			// 
			// SetupPatchingGraphical
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(tableLayoutPanel1);
			DoubleBuffered = true;
			Margin = new Padding(4, 3, 4, 3);
			Name = "SetupPatchingGraphical";
			Size = new Size(525, 635);
			Load += SetupPatchingGraphical_Load;
			Resize += SetupPatchingGraphical_Resize;
			diagramContextMenuStrip.ResumeLayout(false);
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			flowLayoutPanelButton.ResumeLayout(false);
			flowLayoutPanelButton.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Dataweb.NShape.WinFormsUI.Display diagramDisplay;
		private ContextMenuStrip diagramContextMenuStrip;
		private ToolStripMenuItem copyFilterToolStripMenuItem;
		private ToolStripMenuItem pasteFilterToolStripMenuItem;
		private ToolStripMenuItem pasteFilterMultipleToolStripMenuItem;
		private Dataweb.NShape.Project project;
		private Dataweb.NShape.Advanced.CachedRepository cachedRepository;
		private Dataweb.NShape.Controllers.DiagramSetController diagramSetController;
		private Button buttonDeleteFilter;
		private Button buttonAddFilter;
		private ToolTip toolTip1;
		private Button buttonZoomIn;
		private Button buttonZoomOut;
		private Button buttonZoomFit;
		private TableLayoutPanel tableLayoutPanel1;
		private FlowLayoutPanel flowLayoutPanelButton;
	}
}
