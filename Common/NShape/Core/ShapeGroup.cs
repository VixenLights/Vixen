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


namespace Dataweb.NShape.Advanced
{
	/// <summary>
	/// Marker interface for groups.
	/// </summary>
	public interface IShapeGroup
	{
		/// <summary>
		/// Called by a child shape in order to notify its parent that its children's layout will change.
		/// </summary>
		void NotifyChildLayoutChanging();

		/// <summary>
		/// Called by a child shape in order to notify its parent that its children's layout has changed.
		/// </summary>
		void NotifyChildLayoutChanged();
	}


	/// <summary>
	/// Groups a set of shapes together.
	/// </summary>
	/// <remarks>RequiredPermissions set</remarks>
	public class ShapeGroup : Shape, IShapeGroup
	{
		internal static ShapeGroup CreateInstance(ShapeType shapeType, Template template)
		{
			return new ShapeGroup(shapeType, template);
		}

		#region Shape Members

		/// <override></override>
		public override void CopyFrom(Shape source)
		{
			if (source == null) throw new ArgumentNullException("source");

			this.permissionSetName = source.SecurityDomainName;
			this.tag = source.Tag;
			this.displayService = source.DisplayService;
			//this.Parent = source.Parent;

			//this.location.X = source.X;
			//this.location.Y = source.Y;

			// Do not recalculate Center after adding the children because in case the group is rotated, 
			// the rotation center would not be the same.
			if (source.Children != null)
				children.CopyFrom(source.Children);

			// Do not assign to the property but to the field because the children are already on their (rotated) positions
			if (source is IPlanarShape)
				this.angle = ((IPlanarShape) source).Angle;
			if (source is ShapeGroup)
				this.angle = ((ShapeGroup) source).Angle;
		}


		/// <override></override>
		public override ShapeType Type
		{
			get { return shapeType; }
		}


		/// <override></override>
		public override IModelObject ModelObject
		{
			get { return modelObject; }
			set { modelObject = value; }
		}


		/// <override></override>
		public override Template Template
		{
			get { return template; }
		}


		/// <override></override>
		public override Diagram Diagram
		{
			get
			{
				if (owner is DiagramShapeCollection)
					return ((DiagramShapeCollection) owner).Owner;
				else return null;
			}
			internal set
			{
				if (owner != null && owner.Contains(this)) {
					owner.Remove(this);
					owner = null;
				}
				if (value != null) {
					if (value.Shapes is ShapeCollection)
						owner = (ShapeCollection) value.Shapes;
					else
						throw new ArgumentException(string.Format("{0}'s Shapes property must be a {1}", value.GetType().Name,
						                                          typeof (ShapeCollection).Name));
				}
				else owner = null;
			}
		}


		/// <override></override>
		public override Shape Parent
		{
			get
			{
				if (owner is ShapeAggregation) return ((ShapeAggregation) owner).Owner;
				else return null;
			}
			set
			{
				if (value != null) {
					if (owner != null && owner != value.Children) {
						owner.Remove(this);
						owner = null;
					}
					if (value is ShapeGroup)
						owner = ((ShapeGroup) value).children;
					else if (value.Children is ShapeAggregation)
						owner = (ShapeAggregation) value.Children;
					else
						throw new ArgumentException(string.Format("{0}'s Children property must be a {1}", value.GetType().Name,
						                                          typeof (ShapeAggregation).Name));
				}
				else owner = null;
			}
		}


		/// <override></override>
		public override IShapeCollection Children
		{
			get { return children; }
		}


		/// <override></override>
		public override object Tag
		{
			get { return tag; }
			set { tag = value; }
		}


