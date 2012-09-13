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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

using Dataweb.NShape.Advanced;
using Dataweb.Utilities;


namespace Dataweb.NShape {

	/// <summary>
	/// Interface for all graphical objects.
	/// </summary>
	/// <remarks>RequiredPermissions set</remarks>
	/// <status>reviewed</status>
	[TypeDescriptionProvider(typeof(TypeDescriptionProviderDg))]
	public abstract class Shape : IEntity, ISecurityDomainObject, IDisposable, IEquatable<Shape> {

		/// <ToBeCompleted></ToBeCompleted>
		~Shape() {
			Dispose();
		}


		#region IDisposable Members

		/// <override></override>
		public abstract void Dispose();

		#endregion


		#region IEquatable<T> Members

		/// <override></override>
		public bool Equals(Shape other) {
			return other == this;
		}
		
		#endregion


		#region Standard Methods

		/// <override></override>
		public override string ToString() {
			return Type.FullName;
		}


		/// <summary>
		/// Creates a clone, which owns clones of all composite objects in the shape.
		/// </summary>
		public abstract Shape Clone();

		/// <summary>
		/// Copies as many properties as possible from the source shape.
		/// </summary>
		public abstract void CopyFrom(Shape source);

		/// <summary>
		/// Indicates the type of the shape. Is always defined.
		/// </summary>
		[Category("General")]
		[Description("The .NET data type of the shape.")]
		[RequiredPermission(Permission.Data)]
		public abstract ShapeType Type { get; }

		/// <summary>
		/// Indicates the model object displayed by this shape. May be null.
		/// </summary>
		[Browsable(false)]
		[RequiredPermission(Permission.Data)]
		public abstract IModelObject ModelObject { get; set; }

		/// <summary>
		/// Indicates the template for this shape. Is null, if the shape has no template.
		/// </summary>
		[Category("General")]
		[RequiredPermission(Permission.Data)]
		public abstract Template Template { get; }

		/// <summary>
		/// Specifies a user-defined transient general purpose property.
		/// </summary>
		[Browsable(false)]
		public abstract object Tag { get; set; }

		/// <summary>
		/// Indicates the diagram this shape is part of.
		/// </summary>
		[Browsable(false)]
		public abstract Diagram Diagram { get; internal set; }

		/// <summary>
		/// Indicates the parent shape if this shape is part of an aggregation.
		/// </summary>
		[Browsable(false)]
		public abstract Shape Parent { get; set; }

		/// <summary>
		/// Provides access to the child shapes.
		/// </summary>
		[Browsable(false)]
		public abstract IShapeCollection Children { get; }

		/// <summary>
		/// Called upon the active shape of the connection, e.g. by a tool or a command.
		/// If ownPointId is equal to ControlPointId.Reference, the global connection is meant.
		/// </summary>
		public abstract void Connect(ControlPointId ownPointId, Shape otherShape, ControlPointId otherPointId);

		/// <summary>
		/// Disconnects a shape's glue point from its connected shape.
		/// </summary>
		public abstract void Disconnect(ControlPointId ownPointId);

		/// <summary>
		/// Retrieve information of the connection status of this shape.
		/// </summary>
		/// <param name="otherShape">Target shape of the connections to return. 
		/// If null/Nothing, connections to all connected shapes are returned.</param>
		/// <param name="ownPointId">If a valid point id is given, all connections of this connection point are returned. 
		/// If the constant ControlPointId.Any is given, connections of all connection points are returned.</param>
		public abstract IEnumerable<ShapeConnectionInfo> GetConnectionInfos(ControlPointId ownPointId, Shape otherShape);

		/// <summary>
		/// Returns the connection info for given glue point or ShapeConnectionInfo.Empty if the given glue point is not connected.
		/// </summary>
		public abstract ShapeConnectionInfo GetConnectionInfo(ControlPointId gluePointId, Shape otherShape);

