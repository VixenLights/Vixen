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
using System.Drawing;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.GeneralShapes {

	public class Square : SquareBase {

		internal static Shape CreateInstance(ShapeType shapeType, Template template) {
			return new Square(shapeType, template);
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new Square(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal Square(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal Square(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Size / 2f);
				int top = (int)Math.Round(-Size / 2f);

				shapeRect.X = left;
				shapeRect.Y = top;
				shapeRect.Width = Size;
				shapeRect.Height = Size;

				Path.Reset();
				Path.StartFigure();
				Path.AddRectangle(shapeRect);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		#region Fields
		Rectangle shapeRect;
		#endregion
	}


	public class Box : RectangleBase {

		internal static Shape CreateInstance(ShapeType shapeType, Template template) {
			return new Box(shapeType, template);
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new Box(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal Box(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal Box(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();

				Rectangle shapeRect = Rectangle.Empty;
				shapeRect.Offset((int)Math.Round(-Width / 2f), (int)Math.Round(-Height / 2f));
				shapeRect.Width = Width;
				shapeRect.Height = Height;

				Path.StartFigure();
				Path.AddRectangle(shapeRect);
				Path.CloseFigure();
				return true;
			} else return false;
		}
	}


	public class RoundedBox : RectangleBase {

		internal static Shape CreateInstance(ShapeType shapeType, Template template) {
			return new RoundedBox(shapeType, template);
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new RoundedBox(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			Point result = base.CalculateConnectionFoot(startX, startY);
			int radius = CalcCornerRadius();

			if (Angle == 0) {
				int left = (int)Math.Round(X - (Width / 2f));
				int top = (int)Math.Round(Y - (Height / 2f));
				int right = left + Width;
				int bottom = top + Height;
				Point p = Geometry.InvalidPoint;
				if (left <= result.X && result.X <= left + radius && top <= result.Y && result.Y <= top + radius) {
					// Check TopLeft corner
					p = Geometry.IntersectCircleWithLine(left + radius, top + radius, radius, startX, startY, X, Y, false);
					if (Geometry.IsValid(p)) result = p;
				} else if (right - radius <= result.X && result.X <= right && top <= result.Y && result.Y <= top + radius) {
					// Check TopRight corner
					p = Geometry.IntersectCircleWithLine(right - radius, top + radius, radius, startX, startY, X, Y, false);
					if (Geometry.IsValid(p)) result = p;
				} else if (left <= result.X && result.X <= left + radius && bottom - radius <= result.Y && result.Y <= bottom) {
					// Check BottomLeft corner
					p = Geometry.IntersectCircleWithLine(left + radius, bottom - radius, radius, startX, startY, X, Y, false);
					if (Geometry.IsValid(p)) result = p;
				} else if (right - radius <= result.X && result.X <= right && bottom - radius <= result.Y && result.Y <= bottom) {
					// Check BottomRight corner
					p = Geometry.IntersectCircleWithLine(right - radius, bottom - radius, radius, startX, startY, X, Y, false);
					if (Geometry.IsValid(p)) result = p;
				}
			} else {
				// Check the top and bottom side (between the rounded corners:
				// If the line intersects with any of these sides, we need not calculate the rounded corner intersection
				int cornerRadius = CalcCornerRadius();
				float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
				if (!Geometry.RectangleIntersectsWithLine(X, Y, Width - (2 * cornerRadius), Height, angleDeg, startX, startY, result.X, result.Y, true)
					&& !Geometry.RectangleIntersectsWithLine(X, Y, Width, Height - (2 * cornerRadius), angleDeg, startX, startY, result.X, result.Y, true)) {
					// If there is no intersection with any of the straight sides, check the rounded corners:

					// Calculate all center points of all corner roundings
					PointF topLeft = PointF.Empty, topRight = PointF.Empty, bottomRight = PointF.Empty, bottomLeft = PointF.Empty;
					RectangleF rect = RectangleF.Empty;
					rect.X = X - (Width / 2f);
					rect.Y = Y - (Height / 2f);
					rect.Width = Width;
					rect.Height = Height;
					rect.Inflate(-cornerRadius, -cornerRadius);
					Geometry.RotateRectangle(rect, X, Y, angleDeg, out topLeft, out topRight, out bottomRight, out bottomLeft);

					// Check corner roundings for intersection with the calculated line
					PointF p = Geometry.InvalidPointF;
					if (Geometry.CircleIntersectsWithLine(topLeft.X, topLeft.Y, cornerRadius, startX, startY, X, Y, false)) {
						p = Geometry.IntersectCircleWithLine(topLeft.X, topLeft.Y, cornerRadius, startX, startY, X, Y, false);
						if (Geometry.IsValid(p)) result = Point.Round(p);
					} else if (Geometry.CircleIntersectsWithLine(topRight.X, topRight.Y, cornerRadius, startX, startY, X, Y, false)) {
						p = Geometry.IntersectCircleWithLine(topRight.X, topRight.Y, cornerRadius, startX, startY, X, Y, false);
						if (Geometry.IsValid(p)) result = Point.Round(p);
					} else if (Geometry.CircleIntersectsWithLine(bottomRight.X, bottomRight.Y, cornerRadius, startX, startY, X, Y, false)) {
						p = Geometry.IntersectCircleWithLine(bottomRight.X, bottomRight.Y, cornerRadius, startX, startY, X, Y, false);
						if (Geometry.IsValid(p)) result = Point.Round(p);
					} else if (Geometry.CircleIntersectsWithLine(bottomLeft.X, bottomLeft.Y, cornerRadius, startX, startY, X, Y, false)) {
						p = Geometry.IntersectCircleWithLine(bottomLeft.X, bottomLeft.Y, cornerRadius, startX, startY, X, Y, false);
						if (Geometry.IsValid(p)) result = Point.Round(p);
					}
				}
			}
			return result;
		}


		protected internal RoundedBox(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal RoundedBox(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				if (Angle % 900 == 0) return base.CalculateBoundingRectangle(tight);
				else {
					Rectangle result = Geometry.InvalidRectangle;
					// Calculate the minimum bounding box
					int cornerRadius = CalcCornerRadius();
					int w = (int)Math.Round(Width / 2f) - cornerRadius;
					int h = (int)Math.Round(Height / 2f) - cornerRadius;
					Rectangle rect = Rectangle.Empty;
					rect.Offset(X - w, Y - h);
					rect.Width = Width - cornerRadius - cornerRadius;
					rect.Height = Height - cornerRadius - cornerRadius;
					Point topLeft, topRight, bottomLeft, bottomRight;
					Geometry.RotateRectangle(rect, Center, Geometry.TenthsOfDegreeToDegrees(Angle), out topLeft, out topRight, out bottomRight, out bottomLeft);

					result.X = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X));
					result.Y = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y));
					result.Width = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X)) - result.X;
					result.Height = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y)) - result.Y;
					result.Inflate(cornerRadius, cornerRadius);
					ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
					return result;
				}
			} else return base.CalculateBoundingRectangle(tight);
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int r = CalcCornerRadius();

				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;

				Path.Reset();
				Path.StartFigure();
				Path.AddLine(left + r, top, right - r, top);
				Path.AddArc(right - r - r, top, r + r, r + r, -90, 90);
				Path.AddLine(right, top + r, right, bottom - r);
				Path.AddArc(right - r - r, bottom - r - r, r + r, r + r, 0, 90);
				Path.AddLine(right - r, bottom, left + r, bottom);
				Path.AddArc(left, bottom - r - r, r + r, r + r, 90, 90);
				Path.AddLine(left, bottom - r, left, top + r);
				Path.AddArc(left, top, r + r, r + r, 180, 90);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		private int CalcCornerRadius() {
			int result = 10;
			if (Width < 30)
				result = (int)Math.Round((float)(Width - 2) / 3);
			if (Height < 30)
				result = (int)Math.Round((float)(Height - 2) / 3);
			if (result <= 0) result = 1;
			return result;
		}
	}


	public class Diamond : DiamondBase {

		internal static Shape CreateInstance(ShapeType shapeType, Template template) {
			return new Diamond(shapeType, template);
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new Diamond(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal Diamond(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal Diamond(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}

	}

}
