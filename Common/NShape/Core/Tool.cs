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
using System.Diagnostics;
using System.Drawing;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;
using Dataweb.NShape.Controllers;


namespace Dataweb.NShape {

	/// <summary>
	/// Specifies the outcome of a tool execution.
	/// </summary>
	/// <status>reviewed</status>
	public enum ToolResult {
		/// <summary>Tool was successfully executed</summary>
		Executed,
		/// <summary>Tool was canceled</summary>
		Canceled
	}


	/// <summary>
	/// Describes how a tool was executed.
	/// </summary>
	/// <status>reviewed</status>
	public class ToolExecutedEventArgs : EventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public ToolExecutedEventArgs(Tool tool, ToolResult eventType)
			: base() {
			if (tool == null) throw new ArgumentNullException("tool");
			this.tool = tool;
			this.eventType = eventType;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Tool Tool {
			get { return tool; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ToolResult EventType {
			get { return eventType; }
		}


		private Tool tool;
		private ToolResult eventType;

	}


	/// <summary>
	/// Controls a user operation on a diagram.
	/// </summary>
	/// <status>reviewed</status>
	public abstract class Tool : IDisposable {

		#region IDisposable Members

		/// <summary>
		/// Releases all resources.
		/// </summary>
		public virtual void Dispose() {
			if (smallIcon != null)
				smallIcon.Dispose();
			smallIcon = null;

			if (largeIcon != null)
				largeIcon.Dispose();
			largeIcon = null;
		}

		#endregion


		/// <summary>
		/// A culture independent name, used as title if no title was specified.
		/// </summary>
		public string Name {
			get { return name; }
		}


		/// <summary>
		/// A culture dependent title, used as label for toolbox items.
		/// </summary>
		public string Title {
			get { return title; }
			set { title = value; }
		}


		/// <summary>
		/// A culture dependent description, used as description for toolbox items.
		/// </summary>
		public virtual string Description {
			// TODO 2: Remove this implementation, when all derived classes have a better one.
			get { return description; }
			set { description = value; }
		}


		/// <summary>
		/// Specifies the toolbox category.
		/// </summary>
		public string Category {
			get { return category; }
			set { category = value; }
		}


		/// <summary>
		/// Specifies the tool tip text displayed for the toolbox item.
		/// </summary>
		public string ToolTipText {
			get { return Description; }
			set { Description = value; }
		}


		/// <summary>
		/// A small icon for this <see cref="T:Dataweb.NShape.Advanced.Tool" />.
		/// </summary>
		public Bitmap SmallIcon {
			get { return smallIcon; }
			set { smallIcon = value; }
		}


		/// <summary>
		/// A large icon for this <see cref="T:Dataweb.NShape.Advanced.Tool" />.
		/// </summary>
		public Bitmap LargeIcon {
			get { return largeIcon; }
			set { largeIcon = value; }
		}


		/// <summary>
		/// Specifies the minimum distance the mouse has to be moved before the tool is starting a drag action.
		/// </summary>
		public Size MinimumDragDistance {
			get { return minDragDistance; }
			set { minDragDistance = value; }
		}
		
		
		/// <summary>
		/// Called by the <see cref="T:Dataweb.NShape.Advanced.Controllers.IDiagramPresenter" /> when the mouse has entered the control's area.
		/// </summary>
		public abstract void EnterDisplay(IDiagramPresenter diagramPresenter);


		/// <summary>
		/// Called by the <see cref="T:Dataweb.NShape.Advanced.Controllers.IDiagramPresenter" /> when the mouse has left the control's area.
		/// </summary>
		public abstract void LeaveDisplay(IDiagramPresenter diagramPresenter);


		/// <summary>
		/// Processes a mouse event.
		/// </summary>
		/// <param name="diagramPresenter">Diagram presenter where the event occurred.</param>
		/// <param name="e">Description of the mouse event.</param>
		/// <remarks>When overriding, the base classes method has to be called at the end.</remarks>
		/// <returns>True if the event was handled, false if the event was not handled.</returns>
		public virtual bool ProcessMouseEvent(IDiagramPresenter diagramPresenter, MouseEventArgsDg e) {
			if (diagramPresenter == null) throw new ArgumentNullException("display");
			currentMouseState.Buttons = e.Buttons;
			currentMouseState.Modifiers = e.Modifiers;
			diagramPresenter.ControlToDiagram(e.Position, out currentMouseState.Position);
			diagramPresenter.Update();
			return false;
		}


		/// <summary>
		/// Processes a keyboard event.
		/// </summary>
		/// <param name="diagramPresenter">Diagram presenter where the event occurred.</param>
		/// <param name="e">Description of the keyboard event.</param>
		/// <returns>True if the event was handled, false if the event was not handled.</returns>
		public virtual bool ProcessKeyEvent(IDiagramPresenter diagramPresenter, KeyEventArgsDg e) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			bool result = false;
			switch (e.EventType) {
				case KeyEventType.KeyDown:
					// Cancel tool
					if (e.KeyCode == (int)KeysDg.Escape) {
						Cancel();
						result = true;
					}
					break;

				case KeyEventType.KeyPress:
				case KeyEventType.PreviewKeyDown:
				case KeyEventType.KeyUp:
					// do nothing
					break;
				default: throw new NShapeUnsupportedValueException(e.EventType);
			}
			diagramPresenter.Update();
			return result;
		}


		/// <summary>
		/// Sets protected readonly-properties to invalid values and raises the ToolExecuted event.
		/// </summary>
		public void Cancel() {
			// End the tool's action
			while (IsToolActionPending)
				EndToolAction();

			// Reset the tool's state
			CancelCore();

			currentMouseState = MouseState.Empty;

			OnToolExecuted(CancelledEventArgs);
		}


		/// <summary>
		/// Specifis if the tool wants the diagram presenter to scroll when reaching the presenter's bounds.
		/// </summary>
		public virtual bool WantsAutoScroll {
			get {
				if (pendingActions.Count == 0) return false;
				else return pendingActions.Peek().WantsAutoScroll;
			}
		}


		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		public abstract IEnumerable<MenuItemDef> GetMenuItemDefs(IDiagramPresenter diagramPresenter);


		/// <summary>
		/// Request redrawing.
		/// </summary>
		public abstract void Invalidate(IDiagramPresenter diagramPresenter);


		/// <summary>
		/// Draws the tool's preview.
		/// </summary>
		/// <param name="diagramPresenter"></param>
		public abstract void Draw(IDiagramPresenter diagramPresenter);


		/// <summary>
		/// Redraw icons.
		/// </summary>
		public abstract void RefreshIcons();


		/// <ToBeCompleted></ToBeCompleted>
		[Obsolete]
		public bool ToolActionPending {
			get { return IsToolActionPending; }
		}


		/// <summary>
		/// Indicates if the tool has pending actions.
		/// </summary>
		public bool IsToolActionPending {
			get { return pendingActions.Count > 0; }
		}


		/// <summary>
		/// Occurs when the tool was executed or canceled.
		/// </summary>
		public event EventHandler<ToolExecutedEventArgs> ToolExecuted;


		#region [Protected] Construction and Destruction

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.Tool" />.
		/// </summary>
		protected Tool() {
			smallIcon = new Bitmap(16, 16);
			largeIcon = new Bitmap(32, 32);
			name = "Tool_" + this.GetHashCode().ToString();
			ExecutedEventArgs = new ToolExecutedEventArgs(this, ToolResult.Executed);
			CancelledEventArgs = new ToolExecutedEventArgs(this, ToolResult.Canceled);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.Tool" />.
		/// </summary>
		protected Tool(string category)
			: this() {
			if (!string.IsNullOrEmpty(category))
				this.category = category;
		}


		/// <summary>
		/// Finalizer of <see cref="T:Dataweb.NShape.Advanced.Tool" />.
		/// </summary>
		~Tool() {
			Dispose();
		}

		#endregion


		#region [Protected] Properties

		/// <summary>
		/// Current state of the mouse (state after the last ProcessMouseEvent call).
		/// Position is in Diagram coordinates.
		/// </summary>
		protected MouseState CurrentMouseState {
			get { return currentMouseState; }
		}


		/// <summary>
		/// The display used by the current (pending) action.
		/// </summary>
		protected IDiagramPresenter ActionDiagramPresenter {
			get {
				if (pendingActions.Count == 0) throw new NShapeException("The action's current display was not set yet. Call StartToolAction method to set the action's current display.");
				else return pendingActions.Peek().DiagramPresenter;
			}
		}


		/// <summary>
		/// Transformed start coordinates of the current (pending) action (diagram coordinates).
		/// Use SetActionStartPosition method to set this value and ClearActionStartPosition to clear it.
		/// </summary>
		protected MouseState ActionStartMouseState {
			get {
				if (pendingActions.Count == 0) throw new NShapeInternalException("The action's start mouse state was not set yet. Call SetActionStartPosition method to set the start position.");
				else return pendingActions.Peek().MouseState;
			}
		}


		/// <summary>
		/// Specifies the current tool action.
		/// </summary>
		protected ActionDef CurrentToolAction {
			get {
				if (pendingActions.Count > 0)
					return pendingActions.Peek();
				else return ActionDef.Empty;
			}
		}


		/// <summary>
		/// Indicates whether a tool action is pending.
		/// </summary>
		protected IEnumerable<ActionDef> PendingToolActions {
			get { return pendingActions; }
		}


		/// <summary>
		/// Specifies the number of pending tool actions.
		/// </summary>
		protected int PendingToolActionsCount {
			get { return pendingActions.Count; }
		}


		/// <summary>
		/// Buffer for <see cref="T:Dataweb.NShape.Advanced.ToolExecutedEventArgs" /> in order to minimize memory allocation overhead.
		/// </summary>
		protected ToolExecutedEventArgs ExecutedEventArgs;


		/// <summary>
		/// Buffer for <see cref="T:Dataweb.NShape.Advanced.ToolExecutedEventArgs" /> in order to minimize memory allocation overhead.
		/// </summary>
		protected ToolExecutedEventArgs CancelledEventArgs;

		#endregion


		#region [Protected] Methods (overridable)

		/// <summary>
		/// Sets the start coordinates for an action as well as the display to use for the action.
		/// </summary>
		protected virtual void StartToolAction(IDiagramPresenter diagramPresenter, int action, MouseState mouseState, bool wantAutoScroll) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (mouseState == MouseState.Empty) throw new ArgumentException("mouseState");
			if (pendingActions.Count > 0) {
				if (pendingActions.Peek().DiagramPresenter != diagramPresenter)
					throw new NShapeException("There are actions pending for an other diagram presenter!");
			}
			ActionDef actionDef = ActionDef.Create(diagramPresenter, action, mouseState, wantAutoScroll);
			pendingActions.Push(actionDef);
		}


		/// <summary>
		/// Ends a tool's action. Crears the start position for the action and the display used for the action.
		/// </summary>
		protected virtual void EndToolAction() {
			if (pendingActions.Count <= 0) throw new InvalidOperationException("No tool actions pending.");
			IDiagramPresenter diagramPresenter = pendingActions.Peek().DiagramPresenter;
			if (diagramPresenter != null) {
				Invalidate(diagramPresenter);
				diagramPresenter.Capture = false;
				diagramPresenter.SetCursor(CursorProvider.DefaultCursorID);
			}
			pendingActions.Pop();
		}


		/// <summary>
		/// Perform all necessary actions to cancel the current tool actions.
		/// </summary>
		protected abstract void CancelCore();


		/// <summary>
		/// Called when a tool action has finished. Raises the ToolExecuted event.
		/// </summary>
		protected virtual void OnToolExecuted(ToolExecutedEventArgs eventArgs) {
			if (IsToolActionPending) throw new InvalidOperationException(string.Format("{0} tool actions pending.", pendingActions.Count));
			if (ToolExecuted != null) ToolExecuted(this, eventArgs);
		}

		#endregion


		#region [Protected] Methods

		/// <summary>
		/// Disconnect, disposes and deletes the given preview <see cref="T:Dataweb.NShape.Advanced.Shape" />.
		/// </summary>
		/// <param name="shape"></param>
		protected void DeletePreviewShape(ref Shape shape) {
			if (shape != null) {
				// Disconnect all existing connections (disconnects model, too)
				foreach (ControlPointId pointId in shape.GetControlPointIds(ControlPointCapabilities.Connect | ControlPointCapabilities.Glue)) {
					ControlPointId otherPointId = shape.IsConnected(pointId, null);
					if (otherPointId != ControlPointId.None) shape.Disconnect(pointId);
				}
				// Delete model obejct
				if (shape.ModelObject != null)
					shape.ModelObject = null;
				// Delete shape
				shape.Invalidate();
				shape.Dispose();
				shape = null;
			}
		}


		/// <summary>
		/// Indicates whether the given shape can connect to the given targetShape with the specified glue point.
		/// </summary>
		protected bool CanConnectTo(Shape shape, ControlPointId gluePointId, Shape targetShape) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (targetShape == null) throw new ArgumentNullException("targetShape");
			// Connecting to a shape via Pointpto-shape connection is not allowed for both ends
			ControlPointId connectedPoint = ControlPointId.None;
			foreach (ShapeConnectionInfo sci in shape.GetConnectionInfos(ControlPointId.Any, targetShape)) {
				if ((sci.OtherPointId == ControlPointId.Reference && sci.OwnPointId != gluePointId)
					|| (sci.OwnPointId == ControlPointId.Reference && targetShape.HasControlPointCapability(sci.OtherPointId, ControlPointCapabilities.Glue)))
					return false;
			}
			return true;
		}


		/// <summary>
		/// Indicates whether the given shape can connect to the given targetShape with the specified glue point.
		/// </summary>
		protected bool CanConnectTo(IDiagramPresenter diagramPresenter, Shape shape, ControlPointId unmovedGluePoint, ControlPointId movedControlPoint, bool onlyUnselected) {
			if (shape is ILinearShape && ((ILinearShape)shape).VertexCount == 2) {
				Point posA = shape.GetControlPointPosition(unmovedGluePoint);
				Point posB = shape.GetControlPointPosition(movedControlPoint);
				ShapeAtCursorInfo shapeInfoA = FindShapeAtCursor(diagramPresenter, posA.X, posA.Y, ControlPointCapabilities.All, diagramPresenter.ZoomedGripSize, onlyUnselected);
				ShapeAtCursorInfo shapeInfoB = FindShapeAtCursor(diagramPresenter, posB.X, posB.Y, ControlPointCapabilities.All, diagramPresenter.ZoomedGripSize, onlyUnselected);
				if (!shapeInfoA.IsEmpty
					&& shapeInfoA.Shape == shapeInfoB.Shape
					&& (shapeInfoA.ControlPointId == ControlPointId.Reference
						|| shapeInfoB.ControlPointId == ControlPointId.Reference))
					return false;
			}
			return true;
		}


		/// <summary>
		/// Indicates whether a grip was hit
		/// </summary>
		protected bool IsGripHit(IDiagramPresenter diagramPresenter, Shape shape, ControlPointId controlPointId, int x, int y) {
			if (shape == null) throw new ArgumentNullException("shape");
			Point p = shape.GetControlPointPosition(controlPointId);
			return IsGripHit(diagramPresenter, p.X, p.Y, x, y);
		}


		/// <summary>
		/// Indicates whether a grip was hit
		/// </summary>
		protected bool IsGripHit(IDiagramPresenter diagramPresenter, int controlPointX, int controlPointY, int x, int y) {
			if (diagramPresenter == null) throw new ArgumentNullException("display");
			return Geometry.DistancePointPoint(controlPointX, controlPointY, x, y) <= diagramPresenter.ZoomedGripSize;
		}


		/// <summary>
		/// Returns the resize modifiers that have to be applied.
		/// </summary>
		protected ResizeModifiers GetResizeModifier(MouseState mouseState) {
			ResizeModifiers result = ResizeModifiers.None;
			if (!mouseState.IsEmpty) {
				if ((mouseState.Modifiers & KeysDg.Shift) == KeysDg.Shift)
					result |= ResizeModifiers.MaintainAspect;
				if ((mouseState.Modifiers & KeysDg.Control) == KeysDg.Control)
					result |= ResizeModifiers.MirroredResize;
			}
			return result;
		}


#if DEBUG_DIAGNOSTICS
		internal void Assert(bool condition) {
			Assert(condition, null);
		}


		internal void Assert(bool condition, string message) {
			if (condition == false) {
				if (string.IsNullOrEmpty(message)) throw new NShapeInternalException("Assertion Failure.");
				else throw new NShapeInternalException(string.Format("Assertion Failure: {0}", message));
			}
		}
#endif

		#endregion


		#region [Protected] Methods (Drawing and Invalidating)

		/// <summary>
		/// Invalidates all connection targets in range.
		/// </summary>
		protected void InvalidateConnectionTargets(IDiagramPresenter diagramPresenter, int currentPosX, int currentPosY) {
			// invalidate selectedShapes in last range
			diagramPresenter.InvalidateGrips(shapesInRange, ControlPointCapabilities.Connect);

			if (Geometry.IsValid(currentPosX, currentPosY)) {
				ShapeAtCursorInfo shapeAtCursor = FindConnectionTargetFromPosition(diagramPresenter, currentPosX, currentPosY, false, true);
				if (!shapeAtCursor.IsEmpty) shapeAtCursor.Shape.Invalidate();

				// invalidate selectedShapes in current range
				shapesInRange.Clear();
				shapesInRange.AddRange(FindVisibleShapes(diagramPresenter, currentPosX, currentPosY, ControlPointCapabilities.Connect, pointHighlightRange));
				if (shapesInRange.Count > 0)
					diagramPresenter.InvalidateGrips(shapesInRange, ControlPointCapabilities.Connect);
			}
		}


		/// <summary>
		/// Draws all connection targets in range.
		/// </summary>
		protected void DrawConnectionTargets(IDiagramPresenter diagramPresenter, Shape shape, int x, int y) {
			if (!Geometry.IsValid(x, y)) throw new ArgumentException("x, y");
			Point p = Point.Empty;
			p.Offset(x, y);
			DrawConnectionTargets(diagramPresenter, shape, ControlPointId.None, p, EmptyEnumerator<Shape>.Empty);
		}


		/// <summary>
		/// Draws all connection targets in range.
		/// </summary>
		protected void DrawConnectionTargets(IDiagramPresenter diagramPresenter, Shape shape, int x, int y, IEnumerable<Shape> excludedShapes) {
			Point p = Point.Empty;
			p.Offset(x, y);
			DrawConnectionTargets(diagramPresenter, shape, ControlPointId.None, p, excludedShapes);
		}


		/// <summary>
		/// Draws all connection targets in range.
		/// </summary>
		protected void DrawConnectionTargets(IDiagramPresenter diagramPresenter, Shape shape, ControlPointId gluePtId, Point newGluePtPos) {
			DrawConnectionTargets(diagramPresenter, shape, gluePtId, newGluePtPos, EmptyEnumerator<Shape>.Empty);
		}


		/// <summary>
		/// Draws all connection targets in range.
		/// </summary>
		protected void DrawConnectionTargets(IDiagramPresenter diagramPresenter, Shape shape, ControlPointId gluePtId, Point newGluePtPos, IEnumerable<Shape> excludedShapes) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (!Geometry.IsValid(newGluePtPos)) throw new ArgumentException("newGluePtPos");
			if (shape == null) throw new ArgumentNullException("shape");
			//if (gluePtId == ControlPointId.None || gluePtId == ControlPointId.Any)
			//   throw new ArgumentException(string.Format("{0} is not a valid {1} for this operation.", gluePtId, typeof(ControlPointId).Name));
			//if (!shape.HasControlPointCapability(gluePtId, ControlPointCapabilities.Glue))
			//   throw new ArgumentException(string.Format("{0} is not a valid glue point.", gluePtId));
			if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Connect, shape))
				return;

			// Find connection target shape at the given position
			ShapeAtCursorInfo shapeAtCursor = ShapeAtCursorInfo.Empty;
			if (shape != null && gluePtId != ControlPointId.None)
				shapeAtCursor = FindConnectionTarget(diagramPresenter, shape, gluePtId, newGluePtPos, false, false);
			else shapeAtCursor = FindConnectionTargetFromPosition(diagramPresenter, newGluePtPos.X, newGluePtPos.Y, false, false);

			// Add shapes in range to the shapebuffer and then remove all excluded shapes
			shapeBuffer.Clear();
			shapeBuffer.AddRange(shapesInRange);
			foreach (Shape excludedShape in excludedShapes) {
				shapeBuffer.Remove(excludedShape);
				if (excludedShape == shapeAtCursor.Shape)
					shapeAtCursor.Clear();
			}

			// If there is no ControlPoint under the Cursor and the cursor is over a shape, draw the shape's outline
			if (!shapeAtCursor.IsEmpty && shapeAtCursor.ControlPointId == ControlPointId.Reference
				&& shapeAtCursor.Shape.ContainsPoint(newGluePtPos.X, newGluePtPos.Y)) {
				diagramPresenter.DrawShapeOutline(IndicatorDrawMode.Highlighted, shapeAtCursor.Shape);
			}

			// Draw all connectionPoints of all shapes in range (except the excluded ones, see above)
			diagramPresenter.ResetTransformation();
			try {
				for (int i = shapeBuffer.Count - 1; i >= 0; --i) {
					foreach (int ptId in shapeBuffer[i].GetControlPointIds(ControlPointCapabilities.Connect)) {
						IndicatorDrawMode drawMode = IndicatorDrawMode.Normal;
						if (shapeBuffer[i] == shapeAtCursor.Shape && ptId == shapeAtCursor.ControlPointId)
							drawMode = IndicatorDrawMode.Highlighted;
						diagramPresenter.DrawConnectionPoint(drawMode, shapeBuffer[i], ptId);
					}
				}
			} finally { diagramPresenter.RestoreTransformation(); }
		}

		#endregion


		#region [Protected] Methods (Finding shapes and points)

		/// <summary>
		/// Finds the nearest snap point for a point.
		/// </summary>
		/// <param name="diagramPresenter">The <see cref="T:Dataweb.NShape.Controllers.IDiagramPresenter" /></param>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="snapDeltaX">Horizontal distance between x and the nearest snap point.</param>
		/// <param name="snapDeltaY">Vertical distance between y and the nearest snap point.</param>
		/// <returns>Distance to nearest snap point.</returns>
		/// <remarks>If snapping is disabled for the current ownerDisplay, this function does nothing.</remarks>
		protected float FindNearestSnapPoint(IDiagramPresenter diagramPresenter, int x, int y, out int snapDeltaX, out int snapDeltaY) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");

			float distance = float.MaxValue;
			snapDeltaX = snapDeltaY = 0;
			if (diagramPresenter.SnapToGrid) {
				// calculate position of surrounding grid lines
				int gridSize = diagramPresenter.GridSize;
				int left = x - (x % gridSize);
				int above = y - (y % gridSize);
				int right = x - (x % gridSize) + gridSize;
				int below = y - (y % gridSize) + gridSize;
				float currDistance = 0;
				int snapDistance = diagramPresenter.SnapDistance;

				// calculate distance from the given point to the surrounding grid lines
				currDistance = y - above;
				if (currDistance <= snapDistance && currDistance >= 0 && currDistance < distance) {
					distance = currDistance;
					snapDeltaY = above - y;
				}
				currDistance = right - x;
				if (currDistance <= snapDistance && currDistance >= 0 && currDistance < distance) {
					distance = currDistance;
					snapDeltaX = right - x;
				}
				currDistance = below - y;
				if (currDistance <= snapDistance && currDistance >= 0 && currDistance < distance) {
					distance = currDistance;
					snapDeltaY = below - y;
				}
				currDistance = x - left;
				if (currDistance <= snapDistance && currDistance >= 0 && currDistance < distance) {
					distance = currDistance;
					snapDeltaX = left - x;
				}

				// calculate approximate distance from the given point to the surrounding grid points
				currDistance = Geometry.DistancePointPoint(x, y, left, above);
				if (currDistance <= snapDistance && currDistance >= 0 && currDistance < distance) {
					distance = currDistance;
					snapDeltaX = left - x;
					snapDeltaY = above - y;
				}
				currDistance = Geometry.DistancePointPoint(x, y, right, above);
				if (currDistance <= snapDistance && currDistance >= 0 && currDistance < distance) {
					distance = currDistance;
					snapDeltaX = right - x;
					snapDeltaY = above - y;
				}
				currDistance = Geometry.DistancePointPoint(x, y, left, below);
				if (currDistance <= snapDistance && currDistance >= 0 && currDistance < distance) {
					distance = currDistance;
					snapDeltaX = left - x;
					snapDeltaY = below - y;
				}
				currDistance = Geometry.DistancePointPoint(x, y, right, below);
				if (currDistance <= snapDistance && currDistance >= 0 && currDistance < distance) {
					distance = currDistance;
					snapDeltaX = right - x;
					snapDeltaY = below - y;
				}
			}
			return distance;
		}


		/// <summary>
		/// Finds the nearest SnapPoint in range of the given shape's control point.
		/// </summary>
		/// <param name="diagramPresenter">The <see cref="T:Dataweb.NShape.Controllers.IDiagramPresenter" /></param>
		/// <param name="shape">The shape for which the nearest snap point is searched.</param>
		/// <param name="controlPointId">The control point of the shape.</param>
		/// <param name="pointOffsetX">Declares the distance, the shape is moved on X axis before finding snap point.</param>
		/// <param name="pointOffsetY">Declares the distance, the shape is moved on X axis before finding snap point.</param>
		/// <param name="snapDeltaX">Horizontal distance between ptX and the nearest snap point.</param>
		/// <param name="snapDeltaY">Vertical distance between ptY and the nearest snap point.</param>
		/// <returns>Distance to nearest snap point.</returns>
		protected float FindNearestSnapPoint(IDiagramPresenter diagramPresenter, Shape shape, ControlPointId controlPointId,
			int pointOffsetX, int pointOffsetY, out int snapDeltaX, out int snapDeltaY) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (shape == null) throw new ArgumentNullException("shape");

			snapDeltaX = snapDeltaY = 0;
			Point p = shape.GetControlPointPosition(controlPointId);
			return FindNearestSnapPoint(diagramPresenter, p.X + pointOffsetX, p.Y + pointOffsetY, out snapDeltaX, out snapDeltaY);
		}


		/// <summary>
		/// Finds the nearest SnapPoint in range of the given shape.
		/// </summary>
		/// <param name="diagramPresenter">The <see cref="T:Dataweb.NShape.Controllers.IDiagramPresenter" /></param>
		/// <param name="shape">The shape for which the nearest snap point is searched.</param>
		/// <param name="shapeOffsetX">Declares the distance, the shape is moved on X axis before finding snap point.</param>
		/// <param name="shapeOffsetY">Declares the distance, the shape is moved on X axis before finding snap point.</param>
		/// <param name="snapDeltaX">Horizontal distance between ptX and the nearest snap point.</param>
		/// <param name="snapDeltaY">Vertical distance between ptY and the nearest snap point.</param>
		/// <returns>Distance to the calculated snap point.</returns>
		protected float FindNearestSnapPoint(IDiagramPresenter diagramPresenter, Shape shape, int shapeOffsetX, int shapeOffsetY,
			out int snapDeltaX, out int snapDeltaY) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (shape == null) throw new ArgumentNullException("shape");

			snapDeltaX = snapDeltaY = 0;
			int snapDistance = diagramPresenter.SnapDistance;
			float lowestDistance = float.MaxValue;

			Rectangle shapeBounds = shape.GetBoundingRectangle(true);
			shapeBounds.Offset(shapeOffsetX, shapeOffsetY);
			int boundsCenterX = (int)Math.Round(shapeBounds.X + shapeBounds.Width / 2f);
			int boundsCenterY = (int)Math.Round(shapeBounds.Y + shapeBounds.Width / 2f);

			int dx, dy;
			float currDistance;
			// Calculate snap distance of center point
			currDistance = FindNearestSnapPoint(diagramPresenter, boundsCenterX, boundsCenterY, out dx, out dy);
			if (currDistance < lowestDistance && currDistance >= 0 && currDistance <= snapDistance) {
				lowestDistance = currDistance;
				snapDeltaX = dx;
				snapDeltaY = dy;
			}

			// Calculate snap distance of bounding rectangle
			currDistance = FindNearestSnapPoint(diagramPresenter, shapeBounds.Left, shapeBounds.Top, out dx, out dy);
			if (currDistance < lowestDistance && currDistance >= 0 && currDistance <= snapDistance) {
				lowestDistance = currDistance;
				snapDeltaX = dx;
				snapDeltaY = dy;
			}
			currDistance = FindNearestSnapPoint(diagramPresenter, shapeBounds.Right, shapeBounds.Top, out dx, out dy);
			if (currDistance < lowestDistance && currDistance >= 0 && currDistance <= snapDistance) {
				lowestDistance = currDistance;
				snapDeltaX = dx;
				snapDeltaY = dy;
			}
			currDistance = FindNearestSnapPoint(diagramPresenter, shapeBounds.Left, shapeBounds.Bottom, out dx, out dy);
			if (currDistance < lowestDistance && currDistance >= 0 && currDistance <= snapDistance) {
				lowestDistance = currDistance;
				snapDeltaX = dx;
				snapDeltaY = dy;
			}
			currDistance = FindNearestSnapPoint(diagramPresenter, shapeBounds.Right, shapeBounds.Bottom, out dx, out dy);
			if (currDistance < lowestDistance && currDistance >= 0 && currDistance <= snapDistance) {
				lowestDistance = currDistance;
				snapDeltaX = dx;
				snapDeltaY = dy;
			}
			return lowestDistance;
		}


		/// <summary>
		/// Finds the nearest SnapPoint in range of the given shape.
		/// </summary>
		/// <param name="diagramPresenter">The <see cref="T:Dataweb.NShape.Controllers.IDiagramPresenter" /></param>
		/// <param name="shape">The shape for which the nearest snap point is searched.</param>
		/// <param name="pointOffsetX">Declares the distance, the shape is moved on X axis before finding snap point.</param>
		/// <param name="pointOffsetY">Declares the distance, the shape is moved on X axis before finding snap point.</param>
		/// <param name="snapDeltaX">Horizontal distance between ptX and the nearest snap point.</param>
		/// <param name="snapDeltaY">Vertical distance between ptY and the nearest snap point.</param>
		/// <param name="controlPointCapability">Filter for control points taken into 
		/// account while calculating the snap distance.</param>
		/// <returns>Control point of the shape, the calculated distance refers to.</returns>
		protected ControlPointId FindNearestSnapPoint(IDiagramPresenter diagramPresenter, Shape shape, int pointOffsetX, int pointOffsetY,
			out int snapDeltaX, out int snapDeltaY, ControlPointCapabilities controlPointCapability) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (shape == null) throw new ArgumentNullException("shape");

			snapDeltaX = snapDeltaY = 0;
			ControlPointId result = ControlPointId.None;
			int snapDistance = diagramPresenter.SnapDistance;
			float lowestDistance = float.MaxValue;
			foreach (ControlPointId ptId in shape.GetControlPointIds(controlPointCapability)) {
				int dx, dy;
				float currDistance = FindNearestSnapPoint(diagramPresenter, shape, ptId, pointOffsetX, pointOffsetY, out dx, out dy);
				if (currDistance < lowestDistance && currDistance >= 0 && currDistance <= snapDistance) {
					lowestDistance = currDistance;
					result = ptId;
					snapDeltaX = dx;
					snapDeltaY = dy;
				}
			}
			return result;
		}


		/// <summary>
		/// Finds the nearest ControlPoint in range of the given shape's ControlPoint. 
		/// If there is no ControlPoint in range, the snap distance to the nearest grid line will be calculated.
		/// </summary>
		/// <param name="diagramPresenter">The <see cref="T:Dataweb.NShape.Controllers.IDiagramPresenter" /></param>
		/// <param name="shape">The given shape.</param>
		/// <param name="controlPointId">the given shape's ControlPoint</param>
		/// <param name="targetPointCapabilities">Capabilities of the control point to find.</param>
		/// <param name="pointOffsetX">Declares the distance, the shape is moved on X axis before finding snap point.</param>
		/// <param name="pointOffsetY">Declares the distance, the shape is moved on X axis before finding snap point.</param>
		/// <param name="snapDeltaX">Horizontal distance between ptX and the nearest snap point.</param>
		/// <param name="snapDeltaY">Vertical distance between ptY and the nearest snap point.</param>
		/// <param name="resultPointId">The Id of the returned shape's nearest ControlPoint.</param>
		/// <returns>The shape owning the found <see cref="T:Dataweb.NShape.Advanced.ControlPointId" />.</returns>
		protected Shape FindNearestControlPoint(IDiagramPresenter diagramPresenter, Shape shape, ControlPointId controlPointId,
			ControlPointCapabilities targetPointCapabilities, int pointOffsetX, int pointOffsetY,
			out int snapDeltaX, out int snapDeltaY, out ControlPointId resultPointId) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (shape == null) throw new ArgumentNullException("shape");

			Shape result = null;
			snapDeltaX = snapDeltaY = 0;
			resultPointId = ControlPointId.None;

			if (diagramPresenter.Diagram != null) {
				// calculate new position of the ControlPoint
				Point ctrlPtPos = shape.GetControlPointPosition(controlPointId);
				ctrlPtPos.Offset(pointOffsetX, pointOffsetY);

				int snapDistance = diagramPresenter.SnapDistance;
				int resultZOrder = int.MinValue;
				IEnumerable<Shape> foundShapes = FindVisibleShapes(diagramPresenter, ctrlPtPos.X, ctrlPtPos.Y, ControlPointCapabilities.Connect, snapDistance);
				foreach (Shape foundShape in foundShapes) {
					if (foundShape == shape) continue;
					//
					// Find the nearest control point
					float distance, lowestDistance = float.MaxValue;
					ControlPointId foundPtId = foundShape.FindNearestControlPoint(
							ctrlPtPos.X, ctrlPtPos.Y, snapDistance, targetPointCapabilities);
					//
					// Skip shapes without matching control points or below the last matching shape
					if (foundPtId == ControlPointId.None) continue;
					if (foundShape.ZOrder < resultZOrder) continue;
					//
					// If a valid control point was found, check whether it matches the criteria
					if (foundPtId != ControlPointId.Reference) {
						// If the shape itself is hit, do not calculate the snap distance because snapping 
						// to "real" control point has a higher priority.
						// Set TargetPointId and result shape in order to skip snapping to gridlines
						Point targetPtPos = foundShape.GetControlPointPosition(foundPtId);
						distance = Geometry.DistancePointPoint(ctrlPtPos.X, ctrlPtPos.Y, targetPtPos.X, targetPtPos.Y);
						if (distance <= snapDistance && distance < lowestDistance) {
							lowestDistance = distance;
							snapDeltaX = targetPtPos.X - ctrlPtPos.X;
							snapDeltaY = targetPtPos.Y - ctrlPtPos.Y;
						} else continue;
					}
					resultZOrder = foundShape.ZOrder;
					resultPointId = foundPtId;
					result = foundShape;
				}
				// calcualte distance to nearest grid point if there is no suitable control point in range
				if (resultPointId == ControlPointId.None)
					FindNearestSnapPoint(diagramPresenter, ctrlPtPos.X, ctrlPtPos.Y, out snapDeltaX, out snapDeltaY);
				else if (result != null && resultPointId == ControlPointId.Reference) {
					// ToDo: Calculate snap distance if the current mouse position outside the shape's outline
					if (result.ContainsPoint(ctrlPtPos.X, ctrlPtPos.Y)) {

					}
				}
			}
			return result;
		}


		///// <summary>
		///// Find the topmost shape that is not selected and has a valid ConnectionPoint (or ReferencePoint) in range of the given point.
		///// </summary>
		//protected ShapeAtCursorInfo FindConnectionTargetFromPosition(IDiagramPresenter diagramPresenter, int x, int y, bool onlyUnselected) {
		//   return FindConnectionTargetFromPosition(diagramPresenter, x, y, onlyUnselected, true);
		//}


		/// <summary>
		/// Find the topmost shape that is not selected and has a valid ConnectionPoint (or ReferencePoint) in range of the given point.
		/// </summary>
		protected ShapeAtCursorInfo FindConnectionTargetFromPosition(IDiagramPresenter diagramPresenter, int x, int y, bool onlyUnselected, bool snapToConnectionPoints) {
			return DoFindConnectionTarget(diagramPresenter, null, ControlPointId.None, x, y, onlyUnselected, snapToConnectionPoints ? diagramPresenter.ZoomedGripSize : 0);
		}


		/// <summary>
		/// Find the topmost shape that is not selected and has a valid ConnectionPoint (or ReferencePoint) in range of the given point.
		/// </summary>
		protected ShapeAtCursorInfo FindConnectionTarget(IDiagramPresenter diagramPresenter, Shape shape, ControlPointId gluePointId, Point newGluePointPos, bool onlyUnselected, bool snapToConnectionPoints) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (shape == null) throw new ArgumentNullException("shape");
			// Find (non-selected shape) its connection point under cursor
			ShapeAtCursorInfo result = ShapeAtCursorInfo.Empty;
			if (diagramPresenter.Diagram != null)
				result = DoFindConnectionTarget(diagramPresenter, shape, gluePointId, newGluePointPos.X, newGluePointPos.Y, onlyUnselected, snapToConnectionPoints ? diagramPresenter.ZoomedGripSize : 0);
			return result;
		}


		/// <summary>
		/// Find the topmost shape that is at the given point or has a control point with the given capabilities in range of the given point. 
		/// If parameter onlyUnselected is true, only shapes that are not selected will be returned.
		/// </summary>
		protected ShapeAtCursorInfo FindShapeAtCursor(IDiagramPresenter diagramPresenter, int x, int y, ControlPointCapabilities capabilities, int range, bool onlyUnselected) {
			// Find non-selected shape its connection point under cursor
			ShapeAtCursorInfo result = ShapeAtCursorInfo.Empty;
			int zOrder = int.MinValue;
			foreach (Shape shape in FindVisibleShapes(diagramPresenter, x, y, capabilities, range)) {
				// Skip selected shapes (if not wanted)
				if (onlyUnselected && diagramPresenter.SelectedShapes.Contains(shape)) continue;

				// No need to handle Parent shapes here as Children of CompositeShapes cannot be 
				// selected and grouped shapes keep their ZOrder

				// Skip shapes below the last matching shape
				if (shape.ZOrder < zOrder) continue;
				zOrder = shape.ZOrder;
				result.Shape = shape;
				result.ControlPointId = shape.HitTest(x, y, capabilities, range);
				if (result.Shape is ICaptionedShape)
					result.CaptionIndex = ((ICaptionedShape)shape).FindCaptionFromPoint(x, y);
			}
			return result;
		}


		/// <summary>
		/// Finds all shapes that meet the given requirements and are *not* invisible due to layer restrictions
		/// </summary>
		protected IEnumerable<Shape> FindVisibleShapes(IDiagramPresenter diagramPresenter, int x, int y, ControlPointCapabilities pointCapabilities, int distance) {
			if (diagramPresenter.Diagram == null) yield break;
			foreach (Shape s in diagramPresenter.Diagram.Shapes.FindShapes(x, y, pointCapabilities, distance)) {
				if (s.Layers == LayerIds.None || diagramPresenter.IsLayerVisible(s.Layers)) 
					yield return s;
			}
		}


		/// <summary>
		/// Find the topmost shape that is not selected and has a valid ConnectionPoint (or ReferencePoint) 
		/// in range of the given point.
		/// </summary>
		private ShapeAtCursorInfo DoFindConnectionTarget(IDiagramPresenter diagramPresenter, Shape shape, ControlPointId gluePointId, int x, int y, bool onlyUnselected, int range) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (range < 0) throw new ArgumentOutOfRangeException("range");
			// Find (non-selected shape) its connection point under cursor
			ShapeAtCursorInfo result = ShapeAtCursorInfo.Empty;
			int resultZOrder = int.MinValue;
			if (diagramPresenter.Diagram != null) {
				foreach (Shape s in FindVisibleShapes(diagramPresenter, x, y, ControlPointCapabilities.Connect, range)) {
					if (s == shape) continue;
					if (s.Layers != LayerIds.None && !diagramPresenter.IsLayerVisible(s.Layers)) continue;
					// Skip shapes below the last matching shape
					if (s.ZOrder < resultZOrder) continue;
					// If the shape is already connected to the found shape via point-to-shape connection
					if (shape != null) {
						if(!CanConnectTo(shape, gluePointId, s)) continue;
						if (!CanConnectTo(diagramPresenter, shape,
							(gluePointId == ControlPointId.FirstVertex) ? ControlPointId.LastVertex : ControlPointId.FirstVertex,
							gluePointId, onlyUnselected)) continue;
					}
					// Skip selected shapes (if not wanted)
					if (onlyUnselected && diagramPresenter.SelectedShapes.Contains(s)) continue;
					// Perform a HitTest on the shape
					ControlPointId pointId = s.HitTest(x, y, ControlPointCapabilities.Connect, range);
					if (pointId != ControlPointId.None) {
						if (s.HasControlPointCapability(pointId, ControlPointCapabilities.Glue)) continue;
						result.Shape = s;
						result.ControlPointId = pointId;
						resultZOrder = s.ZOrder;
						// If the found connection point is a dedicated connection point, take it. If the found connection point is 
						// the reference point of a shape, continue searching for a dedicated connection point.
						if (result.ControlPointId != ControlPointId.Reference)
							break;
					}
				}
			}
			return result;
		}

		#endregion


		#region [Protected] Types

		/// <summary>
		/// The current state of the mouse.
		/// </summary>
		protected struct MouseState : IEquatable<MouseState> {

			/// <ToBeCompleted></ToBeCompleted>
			public static bool operator ==(MouseState a, MouseState b) {
				return (a.Position == b.Position
					&& a.Modifiers == b.Modifiers
					&& a.Buttons == b.Buttons);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public static bool operator !=(MouseState a, MouseState b) {
				return !(a == b);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public static MouseState Empty;

			/// <override></override>
			public override int GetHashCode() {
				return Position.GetHashCode() ^ Buttons.GetHashCode() ^ Modifiers.GetHashCode();
			}

			/// <override></override>
			public override bool Equals(object obj) {
				return (obj is MouseState && object.ReferenceEquals(this, obj));
			}

			/// <override></override>
			public bool Equals(MouseState other) {
				return other == this;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public int X {
				get { return Position.X; }
			}

			/// <ToBeCompleted></ToBeCompleted>
			public int Y {
				get { return Position.Y; }
			}

			/// <ToBeCompleted></ToBeCompleted>
			public Point Position;

			/// <ToBeCompleted></ToBeCompleted>
			public KeysDg Modifiers;

			/// <ToBeCompleted></ToBeCompleted>
			public MouseButtonsDg Buttons;

			/// <ToBeCompleted></ToBeCompleted>
			public bool IsButtonDown(MouseButtonsDg button) {
				return (Buttons & button) != 0;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool IsKeyPressed(KeysDg modifier) {
				return (Modifiers & modifier) != 0;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool IsEmpty {
				get { return this == Empty; }
			}

			static MouseState() {
				Empty.Position = Geometry.InvalidPoint;
				Empty.Modifiers = KeysDg.None;
				Empty.Buttons = 0;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected struct ShapeAtCursorInfo : IEquatable<ShapeAtCursorInfo> {

			/// <ToBeCompleted></ToBeCompleted>
			public static bool operator ==(ShapeAtCursorInfo a, ShapeAtCursorInfo b) {
				return (a.Shape == b.Shape
					&& a.ControlPointId == b.ControlPointId
					&& a.CaptionIndex == b.CaptionIndex);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public static bool operator !=(ShapeAtCursorInfo a, ShapeAtCursorInfo b) {
				return !(a == b);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public static ShapeAtCursorInfo Empty;

			/// <override></override>
			public override int GetHashCode() {
				return Shape.GetHashCode() ^ ControlPointId.GetHashCode() ^ CaptionIndex.GetHashCode();
			}

			/// <override></override>
			public override bool Equals(object obj) {
				return (obj is ShapeAtCursorInfo && object.ReferenceEquals(this, obj));
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool Equals(ShapeAtCursorInfo other) {
				return other == this;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public void Clear() {
				this.CaptionIndex = Empty.CaptionIndex;
				this.ControlPointId = Empty.ControlPointId;
				this.Shape = Empty.Shape;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public Shape Shape;

			/// <ToBeCompleted></ToBeCompleted>
			public ControlPointId ControlPointId;

			/// <ToBeCompleted></ToBeCompleted>
			public int CaptionIndex;

			/// <ToBeCompleted></ToBeCompleted>
			public bool IsCursorAtGrip {
				get {
					return (Shape != null
					&& ControlPointId != ControlPointId.None
					&& ControlPointId != ControlPointId.Reference);
				}
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool IsCursorAtGluePoint {
				get {
					return (Shape != null
						&& Shape.HasControlPointCapability(ControlPointId, ControlPointCapabilities.Glue));
				}
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool IsCursorAtConnectionPoint {
				get {
					return (Shape != null
						&& Shape.HasControlPointCapability(ControlPointId, ControlPointCapabilities.Connect));
				}
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool IsCursorAtCaption {
				get { return (Shape is ICaptionedShape && CaptionIndex >= 0 && !IsCursorAtGrip); }
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool IsEmpty {
				get { return Shape == null; }
			}

			static ShapeAtCursorInfo() {
				Empty.Shape = null;
				Empty.ControlPointId = ControlPointId.None;
				Empty.CaptionIndex = -1;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected struct ActionDef {

			/// <ToBeCompleted></ToBeCompleted>
			public static readonly ActionDef Empty;

			/// <ToBeCompleted></ToBeCompleted>
			public static ActionDef Create(IDiagramPresenter diagramPresenter, int action, MouseState mouseState, bool wantsAutoScroll) {
				ActionDef result = ActionDef.Empty;
				result.diagramPresenter = diagramPresenter;
				result.action = action;
				result.mouseState = mouseState;
				result.wantsAutoScroll = wantsAutoScroll;
				return result;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public ActionDef(IDiagramPresenter diagramPresenter, int action, MouseState mouseState, bool wantsAutoScroll) {
				this.diagramPresenter = diagramPresenter;
				this.action = action;
				this.mouseState = mouseState;
				this.wantsAutoScroll = wantsAutoScroll;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public IDiagramPresenter DiagramPresenter {
				get { return diagramPresenter; }
			}

			/// <ToBeCompleted></ToBeCompleted>
			public MouseState MouseState {
				get { return mouseState; }
			}

			/// <ToBeCompleted></ToBeCompleted>
			public int Action {
				get { return action; }
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool WantsAutoScroll {
				get { return wantsAutoScroll; }
			}

			static ActionDef() {
				Empty.diagramPresenter = null;
				Empty.action = int.MinValue;
				Empty.mouseState = MouseState.Empty;
				Empty.wantsAutoScroll = false;
			}

			private IDiagramPresenter diagramPresenter;
			private MouseState mouseState;
			private int action;
			private bool wantsAutoScroll;
		}

		#endregion


		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected int margin = 1;
		/// <ToBeCompleted></ToBeCompleted>
		protected Color transparentColor = Color.LightGray;

		// --- Description of the tool ---
		// Unique name of the tool.
		private string name;
		// Title that will be displayed in the tool box
		private string title;
		// Category title of the tool, used for grouping tools in the tool box
		private string category;
		// Hint that will be displayed when the mouse is hovering the tool
		private string description;
		// small icon of the tool
		private Bitmap smallIcon;
		// the large icon of the tool
		private Bitmap largeIcon;
		// minimum distance the mouse has to move before a drag action is considered as drag action
		private Size minDragDistance = new Size(4, 4);
		//
		// margin and background colors of the toolbox icons "LargeIcon" and "SmallIcon"
		// highlighting connection targets in range
		private int pointHighlightRange = 50;
		//
		// --- Mouse state after last mouse event ---
		// State of the mouse after the last ProcessMouseEvents call
		private MouseState currentMouseState = MouseState.Empty;
		// Shapes whose connection points will be highlighted in the next drawing
		private List<Shape> shapesInRange = new List<Shape>();

		// --- Definition of current action(s) ---
		// The stack contains 
		// - the display that is edited with this tool,
		// - transformed coordinates of the mouse position when an action has started (diagram coordinates)
		private Stack<ActionDef> pendingActions = new Stack<ActionDef>(1);
		// 
		// Work buffer for shapes
		private List<Shape> shapeBuffer = new List<Shape>();

		#endregion
	}


	/// <summary>
	/// Lets the user size, move, rotate and select shapes.
	/// </summary>
	[Obsolete("Use SelectionTool instead")]
	public class PointerTool : SelectionTool {
		
		/// <ToBeCompleted></ToBeCompleted>
		public PointerTool() : base() { }

		/// <ToBeCompleted></ToBeCompleted>
		public PointerTool(string category) : base(category) { }

	}


	/// <summary>
	/// Lets the user size, move, rotate and select shapes.
	/// </summary>
	public class SelectionTool : Tool {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.PointerTool" />.
		/// </summary>
		public SelectionTool()
			: base("Standard") {
			Construct();
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.PointerTool" />.
		/// </summary>
		public SelectionTool(string category)
			: base(category) {
			Construct();
		}


		#region [Public] Tool Members

		/// <override></override>
		public override void RefreshIcons() {
			// nothing to do...
		}


		/// <override></override>
		public override bool ProcessMouseEvent(IDiagramPresenter diagramPresenter, MouseEventArgsDg e) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			bool result = false;
			// get new mouse state
			MouseState newMouseState = MouseState.Empty;
			newMouseState.Buttons = e.Buttons;
			newMouseState.Modifiers = e.Modifiers;
			diagramPresenter.ControlToDiagram(e.Position, out newMouseState.Position);

			diagramPresenter.SuspendUpdate();
			try {
				// Only process mouse action if the position of the mouse or a mouse button state changed
				//if (e.EventType != MouseEventType.MouseMove || newMouseState.Position != CurrentMouseState.Position) {
				// Process the mouse event
				switch (e.EventType) {
					case MouseEventType.MouseDown:
						// Start drag action such as drawing a SelectionFrame or moving selectedShapes/shape handles
						result = ProcessMouseDown(diagramPresenter, newMouseState);
						break;

					case MouseEventType.MouseMove:
						// Set cursors depending on HotSpots or draw moving/resizing preview
						result = ProcessMouseMove(diagramPresenter, newMouseState);
						break;

					case MouseEventType.MouseUp:
						// perform selection/moving/resizing
						result = ProcessMouseUp(diagramPresenter, newMouseState);
						if (!result && e.Clicks > 1)
							// perform QuickRotate (90°) if the feature is enabled
							result = ProcessDoubleClick(diagramPresenter, newMouseState, e.Clicks);
						break;

					default: throw new NShapeUnsupportedValueException(e.EventType);
				}
				//}
			} finally { diagramPresenter.ResumeUpdate(); }
			base.ProcessMouseEvent(diagramPresenter, e);
			return result;
		}


		/// <override></override>
		public override bool ProcessKeyEvent(IDiagramPresenter diagramPresenter, KeyEventArgsDg e) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			// if the keyPress was not handled by the base class, try to handle it here
			bool result = false;
			switch (e.EventType) {
				case KeyEventType.PreviewKeyDown:
				case KeyEventType.KeyPress:
					// do nothing
					break;
				case KeyEventType.KeyDown:
				case KeyEventType.KeyUp:
					if ((KeysDg)e.KeyCode == KeysDg.Delete) {
						// Update selected shape unter the mouse cursor because it was propably deleted
						if (!selectedShapeAtCursorInfo.IsEmpty && !diagramPresenter.SelectedShapes.Contains(selectedShapeAtCursorInfo.Shape)) {
							SetSelectedShapeAtCursor(diagramPresenter, CurrentMouseState.X, CurrentMouseState.Y, diagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);
							Invalidate(diagramPresenter);
						}
					}

					// Update Cursor when modifier keys are pressed or released
					if (((KeysDg)e.KeyCode & KeysDg.Shift) == KeysDg.Shift
						|| ((KeysDg)e.KeyCode & KeysDg.ShiftKey) == KeysDg.ShiftKey
						|| ((KeysDg)e.KeyCode & KeysDg.Control) == KeysDg.Control
						|| ((KeysDg)e.KeyCode & KeysDg.ControlKey) == KeysDg.ControlKey
						|| ((KeysDg)e.KeyCode & KeysDg.Alt) == KeysDg.Alt) {
						MouseState mouseState = CurrentMouseState;
						mouseState.Modifiers = (KeysDg)e.Modifiers;
						int cursorId = DetermineCursor(diagramPresenter, mouseState);
						diagramPresenter.SetCursor(cursorId);
					}
					break;
				default: throw new NShapeUnsupportedValueException(e.EventType);
			}
			if (base.ProcessKeyEvent(diagramPresenter, e)) result = true;
			return result;
		}


		/// <override></override>
		public override void EnterDisplay(IDiagramPresenter diagramPresenter) {
			// nothing to do
		}


		/// <override></override>
		public override void LeaveDisplay(IDiagramPresenter diagramPresenter) {
			// nothing to do
		}


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			int mouseX = CurrentMouseState.X;
			int mouseY = CurrentMouseState.Y;

			// Update the selected shape at cursor
			SetSelectedShapeAtCursor(diagramPresenter, mouseX, mouseY, diagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);

			// Return the shape's actions
			if (diagramPresenter.SelectedShapes.Count == 1 && !selectedShapeAtCursorInfo.IsEmpty) {
				bool separatorRequired = false;
				// ToDo: Create an aggregated command that creates a composite shape first and then a template from it
				if (selectedShapeAtCursorInfo.Shape.Template != null) {
					// Deliver Template's actions
					foreach (MenuItemDef action in selectedShapeAtCursorInfo.Shape.Template.GetMenuItemDefs()) {
						if (!separatorRequired) separatorRequired = true;
						yield return action;
					}
				}
				foreach (MenuItemDef action in selectedShapeAtCursorInfo.Shape.GetMenuItemDefs(mouseX, mouseY, diagramPresenter.ZoomedGripSize)) {
					if (separatorRequired) yield return new SeparatorMenuItemDef();
					yield return action;
				}
				if (selectedShapeAtCursorInfo.Shape.ModelObject != null) {
					if (separatorRequired) yield return new SeparatorMenuItemDef();
					foreach (MenuItemDef action in selectedShapeAtCursorInfo.Shape.ModelObject.GetMenuItemDefs())
						yield return action;
				}
			} else {
				// ToDo: Find shape under the cursor and return its actions?
				// ToDo: Collect all actions provided by the diagram if no shape was right-clicked
			}
			// ToDo: Add tool-specific actions?
		}


		/// <override></override>
		public override void Draw(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			switch (CurrentAction) {
				case Action.Select:
					// nothing to do
					break;

				case Action.None:
				case Action.EditCaption:
					// MouseOver-Highlighting of the caption under the cursor 
					// At the moment, the ownerDisplay draws the caption bounds along with the selection highlighting
					IDiagramPresenter presenter = (CurrentAction == Action.None) ? diagramPresenter : ActionDiagramPresenter;
					if (IsEditCaptionFeasible(presenter, CurrentMouseState, selectedShapeAtCursorInfo)) {
						diagramPresenter.ResetTransformation();
						try {
							diagramPresenter.DrawCaptionBounds(IndicatorDrawMode.Highlighted, (ICaptionedShape)selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.CaptionIndex);
						} finally { diagramPresenter.RestoreTransformation(); }
					}
					break;

				case Action.SelectWithFrame:
					diagramPresenter.ResetTransformation();
					try {
						diagramPresenter.DrawSelectionFrame(frameRect);
					} finally { diagramPresenter.RestoreTransformation(); }
					break;

				case Action.MoveShape:
				case Action.MoveHandle:
					// Draw shape previews first
					diagramPresenter.DrawShapes(Previews.Values);

					// Then draw snap-lines and -points
					if (selectedShapeAtCursorInfo != null && (snapPtId > 0 || snapDeltaX != 0 || snapDeltaY != 0)) {
						Shape previewAtCursor = FindPreviewOfShape(selectedShapeAtCursorInfo.Shape);
						diagramPresenter.DrawSnapIndicators(previewAtCursor);
					}
					// Finally, draw highlighted connection points and/or highlighted shape outlines
					if (diagramPresenter.SelectedShapes.Count == 1 && selectedShapeAtCursorInfo.ControlPointId != ControlPointId.None) {
						Shape preview = FindPreviewOfShape(diagramPresenter.SelectedShapes.TopMost);
						if (preview.HasControlPointCapability(selectedShapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Glue)) {
							// Find and highlight valid connection targets in range
							Point p = preview.GetControlPointPosition(selectedShapeAtCursorInfo.ControlPointId);
							DrawConnectionTargets(ActionDiagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, p, ActionDiagramPresenter.SelectedShapes);
						}
					}
					break;

				case Action.PrepareRotate:
				case Action.Rotate:
					if (CurrentAction == Action.Rotate)
						diagramPresenter.DrawShapes(Previews.Values);
					diagramPresenter.ResetTransformation();
					try {
						if (PendingToolActionsCount == 1) {
							diagramPresenter.DrawAnglePreview(rectBuffer.Location, CurrentMouseState.Position, cursors[ToolCursor.Rotate], 0, 0);
						} else {
							// Get MouseState of the first click (on the rotation point)
							MouseState initMouseState = GetPreviousMouseState();
							int startAngle, sweepAngle;
							CalcAngle(initMouseState, ActionStartMouseState, CurrentMouseState, out startAngle, out sweepAngle);

							// ToDo: Determine standard cursor size
							rectBuffer.Location = selectedShapeAtCursorInfo.Shape.GetControlPointPosition(selectedShapeAtCursorInfo.ControlPointId);
							rectBuffer.Width = rectBuffer.Height = (int)Math.Ceiling(Geometry.DistancePointPoint(rectBuffer.Location, CurrentMouseState.Position));

							diagramPresenter.DrawAnglePreview(rectBuffer.Location, CurrentMouseState.Position,
								cursors[ToolCursor.Rotate], startAngle, sweepAngle);
						}
					} finally { diagramPresenter.RestoreTransformation(); }
					break;

				default: throw new NShapeUnsupportedValueException(CurrentAction);
			}
		}


		/// <override></override>
		public override void Invalidate(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			switch (CurrentAction) {
				case Action.None:
				case Action.Select:
				case Action.EditCaption:
					if (!selectedShapeAtCursorInfo.IsEmpty) {
						selectedShapeAtCursorInfo.Shape.Invalidate();
						diagramPresenter.InvalidateGrips(selectedShapeAtCursorInfo.Shape, ControlPointCapabilities.All);
					}
					break;

				case Action.SelectWithFrame:
					diagramPresenter.DisplayService.Invalidate(frameRect);
					break;

				case Action.MoveHandle:
				case Action.MoveShape:
#if DEBUG_DIAGNOSTICS
					Assert(!SelectedShapeAtCursorInfo.IsEmpty);
#endif
					if (Previews.Count > 0) {
						InvalidateShapes(diagramPresenter, Previews.Values, false);
						if (diagramPresenter.SnapToGrid) {
							Shape previewAtCursor = FindPreviewOfShape(selectedShapeAtCursorInfo.Shape);
							diagramPresenter.InvalidateSnapIndicators(previewAtCursor);
						}
						if (CurrentAction == Action.MoveHandle && selectedShapeAtCursorInfo.IsCursorAtGluePoint)
							InvalidateConnectionTargets(diagramPresenter, CurrentMouseState.X, CurrentMouseState.Y);
					}
					break;

				case Action.PrepareRotate:
				case Action.Rotate:
					if (Previews.Count > 0)
						InvalidateShapes(diagramPresenter, Previews.Values, false);
					InvalidateAnglePreview(diagramPresenter);
					break;

				default: throw new NShapeUnsupportedValueException(typeof(MenuItemDef), CurrentAction);
			}
		}


		/// <override></override>
		protected override void CancelCore() {
			frameRect = Rectangle.Empty;
			rectBuffer = Rectangle.Empty;

			//currentToolAction = ToolAction.None;
			selectedShapeAtCursorInfo.Clear();
		}

		#endregion


		#region [Protected] Tool Members

		/// <override></override>
		protected override void StartToolAction(IDiagramPresenter diagramPresenter, int action, MouseState mouseState, bool wantAutoScroll) {
			base.StartToolAction(diagramPresenter, action, mouseState, wantAutoScroll);
			// Empty selection frame
			frameRect.Location = mouseState.Position;
			frameRect.Size = Size.Empty;
		}


		/// <override></override>
		protected override void EndToolAction() {
			base.EndToolAction();
			//currentToolAction = ToolAction.None;
			if (!IsToolActionPending)
				ClearPreviews();
		}

		#endregion


		#region [Private] Properties

		private Action CurrentAction {
			get {
				if (IsToolActionPending)
					return (Action)CurrentToolAction.Action;
				else return Action.None;
			}
		}


		//private ShapeAtCursorInfo SelectedShapeAtCursorInfo {
		//    get { return selectedShapeAtCursorInfo; }
		//}

		#endregion


		#region [Private] MouseEvent processing implementation

		private bool ProcessMouseDown(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			bool result = false;

			// Check if the selected shape at cursor is still valid
			if (!selectedShapeAtCursorInfo.IsEmpty
				&& (!diagramPresenter.SelectedShapes.Contains(selectedShapeAtCursorInfo.Shape)
					|| selectedShapeAtCursorInfo.Shape.HitTest(mouseState.X, mouseState.Y, ControlPointCapabilities.All, diagramPresenter.ZoomedGripSize) == ControlPointId.None)) {
				selectedShapeAtCursorInfo.Clear();
			}

			// If no action is pending, try to start a new one...
			if (CurrentAction == Action.None) {
				// Get suitable action (depending on the currently selected shape under the mouse cursor)
				Action newAction = DetermineMouseDownAction(diagramPresenter, mouseState);
				if (newAction != Action.None) {
					//currentToolAction = newAction;
					bool wantAutoScroll;
					switch (newAction) {
						case Action.SelectWithFrame:
						case Action.MoveHandle:
						case Action.MoveShape:
							wantAutoScroll = true;
							result = true;
							break;
						default: wantAutoScroll = false; break;
					}
					StartToolAction(diagramPresenter, (int)newAction, mouseState, wantAutoScroll);

					// If the action requires preview shapes, create them now...
					switch (CurrentAction) {
						case Action.None:
						case Action.Select:
						case Action.SelectWithFrame:
						case Action.EditCaption:
							break;
						case Action.MoveHandle:
						case Action.MoveShape:
						case Action.PrepareRotate:
						case Action.Rotate:
							CreatePreviewShapes(diagramPresenter);
							result = true;
							break;
						default: throw new NShapeUnsupportedValueException(CurrentAction);
					}
					
					Invalidate(ActionDiagramPresenter);
				}
			} else {
				// ... otherwise cancel the action (if right mouse button was pressed)
				Action newAction = DetermineMouseDownAction(diagramPresenter, mouseState);
				if (newAction == Action.None) {
					Cancel();
					result = true;
				}
			}
			return result;
		}


		private bool ProcessMouseMove(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			bool result = true;

			if (!selectedShapeAtCursorInfo.IsEmpty &&
				!diagramPresenter.SelectedShapes.Contains(selectedShapeAtCursorInfo.Shape))
				selectedShapeAtCursorInfo.Clear();

			Action newAction;
			switch (CurrentAction) {
				case Action.None:
					result = false;
					SetSelectedShapeAtCursor(diagramPresenter, mouseState.X, mouseState.Y, diagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);
					Invalidate(diagramPresenter);
					break;

				case Action.Select:
				case Action.EditCaption:
					// Find unselected shape under the mouse cursor
					ShapeAtCursorInfo shapeAtCursorInfo = ShapeAtCursorInfo.Empty;
					newAction = CurrentAction;
					if (mouseState.IsButtonDown(MouseButtonsDg.Left)) {
						if (IsDragActionFeasible(mouseState, MouseButtonsDg.Left)) {
							// Determine new drag action
							shapeAtCursorInfo = FindShapeAtCursor(ActionDiagramPresenter, ActionStartMouseState.X, ActionStartMouseState.Y, ControlPointCapabilities.None, 0, true);
							newAction = DetermineMouseMoveAction(ActionDiagramPresenter, ActionStartMouseState, shapeAtCursorInfo);
						}
					} else newAction = DetermineMouseMoveAction(ActionDiagramPresenter, ActionStartMouseState, shapeAtCursorInfo);

					// If the action has changed, prepare and start the new action
					if (newAction != CurrentAction) {
						switch (newAction) {
							// Select --> SelectWithFrame
							case Action.SelectWithFrame:
#if DEBUG_DIAGNOSTICS
								Assert(CurrentAction == Action.Select);
#endif
								StartToolAction(diagramPresenter, (int)newAction, ActionStartMouseState, true);
								PrepareSelectionFrame(ActionDiagramPresenter, ActionStartMouseState);
								break;

							// Select --> (Select shape and) move shape
							case Action.MoveShape:
							case Action.MoveHandle:
							case Action.PrepareRotate:
#if DEBUG_DIAGNOSTICS
								Assert(CurrentAction == Action.Select || CurrentAction == Action.EditCaption);
#endif
								if (selectedShapeAtCursorInfo.IsEmpty) {
									// Select shape at cursor before start dragging it
									PerformSelection(ActionDiagramPresenter, ActionStartMouseState, shapeAtCursorInfo);
									SetSelectedShapeAtCursor(diagramPresenter, ActionStartMouseState.X, ActionStartMouseState.Y, 0, ControlPointCapabilities.None);
#if DEBUG_DIAGNOSTICS
									Assert(!SelectedShapeAtCursorInfo.IsEmpty);
#endif
								}
								// Init moving shape
#if DEBUG_DIAGNOSTICS
								Assert(!SelectedShapeAtCursorInfo.IsEmpty);
#endif
								CreatePreviewShapes(ActionDiagramPresenter);
								StartToolAction(diagramPresenter, (int)newAction, ActionStartMouseState, true);
								PrepareMoveShapePreview(ActionDiagramPresenter, ActionStartMouseState);
								break;

							// Select --> (Select shape and) rotate shape / edit shape caption
							case Action.Rotate:
							case Action.EditCaption:
							case Action.None:
							case Action.Select:
								Debug.Fail("Unhandled change of CurrentAction.");
								break;
							default:
								Debug.Fail(string.Format("Unexpected {0} value: {1}", CurrentAction.GetType().Name, CurrentAction));
								break;
						}
						//currentToolAction = newAction;
					}
					Invalidate(ActionDiagramPresenter);
					break;

				case Action.SelectWithFrame:
					PrepareSelectionFrame(ActionDiagramPresenter, mouseState);
					break;

				case Action.MoveHandle:
#if DEBUG_DIAGNOSTICS
					Assert(IsMoveHandleFeasible(ActionDiagramPresenter, SelectedShapeAtCursorInfo));
#endif
					PrepareMoveHandlePreview(ActionDiagramPresenter, mouseState);
					break;

				case Action.MoveShape:
					PrepareMoveShapePreview(diagramPresenter, mouseState);
					break;

				case Action.PrepareRotate:
				case Action.Rotate:
#if DEBUG_DIAGNOSTICS
					Assert(IsRotatatingFeasible(ActionDiagramPresenter, SelectedShapeAtCursorInfo));
#endif
					newAction = CurrentAction;
					// Find unselected shape under the mouse cursor
					newAction = DetermineMouseMoveAction(ActionDiagramPresenter, mouseState, selectedShapeAtCursorInfo);

					// If the action has changed, prepare and start the new action
					if (newAction != CurrentAction) {
						switch (newAction) {
							// Rotate shape -> Prepare shape rotation
							case Action.PrepareRotate:
#if DEBUG_DIAGNOSTICS
								Assert(CurrentAction == Action.Rotate);
#endif
								EndToolAction();
								ClearPreviews();
								break;

							// Prepare shape rotation -> Rotate shape
							case Action.Rotate:
#if DEBUG_DIAGNOSTICS
								Assert(CurrentAction == Action.PrepareRotate);
#endif
								StartToolAction(ActionDiagramPresenter, (int)newAction, mouseState, false);
								CreatePreviewShapes(ActionDiagramPresenter);
								break;

							case Action.SelectWithFrame:
							case Action.MoveShape:
							case Action.EditCaption:
							case Action.MoveHandle:
							case Action.None:
							case Action.Select:
								Debug.Fail("Unhandled change of CurrentAction.");
								break;
							default:
								Debug.Fail(string.Format("Unexpected {0} value: {1}", CurrentAction.GetType().Name, CurrentAction));
								break;
						}
						//currentToolAction = newAction;
					}

					PrepareRotatePreview(ActionDiagramPresenter, mouseState);
					break;

				default: throw new NShapeUnsupportedValueException(typeof(Action), CurrentAction);
			}

			int cursorId = DetermineCursor(diagramPresenter, mouseState);
			if (CurrentAction == Action.None) diagramPresenter.SetCursor(cursorId);
			else ActionDiagramPresenter.SetCursor(cursorId);

			return result;
		}


		private bool ProcessMouseUp(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			bool result = false;

			if (!selectedShapeAtCursorInfo.IsEmpty &&
			    !diagramPresenter.SelectedShapes.Contains(selectedShapeAtCursorInfo.Shape))
			    selectedShapeAtCursorInfo.Clear();
			
			switch (CurrentAction) {
				case Action.None:
					// do nothing
					break;

				case Action.Select:
					// Perform selection
					ShapeAtCursorInfo shapeAtCursorInfo = ShapeAtCursorInfo.Empty;
					// temp hack to always select the top-most shape: don't try and select the lower shapes
					/*
					if (!selectedShapeAtCursorInfo.IsEmpty) {
						if (selectedShapeAtCursorInfo.Shape.ContainsPoint(mouseState.X, mouseState.Y)) {
							Shape shape = ActionDiagramPresenter.Diagram.Shapes.FindShape(mouseState.X, mouseState.Y, ControlPointCapabilities.None, 0, selectedShapeAtCursorInfo.Shape);
							if (shape != null) {
								shapeAtCursorInfo.Shape = shape;
								shapeAtCursorInfo.ControlPointId = shape.HitTest(mouseState.X, mouseState.Y, ControlPointCapabilities.None, 0);
								shapeAtCursorInfo.CaptionIndex = -1;
							}
						}
					} else
					*/
					shapeAtCursorInfo = FindShapeAtCursor(diagramPresenter, mouseState.X, mouseState.Y, ControlPointCapabilities.None, 0, false);
					result = PerformSelection(ActionDiagramPresenter, mouseState, shapeAtCursorInfo);

					SetSelectedShapeAtCursor(ActionDiagramPresenter, mouseState.X, mouseState.Y, ActionDiagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);
					EndToolAction();
					break;

				case Action.SelectWithFrame:
					// select all selectedShapes within the frame
					result = PerformFrameSelection(ActionDiagramPresenter, mouseState);
					while (IsToolActionPending)
						EndToolAction();
					break;

				case Action.EditCaption:
					// if the user clicked a caption, display the caption editor
#if DEBUG_DIAGNOSTICS
					Assert(SelectedShapeAtCursorInfo.IsCursorAtCaption);
#endif
					// Update the selected shape at cursor
					SetSelectedShapeAtCursor(ActionDiagramPresenter, mouseState.X, mouseState.Y, ActionDiagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);
					if (!selectedShapeAtCursorInfo.IsEmpty && selectedShapeAtCursorInfo.IsCursorAtCaption)
						ActionDiagramPresenter.OpenCaptionEditor((ICaptionedShape)selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.CaptionIndex);
					EndToolAction();
					result = true;
					break;

				case Action.MoveHandle:
#if DEBUG_DIAGNOSTICS
					Assert(!SelectedShapeAtCursorInfo.IsEmpty);
#endif
					result = PerformMoveHandle(ActionDiagramPresenter);
					while (IsToolActionPending)
						EndToolAction();
					break;

				case Action.MoveShape:
#if DEBUG_DIAGNOSTICS
					Assert(!SelectedShapeAtCursorInfo.IsEmpty);
#endif
					result = PerformMoveShape(ActionDiagramPresenter);
					while (IsToolActionPending)
						EndToolAction();
					break;

				case Action.PrepareRotate:
				case Action.Rotate:
#if DEBUG_DIAGNOSTICS
					Assert(!SelectedShapeAtCursorInfo.IsEmpty);
#endif
					if (CurrentAction == Action.Rotate)
						result = PerformRotate(ActionDiagramPresenter);
					while (IsToolActionPending)
						EndToolAction();
					break;

				default: throw new NShapeUnsupportedValueException(CurrentAction);
			}

			SetSelectedShapeAtCursor(diagramPresenter, mouseState.X, mouseState.Y, diagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);
			diagramPresenter.SetCursor(DetermineCursor(diagramPresenter, mouseState));

			OnToolExecuted(ExecutedEventArgs);
			return result;
		}


		private bool ProcessDoubleClick(IDiagramPresenter diagramPresenter, MouseState mouseState, int clickCount) {
			bool result = false;
			if (diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, diagramPresenter.SelectedShapes) && enableQuickRotate) {
				if (!selectedShapeAtCursorInfo.IsEmpty && selectedShapeAtCursorInfo.IsCursorAtGrip
					&& selectedShapeAtCursorInfo.Shape.HasControlPointCapability(selectedShapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Rotate)) {
					int angle = 900 * (clickCount - 1);
					if (angle % 3600 != 0) {
						PerformQuickRotate(diagramPresenter, angle);
						result = true;
						OnToolExecuted(ExecutedEventArgs);
					}
				}
			}
			return result;
		}

		#endregion


		#region [Private] Determine action depending on mouse state and event type

		/// <summary>
		/// Decide which tool action is suitable for the current mouse state.
		/// </summary>
		private Action DetermineMouseDownAction(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			if (mouseState.IsButtonDown(MouseButtonsDg.Left)) {
				if (!selectedShapeAtCursorInfo.IsEmpty
					&& IsEditCaptionFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo)) {
					// If the cursor is not over a caption of a selected shape when clicking left mouse button, 
					// we assume the user wants to select something
					// Same thing if no other action is granted.
					return Action.EditCaption;
				} else {
					// Moving shapes and handles is initiated as soon as the user starts drag action (move mouse 
					// while mouse button is pressed) 
					// If the user does not start a drag action, this will result in (un)selecting shapes.
					return Action.Select;
				}
			} else if (mouseState.IsButtonDown(MouseButtonsDg.Right)) {
				// Abort current action when clicking right mouse button
				return Action.None;
			} else {
				// Ignore other pressed mouse buttons
				return CurrentAction;
			}
		}


		/// <summary>
		/// Decide which tool action is suitable for the current mouse state.
		/// </summary>
		private Action DetermineMouseMoveAction(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo) {
			switch (CurrentAction) {
				case Action.None:
				case Action.MoveHandle:
				case Action.MoveShape:
				case Action.SelectWithFrame:
					// Do not change the current action
					return CurrentAction;

				case Action.Select:
				case Action.EditCaption:
					if (mouseState.IsButtonDown(MouseButtonsDg.Left)) {
						// Check if cursor is over a control point and moving grips or rotating is feasible
						if (selectedShapeAtCursorInfo.IsCursorAtGrip) {
							if (IsMoveHandleFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo))
								return Action.MoveHandle;
							else if (IsRotatatingFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo))
								return Action.PrepareRotate;
							else return Action.SelectWithFrame;
						} else {
							// If there is no shape under the cursor, start a SelectWithFrame action,
							// otherwise start a MoveShape action
							bool canMove = false;
							if (!selectedShapeAtCursorInfo.IsEmpty) {
								// If there are selected shapes, check these shapes...
								canMove = IsMoveShapeFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo);
							} else {
								// ... otherwise check the shape under the cursor as it will be selected 
								// before starting a move action
								canMove = IsMoveShapeFeasible(diagramPresenter, mouseState, shapeAtCursorInfo);
							}
							if (canMove)
								return Action.MoveShape;
							else return Action.SelectWithFrame;
						}
					}
					return CurrentAction;

				case Action.PrepareRotate:
					if (mouseState.IsButtonDown(MouseButtonsDg.Left)) {
						// If the mouse has left the min rotate range, start 'real' rotating
						if (IsMinRotateRangeExceeded(diagramPresenter, mouseState))
							return Action.Rotate;
						else return CurrentAction;
					} else return CurrentAction;

				case Action.Rotate:
					if (mouseState.IsButtonDown(MouseButtonsDg.Left)) {
						// If the mouse has entered the min rotate range, start showing rotating hint
						if (!IsMinRotateRangeExceeded(diagramPresenter, mouseState))
							return Action.PrepareRotate;
						else return CurrentAction;
					} else return CurrentAction;

				default: throw new NShapeUnsupportedValueException(CurrentAction);
			}
		}

		#endregion


		#region [Private] Action implementations

		#region Selecting Shapes

		// (Un)Select shape unter the mouse pointer
		private bool PerformSelection(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo) {
			bool result = false;
			bool multiSelect = mouseState.IsKeyPressed(KeysDg.Control) || mouseState.IsKeyPressed(KeysDg.Shift);

			// When selecting shapes conteolpoints should be ignored as the user does not see them 
			// until a shape is selected
			const ControlPointCapabilities capabilities = ControlPointCapabilities.None;
			const int range = 0;

			// Determine the shape that has to be selected:
			Shape shapeToSelect = null;
			if (!selectedShapeAtCursorInfo.IsEmpty) {
				// When in multiSelection mode, unselect the selected shape under the cursor
				if (multiSelect) shapeToSelect = selectedShapeAtCursorInfo.Shape;
				else {
					// First, check if the selected shape under the cursor has children that can be selected
					shapeToSelect = selectedShapeAtCursorInfo.Shape.Children.FindShape(mouseState.X, mouseState.Y, capabilities, range, null);
					// Second, check if the selected shape under the cursor has siblings that can be selected
					if (shapeToSelect == null && selectedShapeAtCursorInfo.Shape.Parent != null) {
						shapeToSelect = selectedShapeAtCursorInfo.Shape.Parent.Children.FindShape(mouseState.X, mouseState.Y, capabilities, range, selectedShapeAtCursorInfo.Shape);
						// Discard found shape if it is the selected shape at cursor
						if (shapeToSelect == selectedShapeAtCursorInfo.Shape) shapeToSelect = null;
						if (shapeToSelect == null) {
							foreach (Shape shape in selectedShapeAtCursorInfo.Shape.Parent.Children.FindShapes(mouseState.X, mouseState.Y, capabilities, range)) {
								if (shape == selectedShapeAtCursorInfo.Shape) continue;
								// Ignore layer visibility for child shapes
								shapeToSelect = shape;
								break;
							}
						}
					}
					// temp hack to always select the top-most shape: don't try and select the lower shapes
					/*
					// Third, check if there are non-selected shapes below the selected shape under the cursor
					Shape startShape = selectedShapeAtCursorInfo.Shape;
					while (startShape.Parent != null) startShape = startShape.Parent;
					if (shapeToSelect == null && diagramPresenter.Diagram.Shapes.Contains(startShape))
						shapeToSelect = diagramPresenter.Diagram.Shapes.FindShape(mouseState.X, mouseState.Y, capabilities, range, startShape);
					*/
				}
			}

			// If there was a shape to select related to the selected shape under the cursor
			// (a child or a sibling of the selected shape or a shape below it),
			// try to select the first non-selected shape under the cursor
			if (shapeToSelect == null && shapeAtCursorInfo.Shape != null
				&& shapeAtCursorInfo.Shape.ContainsPoint(mouseState.X, mouseState.Y))
				shapeToSelect = shapeAtCursorInfo.Shape;

			// If a new shape to select was found, perform selection
			if (shapeToSelect != null) {
				// (check if multiselection mode is enabled (Shift + Click or Ctrl + Click))
				if (multiSelect) {
					// if multiSelect is enabled, add/remove to/from selected selectedShapes...
					if (diagramPresenter.SelectedShapes.Contains(shapeToSelect)) {
						// if object is selected -> remove from selection
						diagramPresenter.UnselectShape(shapeToSelect);
						RemovePreviewOf(shapeToSelect);
						result = true;
					} else {
						// If object is not selected -> add to selection
						diagramPresenter.SelectShape(shapeToSelect, true);
						result = true;
					}
				} else {
					// ... otherwise deselect all selectedShapes but the clicked object
					ClearPreviews();
					// check if the clicked shape is a child of an already selected shape
					Shape childShape = null;
					if (diagramPresenter.SelectedShapes.Count == 1
						&& diagramPresenter.SelectedShapes.TopMost.Children != null
						&& diagramPresenter.SelectedShapes.TopMost.Children.Count > 0) {
						childShape = diagramPresenter.SelectedShapes.TopMost.Children.FindShape(mouseState.X, mouseState.Y, ControlPointCapabilities.None, 0, null);
					}
					if (childShape != null) diagramPresenter.SelectShape(childShape, false);
					else diagramPresenter.SelectShape(shapeToSelect, false);
					result = true;
				}

				// validate if the desired shape or its parent was selected
				if (shapeToSelect.Parent != null) {
					if (!diagramPresenter.SelectedShapes.Contains(shapeToSelect))
						if (diagramPresenter.SelectedShapes.Contains(shapeToSelect.Parent))
							shapeToSelect = shapeToSelect.Parent;
				}
			} else if (selectedShapeAtCursorInfo.IsEmpty) {
				// if there was no other shape to select and none of the selected shapes is under the cursor,
				// clear selection
				if (!multiSelect) {
					if (diagramPresenter.SelectedShapes.Count > 0) {
						diagramPresenter.UnselectAll();
						ClearPreviews();
						result = true;
					}
				}
			} else {
				// if there was no other shape to select and a selected shape is under the cursor,
				// do nothing
			}
			return result;
		}


		// Calculate new selection frame
		private void PrepareSelectionFrame(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			previewMouseState = mouseState;

			Invalidate(ActionDiagramPresenter);

			frameRect.X = Math.Min(ActionStartMouseState.X, mouseState.X);
			frameRect.Y = Math.Min(ActionStartMouseState.Y, mouseState.Y);
			frameRect.Width = Math.Max(ActionStartMouseState.X, mouseState.X) - frameRect.X;
			frameRect.Height = Math.Max(ActionStartMouseState.Y, mouseState.Y) - frameRect.Y;

			Invalidate(ActionDiagramPresenter);
		}


		// Select shapes inside the selection frame
		private bool PerformFrameSelection(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			bool multiSelect = mouseState.IsKeyPressed(KeysDg.Control) || mouseState.IsKeyPressed(KeysDg.Shift);
			diagramPresenter.SelectShapes(frameRect, multiSelect);
			return true;
		}

		#endregion


		#region Connecting / Disconnecting GluePoints

		private bool ShapeHasGluePoint(Shape shape) {
			foreach (ControlPointId id in shape.GetControlPointIds(ControlPointCapabilities.Glue))
				return true;
			return false;
		}


		private void DisconnectGluePoints(IDiagramPresenter diagramPresenter) {
			foreach (Shape selectedShape in diagramPresenter.SelectedShapes) {
				foreach (ControlPointId ptId in selectedShape.GetControlPointIds(ControlPointCapabilities.Connect | ControlPointCapabilities.Glue)) {
					// disconnect GluePoints if they are moved together with their targets
					bool skip = false;
					foreach (ShapeConnectionInfo ci in selectedShape.GetConnectionInfos(ptId, null)) {
						if (ci.OwnPointId != ptId) throw new NShapeInternalException("Fatal error: Unexpected ShapeConnectionInfo was returned.");
						if (diagramPresenter.SelectedShapes.Contains(ci.OtherShape)) {
							skip = false;
							break;
						}
					}
					if (skip) continue;

					// otherwise, compare positions of the GluePoint with it's targetPoint and disconnect if they are not equal
					if (selectedShape.HasControlPointCapability(ptId, ControlPointCapabilities.Glue)) {
						Shape previewShape = FindPreviewOfShape(selectedShape);
						if (selectedShape.GetControlPointPosition(ptId) != previewShape.GetControlPointPosition(ptId)) {
							bool isConnected = false;
							foreach (ShapeConnectionInfo sci in selectedShape.GetConnectionInfos(ptId, null)) {
								if (sci.OwnPointId == ptId) {
									isConnected = true;
									break;
								} else throw new NShapeInternalException("Fatal error: Unexpected ShapeConnectionInfo was returned.");
							}
							if (isConnected) {
								ICommand cmd = new DisconnectCommand(selectedShape, ptId);
								diagramPresenter.Project.ExecuteCommand(cmd);
							}
						}
					}
				}
			}
		}


		private void ConnectGluePoints(IDiagramPresenter diagramPresenter) {
			foreach (Shape selectedShape in diagramPresenter.SelectedShapes) {
				// find selectedShapes that own GluePoints
				foreach (ControlPointId gluePointId in selectedShape.GetControlPointIds(ControlPointCapabilities.Glue)) {
					Point gluePointPos = Point.Empty;
					gluePointPos = selectedShape.GetControlPointPosition(gluePointId);

					// find selectedShapes to connect to
					foreach (Shape shape in FindVisibleShapes(diagramPresenter, gluePointPos.X, gluePointPos.Y, ControlPointCapabilities.Connect, diagramPresenter.GripSize)) {
						if (diagramPresenter.SelectedShapes.Contains(shape)) {
							// restore connections that were disconnected before
							int targetPointId = shape.FindNearestControlPoint(gluePointPos.X, gluePointPos.Y, 0, ControlPointCapabilities.Connect);
							if (targetPointId != ControlPointId.None)
								selectedShape.Connect(gluePointId, shape, targetPointId);
						} else {
							ShapeAtCursorInfo shapeInfo = FindConnectionTarget(diagramPresenter, selectedShape, gluePointId, gluePointPos, true, true);
							if (shapeInfo.ControlPointId != ControlPointId.None) {
								ICommand cmd = new ConnectCommand(selectedShape, gluePointId, shapeInfo.Shape, shapeInfo.ControlPointId);
								diagramPresenter.Project.ExecuteCommand(cmd);
							}
							//else if (shape.ContainsPoint(gluePointPos.X, gluePointPos.Y)) {
							//   ICommand cmd = new ConnectCommand(selectedShape, gluePointId, shape, ControlPointId.Reference);
							//   display.Project.ExecuteCommand(cmd);
							//}
						}
					}
				}
			}
		}

		#endregion


		#region Moving Shapes

		// prepare drawing preview of move action
		private void PrepareMoveShapePreview(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			previewMouseState = mouseState;

#if DEBUG_DIAGNOSTICS
				Assert(diagramPresenter.SelectedShapes.Count > 0);
				Assert(!SelectedShapeAtCursorInfo.IsEmpty);
#endif
			// calculate the movement
			int distanceX = mouseState.X - ActionStartMouseState.X;
			int distanceY = mouseState.Y - ActionStartMouseState.Y;
			// calculate "Snap to Grid" offset
			snapDeltaX = snapDeltaY = 0;
			if (diagramPresenter.SnapToGrid && !selectedShapeAtCursorInfo.IsEmpty) {
				FindNearestSnapPoint(diagramPresenter, selectedShapeAtCursorInfo.Shape, distanceX, distanceY, out snapDeltaX, out snapDeltaY);
				distanceX += snapDeltaX;
				distanceY += snapDeltaY;
			}

			// Reset all shapes to start values
			ResetPreviewShapes(diagramPresenter);

			// Move (preview copies of) the selected shapes
			Rectangle shapeBounds = Rectangle.Empty;
			foreach (Shape originalShape in diagramPresenter.SelectedShapes) {
				// Get preview of the shape to move...
				Shape previewShape = FindPreviewOfShape(originalShape);
				// ...and move the preview shape to the new position
				previewShape.MoveTo(originalShape.X + distanceX, originalShape.Y + distanceY);
			}
		}


		// Apply the move action
		private bool PerformMoveShape(IDiagramPresenter diagramPresenter) {
			bool result = false;
			if (selectedShapeAtCursorInfo.IsEmpty) {
				// This should never happen...
				Debug.Assert(!selectedShapeAtCursorInfo.IsEmpty);
			}

			if (ActionStartMouseState.Position != CurrentMouseState.Position) {
				// calculate the movement
				int distanceX = CurrentMouseState.X - ActionStartMouseState.X;
				int distanceY = CurrentMouseState.Y - ActionStartMouseState.Y;
				//snapDeltaX = snapDeltaY = 0;
				//if (diagramPresenter.SnapToGrid)
				//   FindNearestSnapPoint(diagramPresenter, SelectedShapeAtCursorInfo.Shape, distanceX, distanceY, out snapDeltaX, out snapDeltaY, ControlPointCapabilities.All);

#if DEBUG_DIAGNOSTICS
				int dx = distanceX + snapDeltaX;
				int dy = distanceY + snapDeltaY;
				Debug.Assert(CurrentMouseState.Position == CurrentMouseState.Position);
				foreach (Shape selectedShape in diagramPresenter.SelectedShapes) {
					Shape previewShape = previewShapes[selectedShape];
					Debug.Assert((previewShape.X == selectedShape.X && previewShape.Y == selectedShape.Y)
							|| (previewShape.X == selectedShape.X + dx && previewShape.Y == selectedShape.Y + dy), 
							string.Format("Preview shape '{0}' was expected at position {1} but it is at position {2}.", 
								previewShape.Type.Name, new Point(selectedShape.X, selectedShape.Y), new Point(previewShape.X, previewShape.Y)));
				}
#endif

				ICommand cmd = new MoveShapeByCommand(diagramPresenter.SelectedShapes, distanceX + snapDeltaX, distanceY + snapDeltaY);
				diagramPresenter.Project.ExecuteCommand(cmd);

				snapDeltaX = snapDeltaY = 0;
				snapPtId = ControlPointId.None;
				result = true;
			}
			return result;
		}

		#endregion


		#region Moving ControlPoints

		// prepare drawing preview of resize action 
		private void PrepareMoveHandlePreview(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			previewMouseState = mouseState;

			InvalidateConnectionTargets(diagramPresenter, CurrentMouseState.X, CurrentMouseState.Y);

			int distanceX = mouseState.X - ActionStartMouseState.X;
			int distanceY = mouseState.Y - ActionStartMouseState.Y;

			// calculate "Snap to Grid/ControlPoint" offset
			snapDeltaX = snapDeltaY = 0;
			if (selectedShapeAtCursorInfo.IsCursorAtGluePoint) {
				ControlPointId targetPtId;
				Shape targetShape = FindNearestControlPoint(diagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Connect, distanceX, distanceY, out snapDeltaX, out snapDeltaY, out targetPtId);
			} else
				FindNearestSnapPoint(diagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, distanceX, distanceY, out snapDeltaX, out snapDeltaY);
			distanceX += snapDeltaX;
			distanceY += snapDeltaY;

			// Reset all preview shapes to start values
			ResetPreviewShapes(diagramPresenter);

			// Move selected shapes
			ResizeModifiers resizeModifier = GetResizeModifier(mouseState);
			Point originalPtPos = Point.Empty;
			foreach (Shape selectedShape in diagramPresenter.SelectedShapes) {
				Shape previewShape = FindPreviewOfShape(selectedShape);
				// Perform movement
				if (previewShape.HasControlPointCapability(selectedShapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Resize))
					previewShape.MoveControlPointBy(selectedShapeAtCursorInfo.ControlPointId, distanceX, distanceY, resizeModifier);
			}

			InvalidateConnectionTargets(diagramPresenter, mouseState.X, mouseState.Y);
		}


		// apply the resize action
		private bool PerformMoveHandle(IDiagramPresenter diagramPresenter) {
			bool result = false;
			Invalidate(diagramPresenter);

			int distanceX = CurrentMouseState.X - ActionStartMouseState.X;
			int distanceY = CurrentMouseState.Y - ActionStartMouseState.Y;

			// if the moved ControlPoint is a single GluePoint, snap to ConnectionPoints
			bool isGluePoint = false;
			if (diagramPresenter.SelectedShapes.Count == 1)
				isGluePoint = selectedShapeAtCursorInfo.Shape.HasControlPointCapability(selectedShapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Glue);

			// Snap to Grid or ControlPoint
			bool calcSnapDistance = true;
			ShapeAtCursorInfo targetShapeInfo = ShapeAtCursorInfo.Empty;
			if (isGluePoint) {
				Point currentPtPos = selectedShapeAtCursorInfo.Shape.GetControlPointPosition(selectedShapeAtCursorInfo.ControlPointId);
				Point newPtPos = Point.Empty;
				newPtPos.Offset(currentPtPos.X + distanceX, currentPtPos.Y + distanceY);
				targetShapeInfo = FindConnectionTarget(ActionDiagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, newPtPos, true, true);
				if (!targetShapeInfo.IsEmpty) {
					// If there is a target shape to connect to, get the position of the target connection point 
					// and move the gluepoint exactly to this position
					calcSnapDistance = false;
					if (targetShapeInfo.ControlPointId != ControlPointId.Reference) {
						Point pt = targetShapeInfo.Shape.GetControlPointPosition(targetShapeInfo.ControlPointId);
						distanceX = pt.X - currentPtPos.X;
						distanceY = pt.Y - currentPtPos.Y;
					} else {
						// If the target point is the reference point, use the previously calculated snap distance
						// ToDo: We need a solution for calculating the nearest point on the target shape's outline
						distanceX += snapDeltaX;
						distanceY += snapDeltaY;
					}
				}
			}
			if (calcSnapDistance) {
				FindNearestSnapPoint(diagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, distanceX, distanceY, out snapDeltaX, out snapDeltaY);
				distanceX += snapDeltaX;
				distanceY += snapDeltaY;
			}

			ResizeModifiers resizeModifier = GetResizeModifier(CurrentMouseState);
			if (isGluePoint) {
				ICommand cmd = new MoveGluePointCommand(selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, targetShapeInfo.Shape, targetShapeInfo.ControlPointId, distanceX, distanceY, resizeModifier);
				diagramPresenter.Project.ExecuteCommand(cmd);
			} else {
				ICommand cmd = new MoveControlPointCommand(ActionDiagramPresenter.SelectedShapes, selectedShapeAtCursorInfo.ControlPointId, distanceX, distanceY, resizeModifier);
				diagramPresenter.Project.ExecuteCommand(cmd);
			}

			snapDeltaX = snapDeltaY = 0;
			snapPtId = ControlPointId.None;
			result = true;

			return result;
		}

		#endregion


		#region Rotating Shapes

		private int CalcStartAngle(MouseState startMouseState, MouseState currentMouseState) {
#if DEBUG_DIAGNOSTICS
			Assert(startMouseState != MouseState.Empty);
			Assert(currentMouseState != MouseState.Empty);
#endif
			float angleRad = Geometry.Angle(startMouseState.Position, currentMouseState.Position);
			return (3600 + Geometry.RadiansToTenthsOfDegree(angleRad)) % 3600;
		}


		private int CalcSweepAngle(MouseState initMouseState, MouseState prevMouseState, MouseState newMouseState) {
#if DEBUG_DIAGNOSTICS
			Assert(initMouseState != MouseState.Empty);
			Assert(prevMouseState != MouseState.Empty);
			Assert(newMouseState != MouseState.Empty);
#endif
			float angleRad = Geometry.Angle(initMouseState.Position, prevMouseState.Position, newMouseState.Position);
			return (3600 + Geometry.RadiansToTenthsOfDegree(angleRad)) % 3600;
		}


		private int AlignAngle(int angle, MouseState mouseState) {
			int result = angle;
			if (mouseState.IsKeyPressed(KeysDg.Control) && mouseState.IsKeyPressed(KeysDg.Shift)) {
				// rotate by tenths of degrees
				// do nothing 
			} else if (mouseState.IsKeyPressed(KeysDg.Control)) {
				// rotate by full degrees
				result -= (result % 10);
			} else if (mouseState.IsKeyPressed(KeysDg.Shift)) {
				// rotate by 5 degrees
				result -= (result % 50);
			} else {
				// default:
				// rotate by 15 degrees
				result -= (result % 150);
			}
			return result;
		}


		private void CalcAngle(MouseState initMouseState, MouseState startMouseState, MouseState newMouseState, out int startAngle, out int sweepAngle) {
			startAngle = CalcStartAngle(initMouseState, ActionStartMouseState);
			int rawSweepAngle = CalcSweepAngle(initMouseState, ActionStartMouseState, newMouseState);
			sweepAngle = AlignAngle(rawSweepAngle, newMouseState);
		}


		private void CalcAngle(MouseState initMouseState, MouseState startMouseState, MouseState currentMouseState, MouseState newMouseState, out int startAngle, out int sweepAngle, out int prevSweepAngle) {
			CalcAngle(initMouseState, startMouseState, newMouseState, out startAngle, out sweepAngle);
			int rawPrevSweepAngle = CalcSweepAngle(initMouseState, ActionStartMouseState, currentMouseState);
			prevSweepAngle = AlignAngle(rawPrevSweepAngle, currentMouseState);
		}


		private bool IsMinRotateRangeExceeded(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (mouseState == MouseState.Empty) throw new ArgumentException("mouseState");
			Point p;
			if (PendingToolActionsCount <= 1) p = ActionStartMouseState.Position;
			else {
				MouseState prevMouseState = GetPreviousMouseState();
				p = prevMouseState.Position;
			}
			Debug.Assert(Geometry.IsValid(p));
			int dist = (int)Math.Round(Geometry.DistancePointPoint(p.X, p.Y, mouseState.X, mouseState.Y));
			diagramPresenter.DiagramToControl(dist, out dist);
			return (dist > diagramPresenter.MinRotateRange);
		}


		private MouseState GetPreviousMouseState() {
			MouseState result = MouseState.Empty;
			bool firstItem = true;
			foreach (ActionDef actionDef in PendingToolActions) {
				if (!firstItem) {
					result = actionDef.MouseState;
					break;
				} else firstItem = false;
			}
			return result;
		}


		// prepare drawing preview of rotate action
		private void PrepareRotatePreview(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			previewMouseState = mouseState;
			InvalidateAnglePreview(ActionDiagramPresenter);
			if (PendingToolActionsCount >= 1
				&& ActionStartMouseState.Position != mouseState.Position) {
				if (IsMinRotateRangeExceeded(diagramPresenter, mouseState)) {
					// calculate new angle
					MouseState initMouseState = GetPreviousMouseState();
					int startAngle, sweepAngle, prevSweepAngle;
					CalcAngle(initMouseState, ActionStartMouseState, CurrentMouseState, mouseState,
						out startAngle, out sweepAngle, out prevSweepAngle);

					// Reset all preview shapes to start values
					ResetPreviewShapes(diagramPresenter);

					// ToDo: Implement rotation around a common rotation center
					Point rotationCenter = Point.Empty;
					foreach (Shape selectedShape in diagramPresenter.SelectedShapes) {
						Shape previewShape = FindPreviewOfShape(selectedShape);
						// Get ControlPointId of the first rotate control point
						ControlPointId rotatePtId = ControlPointId.None;
						foreach (ControlPointId id in previewShape.GetControlPointIds(ControlPointCapabilities.Rotate)) {
							rotatePtId = id;
							break;
						}
						if (rotatePtId == ControlPointId.None) throw new NShapeInternalException("{0} has no rotate control point.");
						rotationCenter = previewShape.GetControlPointPosition(rotatePtId);

						//// Restore original shape's angle
						//previewShape.Rotate(-prevSweepAngle, rotationCenter.X, rotationCenter.Y);

						// Perform rotation
						previewShape.Rotate(sweepAngle, rotationCenter.X, rotationCenter.Y);
					}
				}
			}
			InvalidateAnglePreview(ActionDiagramPresenter);
		}


		// apply rotate action
		private bool PerformRotate(IDiagramPresenter diagramPresenter) {
			bool result = false;
			if (PendingToolActionsCount >= 1
				&& ActionStartMouseState.Position != CurrentMouseState.Position
				&& IsMinRotateRangeExceeded(ActionDiagramPresenter, CurrentMouseState)) {
				// Calculate rotation
				MouseState initMouseState = GetPreviousMouseState();
				int startAngle, sweepAngle;
				CalcAngle(initMouseState, ActionStartMouseState, CurrentMouseState, out startAngle, out sweepAngle);
				// Create and execute command
				ICommand cmd = new RotateShapesCommand(diagramPresenter.SelectedShapes, sweepAngle);
				diagramPresenter.Project.ExecuteCommand(cmd);
				result = true;
			}
			return result;
		}


		/// <summary>
		/// Specifies if a double click on the rotation handle will rotate the shape by 90°
		/// </summary>
		public bool EnableQuickRotate {
			get { return enableQuickRotate; }
			set { enableQuickRotate = value; }
		}


		private bool PerformQuickRotate(IDiagramPresenter diagramPresenter, int angle) {
			bool result = false;
			if (enableQuickRotate) {
				ICommand cmd = new RotateShapesCommand(diagramPresenter.SelectedShapes, angle);
				diagramPresenter.Project.ExecuteCommand(cmd);
				InvalidateAnglePreview(diagramPresenter);
				result = true;
			}
			return result;
		}


		private void InvalidateAnglePreview(IDiagramPresenter diagramPresenter) {
			// invalidate previous angle preview
			diagramPresenter.InvalidateDiagram(
				rectBuffer.X - rectBuffer.Width - diagramPresenter.GripSize,
				rectBuffer.Y - rectBuffer.Height - diagramPresenter.GripSize,
				rectBuffer.Width + rectBuffer.Width + (2 * diagramPresenter.GripSize),
				rectBuffer.Height + rectBuffer.Height + (2 * diagramPresenter.GripSize));

			int requiredDistance;
			diagramPresenter.ControlToDiagram(diagramPresenter.MinRotateRange, out requiredDistance);
			int length = (int)Math.Round(Geometry.DistancePointPoint(ActionStartMouseState.X, ActionStartMouseState.Y, CurrentMouseState.X, CurrentMouseState.Y));

			// invalidate current angle preview / instruction preview
			rectBuffer.Location = ActionStartMouseState.Position;
			if (length > requiredDistance)
				rectBuffer.Width = rectBuffer.Height = length;
			else
				rectBuffer.Width = rectBuffer.Height = requiredDistance;
			diagramPresenter.InvalidateDiagram(rectBuffer.X - rectBuffer.Width, rectBuffer.Y - rectBuffer.Height, rectBuffer.Width + rectBuffer.Width, rectBuffer.Height + rectBuffer.Height);
		}

		#endregion

		#endregion


		#region [Private] Preview management implementation

		/// <summary>
		/// The dictionary of preview shapes: The key is the original shape, the value is the preview shape.
		/// </summary>
		private IDictionary<Shape, Shape> Previews {
			get { return previewShapes; }
		}


		private Shape FindPreviewOfShape(Shape shape) {
			if (shape == null) throw new ArgumentNullException("shape");
#if DEBUG_DIAGNOSTICS
			Assert(previewShapes.ContainsKey(shape), string.Format("No preview found for '{0}' shape.", shape.Type.Name));
#endif
			return previewShapes[shape];
		}


		private Shape FindShapeOfPreview(Shape previewShape) {
			if (previewShape == null) throw new ArgumentNullException("previewShape");
#if DEBUG_DIAGNOSTICS
			Assert(originalShapes.ContainsKey(previewShape), string.Format("No original shape found for '{0}' preview shape.", previewShape.Type.Name));
#endif
			return originalShapes[previewShape];
		}


		private void AddPreview(Shape shape, Shape previewShape, IDisplayService displayService) {
			if (originalShapes.ContainsKey(previewShape)) return;
			if (previewShapes.ContainsKey(shape)) return;
			// Set DisplayService for the preview shape
			if (previewShape.DisplayService != displayService)
				previewShape.DisplayService = displayService;

			// Add shape and its preview to the appropriate dictionaries
			previewShapes.Add(shape, previewShape);
			originalShapes.Add(previewShape, shape);

			// Add shape's children and their previews to the appropriate dictionaries
			if (previewShape.Children.Count > 0) {
				IEnumerator<Shape> previewChildren = previewShape.Children.TopDown.GetEnumerator();
				IEnumerator<Shape> originalChildren = shape.Children.TopDown.GetEnumerator();

				previewChildren.Reset();
				originalChildren.Reset();
				bool processNext = false;
				if (previewChildren.MoveNext() && originalChildren.MoveNext())
					processNext = true;
				while (processNext) {
					AddPreview(originalChildren.Current, previewChildren.Current, displayService);
					processNext = (previewChildren.MoveNext() && originalChildren.MoveNext());
				}
			}
		}


		private void RemovePreviewOf(Shape originalShape) {
			if (previewShapes.ContainsKey(originalShape)) {
				// Invalidate Preview Shape
				Shape previewShape = Previews[originalShape];
				previewShape.Invalidate();

				// remove previews of the shape and its children from the preview's dictionary
				previewShapes.Remove(originalShape);
				if (originalShape.Children.Count > 0) {
					foreach (Shape childShape in originalShape.Children)
						previewShapes.Remove(childShape);
				}
				// remove the shape and its children from the shape's dictionary
				originalShapes.Remove(previewShape);
				if (previewShape.Children.Count > 0) {
					foreach (Shape childShape in previewShape.Children)
						originalShapes.Remove(childShape);
				}
			}
		}


		private void RemovePreview(Shape previewShape) {
			Shape origShape = null;
			if (!originalShapes.TryGetValue(previewShape, out origShape))
				throw new NShapeInternalException("This preview shape has no associated original shape in this tool.");
			else {
				// Invalidate Preview Shape
				previewShape.Invalidate();
				// Remove both, original- and preview shape from the appropriate dictionaries
				previewShapes.Remove(origShape);
				originalShapes.Remove(previewShape);
			}
		}


		private void ClearPreviews() {
			foreach (KeyValuePair<Shape, Shape> item in previewShapes) {
				Shape previewShape = item.Value;
				DeletePreviewShape(ref previewShape);
			}
			previewShapes.Clear();
			originalShapes.Clear();
		}


		private bool IsConnectedToNonSelectedShape(IDiagramPresenter diagramPresenter, Shape shape) {
			foreach (ControlPointId gluePointId in shape.GetControlPointIds(ControlPointCapabilities.Glue)) {
				ShapeConnectionInfo sci = shape.GetConnectionInfo(gluePointId, null);
				if (!sci.IsEmpty
					&& !diagramPresenter.SelectedShapes.Contains(sci.OtherShape))
					return true;
			}
			return false;
		}


		/// <summary>
		/// Create previews of shapes connected to the given shape (and it's children) and connect them to the
		/// shape's preview (or the preview of it's child)
		/// </summary>
		private void ConnectPreviewOfShape(IDiagramPresenter diagramPresenter, Shape shape) {
			// process shape's children
			if (shape.Children != null && shape.Children.Count > 0) {
				foreach (Shape childShape in shape.Children)
					ConnectPreviewOfShape(diagramPresenter, childShape);
			}

			Shape preview = FindPreviewOfShape(shape);
			foreach (ShapeConnectionInfo connectionInfo in shape.GetConnectionInfos(ControlPointId.Any, null)) {
				if (!diagramPresenter.IsLayerVisible(connectionInfo.OtherShape.Layers)) continue;
				if (diagramPresenter.SelectedShapes.Contains(connectionInfo.OtherShape)) {
					// Do not connect previews if BOTH of the connected shapes are part of the selection because 
					// this would restrict movement of the connector shapes and decreases performance (many 
					// unnecessary FollowConnectionPointWithGluePoint() calls)
					if (shape.HasControlPointCapability(connectionInfo.OwnPointId, ControlPointCapabilities.Glue)) {
						if (IsConnectedToNonSelectedShape(diagramPresenter, shape)) {
							Shape targetPreview = FindPreviewOfShape(connectionInfo.OtherShape);
							preview.Connect(connectionInfo.OwnPointId, targetPreview, connectionInfo.OtherPointId);
						}
					}
				} else {
					// Connect preview of shape to a non-selected shape if it is a single shape 
					// that has a glue point (e.g. a Label)
					if (preview.HasControlPointCapability(connectionInfo.OwnPointId, ControlPointCapabilities.Glue)) {
						// Only connect if the control point to be connected is not the control point to be moved
						if (shape == selectedShapeAtCursorInfo.Shape && connectionInfo.OwnPointId != selectedShapeAtCursorInfo.ControlPointId)
							preview.Connect(connectionInfo.OwnPointId, connectionInfo.OtherShape, connectionInfo.OtherPointId);
					} else
						// Create a preview of the shape that is connected to the preview (recursive call)
						CreateConnectedTargetPreviewShape(diagramPresenter, preview, connectionInfo);
				}
			}
		}


		/// <summary>
		/// Creates (or finds) a preview of the connection's PassiveShape and connects it to the current preview shape
		/// </summary>
		private void CreateConnectedTargetPreviewShape(IDiagramPresenter diagramPresenter, Shape previewShape, ShapeConnectionInfo connectionInfo) {
			// Check if any other selected shape is connected to the same non-selected shape
			Shape previewTargetShape;
			// If the current passiveShape is already connected to another shape of the current selection,
			// connect the current preview to the other preview's passiveShape
			if (!targetShapeBuffer.TryGetValue(connectionInfo.OtherShape, out previewTargetShape)) {
				// If the current passiveShape is not connected to any other of the selected selectedShapes,
				// create a clone of the passiveShape and connect it to the corresponding preview
				// If the preview exists, abort connecting (in this case, the shape is a preview of a child shape)
				if (previewShapes.ContainsKey(connectionInfo.OtherShape))
					return;
				else {
					previewTargetShape = connectionInfo.OtherShape.Type.CreatePreviewInstance(connectionInfo.OtherShape);
					AddPreview(connectionInfo.OtherShape, previewTargetShape, diagramPresenter.DisplayService);
				}
				// Add passive shape and it's clone to the passive shape dictionary
				targetShapeBuffer.Add(connectionInfo.OtherShape, previewTargetShape);
			}
			// Connect the (new or existing) preview shapes
			// Skip connecting if the preview is already connected.
#if DEBUG_DIAGNOSTICS
			Assert(previewTargetShape != null, "Error while creating connected preview shapes.");
#endif
			if (previewTargetShape.IsConnected(connectionInfo.OtherPointId, null) == ControlPointId.None) {
				previewTargetShape.Connect(connectionInfo.OtherPointId, previewShape, connectionInfo.OwnPointId);
				// check, if any shapes are connected to the connector (that is connected to the selected shape)
				foreach (ShapeConnectionInfo connectorCI in connectionInfo.OtherShape.GetConnectionInfos(ControlPointId.Any, null)) {
					if (!diagramPresenter.IsLayerVisible(connectorCI.OtherShape.Layers)) continue;
					// skip if the connector is connected to the shape with more than one glue point
					if (connectorCI.OtherShape == FindShapeOfPreview(previewShape)) continue;
					if (connectorCI.OwnPointId != connectionInfo.OtherPointId) {
						// Check if the shape on the other end is selected.
						// If it is, connect to it's preview or skip connecting if the target preview does 
						// not exist yet (it will be connected when creating the targt's preview)
						if (diagramPresenter.SelectedShapes.Contains(connectorCI.OtherShape)) {
							if (previewShapes.ContainsKey(connectorCI.OtherShape)) {
								Shape s = FindPreviewOfShape(connectorCI.OtherShape);
								if (s.IsConnected(connectorCI.OtherPointId, previewTargetShape) == ControlPointId.None)
									previewTargetShape.Connect(connectorCI.OwnPointId, s, connectorCI.OtherPointId);
							} else continue;
						} else if (connectorCI.OtherShape.HasControlPointCapability(connectorCI.OtherPointId, ControlPointCapabilities.Glue))
							// Connect connectors connected to the previewTargetShape
							CreateConnectedTargetPreviewShape(diagramPresenter, previewTargetShape, connectorCI);
						else if (connectorCI.OtherPointId == ControlPointId.Reference) {
							// Connect the other end of the previewTargetShape if the connection is a Point-To-Shape connection
#if DEBUG_DIAGNOSTICS
							Assert(connectorCI.OtherShape.IsConnected(connectorCI.OtherPointId, previewTargetShape) == ControlPointId.None);
#endif
							if (previewTargetShape.IsConnected(connectorCI.OwnPointId, null) == ControlPointId.None)
								previewTargetShape.Connect(connectorCI.OwnPointId, connectorCI.OtherShape, connectorCI.OtherPointId);
						}
					}
				}
			}
		}


		// Experimental code
		//private void CreatePreviewsOfConnectedShapes(IDiagramPresenter diagramPresenter, Shape shape) {
		//   // process shape's children
		//   if (shape.Children != null && shape.Children.Count > 0) {
		//      foreach (Shape childShape in shape.Children)
		//         CreatePreviewsOfConnectedShapes(diagramPresenter, childShape);
		//   }

		//   Shape preview = FindPreviewOfShape(shape);
		//   foreach (ShapeConnectionInfo connectionInfo in shape.GetConnectionInfos(ControlPointId.Any, null)) {
		//      if (diagramPresenter.SelectedShapes.Contains(connectionInfo.OtherShape)) continue;
		//      // Create a preview of the shape that is connected to the preview (recursive call)
		//      DoCreatePreviewsOfConnectedShape(diagramPresenter, preview, connectionInfo);
		//   }
		//}


		///// <summary>
		///// Creates (or finds) a preview of the connection's PassiveShape and connects it to the current preview shape
		///// </summary>
		//private void DoCreatePreviewsOfConnectedShape(IDiagramPresenter diagramPresenter, Shape previewShape, ShapeConnectionInfo connectionInfo) {
		//   // Check if any other selected shape is connected to the same non-selected shape
		//   Shape previewTargetShape;
		//   // If the current passiveShape is already connected to another shape of the current selection,
		//   // connect the current preview to the other preview's passiveShape
		//   if (!targetShapeBuffer.TryGetValue(connectionInfo.OtherShape, out previewTargetShape)) {
		//      // If the current passiveShape is not connected to any other of the selected selectedShapes,
		//      // create a clone of the passiveShape and connect it to the corresponding preview
		//      // If the preview exists, abort connecting (in this case, the shape is a preview of a child shape)
		//      if (previewShapes.ContainsKey(connectionInfo.OtherShape)) 
		//         return;
		//      else {
		//         previewTargetShape = connectionInfo.OtherShape.Type.CreatePreviewInstance(connectionInfo.OtherShape);
		//         AddPreview(connectionInfo.OtherShape, previewTargetShape, diagramPresenter.DisplayService);
		//      }
		//      // Add passive shape and it's clone to the passive shape dictionary
		//      targetShapeBuffer.Add(connectionInfo.OtherShape, previewTargetShape);
		//   }
		//   // Connect the (new or existing) preview shapes
		//   // Skip connecting if the preview is already connected.
		//   Assert(previewTargetShape != null, "Error while creating connected preview shapes.");
		//   // check, if any shapes are connected to the connector (that is connected to the selected shape)
		//   foreach (ShapeConnectionInfo connectorCI in connectionInfo.OtherShape.GetConnectionInfos(ControlPointId.Any, null)) {
		//      if (connectorCI.OtherShape == FindShapeOfPreview(previewShape)) continue;
		//      if (connectorCI.OwnPointId == connectionInfo.OtherPointId) continue;
		//      DoCreatePreviewsOfConnectedShape(diagramPresenter, previewTargetShape, connectorCI);
		//   }
		//}


		///// <summary>
		///// Create previews of shapes connected to the given shape (and it's children) and connect them to the
		///// shape's preview (or the preview of it's child)
		///// </summary>
		//private void ConnectPreviewsToShape(IDiagramPresenter diagramPresenter, Shape shape) {
		//   // process shape's children
		//   if (shape.Children != null && shape.Children.Count > 0) {
		//      foreach (Shape childShape in shape.Children)
		//         ConnectPreviewsToShape(diagramPresenter, childShape);
		//   }

		//   Shape preview = FindPreviewOfShape(shape);
		//   foreach (ShapeConnectionInfo connectionInfo in shape.GetConnectionInfos(ControlPointId.Any, null)) {
		//      if (diagramPresenter.SelectedShapes.Contains(connectionInfo.OtherShape)) {
		//         // Do not connect previews if BOTH of the connected shapes are part of the selection because 
		//         // this would restrict movement of the connector shapes and decreases performance (many 
		//         // unnecessary FollowConnectionPointWithGluePoint() calls)
		//         if (shape.HasControlPointCapability(connectionInfo.OwnPointId, ControlPointCapabilities.Glue)) {
		//            if (IsConnectedToNonSelectedShape(diagramPresenter, shape)) {
		//               Shape targetPreview = FindPreviewOfShape(connectionInfo.OtherShape);
		//               preview.Connect(connectionInfo.OwnPointId, targetPreview, connectionInfo.OtherPointId);
		//            }
		//         }
		//      } else {
		//         // Connect preview of shape to a non-selected shape if it is a single shape 
		//         // that has a glue point (e.g. a Label)
		//         if (preview.HasControlPointCapability(connectionInfo.OwnPointId, ControlPointCapabilities.Glue)) {
		//            // Only connect if the control point to be connected is not the control point to be moved
		//            if (shape == SelectedShapeAtCursorInfo.Shape && connectionInfo.OwnPointId != SelectedShapeAtCursorInfo.ControlPointId) {
		//               if (preview.IsConnected(connectionInfo.OwnPointId, null) == ControlPointId.None)
		//                  preview.Connect(connectionInfo.OwnPointId, connectionInfo.OtherShape, connectionInfo.OtherPointId);
		//            }
		//         } else
		//            // Create a preview of the shape that is connected to the preview (recursive call)
		//            DoConnectPreviewsToShape(diagramPresenter, preview, connectionInfo);
		//      }
		//   }
		//}


		///// <summary>
		///// Creates (or finds) a preview of the connection's PassiveShape and connects it to the current preview shape
		///// </summary>
		//private void DoConnectPreviewsToShape(IDiagramPresenter diagramPresenter, Shape previewShape, ShapeConnectionInfo connectionInfo) {
		//   // Check if any other selected shape is connected to the same non-selected shape
		//   Shape previewTargetShape;
		//   // If the current passiveShape is already connected to another shape of the current selection,
		//   // connect the current preview to the other preview's passiveShape
		//   if (!targetShapeBuffer.TryGetValue(connectionInfo.OtherShape, out previewTargetShape)) return;
			
		//   if (previewTargetShape.IsConnected(connectionInfo.OtherPointId, null) == ControlPointId.None) {
		//      previewTargetShape.Connect(connectionInfo.OtherPointId, previewShape, connectionInfo.OwnPointId);
		//      // check, if any shapes are connected to the connector (that is connected to the selected shape)
		//      foreach (ShapeConnectionInfo connectorCI in connectionInfo.OtherShape.GetConnectionInfos(ControlPointId.Any, null)) {
		//         // skip if the connector is connected to the shape with more than one glue point
		//         if (connectorCI.OtherShape == FindShapeOfPreview(previewShape)) continue;
		//         if (connectorCI.OwnPointId != connectionInfo.OtherPointId) {
		//            // Check if the shape on the other end is selected.
		//            // If it is, connect to it's preview or skip connecting if the target preview does 
		//            // not exist yet (it will be connected when creating the targt's preview)
		//            if (diagramPresenter.SelectedShapes.Contains(connectorCI.OtherShape)) {
		//               if (previewShapes.ContainsKey(connectorCI.OtherShape)) {
		//                  Shape s = FindPreviewOfShape(connectorCI.OtherShape);
		//                  if (s.IsConnected(connectorCI.OtherPointId, previewTargetShape) == ControlPointId.None)
		//                     previewTargetShape.Connect(connectorCI.OwnPointId, s, connectorCI.OtherPointId);
		//               } else continue;
		//            } else if (connectorCI.OtherShape.HasControlPointCapability(connectorCI.OtherPointId, ControlPointCapabilities.Glue))
		//               // Connect connectors connected to the previewTargetShape
		//               DoConnectPreviewsToShape(diagramPresenter, previewTargetShape, connectorCI);
		//            else if (connectorCI.OtherPointId == ControlPointId.Reference) {
		//               // Connect the other end of the previewTargetShape if the connection is a Point-To-Shape connection
		//               Assert(connectorCI.OtherShape.IsConnected(connectorCI.OtherPointId, previewTargetShape) == ControlPointId.None);
		//               Assert(previewTargetShape.IsConnected(connectorCI.OwnPointId, null) == ControlPointId.None);
		//               previewTargetShape.Connect(connectorCI.OwnPointId, connectorCI.OtherShape, connectorCI.OtherPointId);
		//            }
		//         }
		//      }
		//   }
		//}

		#endregion


		#region [Private] Helper Methods

		private void SetSelectedShapeAtCursor(IDiagramPresenter diagramPresenter, int mouseX, int mouseY, int handleRadius, ControlPointCapabilities handleCapabilities) {
			// Find the shape under the cursor
			selectedShapeAtCursorInfo.Clear();
			selectedShapeAtCursorInfo.Shape = diagramPresenter.SelectedShapes.FindShape(mouseX, mouseY, handleCapabilities, handleRadius, null);

			// If there is a shape under the cursor, find the nearest control point and caption
			if (!selectedShapeAtCursorInfo.IsEmpty) {
				ControlPointCapabilities capabilities;
				if (CurrentMouseState.IsKeyPressed(KeysDg.Control)) capabilities = ControlPointCapabilities.Resize /*| ControlPointCapabilities.Movable*/;
				else if (CurrentMouseState.IsKeyPressed(KeysDg.Shift)) capabilities = ControlPointCapabilities.Rotate;
				else capabilities = gripCapabilities;

				// Find control point at cursor that belongs to the selected shape at cursor
				selectedShapeAtCursorInfo.ControlPointId = selectedShapeAtCursorInfo.Shape.FindNearestControlPoint(mouseX, mouseY, diagramPresenter.ZoomedGripSize, capabilities);
				// Find caption at cursor (if the shape is a captioned shape)
				if (selectedShapeAtCursorInfo.Shape is ICaptionedShape && ((ICaptionedShape)selectedShapeAtCursorInfo.Shape).CaptionCount > 0)
					selectedShapeAtCursorInfo.CaptionIndex = ((ICaptionedShape)selectedShapeAtCursorInfo.Shape).FindCaptionFromPoint(mouseX, mouseY);
			}
		}


		private bool ShapeOrShapeRelativesContainsPoint(Shape shape, int x, int y, ControlPointCapabilities capabilities, int range) {
			if (shape.HitTest(x, y, capabilities, range) != ControlPointId.None)
				return true;
			else if (shape.Parent != null) {
				if (ShapeOrShapeRelativesContainsPoint(shape.Parent, x, y, capabilities, range))
					return true;
			}
			return false;
		}


		private int DetermineCursor(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			switch (CurrentAction) {
				case Action.None:
					// If no action is pending, the folowing cursors are possible:
					// - Default (no selected shape under cursor or action not granted)
					// - Move shape cursor
					// - Move grip cursor
					// - Rotate cursor
					// - Edit caption cursor
					if (!selectedShapeAtCursorInfo.IsEmpty) {
						// Check if cursor is over a caption and editing caption is feasible
						if (IsEditCaptionFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo))
							//return cursors[ToolCursor.EditCaption];
							return cursors[ToolCursor.MoveShape];
						// Check if cursor is over a control point and moving grips or rotating is feasible
						if (selectedShapeAtCursorInfo.IsCursorAtGrip) {
							if (IsMoveHandleFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo))
								return cursors[ToolCursor.MoveHandle];
							else if (IsRotatatingFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo))
								return cursors[ToolCursor.Rotate];
							else return cursors[ToolCursor.Default];
						}
						// Check if cursor is inside the shape and move shape is feasible
						if (IsMoveShapeFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo))
							return cursors[ToolCursor.MoveShape];
					}
					return cursors[ToolCursor.Default];

				case Action.Select:
				case Action.SelectWithFrame:
					return cursors[ToolCursor.Default];

				case Action.EditCaption:
#if DEBUG_DIAGNOSTICS
					Assert(!SelectedShapeAtCursorInfo.IsEmpty);
					Assert(SelectedShapeAtCursorInfo.Shape is ICaptionedShape);
#endif
					// If the cursor is outside the caption, return default cursor
					int captionIndex = ((ICaptionedShape)selectedShapeAtCursorInfo.Shape).FindCaptionFromPoint(mouseState.X, mouseState.Y);
					if (captionIndex == selectedShapeAtCursorInfo.CaptionIndex)
						return cursors[ToolCursor.EditCaption];
					else return cursors[ToolCursor.Default];

				case Action.MoveHandle:
#if DEBUG_DIAGNOSTICS
					Assert(!SelectedShapeAtCursorInfo.IsEmpty);
					Assert(SelectedShapeAtCursorInfo.IsCursorAtGrip);
#endif
					if (selectedShapeAtCursorInfo.IsCursorAtGluePoint) {
						Shape previewShape = FindPreviewOfShape(selectedShapeAtCursorInfo.Shape);
						Point ptPos = previewShape.GetControlPointPosition(selectedShapeAtCursorInfo.ControlPointId);
						ShapeAtCursorInfo shapeAtCursorInfo = FindConnectionTarget(
							diagramPresenter,
							selectedShapeAtCursorInfo.Shape,
							selectedShapeAtCursorInfo.ControlPointId,
							ptPos,
							true,
							false);
						if (!shapeAtCursorInfo.IsEmpty && shapeAtCursorInfo.IsCursorAtConnectionPoint)
							return cursors[ToolCursor.Connect];
					}
					return cursors[ToolCursor.MoveHandle];

				case Action.MoveShape:
					return cursors[ToolCursor.MoveShape];

				case Action.PrepareRotate:
				case Action.Rotate:
					return cursors[ToolCursor.Rotate];

				default: throw new NShapeUnsupportedValueException(CurrentAction);
			}
		}


		/// <summary>
		/// Create Previews of all shapes selected in the CurrentDisplay.
		/// These previews are connected to all the shapes the original shapes are connected to.
		/// </summary>
		private void CreatePreviewShapes(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (Previews.Count == 0 && diagramPresenter.SelectedShapes.Count > 0) {
				// first, clone all selected shapes...
				foreach (Shape shape in diagramPresenter.SelectedShapes) {
					if (!diagramPresenter.IsLayerVisible(shape.Layers)) continue;
					AddPreview(shape, shape.Type.CreatePreviewInstance(shape), diagramPresenter.DisplayService);
				}
				// ...then restore connections between previews and connections between previews and non-selected shapes
				targetShapeBuffer.Clear();
				foreach (Shape selectedShape in diagramPresenter.SelectedShapes.BottomUp) {
					if (!diagramPresenter.IsLayerVisible(selectedShape.Layers)) continue;
					// AttachGluePointToConnectionPoint the preview shape (and all it's cildren) to all the shapes the original shape was connected to
					// Additionally, create previews for all connected shapes and connect these to the appropriate target shapes
					ConnectPreviewOfShape(diagramPresenter, selectedShape);
				}
				targetShapeBuffer.Clear();
			}
		}


		private void ResetPreviewShapes(IDiagramPresenter diagramPresenter) {
			// Dictionary "originalShapes" contains the previewShape as "Key" and the original shape as "Value"
			foreach (KeyValuePair<Shape, Shape> pair in originalShapes) {
				// Copy all properties of the original shape to the preview shape 
				// (assigns the original model object to the preview shape!)
				IModelObject modelObj = pair.Value.ModelObject;
				pair.Key.CopyFrom(pair.Value);
				// If the original shape has a model object, clone and assign it.
				// Then, transform the copied shape into a preview shape
				pair.Key.ModelObject = null;
				pair.Key.MakePreview(diagramPresenter.Project.Design);
				if (modelObj != null) pair.Key.ModelObject = modelObj.Clone();
			}
		}


		private void InvalidateShapes(IDiagramPresenter diagramPresenter, IEnumerable<Shape> shapes, bool invalidateGrips) {
			foreach (Shape shape in shapes)
				DoInvalidateShape(diagramPresenter, shape, invalidateGrips);
		}


		private void DoInvalidateShape(IDiagramPresenter diagramPresenter, Shape shape, bool invalidateGrips) {
			if (shape.Parent != null)
				DoInvalidateShape(diagramPresenter, shape.Parent, false);
			else {
				shape.Invalidate();
				if (invalidateGrips)
					diagramPresenter.InvalidateGrips(shape, ControlPointCapabilities.All);
			}
		}


		private bool IsDragActionFeasible(MouseState mouseState, MouseButtonsDg button) {
			int dx = 0, dy = 0;
			if (mouseState.IsButtonDown(button) && IsToolActionPending) {
				// Check the minimum drag distance before switching to a drag action
				dx = Math.Abs(mouseState.X - ActionStartMouseState.X);
				dy = Math.Abs(mouseState.Y - ActionStartMouseState.Y);
			}
			return (dx >= MinimumDragDistance.Width || dy >= MinimumDragDistance.Height);
		}


		private bool IsMoveShapeFeasible(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo) {
			if (shapeAtCursorInfo.IsEmpty || mouseState.IsEmpty)
				return false;
			if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, shapeAtCursorInfo.Shape))
				return false;
			if (diagramPresenter.SelectedShapes.Count > 0 && !diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, diagramPresenter.SelectedShapes))
				return false;
			if (!shapeAtCursorInfo.Shape.ContainsPoint(mouseState.X, mouseState.Y))
				return false;
			if (shapeAtCursorInfo.Shape.HasControlPointCapability(ControlPointId.Reference, ControlPointCapabilities.Glue))
				if (shapeAtCursorInfo.Shape.IsConnected(ControlPointId.Reference, null) != ControlPointId.None) return false;

			// Check if the shape is connected to other shapes with its glue points
			if (diagramPresenter.SelectedShapes.Count == 0) {
				// Check if the non-selected shape at cursor (which will be selected) owns glue points connected to other shapes
				foreach (ControlPointId id in shapeAtCursorInfo.Shape.GetControlPointIds(ControlPointCapabilities.Glue))
					if (shapeAtCursorInfo.Shape.IsConnected(id, null) != ControlPointId.None) return false;
			} else if (diagramPresenter.SelectedShapes.Contains(shapeAtCursorInfo.Shape)) {
				// ToDo: If there are *many* shapes selected (e.g. 10000), this check will be extremly slow...
				if (diagramPresenter.SelectedShapes.Count < 10000) {
					// LinearShapes that own connected gluePoints may not be moved.
					foreach (Shape shape in diagramPresenter.SelectedShapes) {
						if (shape is ILinearShape) {
							foreach (ControlPointId gluePointId in shape.GetControlPointIds(ControlPointCapabilities.Glue)) {
								ShapeConnectionInfo sci = shape.GetConnectionInfo(gluePointId, null);
								if (!sci.IsEmpty) {
									// Allow movement if the connected shapes are moved together
									if (!diagramPresenter.SelectedShapes.Contains(sci.OtherShape))
										return false;
								}
							}
						}
					}
				}
			}
			return true;
		}


		private bool IsMoveHandleFeasible(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo) {
			if (shapeAtCursorInfo.IsEmpty)
				return false;
			// Collides with the resize modifiers
			//if (mouseState.IsKeyPressed(KeysDg.Shift)) return false;
			if (shapeAtCursorInfo.Shape.HasControlPointCapability(shapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Glue)) {
				if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Connect, diagramPresenter.SelectedShapes))
					return false;
			} else {
				if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, diagramPresenter.SelectedShapes))
					return false;
			}
			if (!shapeAtCursorInfo.Shape.HasControlPointCapability(shapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Resize | ControlPointCapabilities.Glue /*| ControlPointCapabilities.Movable*/))
				return false;
			if (diagramPresenter.SelectedShapes.Count > 1) {
				// GluePoints may only be moved alone
				if (shapeAtCursorInfo.Shape.HasControlPointCapability(shapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Glue))
					return false;
				// Check if all shapes that are going to be resizes are of the same type
				Shape lastShape = null;
				foreach (Shape shape in diagramPresenter.SelectedShapes) {
					if (lastShape != null && lastShape.Type != shape.Type)
						return false;
					lastShape = shape;
				}
			}
			return true;
		}


		private bool IsRotatatingFeasible(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo) {
			if (shapeAtCursorInfo.IsEmpty)
				return false;
			// Collides with the rotate modifiers
			//if (mouseState.IsKeyPressed(KeysDg.Shift)) return false;
			if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, diagramPresenter.SelectedShapes))
				return false;
			if (!shapeAtCursorInfo.Shape.HasControlPointCapability(shapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Rotate))
				return false;
			if (diagramPresenter.SelectedShapes.Count > 1) {
				// check if all selected shapes have a rotate handle
				foreach (Shape selectedShape in diagramPresenter.SelectedShapes) {
					bool shapeHasRotateHandle = false;
					foreach (ControlPointId ptId in selectedShape.GetControlPointIds(ControlPointCapabilities.Rotate)) {
						shapeHasRotateHandle = true;
						break;
					}
					if (!shapeHasRotateHandle) return false;
				}
			}
			return true;
		}


		private bool IsEditCaptionFeasible(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo) {
			if (shapeAtCursorInfo.IsEmpty || mouseState.IsEmpty)
				return false;
			if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Data, shapeAtCursorInfo.Shape))
				return false;
			if (!shapeAtCursorInfo.IsCursorAtCaption)
				return false;
			// Not necessary any more: Edit caption is triggered on MouseUp event and only of the mouse was not moved.
			//if (mouseState.IsKeyPressed(KeysDg.Control) || mouseState.IsKeyPressed(KeysDg.Shift))
			//    return false;
			return true;
		}

		#endregion


		#region [Private] Construction

		static SelectionTool() {
			cursors = new Dictionary<ToolCursor, int>(8);
			// Register cursors
			cursors.Add(ToolCursor.Default, CursorProvider.DefaultCursorID);
			cursors.Add(ToolCursor.ActionDenied, CursorProvider.RegisterCursor(Properties.Resources.ActionDeniedCursor));
			cursors.Add(ToolCursor.EditCaption, CursorProvider.RegisterCursor(Properties.Resources.EditTextCursor));
			cursors.Add(ToolCursor.MoveShape, CursorProvider.RegisterCursor(Properties.Resources.MoveShapeCursor));
			cursors.Add(ToolCursor.MoveHandle, CursorProvider.RegisterCursor(Properties.Resources.MovePointCursor));
			cursors.Add(ToolCursor.Rotate, CursorProvider.RegisterCursor(Properties.Resources.RotateCursor));
			// ToDo: Create better Connect/Disconnect cursors
			cursors.Add(ToolCursor.Connect, CursorProvider.RegisterCursor(Properties.Resources.HandCursor));
			cursors.Add(ToolCursor.Disconnect, CursorProvider.RegisterCursor(Properties.Resources.HandCursor));
		}


		private void Construct() {
			Title = "Pointer";
			ToolTipText = "Select one or more objects by holding shift while clicking or drawing a frame."
				+ Environment.NewLine
				+ "Selected objects can be moved by dragging them to the target position or resized by dragging "
				+ "a control point to the target position.";

			SmallIcon = global::Dataweb.NShape.Properties.Resources.PointerIconSmall;
			SmallIcon.MakeTransparent(Color.Fuchsia);

			LargeIcon = global::Dataweb.NShape.Properties.Resources.PointerIconLarge;
			LargeIcon.MakeTransparent(Color.Fuchsia);

			frameRect = Rectangle.Empty;
		}

		#endregion


		#region [Private] Types

		private enum Action { None, Select, SelectWithFrame, EditCaption, MoveShape, MoveHandle, PrepareRotate, Rotate }


		private enum ToolCursor {
			Default,
			Rotate,
			MoveHandle,
			MoveShape,
			ActionDenied,
			EditCaption,
			Connect,
			Disconnect
		}


		// connection handling stuff
		private struct ConnectionInfoBuffer : IEquatable<ConnectionInfoBuffer> {

			public static readonly ConnectionInfoBuffer Empty;

			public static bool operator ==(ConnectionInfoBuffer x, ConnectionInfoBuffer y) { return (x.connectionInfo == y.connectionInfo && x.shape == y.shape); }

			public static bool operator !=(ConnectionInfoBuffer x, ConnectionInfoBuffer y) { return !(x == y); }

			public Shape shape;

			public ShapeConnectionInfo connectionInfo;

			/// <override></override>
			public override bool Equals(object obj) { return obj is ConnectionInfoBuffer && this == (ConnectionInfoBuffer)obj; }

			/// <ToBeCompleted></ToBeCompleted>
			public bool Equals(ConnectionInfoBuffer other) {
				return other == this;
			}

			/// <override></override>
			public override int GetHashCode() { return base.GetHashCode(); }

			static ConnectionInfoBuffer() {
				Empty.shape = null;
				Empty.connectionInfo = ShapeConnectionInfo.Empty;
			}
		}

		#endregion


		#region Fields

		// --- Description of the tool ---
		private static Dictionary<ToolCursor, int> cursors;
		//
		private bool enableQuickRotate = false;
		private ControlPointCapabilities gripCapabilities = ControlPointCapabilities.Resize | ControlPointCapabilities.Rotate /*| ControlPointCapabilities.Movable*/;

		// --- State after the last ProcessMouseEvent ---
		// selected shape under the mouse cursor, being highlighted in the next drawing
		private ShapeAtCursorInfo selectedShapeAtCursorInfo;
		// Rectangle that represents the transformed selection area in control coordinates
		private Rectangle frameRect;
		// Stores the distance the SelectedShape was moved on X-axis for snapping the nearest gridpoint
		private int snapDeltaX;
		// Stores the distance the SelectedShape was moved on Y-axis for snapping the nearest gridpoint
		private int snapDeltaY;
		// Index of the controlPoint that snapped to grid/point/swimline
		private int snapPtId;

		// -- Definition of current action
		// indicates the current action depending on the mouseButton State, selected selectedShapes and mouse movement
		//private ToolAction currentToolAction = ToolAction.None;
		// preview shapes (Key = original shape, Value = preview shape)
		private Dictionary<Shape, Shape> previewShapes = new Dictionary<Shape, Shape>();
		// original shapes (Key = preview shape, Value = original shape)
		private Dictionary<Shape, Shape> originalShapes = new Dictionary<Shape, Shape>();
		// 
		// Specifies whether the currently calculated preview has been drawn. 
		// As long as the calculated preview is not drawn yet, the new preview will not be calculated.
		private MouseState previewMouseState = MouseState.Empty;

		// Buffers
		// rectangle buffer 
		private Rectangle rectBuffer;
		// used for buffering selectedShapes connected to the preview selectedShapes: key = passiveShape, values = targetShapes's clone
		private Dictionary<Shape, Shape> targetShapeBuffer = new Dictionary<Shape, Shape>();
		// buffer used for storing connections that are temporarily disconnected for moving shapes
		private List<ConnectionInfoBuffer> connectionsBuffer = new List<ConnectionInfoBuffer>();

		#endregion
	}


	/// <summary>
	/// Lets the user create a templated shape.
	/// </summary>
	public abstract class TemplateTool : Tool {

		/// <ToBeCompleted></ToBeCompleted>
		public Template Template {
			get { return template; }
		}


		/// <override></override>
		public override void Dispose() {
			// Do not dispose the Template - it has to be disposed by the cache
			base.Dispose();
		}


		/// <override></override>
		public override void RefreshIcons() {
			using (Shape clone = Template.Shape.Clone()) {
				clone.DrawThumbnail(base.LargeIcon, margin, transparentColor);
				base.LargeIcon.MakeTransparent(transparentColor);
			}
			using (Shape clone = Template.Shape.Clone()) {
				clone.DrawThumbnail(base.SmallIcon, margin, transparentColor);
				base.SmallIcon.MakeTransparent(transparentColor);
			}
			ClearPreview();
			Title = Template.Title;
		}


		/// <override></override>
		protected TemplateTool(Template template, string category)
			: base(category) {
			if (template == null) throw new ArgumentNullException("template");
			this.template = template;
			Title = template.Title;
			ToolTipText = string.Format("Inserts a {0}.", Title);
			if (!string.IsNullOrEmpty(template.Shape.Type.Description))
				ToolTipText += Environment.NewLine + template.Shape.Type.Description;
			RefreshIcons();
		}


		/// <override></override>
		protected TemplateTool(Template template)
			: this(template, (template != null) ? template.Shape.Type.DefaultCategoryTitle : null) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Shape PreviewShape {
			get { return previewShape; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void CreatePreview(IDiagramPresenter diagramPresenter) {
			using (Shape s = Template.CreateShape())
				previewShape = Template.Shape.Type.CreatePreviewInstance(s);
			previewShape.DisplayService = diagramPresenter.DisplayService;
			previewShape.Invalidate();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void ClearPreview() {
			if (previewShape != null)
				DeletePreviewShape(ref previewShape);
		}


		#region Fields

		private Template template;
		private Shape previewShape;

		#endregion
	}


	/// <summary>
	/// Lets the user create a shape based on a point sequence.
	/// </summary>
	public class LinearShapeCreationTool : TemplateTool {

		/// <ToBeCompleted></ToBeCompleted>
		public LinearShapeCreationTool(Template template)
			: this(template, null) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LinearShapeCreationTool(Template template, string category)
			: base(template, category) {
			if (!(template.Shape is ILinearShape))
				throw new NShapeException("The template's shape does not implement {0}.", typeof(ILinearShape).Name);
			ToolTipText += Environment.NewLine;
			ToolTipText += Environment.NewLine + "Left click starts a new line or adds a point to the new line.";
			ToolTipText += Environment.NewLine + "Right click ends the new line (last point will be discarded).";
			ToolTipText += Environment.NewLine + "Double click ends the new line (including the last point).";
		}


		#region IDisposable Interface

		/// <override></override>
		public override void Dispose() {
			base.Dispose();
		}

		#endregion


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			yield break;
		}


		/// <override></override>
		public override bool ProcessMouseEvent(IDiagramPresenter diagramPresenter, MouseEventArgsDg e) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			bool result = false;

			MouseState newMouseState = MouseState.Empty;
			newMouseState.Buttons = e.Buttons;
			newMouseState.Modifiers = e.Modifiers;
			diagramPresenter.ControlToDiagram(e.Position, out newMouseState.Position);

			diagramPresenter.SuspendUpdate();
			try {
				switch (e.EventType) {
					case MouseEventType.MouseMove:
						if (CurrentMouseState.Position != newMouseState.Position)
							ProcessMouseMove(diagramPresenter, newMouseState);
						break;
					case MouseEventType.MouseDown:
						// MouseDown starts drag-based actions
						// ToDo: Implement these features: Adding Segments to existing Lines, Move existing Lines and their ControlPoints
						if (e.Clicks > 1) result = ProcessDoubleClick(diagramPresenter, newMouseState);
						else result = ProcessMouseClick(diagramPresenter, newMouseState);
						break;

					case MouseEventType.MouseUp:
						// MouseUp finishes drag-actions. Click-based actions are handled by the MouseClick event
						// ToDo: Implement these features: Adding Segments to existing Lines, Move existing Lines and their ControlPoints
						break;

					default: throw new NShapeUnsupportedValueException(e.EventType);
				}
			} finally { diagramPresenter.ResumeUpdate(); }
			base.ProcessMouseEvent(diagramPresenter, e);
			return result;
		}


		/// <override></override>
		public override bool ProcessKeyEvent(IDiagramPresenter diagramPresenter, KeyEventArgsDg e) {
			return base.ProcessKeyEvent(diagramPresenter, e);
		}


		/// <override></override>
		public override void EnterDisplay(IDiagramPresenter diagramPresenter) {
			Invalidate(diagramPresenter);
		}


		/// <override></override>
		public override void LeaveDisplay(IDiagramPresenter diagramPresenter) {
			Invalidate(diagramPresenter);
		}


		/// <override></override>
		public override void Invalidate(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (PreviewShape != null) {
				diagramPresenter.InvalidateGrips(PreviewShape, ControlPointCapabilities.All);
				Point p = PreviewShape.GetControlPointPosition(ControlPointId.LastVertex);
				InvalidateConnectionTargets(diagramPresenter, p.X, p.Y);
			} else InvalidateConnectionTargets(diagramPresenter, CurrentMouseState.X, CurrentMouseState.Y);
		}


		/// <override></override>
		public override void Draw(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			// Draw preview shape
			if (PreviewShape != null) {
				// Draw preview shape and its ControlPoints
				diagramPresenter.DrawShape(PreviewShape);
				diagramPresenter.ResetTransformation();
				try {
					foreach (ControlPointId pointId in PreviewShape.GetControlPointIds(ControlPointCapabilities.Glue | ControlPointCapabilities.Resize))
						diagramPresenter.DrawResizeGrip(IndicatorDrawMode.Normal, PreviewShape, pointId);
				} finally { diagramPresenter.RestoreTransformation(); }
			}

			// Highlight ConnectionPoints in range
			if (!CurrentMouseState.IsEmpty) {
				if (Template.Shape.HasControlPointCapability(ControlPointId.LastVertex, ControlPointCapabilities.Glue)) {
					if (PreviewShape == null) DrawConnectionTargets(diagramPresenter, Template.Shape, CurrentMouseState.X, CurrentMouseState.Y);
					else {
						Point gluePtPos = PreviewShape.GetControlPointPosition(ControlPointId.LastVertex);
						DrawConnectionTargets(diagramPresenter, PreviewShape, ControlPointId.LastVertex, gluePtPos);
					}
				}
			}
		}


		/// <override></override>
		protected override void StartToolAction(IDiagramPresenter diagramPresenter, int action, MouseState mouseState, bool wantAutoScroll) {
			Debug.Print("StartToolAction");
			base.StartToolAction(diagramPresenter, action, mouseState, wantAutoScroll);
		}


		/// <override></override>
		protected override void EndToolAction() {
			Debug.Print("EndToolAction");
			base.EndToolAction();
			ClearPreview();
			modifiedLinearShape = null;
			pointAtCursor = ControlPointId.None;
			lastInsertedPointId = ControlPointId.None;
		}


		/// <override></override>
		protected override void CancelCore() {
			// Create the line until the last point that was created manually.
			// This feature only makes sense if an additional ControlPoint was created (other than the default points)
			ILinearShape templateShape = Template.Shape as ILinearShape;
			ILinearShape previewShape = PreviewShape as ILinearShape;
			if (IsToolActionPending && templateShape != null && previewShape != null 
				&& previewShape.VertexCount > templateShape.VertexCount)
					FinishLine(ActionDiagramPresenter, CurrentMouseState, true);
		}


		static LinearShapeCreationTool() {
			cursors = new Dictionary<ToolCursor, int>(6);
			cursors.Add(ToolCursor.Default, CursorProvider.DefaultCursorID);
			cursors.Add(ToolCursor.Pen, CursorProvider.RegisterCursor(Properties.Resources.PenCursor));
			cursors.Add(ToolCursor.ExtendLine, CursorProvider.RegisterCursor(Properties.Resources.PenPlusCursor));
			cursors.Add(ToolCursor.Connect, CursorProvider.RegisterCursor(Properties.Resources.HandCursor));
			cursors.Add(ToolCursor.Disconnect, CursorProvider.RegisterCursor(Properties.Resources.HandCursor));
			cursors.Add(ToolCursor.NotAllowed, CursorProvider.RegisterCursor(Properties.Resources.ActionDeniedCursor));
			// ToDo: Create better cursors for connecting/disconnecting
		}


		private bool ProcessMouseMove(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			bool result = false;
			ShapeAtCursorInfo shapeAtCursorInfo = ShapeAtCursorInfo.Empty;
			if (pointAtCursor != ControlPointId.None && ((modifiedLinearShape as Shape) != null || PreviewShape != null))
				shapeAtCursorInfo = FindConnectionTarget(diagramPresenter, (modifiedLinearShape as Shape) ?? PreviewShape, pointAtCursor, mouseState.Position, false, true);
			else shapeAtCursorInfo = FindShapeAtCursor(diagramPresenter, mouseState.X, mouseState.Y, ControlPointCapabilities.Connect | ControlPointCapabilities.Glue, diagramPresenter.ZoomedGripSize, false);

			// set cursor depending on the object under the mouse cursor
			int currentCursorId = DetermineCursor(diagramPresenter, shapeAtCursorInfo.Shape, shapeAtCursorInfo.ControlPointId);
			if (CurrentAction == Action.None)
				diagramPresenter.SetCursor(currentCursorId);
			else ActionDiagramPresenter.SetCursor(currentCursorId);

			switch (CurrentAction) {
				case Action.None:
					Invalidate(diagramPresenter);
					break;

				case Action.CreateLine:
				case Action.ExtendLine:
					Invalidate(ActionDiagramPresenter);

					// Check for connectionpoints wihtin the snapArea
					ResizeModifiers resizeModifier = GetResizeModifier(mouseState);
					if (!shapeAtCursorInfo.IsEmpty) {
						Point p = Point.Empty;
						if (shapeAtCursorInfo.IsCursorAtGrip)
							p = shapeAtCursorInfo.Shape.GetControlPointPosition(shapeAtCursorInfo.ControlPointId);
						else p = mouseState.Position;
#if DEBUG_DIAGNOSTICS
						Assert(PreviewShape != null);
#endif
						if (PreviewShape != null)
							PreviewShape.MoveControlPointTo(pointAtCursor, p.X, p.Y, resizeModifier);
					} else {
						int snapDeltaX = 0, snapDeltaY = 0;
						if (diagramPresenter.SnapToGrid)
							FindNearestSnapPoint(diagramPresenter, mouseState.X, mouseState.Y, out snapDeltaX, out snapDeltaY);
#if DEBUG_DIAGNOSTICS
						Assert(PreviewShape != null);
#endif
						if (PreviewShape != null)
							PreviewShape.MoveControlPointTo(pointAtCursor, mouseState.X + snapDeltaX, mouseState.Y + snapDeltaY, resizeModifier);
					}
					Invalidate(ActionDiagramPresenter);
					break;

				default: throw new NShapeUnsupportedValueException(CurrentAction);
			}
			return result;
		}


		private bool ProcessMouseClick(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			Debug.Print("ProcessMouseClick");
			bool result = false;
			switch (CurrentAction) {
				case Action.None:
					if (mouseState.IsButtonDown(MouseButtonsDg.Left)) {
						if (diagramPresenter.SelectedShapes.Count > 0)
							diagramPresenter.UnselectAll();

						ShapeAtCursorInfo targetShapeInfo = FindShapeAtCursor(diagramPresenter, mouseState.X, mouseState.Y, ControlPointCapabilities.Glue, diagramPresenter.ZoomedGripSize, false);
						if (IsExtendLineFeasible(CurrentAction, targetShapeInfo.Shape, targetShapeInfo.ControlPointId)) {
							if (diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, targetShapeInfo.Shape)) {
								ExtendLine(diagramPresenter, mouseState, targetShapeInfo);
								result = true;
							}
						} else {
							// If no other ToolAction is in Progress (e.g. drawing a line or moving a point),
							// a normal MouseClick starts a new line in Point-By-Point mode
							if (diagramPresenter.Project.SecurityManager.IsGranted(Permission.Insert, Template.Shape)) {
								CreateLine(diagramPresenter, mouseState);
								result = true;
							}
						}
					} else if (mouseState.IsButtonDown(MouseButtonsDg.Right)) {
						Cancel();
						result = true;
					}
					break;

				case Action.CreateLine:
				case Action.ExtendLine:
					if (mouseState.IsButtonDown(MouseButtonsDg.Left)) {
						Invalidate(ActionDiagramPresenter);
						bool doFinishLine = false;
						// If the line has reached the MaxVertexCount limit, create it
						if (PreviewLinearShape.VertexCount >= PreviewLinearShape.MaxVertexCount)
							doFinishLine = true;
						else {
							// Check if it has to be connected to a shape or connection point
							ShapeAtCursorInfo shapeAtCursorInfo = base.FindShapeAtCursor(ActionDiagramPresenter, mouseState.X, mouseState.Y, ControlPointCapabilities.Connect, diagramPresenter.ZoomedGripSize, false);
							if (!shapeAtCursorInfo.IsEmpty && !shapeAtCursorInfo.IsCursorAtGluePoint)
								doFinishLine = true;
							else AddNewPoint(ActionDiagramPresenter, mouseState);
						}
						// Create line if necessary
						if (doFinishLine) {
							if (CurrentAction == Action.CreateLine)
								FinishLine(ActionDiagramPresenter, mouseState, false);
							else FinishExtendLine(ActionDiagramPresenter, mouseState, false);

							while (IsToolActionPending)
								EndToolAction();
							OnToolExecuted(ExecutedEventArgs);
						}
					} else if (mouseState.IsButtonDown(MouseButtonsDg.Right)) {
#if DEBUG_DIAGNOSTICS
						Assert(PreviewShape != null);
#endif
						// When creating a line, the new line has to have more than the minimum number of 
						// vertices because the last vertex (sticking to the mouse cursor) will not be created.
						if (CurrentAction == Action.CreateLine) {
							if (PreviewLinearShape.VertexCount <= PreviewLinearShape.MinVertexCount)
								Cancel();
							else FinishLine(ActionDiagramPresenter, mouseState, true);
						} else {
							// When extending a line, the new line has to have more than the minimum number of 
							// vertices and more than the original line because the last vertex will not be created.
							if (PreviewLinearShape.VertexCount <= PreviewLinearShape.MinVertexCount
								|| PreviewLinearShape.VertexCount - 1 == modifiedLinearShape.VertexCount)
								Cancel();
							else FinishExtendLine(ActionDiagramPresenter, mouseState, true);
						}

						while (IsToolActionPending)
							EndToolAction();
						OnToolExecuted(ExecutedEventArgs);
					}
					result = true;
					break;

				default: throw new NShapeUnsupportedValueException(CurrentAction);
			}
			Invalidate(diagramPresenter);
			return result;
		}


		private bool ProcessDoubleClick(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			Debug.Print("ProcessDoubleClick");
			bool result = false;
			if (IsToolActionPending) {
#if DEBUG_DIAGNOSTICS
				Assert(PreviewShape != null);
#endif
				if (CurrentAction == Action.CreateLine)
					FinishLine(ActionDiagramPresenter, mouseState, true);
				else if (CurrentAction == Action.ExtendLine)
					FinishExtendLine(ActionDiagramPresenter, mouseState, true);
				
				while (IsToolActionPending)
					EndToolAction();

				OnToolExecuted(ExecutedEventArgs);
				result = true;
			}
			return result;
		}


		private ILinearShape PreviewLinearShape {
			get { return (ILinearShape)PreviewShape; }
		}


		private Action CurrentAction {
			get {
				if (IsToolActionPending)
					return (Action)CurrentToolAction.Action;
				else return Action.None;
			}
		}


		/// <summary>
		/// Creates a new preview line shape
		/// </summary>
		private void CreateLine(IDiagramPresenter diagramPresenter, MouseState mouseState) {
			// Try to find a connection target
			ShapeAtCursorInfo targetShapeInfo = ShapeAtCursorInfo.Empty;
			if (diagramPresenter.Project.SecurityManager.IsGranted(Permission.Connect, Template.Shape))
				targetShapeInfo = FindConnectionTargetFromPosition(diagramPresenter, mouseState.X, mouseState.Y, false, true);

			int snapDeltaX = 0, snapDeltaY = 0;
			if (diagramPresenter.SnapToGrid) {
				if (targetShapeInfo.IsEmpty || targetShapeInfo.ControlPointId == ControlPointId.Reference)
					FindNearestSnapPoint(diagramPresenter, mouseState.X, mouseState.Y, out snapDeltaX, out snapDeltaY);
				else {
					Point p = targetShapeInfo.Shape.GetControlPointPosition(targetShapeInfo.ControlPointId);
					snapDeltaX = p.X - mouseState.X;
					snapDeltaY = p.Y - mouseState.Y;
				}
			}

			// set line's start coordinates
			Point start = Point.Empty;
			if (!targetShapeInfo.IsEmpty) {
				if (targetShapeInfo.ControlPointId == ControlPointId.Reference) {
					// ToDo: Get nearest point on line
					start = mouseState.Position;
					start.Offset(snapDeltaX, snapDeltaY);
				} else
					start = targetShapeInfo.Shape.GetControlPointPosition(targetShapeInfo.ControlPointId);
			} else {
				start = mouseState.Position;
				start.Offset(snapDeltaX, snapDeltaY);
			}
			// Start ToolAction
			StartToolAction(diagramPresenter, (int)Action.CreateLine, mouseState, true);

			// Create new preview shape
			CreatePreview(diagramPresenter);
			PreviewShape.MoveControlPointTo(ControlPointId.FirstVertex, start.X, start.Y, ResizeModifiers.None);
			PreviewShape.MoveControlPointTo(ControlPointId.LastVertex, mouseState.X, mouseState.Y, ResizeModifiers.None);
			// Connect to target shape if possible
			if (targetShapeInfo.IsCursorAtConnectionPoint) {
				if (CanConnectTo(PreviewShape, ControlPointId.FirstVertex, targetShapeInfo.Shape))
					PreviewShape.Connect(ControlPointId.FirstVertex, targetShapeInfo.Shape, targetShapeInfo.ControlPointId);
			}
			lastInsertedPointId = ControlPointId.FirstVertex;
			pointAtCursor = ControlPointId.LastVertex;
		}


		/// <summary>
		/// Creates a new preview line shape
		/// </summary>
		private void ExtendLine(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo targetShapeInfo) {
			// Try to find a connection target
			if (!targetShapeInfo.IsEmpty && targetShapeInfo.ControlPointId != ControlPointId.None) {
				// Start ToolAction
				StartToolAction(diagramPresenter, (int)Action.ExtendLine, mouseState, true);
				modifiedLinearShape = (ILinearShape)targetShapeInfo.Shape;

				// create new preview shape
				CreatePreview(diagramPresenter);
				PreviewShape.CopyFrom(targetShapeInfo.Shape);	// Template will be copied but this is not really necessary as all styles will be overwritten with preview styles
				PreviewShape.MakePreview(diagramPresenter.Project.Design);

				pointAtCursor = targetShapeInfo.ControlPointId;
				Point pointPos = targetShapeInfo.Shape.GetControlPointPosition(pointAtCursor);
				if (pointAtCursor == ControlPointId.FirstVertex) {
					ControlPointId insertId = PreviewLinearShape.GetNextVertexId(pointAtCursor);
					lastInsertedPointId = PreviewLinearShape.InsertVertex(insertId, pointPos.X, pointPos.Y);
				} else lastInsertedPointId = PreviewLinearShape.InsertVertex(pointAtCursor, pointPos.X, pointPos.Y);

				ResizeModifiers resizeModifier = GetResizeModifier(mouseState);
				PreviewShape.MoveControlPointTo(pointAtCursor, mouseState.X, mouseState.Y, resizeModifier);
			}
		}


		/// <summary>
		/// Inserts a new point into the current preview line before the end point (that is sticking to the mouse cursor).
		/// </summary>
		private void AddNewPoint(IDiagramPresenter diagramPresenter, MouseState mouseState) {
#if DEBUG_DIAGNOSTICS
			Assert(PreviewLinearShape != null);
#endif
			if (PreviewLinearShape.VertexCount < PreviewLinearShape.MaxVertexCount) {
				ControlPointId existingPointId = ControlPointId.None;
				Point pointPos = PreviewShape.GetControlPointPosition(pointAtCursor);
				foreach (ControlPointId ptId in PreviewShape.GetControlPointIds(ControlPointCapabilities.All)) {
					if (ptId == ControlPointId.Reference) continue;
					if (ptId == pointAtCursor) continue;
					Point p = PreviewShape.GetControlPointPosition(ptId);
					if (p == pointPos && ptId != ControlPointId.Reference) {
						existingPointId = ptId;
						break;
					}
				}
				if (existingPointId == ControlPointId.None) {
					//lastInsertedPointId = PreviewLinearShape.InsertVertex(ControlPointId.LastVertex, pointPos.X, pointPos.Y);
					if (pointAtCursor == ControlPointId.FirstVertex) {
						ControlPointId insertBeforeId = PreviewLinearShape.GetNextVertexId(pointAtCursor);
						lastInsertedPointId = PreviewLinearShape.InsertVertex(insertBeforeId, pointPos.X, pointPos.Y);
					} else
						lastInsertedPointId = PreviewLinearShape.InsertVertex(pointAtCursor, pointPos.X, pointPos.Y);
				}
			} else throw new InvalidOperationException(string.Format("Maximum number of verticex reached: {0}", PreviewLinearShape.MaxVertexCount));
		}


		/// <summary>
		/// Creates a new LinearShape and inserts it into the diagram of the CurrentDisplay by executing a Command.
		/// </summary>
		private void FinishLine(IDiagramPresenter diagramPresenter, MouseState mouseState, bool ignorePointAtMouse) {
#if DEBUG_DIAGNOSTICS
			Assert(PreviewShape != null);
#endif
			// Create a new shape from the template
			Shape newShape = Template.CreateShape();
			// Copy points from the PreviewShape to the new shape 
			// The current EndPoint of the preview (sticking to the mouse cursor) will be discarded
			foreach (ControlPointId pointId in PreviewShape.GetControlPointIds(ControlPointCapabilities.Resize)) {
				Point p = PreviewShape.GetControlPointPosition(pointId);
				switch (pointId) {
					case ControlPointId.Reference:
						// Skip ReferencePoint as it is not a physically existing vertex (identically to FirstVertex)
						continue;
					case ControlPointId.LastVertex:
						if (ignorePointAtMouse) continue;
						newShape.MoveControlPointTo(ControlPointId.LastVertex, p.X, p.Y, ResizeModifiers.None);
						break;
					case ControlPointId.FirstVertex:
						newShape.MoveControlPointTo(ControlPointId.FirstVertex, p.X, p.Y, ResizeModifiers.None);
						break;
					default:
						// Treat the last inserted Point as EndPoint
						if ((ignorePointAtMouse && pointId == lastInsertedPointId) || ((ILinearShape)newShape).VertexCount == ((ILinearShape)newShape).MaxVertexCount)
							newShape.MoveControlPointTo(ControlPointId.LastVertex, p.X, p.Y, ResizeModifiers.None);
						else ((ILinearShape)newShape).InsertVertex(ControlPointId.LastVertex, p.X, p.Y);
						break;
				}
			}

			// Insert the new linear shape
			ActionDiagramPresenter.InsertShape(newShape);

			// Create an aggregated command which performs connecting the new shape to other shapes in one step
			AggregatedCommand aggregatedCommand = null;
			// Create connections
			foreach (ControlPointId gluePointId in newShape.GetControlPointIds(ControlPointCapabilities.Glue)) {
				ShapeConnectionInfo sci = PreviewShape.GetConnectionInfo(gluePointId, null);
				if (!sci.IsEmpty) {
					if (aggregatedCommand == null) aggregatedCommand = new AggregatedCommand();
					aggregatedCommand.Add(new ConnectCommand(newShape, gluePointId, sci.OtherShape, sci.OtherPointId));
				} else {
					// Create connection for the last vertex
					Point gluePtPos = newShape.GetControlPointPosition(gluePointId);
					ShapeAtCursorInfo targetInfo = FindConnectionTarget(ActionDiagramPresenter, newShape, ControlPointId.LastVertex, gluePtPos, false, true);
					if (!targetInfo.IsEmpty &&
						!targetInfo.IsCursorAtGluePoint &&
						targetInfo.ControlPointId != ControlPointId.None) {
						if (aggregatedCommand == null) aggregatedCommand = new AggregatedCommand();
						aggregatedCommand.Add(new ConnectCommand(newShape, gluePointId, targetInfo.Shape, targetInfo.ControlPointId));
					}
				}
			}
			// execute command and insert it into the history
			if (aggregatedCommand != null)
				ActionDiagramPresenter.Project.ExecuteCommand(aggregatedCommand);
		}


		/// <summary>
		/// Creates a new LinearShape and inserts it into the diagram of the CurrentDisplay by executing a Command.
		/// </summary>
		private void FinishExtendLine(IDiagramPresenter diagramPresenter, MouseState mouseState, bool ignorePointAtMouse) {
#if DEBUG_DIAGNOSTICS
			Assert(PreviewShape != null);
			Assert(modifiedLinearShape != null);
#endif
			Shape modifiedShape = (Shape)modifiedLinearShape;
			
			// Copy points from the PreviewShape to the new shape 
			// Start at the opposite point of the point at mouse cursor and skip all existing points
			ControlPointId pointId, endPointId;
			bool firstToLast;
			if (pointAtCursor == ControlPointId.FirstVertex) {
				pointId = ControlPointId.LastVertex;
				endPointId = ignorePointAtMouse ? lastInsertedPointId : (ControlPointId)ControlPointId.FirstVertex;
				firstToLast = false;
			} else {
				pointId = ControlPointId.FirstVertex;
				endPointId = ignorePointAtMouse ? lastInsertedPointId : (ControlPointId)ControlPointId.LastVertex;
				firstToLast = true;
			}

			// Create an aggregated command which performs connecting the new shape to other shapes in one step
			AggregatedCommand aggregatedCommand = null;

			// Process all point id's
			do {
				ControlPointId nextPointId = GetNextResizePointId(PreviewLinearShape, pointId, firstToLast);
				ControlPointId nextOrigPtId = GetNextResizePointId(modifiedLinearShape, pointId, firstToLast);
				if (nextPointId != nextOrigPtId && nextPointId != endPointId) {
					// If the next point id of the preview does not equal the original shape's point id,
					// we have to create it...
					Point p = PreviewShape.GetControlPointPosition(nextPointId);
					ControlPointId beforePointId = (firstToLast) ? (ControlPointId)ControlPointId.LastVertex : pointId;
					if (aggregatedCommand == null) aggregatedCommand = new AggregatedCommand();
					aggregatedCommand.Add(new InsertVertexCommand(modifiedShape, beforePointId, p.X, p.Y));
				}
				pointId = nextPointId;
			} while (pointId != endPointId);
			// Set last point's position
			ControlPointId lastPtId = firstToLast ? ControlPointId.LastVertex : ControlPointId.FirstVertex;
			Point currPos = modifiedShape.GetControlPointPosition(lastPtId);
			Point newPos = PreviewShape.GetControlPointPosition(endPointId);
#if DEBUG_DIAGNOSTICS
			Assert(aggregatedCommand != null);
#endif
			aggregatedCommand.Add(new MoveControlPointCommand(modifiedShape, lastPtId, newPos.X - currPos.X, newPos.Y - currPos.Y, ResizeModifiers.None));

			// Create connection for the last vertex
			ShapeAtCursorInfo targetInfo = FindConnectionTarget(ActionDiagramPresenter, modifiedShape, lastPtId, newPos, false, true);
			if (!targetInfo.IsEmpty &&
				!targetInfo.IsCursorAtGluePoint &&
				targetInfo.ControlPointId != ControlPointId.None) {
				if (aggregatedCommand == null) aggregatedCommand = new AggregatedCommand();
				aggregatedCommand.Add(new ConnectCommand(modifiedShape, lastPtId, targetInfo.Shape, targetInfo.ControlPointId));
			}

			// Execute command and insert it into the history
			if (aggregatedCommand != null)
				ActionDiagramPresenter.Project.ExecuteCommand(aggregatedCommand);
		}


		private ControlPointId GetNextResizePointId(ILinearShape lineShape, ControlPointId currentPointId, bool firstToLast) {
			if (firstToLast) return lineShape.GetNextVertexId(currentPointId);
			else return lineShape.GetPreviousVertexId(currentPointId);
		}
		
		
		/// <summary>
		/// Set the cursor for the current action
		/// </summary>
		private int DetermineCursor(IDiagramPresenter diagramPresenter, Shape shape, ControlPointId pointId) {
			switch (CurrentAction) {
				case Action.None:
					if (IsExtendLineFeasible(CurrentAction, shape, pointId)) {
						if (diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, shape))
							return cursors[ToolCursor.ExtendLine];
						else return cursors[ToolCursor.NotAllowed];
					} else if (diagramPresenter.Project.SecurityManager.IsGranted(Permission.Insert, Template.Shape)) {
						if (shape != null && pointId != ControlPointId.None
							&& IsConnectingFeasible(CurrentAction, shape, pointId))
							return cursors[ToolCursor.Connect];
						else return cursors[ToolCursor.Pen];
					} else return cursors[ToolCursor.NotAllowed];

				case Action.CreateLine:
					if (diagramPresenter.Project.SecurityManager.IsGranted(Permission.Insert, Template.Shape)) {
						if (shape != null && pointId != ControlPointId.None
							&& IsConnectingFeasible(CurrentAction, shape, pointId))
							return cursors[ToolCursor.Connect];
						else return cursors[ToolCursor.Pen];
					} else return cursors[ToolCursor.NotAllowed];

				case Action.ExtendLine:
					if (diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, shape ?? (Shape)modifiedLinearShape)) {
						if (IsConnectingFeasible(CurrentAction, shape, pointId))
							return cursors[ToolCursor.Connect];
						else return cursors[ToolCursor.ExtendLine];
					} else return cursors[ToolCursor.NotAllowed];

				default: throw new NShapeUnsupportedValueException(CurrentAction);
			}
		}


		private bool IsExtendLineFeasible(Action action, Shape shape, ControlPointId pointId) {
			if (action != Action.None && action != Action.ExtendLine) return false;
			if ((shape as ILinearShape) == null) return false;
			if (pointId == ControlPointId.None) return false;
			if (shape.Type != Template.Shape.Type) return false;
			if (((ILinearShape)shape).VertexCount >= ((ILinearShape)shape).MaxVertexCount) return false;
			if (!shape.HasControlPointCapability(pointId, ControlPointCapabilities.Glue)) return false;
			if (shape.IsConnected(pointId, null) != ControlPointId.None) return false;
			return true;
		}


		private bool IsConnectingFeasible(Action action, Shape shape, ControlPointId pointId) {
			if (shape != null && pointId != ControlPointId.None
				&& Template.Shape.HasControlPointCapability(ControlPointId.LastVertex, ControlPointCapabilities.Glue)) {
				if (shape.HasControlPointCapability(ControlPointId.Reference, ControlPointCapabilities.Connect))
					return true;
				else if (!shape.HasControlPointCapability(pointId, ControlPointCapabilities.Glue)
					&& shape.HasControlPointCapability(pointId, ControlPointCapabilities.Connect))
					return true;
			}
			return false;
		}


		private enum Action { None, ExtendLine, CreateLine }


		private enum ToolCursor {
			Default,
			NotAllowed,
			ExtendLine,
			Pen,
			Connect,
			Disconnect
		}


		#region Fields

		// Definition of the tool
		private static Dictionary<ToolCursor, int> cursors;

		// Tool's state definition
		// stores the last inserted Point (and its coordinates), which will become the EndPoint when the CurrentTool is cancelled
		private ControlPointId pointAtCursor;
		private ControlPointId lastInsertedPointId;
		// Stores the currently modified ILinearShape. 
		// This could be a new shape created from the template but also an existing line that is extended with new points.
		private ILinearShape modifiedLinearShape = null;

		#endregion
	}


	/// <summary>
	/// Lets the user place a new shape on the diagram.
	/// </summary>
	public class PlanarShapeCreationTool : TemplateTool {

		/// <ToBeCompleted></ToBeCompleted>
		public PlanarShapeCreationTool(Template template)
			: base(template) {
			Construct(template);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public PlanarShapeCreationTool(Template template, string category)
			: base(template, category) {
			Construct(template);
		}


		/// <override></override>
		public override bool ProcessMouseEvent(IDiagramPresenter diagramPresenter, MouseEventArgsDg e) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			bool result = false;
			
			MouseState newMouseState = MouseState.Empty;
			newMouseState.Buttons = e.Buttons;
			newMouseState.Modifiers = e.Modifiers;

			// Return if action is not allowed
			if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Insert, Template.Shape))
				return result;

			diagramPresenter.ControlToDiagram(e.Position, out newMouseState.Position);

			diagramPresenter.SuspendUpdate();
			try {
				switch (e.EventType) {
					case MouseEventType.MouseMove:
						if (newMouseState.Position != CurrentMouseState.Position) {
							// If no Preview exists, create a new one by starting a new ToolAction
							if (!IsToolActionPending)
								StartToolAction(diagramPresenter, (int)Action.Create, newMouseState, false);

							Invalidate(ActionDiagramPresenter);
							// Move preview shape to Mouse Position
							PreviewShape.MoveTo(newMouseState.X, newMouseState.Y);
							// Snap to grid
							if (diagramPresenter.SnapToGrid) {
								int snapDeltaX = 0, snapDeltaY = 0;
								FindNearestSnapPoint(diagramPresenter, PreviewShape, 0, 0, out snapDeltaX, out snapDeltaY);
								PreviewShape.MoveTo(newMouseState.X + snapDeltaX, newMouseState.Y + snapDeltaY);
							}
							Invalidate(ActionDiagramPresenter);
							result = true;
						}
						break;

					case MouseEventType.MouseUp:
						if (IsToolActionPending && newMouseState.IsButtonDown(MouseButtonsDg.Left)) {
							try {
								// Left mouse button was pressed: Create shape
								executing = true;
								Invalidate(ActionDiagramPresenter);
								if (ActionDiagramPresenter.Diagram != null) {
									int x = PreviewShape.X;
									int y = PreviewShape.Y;

									Shape newShape = Template.CreateShape();
									newShape.ZOrder = ActionDiagramPresenter.Project.Repository.ObtainNewTopZOrder(ActionDiagramPresenter.Diagram);
									newShape.MoveTo(x, y);

									ActionDiagramPresenter.InsertShape(newShape);
									result = true;
								}
							} finally {
								executing = false;
							}
							EndToolAction();
							OnToolExecuted(ExecutedEventArgs);
						} else if (newMouseState.IsButtonDown(MouseButtonsDg.Right)) {
							// Right mouse button was pressed: Cancel Tool
							Cancel();
							result = true;
						}
						break;

					case MouseEventType.MouseDown:
						// nothing to to yet
						// ToDo 3: Implement dragging a frame with the mouse and fit the shape into that frame when releasing the button
						break;

					default: throw new NShapeUnsupportedValueException(e.EventType);
				}
			} finally { diagramPresenter.ResumeUpdate(); }
			base.ProcessMouseEvent(diagramPresenter, e);
			return result;
		}


		/// <override></override>
		public override bool ProcessKeyEvent(IDiagramPresenter diagramPresenter, KeyEventArgsDg e) {
			return base.ProcessKeyEvent(diagramPresenter, e);
		}


		/// <override></override>
		public override void EnterDisplay(IDiagramPresenter diagramPresenter) {
			if (!CurrentMouseState.IsEmpty && !executing) {
				if (diagramPresenter.Project.SecurityManager.IsGranted(Permission.Insert, Template.Shape))
					StartToolAction(diagramPresenter, (int)Action.Create, CurrentMouseState, false);
			}
		}


		/// <override></override>
		public override void LeaveDisplay(IDiagramPresenter diagramPresenter) {
			// Do not end tool action while inserting a shape (e.g. when showing a dialog 
			// on the DiagramPresenter's "ShapeInserted" event
			if (!executing && IsToolActionPending) 
				EndToolAction();
		}


		/// <override></override>
		public override void Draw(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (drawPreview) {
				//if (DisplayContainsMousePos(ActionDisplay, CurrentMouseState.Position)) {
				diagramPresenter.DrawShape(PreviewShape);
				if (ActionDiagramPresenter.SnapToGrid)
					diagramPresenter.DrawSnapIndicators(PreviewShape);
				//}
			}
		}


		/// <override></override>
		public override void Invalidate(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (PreviewShape != null && diagramPresenter.SnapToGrid)
				diagramPresenter.InvalidateSnapIndicators(PreviewShape);
		}


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			yield break;
		}


		/// <override></override>
		protected override void CancelCore() {
			if (PreviewShape != null)
				ClearPreview();
		}


		/// <override></override>
		protected override void StartToolAction(IDiagramPresenter diagramPresenter, int action, MouseState mouseState, bool wantAutoScroll) {
			base.StartToolAction(diagramPresenter, action, mouseState, wantAutoScroll);
			CreatePreview(ActionDiagramPresenter);
			PreviewShape.DisplayService = diagramPresenter.DisplayService;
			PreviewShape.MoveTo(mouseState.X, mouseState.Y);
			drawPreview = true;
			diagramPresenter.SetCursor(CurrentCursorId);
		}


		/// <override></override>
		protected override void EndToolAction() {
			base.EndToolAction();
			drawPreview = false;
			ClearPreview();
		}


		static PlanarShapeCreationTool() {
			crossCursorId = CursorProvider.RegisterCursor(Properties.Resources.CrossCursor);
		}


		private void Construct(Template template) {
			if (!(template.Shape is IPlanarShape))
				throw new NShapeException("The template's shape does not implement {0}.", typeof(IPlanarShape).Name);
			drawPreview = false;
		}


		private int CurrentCursorId {
			get { return drawPreview ? crossCursorId : CursorProvider.DefaultCursorID; }
		}


		private enum Action { None, Create}


		#region Fields

		// Definition of the tool
		private static int crossCursorId;
		private bool drawPreview;
		private bool executing = false;

		#endregion
	}

}