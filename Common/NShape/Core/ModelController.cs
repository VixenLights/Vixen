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
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;


namespace Dataweb.NShape.Controllers
{
	/// <ToBeCompleted></ToBeCompleted>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof (ModelController), "ModelController.bmp")]
	public class ModelController : Component
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.ModelController" />.
		/// </summary>
		public ModelController()
			: base()
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.ModelController" />.
		/// </summary>
		public ModelController(DiagramSetController diagramSetController)
			: this()
		{
			if (diagramSetController == null) throw new ArgumentNullException("diagramSetController");
			DiagramSetController = diagramSetController;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.ModelController" />.
		/// </summary>
		public ModelController(Project project)
			: this()
		{
			if (project == null) throw new ArgumentNullException("project");
			Project = project;
		}

		#region [Public] Events

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler Initialized;

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler Uninitialized;

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler<RepositoryModelObjectsEventArgs> ModelObjectsCreated;

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler<RepositoryModelObjectsEventArgs> ModelObjectsChanged;

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler<RepositoryModelObjectsEventArgs> ModelObjectsDeleted;

		/// <summary>
		/// The Changed event will be raised whenever an object somehow related to a model object has changed.
		/// </summary>
		public event EventHandler Changed;

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
		[ReadOnly(true)]
		[Category("NShape")]
		public Project Project
		{
			get { return (diagramSetController == null) ? project : diagramSetController.Project; }
			set
			{
				if (diagramSetController != null && diagramSetController.Project != value) {
					string errMsg = string.Format("A {0} is already assigned. Its project will be used.",
					                              diagramSetController.GetType().Name);
					throw new InvalidOperationException(errMsg);
				}
				if (Project != value) {
					DetachProject();
					project = value;
					AttachProject();
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("NShape")]
		public DiagramSetController DiagramSetController
		{
			get { return diagramSetController; }
			set
			{
				if (Project != null) DetachProject();
				if (diagramSetController != null) UnregisterDiagramSetControllerEvents();
				diagramSetController = value;
				if (diagramSetController != null) {
					RegisterDiagramSetControllerEvents();
					AttachProject();
				}
			}
		}

		#endregion

		#region [Public] Methods

		/// <ToBeCompleted></ToBeCompleted>
		public void CreateModelObject()
		{
			throw new NotImplementedException();
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void RenameModelObject(IModelObject modelObject, string newName)
		{
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			throw new NotImplementedException();
		}


		/// <summary>
		/// Deletes the given model obejcts and their attached shapes
		/// </summary>
		/// <param name="modelObjects"></param>
		public void DeleteModelObjects(IEnumerable<IModelObject> modelObjects)
		{
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			ICommand cmd = new DeleteModelObjectsCommand(modelObjects);
			Project.ExecuteCommand(cmd);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void SetModelObjectParent(IModelObject modelObject, IModelObject parent)
		{
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			ICommand cmd = new SetModelObjectParentCommand(modelObject, parent);
			Project.ExecuteCommand(cmd);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Copy(IModelObject modelObject)
		{
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			copyPasteBuffer.Clear();
			copyPasteBuffer.Add(modelObject.Clone());
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Copy(IEnumerable<IModelObject> modelObjects)
		{
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			copyPasteBuffer.Clear();
			foreach (IModelObject mo in modelObjects)
				copyPasteBuffer.Add(mo.Clone());
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Paste(IModelObject parent)
		{
			// Set parent
			for (int i = copyPasteBuffer.Count - 1; i >= 0; --i)
				copyPasteBuffer[i].Parent = parent;
			// Execute command
			ICommand command = new CreateModelObjectsCommand(copyPasteBuffer);
			Project.ExecuteCommand(command);
			// Copy for next paste action
			for (int i = copyPasteBuffer.Count - 1; i >= 0; --i)
				copyPasteBuffer[i] = copyPasteBuffer[i].Clone();
		}


		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerable<IModelObject> GetChildModelObjects(IModelObject modelObject)
		{
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			return Project.Repository.GetModelObjects(modelObject);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void FindShapes(IEnumerable<IModelObject> modelObjects)
		{
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			if (diagramSetController == null) throw new InvalidOperationException("DiagramSetController is not set");
			diagramSetController.SelectModelObjects(modelObjects);
		}


		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		public IEnumerable<MenuItemDef> GetMenuItemDefs(Dataweb.NShape.Advanced.IReadOnlyCollection<IModelObject> modelObjects)
		{
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");

			// New...
			// Rename
			yield return CreateDeleteModelObjectsAction(modelObjects);

			yield return new SeparatorMenuItemDef();

			yield return CreateCopyModelObjectsAction(modelObjects);
			// Cut
			yield return CreatePasteModelObjectsAction(modelObjects);

			yield return new SeparatorMenuItemDef();

			// Find model object...
			yield return CreateFindShapesAction(modelObjects);
		}

		#endregion

		#region [Private] Methods: (Un)Registering event handlers

		private void DetachProject()
		{
			if (Project != null) {
				UnregisterProjectEvents();
				project = null;
			}
		}


		private void AttachProject()
		{
			if (Project != null) {
				// Register current project
				RegisterProjectEvents();
			}
		}


		private void RegisterDiagramSetControllerEvents()
		{
			Debug.Assert(diagramSetController != null);
			diagramSetController.ProjectChanged += diagramSetController_ProjectChanged;
			diagramSetController.ProjectChanging += diagramSetController_ProjectChanging;
		}


		private void UnregisterDiagramSetControllerEvents()
		{
			Debug.Assert(diagramSetController != null);
			diagramSetController.ProjectChanged -= diagramSetController_ProjectChanged;
			diagramSetController.ProjectChanging -= diagramSetController_ProjectChanging;
		}


		private void RegisterProjectEvents()
		{
			Debug.Assert(Project != null);

			// Register project events
			Project.Opened += project_ProjectOpen;
			Project.Closing += project_ProjectClosing;
			Project.Closed += project_ProjectClosed;

			// Register repository events
			if (Project.IsOpen) project_ProjectOpen(this, null);
		}


		private void UnregisterProjectEvents()
		{
			Debug.Assert(Project != null);

			// Unregister repository events
			if (Project.Repository != null) UnregisterRepositoryEvents();

			// Unregister project events
			Project.Opened -= project_ProjectOpen;
			Project.Closing -= project_ProjectClosing;
			Project.Closed -= project_ProjectClosed;
		}


		private void RegisterRepositoryEvents()
		{
			Debug.Assert(Project != null && Project.Repository != null);
			Project.Repository.ModelObjectsInserted += repository_ModelObjectsInserted;
			Project.Repository.ModelObjectsUpdated += repository_ModelObjectsUpdated;
			Project.Repository.ModelObjectsDeleted += repository_ModelObjectsDeleted;
			Project.Repository.TemplateInserted += repository_TemplateInserted;
			Project.Repository.TemplateUpdated += repository_TemplateUpdated;
			Project.Repository.TemplateDeleted += repository_TemplateDeleted;
			Project.Repository.TemplateShapeReplaced += repository_TemplateShapeReplaced;
		}


		private void UnregisterRepositoryEvents()
		{
			Debug.Assert(Project != null && Project.Repository != null);
			Project.Repository.ModelObjectsInserted -= repository_ModelObjectsInserted;
			Project.Repository.ModelObjectsUpdated -= repository_ModelObjectsUpdated;
			Project.Repository.ModelObjectsDeleted -= repository_ModelObjectsDeleted;
			Project.Repository.TemplateInserted -= repository_TemplateInserted;
			Project.Repository.TemplateUpdated -= repository_TemplateUpdated;
			Project.Repository.TemplateDeleted -= repository_TemplateDeleted;
			Project.Repository.TemplateShapeReplaced -= repository_TemplateShapeReplaced;
		}

		#endregion

		#region [Private] Methods: DiagramSetController event handler implementations

		private void diagramSetController_ProjectChanged(object sender, EventArgs e)
		{
			if (diagramSetController.Project != null) AttachProject();
		}


		private void diagramSetController_ProjectChanging(object sender, EventArgs e)
		{
			if (diagramSetController.Project != null) DetachProject();
		}

		#endregion

		#region [Private] Methods: Project event handler implementations

		private void project_ProjectOpen(object sender, EventArgs e)
		{
			RegisterRepositoryEvents();
			if (Initialized != null) Initialized(this, EventArgs.Empty);
		}


		private void project_ProjectClosing(object sender, EventArgs e)
		{
			UnregisterRepositoryEvents();
			if (Uninitialized != null) Uninitialized(this, EventArgs.Empty);
		}


		private void project_ProjectClosed(object sender, EventArgs e)
		{
			// nothing to do here
		}

		#endregion

		#region [Private] Methods: Repository event handler implementations

		private void repository_ModelObjectsInserted(object sender, RepositoryModelObjectsEventArgs e)
		{
			if (ModelObjectsCreated != null) ModelObjectsCreated(this, e);
		}


		private void repository_ModelObjectsUpdated(object sender, RepositoryModelObjectsEventArgs e)
		{
			if (ModelObjectsChanged != null) ModelObjectsChanged(this, e);
		}


		private void repository_ModelObjectsDeleted(object sender, RepositoryModelObjectsEventArgs e)
		{
			if (ModelObjectsDeleted != null) ModelObjectsDeleted(this, e);
		}


		private void repository_TemplateShapeReplaced(object sender, RepositoryTemplateShapeReplacedEventArgs e)
		{
			if (e.OldTemplateShape.ModelObject != null || e.NewTemplateShape.ModelObject != null)
				if (Changed != null) Changed(this, EventArgs.Empty);
		}


		private void repository_TemplateInserted(object sender, RepositoryTemplateEventArgs e)
		{
			if (e.Template.Shape.ModelObject != null)
				if (Changed != null) Changed(this, EventArgs.Empty);
		}


		private void repository_TemplateUpdated(object sender, RepositoryTemplateEventArgs e)
		{
			if (e.Template.Shape.ModelObject != null)
				if (Changed != null) Changed(this, EventArgs.Empty);
		}


		private void repository_TemplateDeleted(object sender, RepositoryTemplateEventArgs e)
		{
			if (e.Template.Shape.ModelObject != null)
				if (Changed != null) Changed(this, EventArgs.Empty);
		}

		#endregion

		#region [Private] Methods: Create actions

		private MenuItemDef CreateDeleteModelObjectsAction(Dataweb.NShape.Advanced.IReadOnlyCollection<IModelObject> modelObjects)
		{
			string description;
			bool isFeasible;
			if (modelObjects != null && modelObjects.Count > 0) {
				isFeasible = true;
				description = string.Format("Delete {0} model object{1}.", modelObjects.Count,
				                            (modelObjects.Count > 0) ? "s" : string.Empty);
				foreach (IModelObject modelObject in modelObjects) {
					foreach (IModelObject mo in Project.Repository.GetModelObjects(modelObject))
						if (mo.ShapeCount > 0) {
							isFeasible = false;
							description = "One or more child model objects are attached to shapes.";
						}
				}
			}
			else {
				isFeasible = false;
				description = "No model objects selected";
			}

			return new DelegateMenuItemDef("Delete", Properties.Resources.DeleteBtn, "DeleteModelObjectsAction",
			                               description, false, isFeasible, Permission.None,
			                               (a, p) => DeleteModelObjects(modelObjects));
		}


		private MenuItemDef CreateCopyModelObjectsAction(Dataweb.NShape.Advanced.IReadOnlyCollection<IModelObject> modelObjects)
		{
			bool isFeasible = (modelObjects != null && modelObjects.Count > 0);
			string description;
			if (isFeasible)
				description = string.Format("Copy {0} model object{1}.", modelObjects.Count,
				                            (modelObjects.Count > 1) ? "s" : string.Empty);
			else description = "No model objects selected";
			return new DelegateMenuItemDef("Copy", Properties.Resources.CopyBtn, "CopyModelObjectsAction",
			                               description, false, isFeasible, Permission.None,
			                               (a, p) => Copy(modelObjects));
		}


		private MenuItemDef CreatePasteModelObjectsAction(Dataweb.NShape.Advanced.IReadOnlyCollection<IModelObject> modelObjects)
		{
			bool isFeasible = (copyPasteBuffer.Count > 0 && modelObjects.Count <= 1);
			string description;
			if (isFeasible)
				description = string.Format("Paste {0} model object{1}.", copyPasteBuffer.Count,
				                            (copyPasteBuffer.Count > 1) ? "s" : string.Empty);
			else description = "No model objects copied.";

			IModelObject parent = null;
			foreach (IModelObject mo in modelObjects) {
				parent = mo;
				break;
			}
			return new DelegateMenuItemDef("Paste", Properties.Resources.PasteBtn, "PasteModelObjectsAction",
			                               description, false, isFeasible, Permission.None,
			                               (a, p) => Paste(parent));
		}


		private MenuItemDef CreateFindShapesAction(Dataweb.NShape.Advanced.IReadOnlyCollection<IModelObject> modelObjects)
		{
			bool isFeasible = (diagramSetController != null);
			string description = "Find and select all assigned shapes.";
			return new DelegateMenuItemDef("Find assigned shapes", Properties.Resources.FindShapes, "FindShapesAction",
			                               description, false, isFeasible, Permission.None,
			                               (a, p) => FindShapes(modelObjects));
		}

		#endregion

		#region Fields

		private DiagramSetController diagramSetController;
		private Project project;
		private List<IModelObject> copyPasteBuffer = new List<IModelObject>();

		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class ModelObjectSelectedEventArgs : EventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public ModelObjectSelectedEventArgs(IEnumerable<IModelObject> selectedModelObjects, bool ensureVisibility)
		{
			if (selectedModelObjects == null) throw new ArgumentNullException("selectedModelObjects");
			this.modelObjects = new List<IModelObject>(selectedModelObjects);
			this.ensureVisibility = ensureVisibility;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerable<IModelObject> SelectedModelObjects
		{
			get { return modelObjects; }
			internal set
			{
				modelObjects.Clear();
				if (value != null) modelObjects.AddRange(value);
			}
		}

		/// <ToBeCompleted></ToBeCompleted>
		public bool EnsureVisibility
		{
			get { return ensureVisibility; }
			internal set { ensureVisibility = value; }
		}

		internal ModelObjectSelectedEventArgs()
		{
			modelObjects = new List<IModelObject>();
			ensureVisibility = false;
		}

		private List<IModelObject> modelObjects;
		private bool ensureVisibility;
	}
}