		/// <override></override>
		public override char SecurityDomainName
		{
			get { return permissionSetName; }
			set
			{
				if (value < 'A' || value > 'Z')
					throw new ArgumentOutOfRangeException("The domain qualifier has to be an upper case  ANSI letter (A-Z).");
				permissionSetName = value;
			}
		}


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs(int mouseX, int mouseY, int range)
		{
			// no actions for the moment...
			if (template != null) {
				foreach (MenuItemDef action in template.GetMenuItemDefs())
					yield return action;
			}
			if (modelObject != null) {
				foreach (MenuItemDef action in modelObject.GetMenuItemDefs())
					yield return action;
			}
		}


		/// <override></override>
		public override void NotifyModelChanged(int modelPropertyId)
		{
			for (int i = children.Count - 1; i >= 0; --i)
				NotifyModelChanged(modelPropertyId);
		}


		/// <override></override>
		public override void Connect(ControlPointId gluePointId, Shape targetShape, ControlPointId targetPointId)
		{
			// nothing to do
		}


		/// <override></override>
		public override void Disconnect(ControlPointId gluePointId)
		{
			// nothing to do
		}


		/// <override></override>
		public override IEnumerable<ShapeConnectionInfo> GetConnectionInfos(ControlPointId ownPointId, Shape otherShape)
		{
			yield break;
		}


		/// <override></override>
		public override ShapeConnectionInfo GetConnectionInfo(ControlPointId gluePointId, Shape otherShape)
		{
			return ShapeConnectionInfo.Empty;
		}


		/// <override></override>
		public override ControlPointId IsConnected(ControlPointId ownPointId, Shape otherShape)
		{
			return ControlPointId.None;
		}


		/// <override></override>
		public override void FollowConnectionPointWithGluePoint(ControlPointId gluePointId, Shape connectedShape,
		                                                        ControlPointId movedPointId)
		{
			// nothing to do
		}


		/// <override></override>
		public override RelativePosition CalculateRelativePosition(int x, int y)
		{
			if (!Geometry.IsValid(x, y)) throw new ArgumentOutOfRangeException("x, y");
			RelativePosition result = RelativePosition.Empty;
			result.A = x - X;
			result.B = y - Y;
			return result;
		}


		/// <override></override>
		public override Point CalculateAbsolutePosition(RelativePosition relativePosition)
		{
			if (relativePosition == RelativePosition.Empty) throw new ArgumentOutOfRangeException("relativePosition");
			Point result = Point.Empty;
			result.X = relativePosition.A + X;
			result.Y = relativePosition.A + Y;
			return result;
		}


		/// <override></override>
		public override Point CalculateNormalVector(int x, int y)
		{
			return Geometry.CalcNormalVectorOfRectangle(GetBoundingRectangle(true), x, y, 100);
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int fromX, int fromY)
		{
			Point result = Point.Empty;
			result.Offset(X, Y);
			float distance, lowestDistance = float.MaxValue;
			// Use the nearest intersection point not contained by an other shape of the group
			foreach (Shape shape in children) {
				// ToDo3: Improve this implementation by using InterSectOutlineWithLineSegment(fromX, fromY, X, Y)
				Point p = shape.CalculateConnectionFoot(fromX, fromY);
				if (p == Point.Empty) {
					ControlPointId nearestCtrlPtId = shape.FindNearestControlPoint(fromX, fromY, int.MaxValue,
					                                                               ControlPointCapabilities.All);
					if (nearestCtrlPtId != ControlPointId.None)
						p = GetControlPointPosition(nearestCtrlPtId);
					else {
						p.X = int.MaxValue;
						p.Y = int.MaxValue;
					}
				}
				// calculate distance to point and set result if a new nearest point is found
				distance = Geometry.DistancePointPoint(p.X, p.Y, fromX, fromY);
				if (distance < lowestDistance) {
					// If the new nearest point is contained by an other shape:
					// Skip it, as we do not want lines that cross, intersect or end within shapes of the group
					bool otherShapeContainsPoint = false;
					foreach (Shape s in children.BottomUp) {
						if (s == shape) continue;
						if (s.ContainsPoint(p.X, p.Y)) {
							otherShapeContainsPoint = true;
							break;
						}
					}
					if (!otherShapeContainsPoint) {
						lowestDistance = distance;
						result = p;
					}
				}
			}
			return result;
		}


