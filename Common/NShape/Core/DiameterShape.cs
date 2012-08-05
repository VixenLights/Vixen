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


namespace Dataweb.NShape.Advanced {

	/// <remarks>RequiredPermissions set</remarks>
	public abstract class DiameterShapeBase : CaptionedShapeBase {

		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			internalDiameter = 40;
		}


		/// <override></override>
		public override void CopyFrom(Shape source) {
			base.CopyFrom(source);
			// Copy size if the source is a DiameterShape
			if (source is DiameterShapeBase)
				internalDiameter = ((DiameterShapeBase)source).DiameterInternal;
			else {
				// If not, try to calculate the size a good as possible
				Rectangle srcBounds = Geometry.InvalidRectangle;
				if (source is PathBasedPlanarShape) {
					PathBasedPlanarShape src = (PathBasedPlanarShape)source;
					// Calculate the bounds of the (unrotated) resize handles because with 
					// GetBoundingRectangle(), we receive the bounds including the children's bounds
					List<Point> pointBuffer = new List<Point>();
					int centerX = src.X; int centerY = src.Y;
					float angleDeg = Geometry.TenthsOfDegreeToDegrees(-src.Angle);
					foreach (ControlPointId id in source.GetControlPointIds(ControlPointCapabilities.Resize))
						pointBuffer.Add(Geometry.RotatePoint(centerX, centerY, angleDeg, source.GetControlPointPosition(id)));
					Geometry.CalcBoundingRectangle(pointBuffer, out srcBounds);
				} else {
					// Generic approach: try to fit into the bounding rectangle
					srcBounds = source.GetBoundingRectangle(true);
				}
				//
				// Calculate new size
				if (Geometry.IsValid(srcBounds)) {
					float scale = Geometry.CalcScaleFactor(DiameterInternal, DiameterInternal, srcBounds.Width, srcBounds.Height);
					DiameterInternal = (int)Math.Round(DiameterInternal * scale);
				}
			}
		}


