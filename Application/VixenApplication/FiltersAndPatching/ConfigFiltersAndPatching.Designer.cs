namespace VixenApplication
{
	partial class ConfigFiltersAndPatching
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigFiltersAndPatching));
			Dataweb.NShape.RoleBasedSecurityManager roleBasedSecurityManager4 = new Dataweb.NShape.RoleBasedSecurityManager();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.diagramContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteFilterMultipleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.diagramSetController = new Dataweb.NShape.Controllers.DiagramSetController();
			this.project = new Dataweb.NShape.Project(this.components);
			this.cachedRepository = new Dataweb.NShape.Advanced.CachedRepository();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonPatchWizard = new System.Windows.Forms.Button();
			this.diagramDisplay = new Dataweb.NShape.WinFormsUI.Display();
			this.buttonZoomIn = new System.Windows.Forms.Button();
			this.buttonZoomOut = new System.Windows.Forms.Button();
			this.groupBoxFilters = new System.Windows.Forms.GroupBox();
			this.comboBoxNewFilterTypes = new System.Windows.Forms.ComboBox();
			this.buttonAddFilter = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.diagramContextMenuStrip.SuspendLayout();
			this.groupBoxFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(682, 425);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
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
			this.cachedRepository.ShapesUpdated += new System.EventHandler<Dataweb.NShape.RepositoryShapesEventArgs>(this.cachedRepository_ShapesUpdated);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(568, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Double-click to collapse/expand groups and controllers, or to configure filters. " +
    "Right-click and drag to connect modules.";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(586, 425);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(90, 25);
			this.buttonOK.TabIndex = 9;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonPatchWizard
			// 
			this.buttonPatchWizard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonPatchWizard.Location = new System.Drawing.Point(279, 409);
			this.buttonPatchWizard.Name = "buttonPatchWizard";
			this.buttonPatchWizard.Size = new System.Drawing.Size(100, 25);
			this.buttonPatchWizard.TabIndex = 13;
			this.buttonPatchWizard.Text = "Patching Wizard";
			this.buttonPatchWizard.UseVisualStyleBackColor = true;
			this.buttonPatchWizard.Click += new System.EventHandler(this.buttonPatchWizard_Click);
			// 
			// diagramDisplay
			// 
			this.diagramDisplay.AllowDrop = true;
			this.diagramDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.diagramDisplay.BackColorGradient = System.Drawing.SystemColors.Control;
			this.diagramDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.diagramDisplay.ContextMenuStrip = this.diagramContextMenuStrip;
			this.diagramDisplay.DiagramSetController = this.diagramSetController;
			this.diagramDisplay.GridColor = System.Drawing.Color.Gainsboro;
			this.diagramDisplay.GridSize = 19;
			this.diagramDisplay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.diagramDisplay.Location = new System.Drawing.Point(12, 29);
			this.diagramDisplay.Name = "diagramDisplay";
			this.diagramDisplay.PropertyController = null;
			this.diagramDisplay.SelectionHilightColor = System.Drawing.Color.Firebrick;
			this.diagramDisplay.SelectionInactiveColor = System.Drawing.Color.Gray;
			this.diagramDisplay.SelectionInteriorColor = System.Drawing.Color.WhiteSmoke;
			this.diagramDisplay.SelectionNormalColor = System.Drawing.Color.DarkGreen;
			this.diagramDisplay.ShowDefaultContextMenu = false;
			this.diagramDisplay.Size = new System.Drawing.Size(760, 350);
			this.diagramDisplay.SnapToGrid = false;
			this.diagramDisplay.TabIndex = 0;
			this.diagramDisplay.ToolPreviewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
			this.diagramDisplay.ToolPreviewColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
			this.diagramDisplay.ShapeDoubleClick += new System.EventHandler<Dataweb.NShape.Controllers.DiagramPresenterShapeClickEventArgs>(this.displayDiagram_ShapeDoubleClick);
			this.diagramDisplay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.diagramDisplay_KeyDown);
			// 
			// buttonZoomIn
			// 
			this.buttonZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonZoomIn.Image = global::VixenApplication.Properties.Resources.ZoomIn32;
			this.buttonZoomIn.Location = new System.Drawing.Point(403, 401);
			this.buttonZoomIn.Name = "buttonZoomIn";
			this.buttonZoomIn.Size = new System.Drawing.Size(40, 40);
			this.buttonZoomIn.TabIndex = 11;
			this.buttonZoomIn.UseVisualStyleBackColor = true;
			this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
			// 
			// buttonZoomOut
			// 
			this.buttonZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonZoomOut.Image = global::VixenApplication.Properties.Resources.ZoomOut32;
			this.buttonZoomOut.Location = new System.Drawing.Point(453, 401);
			this.buttonZoomOut.Name = "buttonZoomOut";
			this.buttonZoomOut.Size = new System.Drawing.Size(40, 40);
			this.buttonZoomOut.TabIndex = 10;
			this.buttonZoomOut.UseVisualStyleBackColor = true;
			this.buttonZoomOut.Click += new System.EventHandler(this.buttonZoomOut_Click);
			// 
			// groupBoxFilters
			// 
			this.groupBoxFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBoxFilters.Controls.Add(this.comboBoxNewFilterTypes);
			this.groupBoxFilters.Controls.Add(this.buttonAddFilter);
			this.groupBoxFilters.Controls.Add(this.buttonDelete);
			this.groupBoxFilters.Location = new System.Drawing.Point(23, 385);
			this.groupBoxFilters.Name = "groupBoxFilters";
			this.groupBoxFilters.Size = new System.Drawing.Size(232, 65);
			this.groupBoxFilters.TabIndex = 14;
			this.groupBoxFilters.TabStop = false;
			this.groupBoxFilters.Text = "Filters";
			// 
			// comboBoxNewFilterTypes
			// 
			this.comboBoxNewFilterTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNewFilterTypes.FormattingEnabled = true;
			this.comboBoxNewFilterTypes.Location = new System.Drawing.Point(14, 26);
			this.comboBoxNewFilterTypes.Name = "comboBoxNewFilterTypes";
			this.comboBoxNewFilterTypes.Size = new System.Drawing.Size(120, 21);
			this.comboBoxNewFilterTypes.TabIndex = 6;
			// 
			// buttonAddFilter
			// 
			this.buttonAddFilter.Image = global::VixenApplication.Properties.Resources.add_24;
			this.buttonAddFilter.Location = new System.Drawing.Point(144, 20);
			this.buttonAddFilter.Name = "buttonAddFilter";
			this.buttonAddFilter.Size = new System.Drawing.Size(32, 32);
			this.buttonAddFilter.TabIndex = 7;
			this.buttonAddFilter.UseVisualStyleBackColor = true;
			this.buttonAddFilter.Click += new System.EventHandler(this.buttonAddFilter_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Image = global::VixenApplication.Properties.Resources.delete_24;
			this.buttonDelete.Location = new System.Drawing.Point(186, 20);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(32, 32);
			this.buttonDelete.TabIndex = 12;
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// ConfigFiltersAndPatching
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 462);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonZoomIn);
			this.Controls.Add(this.buttonZoomOut);
			this.Controls.Add(this.groupBoxFilters);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonPatchWizard);
			this.Controls.Add(this.diagramDisplay);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(800, 500);
			this.Name = "ConfigFiltersAndPatching";
			this.Text = "Output Filters & Patching Setup";
			this.Load += new System.EventHandler(this.ConfigFiltersAndPatching_Load);
			this.ResizeEnd += new System.EventHandler(this.ConfigFiltersAndPatching_ResizeEnd);
			this.Resize += new System.EventHandler(this.ConfigFiltersAndPatching_Resize);
			this.diagramContextMenuStrip.ResumeLayout(false);
			this.groupBoxFilters.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Dataweb.NShape.WinFormsUI.Display diagramDisplay;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonZoomOut;
		private System.Windows.Forms.Button buttonZoomIn;
		private Dataweb.NShape.Controllers.DiagramSetController diagramSetController;
		private Dataweb.NShape.Project project;
		private Dataweb.NShape.Advanced.CachedRepository cachedRepository;
		private System.Windows.Forms.ContextMenuStrip diagramContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem copyFilterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteFilterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteFilterMultipleToolStripMenuItem;
		private System.Windows.Forms.Button buttonPatchWizard;
		private System.Windows.Forms.GroupBox groupBoxFilters;
		private System.Windows.Forms.ComboBox comboBoxNewFilterTypes;
		private System.Windows.Forms.Button buttonAddFilter;
		private System.Windows.Forms.Button buttonDelete;

	}
}