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


namespace Dataweb.NShape.Controllers {

	/// <summary>
	/// Controller class providing access to the templates of a project.
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(ToolSetController), "ToolSetController.bmp")]
	public class ToolSetController : Component, IDisplayService, IDisposable {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.ToolSetController" />.
		/// </summary>
		public ToolSetController() {
			infoGraphics = Graphics.FromHwnd(IntPtr.Zero);
		}


		/// <summary>
		/// Finalizer of <see cref="T:Dataweb.NShape.Controllers.ToolSetController" />.
		/// </summary>
		~ToolSetController() {
			Dispose();
			infoGraphics.Dispose();
			infoGraphics = null;
		}


		#region [Public] Events

		/// <summary>
		/// Occurs when the content of the toolbox has been cleared.
		/// </summary>
		public event EventHandler Cleared;

		/// <summary>
		/// Occurs when a tool has been selected.
		/// </summary>
		public event EventHandler<ToolEventArgs> ToolSelected;

		/// <summary>
		/// Occurs when a tool has been added to the toolbox.
		/// </summary>
		public event EventHandler<ToolEventArgs> ToolAdded;

		/// <summary>
		/// Occurs when a tool has been removed from the toolbox.
		/// </summary>
		public event EventHandler<ToolEventArgs> ToolRemoved;

		/// <summary>
		/// Occurs when a CurrentTool has been changed.
		/// </summary>
		public event EventHandler<ToolEventArgs> ToolChanged;

		/// <summary>
		/// Occurs when the sesign editor has to be shown.
		/// </summary>
		public event EventHandler DesignEditorSelected;

		/// <summary>
		/// Occurs when the library management dialog has to be shown.
		/// </summary>
		public event EventHandler LibraryManagerSelected;

		/// <summary>
		/// Occurs when the template editor has to be shown.
		/// </summary>
		public event EventHandler<TemplateEditorEventArgs> TemplateEditorSelected;

		#endregion


		#region [Public] Properties

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		public string ProductVersion {
			get { return this.GetType().Assembly.GetName().Version.ToString(); }
		}


		/// <summary>
		/// The <see cref="T:Dataweb.NShape.Controllers.DiagramSetController" /> that uses the SelectedTool for editing diagrams.
		/// </summary>
		[Category("NShape")]
		[Description("Specifies the NShape DiagramSetController to which this toolbox belongs. This is a mandotory property.")]
		public DiagramSetController DiagramSetController {
			get { return diagramSetController; }
			set {
				if (diagramSetController != null) {
					Clear();
					UnregisterDiagramSetControllerEvents();
				}
				diagramSetController = value;
				if (diagramSetController != null)
					RegisterDiagramSetControllerEvents();
			}
		}


		/// <summary>
		/// Provides access to the <see cref="T:Dataweb.NShape.Project" /> of the current <see cref="T:Dataweb.NShape.Controllers.DiagramSetController" />.
		/// </summary>
		[Browsable(false)]
		public Project Project {
			get { return (diagramSetController == null) ? null : diagramSetController.Project; }
		}


		/// <summary>
		/// Provides a collection of tools.
		/// </summary>
		[Browsable(false)]
		public IEnumerable<Tool> Tools {
			get { return tools; }
		}


		/// <summary>
		/// The currently selected tool.
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		public Tool SelectedTool {
			get { return selectedTool; }
			set {
				if (selectedTool != value)
					SelectTool(value);
			}
		}


		/// <summary>
		/// The default tool that will be selected after a tool was executed or canceled.
		/// </summary>
		[Browsable(false)]
		public Tool DefaultTool {
			get { return defaultTool; }
		}

		#endregion


		#region [Public] Methods

		/// <summary>
		/// Removes all tools.
		/// </summary>
		public void Clear() {
			for (int i = tools.Count - 1; i >= 0; --i)
				tools[i].Dispose();
			tools.Clear();
			toolBoxInitialized = false;
			if (Cleared != null) Cleared(this, EventArgs.Empty);
		}


		/// <summary>
		/// Creates a new tool for creating a templated shape.
		/// </summary>
		/// <param name="template">The template of the creation tool.</param>
		public void CreateTemplateTool(Template template) {
			CreateTemplateTool(template, null);
		}


		/// <summary>
		/// Creates a new tool for creating a templated shape.
		/// </summary>
		/// <param name="template">The template of the creation tool.</param>
		/// <param name="categoryTitle">Title of the toolbox category where the tool is inserted</param>
		public void CreateTemplateTool(Template template, string categoryTitle) {
			if (template == null) throw new ArgumentNullException("template");
			if (FindTool(template) == null) {
				Tool tool = null;
				if (template.Shape is ILinearShape) {
					if (string.IsNullOrEmpty(categoryTitle))
						tool = new LinearShapeCreationTool(template);
					else tool = new LinearShapeCreationTool(template, categoryTitle);
				} else {
					if (string.IsNullOrEmpty(categoryTitle))
						tool = new PlanarShapeCreationTool(template);
					else tool = new PlanarShapeCreationTool(template, categoryTitle);
				}
				AddTool(tool);
			}
		}


		/// <summary>
		/// Creates the library indepandent standard tools of teh framework.
		/// </summary>
		public void CreateStandardTools() {
			// First check whether we have a already a pointer tool or a free hand tool.
			bool pointerToolFound = false;
			foreach (Tool t in Tools) {
				if (t is SelectionTool)
					pointerToolFound = true;
					break;
			}
			// If we do not have a pointer tool yet, create one.
			if (!pointerToolFound) AddTool(new SelectionTool(), true);
		}


		/// <summary>
		/// Adds a new tool.
		/// </summary>
		/// <param name="tool"></param>
		public void AddTool(Tool tool) {
			if (tool == null) throw new ArgumentNullException("tool");
			AddTool(tool, tools.Count == 0);
		}


		/// <summary>
		/// Adds a new tool to the toolbox.
		/// </summary>
		/// <param name="tool">CurrentTool to add</param>
		/// <param name="isDefaultTool">If true, this tool becomes the default tool.</param>
		public void AddTool(Tool tool, bool isDefaultTool) {
			if (tool == null) throw new ArgumentNullException("tool");
			tools.Add(tool);
			tool.ToolExecuted += Tool_ToolExecuted;
			if (tool is TemplateTool)
				((TemplateTool)tool).Template.Shape.DisplayService = this;
			tool.RefreshIcons();
			if (isDefaultTool) defaultTool = tool;
			//
			if (ToolAdded != null) {
				toolEventArgs.Tool = tool;
				ToolAdded(this, toolEventArgs);
			}
		}


		/// <summary>
		/// Deletes the given tool.
		/// </summary>
		public void DeleteTool(Tool tool) {
			if (tool == null) throw new ArgumentNullException("tool");
			if (tool == selectedTool) SelectDefaultTool(true);
			if (tool == defaultTool)
				if (tools.Count <= 0) defaultTool = null;
				else defaultTool = tools[0];
			tools.Remove(tool);
			if (ToolRemoved != null) {
				toolEventArgs.Tool = tool;
				ToolRemoved(this, toolEventArgs);
			}
			tool.Dispose();
		}


		/// <summary>
		///  Sets the given tool as the selected tool.
		/// </summary>
		public void SelectTool(Tool tool) {
			if (tool == null) throw new ArgumentNullException("tool");
			SelectTool(tool, false);
		}


		/// <summary>
		/// Selects the given tool.
		/// </summary>
		/// <param name="tool">Tool to select.</param>
		/// <param name="multiUse">If false, the default tool will be selected after executing the tool.</param>
		public void SelectTool(Tool tool, bool multiUse) {
			if (tool == null) throw new ArgumentNullException("tool");
			// If the tool to select equals the currently selected tool, skip tool selection
			if (tool != selectedTool) {
				// CurrentTool.Cancel would normally select the default tool.
				selecting = true;
				try {
					if (selectedTool != null)
						selectedTool.Cancel();
				} finally {
					selecting = false;
				}
				DoSelectTool(tool, multiUse, true);
			}
			executeOnce = !multiUse;
		}


		/// <summary>
		/// Find the tool that contains the given template
		/// </summary>
		public Tool FindTool(Template template) {
			if (template == null) throw new ArgumentNullException("template");
			foreach (Tool t in tools) {
				if (t is TemplateTool && ((TemplateTool)t).Template == template)
					return t;
			}
			return null;
		}


		/// <summary>
		/// Fires the DesignEditorSelected event. 
		/// If it is not handled, or the current role has not the necessary permissions, nothing will happen. 
		/// </summary>
		public void ShowDesignEditor() {
			// Select Default CurrentTool before calling the Editor
			DoSelectTool(defaultTool, false, true);
			OnDesignEditorSelected(EventArgs.Empty);
		}


		/// <summary>
		/// Fires the DesignEditorSelected event. 
		/// If it is not handled, or the current role has not the necessary permissions, nothing will happen. 
		/// </summary>
		public void ShowLibraryManager() {
			// Select Default CurrentTool before calling the Editor
			DoSelectTool(defaultTool, false, true);
			OnLibraryManagerSelected(EventArgs.Empty);
		}


		/// <summary>
		/// Show registered LibraryManagerSelected event for loading library assemblies
		/// </summary>
		public void ShowTemplateEditor(bool editSelectedTemplate) {
			if (editSelectedTemplate) {
				if (selectedTool != null && selectedTool is TemplateTool) {
					TemplateEditorEventArgs e = new TemplateEditorEventArgs(Project, ((TemplateTool)selectedTool).Template);
					DoSelectTool(defaultTool, true, false);
					OnTemplateEditorSelected(e);
				}
			} else {
				TemplateEditorEventArgs e = new TemplateEditorEventArgs(Project);
				DoSelectTool(defaultTool, true, false);
				OnTemplateEditorSelected(e);
			}
		}


		/// <summary>
		/// Deletes the selected tool.
		/// </summary>
		public void DeleteSelectedTemplate() {
			if (selectedTool != null && selectedTool is TemplateTool) {
				Tool t = selectedTool;
				SelectDefaultTool(true);
				tools.Remove(t);
				Project.Repository.DeleteAll(((TemplateTool)t).Template);
				if (ToolRemoved != null) {
					toolEventArgs.Tool = t;
					ToolRemoved(this, toolEventArgs);
				}
			}
		}


		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		public IEnumerable<MenuItemDef> GetMenuItemDefs(Tool clickedTool) {
			// menu structure:
			//
			// Create Template...
			// Edit Template...
			// Delete Template
			// -------------------
			// Show Design Editor
			// Show Library manager

			bool isFeasible;
			string description;
			Template clickedTemplate = null;
			if (clickedTool is TemplateTool) clickedTemplate = ((TemplateTool)clickedTool).Template;

			isFeasible = true;
			description = "Create a new Template";
			yield return new DelegateMenuItemDef("Create Template...", null, description, isFeasible, Permission.Templates,
				(action, project) => OnTemplateEditorSelected(new TemplateEditorEventArgs(project)));

			isFeasible = (clickedTemplate != null);
			description = isFeasible ? string.Format("Edit Template '{0}'", clickedTemplate.Title) :
				"No template tool selected";
			yield return new DelegateMenuItemDef("Edit Template...", null, description, isFeasible, Permission.Templates,
				(action, project) => OnTemplateEditorSelected(new TemplateEditorEventArgs(project, clickedTemplate)));
			
			isFeasible =  (clickedTemplate != null);
			if (!isFeasible) 
				description = "No template tool selected";
			else {
				// Check if template is in use
				isFeasible = !Project.Repository.IsTemplateInUse(clickedTemplate);
				if (isFeasible) description = string.Format("Delete Template '{0}'", clickedTemplate.Title);
				else description = string.Format("Template '{0}' is in use.", clickedTemplate.Title);
			}
			yield return new CommandMenuItemDef("Delete Template...", null, description, isFeasible,
				isFeasible ? new DeleteTemplateCommand(clickedTemplate) : null);

			yield return new SeparatorMenuItemDef();

			isFeasible = true;
			description = "Edit the current design or create new designs";
			yield return new DelegateMenuItemDef("Show Design Editor...", Properties.Resources.DesignEditorBtn,
				description, isFeasible, Permission.Designs,
				(action, project) => OnDesignEditorSelected(EventArgs.Empty));

			isFeasible = true;
			description = "Load and unload shape and/or model libraries";
			yield return new DelegateMenuItemDef("Show Library Manager...", Properties.Resources.LibrariesBtn,
				description, isFeasible, Permission.Templates,
				(action, project) => OnLibraryManagerSelected(EventArgs.Empty));
		}

		#endregion


		#region [Protected] IDisposable Members

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) Clear();
			base.Dispose(disposing);
		}

		#endregion


		#region [Protected] Methods

		/// <summary>
		/// Raises the <see cref="E:Dataweb.NShape.ToolSetController.DesignEditorSelected" /> event.
		/// </summary>
		protected virtual void OnDesignEditorSelected(EventArgs e) {
			if (DesignEditorSelected != null && Project.SecurityManager.IsGranted(Permission.Designs, SecurityAccess.View))
				DesignEditorSelected(this, e);
		}


		/// <summary>
		/// Raises the <see cref="E:Dataweb.NShape.ToolSetController.LibraryManagerSelected" /> event.
		/// </summary>
		protected virtual void OnLibraryManagerSelected(EventArgs e) {
			if (LibraryManagerSelected != null && Project.SecurityManager.IsGranted(Permission.Templates, SecurityAccess.View))
				LibraryManagerSelected(this, e);
		}


		/// <summary>
		/// Raises the <see cref="E:Dataweb.NShape.ToolSetController.TemplateEditorSelected" /> event.
		/// </summary>
		protected virtual void OnTemplateEditorSelected(TemplateEditorEventArgs e) {
			if (TemplateEditorSelected != null && Project.SecurityManager.IsGranted(Permission.Templates, SecurityAccess.View))
				 TemplateEditorSelected(this, e);
		}

		#endregion


		#region [Explicit] IDisplayService Members

		/// <override></override>
		void IDisplayService.Invalidate(int x, int y, int width, int height) { /* nothing to do */ }


		/// <override></override>
		void IDisplayService.Invalidate(Rectangle rectangle) { /* nothing to do */ }


		/// <override></override>
		void IDisplayService.NotifyBoundsChanged() { /* nothing to do */ }


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


		#region [Private] Methods

		private void RefreshTool(Template template) {
			RefreshTool(FindTool(template));
		}


		private void RefreshTool(Tool tool) {
			if (tool != null) {
				tool.RefreshIcons();
				if (tool != null && ToolChanged != null) {
					toolEventArgs.Tool = tool;
					ToolChanged(this, toolEventArgs);
				}
			}
		}


		private void CreateTools() {
			// Create tools for all explicitly loaded templates
			bool found = false;
			if (Project.Repository != null && Project.Repository.IsOpen) {
				foreach (Template template in Project.Repository.GetTemplates()) {
					// Don't create the tool if it already exists
					found = false;
					foreach (Tool t in tools) {
						if (t is TemplateTool && ((TemplateTool)t).Template == template) {
							found = true;
							break;
						}
					}
					if (!found) {
						string categoryTitle = template.Shape != null ? template.Shape.Type.DefaultCategoryTitle : string.Empty;
						CreateTemplateTool(template, categoryTitle);
					}
				}
			}
		}


		private void Initialize() {
			if (diagramSetController == null) throw new ArgumentNullException("Property 'DiagramSetController' is not set.");
			if (Project.Repository != null && !repositoryInitialized)
				RegisterRepositoryEvents();
			if (!toolBoxInitialized) {
				CreateStandardTools();
				if (Project.AutoGenerateTemplates)
					CreateTools();
				toolBoxInitialized = true;
			}
			if (defaultTool != null) SelectDefaultTool(true);
		}


		private void Uninitialize() {
			if (repositoryInitialized)
				UnregisterRepositoryEvents();
			Clear();
			toolBoxInitialized = false;
		}


		private void SelectDefaultTool(bool ensureVisibility) {
			if (selectedTool != defaultTool)
				DoSelectTool(defaultTool, false, ensureVisibility);
		}


		private void SelectDefaultTool() {
			SelectDefaultTool(false);
		}


		private void DoSelectTool(Tool tool, bool multiUse, bool ensureVisibility) {
			this.selectedTool = tool;
			this.executeOnce = !multiUse;

			if (diagramSetController != null)
				this.diagramSetController.ActiveTool = this.selectedTool;

			if (ToolSelected != null) {
				toolEventArgs.Tool = tool;
				ToolSelected(this, toolEventArgs);
			}
		}

		#endregion


		#region [Private] Methods: Registering for events

		private void RegisterDiagramSetControllerEvents() {
			Debug.Assert(diagramSetController != null);
			diagramSetController.ProjectChanged += diagramSetController_ProjectChanged;
			diagramSetController.ProjectChanging += diagramSetController_ProjectChanging;
			if (diagramSetController.Project != null) RegisterProjectEvents();
		}


		private void UnregisterDiagramSetControllerEvents() {
			diagramSetController.ProjectChanged -= diagramSetController_ProjectChanged;
			diagramSetController.ProjectChanging -= diagramSetController_ProjectChanging;
			if (diagramSetController.Project != null) UnregisterProjectEvents();
		}


		private void RegisterProjectEvents() {
			diagramSetController.Project.LibraryLoaded += Project_LibraryLoaded;
			diagramSetController.Project.Opened += Project_ProjectOpened;
			diagramSetController.Project.Closing += Project_ProjectClosing;
			diagramSetController.Project.Closed += Project_ProjectClosed;
			if (diagramSetController.Project.IsOpen)
				Project_ProjectOpened(this, null);
		}


		private void UnregisterProjectEvents() {
			diagramSetController.Project.LibraryLoaded -= Project_LibraryLoaded;
			diagramSetController.Project.Closing -= Project_ProjectClosing;
			diagramSetController.Project.Closed -= Project_ProjectClosed;
			diagramSetController.Project.Opened -= Project_ProjectOpened;
			if (repositoryInitialized)
				UnregisterRepositoryEvents();
		}


		private void RegisterRepositoryEvents() {
			if (diagramSetController.Project.Repository != null) {
				diagramSetController.Project.Repository.StyleUpdated += Repository_StyleUpdated;
				diagramSetController.Project.Repository.TemplateInserted += Repository_TemplateInserted;
				diagramSetController.Project.Repository.TemplateUpdated += Repository_TemplateUpdated;
				diagramSetController.Project.Repository.TemplateDeleted += Repository_TemplateDeleted;
				diagramSetController.Project.Repository.TemplateShapeReplaced += Repository_TemplateShapeReplaced;
				repositoryInitialized = true;
			}
		}


		private void UnregisterRepositoryEvents() {
			if (Project.Repository != null) {
				diagramSetController.Project.Repository.StyleUpdated -= Repository_StyleUpdated;
				diagramSetController.Project.Repository.TemplateInserted -= Repository_TemplateInserted;
				diagramSetController.Project.Repository.TemplateUpdated -= Repository_TemplateUpdated;
				diagramSetController.Project.Repository.TemplateDeleted -= Repository_TemplateDeleted;
				diagramSetController.Project.Repository.TemplateShapeReplaced -= Repository_TemplateShapeReplaced;
				repositoryInitialized = false;
			}
		}

		#endregion


		#region [Private] Methods: EventHandler implementations

		private void diagramSetController_ProjectChanged(object sender, EventArgs e) {
			if (diagramSetController.Project != null) RegisterProjectEvents();
		}


		private void diagramSetController_ProjectChanging(object sender, EventArgs e) {
			if (diagramSetController.Project != null) UnregisterProjectEvents();
		}


		private void Project_LibraryLoaded(object sender, LibraryLoadedEventArgs e) {
			if (toolBoxInitialized) {
				CreateStandardTools();
				CreateTools();
			}
		}


		private void Project_ProjectOpened(object sender, EventArgs e) {
			Initialize();
		}


		private void Project_ProjectClosing(object sender, EventArgs e) {
			// uninitialize if all other components are cleared
			Uninitialize();
		}


		private void Project_ProjectClosed(object sender, EventArgs e) {
			// nothing to do
		}


		private void Repository_StyleUpdated(object sender, RepositoryStyleEventArgs e) {
			foreach (Tool t in tools) {
				if (t is TemplateTool && ((TemplateTool)t).Template.Shape.NotifyStyleChanged(e.Style))
					RefreshTool(t);
			}
		}


		private void Repository_TemplateInserted(object sender, RepositoryTemplateEventArgs e) {
			string categoryTitle = e.Template.Shape != null ? e.Template.Shape.Type.DefaultCategoryTitle : string.Empty;
			CreateTemplateTool(e.Template, categoryTitle);
		}


		private void Repository_TemplateUpdated(object sender, RepositoryTemplateEventArgs e) {
			RefreshTool(e.Template);
		}


		private void Repository_TemplateShapeReplaced(object sender, RepositoryTemplateShapeReplacedEventArgs e) {
			RefreshTool(e.Template);
		}


		private void Repository_TemplateDeleted(object sender, RepositoryTemplateEventArgs e) {
			Tool t = FindTool(e.Template);
			if (t != null) DeleteTool(t);
		}


		private void Tool_ToolExecuted(object sender, ToolExecutedEventArgs e) {
			switch (e.EventType) {
				case ToolResult.Executed:
					if ((SelectedTool == (Tool)sender) && executeOnce)
						SelectDefaultTool();
					break;
				case ToolResult.Canceled:
					if (!selecting) SelectDefaultTool(true);
					break;
				default: throw new NShapeUnsupportedValueException(e.EventType);
			}
		}

		#endregion


		#region Fields

		// IDisplayService
		private Graphics infoGraphics;
		// Provides the project, receives the selected tool
		private DiagramSetController diagramSetController;

		// Tools in the tool box
		private List<Tool> tools = new List<Tool>();
		// Default tool
		private Tool defaultTool;

		// -- Internal State --
		/// <summary>Indicates that event handlers have been added to the cache.</summary>
		private bool repositoryInitialized = false;
		private bool toolBoxInitialized = false;
		// Currently selected tool
		private Tool selectedTool;
		// Currently selected tool is not for multi-selection
		private bool executeOnce;
		// CurrentTool that was selected before the curernt one
		//private CurrentTool lastSelectedTool;
		// Indicates that we are currently selecting another tool
		private bool selecting;

		private ToolEventArgs toolEventArgs = new ToolEventArgs();

		#endregion
	}


	#region EventArgs

	/// <ToBeCompleted></ToBeCompleted>
	public class ToolEventArgs : EventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public ToolEventArgs(Tool tool) {
			if (tool == null) throw new ArgumentNullException("tool");
			this.tool = tool;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public Tool Tool {
			get { return tool; }
			internal set { tool = value; }
		}

		internal ToolEventArgs() { }

		private Tool tool = null;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class TemplateEditorEventArgs : EventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public TemplateEditorEventArgs(Project project) {
			if (project == null) throw new ArgumentNullException("project");
			this.project = project;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public TemplateEditorEventArgs(Project project, Template template) {
			if (project == null) throw new ArgumentNullException("project");
			this.project = project;
			this.template = template;
		}

		/// <summary>
		/// Provides access to a <see cref="T:Dataweb.NShape.Project" />.
		/// </summary>
		public Project Project {
			get { return project; }
			internal set { project = value; }
		}

		/// <ToBeCompleted></ToBeCompleted>
		public Template Template {
			get { return template; }
			internal set { template = value; }
		}

		private Project project = null;
		private Template template = null;
	}

	#endregion

}