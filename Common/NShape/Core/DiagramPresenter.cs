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
using System.Drawing;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.Controllers
{
	/// <summary>
	/// Defines the visual appearance of control point grips.
	/// </summary>
	public enum ControlPointShape
	{
		/// <summary>A rhombical grip.</summary>
		Diamond = 1,

		/// <summary>A hexangular grip.</summary>
		Hexagon = 2,

		/// <summary>A circular grip.</summary>
		Circle = 3,

		/// <summary>A circular arrow grip.</summary>
		RotateArrow = 4,

		/// <summary>A quadratic grip.</summary>
		Square = 5
	}

	#region EventArgs

	/// <ToBeCompleted></ToBeCompleted>
	public class DiagramPresenterShapeClickEventArgs : EventArgs
	{
		/// <summary>
		/// Initializing a new instance of <see cref="T:Dataweb.NShape.Controllers.DiagramPresenterShapeClickEventArgs" />
		/// </summary>
		public DiagramPresenterShapeClickEventArgs(Shape shape, MouseEventArgsDg mouseEventArgs)
			: this()
		{
			this.shape = shape;
			this.mouseEventArgs = mouseEventArgs;
		}

		/// <summary>
		/// Initializing a new instance of <see cref="T:Dataweb.NShape.Controllers.DiagramPresenterShapeClickEventArgs" />
		/// </summary>
		protected internal DiagramPresenterShapeClickEventArgs()
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Shape Shape
		{
			get { return shape; }
			protected internal set { shape = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MouseEventArgsDg Mouse
		{
			get { return mouseEventArgs; }
			protected internal set { mouseEventArgs = value; }
		}


		private Shape shape;
		private MouseEventArgsDg mouseEventArgs;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DiagramPresenterShapesEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.DiagramPresenterShapeEventArgs" />.
		/// </summary>
		public DiagramPresenterShapesEventArgs(Shape shape)
		{
			this.shapes = new ReadOnlyList<Shape>(1);
			this.shapes.Add(shape);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.DiagramPresenterShapeEventArgs" />.
		/// </summary>
		public DiagramPresenterShapesEventArgs(IEnumerable<Shape> shapes)
		{
			this.shapes = new ReadOnlyList<Shape>(shapes);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Dataweb.NShape.Advanced.IReadOnlyCollection<Shape> Shapes
		{
			get { return shapes; }
			set
			{
				shapes.Clear();
				shapes.AddRange(value);
			}
		}


		private ReadOnlyList<Shape> shapes;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class UserMessageEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.UserMessageEventArgs" />.
		/// </summary>
		public UserMessageEventArgs(string messageText)
		{
			this.messageText = messageText;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string MessageText
		{
			get { return messageText; }
			protected internal set { messageText = value; }
		}


		private string messageText;
	}

	#endregion

	// IDiagramPresenter has to be a descendant of IDisplayService because Tools set their IDiagramPresenter as
	// the preview shape's DisplayService
	/// <summary>
	/// Defines the interface between the tool and the diagram presenter.
	/// </summary>
	/// <status>reviewed</status>
	public interface IDiagramPresenter
	{
		#region Events

		/// <summary>Raised when the selected shapes changed.</summary>
		event EventHandler ShapesSelected;

		/// <summary>Raised when a shape was clicked.</summary>
		event EventHandler<DiagramPresenterShapeClickEventArgs> ShapeClick;

		/// <summary>Raised when a shape was double clicked.</summary>
		event EventHandler<DiagramPresenterShapeClickEventArgs> ShapeDoubleClick;

		/// <summary>Raised when a shape was inserted into the diagram of this diagram presenter.</summary>
		event EventHandler<DiagramPresenterShapesEventArgs> ShapesInserted;

		/// <summary>Raised when a shape was removed from the diagram of this diagram presenter.</summary>
		event EventHandler<DiagramPresenterShapesEventArgs> ShapesRemoved;

		/// <summary>Raised when the diagram is going to be changed.</summary>
		event EventHandler DiagramChanging;

		/// <summary>Raised when the diagram was changed.</summary>
		event EventHandler DiagramChanged;

		/// <summary>Raised when the visibility of layers changed.</summary>
		event EventHandler<LayersEventArgs> LayerVisibilityChanged;

		/// <summary>Raised when the active layers changed.</summary>
		event EventHandler<LayersEventArgs> ActiveLayersChanged;

		/// <summary>Raised when the zoom level changed.</summary>
		event EventHandler ZoomChanged;

		/// <summary>
		/// Raised when a non-visual component wants to display a message for the user. 
		/// If this event is not handled by the application, a <see cref="T:System.ComponentModel.WarningException" /> will be thrown when opening or creating a project.
		/// </summary>
		event EventHandler<UserMessageEventArgs> UserMessage;

		#endregion

		#region Properties

		/// <summary>
		/// The DiagramSetController responsible for managing the diagrams in the repositoriy.
		/// </summary>
		DiagramSetController DiagramSetController { get; set; }

		/// <summary>
		/// The DiagramSetController's project.
		/// </summary>
		Project Project { get; }

		/// <summary>
		/// The currently displayed diagram.
		/// </summary>
		Diagram Diagram { get; }

		/// <summary>
		/// Gets the display service of this diagram presenter.
		/// </summary>
		IDisplayService DisplayService { get; }

		/// <summary>
		/// Collection of currently selected shapes.
		/// </summary>
		[Browsable(false)]
		IShapeCollection SelectedShapes { get; }

		/// <summary>
		/// Gets all active layers.
		/// </summary>
		LayerIds ActiveLayers { get; }

		/// <summary>
		/// Gets all hidden layers.
		/// </summary>
		LayerIds HiddenLayers { get; }

		#endregion

		#region Properties: Visuals

		/// <summary>
		/// Font used for hints in the diagram presentation (e.g. current angle during rotation)
		/// </summary>
		Font Font { get; }


		/// <summary>
		/// True, if high quality rendering is switched on.
		/// </summary>
		bool HighQualityRendering { get; }

		#endregion

		#region Properties: Behavior

		/// <summary>
		/// Specifies the distance for snapping shapes and control points to grid lines.
		/// </summary>
		[Category("Behavior")]
		int SnapDistance { get; set; }

		/// <summary>
		/// Enables or disables snapping of shapes and control points to grid lines.
		/// </summary>
		[Category("Behavior")]
		bool SnapToGrid { get; set; }

		/// <summary>
		/// The radius of a control point grip from the center to the outer handle bound.
		/// </summary>
		[Category("Appearance")]
		int GridSize { get; set; }

		/// <summary>
		/// Specifies whether grid lines should be visible.
		/// </summary>
		[Category("Appearance")]
		bool ShowGrid { get; set; }

		/// <summary>
		/// Specifies the shape of grips used for resizing shapes.
		/// </summary>
		[Category("Appearance")]
		ControlPointShape ResizeGripShape { get; set; }

		/// <summary>
		/// Specifies the shape of connection points provided by a shape.
		/// </summary>
		[Category("Appearance")]
		ControlPointShape ConnectionPointShape { get; set; }

		/// <summary>
		/// The size of a ControlPoint handle from the center to the outer handle bound.
		/// </summary>
		[Category("Appearance")]
		int GripSize { get; set; }

		/// <summary>
		/// Provides the size (radius) of ControlPoint grips according to the current zoom in diagram coordinates.
		/// </summary>
		[Browsable(false)]
		int ZoomedGripSize { get; }

		/// <summary>
		/// Zoom in percentage.
		/// </summary>
		int ZoomLevel { get; set; }

		/// <summary>
		/// Specifies whether the component has captured the mouse.
		/// </summary>
		bool Capture { get; set; }

		/// <summary>
		/// Specifies the minimum distance of the mouse cursor from the shape's rotate point while rotating.
		/// </summary>
		[Browsable(false)]
		int MinRotateRange { get; }

		#endregion

		#region Methods: (Un)Selecting shapes

		/// <summary>
		/// Clears the current selection.
		/// </summary>
		void UnselectAll();

		/// <summary>
		/// Removes the given <see cref="T:Dataweb.NShape.Advanced.Shape" /> from the current selection.
		/// </summary>
		void UnselectShape(Shape shape);

		/// <summary>
		/// Selects the given <see cref="T:Dataweb.NShape.Advanced.Shape" />. Current selection will be cleared.
		/// </summary>
		void SelectShape(Shape shape);

		/// <summary>
		/// Selects the given <see cref="T:Dataweb.NShape.Advanced.Shape" />.
		/// </summary>
		/// <param name="shape"><see cref="T:Dataweb.NShape.Advanced.Shape" /> to be selected.</param>
		/// <param name="addToSelection">If true, the given shape will be added to the current selection, otherwise the current selection will be cleared before selecting this shape.</param>
		void SelectShape(Shape shape, bool addToSelection);

		/// <summary>
		/// Selects the given <see cref="T:Dataweb.NShape.Advanced.Shape" />.
		/// </summary>
		/// <param name="shapes"><see cref="T:Dataweb.NShape.Advanced.Shape" /> to be selected.</param>
		/// <param name="addToSelection">If true, the given shape will be added to the current selection, otherwise the current selection will be cleared before selecting this shape.</param>
		void SelectShapes(IEnumerable<Shape> shapes, bool addToSelection);

		/// <summary>
		/// Selects all <see cref="T:Dataweb.NShape.Advanced.Shape" /> within the given area.
		/// </summary>
		/// <param name="area">All shapes in the given rectangle will be selected.</param>
		/// <param name="addToSelection">If true, the given shape will be added to the current selection, otherwise the current selection will be cleared before selecting this shape.</param>
		void SelectShapes(Rectangle area, bool addToSelection);

		/// <summary>
		/// Selectes all <see cref="T:Dataweb.NShape.Advanced.Shape" /> of the <see cref="T:Dataweb.NShape.Diagram" />
		/// </summary>
		void SelectAll();

		#endregion

		#region Methods: Coordinate transformation routines

		/// <summary>
		/// Calculate contol coordinates from diagram coordinates
		/// </summary>
		/// <param name="dX">X value in diagram coordinates</param>
		/// <param name="dY">Y value in diagram coordinates</param>
		/// <param name="cX">X value in control coordinates</param>
		/// <param name="cY">Y value in control coordinates</param>
		void DiagramToControl(int dX, int dY, out int cX, out int cY);

		/// <summary>
		/// Calculate contol coordinates from diagram coordinates
		/// </summary>
		/// <param name="dPt">Point in diagram coordinates</param>
		/// <param name="cPt">Point in control Coordinates</param>
		void DiagramToControl(Point dPt, out Point cPt);

		/// <summary>
		/// Calculate contol coordinates from diagram coordinates
		/// </summary>
		/// <param name="dRect">Rectangle in diagram coordinates</param>
		/// <param name="cRect">Rectangle in control coordinates</param>
		void DiagramToControl(Rectangle dRect, out Rectangle cRect);

		/// <summary>
		/// Calculate contol coordinates from diagram coordinates
		/// </summary>
		/// <param name="dDistance">Distance in diagram coordinates</param>
		/// <param name="cDistance">Distance in control coordinates</param>
		void DiagramToControl(int dDistance, out int cDistance);

		/// <summary>
		/// Calculate contol coordinates from diagram coordinates
		/// </summary>
		/// <param name="dSize">Size in diagram coordinates</param>
		/// <param name="cSize">Size in control coordinates</param>
		void DiagramToControl(Size dSize, out Size cSize);

		/// <summary>
		/// Calculate diagram coordinates from control coordinates
		/// </summary>
		/// <param name="cX">X value in control coordinates</param>
		/// <param name="cY">Y value in control coordinates</param>
		/// <param name="dX">X value in diagram coordinates</param>
		/// <param name="dY">Y value in diagram coordinates</param>
		void ControlToDiagram(int cX, int cY, out int dX, out int dY);

		/// <summary>
		/// Calculate diagram coordinates from control coordinates
		/// </summary>
		/// <param name="cPt">Point in control coordinates</param>
		/// <param name="dPt">Point in diagram coordinates</param>
		void ControlToDiagram(Point cPt, out Point dPt);

		/// <summary>
		/// Calculate diagram coordinates from control coordinates
		/// </summary>
		/// <param name="cRect">Rectangle in control coordinates</param>
		/// <param name="dRect">Rectangle in diagram coordinates</param>
		void ControlToDiagram(Rectangle cRect, out Rectangle dRect);

		/// <summary>
		/// Calculate diagram coordinates from control coordinates
		/// </summary>
		/// <param name="cSize">Size in control coordinates</param>
		/// <param name="dSize">Size in diagram coordinates</param>
		void ControlToDiagram(Size cSize, out Size dSize);

		/// <summary>
		/// Calculate diagram coordinates from control coordinates
		/// </summary>
		/// <param name="cDistance">Distance in control coordinates</param>
		/// <param name="dDistance">Distance in diagram coordinates</param>
		void ControlToDiagram(int cDistance, out int dDistance);

		/// <summary>
		/// Calculate diagram coorinates from screen coordinates
		/// </summary>
		/// <param name="sPt">Point in Screen coordinates</param>
		/// <param name="dPt">Point in diagram coordinates</param>
		void ScreenToDiagram(Point sPt, out Point dPt);

		/// <summary>
		/// Calculate diagram coorinates from screen coordinates
		/// </summary>
		/// <param name="sRect">Rectangle in screen coordinates</param>
		/// <param name="dRect">Rectangle in diagram coordinates</param>
		void ScreenToDiagram(Rectangle sRect, out Rectangle dRect);

		#endregion

		#region Methods: Drawing and Invalidating

		/// <summary>
		/// Draws rotation preview for the rotated shape.
		/// </summary>
		/// <param name="center">The rotation center.</param>
		/// <param name="mousePos">The current position of the mouse.</param>
		/// <param name="cursorId">The id of the registered cursor.</param>
		/// <param name="startAngle">Initial rotation angle.</param>
		/// <param name="sweepAngle">Rotation angle</param>
		void DrawAnglePreview(Point center, Point mousePos, int cursorId, int startAngle, int sweepAngle);

		/// <summary>
		/// Draws the text bounds of the given caption.
		/// </summary>
		void DrawCaptionBounds(IndicatorDrawMode drawMode, ICaptionedShape shape, int captionIndex);

		/// <summary>
		/// Draws a grip indicating a connection point at the position of the given control point.
		/// </summary>
		void DrawConnectionPoint(IndicatorDrawMode drawMode, Shape shape, ControlPointId pointId);

		/// <summary>
		/// Draws a line.
		/// </summary>
		void DrawLine(Point a, Point b);

		/// <summary>
		/// Draws a grip indicating a control point used for resizing shapes at the position of the given control point.
		/// </summary>
		void DrawResizeGrip(IndicatorDrawMode drawMode, Shape shape, ControlPointId pointId);

		/// <summary>
		/// Draws a grip indicating a control point used for rotating shapes at the position of the given control point.
		/// </summary>
		void DrawRotateGrip(IndicatorDrawMode drawMode, Shape shape, ControlPointId pointId);

		/// <summary>
		/// Draws a selection frame.
		/// </summary>
		/// <param name="frameRect">Bounds of the selection frame in diagram coordinates.</param>
		void DrawSelectionFrame(Rectangle frameRect);

		/// <summary>
		/// Draws the given shape.
		/// </summary>
		void DrawShape(Shape shape);

		/// <summary>
		/// Draws the outline of the given shape.
		/// </summary>
		void DrawShapeOutline(IndicatorDrawMode drawMode, Shape shape);

		/// <summary>
		/// Draws all shapes of the given collection.
		/// </summary>
		void DrawShapes(IEnumerable<Shape> shapes);

		/// <summary>
		/// Draw lines and grips indicating that a dragged control point or shape has been snapped to a nearby grid line.
		/// </summary>
		void DrawSnapIndicators(Shape shape);

		/// <summary>
		/// Resets transformation of the graphics context.
		/// </summary>
		void ResetTransformation();

		/// <summary>
		/// Transforms the graphics context.
		/// </summary>
		void RestoreTransformation();

		/// <summary>
		/// Invalidate the given area of the diagram in order to cause redrawing.
		/// </summary>
		void InvalidateDiagram(Rectangle rect);

		/// <summary>
		/// Invalidate the given area of the diagram in order to cause redrawing.
		/// </summary>
		void InvalidateDiagram(int left, int top, int width, int height);

		/// <summary>
		/// Invalidates the bounding rectangle around the shape and all its (suitable) control points.
		/// </summary>
		void InvalidateGrips(Shape shape, ControlPointCapabilities controlPointCapability);

		/// <summary>
		/// Invalidates the bounding rectangle around all shapes and all their (suitable) control points.
		/// </summary>
		void InvalidateGrips(IEnumerable<Shape> shapes, ControlPointCapabilities controlPointCapability);

		/// <summary>
		/// Invalidate snap indicators drawn for the given shape.
		/// </summary>
		void InvalidateSnapIndicators(Shape preview);

		/// <summary>
		/// Collects areas of all calls of "InvalidateDiagram" until ResumeUpdate is called.
		/// </summary>
		void SuspendUpdate();

		/// <summary>
		/// Ends collecting calls of "InvalidateDiagram" and executes the collected calls at once.
		/// </summary>
		void ResumeUpdate();

		/// <summary>
		/// Updates (redraws) the invalidated areas immediately.
		/// </summary>
		void Update();

		#endregion

		#region Methods

		/// <summary>
		/// Inserts the given shape in the displayed diagram.
		/// </summary>
		void InsertShape(Shape shape);

		/// <summary>
		/// Inserts the given shapes in the displayed diagram.
		/// </summary>
		void InsertShapes(IEnumerable<Shape> shapes);

		/// <summary>
		/// Deletes the given shape in the displayed diagram.
		/// </summary>
		void DeleteShape(Shape shape, bool withModelObjects);

		/// <summary>
		/// Deletes the given shapes in the displayed diagram.
		/// </summary>
		void DeleteShapes(IEnumerable<Shape> shapes, bool withModelObjects);

		/// <summary>
		/// Cuts the selected shapes.
		/// </summary>
		void Cut();

		/// <summary>
		/// Copies the selected shapes.
		/// </summary>
		void Copy();

		/// <summary>
		/// Pastes the previously copied or cut shapes into the displayed diagram.
		/// </summary>
		void Paste();

		/// <summary>
		/// Opens a text editor for editing a caption of the given <see cref="T:Dataweb.NShape.Advanced.ICaptionedShape" /> instance located at the given position.
		/// </summary>
		void OpenCaptionEditor(ICaptionedShape shape, int x, int y);

		/// <summary>
		/// Opens a text editor for editing the indicated caption of the given <see cref="T:Dataweb.NShape.Advanced.ICaptionedShape" /> instance.
		/// </summary>
		void OpenCaptionEditor(ICaptionedShape shape, int labelIndex);

		/// <summary>
		/// Opens a text editor for editing the indicated caption of the given <see cref="T:Dataweb.NShape.Advanced.ICaptionedShape" /> instance.
		/// </summary>
		void OpenCaptionEditor(ICaptionedShape shape, int labelIndex, string newText);

		///// <summary>
		///// Notifies the DiagramPresenter that shapes have been inserted.
		///// </summary>
		//void NotifyShapesInserted(IEnumerable<Shape> shapes);

		///// <summary>
		///// Notifies the DiagramPresenter that shapes have been removed.
		///// </summary>
		//void NotifyShapesRemoved(IEnumerable<Shape> shapes);

		/// <summary>
		/// Ensures that the given point is visible. 
		/// If the given point is outside the displayed area, the diagram will be scrolled.
		/// </summary>
		void EnsureVisible(int x, int y);

		/// <summary>
		/// Ensures that the given shape is completely visible. 
		/// If the given shape is outside the displayed area, the diagram will be scrolled and/or zoomed.
		/// </summary>
		void EnsureVisible(Shape shape);

		/// <summary>
		/// Ensures that the given area is visible. 
		/// If the given area is outside the displayed area, the diagram will be scrolled and/or zoomed.
		/// </summary>
		void EnsureVisible(Rectangle area);

		/// <summary>
		/// Sets a previously registered cursor.
		/// </summary>
		/// <param name="cursorId">The id of the registered cursor to set.</param>
		void SetCursor(int cursorId);

		/// <summary>
		/// Sets the visibility of the given layers.
		/// </summary>
		void SetLayerVisibility(LayerIds layerIds, bool visible);

		/// <summary>
		/// Sets the given layers as the active layers.
		/// </summary>
		void SetLayerActive(LayerIds layerIds, bool active);

		/// <summary>
		/// Checks whether any of the given layers is visible.
		/// </summary>
		bool IsLayerVisible(LayerIds layerId);

		/// <summary>
		/// Checks whether all of the given layers are active.
		/// </summary>
		bool IsLayerActive(LayerIds layerId);

		#endregion
	}


	///// <ToBeCompleted></ToBeCompleted>
	//public interface IDiagramPresenterNotificaton {

	//   /// <ToBeCompleted></ToBeCompleted>
	//   void NotifyUserMessage(string messageText);

	//   /// <ToBeCompleted></ToBeCompleted>
	//   void NotifyShapesInserted(IEnumerable<Shape> shapes);

	//   /// <ToBeCompleted></ToBeCompleted>
	//   void NotifyShapesRemoved(IEnumerable<Shape> shapes);

	//}
}