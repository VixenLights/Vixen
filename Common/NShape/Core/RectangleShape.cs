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
using System.Drawing.Drawing2D;


namespace Dataweb.NShape.Advanced {

	/// <remarks>RequiredPermissions set</remarks>
	public abstract class RectangleBase : CaptionedShapeBase {

		#region Properties

		/// <ToBeCompleted></ToBeCompleted>
		[Category("Layout")]
		[Description("The horizontal size of the Shape.")]
		[PropertyMappingId(PropertyIdWidth)]
		[RequiredPermission(Permission.Layout)]
		public virtual int Width {
			get { return size.Width; }
			set {
				if (value < 0) throw new ArgumentOutOfRangeException("Width");
				if (value != size.Width) {
					Invalidate();
					//
					if (Owner != null) Owner.NotifyChildResizing(this);
					int delta = value - size.Width;
					//
					size.Width = value;
					InvalidateDrawCache();
					//
					if (ChildrenCollection != null) ChildrenCollection.NotifyParentSized(delta, 0);
					if (Owner != null) Owner.NotifyChildResized(this);
					ControlPointsHaveMoved();
					Invalidate();
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Layout")]
		[Description("The vertical size of the Shape.")]
		[PropertyMappingId(PropertyIdHeight)]
		[RequiredPermission(Permission.Layout)]
		public virtual int Height {
			get { return size.Height; }
			set {
				if (value < 0) throw new ArgumentOutOfRangeException("Height");
				if (value != size.Height) {
					Invalidate();
					//
					if (Owner != null) Owner.NotifyChildResizing(this);
					int delta = value - size.Height;
					//
					size.Height = value;
					InvalidateDrawCache();
					//
					if (ChildrenCollection != null) ChildrenCollection.NotifyParentSized(0, delta);
					if (Owner != null) Owner.NotifyChildResized(this);
					ControlPointsHaveMoved();
					Invalidate();
				}
			}
		}


		/// <override></override>
		[Browsable(false)]
		protected internal override int ControlPointCount {
			get { return 9; }
		}

		#endregion


		/// <override></override>
		public override void CopyFrom(Shape source) {
			base.CopyFrom(source);
			if (source is RectangleBase) {
				size.Width = ((RectangleBase)source).Width;
				size.Height = ((RectangleBase)source).Height;
			} else {
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
				if (Geometry.IsValid(srcBounds))
					size = srcBounds.Size;
			}

		}


		/// <override></override>
		public override Point CalculateAbsolutePosition(RelativePosition relativePosition) {
			if (relativePosition == RelativePosition.Empty) throw new ArgumentOutOfRangeException("relativePosition");
			// The RelativePosition of a RectangleBased shape is:
			// A = Tenths of percent of Width
			// B = Tenths of percent of Height
			Point result = Point.Empty;
			result.X = (int)Math.Round((X - (Width / 2f)) + (relativePosition.A * (Width / 1000f)));
			result.Y = (int)Math.Round((Y - (Height / 2f)) + (relativePosition.B * (Height / 1000f)));
			if (Angle != 0) result = Geometry.RotatePoint(Center, Geometry.TenthsOfDegreeToDegrees(Angle), result);
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
			result.A = (int)Math.Round((x - (X - (Width / 2f))) / (Width / 1000f));
			result.B = (int)Math.Round((y - (Y - (Height / 2f))) / (Height / 1000f));
			return result;
		}


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
		public override Point CalculateConnectionFoot(int startX, int startY) {
			// Set result to a default return value
			Point result = Point.Empty;
			result.Offset(X, Y);
			
			float currDist, dist = float.MaxValue;
			float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
			int x1, y1, x2, y2;
			Point p;

			int left = (int)Math.Round(X - (Width / 2f));
			int top = (int)Math.Round(Y - (Height / 2f));
			int right = left + Width;
			int bottom = top + Height;
			// Check top side for intersection
			x1 = left; y1 = top;
			x2 = right; y2 = top;
			Geometry.RotatePoint(X, Y, angleDeg, ref x1, ref y1);
			Geometry.RotatePoint(X, Y, angleDeg, ref x2, ref y2);
			p = Geometry.IntersectLineWithLineSegment(startX, startY, X, Y, x1, y1, x2, y2);
			if (Geometry.IsValid(p)) {
				currDist = Geometry.DistancePointPoint(p.X, p.Y, startX, startY);
				if (currDist < dist) {
					dist = currDist;
					result = p;
				}
			}
			// check right side for intersection
			x1 = right; y1 = top;
			x2 = right; y2 = bottom;
			Geometry.RotatePoint(X, Y, angleDeg, ref x1, ref y1);
			Geometry.RotatePoint(X, Y, angleDeg, ref x2, ref y2);
			p = Geometry.IntersectLineWithLineSegment(startX, startY, X, Y, x1, y1, x2, y2);
			if (Geometry.IsValid(p)) {
				currDist = Geometry.DistancePointPoint(p.X, p.Y, startX, startY);
				if (currDist < dist) {
					dist = currDist;
					result = p;
				}
			}
			// check bottom side for intersection
			x1 = right; y1 = bottom;
			x2 = left; y2 = bottom;
			Geometry.RotatePoint(X, Y, angleDeg, ref x1, ref y1);
			Geometry.RotatePoint(X, Y, angleDeg, ref x2, ref y2);
			p = Geometry.IntersectLineWithLineSegment(startX, startY, X, Y, x1, y1, x2, y2);
			if (Geometry.IsValid(p)) {
				currDist = Geometry.DistancePointPoint(p.X, p.Y, startX, startY);
				if (currDist < dist) {
					dist = currDist;
					result = p;
				}
			}
			// check left side for intersection
			x1 = left; y1 = bottom;
			x2 = left; y2 = top;
			Geometry.RotatePoint(X, Y, angleDeg, ref x1, ref y1);
			Geometry.RotatePoint(X, Y, angleDeg, ref x2, ref y2);
			p = Geometry.IntersectLineWithLineSegment(startX, startY, X, Y, x1, y1, x2, y2);
			if (Geometry.IsValid(p)) {
				currDist = Geometry.DistancePointPoint(p.X, p.Y, startX, startY);
				if (currDist < dist) {
					dist = currDist;
					result = p;
				}
			}
			return result;
		}


		/// <override></override>
		public override void Fit(int x, int y, int width, int height) {
			// Calculate bounds (including children)
			Rectangle bounds = GetBoundingRectangle(true);
			// Calculate the shape's offset relative to the bounds
			float offsetX = (X - this.Width / 2f) - bounds.X;
			float offsetY = (Y - this.Height / 2f) - bounds.Y;
			// Calculate the scaling factor and the new position
			float scale = Geometry.CalcScaleFactor(bounds.Width, bounds.Height, width, height);
			float dstX = x + (width / 2f) + (offsetX * scale);
			float dstY = y + (height / 2f) + (offsetY * scale);
			// Move to new position and apply scaling
			MoveTo((int)Math.Round(dstX), (int)Math.Round(dstY));
			Width = (int)Math.Round(this.Width * scale);
			Height = (int)Math.Round(this.Height * scale);
		}


		/// <override></override>
		public override void Draw(Graphics graphics) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			DrawPath(graphics, LineStyle, FillStyle);
			DrawCaption(graphics);
			base.Draw(graphics);
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			// Tight and loose bounding rectangles are equal
			Rectangle result = Geometry.InvalidRectangle;
			if (Width >= 0 && Height >= 0) {
				if (Angle == 0 || Angle == 1800) {
					result.X = X - (int)Math.Round(Width / 2f);
					result.Y = Y - (int)Math.Round(Height / 2f);
					result.Width = Width;
					result.Height = Height;
				} else if (Angle == 900 || Angle == 2700) {
					result.X = X - (int)Math.Round(Height / 2f);
					result.Y = Y - (int)Math.Round(Width / 2f);
					result.Width = Height;
					result.Height = Width;
				} else {
					float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
					int x1, y1, x2, y2, x3, y3, x4, y4;
					int left = (int)Math.Round(X - (Width / 2f));
					int top = (int)Math.Round(Y - (Height / 2f));
					int right = left + Width;
					int bottom = top + Height;

					x1 = left; y1 = top;
					x2 = right; y2 = top;
					x3 = right; y3 = bottom;
					x4 = left; y4 = bottom;
					Geometry.RotatePoint(X, Y, angleDeg, ref x1, ref y1);
					Geometry.RotatePoint(X, Y, angleDeg, ref x2, ref y2);
					Geometry.RotatePoint(X, Y, angleDeg, ref x3, ref y3);
					Geometry.RotatePoint(X, Y, angleDeg, ref x4, ref y4);

					result.X = Math.Min(Math.Min(x1, x2), Math.Min(x3, x4));
					result.Y = Math.Min(Math.Min(y1, y2), Math.Min(y3, y4));
					result.Width = Math.Max(Math.Max(x1, x2), Math.Max(x3, x4)) - result.X;
					result.Height = Math.Max(Math.Max(y1, y2), Math.Max(y3, y4)) - result.Y;
				}
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
			}
			return result;
		}


		/// <override></override>
		protected internal RectangleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal RectangleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			size.Width = 60;
			size.Height = 40;
		}


		/// <override></override>
		protected override int DivFactorX { get { return 2; } }


		/// <override></override>
		protected override int DivFactorY { get { return 2; } }


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			return Geometry.RectangleContainsPoint(X - (Width / 2), Y - (Height / 2), Width, Height, Geometry.TenthsOfDegreeToDegrees(Angle), x, y, true);
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			if (Angle % 900 == 0) {
				Rectangle r = GetBoundingRectangle(true);
				if (Geometry.RectangleIntersectsWithRectangle(r, x, y, width, height)
					|| Geometry.RectangleContainsRectangle(r, x, y, width, height))
					return true;
			} else {
				if (rotatedBounds.Length != 4)
					Array.Resize<PointF>(ref rotatedBounds, 4);
				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);
				float ptX, ptY;
				ptX = X - (Width / 2f);		// left
				ptY = Y - (Height / 2f);	// top
				Geometry.RotatePoint(X, Y, angle, ref ptX, ref ptY);
				rotatedBounds[0].X = ptX;
				rotatedBounds[0].Y = ptY;

				ptX = X + (Width / 2f);		// right
				ptY = Y - (Height / 2f);	// top
				Geometry.RotatePoint(X, Y, angle, ref ptX, ref ptY);
				rotatedBounds[1].X = ptX;
				rotatedBounds[1].Y = ptY;

				ptX = X + (Width / 2f);		// right
				ptY = Y + (Height / 2f);	// bottom
				Geometry.RotatePoint(X, Y, angle, ref ptX, ref ptY);
				rotatedBounds[2].X = ptX;
				rotatedBounds[2].Y = ptY;

				ptX = X - (Width / 2f);		// left
				ptY = Y + (Height / 2f);	// bottom
				Geometry.RotatePoint(X, Y, angle, ref ptX, ref ptY);
				rotatedBounds[3].X = ptX;
				rotatedBounds[3].Y = ptY;

				Rectangle rectangle = Rectangle.Empty;
				rectangle.Offset(x, y);
				rectangle.Width = width;
				rectangle.Height = height;
				if (Geometry.PolygonIntersectsWithRectangle(rotatedBounds, rectangle))
					return true;
			}
			return false;
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers) {
			bool result = true;
			int dx = 0, dy = 0;
			int width = size.Width; int height = size.Height;
			int newWidth = -1, newHeight = -1;
			switch (pointId) {
				case TopLeftControlPoint:
					if (!Geometry.MoveRectangleTopLeft(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out newWidth, out newHeight))
						result = false;
					break;
				case TopCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleTop(width, height, 0, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out newWidth, out newHeight))
						result = false;
					break;
				case TopRightControlPoint:
					if (!Geometry.MoveRectangleTopRight(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out newWidth, out newHeight))
						result = false;
					break;
				case MiddleLeftControlPoint:
					result = (transformedDeltaY == 0);
					if (!Geometry.MoveRectangleLeft(width, height, transformedDeltaX, 0, cos, sin, modifiers, out dx, out dy, out newWidth, out newHeight))
						result = false;
					break;
				case MiddleRightControlPoint:
					result = (transformedDeltaY == 0);
					if (!Geometry.MoveRectangleRight(width, height, transformedDeltaX, 0, cos, sin, modifiers, out dx, out dy, out newWidth, out newHeight))
						result = false;
					break;
				case BottomLeftControlPoint:
					if (!Geometry.MoveRectangleBottomLeft(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out newWidth, out newHeight))
						result = false;
					break;
				case BottomCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleBottom(width, height, 0, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out newWidth, out newHeight))
						result = false;
					break;
				case BottomRightControlPoint:
					if (!Geometry.MoveRectangleBottomRight(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out newWidth, out newHeight))
						result = false;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
			System.Diagnostics.Debug.Assert(newWidth >= 0 && newHeight >= 0);
			// Perform Resizing
			size.Width = newWidth;
			size.Height = newHeight;
			MoveByCore(dx, dy);

