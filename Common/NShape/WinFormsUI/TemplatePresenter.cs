/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.WinFormsUI {

	/// <summary>
	/// Implementation of a <see cref="T:Dataweb.NShape.WinFormsUI.TemplatePresenter" /> used for editing templates.
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(TemplatePresenter), "TemplatePresenter.bmp")]
	public partial class TemplatePresenter : UserControl, IDisplayService {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TemplatePresenter" />.
		/// </summary>
		public TemplatePresenter() {
			DoubleBuffered = true;
			InitializeComponent();

			infoGraphics = Graphics.FromHwnd(Handle);

			stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			isInitialized = false;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TemplatePresenter" />.
		/// </summary>
		public TemplatePresenter(Project project, Template template)
			: this() {
			if (project == null) throw new ArgumentNullException("project");
			TemplateController = new TemplateController(project, template);
			disposeTemplateEditor = true;
			Initialize();
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TemplatePresenter" />.
		/// </summary>
		public TemplatePresenter(Project project)
			: this(project, null) {
		}


		/// <summary>
		/// Finalizer of <see cref="T:Dataweb.NShape.WinFormsUI.TemplatePresenter" />
		/// </summary>
		~TemplatePresenter() {
			Dispose();
		}


		#region IDisplayService Members (explicit implementation)

		/// <override></override>
		void IDisplayService.Invalidate(int x, int y, int width, int height) {
			rectBuffer.X = x;
			rectBuffer.Y = y;
			rectBuffer.Width = width;
			rectBuffer.Height = height;
			previewPanel.Invalidate(rectBuffer);
		}


		/// <override></override>
		void IDisplayService.Invalidate(Rectangle rectangle) {
			previewPanel.Invalidate();
		}


		/// <override></override>
		void IDisplayService.NotifyBoundsChanged() {
			// nothing to do
		}
		

		/// <override></override>
		Graphics IDisplayService.InfoGraphics {
			get { return infoGraphics; }
		}


		/// <override></override>
		IFillStyle IDisplayService.HintBackgroundStyle {
			get {
				if (Project != null && Project.IsOpen)
					return Project.Design.FillStyles.White;
				else return null;
			}
		}


		/// <override></override>
		ILineStyle IDisplayService.HintForegroundStyle {
			get {
				if (Project != null && Project.IsOpen)
					return Project.Design.LineStyles.Normal;
				else return null;
			}
		}

		#endregion


		#region [Public] Properties

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		[Browsable(true)]
		public new string ProductVersion {
			get { return base.ProductVersion; }
		}


		/// <summary>
		/// Gets or sets the controller for tis presenter.
		/// </summary>
		[Category("NShape")]
		public TemplateController TemplateController {
			get { return templateController; }
			set {
				if (templateController != null) {
					UnregisterTemplateControllerEvents();
					if (disposeTemplateEditor) {
						propertyPresenter.PropertyController = null;
						templateController.Dispose();
						templateController = null;
					}
					disposeTemplateEditor = false;
				}
				templateController = value;
				if (templateController != null) {
					RegisterTemplateControllerEvents();
					if (templateController.IsInitialized && !this.isInitialized)
						Initialize();
				}
			}
		}


		/// <summary>
		/// Specifies whether the template was modified.
		/// </summary>
		public bool TemplateWasModified {
			get { return templateModified; }
		}

		#endregion


		#region [Public] Methods

		/// <summary>
		/// Apply the changes performed on the template.
		/// </summary>
		public void ApplyChanges() {
			templateController.ApplyChanges();
		}


		/// <summary>
		/// Cancel all changes performed on the template.
		/// </summary>
		public void DiscardChanges() {
			templateController.DiscardChanges();
		}

		#endregion


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				propertyPresenter.PropertyController = null;
				propertyController.Project = null;
				if (disposeTemplateEditor) {
					templateController.Dispose();
					templateController = null;
				}
				GdiHelpers.DisposeObject(ref deactBrush);
				GdiHelpers.DisposeObject(ref actBrush);
				GdiHelpers.DisposeObject(ref deactPen);
				GdiHelpers.DisposeObject(ref actPen);
				if (components != null)
					components.Dispose();

				TemplateController = null;
				GdiHelpers.DisposeObject(ref infoGraphics);
			}
			base.Dispose(disposing);
		}


		private Project Project {
			get { return (templateController != null) ? templateController.Project : null; }
		}
		
		
		#region [Private] Constructing and Initializing

		private void Initialize() {
			if (templateController == null) 
				throw new NShapeException("Unable to initialize {0}: {1} is not set.", this.GetType().Name, typeof(TemplateController).Name);
			
			// if the user has insufficient privileges, deactivate all controls
			if (!templateController.Project.SecurityManager.IsGranted(Permission.Templates)) {
				int cnt = Controls.Count;
				for (int i = 0; i < cnt; ++i)
					Controls[i].Enabled = false;
			}

			// Set current Design for the Style UITypeEditor
			StyleUITypeEditor.Project = templateController.Project;
			// Initialize the propertyGridAdapter
			propertyController.Project = Project;
			propertyPresenter.PropertyController = propertyController;
			propertyPresenter.PrimaryPropertyGrid = shapePropertyGrid;

			// Initialize user interface
			InitShapeComboBox();
			InitModelObjectComboBox();
			InitControlPointMappingTab();
			Template template = templateController.WorkTemplate;
			nameTextBox.Text = template.Name;
			titleTextBox.Text = template.Title;
			descriptionTextBox.Text = template.Description;
			// assign template's shape to the PropertyGrid
			propertyController.SetObject(0, template.Shape, false);
			propertyController.SetObject(1, template.Shape.ModelObject, false);
			//
			// Initialize ModelObject dependent TabPages
			if (templateController.ModelObjects.Count != 0) {
				// ToDo: Add implementation for these methods:
				 //InitModelPropertiesPage(workTemplate.ModelObject);
				 //InitPropertyMappingPage(workTemplate.ModelObject);
			}
			InitializeUI();

			isInitialized = true;
		}


		private void RegisterTemplateControllerEvents() {
			if (templateController != null) {
				templateController.ApplyingChanges += templateController_ApplyingChanges;
				templateController.DiscardingChanges += templateController_DiscardingChanges;
				templateController.Initializing += templateController_Initializing;
				templateController.TemplateModelObjectChanged += templateController_TemplateModelObjectChanged;
				templateController.TemplateModified += templateController_TemplateModified;
				templateController.TemplateShapeControlPointMappingChanged += templateController_PointMappingChanged;
				templateController.TemplateShapePropertyMappingSet += templateController_PropertyMappingChanged;
				templateController.TemplateShapePropertyMappingDeleted += templateController_PropertyMappingChanged;
				templateController.TemplateShapeChanged += templateController_TemplateShapeChanged;
			}
		}


		private void UnregisterTemplateControllerEvents() {
			if (templateController != null) {
				templateController.ApplyingChanges -= templateController_ApplyingChanges;
				templateController.DiscardingChanges -= templateController_DiscardingChanges;
				templateController.Initializing -= templateController_Initializing;
				templateController.TemplateModelObjectChanged -= templateController_TemplateModelObjectChanged;
				templateController.TemplateModified -= templateController_TemplateModified;
				templateController.TemplateShapeControlPointMappingChanged -= templateController_PointMappingChanged;
				templateController.TemplateShapePropertyMappingSet -= templateController_PropertyMappingChanged;
				templateController.TemplateShapePropertyMappingDeleted += templateController_PropertyMappingChanged;
				templateController.TemplateShapeChanged -= templateController_TemplateShapeChanged;
			}
		}

		#endregion


		#region [Private] User interface implementation

		private void InitShapeComboBox() {
			Debug.Assert(templateController != null && templateController.IsInitialized); 
			if (shapeComboBox.Items.Count > 0)
				shapeComboBox.Items.Clear();

			ShapeType shapeType = null;
			if (templateController.WorkTemplate.Shape != null)
				shapeType = templateController.WorkTemplate.Shape.Type;

			foreach (Shape shape in templateController.Shapes) {
				if (!shape.Type.SupportsAutoTemplates) continue;
				ShapeItem item = new ShapeItem(shape, shapeComboBox.Items.Count);
				int idx = shapeComboBox.Items.Add(item);
				Debug.Assert(idx == item.Index);
				if (shape.DisplayService != this) shape.DisplayService = this;
				if (shapeType != null && shape.Type == shapeType)
					shapeComboBox.SelectedIndex = item.Index;
			}
		}


		private void InitModelObjectComboBox() {
			Debug.Assert(templateController != null && templateController.IsInitialized);
			if (modelObjectComboBox.Items.Count > 0)
				modelObjectComboBox.Items.Clear();

			bool templateShapeExists = templateController.WorkTemplate.Shape != null;
			modelObjectComboBox.Enabled = templateShapeExists;
			ModelObjectType modelObjectType = null;
			if (templateShapeExists && templateController.WorkTemplate.Shape.ModelObject != null)
				modelObjectType = templateController.WorkTemplate.Shape.ModelObject.Type;

			modelObjectComboBox.Items.Add(string.Empty);
			foreach (IModelObject modelObject in templateController.ModelObjects) {
				modelObjectComboBox.Items.Add(modelObject);
				if (modelObjectType != null && modelObject.Type == modelObjectType)
					modelObjectComboBox.SelectedItem = modelObject;
			}

			// Check if a shape has already been created from this template
			// ToDo: There should be a cache method providing this information
			if (templateController.EditMode == TemplateControllerEditMode.EditTemplate) {
				IRepository repository = templateController.Project.Repository;
				shapesCreatedFromTemplate = false;
				foreach (Diagram diagram in repository.GetDiagrams()) {
					foreach (Shape shape in diagram.Shapes) {
						if (shape.Template == templateController.OriginalTemplate) {
							// if a shape was created from this template, disable the modelObjectComboBox because the 
							// ModelObject may not been changed in this case
							shapesCreatedFromTemplate = true;
							break;
						}
					}
					if (shapesCreatedFromTemplate) break;
				}
				modelObjectComboBox.Enabled = !shapesCreatedFromTemplate;
			}
		}


		private void InitControlPointMappingTab() {
			Debug.Assert(templateController != null && templateController.WorkTemplate != null);
			if (!pointMappingTabInitialized) {
				Template template = templateController.WorkTemplate;
				Shape shape = template.Shape;
				if (shape != null) {
					IModelObject modelObject = shape.ModelObject;
					
					controlPointMappingGrid.SuspendLayout();
					TerminalColumn.Items.Clear();
					string terminalName = template.GetMappedTerminalName(ControlPointId.None);
					TerminalColumn.Items.Add(terminalName ?? deactivatedTag);
					if (modelObject != null) {
						for (int i = 1; i <= modelObject.Type.MaxTerminalId; ++i)
							TerminalColumn.Items.Add(modelObject.Type.GetTerminalName(i));
					}

					// add a row for each ControlPoint of the shape
					foreach (ControlPointId pointId in shape.GetControlPointIds(ControlPointCapabilities.Connect)) {
						ControlPointMappingInfo ctrlPtMappingInfo = ControlPointMappingInfo.Empty;
						ctrlPtMappingInfo.pointId = pointId;
						ctrlPtMappingInfo.terminalId = template.GetMappedTerminalId(pointId);
						ctrlPtMappingInfo.terminalName = template.GetMappedTerminalName(ctrlPtMappingInfo.pointId) ?? deactivatedTag;

						if (!TerminalColumn.Items.Contains(ctrlPtMappingInfo.terminalName))
							TerminalColumn.Items.Add(ctrlPtMappingInfo.terminalName);

						int rowIdx = controlPointMappingGrid.Rows.Add();
						controlPointMappingGrid.Rows[rowIdx].Cells[0].Value = ctrlPtMappingInfo.pointId;
						controlPointMappingGrid.Rows[rowIdx].Cells[1].Value = ctrlPtMappingInfo.terminalName;
						controlPointMappingGrid.Rows[rowIdx].Tag = ctrlPtMappingInfo;
					}
					pointMappingTabInitialized = true;

					// ToDo: We have to handle when connection point mappings are changed by the user.
					controlPointMappingGrid.Enabled = !shapesCreatedFromTemplate;
					controlPointMappingGrid.ResumeLayout();
					controlPointsTab.Show();
				} else controlPointsTab.Hide();
			}
		}


		private void ClearControlPointMappingTab() {
			// clear current mappings
			controlPointMappingGrid.SuspendLayout();
			controlPointMappingGrid.Rows.Clear();
			TerminalColumn.Items.Clear();
			// set isInitialized state
			pointMappingTabInitialized = false;
		}


		private void ClearPropertyMappingTab() {
			// clear current mappings
			propertyMappingGrid.SuspendLayout();
			propertyMappingGrid.Rows.Clear();
			// set isInitialized state
			propsMappingTabInitialized = false;
		}


		private void InitializeUI() {
			ClearControlPointMappingTab();
			ClearPropertyMappingTab();
			modelObjectComboBox.Enabled = (templateController.WorkTemplate.Shape != null && !shapesCreatedFromTemplate);

			if (templateController.WorkTemplate.Shape != null) {
				templateController.WorkTemplate.Shape.DisplayService = this;
				CenterShape(templateController.WorkTemplate.Shape);
				InitControlPointMappingTab();

				if (templateController.WorkTemplate.Shape.ModelObject == null) {
					if (tabControl.TabPages.Contains(modelPropertiesTab))
						tabControl.TabPages.Remove(modelPropertiesTab);
					if (tabControl.TabPages.Contains(propertyMappingTab))
						tabControl.TabPages.Remove(propertyMappingTab);
				} else {
					if (!tabControl.TabPages.Contains(modelPropertiesTab))
						tabControl.TabPages.Insert(1, modelPropertiesTab);
					if (!tabControl.TabPages.Contains(propertyMappingTab))
						tabControl.TabPages.Insert(3, propertyMappingTab);
					InitModelMappingTab();
				}
			}
		}


		private int CountControlPoints(Shape shape, ControlPointCapabilities pointCapability) {
			int result = 0;
			foreach (ControlPointId id in shape.GetControlPointIds(pointCapability))
				++result;
			return result;
		}
		
		
		private void SetDrawArea() {
			drawArea.X = previewPanel.Width / 8;
			drawArea.Y = previewPanel.Height / 8;
			drawArea.Width = previewPanel.Width - drawArea.X - drawArea.X;
			drawArea.Height = previewPanel.Height - drawArea.Y - drawArea.Y;
		}


		private void CenterShape(Shape shape) {
			SetDrawArea();
			if (shape is ILinearShape)
				shape.Fit(-(drawArea.Width / 2), -(drawArea.Height / 2), drawArea.Width, drawArea.Height);
			else shape.MoveTo(0, 0);
			
			//int x, y;
			//if (shape is ILinearShape) {
			//   int ptCnt = ((ILinearShape)shape).VertexCount;
			//   int dx = drawArea.Width / (ptCnt - 1);
			//   int dy = drawArea.Height / (ptCnt - 1);
			//   int i = 0;
			//   foreach (ControlPointId id in shape.GetControlPointIds(ControlPointCapabilities.Resize)) {
			//      x = -(drawArea.Width / 2) + (i * dx);
			//      y = -(drawArea.Height / 2) + (i * dy);
			//      shape.MoveControlPointTo(id, x, y, ResizeModifiers.None);
			//      ++i;
			//   }
			//} else shape.MoveControlPointTo(ControlPointId.Reference, 0, 0, ResizeModifiers.None);
		}
		
		#endregion


		#region [Private] User interface implementation: Visual Property Mapping Tab

		private void InitModelMappingTab() {
			Debug.Assert(templateController != null && templateController.WorkTemplate != null);
			if (!propsMappingTabInitialized) {
				// If a shape and a model object are set, add all mappable properties to the propertyMappingGrid.
				// If the tamplate has no shape or no model object, hide the whole property mapping tab
				Shape shape = templateController.WorkTemplate.Shape;
				if (shape == null || shape.ModelObject == null)
					propertyMappingTab.Hide();
				else {
					propertyMappingGrid.SuspendLayout();
					valueMappingGrid.SuspendLayout();

					// Clear grids
					propertyMappingGrid.Rows.Clear();
					valueMappingGrid.Rows.Clear();
					valueMappingGrid.Columns.Clear();

					// Retrieve all mappable properties from shape and model object
					GetPropertyInfos(shape.GetType(), shapePropertyInfos);
					GetPropertyInfos(shape.ModelObject.GetType(), modelPropertyInfos);
					if (shapePropertyInfos.Count > 0 && modelPropertyInfos.Count > 0)
						PopulatePropertyMappingGrid();
					
					valueMappingGrid.ResumeLayout();
					propertyMappingGrid.ResumeLayout();
					
					propsMappingTabInitialized = true;
					propertyMappingGrid.Show();
				}
			}
		}


		// PropertyMapping Grid
		private void InitPropertyColumn(DataGridViewComboBoxColumn column, List<PropertyInfo> propertyInfos){
			column.Items.Clear();
			// Add all mappable properties
			for (int i = 0; i < propertyInfos.Count; ++i) {
				if (GetPropertyId(propertyInfos[i]).HasValue)
					column.Items.Add(propertyInfos[i].Name);
			}
			// Insert empty item
			if (column.Items.Count > 0) column.Items.Insert(0, string.Empty);
		}


		/// <summary>
		/// Creates a row in the property mapping grid for each existing model mapping of the given template
		/// </summary>
		private void PopulatePropertyMappingGrid() {
			propertyMappingGridPopulating = true;
			// Add all (writable) shape and model properties with a PropertyIdAttribute set
			InitPropertyColumn(shapePropertyColumn, shapePropertyInfos);
			InitPropertyColumn(modelPropertyColumn, modelPropertyInfos);

			// Process all existing ModelMappings of the template
			foreach (IModelMapping modelMapping in templateController.WorkTemplate.GetPropertyMappings()) {
				if (modelMapping != null) {
					if (modelMapping is NumericModelMapping
						|| modelMapping is FormatModelMapping
						|| modelMapping is StyleModelMapping)
						DoCreatePropertyMappingGridRow(modelMapping);
					else throw new NotSupportedException(string.Format("ModelMappings of type {0} are currently not supported.", modelMapping.GetType().Name));
				}
			}
			// Add an extra row for creating new mappings
			propertyMappingGrid.Rows.Add();
			propertyMappingGridPopulating = false;
		}


		private void DoCreatePropertyMappingGridRow(IModelMapping modelMapping) {
			// Find corresponding PropertyInfos 
			PropertyInfo shapePropertyInfo = FindPropertyInfo(shapePropertyInfos, modelMapping.ShapePropertyId);
			PropertyInfo modelPropertyInfo = FindPropertyInfo(modelPropertyInfos, modelMapping.ModelPropertyId);
			// create a new row for the existing ModelMapping
			int rowIdx = propertyMappingGrid.Rows.Add();
			propertyMappingGrid.Rows[rowIdx].Tag = modelMapping;
			if (modelPropertyInfo != null) propertyMappingGrid.Rows[rowIdx].Cells[modelColumnIdx].Value = modelPropertyInfo.Name;
			if (shapePropertyInfo != null) propertyMappingGrid.Rows[rowIdx].Cells[shapeColumnIdx].Value = shapePropertyInfo.Name;
		}


		private bool TryCreatePropertyMappingFromGridRow(int rowIndex) {
			bool result = false;
			// Check if the property is already mapped to an other property
			if (IsModelPropertyMapped(rowIndex)) {
				string errMsg = string.Format("'{0}' is already mapped to an other property.", propertyMappingGrid.Rows[rowIndex].Cells[shapeColumnIdx].Value);
				propertyMappingGrid.Rows[rowIndex].Cells[shapeColumnIdx].Value = null;
				//throw new NShapeException(errMsg);
				MessageBox.Show(this, errMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return result;
			} 
			if (IsShapePropertyMapped(rowIndex)) {
				string errMsg = string.Format("'{0}' is already mapped to an other property.", propertyMappingGrid.Rows[rowIndex].Cells[shapeColumnIdx].Value);
				propertyMappingGrid.Rows[rowIndex].Cells[shapeColumnIdx].Value = null;
				//throw new NShapeException(errMsg);
				MessageBox.Show(this, errMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return result;
			}

			// If model property and shape property were selected, create mapping and add a new Row to the grid
			string modelPropertyName = (string)propertyMappingGrid.Rows[rowIndex].Cells[modelColumnIdx].Value;
			string shapePropertyName = (string)propertyMappingGrid.Rows[rowIndex].Cells[shapeColumnIdx].Value;
			if (!string.IsNullOrEmpty(modelPropertyName) && !string.IsNullOrEmpty(shapePropertyName)) {
				// First, find corresponding PropertyInfos
				PropertyInfo shapePropertyInfo = FindPropertyInfo(shapePropertyInfos, shapePropertyName);
				PropertyInfo modelPropertyInfo = FindPropertyInfo(modelPropertyInfos, modelPropertyName);

				// Then, get PropertyId's and create ModelMapping
				Debug.Assert(shapePropertyInfo != null && modelPropertyInfo != null);
				IModelMapping modelMapping = CreatePropertyMapping(shapePropertyInfo, modelPropertyInfo);
				if (modelMapping != null) {
					propertyMappingGrid.Rows[rowIndex].Tag = modelMapping;
					templateController.SetModelMapping(modelMapping);
					result = true;
				}
			} else if (propertyMappingGrid.Rows[rowIndex].Tag is IModelMapping) {
				templateController.WorkTemplate.UnmapProperties((IModelMapping)propertyMappingGrid.Rows[rowIndex].Tag);
				propertyMappingGrid.Rows[rowIndex].Tag = null;
			}
			return result;
		}


		/// <summary>
		/// Checks if the shape property is already mapped to another model property
		/// </summary>
		private bool IsShapePropertyMapped(int rowIndex) {
			object currentCellValue = propertyMappingGrid.Rows[rowIndex].Cells[shapeColumnIdx].Value;
			string currentValStr = string.Format("{0}", currentCellValue);
			if (!string.IsNullOrEmpty(currentValStr)) {
				for (int i = 0; i < propertyMappingGrid.Rows.Count; ++i) {
					if (i == rowIndex) continue;
					string cellValStr = string.Format("{0}", propertyMappingGrid.Rows[i].Cells[shapeColumnIdx].Value);
					if (cellValStr == currentValStr)
						return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Checks if the shape property is already mapped to another model property
		/// </summary>
		private bool IsModelPropertyMapped(int rowIndex) {
			object currentCellValue = propertyMappingGrid.Rows[rowIndex].Cells[modelColumnIdx].Value;
			string currentValStr = string.Format("{0}", currentCellValue);
			if (!string.IsNullOrEmpty(currentValStr)) {
				for (int i = 0; i < propertyMappingGrid.Rows.Count; ++i) {
					if (i == rowIndex) continue;
					string cellValStr = string.Format("{0}", propertyMappingGrid.Rows[i].Cells[modelColumnIdx].Value);
					if (cellValStr == currentValStr) return true;
				}
			}
			return false;
		}


		// Value Mapping Grid
		private void PopulateValueMappingGrid(IModelMapping modelMapping) {
			valueMappingGridPopulating = true;
			// Clear value mapping grid
			valueMappingGrid.Rows.Clear();
			valueMappingGrid.Columns.Clear();

			// Add columns and rows
			if (modelMapping is NumericModelMapping) {
				CreateNumericMappingColumns((NumericModelMapping)modelMapping);
				CreateNumericMappingRows((NumericModelMapping)modelMapping);
			} else if (modelMapping is FormatModelMapping) {
				CreateFormatMappingColumns((FormatModelMapping)modelMapping);
				CreateFormatMappingRows((FormatModelMapping)modelMapping);
			} else if (modelMapping is StyleModelMapping) {
				CreateStyleMappingColumns((StyleModelMapping)modelMapping);
				CreateStyleMappingRows((StyleModelMapping)modelMapping);
			}
			valueMappingGridPopulating = false;
		}


		private void CreateNumericMappingColumns(NumericModelMapping modelMapping) {
			valueMappingGrid.Columns.Clear();
			// Column for property setting title ("Slope" and "Intercept" in this case)
			valueMappingGrid.Columns.Add(mappingPropertyNameCol, titlePropertyName);
			valueMappingGrid.Columns[mappingPropertyNameCol].ValueType = typeof(string);
			valueMappingGrid.Columns[mappingPropertyNameCol].ReadOnly = true;
			valueMappingGrid.Columns[mappingPropertyNameCol].Width = valueMappingGrid.Width / 2;
			// Column for property setting values
			valueMappingGrid.Columns.Add(mappingPropertyValueCol, titlePropertyValue);
			valueMappingGrid.Columns[mappingPropertyValueCol].ValueType = typeof(float);
			valueMappingGrid.Columns[mappingPropertyValueCol].ReadOnly = false;
			valueMappingGrid.Columns[mappingPropertyValueCol].Width = valueMappingGrid.Width / 2;
		}


		private void CreateNumericMappingRows(NumericModelMapping modelMapping) {
			// Add rows for Intercept and Slope
			valueMappingGrid.Rows.Clear();
			valueMappingGrid.AllowUserToAddRows = false;
			valueMappingGrid.Rows.Add(titleSlope, ((NumericModelMapping)modelMapping).Slope);
			valueMappingGrid.Rows.Add(titleIntercept, ((NumericModelMapping)modelMapping).Intercept);
		}


		private void CreateFormatMappingColumns(FormatModelMapping modelMapping) {
			Debug.Assert(modelMapping != null);
			valueMappingGrid.Columns.Clear();
			// Column for property setting title ("Format" and "Format String" in this case)
			valueMappingGrid.Columns.Add(mappingPropertyNameCol, titlePropertyName);
			valueMappingGrid.Columns[mappingPropertyNameCol].ValueType = typeof(string);
			valueMappingGrid.Columns[mappingPropertyNameCol].ReadOnly = true;
			valueMappingGrid.Columns[mappingPropertyNameCol].Width = valueMappingGrid.Width / 2;
			// Column for property setting values
			valueMappingGrid.Columns.Add(mappingPropertyValueCol, titlePropertyValue);
			valueMappingGrid.Columns[mappingPropertyValueCol].ValueType = typeof(string);
			valueMappingGrid.Columns[mappingPropertyValueCol].ReadOnly = false;
			valueMappingGrid.Columns[mappingPropertyValueCol].Width = valueMappingGrid.Width / 2;
		}


		private void CreateFormatMappingRows(FormatModelMapping modelMapping) {
			Debug.Assert(modelMapping != null);
			// Add only one row for format-string
			valueMappingGrid.Rows.Clear();
			valueMappingGrid.AllowUserToAddRows = false;
			valueMappingGrid.Rows.Add(titleFormat, ((FormatModelMapping)modelMapping).Format);
		}


		private void CreateStyleMappingColumns(StyleModelMapping modelMapping) {
			Debug.Assert(modelMapping != null);
			valueMappingGrid.Columns.Clear();
			// Column for range start value
			valueMappingGrid.Columns.Add(mappingPropertyNameCol, titleRangeValue);
			if (modelMapping.Type == StyleModelMapping.MappingType.IntegerStyle)
				valueMappingGrid.Columns[mappingPropertyNameCol].ValueType = typeof(int);
			else if (modelMapping.Type == StyleModelMapping.MappingType.FloatStyle)
				valueMappingGrid.Columns[mappingPropertyNameCol].ValueType = typeof(float);
			else throw new NShapeUnsupportedValueException(modelMapping.Type);
			valueMappingGrid.Columns[mappingPropertyNameCol].ReadOnly = false;
			valueMappingGrid.Columns[mappingPropertyNameCol].Width = valueMappingGrid.Width / 2;

			// Column for mapped Style
			PropertyInfo shapePropInfo = FindPropertyInfo(shapePropertyInfos, modelMapping.ShapePropertyId);
			DataGridViewComboBoxColumn styleValueColumn = new DataGridViewComboBoxColumn();
			styleValueColumn.Name = mappingPropertyValueCol;
			styleValueColumn.HeaderText = titleStyleValue;
			styleValueColumn.ValueType = typeof(string);
			styleValueColumn.ReadOnly = false;
			styleValueColumn.Width = valueMappingGrid.Width / 2;
			styleValueColumn.MaxDropDownItems = 10;
			styleValueColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
			styleValueColumn.Items.Clear();
			foreach (IStyle style in TemplateController.Project.Design.Styles) {
				Type styleType = style.GetType();
				if (!IsOfType(styleType, shapePropInfo.PropertyType)) continue;
				else styleValueColumn.Items.Add(style.Title);
			}
			valueMappingGrid.Columns.Add(styleValueColumn);
		}


		private void CreateStyleMappingRows(StyleModelMapping modelMapping) {
			Debug.Assert(modelMapping != null);
			valueMappingGrid.Rows.Clear();
			valueMappingGrid.AllowUserToAddRows = true;
			if (modelMapping.Type == StyleModelMapping.MappingType.IntegerStyle) {
				foreach (object val in modelMapping.ValueRanges) {
					IStyle style = modelMapping[(int)val];
					valueMappingGrid.Rows.Add(new object[] { (int)val, style.Title });
				}
			} else if (modelMapping.Type == StyleModelMapping.MappingType.FloatStyle) {
				foreach (object val in modelMapping.ValueRanges) {
					IStyle style = modelMapping[(float)val];
					valueMappingGrid.Rows.Add(new object[] { (float)val, style.Title });
				}
			} else throw new NShapeUnsupportedValueException(modelMapping.Type);
			// Add an empty row for creating new range/style mappings
			valueMappingGrid.Rows.Add();
		}


		/// <summary>
		/// Updates the given ModelMapping with the information from the given row's cells
		/// </summary>
		private void UpdateNumericModelMapping(NumericModelMapping numericMapping, int rowIndex) {
			object cellValue;
			for (int i = 0; i < valueMappingGrid.Rows.Count; ++i) {
				if (string.Compare((string)valueMappingGrid.Rows[i].Cells[mappingPropertyNameCol].Value, titleSlope) == 0) {
					// Get slope value
					cellValue = valueMappingGrid.Rows[i].Cells[mappingPropertyValueCol].Value;
					if (cellValue is float)	
						numericMapping.Slope = (float)cellValue;
					else if (cellValue is int)
						numericMapping.Slope = (int)cellValue;
					else Debug.Fail("Unsupported cell ValueType");
				} else if (string.Compare((string)valueMappingGrid.Rows[i].Cells[mappingPropertyNameCol].Value, titleIntercept) == 0) {
					// Get intercept value
					cellValue = valueMappingGrid.Rows[i].Cells[mappingPropertyValueCol].Value;
					if (cellValue is float)
						numericMapping.Intercept = (float)cellValue;
					else if (cellValue is int)
						numericMapping.Intercept = (int)cellValue;
					else Debug.Fail("Unsupported cell ValueType");
				}
			}
			// Set ModelMapping
			templateController.SetModelMapping(numericMapping);
		}


		/// <summary>
		/// Updates the given ModelMapping with the information from the given row's cells
		/// </summary>
		private void UpdateFormatModelMapping(FormatModelMapping formatMapping, int rowIndex) {
			// Get format string
			object cellValue = valueMappingGrid.Rows[0].Cells[mappingPropertyValueCol].Value;
			Debug.Assert(cellValue is string);
			string formatString = (string)cellValue;

			// Check if the format string is valid
			try {
				string tstString;
				if (formatMapping.Type == FormatModelMapping.MappingType.FloatString)
					tstString = string.Format(formatString, 0f);
				else if (formatMapping.Type == FormatModelMapping.MappingType.IntegerString)
					tstString = string.Format(formatString, 0);
				else if (formatMapping.Type == FormatModelMapping.MappingType.StringString)
					tstString = string.Format(formatString, " ");
				else throw new NShapeUnsupportedValueException(formatMapping.Type);
				// Set ModelMapping
				formatMapping.Format = formatString;
				templateController.SetModelMapping(formatMapping);
			} catch (Exception ex) {
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		/// <summary>
		/// Updates the given ModelMapping with the information from the given row's cells
		/// </summary>
		private void UpdateStyleModelMapping(StyleModelMapping styleMapping, int rowIndex) {
			// First, clear all ranges
			// ToDo: Optimize this: add new ranges, remove deleted ranges, do not recreate all ranges each time
			styleMapping.ClearValueRanges();

			int rangeColIdx = 0;
			int styleColIdx = 1;
			for (int i = 0; i < valueMappingGrid.Rows.Count; ++i) {
				object rangeValue = valueMappingGrid.Rows[i].Cells[rangeColIdx].Value;
				object styleName = valueMappingGrid.Rows[i].Cells[styleColIdx].Value;
				IStyle style = null;
				// Find style
				if (valueMappingGrid.Rows[i].Cells[styleColIdx].Value != null) {
					PropertyInfo shapePropInfo = FindPropertyInfo(shapePropertyInfos, styleMapping.ShapePropertyId);
					// Get Style value
					string styleTitle = valueMappingGrid.Rows[i].Cells[styleColIdx].Value.ToString();
					foreach (IStyle s in TemplateController.Project.Design.Styles) {
						Type styleType = s.GetType();
						if (!IsOfType(styleType, shapePropInfo.PropertyType)) continue;
						else if (s.Title.Equals(styleTitle, StringComparison.InvariantCultureIgnoreCase)) {
							style = s;
							break;
						}
					}
				}

				// if all required values were found, create a new mapping range
				if (rangeValue != null && style != null) {
					if (styleMapping.Type == StyleModelMapping.MappingType.IntegerStyle)
						styleMapping.AddValueRange((int)rangeValue, style);
					else if (styleMapping.Type == StyleModelMapping.MappingType.FloatStyle)
						styleMapping.AddValueRange((float)rangeValue, style);
					else throw new NShapeUnsupportedValueException(styleMapping.Type);
				}
			}
			// Set ModelMapping
			templateController.SetModelMapping(styleMapping);
		}
	

		/// <summary>
		/// Removes all empty rows from the given grid. Adds an empty row at the bottom if desired
		/// </summary>
		private void MaintainEmptyRows(DataGridView dataGrid, bool keepLastRowEmpty) {
			// Remove all empty rows
			for (int i = dataGrid.Rows.Count - 1; i >= 0; --i) {
				if (IsRowEmpty(dataGrid.Rows, i)) dataGrid.Rows.RemoveAt(i);
			}
			// Add an empty row if desired
			if (keepLastRowEmpty) dataGrid.Rows.Add();
		}
		
		
		/// <summary>
		/// Checks if the given Row contans values
		/// </summary>
		private bool IsRowEmpty(DataGridViewRowCollection rows, int rowIndex) {
			for (int i = 0; i < rows[rowIndex].Cells.Count; ++i) {
				string valStr = string.Format("{0}", rows[rowIndex].Cells[i].Value);
				if (!string.IsNullOrEmpty(valStr)) return false;
			}
			return true;
		}

		#endregion


		#region [Private] TemplateController event implementations

		private void templateController_ApplyingChanges(object sender, EventArgs e) {
			templateModified = false;
		}


		private void templateController_DiscardingChanges(object sender, EventArgs e) {
			templateModified = false;
		}


		private void templateController_Initializing(object sender, TemplateControllerInitializingEventArgs e) {
			Initialize();
		}


		private void templateController_TemplateModified(object sender, EventArgs e) {
			if (!templateModified) templateModified = true;
		}


		private void templateController_PropertyMappingChanged(object sender, TemplateControllerPropertyMappingChangedEventArgs e) {
			if (!templateModified) templateModified = true;
		}


		private void templateController_TemplateShapeChanged(object sender, TemplateControllerTemplateShapeReplacedEventArgs e) {
			Debug.Assert(templateController != null && templateController.IsInitialized);
			if (templateController.WorkTemplate.Shape != null) {
				// assign displayservice so that the shape can be painted onto the previewPanel
				templateController.WorkTemplate.Shape.DisplayService = this;
				CenterShape(templateController.WorkTemplate.Shape);
				// Select appropiate Type in the shape list
				int cnt = templateController.Shapes.Count;
				ShapeType shapeType = templateController.WorkTemplate.Shape.Type;
				foreach (Shape shape in templateController.Shapes) {
					if (shape.Type == shapeType) {
						for (int i = 0; i < shapeComboBox.Items.Count; ++i) {
							ShapeItem item = (ShapeItem)shapeComboBox.Items[i];
							if (item.Shape == shape) {
								shapeComboBox.SelectedIndex = item.Index;
								break;
							}
						}
						break;
					}
				}
			}
			previewPanel.Invalidate();
		}


		private void templateController_PointMappingChanged(object sender, TemplateControllerPointMappingChangedEventArgs e) {
			if (!templateModified) templateModified = true;
			previewPanel.Invalidate();
		}


		private void templateController_TemplateModelObjectChanged(object sender, TemplateControllerModelObjectReplacedEventArgs e) {
			Debug.Assert(templateController != null && templateController.IsInitialized);
			if (templateController.WorkTemplate.Shape != null && templateController.WorkTemplate.Shape.ModelObject != null) {
				// Select appropiate Type in the newModelObject list
				int cnt = templateController.ModelObjects.Count;
				ModelObjectType modelObjectType = templateController.WorkTemplate.Shape.ModelObject.Type;
				foreach (IModelObject modelObject in templateController.ModelObjects) {
					if (modelObject.Type == modelObjectType) {
						modelObjectComboBox.SelectedItem = modelObject;
						break;
					}
				}
			}
		}

		#endregion


		#region [Private] Event Handler implementations

		// Form's events
		private void previewPanel_Resize(object sender, EventArgs e) {
			SetDrawArea();
			if (templateController != null
				&& templateController.WorkTemplate != null
				&& templateController.WorkTemplate.Shape != null) {
				// redraw shape 
				CenterShape(templateController.WorkTemplate.Shape);
				previewPanel.Invalidate();
			}
		}

		
		private void previewPanel_Paint(object sender, PaintEventArgs e) {
			if (templateController != null) {
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

				Template template = templateController.WorkTemplate;
				if (template != null && template.Shape != null) {
					// calculate shape Bounds
					if (drawArea.IsEmpty)
						CenterShape(template.Shape);

					int offsetX, offsetY;
					float scale = 1;
					if (tabControl.SelectedTab == controlPointsTab) {
						Rectangle boundingRect = template.Shape.GetBoundingRectangle(false);
						scale = Math.Min((float)drawArea.Width / boundingRect.Width, (float)drawArea.Height / boundingRect.Height);
						offsetX = (int)Math.Round((drawArea.X / scale) + (drawArea.Width / scale) / 2);
						offsetY = (int)Math.Round((drawArea.Y / scale) + (drawArea.Height / scale) / 2);
						e.Graphics.ResetTransform();
						e.Graphics.ScaleTransform(scale, scale);
					} else {
						offsetX = drawArea.X + (drawArea.Width / 2); //-(shapeBounds.Width / 2);
						offsetY = drawArea.Y + (drawArea.Height / 2); //-(shapeBounds.Height / 2);
					}
					e.Graphics.TranslateTransform(offsetX, offsetY);

					// draw Shape
					template.Shape.Draw(e.Graphics);
					e.Graphics.ResetTransform();

					// draw ConnectionPoints
					if (tabControl.SelectedTab == controlPointsTab) {
						//int cpSize = this.Font.Height + 4;	// 14
						foreach (ControlPointId ptId in template.Shape.GetControlPointIds(ControlPointCapabilities.Connect)) {
							Point p = Point.Empty;
							p = template.Shape.GetControlPointPosition(ptId);
							string ptName = ptId.ToString();

							// measure text of ControlPointId
							layoutRect.Size = TextMeasurer.MeasureText(e.Graphics, ptName, Font, Size.Empty, stringFormat);
							layoutRect.Width *= 1.5f;
							layoutRect.Height *= 1.5f;
							layoutRect.Width = Math.Max(layoutRect.Width, layoutRect.Height);
							layoutRect.X = ((offsetX + p.X) * scale) - (layoutRect.Width / 2f);
							layoutRect.Y = ((offsetY + p.Y) * scale) - (layoutRect.Height / 2f);

							//layoutRect.X = ((offsetX + p.X) * scale) - ((float)cpSize / 2) + 0.5f;
							//layoutRect.Y = ((offsetY + p.Y) * scale) - ((float)cpSize / 2) + 0.75f;
							//layoutRect.Width = cpSize;
							//if (ptName.Length > 1)
							//   layoutRect.Width *= (ptName.Length * 0.66f);
							//layoutRect.Height = cpSize;

							// choose color for (de)activated connection points
							Brush backBrush, fontBrush;
							Pen pen;
							if (template.GetMappedTerminalId(ptId) != TerminalId.Invalid) {
								backBrush = actBrush;
								fontBrush = Brushes.White;
								pen = actPen;
							} else {
								backBrush = deactBrush;
								fontBrush = Brushes.Black;
								pen = deactPen;
							}
							e.Graphics.FillEllipse(backBrush, layoutRect.X, layoutRect.Y, layoutRect.Width, layoutRect.Height);
							e.Graphics.DrawEllipse(pen, layoutRect.X, layoutRect.Y, layoutRect.Width, layoutRect.Height);
							e.Graphics.DrawString(ptName, Font, fontBrush, layoutRect, stringFormat);
						}
					}
				}
			}
		}


		// Controls for general template properties
		private void nameTextBox_TextChanged(object sender, EventArgs e) {
			if (templateController.WorkTemplate.Name != nameTextBox.Text)
				templateController.SetTemplateName(nameTextBox.Text);
		}


		private void titleTextBox_TextChanged(object sender, EventArgs e) {
			if (templateController.WorkTemplate.Title != titleTextBox.Text)
				templateController.SetTemplateTitle(titleTextBox.Text);
		}


		private void descriptionTextBox_TextChanged(object sender, EventArgs e) {
			if (templateController.WorkTemplate.Description != descriptionTextBox.Text)
				templateController.SetTemplateDescription(descriptionTextBox.Text);
		}


		private void shapesComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			if (templateController.WorkTemplate != null && templateController.WorkTemplate.Shape != null) {
				Debug.Assert(shapeComboBox.SelectedItem is ShapeItem);
				// Assign the selected shape only if the shape type differs from the current shape's shape type
				if (isInitialized && shapeComboBox.SelectedIndex >= 0
					&& ((ShapeItem)shapeComboBox.SelectedItem).Shape.Type != TemplateController.WorkTemplate.Shape.Type) {
					// Create and assign a clone of the selected shape
					Shape newShape = ((ShapeItem)shapeComboBox.SelectedItem).Shape.Clone();
					// Do not copy shape properties if a new shape is created because usually the first selected shape 
					// is a TextBox and thus has transparent Fill- and LineStyles which should not be copied
					if (templateController.EditMode == TemplateControllerEditMode.EditTemplate) {
						newShape.CopyFrom(TemplateController.OriginalTemplate.Shape);
						// Try to fit the shape to the size of the original
						Rectangle origBounds = TemplateController.OriginalTemplate.Shape.GetBoundingRectangle(true);
						Rectangle newBounds = newShape.GetBoundingRectangle(true);
						if (origBounds.Size != newBounds.Size)
							newShape.Fit(origBounds.X, origBounds.Y, origBounds.Width, origBounds.Height);
					}
					newShape.ModelObject = TemplateController.WorkTemplate.Shape.ModelObject;
					templateController.SetTemplateShape(newShape);
					propertyController.SetObject(0, newShape, false);
					propertyController.SetObject(1, newShape.ModelObject, false);
					
					// Update user interface
					InitializeUI();
				}
			}
		}


		private void modelObjectComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			if (isInitialized) {
				IModelObject newModelObject = null;
				if (!(modelObjectComboBox.SelectedItem is string && string.Empty.Equals((string)modelObjectComboBox.SelectedItem))) {
					Debug.Assert(modelObjectComboBox.SelectedItem is IModelObject);
					newModelObject = ((IModelObject)modelObjectComboBox.SelectedItem).Clone();
					if (templateController.WorkTemplate.Shape != null && templateController.WorkTemplate.Shape.ModelObject != null) {
						// ToDo: Implement ModelObject.CopyFrom()
						//newModelObject.CopyFrom(workTemplate.ModelObject);
					}
				}
				templateController.SetTemplateModel(newModelObject);
				if (newModelObject != null) {
				   propertyController.SetObjects(0, newModelObject.Shapes, false);
				   propertyController.SetObject(1, newModelObject, false);
				} else propertyController.SetObject(0, templateController.WorkTemplate.Shape, false);


				InitializeUI();
			}
		}


		private void propertyController_PropertyChanged(object sender, PropertyControllerPropertyChangedEventArgs e) {
			templateModified = true;
			templateController.NotifyTemplateShapeChanged();
			previewPanel.Invalidate();
		}


		private void dataGridView_Resize(object sender, EventArgs e) {
			if (sender is DataGridView) {
				DataGridView grid = (DataGridView)sender;
				int cnt = grid.Columns.Count;
				if (cnt > 0) {
					int colWidth = (grid.ClientRectangle.Width / cnt) - 4;
					for (int i = 0; i < cnt; ++i)
						grid.Columns[i].Width = colWidth;
					grid.Invalidate();
				}
			}
		}


		// Tabs
		private void tabControl_SelectedIndexChanged(object sender, EventArgs e) {
			previewPanel.Invalidate();
		}


		// ControlPointMapping tab
		private void controlPointMappingGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (isInitialized && templateController.WorkTemplate.Shape != null) {
				int pointId = ControlPointId.None;
				if (int.TryParse(controlPointMappingGrid[0, e.RowIndex].Value.ToString(), out pointId)) {
					// Get ControlPointId and TerminalId
					TerminalId terminalId = TerminalId.Invalid;
					string terminalName = (string)controlPointMappingGrid[1, e.RowIndex].Value;
					if (templateController.WorkTemplate.Shape != null && templateController.WorkTemplate.Shape.ModelObject != null)
						terminalId = templateController.WorkTemplate.Shape.ModelObject.Type.FindTerminalId(terminalName);
					else {
						if (terminalName != templateController.WorkTemplate.GetMappedTerminalName(ControlPointId.None))
							terminalId = pointId;
					}
					templateController.SetTerminalConnectionPointMapping(pointId, terminalId);
				}
			}
		}


		// PropertyMapping tab
		private void propertyMappingGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (isInitialized && !propertyMappingGridPopulating && e.RowIndex >= 0) {
				bool mappingCreated = TryCreatePropertyMappingFromGridRow(e.RowIndex);
				MaintainEmptyRows(propertyMappingGrid, true);
				PopulateValueMappingGrid(propertyMappingGrid.Rows[e.RowIndex].Tag as IModelMapping);
			}
		}


		private void propertyMappingGrid_SelectionChanged(object sender, EventArgs e) {
			if (isInitialized && !propertyMappingGridPopulating && propertyMappingGrid.SelectedRows.Count == 1)
				PopulateValueMappingGrid(propertyMappingGrid.SelectedRows[0].Tag as IModelMapping);
		}


		private void valueMappingGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (isInitialized && !valueMappingGridPopulating) {
				// check if a ModelMapping was assigned to the cell's Tag property
				Debug.Assert(propertyMappingGrid.SelectedRows.Count == 1);
				IModelMapping modelMapping = propertyMappingGrid.SelectedRows[0].Tag as IModelMapping;
				if (modelMapping is NumericModelMapping)
					UpdateNumericModelMapping((NumericModelMapping)modelMapping, e.RowIndex);
				else if (modelMapping is FormatModelMapping)
					UpdateFormatModelMapping((FormatModelMapping)modelMapping, e.RowIndex);
				else if (modelMapping is StyleModelMapping)
					UpdateStyleModelMapping((StyleModelMapping)modelMapping, e.RowIndex);
				else Debug.Fail(string.Format("Unexpected '{0}' type '{1}'", typeof(IModelMapping).Name, modelMapping.GetType().Name));
			}
		}

		#endregion


		#region [Private] ModelMapping helpers

		private int? GetPropertyId(PropertyInfo propertyInfo) {
			if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
			// Use the static method Attribute.GetCustomAttributes() instead of propertyInfo.GetCustomAttributes()
			// for retrieving the PropertyMapping attributes because the propertyInfo's method only considers its 
			// own contents whereas the static method considers the whole hierarchy
			object[] idAttribs = Attribute.GetCustomAttributes(propertyInfo, typeof(PropertyMappingIdAttribute), true);
			if (idAttribs.Length == 1) {
				Debug.Assert(idAttribs[0] is PropertyMappingIdAttribute);
				return ((PropertyMappingIdAttribute)idAttribs[0]).Id;
			} else if (idAttribs.Length == 0) return null;
			else throw new NShapeException("Property {0} of {1} has more than 1 {2}.", propertyInfo.Name, propertyInfo.DeclaringType.Name, typeof(PropertyMappingIdAttribute).Name);
		}


		private void GetPropertyInfos(Type type, IList<PropertyInfo> propertyInfos) {
			propertyInfos.Clear();
			PropertyInfo[] piArray = type.GetProperties(bindingFlags);
			for (int i = 0; i < piArray.Length; ++i)
				propertyInfos.Add(piArray[i]);
		}


		// Find PropertyInfo with the given name
		private PropertyInfo FindPropertyInfo(IList<PropertyInfo> propertyInfos, string propertyName) {
			PropertyInfo result = null;
			for (int i = 0; i < propertyInfos.Count; ++i) {
				if (propertyInfos[i].Name == propertyName) {
					result = propertyInfos[i];
					break;
				}
			}
			return result;
		}


		// Find propertyInfo with the given PropertyId
		private PropertyInfo FindPropertyInfo(IList<PropertyInfo> propertyInfos, int propertyId) {
			PropertyInfo result = null;
			for (int i = 0; i < propertyInfos.Count; ++i) {
				int? id = GetPropertyId(propertyInfos[i]);
				if (id.HasValue && id.Value == propertyId) {
					result = propertyInfos[i];
					break;
				}
			}
			return result;
		}


		// Create a ModelMapping from the given PropertyInfos
		private IModelMapping CreatePropertyMapping(PropertyInfo shapePropertyInfo, PropertyInfo modelPropertyInfo) {
			int? shapePropertyId = GetPropertyId(shapePropertyInfo);
			int? modelPropertyId = GetPropertyId(modelPropertyInfo);
			Debug.Assert(shapePropertyId.HasValue && modelPropertyId.HasValue);

			MappedPropertyType shapePropType;
			if (IsIntegerType(shapePropertyInfo.PropertyType)) shapePropType = MappedPropertyType.Int;
			else if (IsFloatType(shapePropertyInfo.PropertyType)) shapePropType = MappedPropertyType.Float;
			else if (IsStringType(shapePropertyInfo.PropertyType)) shapePropType = MappedPropertyType.String;
			else if (IsStyleType(shapePropertyInfo.PropertyType)) shapePropType = MappedPropertyType.Style;
			else throw new NotSupportedException();

			MappedPropertyType modelPropType;
			if (IsIntegerType(modelPropertyInfo.PropertyType)) modelPropType = MappedPropertyType.Int;
			else if (IsFloatType(modelPropertyInfo.PropertyType)) modelPropType = MappedPropertyType.Float;
			else if (IsStringType(modelPropertyInfo.PropertyType)) modelPropType = MappedPropertyType.String;
			else throw new NotSupportedException();

			// Create a suitable ModelMapping:
			IModelMapping result = null;
			switch (modelPropType) {
				case MappedPropertyType.Float:
					switch (shapePropType) {
						case MappedPropertyType.Float: result = new NumericModelMapping(shapePropertyId.Value, modelPropertyId.Value, NumericModelMapping.MappingType.FloatFloat); break;
						case MappedPropertyType.Int: result = new NumericModelMapping(shapePropertyId.Value, modelPropertyId.Value, NumericModelMapping.MappingType.FloatInteger); break;
						case MappedPropertyType.String: result = new FormatModelMapping(shapePropertyId.Value, modelPropertyId.Value, FormatModelMapping.MappingType.FloatString); break;
						case MappedPropertyType.Style: result = new StyleModelMapping(shapePropertyId.Value, modelPropertyId.Value, StyleModelMapping.MappingType.FloatStyle); break;
						default: throw new NotSupportedException(string.Format("Property mappings from '{0}' to '{1}' are not supported.", modelPropType, shapePropType));
					}
					break;
				case MappedPropertyType.Int:
					switch (shapePropType) {
						case MappedPropertyType.Float: result = new NumericModelMapping(shapePropertyId.Value, modelPropertyId.Value, NumericModelMapping.MappingType.IntegerFloat); break;
						case MappedPropertyType.Int: result = new NumericModelMapping(shapePropertyId.Value, modelPropertyId.Value, NumericModelMapping.MappingType.IntegerInteger); break;
						case MappedPropertyType.String: result = new FormatModelMapping(shapePropertyId.Value, modelPropertyId.Value, FormatModelMapping.MappingType.IntegerString); break;
						case MappedPropertyType.Style: result = new StyleModelMapping(shapePropertyId.Value, modelPropertyId.Value, StyleModelMapping.MappingType.IntegerStyle); break;
						default: throw new NotSupportedException(string.Format("Property mappings from '{0}' to '{1}' are not supported.", modelPropType, shapePropType));
					}
					break;
				case MappedPropertyType.String:
					if (shapePropType == MappedPropertyType.String)
						result = new FormatModelMapping(shapePropertyId.Value, modelPropertyId.Value, FormatModelMapping.MappingType.StringString);
					else throw new NotSupportedException(string.Format("Property mappings from '{0}' to '{1}' are not supported.", modelPropType, shapePropType));
					break;
				default: throw new NotSupportedException();
			}
			return result;
		}


		// Check if the given Type is an integer type (byte, int16, int32, int64, enum)
		private bool IsIntegerType(Type type) {
			return (type == typeof(Byte)
				|| type == typeof(Int16)
				|| type == typeof(Int32)
				|| type == typeof(Int64)
				|| type == typeof(SByte)
				|| type == typeof(UInt16)
				|| type == typeof(UInt32)
				|| type == typeof(UInt64)
				|| type == typeof(Enum));
		}


		// Check if the given Type is a float type (single, double or decimal)
		private bool IsFloatType(Type type) {
			return (type == typeof(Single)
				|| type == typeof(Double)
				|| type == typeof(Decimal));
		}


		// Check if the given Type is a string type (char or string)
		private bool IsStringType(Type type) {
			return (type == typeof(String)
				|| type == typeof(Char));
		}


		// Check if the given type is based on IStyle
		private bool IsStyleType(Type type) {
			return IsOfType(type, typeof(IStyle));
		}


		// Check if the given type is based on targetType
		private bool IsOfType(Type type, Type targetType) {
			return (type.IsSubclassOf(targetType) || type.GetInterface(targetType.Name, true) != null);
		}

		#endregion


		#region [Private] Types

		private struct ControlPointMappingInfo : IEquatable<ControlPointMappingInfo> {

			public static readonly ControlPointMappingInfo Empty;

			public ControlPointMappingInfo(ControlPointId pointId, TerminalId terminalId, string terminalName) {
				this.pointId = pointId;
				this.terminalId = terminalId;
				this.terminalName = terminalName;
			}

			public ControlPointId pointId;

			public TerminalId terminalId;

			public string terminalName;

			public bool Equals(ControlPointMappingInfo other) {
				return (other.pointId == this.pointId
					&& other.terminalId == this.terminalId
					&& other.terminalName == this.terminalName);
			}

			static ControlPointMappingInfo(){
				Empty.pointId = ControlPointId.None;
				Empty.terminalId = TerminalId.Invalid;
				Empty.terminalName = string.Empty;
			}
		}


		private class ShapeItem {

			public ShapeItem(Shape shape, int itemIndex) {
				this.shape = shape;
				this.index = itemIndex;
			}

			/// <override></override>
			public override string ToString() {
				return shape.ToString();
			}

			public Shape Shape { get { return shape; } }

			public int Index { get { return index; } }

			private Shape shape;
			private int index;
		}


		private enum MappedPropertyType { Int, Float, String, Style };

		#endregion


		#region Fields
		
		private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;

		// IDisplayService fields
		private Graphics infoGraphics;
		
		// Template editor dialog stuff
		private TemplateController templateController;

		private bool isInitialized = false;
		private bool templateModified = false;
		private bool shapesCreatedFromTemplate = false;
		private bool pointMappingTabInitialized = false;
		private bool propsMappingTabInitialized = false;
		private bool propertyMappingGridPopulating = false;
		private bool valueMappingGridPopulating = false;
		private bool disposeTemplateEditor = false;

		// ControlPoint mapping stuff
		private const string activatedTag = "Activated";
		private const string deactivatedTag = "Deactivated";

		// Column Names
		private const string mappingPropertyNameCol = "MappingPropertyNameColumn";
		private const string mappingPropertyValueCol = "MappingPropertyValueColumn";
		// Cell/Row titles
		private const string titlePropertyName = "Mapping Property";
		private const string titlePropertyValue = "Value";
		private const string titleSlope = "Slope";
		private const string titleIntercept = "Intercept";
		private const string titleFormat = "Format String";
		private const string titleRangeValue = "Model Value";
		private const string titleStyleValue = "Style";
		// Row identifiers
		private const int rowIdSlope = 0;
		private const int rowIdIntercept = 1;
		private const int rowIdFormat = 0;

		// PropertyMapping stuff
		private List<PropertyInfo> modelPropertyInfos = new List<PropertyInfo>();
		private List<PropertyInfo> shapePropertyInfos = new List<PropertyInfo>();
		private const int modelColumnIdx = 0;
		private const int shapeColumnIdx = 1;

		// Drawing stuff
		private Rectangle rectBuffer = Rectangle.Empty;
		private Rectangle drawArea = Rectangle.Empty;
		private RectangleF layoutRect = RectangleF.Empty;
		private StringFormat stringFormat;
		private Pen deactPen = new Pen(Color.DarkGray);
		private Brush deactBrush = new SolidBrush(System.Drawing.Color.FromArgb(192, System.Drawing.Color.LightGray));
		private Pen actPen = new Pen(Color.Black);
		private Brush actBrush = new SolidBrush(System.Drawing.Color.FromArgb(128, System.Drawing.Color.DarkGreen));
		#endregion
	}

}