		/// <override></override>
		public override bool IntersectsWith(int x, int y, int width, int height)
		{
			Rectangle rect = Rectangle.Empty;
			rect.X = x;
			rect.Y = y;
			rect.Width = width;
			rect.Height = height;
			if (children.Count <= 0) return Geometry.RectangleContainsPoint(rect, X, Y);
			else if (Geometry.RectangleIntersectsWithRectangle(rect, children.GetBoundingRectangle(false))) {
				foreach (Shape shape in children)
					if (shape.IntersectsWith(x, y, width, height))
						return true;
			}
			return false;
		}


		/// <override></override>
		public override IEnumerable<Point> IntersectOutlineWithLineSegment(int x1, int y1, int x2, int y2)
		{
			// Use the nearest intersection point not contained by an other shape of the group
			foreach (Shape shape in children) {
				foreach (Point p in shape.IntersectOutlineWithLineSegment(x1, y1, x2, y2))
					if (!ContainsPoint(p.X, p.Y)) yield return p;
			}
		}


		/// <override></override>
		public override ControlPointId HitTest(int x, int y, ControlPointCapabilities controlPointCapability, int range)
		{
			//if ((controlPointCapability & ControlPointCapabilities.Reference) > 0)
			//   if (Geometry.DistancePointPoint(X, Y, x, y) <= distance)
			//      return true;
			if ((controlPointCapability & ControlPointCapabilities.Rotate) > 0) {
				if (Geometry.DistancePointPoint(RotatePoint.X, RotatePoint.Y, x, y) <= range)
					return RotatePointId;
				controlPointCapability ^= ControlPointCapabilities.Rotate;
			}
			foreach (Shape shape in children) {
				ControlPointId pointId = shape.HitTest(x, y, controlPointCapability, range);
				//if (pointId != ControlPointId.None) return pointId;

				// All control points but the rotate point are deactivated
				if (pointId != ControlPointId.None) return ControlPointId.Reference;
			}
			return ControlPointId.None;
		}


		/// <override></override>
		public override bool ContainsPoint(int x, int y)
		{
			if (X == x && Y == y)
				return true;
			// TODO 2: Can be optimized using the shape map.
			foreach (Shape shape in children)
				if (shape.ContainsPoint(x, y))
					return true;
			return false;
		}


		/// <override></override>
		public override Rectangle GetBoundingRectangle(bool tight)
		{
			Rectangle result = Rectangle.Empty;
			if (children.Count <= 0) result.Offset(X, Y);
			else {
				result = children.GetBoundingRectangle(tight);
				if (!Geometry.RectangleContainsPoint(result, X, Y))
					result = Geometry.UniteRectangles(X, Y, X, Y, result);
			}
			return result;
		}


		/// <override></override>
		protected internal override IEnumerable<Point> CalculateCells(int cellSize)
		{
			foreach (Shape s in children)
				foreach (Point p in s.CalculateCells(cellSize))
					yield return p;
		}


		/// <override></override>
		public override int X
		{
			get { return children.Center.X; }
			set
			{
				int origValue = children.Center.X;
				if (!MoveTo(value, children.Center.Y)) {
					MoveTo(origValue, children.Center.Y);
					throw new InvalidOperationException(string.Format("Shape cannot move to {0}.", new Point(value, children.Center.Y)));
				}
			}
		}


		/// <override></override>
		public override int Y
		{
			get { return children.Center.Y; }
			set
			{
				int origValue = children.Center.Y;
				if (!MoveTo(children.Center.X, value)) {
					MoveTo(children.Center.X, origValue);
					throw new InvalidOperationException(string.Format("Shape cannot move to {0}.", new Point(children.Center.X, value)));
				}
			}
		}


		/// <override></override>
		public override void Fit(int x, int y, int width, int height)
		{
			MoveTo(x + width/2, y + height/2);
		}