		/// <summary>
		/// Tests, whether the shape is connected to a given other shape.
		/// </summary>
		/// <param name="ownPointId">
		/// Id of shape's own connection point, which is to be tested. 
		/// ControlPointId.Any, if any connection point is taken into account.
		/// </param>
		/// <param name="otherShape">Other shape. Null if any other shape is taken into account.</param>
		/// <returns>The ControlPointId of the connected shape.</returns>
		public abstract ControlPointId IsConnected(ControlPointId ownPointId, Shape otherShape);

		/// <summary>
		/// Determines whether the given rectangular region intersects with the shape.
		/// </summary>
		public abstract bool IntersectsWith(int x, int y, int width, int height);

		/// <summary>
		/// Calculates all intersection points of the shape's outline with the given line segment. Does not return any point if there is no intersection.
		/// </summary>
		/// <param name="x1">X coordinate of the line segment's start point.</param>
		/// <param name="y1">Y coordinate of the line segment's start point.</param>
		/// <param name="x2">X coordinate of the line segment's end point.</param>
		/// <param name="y2">Y coordinate of the line segment's end point.</param>
		public abstract IEnumerable<Point> IntersectOutlineWithLineSegment(int x1, int y1, int x2, int y2);

		/// <summary>
		/// Determines whether the given point is inside the shape.
		/// </summary>
		public abstract bool ContainsPoint(int x, int y);

		/// <summary>
		/// Determines if the given point is inside the shape or near a control point having one of the 
		/// given control point capabilities.
		/// </summary>
		/// <returns>Control point id of hit control point, ControlPointId.Reference if shape is hit, 
		/// ControlPointId.None if nothing is hit.</returns>
		public abstract ControlPointId HitTest(int x, int y, ControlPointCapabilities controlPointCapability, int range);

		/// <summary>
		/// Calculates the axis-aligned bounding rectangle of the shape.
		/// </summary>
		/// <param name="tight">
		/// If true, the the minimum bounding rectangle of the shape will be calculated.
		/// If false, the bounding rectangle of the shape including all its control points will be calculated.
		/// </param>
		/// <returns>The axis-aligned bounding rectangle.</returns>
		public abstract Rectangle GetBoundingRectangle(bool tight);

		/// <summary>
		/// Calculates the diagram cells occupied by this shape.
		/// </summary>
		/// <param name="cellSize">Size of cell in vertical and horizontal direction.</param>
		/// <returns>Cell indices starting with (0, 0) from the upper left corner.</returns>
		/// <remarks>Method is called by large shape collections to create a kind of spatial index for the 
		/// shapes for faster searching. Implementation must not rely on display service. Duplicate cells
		/// are allowed but should be minimized for performance reasons.</remarks>
		protected internal abstract IEnumerable<Point> CalculateCells(int cellSize);

		/// <summary>
		/// Gets or sets the x-coordinate of the shape's location.
		/// </summary>
		[Category("Layout")]
		[Description("Horizontal position of the shape's reference point.")]
		[RequiredPermission(Permission.Layout)]
		public abstract int X { get; set; }

		/// <summary>
		/// Gets or sets the y-coordinate of the shape's location.
		/// </summary>
		[Category("Layout")]
		[Description("Vertical position of the shape's reference point.")]
		[RequiredPermission(Permission.Layout)]
		public abstract int Y { get; set; }

		/// <summary>
		/// Fits the shape in the given rectangle.
		/// </summary>
		public abstract void Fit(int x, int y, int width, int height);

		/// <summary>
		/// Moves the shape to the given coordinates, if possible.
		/// </summary>
		/// <param name="toX"></param>
		/// <param name="toY"></param>
		/// <returns>True, if move was possible, else false.</returns>
		public bool MoveTo(int toX, int toY) {
			return MoveBy(toX - X, toY - Y);
		}

		/// <summary>
		/// Moves the shape by the given distances, if possible.
		/// </summary>
		/// <returns>True, if move was possible, else false.</returns>
		public abstract bool MoveBy(int deltaX, int deltaY);