			return result;
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

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
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			captionBounds = Rectangle.Empty;
			captionBounds.X = (int)Math.Round(-Width / 2f);
			captionBounds.Y = (int)Math.Round(-Height / 2f);
			captionBounds.Width = Width;
			captionBounds.Height = Height;
		}


		/// <override></override>
		protected override void ProcessExecModelPropertyChange(IModelMapping propertyMapping) {
			switch (propertyMapping.ShapePropertyId) {
				case PropertyIdWidth:
					Width = propertyMapping.GetInteger();
					break;
				case PropertyIdHeight :
					Height = propertyMapping.GetInteger();
					break;
				default:
					base.ProcessExecModelPropertyChange(propertyMapping);
					break;
			}
		}


		#region IEntity Members

		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.RectangleBase" />.
		/// </summary>
		new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
			foreach (EntityPropertyDefinition pi in CaptionedShapeBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("Width", typeof(int));
			yield return new EntityFieldDefinition("Height", typeof(int));
		}


		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version) {
			base.LoadFieldsCore(reader, version);
			Width = reader.ReadInt32();
			Height = reader.ReadInt32();
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version) {
			base.SaveFieldsCore(writer, version);
			writer.WriteInt32(Width);
			writer.WriteInt32(Height);
		}

		#endregion


		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdWidth = 7;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdHeight = 8;

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

		private Size size = Size.Empty;
		private PointF[] rotatedBounds = new PointF[4];

		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public abstract class EllipseBase : RectangleBase {

		/// <override></override>
		protected internal override int ControlPointCount { get { return 13; } }


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
			Point result = Geometry.GetNearestPoint(startX, startY, Geometry.IntersectEllipseLine(X, Y, Width, Height, Geometry.TenthsOfDegreeToDegrees(Angle), startX, startY, X, Y, false));
			if (!Geometry.IsValid(result)) return Center;
			else return result;
		}


		/// <override></override>
		protected internal EllipseBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal EllipseBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			Rectangle rectangle = Rectangle.Empty;
			rectangle.Offset(x, y);
			rectangle.Width = width;
			rectangle.Height = height;
			Rectangle boundingRect = GetBoundingRectangle(false);
			if (Geometry.RectangleContainsRectangle(rectangle, boundingRect) 
				|| Geometry.RectangleContainsRectangle(boundingRect, rectangle) 
				|| Geometry.RectangleIntersectsWithRectangle(boundingRect, rectangle))
				return Geometry.EllipseIntersectsWithRectangle(X, Y, Width, Height, Geometry.TenthsOfDegreeToDegrees(Angle), rectangle);
			else return false;
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			if (!Geometry.IsValid(x, y)) return false;
			return Geometry.EllipseContainsPoint(X, Y, Width, Height, Geometry.TenthsOfDegreeToDegrees(Angle), x, y);
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				Rectangle result = Rectangle.Empty;
				if (Width >= 0 && Height >= 0) {
					if (Angle == 0 || Angle == 1800) {
						result.X = X - (Width / 2);
						result.Y = Y - (Height / 2);
						result.Width = Width;
						result.Height = Height;
					} else if (Angle == 900 || Angle == 2700) {
						result.X = X - (Height / 2);
						result.Y = Y - (Width / 2);
						result.Width = Height;
						result.Height = Width;
					} else
						Geometry.CalcBoundingRectangleEllipse(X, Y, Width, Height, Geometry.TenthsOfDegreeToDegrees(Angle), out result);
					ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				}
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			double angle = Geometry.DegreesToRadians(45);
			int dx = (int)Math.Round((Width / 2f) - ((Width / 2f) * Math.Cos(angle)));
			int dy = (int)Math.Round((Height / 2f) - ((Height / 2f) * Math.Sin(angle)));
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;
			// top left
			ControlPoints[0].X = left;
			ControlPoints[0].Y = top;
			// top
			ControlPoints[1].X = 0;
			ControlPoints[1].Y = top;
			// top right
			ControlPoints[2].X = right;
			ControlPoints[2].Y = top;
			// left
			ControlPoints[3].X = left;
			ControlPoints[3].Y = 0;
			// right
			ControlPoints[4].X = right;
			ControlPoints[4].Y = 0;
			// bottom left
			ControlPoints[5].X = left;
			ControlPoints[5].Y = bottom;
			// bottom
			ControlPoints[6].X = 0;
			ControlPoints[6].Y = bottom;
			// bottom right
			ControlPoints[7].X = right;
			ControlPoints[7].Y = bottom;

			ControlPoints[8].X = 0;
			ControlPoints[8].Y = 0;

			// top left
			ControlPoints[9].X = left + dx;
			ControlPoints[9].Y = top + dy;
			// top right
			ControlPoints[10].X = right - dx;
			ControlPoints[10].Y = top + dy;
			// bottom left
			ControlPoints[11].X = left + dx;
			ControlPoints[11].Y = bottom - dy;
			// bottom right
			ControlPoints[12].X = right - dx;
			ControlPoints[12].Y = bottom - dy;
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			captionBounds = Rectangle.Empty;
			captionBounds.X = (int)Math.Round((-Width / 2f) + (Width / 8f));
			captionBounds.Y = (int)Math.Round((-Height / 2f) + (Height / 8f));
			captionBounds.Width = (int)Math.Round(Width - (Width / 4f));
			captionBounds.Height = (int)Math.Round(Height - (Height / 4f));
		}


		#region Fields

		// ControlPoint Id Constants
		private const int TopLeftControlPoint = 1;
		private const int TopRightControlPoint = 3;
		private const int BottomLeftControlPoint = 6;
		private const int BottomRightControlPoint = 8;

		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public abstract class DiamondBase : RectangleBase {

		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			Point result = Point.Empty;
			// Calculate current (unrotated) position of the shape's points
			int left = (int)Math.Round(X - (Width / 2f));
			int top = (int)Math.Round(Y - (Height / 2f));
			int right = left + Width;
			int bottom = top + Height;
			pointBuffer[0].X = X;
			pointBuffer[0].Y = top;
			pointBuffer[1].X = right;
			pointBuffer[1].Y = Y;
			pointBuffer[2].X = X;
			pointBuffer[2].Y = bottom;
			pointBuffer[3].X = left;
			pointBuffer[3].Y = Y;
			// Rotate shape points if necessary
			if (Angle % 1800 != 0) {
				Matrix.Reset();
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center);
				Matrix.TransformPoints(pointBuffer);
			}
			// Calculate intersection points and return the nearest (or the shape's Center if there is no intersection point)
			result = Geometry.GetNearestPoint(startX, startY, Geometry.IntersectPolygonLine(pointBuffer, startX, startY, X, Y, true));
			if (!Geometry.IsValid(result)) return Center;
			else return result;
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
		protected internal override int ControlPointCount {
			get { return 13; }
		}


		/// <override></override>
		protected internal DiamondBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal DiamondBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				Rectangle result = Rectangle.Empty;
				if (Width >= 0 && Height >= 0) {
					CalcTransformedShapePoints();
					result.X = Math.Min(Math.Min(pointBuffer[0].X, pointBuffer[1].X), Math.Min(pointBuffer[2].X, pointBuffer[3].X));
					result.Y = Math.Min(Math.Min(pointBuffer[0].Y, pointBuffer[1].Y), Math.Min(pointBuffer[2].Y, pointBuffer[3].Y));
					result.Width = Math.Max(Math.Max(pointBuffer[0].X, pointBuffer[1].X), Math.Max(pointBuffer[2].X, pointBuffer[3].X)) - result.X;
					result.Height = Math.Max(Math.Max(pointBuffer[0].Y, pointBuffer[1].Y), Math.Max(pointBuffer[2].Y, pointBuffer[3].Y)) - result.Y;
					ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				}
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			Rectangle rectangle = Rectangle.Empty;
			rectangle.X = x;
			rectangle.Y = y;
			rectangle.Width = width;
			rectangle.Height = height;

			CalcTransformedShapePoints();
			return Geometry.PolygonIntersectsWithRectangle(pointBuffer, rectangle);
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			CalcTransformedShapePoints();
			return Geometry.QuadrangleContainsPoint(pointBuffer[0], pointBuffer[1], pointBuffer[2], pointBuffer[3], x, y);
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			float insetX = Width / 8f;
			float insetY = Height / 8f;
			captionBounds = Rectangle.Empty;
			captionBounds.X = (int)Math.Round((-Width / 2f) + insetX);
			captionBounds.Y = (int)Math.Round((-Height / 2f) + insetY);
			captionBounds.Width = (int)Math.Round(Width - insetX - insetX);
			captionBounds.Height = (int)Math.Round(Height - insetY - insetY);
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			// top left
			ControlPoints[0].X = left;
			ControlPoints[0].Y = top;
			// top
			ControlPoints[1].X = 0;
			ControlPoints[1].Y = top;
			// top right
			ControlPoints[2].X = right;
			ControlPoints[2].Y = top;
			// left
			ControlPoints[3].X = left;
			ControlPoints[3].Y = 0;
			// right
			ControlPoints[4].X = right;
			ControlPoints[4].Y = 0;
			// bottom left
			ControlPoints[5].X = left;
			ControlPoints[5].Y = bottom;
			// bottom
			ControlPoints[6].X = 0;
			ControlPoints[6].Y = bottom;
			// bottom right
			ControlPoints[7].X = right;
			ControlPoints[7].Y = bottom;
			// rotate
			ControlPoints[8].X = 0;
			ControlPoints[8].Y = 0;

			if (ControlPoints.Length > 9) {
				int dX = (int)Math.Round(Width / 4f);
				int dY = (int)Math.Round(Height / 4f);

				// top left side
				ControlPoints[9].X = left + dX;
				ControlPoints[9].Y = top + dY;

				// top right side
				ControlPoints[10].X = right - dX;
				ControlPoints[10].Y = top + dY;

				// bottom left side
				ControlPoints[11].X = left + dX;
				ControlPoints[11].Y = bottom - dY;

				// bottom right side
				ControlPoints[12].X = right - dX;
				ControlPoints[12].Y = bottom - dY;
			}
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;

				pointBuffer[0].X = 0;
				pointBuffer[0].Y = top;
				pointBuffer[1].X = right;
				pointBuffer[1].Y = 0;
				pointBuffer[2].X = 0;
				pointBuffer[2].Y = bottom;
				pointBuffer[3].X = left;
				pointBuffer[3].Y = 0;

				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(pointBuffer);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		private void CalcTransformedShapePoints() {
			int left = (int)Math.Round(X - (Width / 2f));
			int top = (int)Math.Round(Y - (Height / 2f));
			int right = left + Width;
			int bottom = top + Height;
			pointBuffer[0].X = X;
			pointBuffer[0].Y = top;
			pointBuffer[1].X = right;
			pointBuffer[1].Y = Y;
			pointBuffer[2].X = X;
			pointBuffer[2].Y = bottom;
			pointBuffer[3].X = left;
			pointBuffer[3].Y = Y;
			if (Angle != 0) {
				Matrix.Reset();
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center);
				Matrix.TransformPoints(pointBuffer);
			}
		}


		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected Point[] pointBuffer = new Point[4];

		// ControlPoint Id Constants
		private const int TopLeftControlPoint = 1;
		private const int TopRightControlPoint = 3;
		private const int BottomLeftControlPoint = 6;
		private const int BottomRightControlPoint = 8;

		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public abstract class IsoscelesTriangleBase : RectangleBase {

		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Width = 40;
			Height = 40;
		}


		/// <override></override>
		protected internal override int ControlPointCount {
			get { return 5; }
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case ControlPointId.Reference:
				case RotatePointControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Reference) != 0
								|| (controlPointCapability & ControlPointCapabilities.Rotate) != 0
								|| ((controlPointCapability & ControlPointCapabilities.Connect) != 0
										&& IsConnectionPointEnabled(controlPointId)));
				default:
					if (controlPointId > 0 && controlPointId < RotatePointControlPoint)
						return ((controlPointCapability & ControlPointCapabilities.Resize) != 0
									|| ((controlPointCapability & ControlPointCapabilities.Connect) != 0
											&& IsConnectionPointEnabled(controlPointId)));
					else
						return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			Rectangle boundingRect = GetBoundingRectangle(true);
			int x = boundingRect.X + (int)Math.Round(boundingRect.Width / 2f);
			int y = boundingRect.Y + (int)Math.Round(boundingRect.Height / 2f);
			Point result = Geometry.InvalidPoint;
			CalculateTranslatedShapePoints();
			result = Geometry.GetNearestPoint(startX, startY, Geometry.IntersectPolygonLine(shapePoints, startX, startY, x, y, true));
			if (!Geometry.IsValid(result)) result = Center;
			return result;
		}


		/// <override></override>
		public override void Fit(int x, int y, int width, int height) {
			float scale = Geometry.CalcScaleFactor(Width, Height, width, height);
			this.Width = (int)Math.Floor(Width * scale);
			this.Height = (int)Math.Floor(Height * scale);
			MoveControlPointTo(ControlPointId.Reference, x + (int)Math.Round(width * CenterPosFactorX), y + (int)Math.Round(height * CenterPosFactorY), ResizeModifiers.None);
		}


		/// <override></override>
		protected internal IsoscelesTriangleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal IsoscelesTriangleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			CalculateTranslatedShapePoints();
			return Geometry.TriangleContainsPoint(shapePoints[0], shapePoints[1], shapePoints[2], x, y);
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			Rectangle r = Rectangle.Empty;
			r.Offset(x, y);
			r.Width=width;
			r.Height = height;
			CalculateTranslatedShapePoints();
			return Geometry.PolygonIntersectsWithRectangle(shapePoints, r);
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers) {
			bool result = true;

			int dx = 0, dy = 0;
			int width = Width;
			int height = Height;
			switch (pointId) {
				case TopCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleTop(width, height, 0, centerPosFactorX, centerPosFactorY, DivFactorX, DivFactorY, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
						result = false;
					break;

				case BottomLeftControlPoint:
					if (!Geometry.MoveRectangleBottomLeft(width, height, 0, 0, centerPosFactorX, centerPosFactorY, DivFactorX, DivFactorY, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
						result = false;
					break;

				case BottomRightControlPoint:
					if (!Geometry.MoveRectangleBottomRight(width, height, 0, 0, centerPosFactorX, centerPosFactorY, DivFactorX, DivFactorY, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
						result = false;
					break;

				case BottomCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleBottom(width, height, 0, centerPosFactorX, centerPosFactorY, DivFactorX, DivFactorY, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
						result = false;
					break;

				default:
					break;
			}
			Width = width;
			Height = height;
			MoveByCore(dx, dy);
			ControlPointsHaveMoved();

			return result;
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height * centerPosFactorY);
			int right = left + Width;
			int bottom = (int)Math.Round(Height - (Height * centerPosFactorY));

			ControlPoints[0].X = 0;
			ControlPoints[0].Y = top;

			ControlPoints[1].X = left;
			ControlPoints[1].Y = bottom;

			ControlPoints[2].X = 0;
			ControlPoints[2].Y = bottom;

			ControlPoints[3].X = right;
			ControlPoints[3].Y = bottom;

			ControlPoints[4].X = 0;
			ControlPoints[4].Y = 0;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			// Tight and loose fitting bounding rectangles are equal
			Rectangle result = Geometry.InvalidRectangle;
			if (Width >= 0 && Height >= 0) {
				int left = (int)Math.Round(X - (Width / 2f));
				int top = Y - (int)Math.Round(Height * CenterPosFactorY);
				int right = left + Width;
				int bottom = top + Height;
				// If shape's angle is a multiple of 90°, the calculation of the bounding rectangle is easy (and fast)
				if (Angle == 0) {
					result.X = left;
					result.Y = top;
					result.Width = Width;
					result.Height = Height;
				} else if (Angle == 900) {
					result.X = X - (int)Math.Round(Height * (1 - CenterPosFactorY));
					result.Y = Y - (int)Math.Round(Width * (1 - CenterPosFactorX));
					result.Width = Height;
					result.Height = Width;
				} else if (Angle == 1800) {
					result.X = X - (int)Math.Round(Width * (1 - CenterPosFactorX)); ;
					result.Y = Y - (int)Math.Round(Height * (1 - CenterPosFactorY)); ;
					result.Width = Width;
					result.Height = Height;
				} else if (Angle == 2700) {
					result.X = X - (int)Math.Round(Height * CenterPosFactorY);
					result.Y = Y - (int)Math.Round(Width * CenterPosFactorX);
					result.Width = Height;
					result.Height = Width;
				} else {
					CalculateTranslatedShapePoints();
					Geometry.CalcBoundingRectangle(shapePoints[0], shapePoints[1], shapePoints[2], shapePoints[0], out result);
				}
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
			}
			return result;
		}


		/// <summary>
		/// Calculated the untransformed shape points of the triangle
		/// </summary>
		protected virtual void CalculateShapePoints() {
			int left = (int)Math.Round(-Width * centerPosFactorX);
			int top = (int)Math.Round(-Height * centerPosFactorY);
			int right = left + Width;
			int bottom = top + Height;

			shapePoints[0].X = 0;
			shapePoints[0].Y = top;
			shapePoints[1].X = left;
			shapePoints[1].Y = bottom;
			shapePoints[2].X = right;
			shapePoints[2].Y = bottom;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void CalculateTranslatedShapePoints() {
			CalculateShapePoints();
			Matrix.Reset();
			Matrix.Translate(X, Y);
			if (Angle != 0) Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center, MatrixOrder.Append);
			Matrix.TransformPoints(shapePoints);
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				CalculateShapePoints();

				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		/// <override></override>
		protected override int DivFactorY { get { return 3; } }


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual float CenterPosFactorX { get { return centerPosFactorX; } }


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual float CenterPosFactorY { get { return centerPosFactorY; } }


		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected Point[] shapePoints = new Point[3];
		
		private const float centerPosFactorX = 0.5f;
		private const float centerPosFactorY = 0.66666666f;

		private const int TopCenterControlPoint = 1;
		private const int BottomLeftControlPoint = 2;
		private const int BottomCenterControlPoint = 3;
		private const int BottomRightControlPoint = 4;
		private const int RotatePointControlPoint = 5;

		#endregion
	}

}