		///// <override></override>
		//public override bool MoveTo(int toX, int toY) {
		//    bool result = false;
		//    Invalidate();
		//    if (Owner != null) Owner.NotifyChildMoving(this);

		//    result = children.NotifyParentMoved(toX - X, toY - Y);

		//    if (Owner != null) Owner.NotifyChildMoved(this);
		//    Invalidate();
		//    return result;
		//}


		/// <override></override>
		public override bool MoveBy(int deltaX, int deltaY)
		{
			bool result = false;
			Invalidate();
			if (Owner != null) Owner.NotifyChildMoving(this);

			//location.Offset(deltaX, deltaY);
			result = children.NotifyParentMoved(deltaX, deltaY);

			if (Owner != null) Owner.NotifyChildMoved(this);
			Invalidate();
			return result;
		}


		/// <override></override>
		public override bool MoveControlPointBy(ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers)
		{
			bool result = false;
			if (pointId == ControlPointId.Reference)
				result = MoveBy(deltaX, deltaY);
			else {
				if (Owner != null) Owner.NotifyChildResizing(this);
				result = false;
				if (Owner != null) Owner.NotifyChildResized(this);
			}
			return result;
		}


		/// <override></override>
		public override bool Rotate(int deltaAngle, int x, int y)
		{
			bool result = true;
			// Notify Owner
			if (Owner != null) Owner.NotifyChildRotating(this);

			// First, perform rotation around the center point...
			angle = (3600 + angle + deltaAngle)%3600;
			// ...then, rotate the shape's center around the given rotation center and 
			// move the shape (including its children) to this point
			if (x != X || y != Y) {
				int toX = X;
				int toY = Y;
				Geometry.RotatePoint(x, y, Geometry.TenthsOfDegreeToDegrees(deltaAngle), ref toX, ref toY);
				if (!MoveTo(toX, toY)) result = false;
			}

			// Notify children and owner
			if (!children.NotifyParentRotated(deltaAngle, X, Y)) result = false;
			if (Owner != null) Owner.NotifyChildRotated(this);
			return result;
		}


		/// <override></override>
		public override Point GetControlPointPosition(ControlPointId controlPointId)
		{
			if (controlPointId == ControlPointId.Reference)
				//return location;
				return children.Center;
			else if (controlPointId == RotatePointId)
				return RotatePoint;
			else if (controlPointId == ControlPointId.None)
				throw new ArgumentException(string.Format("{0} is not a valid {1} for this operation.", controlPointId,
				                                          typeof (ControlPointId).Name));
			return Point.Empty;
		}


		/// <override></override>
		public override IEnumerable<ControlPointId> GetControlPointIds(ControlPointCapabilities controlPointCapability)
		{
			if ((controlPointCapability & ControlPointCapabilities.Reference) > 0
			    || (controlPointCapability & ControlPointCapabilities.Rotate) > 0)
				yield return RotatePointId;
		}


		/// <override></override>
		public override ControlPointId FindNearestControlPoint(int x, int y, int distance,
		                                                       ControlPointCapabilities controlPointCapability)
		{
			if ((controlPointCapability & ControlPointCapabilities.Reference) > 0) {
				if (Geometry.DistancePointPoint(x, y, X, Y) <= distance)
					return ControlPointId.Reference;
			}
			else if ((controlPointCapability & ControlPointCapabilities.Rotate) > 0) {
				if (Geometry.DistancePointPoint(x, y, RotatePoint.X, RotatePoint.Y) <= distance)
					return RotatePointId;
			}
			return ControlPointId.None;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId,
		                                               ControlPointCapabilities controlPointCapability)
		{
			if (controlPointId == RotatePointId || controlPointId == ControlPointId.Reference)
				return ((controlPointCapability & ControlPointCapabilities.Rotate) > 0
				        || (controlPointCapability & ControlPointCapabilities.Reference) > 0);
			return false;
		}