		/// <summary>
		/// Moves the given control point to the indicated coordinates if possible.
		/// </summary>
		/// <param name="pointId"></param>
		/// <param name="toX"></param>
		/// <param name="toY"></param>
		/// <param name="modifiers"></param>
		/// <returns>True, if the control point could be moved, else false.</returns>
		public bool MoveControlPointTo(ControlPointId pointId, int toX, int toY, ResizeModifiers modifiers) {
			Point ptPos = GetControlPointPosition(pointId);
			return MoveControlPointBy(pointId, toX - ptPos.X, toY - ptPos.Y, modifiers);
		}

		/// <summary>
		/// Moves the given control point by the indicated distances if possible.
		/// </summary>
		/// <param name="pointId">Id of the point to move</param>
		/// <param name="deltaX">Distance on X axis</param>
		/// <param name="deltaY">Distance on Y axis</param>
		/// <param name="modifiers">Modifiers for the move action.</param>
		/// <returns>True, if the control point could be moved, else false.</returns>
		public abstract bool MoveControlPointBy(ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers);

		/// <summary>
		/// Rotates the shape.
		/// </summary>
		/// <param name="angle">Clockwise rotation angle in tenths of a degree.</param>
		/// <param name="x">X coordinate of rotation center.</param>
		/// <param name="y">Y coordinate of rotation center.</param>
		/// <returns></returns>
		public abstract bool Rotate(int angle, int x, int y);

		/// <summary>
		/// Retrieves the current control point position.
		/// </summary>
		/// <param name="controlPointId"></param>
		/// <returns></returns>
		public abstract Point GetControlPointPosition(ControlPointId controlPointId);

		/// <summary>
		/// Lists the control points of the shape.
		/// </summary>
		/// <param name="controlPointCapability">Filters for the indicated control point capabilities. ControlPointId.None for all controll points.</param>
		/// <returns></returns>
		public abstract IEnumerable<ControlPointId> GetControlPointIds(ControlPointCapabilities controlPointCapability);

		/// <summary>
		/// Finds the control point, with the least distance to the given coordinates.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="distance"></param>
		/// <param name="controlPointCapability"></param>
		public abstract ControlPointId FindNearestControlPoint(int x, int y, int distance,
			ControlPointCapabilities controlPointCapability);

		/// <summary>
		/// Tests, whether a control point has at least one of a set of given capabilities.
		/// </summary>
		/// <param name="controlPointId"></param>
		/// <param name="controlPointCapability"></param>
		public abstract bool HasControlPointCapability(ControlPointId controlPointId,
			ControlPointCapabilities controlPointCapability);

		/// <summary>
		/// Returns a ReplativePosition structure that representing the position of 
		/// the given point relative to the shape.
		/// </summary>
		/// <remarks> The RelativePosition structure can only be used for shapes of the 
		/// same type as the calculation method for the relative/absolute varies from 
		/// shape to shape.</remarks>
		public abstract RelativePosition CalculateRelativePosition(int x, int y);

		/// <summary>
		/// Calculates the absolute position of a RelativePosition.
		/// </summary>
		public abstract Point CalculateAbsolutePosition(RelativePosition relativePosition);

		/// <summary>
		/// Calculates the normal vector for the given coordinates. The coordinates have to be on the outline 
		/// of the shape, otherwise the returned vector is not defined.
		/// </summary>
		public abstract Point CalculateNormalVector(int x, int y);

		/// <summary>
		/// Calculates the intersection point of the shape's outline with the line between the given coordinates and the shape's balance point.
		/// </summary>
		public abstract Point CalculateConnectionFoot(int fromX, int fromY);

		/// <summary>
		/// Sets private preview styles for all used styles.
		/// </summary>
		/// <param name="styleSet"></param>
		public abstract void MakePreview(IStyleSet styleSet);

		/// <summary>Checks whether the given style is used by the shape.</summary>
		/// <returns>True if the given style is used by the shape.</returns>
		public abstract bool HasStyle(IStyle style);

		/// <summary>
		/// Indicates the name of the security domain this shape belongs to.
		/// </summary>
		[Category("General")]
		[Description("Modify the security domain of the shape.")]
		[RequiredPermission(Permission.Security)]
		public abstract char SecurityDomainName { get; set; }

		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		/// <param name="x">The X coordinate of the point clicked by the user.</param>
		/// <param name="y">The Y coordinate of the point clicked by the user.</param>
		/// <param name="range">The radius of the grips representing the shape's control points.</param>
		public abstract IEnumerable<MenuItemDef> GetMenuItemDefs(int x, int y, int range);

