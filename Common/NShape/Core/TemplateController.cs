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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;


namespace Dataweb.NShape.Controllers
{
	/// <summary>
	/// A non-visual component for editing templates. 
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof (TemplateController), "TemplateController.bmp")]
	public class TemplateController : Component, IDisplayService
	{
		/// <summary>
		/// Creates a new <see cref="T:Dataweb.NShape.Controllers.TemplateController" /> instance
		/// </summary>
		public TemplateController()
		{
			infoGraphics = Graphics.FromHwnd(IntPtr.Zero);
		}


		/// <summary>
		/// Creates and initializes a new <see cref="T:Dataweb.NShape.Controllers.TemplateController" /> instance
		/// </summary>
		public TemplateController(Project project, Template template)
			: this()
		{
			if (project == null) throw new ArgumentNullException("project");
			Initialize(project, template);
		}


		/// <ToBeCompleted></ToBeCompleted>
		~TemplateController()
		{
			infoGraphics.Dispose();
			infoGraphics = null;
		}

		#region IDisplayService Members

		/// <override></override>
		void IDisplayService.Invalidate(int x, int y, int width, int height)
		{
			// nothing to do
		}


		/// <override></override>
		void IDisplayService.Invalidate(Rectangle rectangle)
		{
			// nothing to do
		}


		/// <override></override>
		void IDisplayService.NotifyBoundsChanged()
		{
			// nothing to do
		}


		/// <override></override>
		Graphics IDisplayService.InfoGraphics
		{
			get { return infoGraphics; }
		}


		IFillStyle IDisplayService.HintBackgroundStyle
		{
			get
			{
				if (Project != null && Project.IsOpen)
					return Project.Design.FillStyles.White;
				else return null;
			}
		}


		ILineStyle IDisplayService.HintForegroundStyle
		{
			get
			{
				if (Project != null && Project.IsOpen)
					return Project.Design.LineStyles.Normal;
				else return null;
			}
		}

		#endregion

		#region [Public] Events

		/// <summary>
		/// Raised when the <see cref="T:Dataweb.NShape.Controllers.TemplateController" /> initializes.
		/// </summary>
		public event EventHandler<TemplateControllerInitializingEventArgs> Initializing;

		/// <summary>
		/// Raised when changes are applied.
		/// </summary>
		public event EventHandler ApplyingChanges;

		/// <summary>
		/// Raised when changes are discarded.
		/// </summary>
		public event EventHandler DiscardingChanges;

		/// <summary>
		/// Raised when ever a property of the template (such as name, title or description) was changed.
		/// </summary>
		public event EventHandler TemplateModified;

		/// <summary>
		/// Raised when ever a property of the template's shape was changed.
		/// </summary>
		public event EventHandler TemplateShapeModified;

		/// <summary>
		/// Raised when ever a property of the template's model object was changed.
		/// </summary>
		public event EventHandler TemplateModelObjectModified;

		/// <summary>
		/// Raised when the template's shape is replaced by another shape.
		/// </summary>
		public event EventHandler<TemplateControllerTemplateShapeReplacedEventArgs> TemplateShapeChanged;

		/// <summary>
		/// Raised when the template's ModelObject is replaced by another ModelObject
		/// </summary>
		public event EventHandler<TemplateControllerModelObjectReplacedEventArgs> TemplateModelObjectChanged;

		/// <summary>
		/// Raised when the property mapping between shape and ModelObject was created or changed
		/// </summary>
		public event EventHandler<TemplateControllerPropertyMappingChangedEventArgs> TemplateShapePropertyMappingSet;

		/// <summary>
		/// Raised when the property mapping between shape and ModelObject was deleted
		/// </summary>
		public event EventHandler<TemplateControllerPropertyMappingChangedEventArgs> TemplateShapePropertyMappingDeleted;

		/// <summary>
		/// Raised when ConnectionPoints were enabled/disabled or mapped to other Terminals of the underlying ModelObject
		/// </summary>
		public event EventHandler<TemplateControllerPointMappingChangedEventArgs> TemplateShapeControlPointMappingChanged;

		#endregion

		#region [Public] Properties

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		public string ProductVersion
		{
			get { return this.GetType().Assembly.GetName().Version.ToString(); }
		}


		/// <summary>
		/// Provides access to a <see cref="T:Dataweb.NShape.Project" />.
		/// </summary>
		[Category("NShape")]
		public Project Project
		{
			get { return project; }
			set
			{
				if (project != null) UnregisterProjectEvents();
				project = value;
				if (project != null) {
					RegisterProjectEvents();
					if (!isInitializing && project.IsOpen)
						Initialize(project, OriginalTemplate);
				}
			}
		}


		/// <summary>
		/// Specified whether the <see cref="T:Dataweb.NShape.Controllers.TemplateController" /> edits an existing or creates a new template.
		/// </summary>
		[Browsable(false)]
		public TemplateControllerEditMode EditMode
		{
			get { return editMode; }
		}


		/// <summary>
		/// A list of all shapes available.
		/// </summary>
		[Browsable(false)]
		public IReadOnlyCollection<Shape> Shapes
		{
			get { return shapes; }
		}


		/// <summary>
		/// A list of all model objects available.
		/// </summary>
		[Browsable(false)]
		public IReadOnlyCollection<IModelObject> ModelObjects
		{
			get { return modelObjects; }
		}


		/// <summary>
		/// A clone of the original template. This template will be modified. 
		/// When applying the changes, it will be copied into the original template property-by-property .
		/// </summary>
		[Browsable(false)]
		public Template WorkTemplate
		{
			get { return workTemplate; }
		}


		/// <summary>
		/// The original template. Remains unchanged until applying changes.
		/// </summary>
		[Browsable(false)]
		public Template OriginalTemplate
		{
			get { return originalTemplate; }
		}


		/// <summary>
		/// Specifies whether the <see cref="T:Dataweb.NShape.Controllers.TemplateController" /> was initialized completly.
		/// </summary>
		[Browsable(false)]
		public bool IsInitialized
		{
			get { return isInitialized; }
		}

		#endregion

		#region [Public] Methods

		/// <summary>
		/// Calling this method initializes the <see cref="T:Dataweb.NShape.Controllers.TemplateController" />.
		/// </summary>
		public void Initialize(Template template)
		{
			if (project == null) throw new ArgumentNullException("Property 'Project' is not set.");
			Initialize(Project, template);
		}


		/// <summary>
		/// Calling this method initializes the <see cref="T:Dataweb.NShape.Controllers.TemplateController" />.
		/// </summary>
		public void Initialize(Project project, Template template)
		{
			if (isInitializing) {
				Debug.Fail("Already initializing");
				return;
			}
			try {
				isInitializing = true;
				if (project == null) throw new ArgumentNullException("project");
				if (this.project != project) Project = project;

#if DEBUG_DIAGNOSTICS
				if (template != null)
					template.Tag = template.Name;
#endif

				// Check if there are shape types supporting templating
				bool templateSupportingShapeTypeFound = false;
				foreach (ShapeType shapeType in project.ShapeTypes) {
					if (shapeType.SupportsAutoTemplates) {
						templateSupportingShapeTypeFound = true;
						break;
					}
				}
				if (!templateSupportingShapeTypeFound)
					throw new NShapeException("No template supporting shape types found. Load a shape library first.");

				// Create a copy of the template
				if (template != null) {
					editMode = TemplateControllerEditMode.EditTemplate;
					originalTemplate = template;
					workTemplate = new Template(originalTemplate.Name, originalTemplate.Shape.Clone());
					workTemplate.CopyFrom(originalTemplate);
					workTemplate.Shape.DisplayService = this;
				}
				else {
					// Create a new Template
					editMode = TemplateControllerEditMode.CreateTemplate;
					originalTemplate = null;

					// As a shape is mandatory for every template, find a shape first
					Shape shape = FindFirstShapeOfType(typeof (IPlanarShape));
					if (shape == null) shape = FindFirstShapeOfType(typeof (Shape)); // if no planar shape was found, get the first one
					int templateCnt = 1;
					foreach (Template t in project.Repository.GetTemplates()) ++templateCnt;
					workTemplate = new Template(string.Format("Template {0}", templateCnt), shape);
					shape.DisplayService = this;
				}

				// Disable all controls if the user has not the appropriate access rights
				if (!project.SecurityManager.IsGranted(Permission.Templates)) {
					// ToDo: implement access right restrictions
				}

				InitShapeList();
				InitModelObjectList();
				isInitialized = true;

				if (Initializing != null) {
					TemplateControllerInitializingEventArgs eventArgs = new TemplateControllerInitializingEventArgs(editMode, template);
					Initializing(this, eventArgs);
				}
			}
			finally {
				isInitializing = false;
			}
		}


		/// <summary>
		/// Rename the current template.
		/// </summary>
		/// <param name="name"></param>
		public void SetTemplateName(string name)
		{
			if (workTemplate.Name != name) {
				string oldName = workTemplate.Name;
				workTemplate.Name = name;
				TemplateWasChanged = true;

				if (TemplateModified != null) TemplateModified(this, EventArgs.Empty);
			}
		}


		/// <summary>
		/// Change the current template's title.
		/// </summary>
		public void SetTemplateTitle(string title)
		{
			if (workTemplate.Title != title) {
				string oldTitle = workTemplate.Title;
				workTemplate.Title = title;
				TemplateWasChanged = true;

				if (TemplateModified != null) TemplateModified(this, EventArgs.Empty);
			}
		}


		/// <summary>
		/// Change the current template's description.
		/// </summary>
		public void SetTemplateDescription(string description)
		{
			if (workTemplate.Description != description) {
				string oldDescription = workTemplate.Name;
				workTemplate.Description = description;
				TemplateWasChanged = true;

				if (TemplateModified != null) TemplateModified(this, EventArgs.Empty);
			}
		}


		/// <summary>
		/// Set the given shape as the template's shape.
		/// </summary>
		public void SetTemplateShape(Shape newShape)
		{
			if (newShape == null) throw new ArgumentNullException("newShape");
			// buffer the current template shape
			Shape oldShape = workTemplate.Shape;
			if (oldShape != null)
				oldShape.Invalidate();

			// set the new template shape
			newShape.DisplayService = this;
			newShape.Invalidate();
			workTemplate.Shape = newShape;

			TemplateWasChanged = true;
			if (TemplateShapeChanged != null) {
				shapeReplacedEventArgs.Template = workTemplate;
				shapeReplacedEventArgs.OldTemplateShape = oldShape;
				shapeReplacedEventArgs.NewTemplateShape = newShape;
				TemplateShapeChanged(this, shapeReplacedEventArgs);
			}
		}


		/// <summary>
		/// Set the given Modelobject as the template's ModelObject
		/// </summary>
		public void SetTemplateModel(IModelObject newModelObject)
		{
			if (workTemplate.Shape == null)
				throw new NShapeException("The template's shape property is not set to a reference of an object.");
			IModelObject oldModelObject = workTemplate.Shape.ModelObject;
			if (oldModelObject != null) {
				// ToDo: Implement ModelObject.CopyFrom()
				//newModelObject.CopyFrom(oldModelObject);
			}
			workTemplate.UnmapAllTerminals();
			workTemplate.Shape.ModelObject = newModelObject;
			TemplateWasChanged = true;

			if (TemplateModelObjectChanged != null) {
				modelObjectReplacedEventArgs.Template = workTemplate;
				modelObjectReplacedEventArgs.OldModelObject = oldModelObject;
				modelObjectReplacedEventArgs.NewModelObject = newModelObject;
				TemplateModelObjectChanged(this, modelObjectReplacedEventArgs);
			}
			if (TemplateModelObjectChanged != null)
				TemplateModelObjectChanged(this, null);
		}


		/// <summary>
		/// Define a new model-to-shape property mapping.
		/// </summary>
		public void SetModelMapping(IModelMapping modelMapping)
		{
			if (modelMapping == null) throw new ArgumentNullException("modelMapping");
			workTemplate.MapProperties(modelMapping);
			TemplateWasChanged = true;
			if (TemplateShapePropertyMappingSet != null) {
				modelMappingChangedEventArgs.Template = workTemplate;
				modelMappingChangedEventArgs.ModelMapping = modelMapping;
				TemplateShapePropertyMappingSet(this, modelMappingChangedEventArgs);
			}
		}


		/// <summary>
		/// Deletes a model-to-shape property mapping
		/// </summary>
		/// <param name="modelMapping"></param>
		public void DeleteModelMapping(IModelMapping modelMapping)
		{
			if (modelMapping == null) throw new ArgumentNullException("modelMapping");
			workTemplate.UnmapProperties(modelMapping);
			TemplateWasChanged = true;
			if (TemplateShapePropertyMappingDeleted != null) {
				modelMappingChangedEventArgs.Template = workTemplate;
				modelMappingChangedEventArgs.ModelMapping = modelMapping;
				TemplateShapePropertyMappingDeleted(this, modelMappingChangedEventArgs);
			}
		}


		/// <summary>
		/// If the template has no Modelobject, this method enables/disables ConnectionPoints of the shape.
		/// If the template has a ModelObject, this method assigns a ModelObject terminal to a ConnectionPoint of the shape
		/// </summary>
		/// <param name="controlPointId">Id of the shape's ControlPoint</param>
		/// <param name="terminalId">Id of the Modelobject's Terminal. Pass -1 in order to clear the mapping.</param>
		public void SetTerminalConnectionPointMapping(ControlPointId controlPointId, TerminalId terminalId)
		{
			TerminalId oldTerminalId = workTemplate.GetMappedTerminalId(controlPointId);
			workTemplate.MapTerminal(terminalId, controlPointId);
			TemplateWasChanged = true;

			if (TemplateShapeControlPointMappingChanged != null) {
				controlPointMappingChangedEventArgs.ControlPointId = controlPointId;
				controlPointMappingChangedEventArgs.OldTerminalId = oldTerminalId;
				controlPointMappingChangedEventArgs.NewTerminalId = terminalId;
				TemplateShapeControlPointMappingChanged(this, controlPointMappingChangedEventArgs);
			}
		}


		/// <summary>
		/// Applies all changes made on the working template to the original template.
		/// </summary>
		public void ApplyChanges()
		{
			if (string.IsNullOrEmpty(workTemplate.Name)) throw new NShapeException("The template's name must not be empty.");
			if (TemplateWasChanged) {
				ICommand cmd = null;
				switch (editMode) {
					case TemplateControllerEditMode.CreateTemplate:
						cmd = new CreateTemplateCommand(workTemplate);
						project.ExecuteCommand(cmd);
						// after inserting the template into the cache, the template becomes the new 
						// originalTemplate and a new workTemplate has to be cloned.
						// TemplateControllerEditMode is changed from Create to Edit so the user can continue editing the 
						// template until the template editor is closed
						originalTemplate = workTemplate;
						// ToDo: Set appropriate DisplayService
						originalTemplate.Shape.DisplayService = null;
						workTemplate = originalTemplate.Clone();
						editMode = TemplateControllerEditMode.EditTemplate;
						break;

					case TemplateControllerEditMode.EditTemplate:
						// set workTemplate.Shape's DisplayService to the original shape's DisplayService 
						// (typically the ToolSetController)
						workTemplate.Shape.DisplayService = originalTemplate.Shape.DisplayService;
						if (workTemplate.Shape.Type != originalTemplate.Shape.Type)
							cmd = new ExchangeTemplateShapeCommand(originalTemplate, workTemplate);
						else
							cmd = new CopyTemplateFromTemplateCommand(originalTemplate, workTemplate);
						project.ExecuteCommand(cmd);
						break;

					default:
						throw new NShapeUnsupportedValueException(typeof (TemplateControllerEditMode), editMode);
				}
				TemplateWasChanged = false;
				if (ApplyingChanges != null) ApplyingChanges(this, EventArgs.Empty);
			}
		}


		/// <summary>
		/// Discards all changes made to the working copy of the original template.
		/// </summary>
		public void DiscardChanges()
		{
			if (EditMode == TemplateControllerEditMode.CreateTemplate)
				Initialize(project, null);
			else
				Initialize(project, originalTemplate);
			if (DiscardingChanges != null) DiscardingChanges(this, EventArgs.Empty);
		}


		/// <summary>
		/// Clears all buffers and objects used by the <see cref="T:Dataweb.NShape.Controllers.TemplateController" />
		/// </summary>
		public void Clear()
		{
			ClearShapeList();
			ClearModelObjectList();

			workTemplate = null;
			originalTemplate = null;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void NotifyTemplateShapeChanged()
		{
			templateWasChanged = true;
			if (TemplateShapeModified != null) TemplateShapeModified(this, EventArgs.Empty);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void NotifyTemplateModelObjectChanged()
		{
			templateWasChanged = true;
			if (TemplateModelObjectModified != null) TemplateModelObjectModified(this, EventArgs.Empty);
		}

		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				Clear();
				infoGraphics.Dispose();
			}
			base.Dispose(disposing);
		}


		private bool TemplateWasChanged
		{
			get { return templateWasChanged; }
			set
			{
				if (project.SecurityManager.IsGranted(Permission.Templates)) {
					templateWasChanged = value;
					if (TemplateModified != null) TemplateModified(this, EventArgs.Empty);
				}
			}
		}

		#region [Private] Methods

		private void RegisterProjectEvents()
		{
			Debug.Assert(project != null);
			if (project != null) {
				project.Closing += project_Closing;
				project.Opened += project_Opened;
			}
		}


		private void UnregisterProjectEvents()
		{
			Debug.Assert(project != null);
			if (project != null) {
				project.Closing -= project_Closing;
				project.Opened -= project_Opened;
			}
		}


		private void project_Opened(object sender, EventArgs e)
		{
		}


		private void project_Closing(object sender, EventArgs e)
		{
		}


		private bool IsOfType(Type type, Type targetType)
		{
			return (type == targetType || type.IsSubclassOf(targetType) || type.GetInterface(targetType.Name, true) != null);
		}


		private void ClearShapeList()
		{
			//foreach (Shape shape in shapes)
			//   shape.Dispose();
			shapes.Clear();
		}


		private void ClearModelObjectList()
		{
			//foreach (IModelObject modelObject in modelObjects)
			//   modelObject.Dispose();
			modelObjects.Clear();
		}


		private void InitShapeList()
		{
			ClearShapeList();
			foreach (ShapeType shapeType in project.ShapeTypes) {
				if (!shapeType.SupportsAutoTemplates) continue;
				Shape shape = shapeType.CreateInstance();
				shape.DisplayService = this;
				shapes.Add(shape);
			}
		}


		private void InitModelObjectList()
		{
			ClearModelObjectList();
			foreach (ModelObjectType modelObjectType in project.ModelObjectTypes) {
				IModelObject modelObject = modelObjectType.CreateInstance();
				modelObjects.Add(modelObject);
			}
		}


		private Shape FindFirstShapeOfType(Type type)
		{
			foreach (ShapeType shapeType in project.ShapeTypes) {
				if (!shapeType.SupportsAutoTemplates) continue;
				Shape shape = shapeType.CreateInstance();
				if (IsOfType(shape.GetType(), type)) {
					if (shape is IPlanarShape) return shape;
				}
				else return shape;
			}
			return null;
		}

		#endregion

		#region Fields

		// IDisplayService fields
		private Graphics infoGraphics;
		// TemplateController fields
		private Project project;

		private TemplateControllerEditMode editMode;
		private Template originalTemplate;
		private Template workTemplate;
		private ReadOnlyList<Shape> shapes = new ReadOnlyList<Shape>();
		private ReadOnlyList<IModelObject> modelObjects = new ReadOnlyList<IModelObject>();
		private bool templateWasChanged = false;
		private bool isInitializing = false;
		private bool isInitialized = false;
		// EventArgs buffers
		private TemplateControllerStringChangedEventArgs stringChangedEventArgs
			= new TemplateControllerStringChangedEventArgs(string.Empty, string.Empty);

		private TemplateControllerTemplateShapeReplacedEventArgs shapeReplacedEventArgs
			= new TemplateControllerTemplateShapeReplacedEventArgs(null, null, null);

		private TemplateControllerModelObjectReplacedEventArgs modelObjectReplacedEventArgs
			= new TemplateControllerModelObjectReplacedEventArgs(null, null, null);

		private TemplateControllerPointMappingChangedEventArgs controlPointMappingChangedEventArgs
			= new TemplateControllerPointMappingChangedEventArgs(null, ControlPointId.None, TerminalId.Invalid,
			                                                     TerminalId.Invalid);

		private TemplateControllerPropertyMappingChangedEventArgs modelMappingChangedEventArgs
			= new TemplateControllerPropertyMappingChangedEventArgs(null, null);

		#endregion
	}


	/// <summary>
	/// Specifies the edit mode of a <see cref="T:Dataweb.NShape.Controllers.TemplateController" />.
	/// </summary>
	public enum TemplateControllerEditMode
	{
		/// <summary>Compose a new template.</summary>
		CreateTemplate,

		/// <summary>Modify an existing template.</summary>
		EditTemplate
	};

	#region EventArgs

	/// <summary>
	/// Encapsulates parameters for an event raised when the <see cref="T:Dataweb.NShape.Controllers.TemplateController" /> is initialized.
	/// </summary>
	public class TemplateControllerInitializingEventArgs : EventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public TemplateControllerInitializingEventArgs(TemplateControllerEditMode editMode, Template template)
		{
			this.editMode = editMode;
			this.template = template;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public TemplateControllerEditMode EditMode
		{
			get { return editMode; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Template Template
		{
			get { return template; }
		}


		private TemplateControllerEditMode editMode;
		private Template template;
	}


	/// <summary>
	/// Encapsulates parameters for an event raised when the name of the <see cref="T:Dataweb.NShape.Controllers.TemplateController" />'s template is modified.
	/// </summary>
	public class TemplateControllerStringChangedEventArgs : EventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public TemplateControllerStringChangedEventArgs(string oldString, string newString)
		{
			this.oldString = oldString;
			this.newString = newString;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string OldString
		{
			get { return oldString; }
			internal set { oldString = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string NewString
		{
			get { return newString; }
			internal set { newString = value; }
		}


		private string newString;
		private string oldString;
	}


	/// <summary>
	/// Encapsulates parameters for a template-related <see cref="T:Dataweb.NShape.Controllers.TemplateController" /> event.
	/// </summary>
	public class TemplateControllerTemplateEventArgs : EventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public TemplateControllerTemplateEventArgs(Template template)
		{
			this.template = template;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Template Template
		{
			get { return template; }
			internal set { template = value; }
		}


		private Template template;
	}


	/// <summary>
	/// Encapsulates parameters for an event raised when the <see cref="T:Dataweb.NShape.Controllers.TemplateController" />'s template shape is replaced ba a shape of another Type.
	/// </summary>
	public class TemplateControllerTemplateShapeReplacedEventArgs : TemplateControllerTemplateEventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public TemplateControllerTemplateShapeReplacedEventArgs(Template template, Shape oldTemplateShape,
		                                                        Shape newTemplateShape)
			: base(template)
		{
			this.oldTemplateShape = oldTemplateShape;
			this.newTemplateShape = newTemplateShape;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Shape OldTemplateShape
		{
			get { return oldTemplateShape; }
			internal set { oldTemplateShape = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Shape NewTemplateShape
		{
			get { return newTemplateShape; }
			internal set { newTemplateShape = value; }
		}


		private Shape oldTemplateShape;
		private Shape newTemplateShape;
	}


	/// <summary>
	/// Encapsulates parameters for an event raised when the <see cref="T:Dataweb.NShape.Controllers.TemplateController" />'s template model object is replaced by a model object of another model object type.
	/// </summary>
	public class TemplateControllerModelObjectReplacedEventArgs : TemplateControllerTemplateEventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public TemplateControllerModelObjectReplacedEventArgs(Template template,
		                                                      IModelObject oldModelObject, IModelObject newModelObject)
			: base(template)
		{
			this.oldModelObject = oldModelObject;
			this.newModelObject = newModelObject;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public IModelObject OldModelObject
		{
			get { return oldModelObject; }
			internal set { oldModelObject = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public IModelObject NewModelObject
		{
			get { return newModelObject; }
			internal set { newModelObject = value; }
		}

		private IModelObject oldModelObject;
		private IModelObject newModelObject;
	}


	/// <summary>
	/// Encapsulates parameters for a <see cref="T:Dataweb.NShape.Controllers.TemplateController" /> event raised when 
	/// the mapping of a <see cref="T:Dataweb.NShape.Advanced.ControlPointId" /> to a <see cref="T:Dataweb.NShape.Advanced.TerminalId" /> is modified.
	/// </summary>
	public class TemplateControllerPropertyMappingChangedEventArgs : TemplateControllerTemplateEventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public TemplateControllerPropertyMappingChangedEventArgs(Template template, IModelMapping modelMapping)
			: base(template)
		{
			this.propertyMapping = modelMapping;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public IModelMapping ModelMapping
		{
			get { return propertyMapping; }
			internal set { propertyMapping = value; }
		}

		private IModelMapping propertyMapping = null;
	}


	/// <summary>
	/// Encapsulates parameters for a <see cref="T:Dataweb.NShape.Controllers.TemplateController" /> event raised when the mapping of shape's properties to modeloject's properties is modified.
	/// </summary>
	public class TemplateControllerPointMappingChangedEventArgs : TemplateControllerTemplateEventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public TemplateControllerPointMappingChangedEventArgs(Template template, ControlPointId controlPointId,
		                                                      TerminalId oldTerminalId, TerminalId newTerminalId)
			: base(template)
		{
			this.controlPointId = controlPointId;
			this.oldTerminalId = oldTerminalId;
			this.newTerminalId = newTerminalId;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public ControlPointId ControlPointId
		{
			get { return controlPointId; }
			internal set { controlPointId = value; }
		}

		/// <ToBeCompleted></ToBeCompleted>
		public TerminalId OldTerminalId
		{
			get { return oldTerminalId; }
			internal set { oldTerminalId = value; }
		}

		/// <ToBeCompleted></ToBeCompleted>
		public TerminalId NewTerminalId
		{
			get { return newTerminalId; }
			internal set { newTerminalId = value; }
		}

		private ControlPointId controlPointId;
		private TerminalId oldTerminalId;
		private TerminalId newTerminalId;
	}

	#endregion
}