		/// <override></override>
		public override IDisplayService DisplayService
		{
			get { return displayService; }
			set
			{
				if (displayService != value) {
					displayService = value;
					if (children != null && children.Count > 0)
						children.SetDisplayService(displayService);
				}
			}
		}


		/// <override></override>
		public override void MakePreview(IStyleSet styleSet)
		{
			foreach (Shape shape in children)
				shape.MakePreview(styleSet);
		}


		/// <override></override>
		public override ILineStyle LineStyle
		{
			// Shape groups do not have a line style. They return null and ignore the setting.
			get { return null; }
			set
			{
				// Nothing to do
			}
		}


		/// <override></override>
		public override bool HasStyle(IStyle style)
		{
			bool result = false;
			foreach (Shape shape in children)
				if (shape.HasStyle(style)) result = true;
			return result;
		}


		/// <override></override>
		protected internal override bool NotifyStyleChanged(IStyle style)
		{
			bool result = false;
			foreach (Shape shape in children)
				if (shape.NotifyStyleChanged(style)) result = true;
			return result;
		}


		/// <override></override>
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) throw new ArgumentNullException("graphics");
			children.Draw(graphics);
		}


		/// <override></override>
		public override void DrawOutline(Graphics graphics, Pen pen)
		{
			if (graphics == null) throw new ArgumentNullException("graphics");
			if (pen == null) throw new ArgumentNullException("pen");
			children.DrawOutline(graphics, pen);
		}


		/// <override></override>
		public override void DrawThumbnail(Image image, int margin, Color transparentColor)
		{
			if (image == null) throw new ArgumentNullException("image");
			using (Graphics gfx = Graphics.FromImage(image)) {
				GdiHelpers.ApplyGraphicsSettings(gfx, RenderingQuality.MaximumQuality);
				gfx.Clear(transparentColor);

				using (Font font = new Font(FontFamily.GenericSansSerif, 9)) {
					Rectangle layoutRect = Rectangle.Empty;
					layoutRect.Size = image.Size;
					layoutRect.Inflate(-margin, -margin);
					gfx.DrawString("Icon", font, Brushes.Black, layoutRect);
					font.Dispose();
				}
			}
		}


		/// <override></override>
		public override void Invalidate()
		{
			if (displayService != null)
				displayService.Invalidate(GetBoundingRectangle(false));
		}

		#endregion

		/// <ToBeCompleted></ToBeCompleted>
		[Category("Layout")]
		[Description("Rotation angle of the Shape in tenths of degree.")]
		[PropertyMappingId(PropertyIdAngle)]
		[RequiredPermission(Permission.Layout)]
		public int Angle
		{
			get { return angle; }
			set
			{
				Invalidate();
				int deltaAngle = value - angle;
				Rotate(deltaAngle, X, Y);
				Invalidate();
			}
		}

		#region IShapeGroup Members

		/// <ToBeCompleted></ToBeCompleted>
		public void NotifyChildLayoutChanging()
		{
			if (Owner != null) Owner.NotifyChildResizing(this);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void NotifyChildLayoutChanged()
		{
			if (Owner != null) Owner.NotifyChildResized(this);
		}

		#endregion

		#region IEntity Members

		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.ShapeGroup" />.
		/// </summary>
		public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			yield return new EntityFieldDefinition("Template", typeof (object));
			yield return new EntityFieldDefinition("X", typeof (int));
			yield return new EntityFieldDefinition("Y", typeof (int));
			yield return new EntityFieldDefinition("ZOrder", typeof (int));
			yield return new EntityFieldDefinition("Layers", typeof (int));
			yield return new EntityFieldDefinition("Angle", typeof (int));
			// yield return new EntityInnerObjectsDefinition(childPosPropertyName, "Core.Point", pointAttrNames, pointAttrTypes);
		}


		/// <override></override>
		protected override sealed object IdCore
		{
			get { return id; }
		}


		/// <override></override>
		protected override sealed void AssignIdCore(object id)
		{
			if (id == null) throw new ArgumentNullException("id");
			if (this.id != null) throw new InvalidOperationException("Shape group has already an id.");
			this.id = id;
		}


		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version)
		{
			template = reader.ReadTemplate();
			int x = reader.ReadInt32();
			int y = reader.ReadInt32();
			MoveTo(x, y);
			ZOrder = reader.ReadInt32();
			Layers = (LayerIds) reader.ReadInt32();
			angle = reader.ReadInt32();
		}


		/// <override></override>
		protected override void LoadInnerObjectsCore(string propertyName, IRepositoryReader reader, int version)
		{
			// nothing to do
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version)
		{
			// Do the actual writing
			writer.WriteTemplate(template);
			writer.WriteInt32(X);
			writer.WriteInt32(Y);
			writer.WriteInt32(ZOrder);
			writer.WriteInt32((int) Layers);
			writer.WriteInt32(angle);
		}


		/// <override></override>
		protected override void SaveInnerObjectsCore(string propertyName, IRepositoryWriter writer, int version)
		{
			// nothing to do
		}


		/// <override></override>
		protected override void DeleteCore(IRepositoryWriter writer, int version)
		{
			// nothing to do
		}

		#endregion

		#region ICloneable Members

		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new ShapeGroup(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}

		#endregion

		#region IDisposable Members

		/// <override></override>
		public override void Dispose()
		{
			if (ModelObject != null) ModelObject = null;
			foreach (Shape child in children) child.Dispose();
		}

		#endregion

		/// <override></override>
		protected internal ShapeGroup(ShapeType shapeType, Template template)
			: base()
		{
			if (shapeType == null) throw new ArgumentNullException("shapeType");
			this.shapeType = shapeType;
			this.template = template;
			children = new GroupShapeAggregation(this);
		}


		/// <override></override>
		protected internal ShapeGroup(ShapeType shapeType, IStyleSet styleSet)
			: base()
		{
			if (shapeType == null) throw new ArgumentNullException("shapeType");
			this.shapeType = shapeType;
			this.template = null;
			children = new GroupShapeAggregation(this);
		}


		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet)
		{
			foreach (Shape shape in children)
				shape.InitializeToDefault(styleSet);
		}


		/// <override></override>
		protected internal override void AttachGluePointToConnectionPoint(ControlPointId ownPointId, Shape otherShape,
		                                                                  ControlPointId gluePointId)
		{
			if (ownPointId != ControlPointId.Reference &&
			    !HasControlPointCapability(ownPointId, ControlPointCapabilities.Connect))
				throw new NShapeException(string.Format("{0}'s point {1} has to be a connection point.", Type.Name, ownPointId));
			if (!otherShape.HasControlPointCapability(gluePointId, ControlPointCapabilities.Glue))
				throw new NShapeException(string.Format("{0}'s point {1} has to be a glue point.", otherShape.Type.Name, gluePointId));
			throw new NotSupportedException();
		}


		/// <override></override>
		protected internal override void DetachGluePointFromConnectionPoint(ControlPointId ownPointId, Shape targetShape,
		                                                                    ControlPointId targetPointId)
		{
			throw new NotSupportedException();
		}


		/// <override></override>
		protected internal override object InternalTag
		{
			get { return internalTag; }
			set { internalTag = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected ShapeCollection Owner
		{
			get { return owner; }
		}


		private Point RotatePoint
		{
			get { return (children != null) ? children.Center : Point.Empty; }
		}

		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdAngle = 2;

		// Shape fields
		private Template template = null;
		private IModelObject modelObject = null;
		private ShapeType shapeType = null;
		private IDisplayService displayService;
		private char permissionSetName = 'A';
		private ShapeCollection owner = null;
		private GroupShapeAggregation children;
		private object id = null;
		private object tag = null;
		private object internalTag;
		//private Point location;

		// ShapeGroup fields
		private const int RotatePointId = 1;
		private int angle;

		#endregion
	}
}