		#endregion


		#region Rendering Elements

		/// <summary>
		/// Defines the display service, which every displayed shape must have.
		/// </summary>
		[Browsable(false)]
		public abstract IDisplayService DisplayService { get; set; }

		/// <summary>
		/// Style used to draw the shape's outline.
		/// </summary>
		/// <remarks>Can be null, if no outline has to be drawn.</remarks>
		[Category("Appearance")]
		[Description("Defines the appearence of the shape's outline.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdLineStyle)]
		[RequiredPermission(Permission.Present)]
		public abstract ILineStyle LineStyle { get; set; }

		/// <summary>
		/// Draws the shape into the given graphics.
		/// </summary>
		/// <param name="graphics"></param>
		public abstract void Draw(Graphics graphics);

		/// <summary>
		/// Draws the outline of the shape into the given graphics.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="pen"></param>
		public abstract void DrawOutline(Graphics graphics, Pen pen);

		/// <summary>
		/// Draws a small preview of the shape into the image.
		/// </summary>
		/// <param name="image">The image the shape is drawn into</param>
		/// <param name="margin"></param>
		/// <param name="transparentColor"></param>
		public abstract void DrawThumbnail(Image image, int margin, Color transparentColor);

		/// <summary>
		/// Invalidates the region occupied by the shape in the display.
		/// </summary>
		public abstract void Invalidate();

		#endregion


		#region IEntity Members (protected implementation)

		/// <ToBeCompleted></ToBeCompleted>
		protected abstract object IdCore { get; }

		/// <ToBeCompleted></ToBeCompleted>
		protected abstract void AssignIdCore(object id);

		/// <ToBeCompleted></ToBeCompleted>
		protected abstract void DeleteCore(IRepositoryWriter writer, int version);

		/// <ToBeCompleted></ToBeCompleted>
		protected abstract void LoadFieldsCore(IRepositoryReader reader, int version);

		/// <ToBeCompleted></ToBeCompleted>
		protected abstract void LoadInnerObjectsCore(string propertyName, IRepositoryReader reader, int version);

		/// <ToBeCompleted></ToBeCompleted>
		protected abstract void SaveFieldsCore(IRepositoryWriter writer, int version);

		/// <ToBeCompleted></ToBeCompleted>
		protected abstract void SaveInnerObjectsCore(string propertyName, IRepositoryWriter writer, int version);

		#endregion


		#region IEntity Members (explicit implementation)

		object IEntity.Id { get { return IdCore; } }


		void IEntity.AssignId(object id) {
			AssignIdCore(id);
		}


		void IEntity.LoadFields(IRepositoryReader reader, int version) {
			LoadFieldsCore(reader, version);
		}


		void IEntity.LoadInnerObjects(string propertyName, IRepositoryReader reader, int version) {
			LoadInnerObjectsCore(propertyName, reader, version);
		}


		void IEntity.SaveFields(IRepositoryWriter writer, int version) {
			SaveFieldsCore(writer, version);
		}


		void IEntity.SaveInnerObjects(string propertyName, IRepositoryWriter writer, int version) {
			SaveInnerObjectsCore(propertyName, writer, version);
		}


		void IEntity.Delete(IRepositoryWriter writer, int version) {
			DeleteCore(writer, version);
		}

		#endregion


		/// <summary>
		/// Sets the state of a freshly created shape to the default values.
		/// </summary>
		/// <param name="styleSet"></param>
		/// <remarks>Only used by the ShapeType class.</remarks>
		protected internal abstract void InitializeToDefault(IStyleSet styleSet);


		#region Methods for exclusive Framework Use

		/// <summary>
		/// Informs the shape about a changed property in its model object.
		/// </summary>
		/// <param name="propertyId"></param>
		public abstract void NotifyModelChanged(int propertyId);

		/// <summary>
		/// Called upon the active shape of the connection, e.g. by a CurrentTool or a command.
		/// The active shape calculates the new position of it's GluePoint and moves it to the new position
		/// </summary>
		/// <param name="gluePointId">Id of the GluePoint connected to the moved ControlPoint</param>
		/// <param name="connectedShape">The passive shape of the connection</param>
		/// <param name="connectedPointId">Id of the ControlPoint that has moved</param>
		public abstract void FollowConnectionPointWithGluePoint(ControlPointId gluePointId, Shape connectedShape, ControlPointId connectedPointId);

		// For performance reasons the diagram stores the z-order and the layers
		// of its shapes within the shapes themselves.
		// These members must not be accessed but by the diagram.
		[Browsable(false)]
		[RequiredPermission(Permission.Layout)]
		public int ZOrder {
			get { return zOrder; }
			set { zOrder = value; }
		}

		/// <ToBeCompleted></ToBeCompleted>
		[Category("Layout")]
		[Description("Specifies the layers the shape is part of.")]
		[RequiredPermission(Permission.Layout)]
		[Editor("Dataweb.NShape.WinFormsUI.LayerUITypeEditor, Dataweb.NShape.WinFormsUI", typeof(System.Drawing.Design.UITypeEditor))]
		public LayerIds Layers {
			get { return layers; }
			internal set { layers = value; }
		}

		/// <summary>
		/// Notifies the shape that a style has changed. The shape decides what to do.
		/// </summary>
		/// <param name="style">The changed style</param>
		/// <returns>True if the given style is used by the shape</returns>
		protected internal abstract bool NotifyStyleChanged(IStyle style);

		/// <summary>
		/// Called upon the passive shape of the connection by the active shape. 
		/// If ownPointId is equal to ControlPointId.Reference, the global connection is meant.
		/// </summary>
		protected internal abstract void AttachGluePointToConnectionPoint(ControlPointId ownPointId, Shape otherShape, ControlPointId gluePointId);


		/// <summary>
		/// Called upon the passive shape of the connection by the active shape. 
		/// If ownPointId is equal to ControlPointId.Reference, the global connection is meant.
		/// </summary>
		protected internal abstract void DetachGluePointFromConnectionPoint(ControlPointId ownPointId, Shape otherShape, ControlPointId gluePointId);


		/// <summary>
		/// Specifies a general purpose property for internal use.
		/// </summary>
		[Browsable(false)]
		protected internal abstract object InternalTag { get; set; }


		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdLineStyle = 1;

		private int zOrder = 0;
		private LayerIds layers = LayerIds.None;

		#endregion

	}


	/// <summary>
	/// Represents a two-dimensional shape.
	/// </summary>
	/// <status>reviewed</status>
	public interface IPlanarShape {

		/// <summary>
		/// Rotation angle of the shape. 
		/// The shape is rotated around its rotation control point in tenths of a degree.
		/// </summary>
		int Angle { get; set; }

		/// <summary>
		/// Style used to paint the shape's area.
		/// </summary>
		IFillStyle FillStyle { get; set; }

		///// <summary>
		///// A text displayed inside the shape
		///// </summary>
		//string Text { get; set; }
	}


	/// <summary>
	/// One-dimensional shape defined by a sequence of vertices.
	/// </summary>
	public interface ILinearShape {

		/// <summary>
		/// Adds a new ControlPoint to the interior of the shape.
		/// </summary>
		/// <param name="x">X coordinate of the new Controlpoint.</param>
		/// <param name="y">Y coordinate of the new ControlPoint.</param>
		/// <returns>ControlPointId of the new ControlPoint</returns>
		ControlPointId AddVertex(int x, int y);

		/// <summary>
		/// Inserts a new control point to the shape before the control point with the 
		/// given Id. The new Point may be outside the shape, thus changing the outline of the shape.
		/// </summary>
		/// <param name="beforePointId">PointId of the ControlPoint the new point should be inserted before.</param>
		/// <param name="x">X coordinate of the new ControlPoint.</param>
		/// <param name="y">Y coordinate of the new ControlPoint.</param>
		/// <returns>ControlPointId of the new ControlPoint</returns>
		ControlPointId InsertVertex(ControlPointId beforePointId, int x, int y);

		/// <summary>
		/// Removes the point with the given ControlPointId from the line.
		/// </summary>
		/// <param name="pointId">PointId of the point to remove.</param>
		void RemoveVertex(ControlPointId pointId);

		/// <summary>
		/// Creates a new connection point at the specified position and returns the ControlPointId of the created connection point.
		/// The specified coordinates have to be on the outline of the linear shape.
		/// </summary>
		ControlPointId AddConnectionPoint(int x, int y);

		/// <summary>
		/// Removes the specified (dynamic) connection point from the linear shape.
		/// If shapes are connected to the connection point, these shapes will be disconnected.
		/// If the specified ControlPointId is not a dynamic connection point (e.g. a resize point), an exception will be thrown.
		/// </summary>
		void RemoveConnectionPoint(ControlPointId pointId);

		/// <summary>
		/// Returns the <see cref="T:Dataweb.NShape.Advanced.ControlPointId" /> of the vertex next to the vertex associated with the given <see cref="T:Dataweb.NShape.Advanced.ControlPointId" /> (Direction: <see cref="T:Dataweb.NShape.Advanced.ControlPointId.FirstVertex" /> to <see cref="T:Dataweb.NShape.Advanced.ControlPointId.LastVertex" />).
		/// </summary>
		ControlPointId GetNextVertexId(ControlPointId vertexId);

		/// <summary>
		/// Returns the <see cref="T:Dataweb.NShape.Advanced.ControlPointId" /> of the vertex next to the vertex associated with the given <see cref="T:Dataweb.NShape.Advanced.ControlPointId" /> (Direction: <see cref="T:Dataweb.NShape.Advanced.ControlPointId.LastVertex" /> to <see cref="T:Dataweb.NShape.Advanced.ControlPointId.FirstVertex" />).
		/// </summary>
		ControlPointId GetPreviousVertexId(ControlPointId vertexId);

		/// <summary>Specifies the minimum number of vertices needed for defining the linear shape</summary>
		int MinVertexCount { get; }

		/// <summary>Specifies the maximum number of vertices allowed for defining the shape.</summary>
		int MaxVertexCount { get; }

		/// <summary>Specifies the current number of vertices the linear shape consists of.</summary>
		int VertexCount { get; }

		/// <summary>Calculates the normal vector with length 100 in the given point with respect to the outline.</summary>
		Point CalcNormalVector(Point point);

		/// <summary>Indicates if the line has a direction, e.g. a cap on one side.</summary>
		bool IsDirected { get; }

	}


	/// <summary>
	/// Describes a connection between an active and a passive shape.
	/// </summary>
	/// <status>reviewed</status>
	public struct ShapeConnectionInfo : IEquatable<ShapeConnectionInfo> {

		/// <ToBeCompleted></ToBeCompleted>
		public static readonly ShapeConnectionInfo Empty;


		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator ==(ShapeConnectionInfo x, ShapeConnectionInfo y) {
			return (x.OwnPointId == y.OwnPointId
				&& x.OtherShape == y.OtherShape
				&& x.OtherPointId == y.OtherPointId);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator !=(ShapeConnectionInfo x, ShapeConnectionInfo y) { 
			return !(x == y); 
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static ShapeConnectionInfo Create(ControlPointId ownPointId, Shape otherShape, ControlPointId otherPointId) {
			ShapeConnectionInfo result = ShapeConnectionInfo.Empty;
			result.ownPointId = ownPointId;
			result.otherShape = otherShape;
			result.otherPointId = otherPointId;
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ShapeConnectionInfo(ControlPointId ownPointId, Shape otherShape, ControlPointId otherPointId) {
		   this.ownPointId = ownPointId;
		   this.otherShape = otherShape;
		   this.otherPointId = otherPointId;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Shape OtherShape {
			get { return otherShape; }
			internal set { otherShape = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ControlPointId OtherPointId {
			get { return otherPointId; }
			internal set { otherPointId = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ControlPointId OwnPointId {
			get { return ownPointId; }
			internal set { ownPointId = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool IsEmpty {
			get {
				return (this.ownPointId == Empty.ownPointId
					&& this.otherShape == Empty.otherShape
					&& this.otherPointId == Empty.otherPointId);
			}
		}


		/// <override></override>
		public override bool Equals(object obj) {
			return (obj is ShapeConnectionInfo && this == (ShapeConnectionInfo)obj);
		}


		/// <override></override>
		public bool Equals(ShapeConnectionInfo other) {
			return other == this;
		}


		/// <override></override>
		public override int GetHashCode() {
			int result = otherPointId.GetHashCode() ^ ownPointId.GetHashCode();
			if (otherShape != null) result ^= otherShape.GetHashCode();
			return result;
		}


		static ShapeConnectionInfo() {
			Empty.OwnPointId = ControlPointId.None;
			Empty.OtherShape = null;
			Empty.OtherPointId = ControlPointId.None;
		}

		/// <summary>other shape connected via this connection</summary>
		private Shape otherShape;
		/// <summary>control point of other shape used to connect</summary>
		private ControlPointId otherPointId;
		/// <summary>control point of this shape used to connect</summary>
		private ControlPointId ownPointId;
	}


	/// <summary>
	/// Describes the capabilities of a control point.
	/// </summary>
	[Flags]
	public enum ControlPointCapabilities {
		/// <summary>Nothing</summary>
		None = 0x0,
		/// <summary>Reference point</summary>
		Reference = 0x01,
		/// <summary>Can be used to resize the shape.</summary>
		Resize = 0x02,
		/// <summary>Center for rotations.</summary>
		Rotate = 0x04,
		/// <summary>Glue points can connect to this point.</summary>
		Connect = 0x08,
		/// <summary>A control point that can connect to connection points.</summary>
		Glue = 0x10,
		/// <summary>A control point that can be moved but does not affect the outline of a shape.</summary>
		Movable = 0x20,
		/// <summary>All capabilities</summary>
		All = 0xFF
	}


	/// <summary>
	/// Identifies a control point within a shape.
	/// </summary>
	/// <remarks>Regular control points have integer ids greater than 0. Special control point ids are
	/// -1: First vertex of a linear shape; -2: Last vertex of a linear shape.
	/// </remarks>
	public struct ControlPointId : IConvertible, IEquatable<ControlPointId>, IEquatable<int>, IComparable, IComparable<int>, IComparable<ControlPointId> {

		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator ==(ControlPointId id1, ControlPointId id2) {
			return id1.id == id2.id;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator ==(ControlPointId id1, int id2) {
			return id1.id == id2;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator ==(int id1, ControlPointId id2) {
			return id1 == id2.id;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator !=(ControlPointId id1, ControlPointId id2) {
			return id1.id != id2.id;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator !=(ControlPointId id1, int id2) {
			return id1.id != id2;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator !=(int id1, ControlPointId id2) {
			return id1 != id2.id;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static implicit operator ControlPointId(int id) {
			return ControlPointId.Create(id);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static implicit operator int(ControlPointId cpi) {
			return cpi.id;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static ControlPointId Create(int id) {
			ControlPointId result = empty;
			result.id = id;
			return result;
		}


		/// <override></override>
		public bool Equals(ControlPointId other) {
			return other == this;
		}


		/// <override></override>
		public bool Equals(int other) {
			return other == this.id;
		}


		/// <override></override>
		public override bool Equals(object obj) {
			if (!(obj is ControlPointId)) return false;
			else return this == (ControlPointId)obj;
		}


		/// <override></override>
		public override int GetHashCode() {
			return id.GetHashCode();
		}


		/// <override></override>
		public override string ToString() {
			return id.ToString();
		}


		/// <summary>
		/// Generic control point id used to indicate no or an invalid control point.
		/// </summary>
		public const int None = int.MinValue;

		/// <summary>
		/// Generic control point id used to designate any control point.
		/// </summary>
		public const int Any = int.MaxValue;

		/// <summary>
		/// Generic control point id used to identify the reference point.
		/// </summary>
		public const int Reference = 0;

		/// <summary>
		/// Generic control point id used to identify the start point of a linear shape.
		/// </summary>
		public const int FirstVertex = -1;

		/// <summary>
		/// Generic control point id used to identify the end point of a linear shape.
		/// </summary>
		public const int LastVertex = -2;


		#region IComparable Members

		/// <ToBeCompleted></ToBeCompleted>
		public int CompareTo(object obj) {
			if (obj == null) return 1;
			if (obj is ControlPointId) 
				return CompareTo((ControlPointId)obj);
			else if (obj is int || obj is short || obj is byte)
				return CompareTo((int)obj);
			else throw new ArgumentException("obj");
		}

		#endregion


		#region IComparable<int> Members

		/// <ToBeCompleted></ToBeCompleted>
		public int CompareTo(int other) {
			if (this < other) return -1;
			if (this > other) return 1;
			return 0;
		}

		#endregion


		#region IComparable<ControlPointId> Members

		/// <ToBeCompleted></ToBeCompleted>
		public int CompareTo(ControlPointId other) {
			if (this < other) return -1;
			if (this > other) return 1;
			return 0;
		}

		#endregion


		#region IConvertible Members

		TypeCode IConvertible.GetTypeCode() {
			return TypeCode.Int32;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		byte IConvertible.ToByte(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		char IConvertible.ToChar(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		double IConvertible.ToDouble(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		short IConvertible.ToInt16(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		int IConvertible.ToInt32(IFormatProvider provider) {
			return id;
		}

		long IConvertible.ToInt64(IFormatProvider provider) {
			return id;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		float IConvertible.ToSingle(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		string IConvertible.ToString(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
			if (conversionType == typeof(int)) return id;
			else if (conversionType == typeof(long)) return id;
			else throw new InvalidCastException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider) {
			return (ulong)id;
		}

		#endregion


		static ControlPointId() {
			empty = None;
		}


		private static readonly ControlPointId empty;
		private int id;
	}


	/// <summary>
	/// ResizeModifiers for resizing selected shapes.
	/// </summary>
	/// <status>reviewed</status>
	[Flags]
	public enum ResizeModifiers {
		/// <summary>Standard resizing</summary>
		None = 0,
		/// <summary>Maintain aspect ratio while resizing</summary>
		MaintainAspect = 1,
		/// <summary>Resizing is also applied to the opposite side</summary>
		MirroredResize = 2
	}


	/// <summary>
	/// Stores the relative Position of a Shape. The way how an absolute position is 
	/// calculated from this relative position depends on the shape.
	/// </summary>
	/// <status>reviewed</status>
	public struct RelativePosition : IEquatable<RelativePosition> {

		/// <ToBeCompleted></ToBeCompleted>
		public static readonly RelativePosition Empty;


		/// <summary>
		/// Tests whether two specified <see cref="T:Dataweb.NShape.Advanced.RelativePosition" /> objects are equivalent.
		/// </summary>
		public static bool operator ==(RelativePosition x, RelativePosition y) { 
			return (x.A == y.A && x.B == y.B && x.C == y.C); 
		}


		/// <summary>
		/// Tests whether two specified <see cref="T:Dataweb.NShape.Advanced.RelativePosition" /> objects are not equivalent.
		/// </summary>
		public static bool operator !=(RelativePosition x, RelativePosition y) { 
			return !(x == y); 
		}


		/// <ToBeCompleted></ToBeCompleted>
		public int A;


		/// <ToBeCompleted></ToBeCompleted>
		public int B;


		/// <ToBeCompleted></ToBeCompleted>
		public int C;


		/// <override></override>
		public override bool Equals(object obj) { 
			return obj is RelativePosition && this == (RelativePosition)obj; 
		}


		/// <override></override>
		public bool Equals(RelativePosition other) {
			return other == this;
		}


		/// <override></override>
		public override int GetHashCode() {
			return (A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode());
		}

		
		static RelativePosition() {
			Empty.A = int.MinValue;
			Empty.B = int.MinValue;
			Empty.C = int.MinValue;
		}
	}

}