		#region IPersistable Members


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.DiameterShapeBase" />.
		/// </summary>
		new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
			foreach (EntityPropertyDefinition pi in CaptionedShapeBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("Diameter", typeof(Int32));
		}


		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version) {
			base.LoadFieldsCore(reader, version);
			internalDiameter = reader.ReadInt32();
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version) {
			base.SaveFieldsCore(writer, version);
			writer.WriteInt32(internalDiameter);
		}

		#endregion


		#region [Public] Properties

		/// <override></override>
		[Browsable(false)]
		protected internal override int ControlPointCount {
			get { return 9; }
		}

		#endregion


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopLeftControlPoint:
				case TopCenterControlPoint:
				case TopRightControlPoint:
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
				case BottomLeftControlPoint:
				case BottomCenterControlPoint:
				case BottomRightControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) > 0
							|| ((controlPointCapability & ControlPointCapabilities.Connect) > 0
								&& IsConnectionPointEnabled(controlPointId)));
				case ControlPointId.Reference:
				case MiddleCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Reference) > 0
							|| (controlPointCapability & ControlPointCapabilities.Rotate) > 0
							|| ((controlPointCapability & ControlPointCapabilities.Connect) > 0 
									&& IsConnectionPointEnabled(controlPointId)));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		public override Point CalculateAbsolutePosition(RelativePosition relativePosition) {
			if (relativePosition == RelativePosition.Empty) throw new ArgumentOutOfRangeException("relativePosition");
			// The RelativePosition of a RectangleBased shape is:
			// A = Tenths of percent of Width
			// B = Tenths of percent of Height
			Point result = Point.Empty;
			result.X = (int)Math.Round((X - DiameterInternal / 2f) + (DiameterInternal * (relativePosition.A / 1000f)));
			result.Y = (int)Math.Round((Y - DiameterInternal / 2f) + (DiameterInternal * (relativePosition.B / 1000f)));
			result = Geometry.RotatePoint(Center, Geometry.TenthsOfDegreeToDegrees(Angle), result);
			return result;
		}


		/// <override></override>
		public override RelativePosition CalculateRelativePosition(int x, int y) {
			if (!Geometry.IsValid(x, y)) throw new ArgumentOutOfRangeException("x / y");
			// The RelativePosition of a RectangleBased shape is:
			// A = Tenths of percent of Width
			// B = Tenths of percent of Height
			RelativePosition result = RelativePosition.Empty;
			if (Angle != 0) {
				float ptX = x;
				float ptY = y;
				Geometry.RotatePoint(X, Y, Geometry.TenthsOfDegreeToDegrees(-Angle), ref x, ref y);
			}
			result.A = (int)Math.Round((x - (X - DiameterInternal / 2f)) / (this.DiameterInternal / 1000f));
			result.B = (int)Math.Round((y - (Y - DiameterInternal / 2f)) / (this.DiameterInternal / 1000f));
			return result;
		}


		/// <override></override>
		public override void Fit(int x, int y, int width, int height) {
			// Calculate bounds (including children)
			Rectangle bounds = GetBoundingRectangle(true);
			// Calculate the shape's offset relative to the bounds
			float offsetX = (X - DiameterInternal / 2f) - bounds.X;
			float offsetY = (Y - DiameterInternal / 2f) - bounds.Y;
			// Calculate the scaling factor and the new position
			float scale = Geometry.CalcScaleFactor(bounds.Width, bounds.Height, width, height);
			float dstX = x + (width / 2f) + (offsetX * scale);
			float dstY = y + (height / 2f) + (offsetY * scale);
			// Move to new position and apply scaling
			MoveTo((int)Math.Round(dstX), (int)Math.Round(dstY));
			DiameterInternal = (int)Math.Floor(DiameterInternal * scale);
		}


		/// <override></override>
		public override void Draw(Graphics graphics) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			UpdateDrawCache();
			DrawPath(graphics, LineStyle, FillStyle);
			DrawCaption(graphics);
			base.Draw(graphics);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal DiameterShapeBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal DiameterShapeBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override int DivFactorX { get { return 2; } }


		/// <override></override>
		protected override int DivFactorY { get { return 2; } }


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers) {
			bool result = true;
			int dx = 0, dy = 0;
			int size = DiameterInternal;
			int hSize, vSize;
			// Diameter shapes always have to be resized with "MaintainAspect"!
			modifiers |= ResizeModifiers.MaintainAspect;
			switch (pointId) {
				// Top Left
				case TopLeftControlPoint:
					result = (transformedDeltaX == transformedDeltaY);
					if (!Geometry.MoveRectangleTopLeft(size, size, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out hSize, out vSize))
						result = false;
					size = Math.Min(hSize, vSize);
					break;
				// Top Center
				case TopCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleTop(size, size, 0, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out hSize, out vSize))
						result = false;
					size = vSize;
					break;
				// Top Right
				case TopRightControlPoint:
					result = (transformedDeltaX == -transformedDeltaY);
					if (!Geometry.MoveRectangleTopRight(size, size, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out hSize, out vSize))
						result = false;
					size = Math.Min(hSize, vSize);
					break;
				// Middle left
				case MiddleLeftControlPoint:
					result = (transformedDeltaY == 0);
					if (!Geometry.MoveRectangleLeft(size, size, transformedDeltaX, 0, cos, sin, modifiers, out dx, out dy, out hSize, out vSize))
						result = false;
					size = hSize;
					break;
				// Middle right
				case MiddleRightControlPoint:
					result = (transformedDeltaY == 0);
					if (!Geometry.MoveRectangleRight(size, size, transformedDeltaX, 0, cos, sin, modifiers, out dx, out dy, out hSize, out vSize))
						result = false;
					size = hSize;
					break;
				// Bottom left
				case BottomLeftControlPoint:
					result = (-transformedDeltaX == transformedDeltaY);
					if (!Geometry.MoveRectangleBottomLeft(size, size, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out hSize, out vSize))
						result = false;
					size = Math.Min(hSize, vSize);
					break;
				// Bottom Center
				case BottomCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleBottom(size, size, 0, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out hSize, out vSize))
						result = false;
					size = vSize;
					break;
				// Bottom right
				case BottomRightControlPoint:
					result = (transformedDeltaX == transformedDeltaY);
					if (!Geometry.MoveRectangleBottomRight(size, size, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out hSize, out vSize))
						result = false;
					size = Math.Min(hSize, vSize);
					break;
			}
			// Set field in order to avoid the shape being inserted into the owner's shape map
			internalDiameter = size;
			MoveByCore(dx, dy);
			ControlPointsHaveMoved();

			return result;
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			captionBounds = Rectangle.Empty;
			captionBounds.X = captionBounds.Y = (int)Math.Round(-DiameterInternal / 2f);
			captionBounds.Width = captionBounds.Height = DiameterInternal;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected int DiameterInternal {
			get { return internalDiameter; }
			set {
				if (value < 0)  throw new ArgumentOutOfRangeException();
				Invalidate();
				if (Owner != null) Owner.NotifyChildResizing(this);
				int delta = value - internalDiameter;

				internalDiameter = value;
				InvalidateDrawCache();

				if (ChildrenCollection != null) ChildrenCollection.NotifyParentSized(delta, delta);
				if (Owner != null) Owner.NotifyChildResized(this);
				ControlPointsHaveMoved();
				Invalidate();
			}
		}


		/// <override></override>
		protected override void ProcessExecModelPropertyChange(IModelMapping propertyMapping) {
			switch (propertyMapping.ShapePropertyId) {
				case PropertyIdDiameter:
					DiameterInternal = propertyMapping.GetInteger();
					break;
				default:
					base.ProcessExecModelPropertyChange(propertyMapping);
					break;
			}
		}


		#region Fields
		// PropertyId constant
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdDiameter = 7;

		// ControlPoint Id Constants
		private const int TopLeftControlPoint = 1;
		private const int TopCenterControlPoint = 2;
		private const int TopRightControlPoint = 3;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomLeftControlPoint = 6;
		private const int BottomCenterControlPoint = 7;
		private const int BottomRightControlPoint = 8;
		private const int MiddleCenterControlPoint = 9;

		private int internalDiameter = 0;
		#endregion
	}


	/// <remarks>RequiredPermissions set</remarks>
	public abstract class SquareBase : DiameterShapeBase {

		/// <ToBeCompleted></ToBeCompleted>
		[Category("Layout")]
		[Description("Size of the sqare.")]
		[PropertyMappingId(PropertyIdDiameter)]
		[RequiredPermission(Permission.Layout)]
		public int Size {
			get { return base.DiameterInternal; }
			set { base.DiameterInternal = value; }
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			Rectangle result = Geometry.InvalidRectangle;
			if (Size >= 0) {
				result.X = X - (int)Math.Round(Size / 2f);
				result.Y = Y - (int)Math.Round(Size / 2f);
				result.Width = result.Height = Size;
				if (Angle % 900 != 0) {
					Point tl, tr, bl, br;
					Geometry.RotateRectangle(result, Center, Geometry.TenthsOfDegreeToDegrees(Angle), out tl, out tr, out br, out bl);
					Geometry.CalcBoundingRectangle(tl, tr, bl, br, out result);
				}
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal SquareBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal SquareBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int left = (int)Math.Round(-Size / 2f);
			int top = (int)Math.Round(-Size / 2f);
			int right = left + Size;
			int bottom = top + Size;

			// top row (left to right)			
			ControlPoints[0].X = left;
			ControlPoints[0].Y = top;
			ControlPoints[1].X = 0;
			ControlPoints[1].Y = top;
			ControlPoints[2].X = right;
			ControlPoints[2].Y = top;

			// middle row (left to right)
			ControlPoints[3].X = left;
			ControlPoints[3].Y = 0;
			ControlPoints[4].X = right;
			ControlPoints[4].Y = 0;

			// bottom row (left to right)
			ControlPoints[5].X = left;
			ControlPoints[5].Y = bottom;
			ControlPoints[6].X = 0;
			ControlPoints[6].Y = bottom;
			ControlPoints[7].X = right;
			ControlPoints[7].Y = bottom;

			// rotate handle
			ControlPoints[8].X = 0;
			ControlPoints[8].Y = 0;
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			return Geometry.RectangleContainsPoint(X - DiameterInternal / 2, Y - DiameterInternal / 2, DiameterInternal, DiameterInternal, Geometry.TenthsOfDegreeToDegrees(Angle), x, y, true);
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			Rectangle rectangle = Rectangle.Empty;
			rectangle.X = x;
			rectangle.Y = y;
			rectangle.Width = width;
			rectangle.Height = height;

			if (Angle % 900 == 0) {
				Rectangle bounds = Rectangle.Empty;
				bounds.X = X - (Size / 2);
				bounds.Y = Y - (Size / 2);
				bounds.Width = bounds.Height = Size;
				return Geometry.RectangleIntersectsWithRectangle(rectangle, bounds);
			} else {
				if (rotatedBounds.Length != 4)
					Array.Resize<PointF>(ref rotatedBounds, 4);
				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);
				float ptX, ptY;
				float halfSize = Size / 2f;
				ptX = X - halfSize;		// left
				ptY = Y - halfSize;	// top
				Geometry.RotatePoint(X, Y, angle, ref ptX, ref ptY);
				rotatedBounds[0].X = ptX;
				rotatedBounds[0].Y = ptY;

				ptX = X + halfSize;		// right
				ptY = Y - halfSize;		// top
				Geometry.RotatePoint(X, Y, angle, ref ptX, ref ptY);
				rotatedBounds[1].X = ptX;
				rotatedBounds[1].Y = ptY;

				ptX = X + halfSize;		// right
				ptY = Y + halfSize;	// bottom
				Geometry.RotatePoint(X, Y, angle, ref ptX, ref ptY);
				rotatedBounds[2].X = ptX;
				rotatedBounds[2].Y = ptY;

				ptX = X - halfSize;		// left
				ptY = Y + halfSize;	// bottom
				Geometry.RotatePoint(X, Y, angle, ref ptX, ref ptY);
				rotatedBounds[3].X = ptX;
				rotatedBounds[3].Y = ptY;

				return Geometry.PolygonIntersectsWithRectangle(rotatedBounds, rectangle);
			}
		}


		/// <override></override>
		protected internal override int ControlPointCount {
			get { return 9; }
		}


		#region Fields
		private PointF[] rotatedBounds = new PointF[4];
		#endregion
	}


	/// <remarks>RequiredPermissions set</remarks>
	public abstract class CircleBase : DiameterShapeBase {

		/// <ToBeCompleted></ToBeCompleted>
		[Category("Layout")]
		[Description("Diameter of the circle.")]
		[PropertyMappingId(PropertyIdDiameter)]
		[RequiredPermission(Permission.Layout)]
		public int Diameter {
			get { return base.DiameterInternal; }
			set { base.DiameterInternal = value; }
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopLeftControlPoint:
				case TopRightControlPoint:
				case BottomLeftControlPoint:
				case BottomRightControlPoint:
					return (controlPointCapability & ControlPointCapabilities.Resize) != 0;
				case 10:
				case 11:
				case 12:
				case 13:
					return ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			Point p = Geometry.IntersectCircleWithLine(X, Y, (int)Math.Round(Diameter / 2f), startX, startY, X, Y, true);
			if (Geometry.IsValid(p)) return p;
			else return Center;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal CircleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal CircleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			Rectangle result = Geometry.InvalidRectangle;
			if (tight) {
				if (DiameterInternal >= 0) {
					// No need to rotate the tight bounding rectangle of a circle
					result.X = X - (int)Math.Round(Diameter / 2f);
					result.Y = Y - (int)Math.Round(Diameter / 2f);
					result.Width = result.Height = Diameter;
				}
			} else result = base.CalculateBoundingRectangle(tight);
			if (Geometry.IsValid(result))
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
			return result;
		}


		/// <override></override>
		protected internal override int ControlPointCount { 
			get { return 13; } 
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			return Geometry.CircleContainsPoint(X, Y, Diameter / 2f, x, y, 0);
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			Rectangle r = Rectangle.Empty;
			r.X = x;
			r.Y = y;
			r.Width = width;
			r.Height = height;
			return Geometry.CircleIntersectsWithRectangle(r, Center, Diameter / 2f);
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int left = (int)Math.Round(-Diameter / 2f);
			int top = (int)Math.Round(-Diameter / 2f);
			int right = left + Diameter;
			int bottom = top + Diameter;

			// Top left
			ControlPoints[0].X = left;
			ControlPoints[0].Y = top;
			// Top
			ControlPoints[1].X = 0;
			ControlPoints[1].Y = top;
			// Top right
			ControlPoints[2].X = right;
			ControlPoints[2].Y = top;
			// Left
			ControlPoints[3].X = left;
			ControlPoints[3].Y = 0;
			// Right
			ControlPoints[4].X = right;
			ControlPoints[4].Y = 0;
			// Bottom left
			ControlPoints[5].X = left;
			ControlPoints[5].Y = bottom;
			// Bottom
			ControlPoints[6].X = 0;
			ControlPoints[6].Y = bottom;
			// Bottom right
			ControlPoints[7].X = right;
			ControlPoints[7].Y = bottom;
			// Center
			ControlPoints[8].X = 0;
			ControlPoints[8].Y = 0;

			if (ControlPointCount > 9) {
				double angle = Geometry.DegreesToRadians(45);
				int dx = (int)Math.Round((Diameter / 2f) - ((Diameter / 2f) * Math.Cos(angle)));
				int dy = (int)Math.Round((Diameter / 2f) - ((Diameter / 2f) * Math.Sin(angle)));
				// Top left
				ControlPoints[9].X = left + dx;
				ControlPoints[9].Y = top + dy;
				// Top right
				ControlPoints[10].X = right - dx;
				ControlPoints[10].Y = top + dy;
				// Bottom left
				ControlPoints[11].X = left + dx;
				ControlPoints[11].Y = bottom - dy;
				// Bottom right
				ControlPoints[12].X = right - dx;
				ControlPoints[12].Y = bottom - dy;
			}
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			captionBounds = Rectangle.Empty;
			captionBounds.X = (int)Math.Round((-Diameter / 2f) + (Diameter / 8f));
			captionBounds.Y = (int)Math.Round((-Diameter / 2f) + (Diameter / 8f));
			captionBounds.Width = (int)Math.Round(Diameter - (Diameter / 4f));
			captionBounds.Height = (int)Math.Round(Diameter - (Diameter / 4f));
		}


		// ControlPoint Id Constants
		private const int TopLeftControlPoint = 1;
		private const int TopRightControlPoint = 3;
		private const int BottomLeftControlPoint = 6;
		private const int BottomRightControlPoint = 8;
	}

}