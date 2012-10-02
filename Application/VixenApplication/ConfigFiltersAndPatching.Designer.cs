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
			Dataweb.NShape.RoleBasedSecurityManager roleBasedSecurityManager1 = new Dataweb.NShape.RoleBasedSecurityManager();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonAddFilter = new System.Windows.Forms.Button();
			this.comboBoxNewFilterTypes = new System.Windows.Forms.ComboBox();
			this.diagramDisplay = new Dataweb.NShape.WinFormsUI.Display();
			this.diagramSetController = new Dataweb.NShape.Controllers.DiagramSetController();
			this.project = new Dataweb.NShape.Project(this.components);
			this.cachedRepository = new Dataweb.NShape.Advanced.CachedRepository();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonZoomOut = new System.Windows.Forms.Button();
			this.buttonZoomIn = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(752, 525);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonAddFilter
			// 
			this.buttonAddFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAddFilter.Location = new System.Drawing.Point(434, 478);
			this.buttonAddFilter.Name = "buttonAddFilter";
			this.buttonAddFilter.Size = new System.Drawing.Size(100, 25);
			this.buttonAddFilter.TabIndex = 7;
			this.buttonAddFilter.Text = "Add Filter";
			this.buttonAddFilter.UseVisualStyleBackColor = true;
			this.buttonAddFilter.Click += new System.EventHandler(this.buttonAddFilter_Click);
			// 
			// comboBoxNewFilterTypes
			// 
			this.comboBoxNewFilterTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboBoxNewFilterTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNewFilterTypes.FormattingEnabled = true;
			this.comboBoxNewFilterTypes.Location = new System.Drawing.Point(268, 481);
			this.comboBoxNewFilterTypes.Name = "comboBoxNewFilterTypes";
			this.comboBoxNewFilterTypes.Size = new System.Drawing.Size(160, 21);
			this.comboBoxNewFilterTypes.TabIndex = 6;
			// 
			// diagramDisplay
			// 
			this.diagramDisplay.AllowDrop = true;
			this.diagramDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.diagramDisplay.BackColorGradient = System.Drawing.SystemColors.Control;
			this.diagramDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.diagramDisplay.DiagramSetController = this.diagramSetController;
			this.diagramDisplay.GridColor = System.Drawing.Color.Gainsboro;
			this.diagramDisplay.GridSize = 19;
			this.diagramDisplay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.diagramDisplay.Location = new System.Drawing.Point(12, 12);
			this.diagramDisplay.Name = "diagramDisplay";
			this.diagramDisplay.PropertyController = null;
			this.diagramDisplay.SelectionHilightColor = System.Drawing.Color.Firebrick;
			this.diagramDisplay.SelectionInactiveColor = System.Drawing.Color.Gray;
			this.diagramDisplay.SelectionInteriorColor = System.Drawing.Color.WhiteSmoke;
			this.diagramDisplay.SelectionNormalColor = System.Drawing.Color.DarkGreen;
			this.diagramDisplay.Size = new System.Drawing.Size(830, 450);
			this.diagramDisplay.SnapToGrid = false;
			this.diagramDisplay.TabIndex = 0;
			this.diagramDisplay.ToolPreviewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
			this.diagramDisplay.ToolPreviewColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
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
			roleBasedSecurityManager1.CurrentRole = Dataweb.NShape.StandardRole.Administrator;
			roleBasedSecurityManager1.CurrentRoleName = "Administrator";
			this.project.SecurityManager = roleBasedSecurityManager1;
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
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 481);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(201, 52);
			this.label1.TabIndex = 8;
			this.label1.Text = "Double-click to collapse Channel groups\r\nand controllers, or to configure filters" +
    ".\r\n\r\nRight-click and drag to connect modules.";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(656, 525);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(90, 25);
			this.buttonOK.TabIndex = 9;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonZoomOut
			// 
			this.buttonZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonZoomOut.Image = global::VixenApplication.Properties.Resources.ZoomOut32;
			this.buttonZoomOut.Location = new System.Drawing.Point(802, 467);
			this.buttonZoomOut.Name = "buttonZoomOut";
			this.buttonZoomOut.Size = new System.Drawing.Size(40, 40);
			this.buttonZoomOut.TabIndex = 10;
			this.buttonZoomOut.UseVisualStyleBackColor = true;
			this.buttonZoomOut.Click += new System.EventHandler(this.buttonZoomOut_Click);
			// 
			// buttonZoomIn
			// 
			this.buttonZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonZoomIn.Image = global::VixenApplication.Properties.Resources.ZoomIn32;
			this.buttonZoomIn.Location = new System.Drawing.Point(756, 468);
			this.buttonZoomIn.Name = "buttonZoomIn";
			this.buttonZoomIn.Size = new System.Drawing.Size(40, 40);
			this.buttonZoomIn.TabIndex = 11;
			this.buttonZoomIn.UseVisualStyleBackColor = true;
			this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDelete.Location = new System.Drawing.Point(540, 478);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(100, 25);
			this.buttonDelete.TabIndex = 12;
			this.buttonDelete.Text = "Delete Selected";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// ConfigFiltersAndPatching
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(854, 562);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.buttonZoomIn);
			this.Controls.Add(this.buttonZoomOut);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonAddFilter);
			this.Controls.Add(this.comboBoxNewFilterTypes);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.diagramDisplay);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(750, 500);
			this.Name = "ConfigFiltersAndPatching";
			this.Text = "Output Filters & Patching Setup";
			this.Load += new System.EventHandler(this.ConfigFiltersAndPatching_Load);
			this.ResizeEnd += new System.EventHandler(this.ConfigFiltersAndPatching_ResizeEnd);
			this.Resize += new System.EventHandler(this.ConfigFiltersAndPatching_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Dataweb.NShape.WinFormsUI.Display diagramDisplay;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonAddFilter;
		private System.Windows.Forms.ComboBox comboBoxNewFilterTypes;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonZoomOut;
		private System.Windows.Forms.Button buttonZoomIn;
		private System.Windows.Forms.Button buttonDelete;
		private Dataweb.NShape.Controllers.DiagramSetController diagramSetController;
		private Dataweb.NShape.Project project;
		private Dataweb.NShape.Advanced.CachedRepository cachedRepository;

	}
}