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
	/// Controls the behavior of a set of diagrams.
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(DiagramSetController), "DiagramSetController.bmp")]
	public class DiagramSetController : Component {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.DiagramSetController" />.
		/// </summary>
		public DiagramSetController() {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.DiagramSetController" />.
		/// </summary>
		public DiagramSetController(Project project)
			: this() {
			if (project == null) throw new ArgumentNullException("project");
			Project = project;
		}


		#region [Public] Events

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler ProjectChanging;

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler ProjectChanged;

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler ToolChanged;

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler<ModelObjectsEventArgs> SelectModelObjectsRequested;

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler<DiagramEventArgs> DiagramAdded;

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler<DiagramEventArgs> DiagramRemoved;

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
		/// Provides access to a <see cref="T:Dataweb.NShape.Project" />.
		/// </summary>
		[Category("NShape")]
		public Project Project {
			get { return project; }
			set {
				if (ProjectChanging != null) ProjectChanging(this, EventArgs.Empty);
				if (project != null) UnregisterProjectEvents();
				project = value;
				if (project != null) RegisterProjectEvents();
				if (ProjectChanged != null) ProjectChanged(this, EventArgs.Empty);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Browsable(false)]
		public Tool ActiveTool {
			get { return tool; }
			set {
				tool = value;
				if (ToolChanged != null) ToolChanged(this, EventArgs.Empty);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Browsable(false)]
		public IEnumerable<Diagram> Diagrams {
			get {
				for (int i = 0; i < diagramControllers.Count; ++i)
					yield return diagramControllers[i].Diagram;
			}
		}

		#endregion


		#region [Public] Methods

		/// <ToBeCompleted></ToBeCompleted>
		public Diagram CreateDiagram(string name) {
			if (name == null) throw new ArgumentNullException("name");
			AssertProjectIsOpen();
			// Create new diagram
			Diagram diagram = new Diagram(name);
			diagram.Width = 1000;
			diagram.Height = 1000;
			ICommand cmd = new CreateDiagramCommand(diagram);
			project.ExecuteCommand(cmd);
			return diagram;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void CloseDiagram(string name) {
			if (name == null) throw new ArgumentNullException("name");
			AssertProjectIsOpen();
			int idx = IndexOf(name);
			if (idx >= 0) {
				DiagramEventArgs eventArgs = GetDiagramEventArgs(diagramControllers[idx].Diagram);
				diagramControllers.RemoveAt(idx);
				if (DiagramRemoved != null) DiagramRemoved(this, eventArgs);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void CloseDiagram(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertProjectIsOpen();
			int idx = DiagramControllerIndexOf(diagram);
			if (idx >= 0) {
				DiagramController controller = diagramControllers[idx];
				diagramControllers.RemoveAt(idx);

				DiagramEventArgs eventArgs = GetDiagramEventArgs(controller.Diagram);
				if (DiagramRemoved != null) DiagramRemoved(this, eventArgs);
				controller.Diagram = null;
				controller = null;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void DeleteDiagram(string name) {
			if (name == null) throw new ArgumentNullException("name");
			AssertProjectIsOpen();
			int idx = IndexOf(name);
			if (idx >= 0) {
				DiagramController controller = diagramControllers[idx];
				ICommand cmd = new DeleteDiagramCommand(controller.Diagram);
				project.ExecuteCommand(cmd);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void DeleteDiagram(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertProjectIsOpen();
			int idx = DiagramControllerIndexOf(diagram);
			if (idx >= 0) {
				DiagramController controller = diagramControllers[idx];
				ICommand cmd = new DeleteDiagramCommand(controller.Diagram);
				project.ExecuteCommand(cmd);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void SelectModelObjects(IEnumerable<IModelObject> modelObjects) {
			if (SelectModelObjectsRequested != null) 
				SelectModelObjectsRequested(this, GetModelObjectsEventArgs(modelObjects));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void InsertShape(Diagram diagram, Shape shape, LayerIds activeLayers, bool withModelObjects) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shape == null) throw new ArgumentNullException("shape");
			InsertShapes(diagram, SingleInstanceEnumerator<Shape>.Create(shape), activeLayers, withModelObjects);
		}

		
		/// <ToBeCompleted></ToBeCompleted>
		public void InsertShapes(Diagram diagram, IEnumerable<Shape> shapes, LayerIds activeLayers, bool withModelObjects) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			ICommand cmd = new CreateShapesCommand(diagram, activeLayers, shapes, withModelObjects, true);
			Project.ExecuteCommand(cmd);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void DeleteShapes(Diagram diagram, Shape shape, bool withModelObjects) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shape == null) throw new ArgumentNullException("shape");
			DeleteShapes(diagram, SingleInstanceEnumerator<Shape>.Create(shape), withModelObjects);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void DeleteShapes(Diagram diagram, IEnumerable<Shape> shapes, bool withModelObjects) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			if (withModelObjects) {
				foreach (Shape s in shapes) {
					if (!diagram.Shapes.Contains(s))
						throw new NShapeException("One of the given shapes is not part of the given diagram.");
					if (s.ModelObject != null && s.ModelObject.ShapeCount > 1) {
						string messageText = string.Format("{0} '{1}' can not be deleted while more than one shapes refer to it.",
															s.ModelObject.Type.Name, s.ModelObject.Name);
						throw new NShapeException(messageText);
					}
				}
			}
			ICommand cmd = new DeleteShapesCommand(diagram, shapes, withModelObjects);
			Project.ExecuteCommand(cmd);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Copy(Diagram source, IEnumerable<Shape> shapes, bool withModelObjects) {
			Copy(source, shapes, withModelObjects, Geometry.InvalidPoint);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Copy(Diagram source, IEnumerable<Shape> shapes, bool withModelObjects, Point startPos) {
			if (source == null) throw new ArgumentNullException("source");
			if (shapes == null) throw new ArgumentNullException("shapes");

			editBuffer.Clear();
			editBuffer.action = EditAction.Copy;
			editBuffer.withModelObjects = withModelObjects;
			editBuffer.initialMousePos = startPos;
			editBuffer.shapes.AddRange(shapes);
			
			// We have to copy the shapes immediately because shapes (and/or model objects) may 
			// be deleted after they are copied to 'clipboard'.
			// Copy shapes:
			// Use the ShapeCollection's Clone method in order to maintain connections 
			// between shapes inside the collection
			editBuffer.shapes = editBuffer.shapes.Clone(withModelObjects);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Cut(Diagram source, IEnumerable<Shape> shapes, bool withModelObjects) {
			Cut(source, shapes, withModelObjects, Geometry.InvalidPoint);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Cut(Diagram source, IEnumerable<Shape> shapes, bool withModelObjects, Point startPos) {
			if (source == null) throw new ArgumentNullException("source");
			if (shapes == null) throw new ArgumentNullException("shapes");

			editBuffer.Clear();
			editBuffer.action = EditAction.Cut;
			editBuffer.withModelObjects = withModelObjects;
			editBuffer.initialMousePos = startPos;
			editBuffer.shapes.AddRange(shapes);
			// Store all connections of *active* shapes to other shapes that were also cut
			foreach (Shape s in editBuffer.shapes) {
				foreach (ShapeConnectionInfo ci in s.GetConnectionInfos(ControlPointId.Any, null)) {
					// Skip shapes that are not the active shapes
					if (s.HasControlPointCapability(ci.OwnPointId, ControlPointCapabilities.Glue)) continue;
					// Skip connections to shapes that were not cut
					if (!editBuffer.shapes.Contains(ci.OtherShape)) continue;

					ShapeConnection conn = ShapeConnection.Empty;
					conn.ConnectorShape = s;
					conn.GluePointId = ci.OwnPointId;
					conn.TargetShape = ci.OtherShape;
					conn.TargetPointId = ci.OtherPointId;
					editBuffer.connections.Add(conn);
				}
			}

			ICommand cmd = new DeleteShapesCommand(source, editBuffer.shapes, withModelObjects);
			project.ExecuteCommand(cmd);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Paste(Diagram destination, LayerIds activeLayers) {
			Paste(destination, activeLayers, 20, 20);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Paste(Diagram destination, LayerIds activeLayers, Point position) {
			if (!editBuffer.IsEmpty) {
				int dx = 40, dy = 40;
				if (Geometry.IsValid(position)) {
					Rectangle rect = editBuffer.shapes.GetBoundingRectangle(true);
					dx = position.X - (rect.X + (rect.Width / 2));
					dy = position.Y - (rect.Y + (rect.Height / 2));
				}
				Paste(destination, activeLayers, dx, dy);
				editBuffer.initialMousePos = position;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Paste(Diagram destination, LayerIds activeLayers, int offsetX, int offsetY) {
			if (destination == null) throw new ArgumentNullException("destination");
			if (!editBuffer.IsEmpty) {
				++editBuffer.pasteCount;

				// Check if there are connections to restore
				if (editBuffer.connections.Count > 0) {
					// Restore connections of cut shapes at first paste action.
					for (int i = editBuffer.connections.Count - 1; i >= 0; --i) {
						editBuffer.connections[i].ConnectorShape.Connect(
							editBuffer.connections[i].GluePointId,
							editBuffer.connections[i].TargetShape,
							editBuffer.connections[i].TargetPointId);
					}
					// After the paste action, the (then connected) shapes are cloned 
					// with their connections, so we can empty the buffer here.
					editBuffer.connections.Clear();
				}

				// Create command
				ICommand cmd = new CreateShapesCommand(
					destination,
					activeLayers,
					editBuffer.shapes,
					editBuffer.withModelObjects,
					(editBuffer.action == EditAction.Cut),
					offsetX,
					offsetY);
				// Execute InsertCommand and select inserted shapes
				Project.ExecuteCommand(cmd);

				// Clone shapes for another paste operation
				// We have to copy the shapes immediately because shapes (and/or model objects) may 
				// be deleted after they are copied to 'clipboard'.
				editBuffer.shapes = editBuffer.shapes.Clone(editBuffer.withModelObjects);
				if (editBuffer.action == EditAction.Cut) editBuffer.action = EditAction.Copy;
			}
		}


		/// <summary>
		/// Aggregates the given shapes to a group.
		/// </summary>
		public void GroupShapes(Diagram diagram, IEnumerable<Shape> shapes, LayerIds activeLayers) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			int cnt = 0;
			foreach (Shape s in shapes)
				if (++cnt > 1) break;
			if (cnt > 1) {
				ShapeType groupShapeType = Project.ShapeTypes["ShapeGroup"];
				Debug.Assert(groupShapeType != null);

				Shape groupShape = groupShapeType.CreateInstance();
				ICommand cmd = new GroupShapesCommand(diagram, activeLayers, groupShape, shapes);
				Project.ExecuteCommand(cmd);
			}
		}


		/// <summary>
		/// Aggregate the given shapes to a composite shape based on the bottom shape.
		/// </summary>
		public void AggregateCompositeShape(Diagram diagram, Shape compositeShape, IEnumerable<Shape> shapes, LayerIds activeLayers) {
			if (compositeShape == null) throw new ArgumentNullException("compositeShape");
			if (shapes == null) throw new ArgumentNullException("shapes");
			// Add shapes to buffer (TopDown)
			shapeBuffer.Clear();
			foreach (Shape shape in shapes) {
				if (shape == compositeShape) continue;
				shapeBuffer.Add(shape);
			}
			ICommand cmd = new AggregateCompositeShapeCommand(diagram, activeLayers, compositeShape, shapeBuffer);
			Project.ExecuteCommand(cmd);
			shapeBuffer.Clear();
		}


		/// <summary>
		/// Ungroups the given shape group
		/// </summary>
		public void UngroupShapes(Diagram diagram, Shape groupShape) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (groupShape == null) throw new ArgumentNullException("groupShape");
			if (!(groupShape is IShapeGroup)) throw new ArgumentException(string.Format("groupShape does not implpement interface {0}", typeof(IShapeGroup).Name));
			// Add grouped shapes to shape buffer for selecting them later
			shapeBuffer.Clear();
			shapeBuffer.AddRange(groupShape.Children);

			ICommand cmd = new UngroupShapesCommand(diagram, groupShape);
			Project.ExecuteCommand(cmd);

			shapeBuffer.Clear();
		}


		/// <summary>
		/// Splits the given composite shape into independent shapes.
		/// </summary>
		public void SplitCompositeShape(Diagram diagram, Shape compositeShape) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (compositeShape == null) throw new ArgumentNullException("compositeShape");
			if (compositeShape == null) throw new ArgumentNullException("compositeShape");
			Debug.Assert(!(compositeShape is IShapeGroup));
			// Add grouped shapes to shape buffer for selecting them later
			shapeBuffer.Clear();
			shapeBuffer.AddRange(compositeShape.Children);
			shapeBuffer.Add(compositeShape);

			ICommand cmd = new SplitCompositeShapeCommand(diagram, diagram.GetShapeLayers(compositeShape), compositeShape);
			Project.ExecuteCommand(cmd);

			shapeBuffer.Clear();
		}


		/// <summary>
		/// Adds the given shapes to the given layers.
		/// </summary>
		public void AddShapesToLayers(Diagram diagram, IEnumerable<Shape> shapes, LayerIds layerIds) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			
			ICommand cmd = new AddShapesToLayersCommand(diagram, shapes, layerIds);
			Project.ExecuteCommand(cmd);
		}


		/// <summary>
		/// Assigns the given shapes to the given layers. If the shape was assigned to layers, these will be replaced.
		/// </summary>
		public void AssignShapesToLayers(Diagram diagram, IEnumerable<Shape> shapes, LayerIds layerIds) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");

			ICommand cmd = new AssignShapesToLayersCommand(diagram, shapes, layerIds);
			Project.ExecuteCommand(cmd);
		}


		/// <summary>
		/// Removes the given shapes from all layers.
		/// </summary>
		public void RemoveShapesFromLayers(Diagram diagram, IEnumerable<Shape> shapes) {
			RemoveShapesFromLayers(diagram, shapes, LayerIds.All);
		}


		/// <summary>
		/// Removes the given shapes from the given layers.
		/// </summary>
		public void RemoveShapesFromLayers(Diagram diagram, IEnumerable<Shape> shapes, LayerIds layerIds) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");

			ICommand cmd = new RemoveShapesFromLayersCommand(diagram, shapes);
			Project.ExecuteCommand(cmd);
		}


		/// <summary>
		/// Lists one shape on top or to bottom
		/// </summary>
		public void LiftShape(Diagram diagram, Shape shape, ZOrderDestination liftMode) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shape == null) throw new ArgumentNullException("shape");
			ICommand cmd = null;
			cmd = new LiftShapeCommand(diagram, shape, liftMode);
			Project.ExecuteCommand(cmd);
		}


		/// <summary>
		/// Lifts a collection of shapes on top or to bottom.
		/// </summary>
		public void LiftShapes(Diagram diagram, IEnumerable<Shape> shapes, ZOrderDestination liftMode) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			ICommand cmd = new LiftShapeCommand(diagram, shapes, liftMode);
			Project.ExecuteCommand(cmd);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanInsertShapes(Diagram diagram, IEnumerable<Shape> shapes, out string reason) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			reason = null;
			if (!Project.SecurityManager.IsGranted(Permission.Insert, shapes)) {
				reason = string.Format("Permission {0} not granted.", Permission.Insert);
				return false;
			} else if (diagram.Shapes.ContainsAny(shapes)) {
				reason = "Diagram already containsat least one sof the shapes to be inserted.";
				return false;
			} else return true;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanInsertShapes(Diagram diagram, IEnumerable<Shape> shapes) {
			string reason;
			return CanInsertShapes(diagram, shapes, out reason);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanDeleteShapes(Diagram diagram, IEnumerable<Shape> shapes) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			return Project.SecurityManager.IsGranted(Permission.Delete, shapes)
				&& diagram.Shapes.ContainsAll(shapes);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanCut(Diagram diagram, IEnumerable<Shape> shapes) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			return CanCopy(shapes) && CanDeleteShapes(diagram, shapes);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanCopy(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			// Check if shapes is not an empty collection
			foreach (Shape s in shapes) return true;
			return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanPaste(Diagram diagram) {
			string reason;
			return CanPaste(diagram, out reason);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanPaste(Diagram diagram, out string reason) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			reason = null;
			if (editBuffer.IsEmpty) {
				reason = "No shapes cut/copied.";
				return false;
			} else {
				if (editBuffer.action != EditAction.Copy)
					if (!CanInsertShapes(diagram, editBuffer.shapes))
						return false;
				if (!Project.SecurityManager.IsGranted(Permission.Insert, editBuffer.shapes)) {
					reason = string.Format("Permission '{0}' not granted.", Permission.Insert);
					return false;
				} else return true;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanGroupShapes(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			int cnt= 0;
			foreach (Shape shape in shapes) {
				++cnt;
				if (cnt >= 2) return Project.SecurityManager.IsGranted(Permission.Delete, shapes);
			}
			return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanUngroupShape(Diagram diagram, IEnumerable<Shape> shapes, out string reason) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			foreach (Shape shape in shapes) {
				if (shape is IShapeGroup && shape.Parent == null)
					return CanInsertShapes(diagram, shape.Children, out reason);
				else {
					reason = "Shape is not a group shape.";
					return false;
				}
			}
			reason = "No shapes to ungroup";
			return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanUngroupShape(Diagram diagram, IEnumerable<Shape> shapes) {
			string reason;
			return CanUngroupShape(diagram, shapes, out reason);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanAggregateShapes(Diagram diagram, IReadOnlyShapeCollection shapes) {
			string reason;
			return CanAggregateShapes(diagram, shapes, out reason);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanAggregateShapes(Diagram diagram, IReadOnlyShapeCollection shapes, out string reason) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			reason = null;
			Shape compositeShape = shapes.Bottom;
			if (shapes.Count <= 1)
				reason = "No shapes selected.";
			else if (shapes.Count <= 1)
				reason = "Not enough shapes selected.";
			else if (!CanDeleteShapes(diagram, shapes))
				reason = string.Format("Permission {0} is not granted.", Permission.Delete);
			else if (!Project.SecurityManager.IsGranted(Permission.Present, compositeShape))
				reason = string.Format("Permission {0} is not granted.", Permission.Present);
			else if (compositeShape is IShapeGroup)
				reason = "Groups cannot be aggregated.";
			return string.IsNullOrEmpty(reason);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanSplitShapeAggregation(Diagram diagram, IReadOnlyShapeCollection shapes) {
			string reason;
			return CanSplitShapeAggregation(diagram, shapes, out reason);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanSplitShapeAggregation(Diagram diagram, IReadOnlyShapeCollection shapes, out string reason) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			reason = null;
			Shape compositeShape = shapes.TopMost;
			if (shapes.Count == 0)
				reason = "No shapes selected.";
			else if (shapes.Count > 1)
				reason = "Too many shapes selected.";
			else if (compositeShape is IShapeGroup)
				reason = "Groups cannot be disaggregated.";
			else if (!CanInsertShapes(diagram, compositeShape.Children))
				reason = string.Format("Permission {0} is not granted.", Permission.Insert);
			else if (!Project.SecurityManager.IsGranted(Permission.Present, compositeShape))
				reason = string.Format("Permission {0} is not granted.", Permission.Present);
			return string.IsNullOrEmpty(reason);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanLiftShapes(Diagram diagram, IEnumerable<Shape> shapes) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			return Project.SecurityManager.IsGranted(Permission.Layout, shapes)
				&& diagram.Shapes.ContainsAll(shapes);
		}
		
		#endregion


		#region [Internal] Types

		internal enum EditAction { None, Copy, Cut }


		internal class EditBuffer {

			public EditBuffer() {
				action = EditAction.None;
				initialMousePos = Geometry.InvalidPoint;
				pasteCount = 0;
				shapes = new ShapeCollection();
				connections = new List<ShapeConnection>();
			}

			public bool IsEmpty {
				get { return shapes.Count == 0; }
			}
			
			public void Clear() {
				initialMousePos = Geometry.InvalidPoint;
				action = EditAction.None;
				pasteCount = 0;
				shapes.Clear();
				connections.Clear();
			}

			public Point initialMousePos;

			public EditAction action;

			public int pasteCount;

			public bool withModelObjects;

			public ShapeCollection shapes;

			public List<ShapeConnection> connections;
		}

		#endregion


		#region [Internal] Properties

		internal IReadOnlyCollection<DiagramController> DiagramControllers {
			get { return diagramControllers; }
		}

		#endregion


		#region [Public/Internal] Methods

		// ToDo: Make these methods protected internal as soon as the WinFormsUI.Display class 
		// is split into DiagramPresenter and Display:IDiagramView
		/// <ToBeCompleted></ToBeCompleted>
		public DiagramController OpenDiagram(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			return DoAddDiagramController(diagram);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DiagramController OpenDiagram(string name) {
			if (name == null) throw new ArgumentNullException("name");
			AssertProjectIsOpen();
			// Try to find diagram with given name
			Diagram diagram = null;
			foreach (Diagram d in project.Repository.GetDiagrams()) {
				if (string.Compare(d.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0) {
					diagram = d;
					break;
				}
			}
			// If a suitable diagram was found, create a diagramController for it
			if (diagram == null) return null;
			else return DoAddDiagramController(diagram);
		}

		#endregion


		#region [Private] Methods: Registering event handlers

		private void RegisterProjectEvents() {
			project.Opened += project_ProjectOpen;
			project.Closed += project_ProjectClosed;
			if (project.IsOpen) RegisterRepositoryEvents();
		}


		private void UnregisterProjectEvents(){
			project.Opened -= project_ProjectOpen;
			project.Closed -= project_ProjectClosed;
		}


		private void RegisterRepositoryEvents() {
			project.Repository.DiagramInserted += Repository_DiagramInserted;
			project.Repository.DiagramDeleted += Repository_DiagramDeleted;
			
			project.Repository.DesignUpdated += Repository_DesignUpdated;

			project.Repository.TemplateShapeReplaced += Repository_TemplateShapeReplaced;

			project.Repository.ShapesInserted += Repository_ShapesInserted;
			project.Repository.ShapesDeleted += Repository_ShapesDeleted;
		}


		private void UnregisterRepositoryEvents() {
			project.Repository.DiagramInserted -= Repository_DiagramInserted;
			project.Repository.DiagramDeleted -= Repository_DiagramDeleted;
			
			project.Repository.DesignUpdated -= Repository_DesignUpdated;
			
			project.Repository.TemplateShapeReplaced -= Repository_TemplateShapeReplaced;

			project.Repository.ShapesInserted -= Repository_ShapesInserted;
			project.Repository.ShapesDeleted -= Repository_ShapesDeleted;
		}

		#endregion


		#region [Private] Methods: Event handler implementations

		private void project_ProjectClosed(object sender, EventArgs e) {
			UnregisterRepositoryEvents();
		}

		
		private void project_ProjectOpen(object sender, EventArgs e) {
			Debug.Assert(project.Repository != null);
			RegisterRepositoryEvents();
		}

	
		private void Repository_DesignUpdated(object sender, RepositoryDesignEventArgs e) {
			// nothing to do
		}


		private void Repository_DiagramDeleted(object sender, RepositoryDiagramEventArgs e) {
			CloseDiagram(e.Diagram);
		}


		private void Repository_DiagramInserted(object sender, RepositoryDiagramEventArgs e) {
			// nothing to do
		}


		private void Repository_TemplateShapeReplaced(object sender, RepositoryTemplateShapeReplacedEventArgs e) {
			// Nothing to do here... Should be done by the ReplaceShapeCommand

			//foreach (Diagram diagram in Diagrams) {
			//   foreach (Shape oldShape in diagram.Shapes) {
			//      if (oldShape.Template == e.Template) {
			//         Shape newShape = e.Template.CreateShape();
			//         newShape.CopyFrom(oldShape);
			//         diagram.Shapes.Replace(oldShape, newShape);
			//      }
			//   }
			//}
		}


		private void Repository_ShapesDeleted(object sender, RepositoryShapesEventArgs e) {
			// Check if the deleted shapes still exists in its diagram and remove them in this case
			foreach (Shape s in e.Shapes) {
				if (s.Diagram != null) {
					Diagram d = s.Diagram;
					d.Shapes.Remove(s);
				}
			}
		}


		private void Repository_ShapesInserted(object sender, RepositoryShapesEventArgs e) {
			// Insert shapes that are not yet part of its diagram
			foreach (Shape shape in e.Shapes) {
				Diagram d = e.GetDiagram(shape);
				if (d != null && !d.Shapes.Contains(shape))
					d.Shapes.Add(shape);
			}
		}

		#endregion


		#region [Private] Methods

		private void AssertProjectIsOpen() {
			if (project == null) throw new NShapePropertyNotSetException(this, "Project");
			if (!project.IsOpen) throw new NShapeException("Project is not open.");
		}


		private int IndexOf(string name) {
			for (int i = diagramControllers.Count - 1; i >= 0; --i) {
				if (string.Compare(diagramControllers[i].Diagram.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
					return i;
			}
			return -1;
		}
		
		
		private int DiagramControllerIndexOf(Diagram diagram) {
			for (int i = diagramControllers.Count - 1; i >= 0; --i) {
				if (diagramControllers[i].Diagram == diagram)
					return i;
			}
			return -1;
		}


		private DiagramController DoAddDiagramController(Diagram diagram) {
			//if (DiagramControllerIndexOf(diagram) >= 0) throw new ArgumentException("The diagram was already opened.");
			int controllerIdx = DiagramControllerIndexOf(diagram);
			if (controllerIdx < 0) {
				DiagramController controller = new DiagramController(this, diagram);
				diagramControllers.Add(controller);
				if (DiagramAdded != null) DiagramAdded(this, GetDiagramEventArgs(controller.Diagram));
				return controller;
			} else return diagramControllers[controllerIdx];
		}


		private DiagramEventArgs GetDiagramEventArgs(Diagram diagram) {
			diagramEventArgs.Diagram = diagram;
			return diagramEventArgs;
		}


		private DiagramShapeEventArgs GetShapeEventArgs(Shape shape, Diagram diagram) {
			if (shape == null) throw new ArgumentNullException("shape");
			diagramShapeEventArgs.SetDiagramShapes(shape, diagram);
			return diagramShapeEventArgs;
		}


		private DiagramShapeEventArgs GetShapeEventArgs(IEnumerable<Shape> shapes, Diagram diagram) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			diagramShapeEventArgs.SetDiagramShapes(shapes, diagram);
			return diagramShapeEventArgs;
		}


		private ModelObjectsEventArgs GetModelObjectsEventArgs(IEnumerable<IModelObject> modelObjects) {
			modelObjectEventArgs.SetModelObjects(modelObjects);
			return modelObjectEventArgs;
		}

		#endregion


		#region Fields

		private Project project = null;
		private Tool tool;
		private ReadOnlyList<DiagramController> diagramControllers = new ReadOnlyList<DiagramController>();

		// Cut'n'Paste buffers
		private EditBuffer editBuffer = new EditBuffer();		// Buffer for Copy/Cut/Paste-Actions
		private Rectangle copyCutBounds = Rectangle.Empty;
		private Point copyCutMousePos = Point.Empty;
		// Other buffers
		private List<Shape> shapeBuffer = new List<Shape>();
		private List<IModelObject> modelBuffer = new List<IModelObject>();
		// EventArgs buffers
		private DiagramEventArgs diagramEventArgs = new DiagramEventArgs();
		private DiagramShapeEventArgs diagramShapeEventArgs = new DiagramShapeEventArgs();
		private ModelObjectsEventArgs modelObjectEventArgs = new ModelObjectsEventArgs();

		#endregion
	}


	#region Enums

	/// <summary>
	/// Specifies the draw mode of selection indicators and grips.
	/// </summary>
	public enum IndicatorDrawMode { 
		/// <ToBeCompleted></ToBeCompleted>
		Normal,
		/// <ToBeCompleted></ToBeCompleted>
		Highlighted,
		/// <ToBeCompleted></ToBeCompleted>
		Deactivated
	};


	/// <summary>
	/// This is the NShape representation of <see cref="T:System.Windows.Forms.MouseButtons" /> (Framework 2.0)
	/// </summary>
	[Flags]
	public enum MouseButtonsDg {
		/// <summary>No mouse button was pressed.</summary>
		None = 0,
		/// <summary>The left mouse button was pressed.</summary>
		Left = 0x100000,
		/// <summary>The right mouse button was pressed.</summary>
		Right = 0x200000,
		/// <summary>The middle mouse button was pressed.</summary>
		Middle = 0x400000,
		/// <summary>The first XButton was pressed.</summary>
		ExtraButton1 = 0x800000,
		/// <summary>The second XButton was pressed.</summary>
		ExtraButton2 = 0x1000000,
	}


	/// <summary>
	/// Specifies the kind of mouse event.
	/// </summary>
	public enum MouseEventType { 
		/// <summary>A mouse button was pressed.</summary>
		MouseDown,
		/// <summary>The mouse was moved.</summary>
		MouseMove,
		/// <summary>A mouse button was released.</summary>
		MouseUp 
	};


	/// <summary>
	/// Specifies the kind of key event.
	/// </summary>
	public enum KeyEventType { 
		/// <summary>A key was pressed down.</summary>
		KeyDown, 
		/// <summary>A key was pressed.</summary>
		KeyPress, 
		/// <summary>A key was released.</summary>
		KeyUp, 
		/// <summary>A key is going to be pressed.</summary>
		PreviewKeyDown 
	}


	/// <summary>
	/// Specifies key codes and modifiers. This is the NShape representation of <see cref="T:System.Windows.Forms.Keys" /> (Framework 2.0)
	/// </summary>
	[Flags]
	public enum KeysDg {
		/// <summary>The A key.</summary>
		A = 0x41,
		/// <summary>The add key.</summary>
		Add = 0x6b,
		/// <summary>The ALT modifier key.</summary>
		Alt = 0x40000,
		/// <summary>The application key (Microsoft Natural Keyboard).</summary>
		Apps = 0x5d,
		/// <summary>The ATTN key.</summary>
		Attn = 0xf6,
		/// <summary>The B key.</summary>
		B = 0x42,
		/// <summary>The BACKSPACE key.</summary>
		Back = 8,
		/// <summary>The browser back key (Windows 2000 or later).</summary>
		BrowserBack = 0xa6,
		/// <summary>The browser favorites key (Windows 2000 or later).</summary>
		BrowserFavorites = 0xab,
		/// <summary>The browser forward key (Windows 2000 or later).</summary>
		BrowserForward = 0xa7,
		/// <summary>The browser home key (Windows 2000 or later).</summary>
		BrowserHome = 0xac,
		/// <summary>The browser refresh key (Windows 2000 or later).</summary>
		BrowserRefresh = 0xa8,
		/// <summary>The browser search key (Windows 2000 or later).</summary>
		BrowserSearch = 170,
		/// <summary>The browser stop key (Windows 2000 or later).</summary>
		BrowserStop = 0xa9,
		/// <summary>The C key.</summary>
		C = 0x43,
		/// <summary>The CANCEL key.</summary>
		Cancel = 3,
		/// <summary>The CAPS LOCK key.</summary>
		Capital = 20,
		/// <summary>The CAPS LOCK key.</summary>
		CapsLock = 20,
		/// <summary>The CLEAR key.</summary>
		Clear = 12,
		/// <summary>The CTRL modifier key.</summary>
		Control = 0x20000,
		/// <summary>The CTRL key.</summary>
		ControlKey = 0x11,
		/// <summary>The CRSEL key.</summary>
		Crsel = 0xf7,
		/// <summary>The D key.</summary>
		D = 0x44,
		/// <summary>The 0 key.</summary>
		D0 = 0x30,
		/// <summary>The 1 key.</summary>
		D1 = 0x31,
		/// <summary>The 2 key.</summary>
		D2 = 50,
		/// <summary>The 3 key.</summary>
		D3 = 0x33,
		/// <summary>The 4 key.</summary>
		D4 = 0x34,
		/// <summary>The 5 key.</summary>
		D5 = 0x35,
		/// <summary>The 6 key.</summary>
		D6 = 0x36,
		/// <summary>The 7 key.</summary>
		D7 = 0x37,
		/// <summary>The 8 key.</summary>
		D8 = 0x38,
		/// <summary>The 9 key.</summary>
		D9 = 0x39,
		/// <summary>The decimal key.</summary>
		Decimal = 110,
		/// <summary>The DEL key.</summary>
		Delete = 0x2e,
		/// <summary>The divide key.</summary>
		Divide = 0x6f,
		/// <summary>The DOWN ARROW key.</summary>
		Down = 40,
		/// <summary>The E key.</summary>
		E = 0x45,
		/// <summary>The END key.</summary>
		End = 0x23,
		/// <summary>The ENTER key.</summary>
		Enter = 13,
		/// <summary>The ERASE EOF key.</summary>
		EraseEof = 0xf9,
		/// <summary>The ESC key.</summary>
		Escape = 0x1b,
		/// <summary>The EXECUTE key.</summary>
		Execute = 0x2b,
		/// <summary>The EXSEL key.</summary>
		Exsel = 0xf8,
		/// <summary>The F key.</summary>
		F = 70,
		/// <summary>The F1 key.</summary>
		F1 = 0x70,
		/// <summary>The F10 key.</summary>
		F10 = 0x79,
		/// <summary>The F11 key.</summary>
		F11 = 0x7a,
		/// <summary>The F12 key.</summary>
		F12 = 0x7b,
		/// <summary>The F13 key.</summary>
		F13 = 0x7c,
		/// <summary>The F14 key.</summary>
		F14 = 0x7d,
		/// <summary>The F15 key.</summary>
		F15 = 0x7e,
		/// <summary>The F16 key.</summary>
		F16 = 0x7f,
		/// <summary>The F17 key.</summary>
		F17 = 0x80,
		/// <summary>The F18 key.</summary>
		F18 = 0x81,
		/// <summary>The F19 key.</summary>
		F19 = 130,
		/// <summary>The F2 key.</summary>
		F2 = 0x71,
		/// <summary>The F20 key.</summary>
		F20 = 0x83,
		/// <summary>The F21 key.</summary>
		F21 = 0x84,
		/// <summary>The F22 key.</summary>
		F22 = 0x85,
		/// <summary>The F23 key.</summary>
		F23 = 0x86,
		/// <summary>The F24 key.</summary>
		F24 = 0x87,
		/// <summary>The F3 key.</summary>
		F3 = 0x72,
		/// <summary>The F4 key.</summary>
		F4 = 0x73,
		/// <summary>The F5 key.</summary>
		F5 = 0x74,
		/// <summary>The F6 key.</summary>
		F6 = 0x75,
		/// <summary>The F7 key.</summary>
		F7 = 0x76,
		/// <summary>The F8 key.</summary>
		F8 = 0x77,
		/// <summary>The F9 key.</summary>
		F9 = 120,
		/// <summary>The IME final mode key.</summary>
		FinalMode = 0x18,
		/// <summary>The G key.</summary>
		G = 0x47,
		/// <summary>The H key.</summary>
		H = 0x48,
		/// <summary>The IME Hanguel mode key. (maintained for compatibility; use HangulMode) </summary>
		HanguelMode = 0x15,
		/// <summary>The IME Hangul mode key.</summary>
		HangulMode = 0x15,
		/// <summary>The IME Hanja mode key.</summary>
		HanjaMode = 0x19,
		/// <summary>The HELP key.</summary>
		Help = 0x2f,
		/// <summary>The HOME key.</summary>
		Home = 0x24,
		/// <summary>The I key.</summary>
		I = 0x49,
		/// <summary>The IME accept key, replaces <see cref="F:System.Windows.Forms.Keys.IMEAceept"></see>.</summary>
		IMEAccept = 30,
		/// <summary>The IME accept key. Obsolete, use <see cref="F:System.Windows.Forms.Keys.IMEAccept"></see> instead.</summary>
		IMEAceept = 30,
		/// <summary>The IME convert key.</summary>
		IMEConvert = 0x1c,
		/// <summary>The IME mode change key.</summary>
		IMEModeChange = 0x1f,
		/// <summary>The IME nonconvert key.</summary>
		IMENonconvert = 0x1d,
		/// <summary>The INS key.</summary>
		Insert = 0x2d,
		/// <summary>The J key.</summary>
		J = 0x4a,
		/// <summary>The IME Junja mode key.</summary>
		JunjaMode = 0x17,
		/// <summary>The K key.</summary>
		K = 0x4b,
		/// <summary>The IME Kana mode key.</summary>
		KanaMode = 0x15,
		/// <summary>The IME Kanji mode key.</summary>
		KanjiMode = 0x19,
		/// <summary>The bitmask to extract a key code from a key value.</summary>
		KeyCode = 0xffff,
		/// <summary>The L key.</summary>
		L = 0x4c,
		/// <summary>The start application one key (Windows 2000 or later).</summary>
		LaunchApplication1 = 0xb6,
		/// <summary>The start application two key (Windows 2000 or later).</summary>
		LaunchApplication2 = 0xb7,
		/// <summary>The launch mail key (Windows 2000 or later).</summary>
		LaunchMail = 180,
		/// <summary>The left mouse button.</summary>
		LButton = 1,
		/// <summary>The left CTRL key.</summary>
		LControlKey = 0xa2,
		/// <summary>The LEFT ARROW key.</summary>
		Left = 0x25,
		/// <summary>The LINEFEED key.</summary>
		LineFeed = 10,
		/// <summary>The left ALT key.</summary>
		LMenu = 0xa4,
		/// <summary>The left SHIFT key.</summary>
		LShiftKey = 160,
		/// <summary>The left Windows logo key (Microsoft Natural Keyboard).</summary>
		LWin = 0x5b,
		/// <summary>The M key.</summary>
		M = 0x4d,
		/// <summary>The middle mouse button (three-button mouse).</summary>
		MButton = 4,
		/// <summary>The media next track key (Windows 2000 or later).</summary>
		MediaNextTrack = 0xb0,
		/// <summary>The media play pause key (Windows 2000 or later).</summary>
		MediaPlayPause = 0xb3,
		/// <summary>The media previous track key (Windows 2000 or later).</summary>
		MediaPreviousTrack = 0xb1,
		/// <summary>The media Stop key (Windows 2000 or later).</summary>
		MediaStop = 0xb2,
		/// <summary>The ALT key.</summary>
		Menu = 0x12,
		/// <summary>The bitmask to extract modifiers from a key value.</summary>
		Modifiers = -65536,
		/// <summary>The multiply key.</summary>
		Multiply = 0x6a,
		/// <summary>The N key.</summary>
		N = 0x4e,
		/// <summary>The PAGE DOWN key.</summary>
		Next = 0x22,
		/// <summary>A constant reserved for future use.</summary>
		NoName = 0xfc,
		/// <summary>No key pressed.</summary>
		None = 0,
		/// <summary>The NUM LOCK key.</summary>
		NumLock = 0x90,
		/// <summary>The 0 key on the numeric keypad.</summary>
		NumPad0 = 0x60,
		/// <summary>The 1 key on the numeric keypad.</summary>
		NumPad1 = 0x61,
		/// <summary>The 2 key on the numeric keypad.</summary>
		NumPad2 = 0x62,
		/// <summary>The 3 key on the numeric keypad.</summary>
		NumPad3 = 0x63,
		/// <summary>The 4 key on the numeric keypad.</summary>
		NumPad4 = 100,
		/// <summary>The 5 key on the numeric keypad.</summary>
		NumPad5 = 0x65,
		/// <summary>The 6 key on the numeric keypad.</summary>
		NumPad6 = 0x66,
		/// <summary>The 7 key on the numeric keypad.</summary>
		NumPad7 = 0x67,
		/// <summary>The 8 key on the numeric keypad.</summary>
		NumPad8 = 0x68,
		/// <summary>The 9 key on the numeric keypad.</summary>
		NumPad9 = 0x69,
		/// <summary>The O key.</summary>
		O = 0x4f,
		/// <summary>The OEM 1 key.</summary>
		Oem1 = 0xba,
		/// <summary>The OEM 102 key.</summary>
		Oem102 = 0xe2,
		/// <summary>The OEM 2 key.</summary>
		Oem2 = 0xbf,
		/// <summary>The OEM 3 key.</summary>
		Oem3 = 0xc0,
		/// <summary>The OEM 4 key.</summary>
		Oem4 = 0xdb,
		/// <summary>The OEM 5 key.</summary>
		Oem5 = 220,
		/// <summary>The OEM 6 key.</summary>
		Oem6 = 0xdd,
		/// <summary>The OEM 7 key.</summary>
		Oem7 = 0xde,
		/// <summary>The OEM 8 key.</summary>
		Oem8 = 0xdf,
		/// <summary>The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000 or later).</summary>
		OemBackslash = 0xe2,
		/// <summary>The CLEAR key.</summary>
		OemClear = 0xfe,
		/// <summary>The OEM close bracket key on a US standard keyboard (Windows 2000 or later).</summary>
		OemCloseBrackets = 0xdd,
		/// <summary>The OEM comma key on any country/region keyboard (Windows 2000 or later).</summary>
		Oemcomma = 0xbc,
		/// <summary>The OEM minus key on any country/region keyboard (Windows 2000 or later).</summary>
		OemMinus = 0xbd,
		/// <summary>The OEM open bracket key on a US standard keyboard (Windows 2000 or later).</summary>
		OemOpenBrackets = 0xdb,
		/// <summary>The OEM period key on any country/region keyboard (Windows 2000 or later).</summary>
		OemPeriod = 190,
		/// <summary>The OEM pipe key on a US standard keyboard (Windows 2000 or later).</summary>
		OemPipe = 220,
		/// <summary>The OEM plus key on any country/region keyboard (Windows 2000 or later).</summary>
		Oemplus = 0xbb,
		/// <summary>The OEM question mark key on a US standard keyboard (Windows 2000 or later).</summary>
		OemQuestion = 0xbf,
		/// <summary>The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).</summary>
		OemQuotes = 0xde,
		/// <summary>The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).</summary>
		OemSemicolon = 0xba,
		/// <summary>The OEM tilde key on a US standard keyboard (Windows 2000 or later).</summary>
		Oemtilde = 0xc0,
		/// <summary>The P key.</summary>
		P = 80,
		/// <summary>The PA1 key.</summary>
		Pa1 = 0xfd,
		/// <summary>Used to pass Unicode characters as if they were keystrokes. The Packet key value is the low word of a 32-bit virtual-key value used for non-keyboard input methods.</summary>
		Packet = 0xe7,
		/// <summary>The PAGE DOWN key.</summary>
		PageDown = 0x22,
		/// <summary>The PAGE UP key.</summary>
		PageUp = 0x21,
		/// <summary>The PAUSE key.</summary>
		Pause = 0x13,
		/// <summary>The PLAY key.</summary>
		Play = 250,
		/// <summary>The PRINT key.</summary>
		Print = 0x2a,
		/// <summary>The PRINT SCREEN key.</summary>
		PrintScreen = 0x2c,
		/// <summary>The PAGE UP key.</summary>
		Prior = 0x21,
		/// <summary>The PROCESS KEY key.</summary>
		ProcessKey = 0xe5,
		/// <summary>The Q key.</summary>
		Q = 0x51,
		/// <summary>The R key.</summary>
		R = 0x52,
		/// <summary>The right mouse button.</summary>
		RButton = 2,
		/// <summary>The right CTRL key.</summary>
		RControlKey = 0xa3,
		/// <summary>The RETURN key.</summary>
		Return = 13,
		/// <summary>The RIGHT ARROW key.</summary>
		Right = 0x27,
		/// <summary>The right ALT key.</summary>
		RMenu = 0xa5,
		/// <summary>The right SHIFT key.</summary>
		RShiftKey = 0xa1,
		/// <summary>The right Windows logo key (Microsoft Natural Keyboard).</summary>
		RWin = 0x5c,
		/// <summary>The S key.</summary>
		S = 0x53,
		/// <summary>The SCROLL LOCK key.</summary>
		Scroll = 0x91,
		/// <summary>The SELECT key.</summary>
		Select = 0x29,
		/// <summary>The select media key (Windows 2000 or later).</summary>
		SelectMedia = 0xb5,
		/// <summary>The separator key.</summary>
		Separator = 0x6c,
		/// <summary>The SHIFT modifier key.</summary>
		Shift = 0x10000,
		/// <summary>The SHIFT key.</summary>
		ShiftKey = 0x10,
		/// <summary>The computer sleep key.</summary>
		Sleep = 0x5f,
		/// <summary>The PRINT SCREEN key.</summary>
		Snapshot = 0x2c,
		/// <summary>The SPACEBAR key.</summary>
		Space = 0x20,
		/// <summary>The subtract key.</summary>
		Subtract = 0x6d,
		/// <summary>The T key.</summary>
		T = 0x54,
		/// <summary>The TAB key.</summary>
		Tab = 9,
		/// <summary>The U key.</summary>
		U = 0x55,
		/// <summary>The UP ARROW key.</summary>
		Up = 0x26,
		/// <summary>The V key.</summary>
		V = 0x56,
		/// <summary>The volume down key (Windows 2000 or later).</summary>
		VolumeDown = 0xae,
		/// <summary>The volume mute key (Windows 2000 or later).</summary>
		VolumeMute = 0xad,
		/// <summary>The volume up key (Windows 2000 or later).</summary>
		VolumeUp = 0xaf,
		/// <summary>The W key.</summary>
		W = 0x57,
		/// <summary>The X key.</summary>
		X = 0x58,
		/// <summary>The first x mouse button (five-button mouse).</summary>
		XButton1 = 5,
		/// <summary>The second x mouse button (five-button mouse).</summary>
		XButton2 = 6,
		/// <summary>The Y key.</summary>
		Y = 0x59,
		/// <summary>The Z key.</summary>
		Z = 90,
		/// <summary>The ZOOM key.</summary>
		Zoom = 0xfb
	}

	#endregion


	#region EventArgs

	/// <summary>Provides data for mouse events.</summary>
	public class MouseEventArgsDg : EventArgs {

		/// <summary>
		/// Initializing a new instance of <see cref="T:Dataweb.NShape.Controllers.MouseEventArgsDg" />.
		/// </summary>
		public MouseEventArgsDg(MouseEventType eventType, MouseButtonsDg buttons, int clicks, int delta, Point location, KeysDg modifiers) {
			this.buttons = buttons;
			this.clicks = clicks;
			this.wheelDelta = delta;
			this.eventType = eventType;
			this.position = location;
			this.modifiers = modifiers;
		}


		/// <summary>
		/// Contains the type of MouseEvent that was raised.
		/// </summary>
		public MouseEventType EventType {
			get { return eventType; }
		}


		/// <summary>
		/// Contains a combination of all MouseButtons that were pressed.
		/// </summary>
		public MouseButtonsDg Buttons {
			get { return buttons; }
		}


		/// <summary>
		/// Contains the number of clicks.
		/// </summary>
		public int Clicks {
			get { return clicks; }
		}


		/// <summary>
		/// Contains a (signed) count of the number of detents the mouse wheel was rotated.
		/// A detent is one notch of the mouse wheel.
		/// </summary>
		public int WheelDelta {
			get { return wheelDelta; }
		}


		/// <summary>
		/// Contains the position (in diagram coordinates) of the mouse cursor at the time the event was raised.
		/// </summary>
		public Point Position {
			get { return position; }
		}


		/// <summary>
		/// Contains the modifiers in case any modifier keys were pressed
		/// </summary>
		public KeysDg Modifiers {
			get { return modifiers; }
		}


		/// <summary>
		/// Initializes an new empty instance of <see cref="T:Dataweb.NShape.Controllers.MouseEventArgsDg" />.
		/// </summary>
		protected internal MouseEventArgsDg() {
			this.buttons = MouseButtonsDg.None;
			this.clicks = 0;
			this.wheelDelta = 0;
			this.eventType = MouseEventType.MouseMove;
			this.position = Point.Empty;
		}


		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected MouseEventType eventType;
		/// <ToBeCompleted></ToBeCompleted>
		protected MouseButtonsDg buttons;
		/// <ToBeCompleted></ToBeCompleted>
		protected Point position;
		/// <ToBeCompleted></ToBeCompleted>
		protected int wheelDelta;
		/// <ToBeCompleted></ToBeCompleted>
		protected int clicks;
		/// <ToBeCompleted></ToBeCompleted>
		protected KeysDg modifiers;
		
		#endregion
	}


	/// <summary>Provides data for keyboard events.</summary>
	public class KeyEventArgsDg : EventArgs {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.KeyEventArgsDg" />.
		/// </summary>
		public KeyEventArgsDg(KeyEventType eventType, int keyData, char keyChar, bool handled, bool suppressKeyPress) {
			this.eventType = eventType;
			this.handled = handled;
			this.keyChar = keyChar;
			this.keyData = keyData;
			this.suppressKeyPress = suppressKeyPress;
		}


		/// <summary>Specifies the kind of key event.</summary>
		public KeyEventType EventType {
			get { return eventType; }
		}


		/// <summary>Gets the character corresponding to the key pressed.</summary>
		public char KeyChar {
			get { return keyChar; }
		}


		/// <summary>Gets the key data for a keyboard event.</summary>
		public int KeyData {
			get { return keyData; }
		}


		/// <summary>Gets or sets a value indicating whether the event was handled.</summary>
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}


		/// <summary>Gets or sets a value indicating whether the key event should be passed on to the underlying control.</summary>
		public bool SuppressKeyPress {
			get { return suppressKeyPress; }
			set { suppressKeyPress = value; }
		}


		/// <summary>Gets a value indicating whether the CTRL key was pressed.</summary>
		public bool Control {
			get { return (keyData & control) == control; }
		}


		/// <summary>Gets or sets a value indicating whether the key event should be passed on to the underlying control</summary>
		public bool Shift {
			get { return (keyData & shift) == shift; }
		}


		/// <summary>Gets a value indicating whether the ALT key was pressed.</summary>
		public bool Alt {
			get { return (keyData & alt) == alt; }
		}


		/// <summary>Gets the keyboard code for a keyboard event.</summary>
		public int KeyCode {
			get { return keyData & keyCode; }
		}


		/// <summary>Specifies the currently pressed modifier keys.</summary>
		public int Modifiers {
			get { return (keyData & ~keyCode); }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal KeyEventArgsDg() {
			this.eventType = KeyEventType.PreviewKeyDown;
			this.handled = false;
			this.keyChar = '\0';
			this.keyData = 0;
			this.suppressKeyPress = false;
		}


		/// <summary>
		/// The bitmask to extract modifiers from a key value.
		/// </summary>
		protected const int modifiers = -65536;

	
		/// <summary>
		/// The bitmask to extract a key code from a key value.
		/// </summary>
		protected const int keyCode = ushort.MaxValue;

		
		/// <summary>
		/// The SHIFT modifier key.
		/// </summary>
		protected const int shift = ushort.MaxValue;

		
		/// <summary>
		/// The CTRL modifier key.
		/// </summary>
		protected const int control = 131072;

		
		/// <summary>
		/// The ALT modifier key.
		/// </summary>
		protected const int alt = 262144;


		/// <ToBeCompleted></ToBeCompleted>
		protected KeyEventType eventType;
		/// <ToBeCompleted></ToBeCompleted>
		protected char keyChar;
		/// <ToBeCompleted></ToBeCompleted>
		protected int keyData;
		/// <ToBeCompleted></ToBeCompleted>
		protected bool handled;
		/// <ToBeCompleted></ToBeCompleted>
		protected bool suppressKeyPress;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DiagramEventArgs : EventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public DiagramEventArgs(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public Diagram Diagram {
			get { return diagram; }
			internal set { diagram = value; }
		}

		internal DiagramEventArgs() { }

		private Diagram diagram;

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class ShapeEventArgs : EventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public ShapeEventArgs(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			this.shapes.AddRange(shapes);
		}

		/// <ToBeCompleted></ToBeCompleted>
		public IReadOnlyCollection<Shape> Shapes {
			get { return shapes; }
		}

		internal ShapeEventArgs() {
			this.shapes.Clear();
		}

		internal void SetShapes(IEnumerable<Shape> shapes) {
			this.shapes.Clear();
			this.shapes.AddRange(shapes);
		}

		internal void SetShape(Shape shape) {
			this.shapes.Clear();
			this.shapes.Add(shape);
		}

		private ReadOnlyList<Shape> shapes = new ReadOnlyList<Shape>();
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class ModelObjectsEventArgs : EventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public ModelObjectsEventArgs(IEnumerable<IModelObject> modelObjects) {
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			this.modelObjects.AddRange(modelObjects);
		}

		/// <ToBeCompleted></ToBeCompleted>
		public IReadOnlyCollection<IModelObject> ModelObjects {
			get { return modelObjects; }
		}

		internal ModelObjectsEventArgs() {
			this.modelObjects.Clear();
		}

		internal void SetModelObjects(IEnumerable<IModelObject> modelObjects) {
			this.modelObjects.Clear();
			this.modelObjects.AddRange(modelObjects);
		}

		internal void SetModelObject(IModelObject modelObject) {
			this.modelObjects.Clear();
			this.modelObjects.Add(modelObject);
		}

		private ReadOnlyList<IModelObject> modelObjects = new ReadOnlyList<IModelObject>();
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DiagramShapeEventArgs : ShapeEventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public DiagramShapeEventArgs(IEnumerable<Shape> shapes, Diagram diagram)
			: base(shapes) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Diagram Diagram {
			get { return diagram; }
		}


		internal DiagramShapeEventArgs()
			: base() {
		}


		internal void SetDiagram(Diagram diagram) {
			this.diagram = diagram;
		}
		
		
		internal void SetDiagramShapes(IEnumerable<Shape> shapes, Diagram diagram) {
			SetShapes(shapes);
			SetDiagram(diagram);
		}


		internal void SetDiagramShapes(Shape shape, Diagram diagram) {
			SetShape(shape);
			SetDiagram(diagram);
		}


		private Diagram diagram;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class ShapeMouseEventArgs : MouseEventArgsDg {

		/// <ToBeCompleted></ToBeCompleted>
		public ShapeMouseEventArgs(IEnumerable<Shape> shapes, Diagram diagram, MouseEventType eventType, MouseButtonsDg buttons, int clicks, int delta, Point location, KeysDg modifiers)
			: base(eventType, buttons, clicks, delta, location, modifiers) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			this.shapes.AddRange(shapes);
			this.diagram = diagram;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public IReadOnlyCollection<Shape> Shapes { get { return shapes; } }

		internal ShapeMouseEventArgs()
			: base() {
			this.shapes.Clear();
		}

		internal void SetShapes(IEnumerable<Shape> shapes) {
			this.shapes.Clear();
			this.shapes.AddRange(shapes);
		}

		internal void SetShape(Shape shape) {
			this.shapes.Clear();
			this.shapes.Add(shape);
		}

		private ReadOnlyList<Shape> shapes = new ReadOnlyList<Shape>();
		private Diagram diagram = null;
	}

	#endregion


	#region Exceptions

	/// <ToBeCompleted></ToBeCompleted>
	public class DiagramControllerNotFoundException : NShapeException {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.DiagramControllerNotFoundException" />.
		/// </summary>
		public DiagramControllerNotFoundException(Diagram diagram)
			: base("No {0} found for {1} '{2}'", typeof(DiagramController).Name, typeof(Diagram).Name, (diagram != null) ? diagram.Name : string.Empty) {
		}

	}

	#endregion

}
