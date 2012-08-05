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
using System.Drawing.Drawing2D;

using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.FlowChartShapes {

	public abstract class FlowChartRectangleBase : RectangleBase {

		/// <override></override>
		protected override int ControlPointCount { get { return 9; } }


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopCenterControlPoint:
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
				case BottomCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0
						|| (controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId));
				case TopLeftControlPoint:
				case TopRightControlPoint:
				case BottomLeftControlPoint:
				case BottomRightControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
				case MiddleCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Rotate) != 0 || (controlPointCapability & ControlPointCapabilities.Reference) != 0);
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		protected internal FlowChartRectangleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal FlowChartRectangleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


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
	}


	public abstract class FlowChartSquareBase : SquareBase {

		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopCenterControlPoint:
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
				case BottomCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0 || ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId)));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		protected override int ControlPointCount { get { return 9; } }


		protected internal FlowChartSquareBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal FlowChartSquareBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		// ControlPoint Id Constants
		private const int TopCenterControlPoint = 2;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomCenterControlPoint = 7;
	}


	public abstract class FlowChartEllipseBase : EllipseBase {

		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopCenterControlPoint:
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
				case BottomCenterControlPoint:
					return (controlPointCapability & ControlPointCapabilities.Resize) != 0 && (IsConnectionPointEnabled(controlPointId));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		protected override int ControlPointCount { get { return 9; } }


		protected internal FlowChartEllipseBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal FlowChartEllipseBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		// ControlPoint Id Constants
		private const int TopCenterControlPoint = 2;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomCenterControlPoint = 7;
	}


	public abstract class FlowChartCircleBase : CircleBase {

		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopLeftControlPoint:
				case TopRightControlPoint:
				case BottomLeftControlPoint:
				case BottomRightControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		protected override int ControlPointCount { get { return 9; } }


		protected internal FlowChartCircleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal FlowChartCircleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		// ControlPoint Id Constants
		private const int TopLeftControlPoint = 1;
		private const int TopRightControlPoint = 3;
		private const int BottomLeftControlPoint = 6;
		private const int BottomRightControlPoint = 8;
	}


	public abstract class FlowChartDiamondBase : DiamondBase {

		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopCenterControlPoint:
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
				case BottomCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0 || (controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId));
				case MiddleCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Rotate) != 0 || (controlPointCapability & ControlPointCapabilities.Reference) != 0);
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		protected override int ControlPointCount {
			get { return 9; }
		}


		protected internal FlowChartDiamondBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal FlowChartDiamondBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		// ControlPoint Id Constants
		private const int TopCenterControlPoint = 2;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomCenterControlPoint = 7;
		private const int MiddleCenterControlPoint = 9;
	}


	public abstract class FlowChartTriangleBase : IsoscelesTriangleBase {

		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case ControlPointId.Reference:
				case MiddleCenterControlPoint:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
				case BottomCenterControlPoint:
				case TopCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0 || (controlPointCapability & ControlPointCapabilities.Connect) != 0);
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
				//return (controlPointCapability & ControlPointCapabilities.Resize) != 0;
			}
		}


		/// <override></override>
		protected override int ControlPointCount { get { return 5; } }


		protected internal FlowChartTriangleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal FlowChartTriangleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		// ControlPoint Id Constants
		private const int TopCenterControlPoint = 1;
		private const int BottomLeftControlPoint = 2;
		private const int BottomCenterControlPoint = 3;
		private const int BottomRightControlPoint = 4;
		private const int MiddleCenterControlPoint = 5;
		private const int LeftConnectionPoint = 6;
		private const int RightConnectionPoint = 7;
	}


	public class TerminatorSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new TerminatorSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				Rectangle result = Rectangle.Empty; ;
				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);
				if (angle == 0 || angle == 180) {
					result.X = X - Width / 2;
					result.Y = Y - Height / 2;
					result.Width = Width;
					result.Height = Height;
				} else if (angle == 90 || angle == 270) {
					result.X = X - Height / 2;
					result.Y = Y - Width / 2;
					result.Width = Height;
					result.Height = Width;
				} else {
					float arcRadius = ArcDiameter / 2f;
					PointF topLeftCenter, bottomRightCenter;
					if (ArcDiameter == Width) {
						topLeftCenter = Geometry.RotatePoint(X, Y, angle, X, Y - (Height / 2f) + arcRadius);
						bottomRightCenter = Geometry.RotatePoint(X, Y, angle, X, Y + (Height / 2f) - arcRadius);
					} else {
						topLeftCenter = Geometry.RotatePoint(X, Y, angle, X - (Width / 2f) + arcRadius, Y);
						bottomRightCenter = Geometry.RotatePoint(X, Y, angle, X + (Width / 2f) - arcRadius, Y);
					}
					result.X = (int)Math.Round(Math.Min(topLeftCenter.X - arcRadius, bottomRightCenter.X - arcRadius));
					result.Y = (int)Math.Round(Math.Min(topLeftCenter.Y - arcRadius, bottomRightCenter.Y - arcRadius));
					result.Width = (int)Math.Round(Math.Max(topLeftCenter.X + arcRadius, bottomRightCenter.X + arcRadius)) - result.X;
					result.Height = (int)Math.Round(Math.Max(topLeftCenter.Y + arcRadius, bottomRightCenter.Y + arcRadius)) - result.Y;
				}
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		/// <override></override>
		protected internal TerminatorSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal TerminatorSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			captionBounds = Rectangle.Empty;
			captionBounds.X = left + (ArcDiameter / 4);
			captionBounds.Y = top;
			captionBounds.Width = Width - (ArcDiameter / 2);
			captionBounds.Height = Height;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;

				int startAngle = (ArcDiameter == Width) ? 180 : 90;
				const int sweepAngle = 180;

				Path.Reset();
				Path.StartFigure();
				float arcRadius = ArcDiameter / 2f;
				if (ArcDiameter == Height) {
					Path.AddArc(left, top, ArcDiameter, ArcDiameter, startAngle, sweepAngle);
					if (Width > ArcDiameter)
						Path.AddLine(left + arcRadius, top, right - arcRadius, top);
					Path.AddArc(right - ArcDiameter, top, ArcDiameter, ArcDiameter, 180 + startAngle, sweepAngle);
					if (Width > ArcDiameter)
						Path.AddLine(right - arcRadius, bottom, left + arcRadius, bottom);
				} else {
					Path.AddArc(left, top, ArcDiameter, ArcDiameter, startAngle, sweepAngle);
					if (Height > ArcDiameter) 
						Path.AddLine(right, top + arcRadius, right, bottom - arcRadius);
					Path.AddArc(left, bottom - ArcDiameter, ArcDiameter, ArcDiameter, 180 + startAngle, sweepAngle);
					if (Height > ArcDiameter) 
						Path.AddLine(left, bottom - arcRadius, left, top + arcRadius);
				}
				Path.CloseFigure();
				return true;
			} else return false;
		}


		private int ArcDiameter {
			get { return (Width < Height) ? Width : Height; }
		}

	}


	public class ProcessSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new ProcessSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override int ControlPointCount { get { return base.ControlPointCount + 4; } }


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopLeftConnectionPoint:
				case TopRightConnectionPoint:
				case BottomLeftConnectionPoint:
				case BottomRightConnectionPoint:
					return ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			base.CalcControlPoints();

			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			ControlPoints[9].X = left + (int)Math.Round(Width / 4f);
			ControlPoints[9].Y = top;
			ControlPoints[10].X = right - (int)Math.Round(Width / 4f);
			ControlPoints[10].Y = top;
			ControlPoints[11].X = left + (int)Math.Round(Width / 4f);
			ControlPoints[11].Y = bottom;
			ControlPoints[12].X = right - (int)Math.Round(Width / 4f);
			ControlPoints[12].Y = bottom;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);

				shapeRect.X = left;
				shapeRect.Y = top;
				shapeRect.Width = Width;
				shapeRect.Height = Height;

				Path.Reset();
				Path.StartFigure();
				Path.AddRectangle(shapeRect);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		/// <override></override>
		protected internal ProcessSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal ProcessSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		#region Fields
		private const int TopLeftConnectionPoint = 10;
		private const int TopRightConnectionPoint = 11;
		private const int BottomLeftConnectionPoint = 12;
		private const int BottomRightConnectionPoint = 13;
		Rectangle shapeRect;
		#endregion
	}


	public class PredefinedProcessSymbol : ProcessSymbol {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new PredefinedProcessSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected internal PredefinedProcessSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal PredefinedProcessSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			base.CalcCaptionBounds(index, out captionBounds);
			captionBounds.X = (int)Math.Round(-(Width / 2f) + (Width / 8f));
			captionBounds.Width = Width - (Width / 4);
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();

				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int sideBarWidth = 10;
				if (Width / 8 < 10)
					sideBarWidth = Width / 8;
				Rectangle rect = Rectangle.Empty;

				Path.StartFigure();
				rect.X = left; rect.Y = top; rect.Width = Width; rect.Height = Height;
				Path.AddRectangle(rect);

				rect.Width = sideBarWidth;
				Path.AddRectangle(rect);

				rect.X = right - sideBarWidth;
				Path.AddRectangle(rect);
				Path.CloseFigure();
				Path.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
				return true;
			} else return false;
		}
	}


	public class DecisionSymbol : FlowChartDiamondBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new DecisionSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected internal DecisionSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal DecisionSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		#region Fields
		Point[] shapeBuffer = new Point[4];
		#endregion
	}


	public class InputOutputSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new InputOutputSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				int left = X + (int)Math.Round(-Width / 2f);
				int top = Y + (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int offset = GetParallelOffset();
				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);

				Point tl = Geometry.RotatePoint(X, Y, angle, left + offset, top);
				Point tr = Geometry.RotatePoint(X, Y, angle, right, top);
				Point bl = Geometry.RotatePoint(X, Y, angle, left, bottom);
				Point br = Geometry.RotatePoint(X, Y, angle, right - offset, bottom);

				Rectangle result;
				Geometry.CalcBoundingRectangle(tl, tr, bl, br, out result);
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);

				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		/// <override></override>
		protected internal InputOutputSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal InputOutputSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int offset = GetParallelOffset();

			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			// top row (left to right)
			ControlPoints[0].X = left + offset;
			ControlPoints[0].Y = top;
			ControlPoints[1].X = 0;
			ControlPoints[1].Y = top;
			ControlPoints[2].X = right;
			ControlPoints[2].Y = top;

			// middle row (left to right)
			ControlPoints[3].X = left + (offset / 2);
			ControlPoints[3].Y = 0;
			ControlPoints[4].X = right - (offset / 2);
			ControlPoints[4].Y = 0;

			// bottom row (left to right)
			ControlPoints[5].X = left;
			ControlPoints[5].Y = bottom;
			ControlPoints[6].X = 0;
			ControlPoints[6].Y = bottom;
			ControlPoints[7].X = right - offset;
			ControlPoints[7].Y = bottom;

			// rotate handle
			ControlPoints[8].X = 0;
			ControlPoints[8].Y = 0;
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			base.CalcCaptionBounds(index, out captionBounds);
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int offset = Height / 4;
				if (offset > Width)
					offset = Width / 4;

				// top row (left to right)
				shapePoints[0].X = left + offset; ;
				shapePoints[0].Y = top;
				shapePoints[1].X = right;
				shapePoints[1].Y = top;

				// bottom row (right to left)
				shapePoints[2].X = right - offset;
				shapePoints[2].Y = bottom;
				shapePoints[3].X = left;
				shapePoints[3].Y = bottom;

				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		private int GetParallelOffset() {
			return Math.Min(Height / 4, Width / 4);
		}


		#region Fields
		Point[] shapePoints = new Point[4];
		#endregion
	}


	public class DocumentSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new DocumentSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			if (controlPointId == BottomCenterControlPoint && (controlPointCapability == ControlPointCapabilities.Connect))
				return false;
			else return base.HasControlPointCapability(controlPointId, controlPointCapability);
		}


		/// <override></override>
		protected internal DocumentSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal DocumentSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			base.CalcCaptionBounds(index, out captionBounds);
			captionBounds.Height -= (2 * CalcTearOffHeight());
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int tearOffHeight = CalcTearOffHeight();

				Path.Reset();
				Path.StartFigure();
				Path.AddLine(left, top, left, bottom - tearOffHeight);
				Path.AddBezier(left, bottom - tearOffHeight, 0, bottom + (2 * tearOffHeight), 0, bottom - (4 * tearOffHeight), right, bottom - tearOffHeight);
				Path.AddLine(right, bottom - tearOffHeight, right, top);
				Path.AddLine(right, top, left, top);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		private int CalcTearOffHeight() { return (int)Math.Round(Math.Min(20, Height / 8f)); }


		// ControlPoint Id Constants
		private const int BottomCenterControlPoint = 7;
	}


	public class ConnectorSymbol : FlowChartCircleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new ConnectorSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected internal ConnectorSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal ConnectorSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Diameter / 2f);
				int top = (int)Math.Round(-Diameter / 2f);
				Path.StartFigure();
				Path.AddEllipse(left, top, Diameter, Diameter);
				Path.CloseFigure();
				return true;
			} else return false;
		}
	}


	public class OffpageConnectorSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new OffpageConnectorSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				CalcShapePoints(ref shapePoints);

				Matrix.Reset();
				Matrix.Translate(X, Y);
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center, MatrixOrder.Append);
				Matrix.TransformPoints(shapePoints);

				Rectangle result;
				Geometry.CalcBoundingRectangle(shapePoints, out result);
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			if (controlPointId == MiddleCenterControlPoint) {
				if ((controlPointCapability & ControlPointCapabilities.Connect) != 0
					|| (controlPointCapability & ControlPointCapabilities.Reference) != 0
					|| (controlPointCapability & ControlPointCapabilities.Rotate) != 0)
					return true;
			} else if ((controlPointCapability & ControlPointCapabilities.Connect) != 0
				 || (controlPointCapability & ControlPointCapabilities.Resize) != 0)
				return IsConnectionPointEnabled(controlPointId);
			if ((controlPointCapability & ControlPointCapabilities.Glue) != 0)
				return false;
			return false;
		}


		/// <override></override>
		protected override int ControlPointCount {
			get { return 7; }
		}


		protected internal OffpageConnectorSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal OffpageConnectorSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			// top row
			ControlPoints[0].X = left;
			ControlPoints[0].Y = top;
			ControlPoints[1].X = 0;
			ControlPoints[1].Y = top;

			// middle row
			ControlPoints[2].X = left;
			ControlPoints[2].Y = 0;
			ControlPoints[3].X = right;
			ControlPoints[3].Y = 0;

			// bottom row
			ControlPoints[4].X = left;
			ControlPoints[4].Y = bottom;
			ControlPoints[5].X = 0;
			ControlPoints[5].Y = bottom;

			// Rotate point
			ControlPoints[6].X = 0;
			ControlPoints[6].Y = 0;
		}


		protected override ControlPointId GetControlPointId(int index) {
			switch (index) {
				case 0: return TopLeftControlPoint;
				case 1: return TopCenterControlPoint;
				case 2: return MiddleLeftControlPoint;
				case 3: return MiddleRightControlPoint;
				case 4: return BottomLeftControlPoint;
				case 5: return BottomCenterControlPoint;
				case 6: return MiddleCenterControlPoint;
				default: return ControlPointId.None;
			}
		}


		protected override int GetControlPointIndex(ControlPointId id) {
			switch (id) {
				case TopLeftControlPoint: return 0;
				case TopCenterControlPoint: return 1;
				case MiddleLeftControlPoint: return 2;
				case MiddleRightControlPoint: return 3;
				case BottomLeftControlPoint: return 4;
				case BottomCenterControlPoint: return 5;
				case MiddleCenterControlPoint: return 6;
				default: return -1;
			}
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			base.CalcCaptionBounds(index, out captionBounds);
			int top = (int)Math.Round(-Height / 2f);
			captionBounds.Y = top + (int)Math.Round(Height / 4f);
			captionBounds.Height = (int)Math.Round(Height / 2f);
			captionBounds.Width = (int)Math.Round(Width - Width / 4f);
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				CalcShapePoints(ref shapePoints);

				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		private void CalcShapePoints(ref Point[] points) {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			shapePoints[0].X = left;
			shapePoints[0].Y = top;
			shapePoints[1].X = 0;
			shapePoints[1].Y = top;
			shapePoints[2].X = right;
			shapePoints[2].Y = 0;
			shapePoints[3].X = 0;
			shapePoints[3].Y = bottom;
			shapePoints[4].X = left;
			shapePoints[4].Y = bottom;
		}


		#region Fields
		private const int TopLeftControlPoint = 1;
		private const int TopCenterControlPoint = 2;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomLeftControlPoint = 6;
		private const int BottomCenterControlPoint = 7;
		private const int MiddleCenterControlPoint = 9;

		Point[] shapePoints = new Point[5];
		#endregion
	}


	public class ExtractSymbol : FlowChartTriangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new ExtractSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopCenterControlPoint:
				case BottomCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0 || ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId)));
				case BottomLeftControlPoint:
				case BottomRightControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
				case LeftConnectionPoint:
				case RightConnectionPoint:
					return ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		protected override int ControlPointCount { get { return base.ControlPointCount + 2; } }


		protected internal ExtractSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal ExtractSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			base.CalcControlPoints();

			int left = (int)Math.Round(-Width * CenterPosFactorX);
			int right = left + Width;
			int bottom = (int)Math.Round(Height * (1 - CenterPosFactorY));

			ControlPoints[5].X = left + (int)Math.Round(Width / 4f);
			ControlPoints[5].Y = bottom;
			ControlPoints[6].X = right - (int)Math.Round(Width / 4f);
			ControlPoints[6].Y = bottom;
		}


		#region Fields

		// ControlPoint Id Constants
		private const int TopCenterControlPoint = 1;
		private const int BottomLeftControlPoint = 2;
		private const int BottomCenterControlPoint = 3;
		private const int BottomRightControlPoint = 4;
		private const int MiddleCenterControlPoint = 5;
		private const int LeftConnectionPoint = 6;
		private const int RightConnectionPoint = 7;
		#endregion
	}


	public class MergeSymbol : FlowChartTriangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new MergeSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopCenterControlPoint:
				case BottomCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0 || ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId)));
				case TopLeftControlPoint:
				case TopRightControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
				case LeftConnectionPoint:
				case RightConnectionPoint:
					return ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		protected override int ControlPointCount { get { return base.ControlPointCount + 2; } }


		protected internal MergeSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal MergeSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers) {
			bool result = true;
			int dx = 0, dy = 0;
			int width = Width;
			int height = Height;
			switch ((int)pointId) {
				case TopLeftControlPoint:
					if (!Geometry.MoveRectangleTopLeft(width, height, 0, 0, CenterPosFactorX, CenterPosFactorY, DivFactorX, DivFactorY, 
													transformedDeltaX, transformedDeltaY, cos, sin, modifiers, 
													out dx, out dy, out width, out height))
						result = false;
					break;

				case TopCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleTop(width, height, 0, CenterPosFactorX, CenterPosFactorY, DivFactorX, DivFactorY, 
													transformedDeltaX, transformedDeltaY, cos, sin, modifiers, 
													out dx, out dy, out width, out height))
						result = false;
					break;

				case TopRightControlPoint:
					if (!Geometry.MoveRectangleTopRight(width, height, 0, 0, CenterPosFactorX, CenterPosFactorY, DivFactorX, DivFactorY, 
													transformedDeltaX, transformedDeltaY, cos, sin, modifiers, 
													out dx, out dy, out width, out height))
						result = false;
					break;

				case BottomCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleBottom(width, height, 0, CenterPosFactorX, CenterPosFactorY, 
													DivFactorX, DivFactorY, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, 
													out dx, out dy, out width, out height))
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
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			// tight and loose fitting bounding rectangles are equal
			if (Angle == 0 || Angle == 900 || Angle == 1800 || Angle == 2700)
				return base.CalculateBoundingRectangle(tight);
			else {
				// Calculate tight fitting bounding rectangle
				Rectangle result = Geometry.InvalidRectangle;
				float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
				int x1, y1, x2, y2, x3, y3;
				int left = (int)Math.Round(X - (Width / 2f));
				int top = Y - (int)Math.Round(Height * CenterPosFactorY);
				int right = left + Width;
				int bottom = top + Height;
				x1 = left; y1 = top;
				x2 = right; y2 = top;
				x3 = X; y3 = bottom;
				Geometry.RotatePoint(X, Y, angleDeg, ref x1, ref y1);
				Geometry.RotatePoint(X, Y, angleDeg, ref x2, ref y2);
				Geometry.RotatePoint(X, Y, angleDeg, ref x3, ref y3);

				result.X = Math.Min(Math.Min(x1, x2), Math.Min(x1, x3));
				result.Y = Math.Min(Math.Min(y1, y2), Math.Min(y1, y3));
				result.Width = Math.Max(Math.Max(x1, x2), Math.Max(x1, x3)) - result.X;
				result.Height = Math.Max(Math.Max(y1, y2), Math.Max(y1, y3)) - result.Y;
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				return result;
			}
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height * CenterPosFactorY);
			int right = left + Width;
			int bottom = top + Height;

			ControlPoints[0].X = left;
			ControlPoints[0].Y = top;
			ControlPoints[1].X = 0;
			ControlPoints[1].Y = top;
			ControlPoints[2].X = right;
			ControlPoints[2].Y = top;
			ControlPoints[3].X = 0;
			ControlPoints[3].Y = bottom;
			ControlPoints[4].X = 0;
			ControlPoints[4].Y = 0;
			ControlPoints[5].X = left + (int)Math.Round(Width * (CenterPosFactorX / 2f));
			ControlPoints[5].Y = top;
			ControlPoints[6].X = right - (int)Math.Round(Width * (CenterPosFactorX / 2f));
			ControlPoints[6].Y = top;
		}


		/// <override></override>
		protected override void CalculateShapePoints() {
			int left = (int)Math.Round(-Width * CenterPosFactorX);
			int top = (int)Math.Round(-Height * CenterPosFactorY);
			int right = left + Width;
			int bottom = top + Height;

			shapePoints[0].X = left;
			shapePoints[0].Y = top;
			shapePoints[1].X = right;
			shapePoints[1].Y = top;
			shapePoints[2].X = 0;
			shapePoints[2].Y = bottom;
		}


		/// <override></override>
		protected override float CenterPosFactorY { get { return centerPosFactorY; } }


		#region Fields
		private const float centerPosFactorY = 0.3333333333f;

		Point[] shapeBuffer = new Point[3];
		private const int TopLeftControlPoint = 1;
		private const int TopCenterControlPoint = 2;
		private const int TopRightControlPoint = 3;
		private const int BottomCenterControlPoint = 4;
		private const int BalancePointControlPoint = 5;
		private const int LeftConnectionPoint = 6;
		private const int RightConnectionPoint = 7;
		#endregion
	}


	public class OnlineStorageSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new OnlineStorageSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				int arcRadius = GetArcRadius();
				if (arcRadius > 0) {
					int left = X + (int)Math.Round(-Width / 2f);
					int top = Y + (int)Math.Round(-Height / 2f);
					int right = left + Width;
					int bottom = top + Height;

					float angle = Geometry.TenthsOfDegreeToDegrees(Angle);
					Point tl = Geometry.RotatePoint(X, Y, angle, left + arcRadius, top);
					Point tr = Geometry.RotatePoint(X, Y, angle, right, top);
					Point bl = Geometry.RotatePoint(X, Y, angle, left + arcRadius, bottom);
					Point br = Geometry.RotatePoint(X, Y, angle, right, bottom);
					Point arcCenter = Geometry.RotatePoint(X, Y, angle, left + arcRadius, Y);

					// Calculate intersection with ellipse
					Rectangle arcBounds;
					Geometry.CalcBoundingRectangleEllipse(arcCenter.X, arcCenter.Y, arcRadius + arcRadius, Height, angle, out arcBounds);

					Rectangle result = Rectangle.Empty;
					Geometry.CalcBoundingRectangle(tl, tr, bl, br, out result);
					result = Geometry.UniteRectangles(arcBounds, result);
					ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
					return result;
				} else return base.CalculateBoundingRectangle(tight);
			} else return base.CalculateBoundingRectangle(tight);
		}


		/// <override></override>
		protected internal OnlineStorageSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal OnlineStorageSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			base.CalcCaptionBounds(index, out captionBounds);
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int arcRadius = GetArcRadius();
			captionBounds.X = left + arcRadius;
			captionBounds.Y = top;
			captionBounds.Width = Width - arcRadius - arcRadius;
			captionBounds.Height = Height;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int arcRadius = GetArcRadius();

				Path.StartFigure();
				Path.AddArc(left, top, arcRadius + arcRadius, Height, 90, 180);
				Path.AddLine(left + arcRadius, top, right, top);
				Path.AddArc(right - arcRadius, top, arcRadius + arcRadius, Height, -90, -180);
				Path.AddLine(right, bottom, left + arcRadius, bottom);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		private int GetArcRadius() {
			return (Width < Height) ? Width / 4 : Height / 4;
		}

	}


	public class OfflineStorageSymbol : MergeSymbol {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new OfflineStorageSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal OfflineStorageSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal OfflineStorageSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width * CenterPosFactorX);
				int top = (int)Math.Round(-Height * CenterPosFactorY);
				int right = left + Width;
				int bottom = top + Height;

				shapePoints[0].X = left;
				shapePoints[0].Y = top;
				shapePoints[1].X = right;
				shapePoints[1].Y = top;
				shapePoints[2].X = 0;
				shapePoints[2].Y = bottom;

				int x1, x2, y1, y2;
				int a1, b1, c1, a2, b2, c2, a, b, c;
				Geometry.CalcLine(0, bottom, left, top, out a1, out b1, out c1);
				Geometry.CalcLine(0, bottom, right, top, out a2, out b2, out c2);
				Geometry.CalcLine(left, bottom - (Height / 3), right, bottom - (Height / 3), out a, out b, out c);
				//Geometry.SolveLinear22System(a, b, a1, b1, c, c1, out x1, out y1);
				Geometry.IntersectLines(a, b, c, a1, b1, c1, out x1, out y1);
				//Geometry.SolveLinear22System(a, b, a2, b2, c, c2, out x2, out y2);
				Geometry.IntersectLines(a, b, c, a2, b2, c2, out x2, out y2);

				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				Path.StartFigure();
				Path.AddLine(x1, y1, x2, y2);
				Path.CloseFigure();
				return true;
			} else return false;
		}
	}


	public class DrumStorageSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new DrumStorageSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				int arcRadius = GetArcRadius();
				if (arcRadius > 0) {
					int left = X + (int)Math.Round(-Width / 2f);
					int top = Y + (int)Math.Round(-Height / 2f);
					int right = left + Width;
					int bottom = top + Height;
					float angle = Geometry.TenthsOfDegreeToDegrees(Angle);

					Point leftCenter = Geometry.RotatePoint(X, Y, angle, left + arcRadius, Y);
					Point rightCenter = Geometry.RotatePoint(X, Y, angle, right - arcRadius, Y);

					Rectangle leftSideBounds;
					Geometry.CalcBoundingRectangleEllipse(leftCenter.X, leftCenter.Y, 2 * arcRadius, Height, angle, out leftSideBounds);
					ShapeUtils.InflateBoundingRectangle(ref leftSideBounds, LineStyle);

					Rectangle rightSideBounds;
					Geometry.CalcBoundingRectangleEllipse(rightCenter.X, rightCenter.Y, 2 * arcRadius, Height, angle, out rightSideBounds);
					ShapeUtils.InflateBoundingRectangle(ref rightSideBounds, LineStyle);

					return Geometry.UniteRectangles(leftSideBounds, rightSideBounds);
				} else return base.CalculateBoundingRectangle(tight);
			} else return base.CalculateBoundingRectangle(tight);
		}


		protected internal DrumStorageSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal DrumStorageSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int arcRadius = GetArcRadius();
			captionBounds = Rectangle.Empty;
			captionBounds.X = left + arcRadius;
			captionBounds.Y = top;
			captionBounds.Width = Width - (2 * arcRadius);
			captionBounds.Height = Height;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int arcRadius = GetArcRadius();

				Path.StartFigure();
				Path.AddArc(right - (2 * arcRadius), top, arcRadius + arcRadius, Height, -90, -180);
				Path.AddLine(right - (2 * arcRadius), bottom, left + arcRadius, bottom);
				Path.AddArc(left, top, arcRadius + arcRadius, Height, 90, 180);
				Path.AddLine(left + arcRadius, top, right - arcRadius - arcRadius, top);
				Path.AddArc(right - (2 * arcRadius), top, arcRadius + arcRadius, Height, -90, 180);
				Path.AddArc(right - (2 * arcRadius), top, arcRadius + arcRadius, Height, 90, 180);
				Path.CloseFigure();
				Path.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
				return true;
			} else return false;
		}


		private int GetArcRadius() {
			return (Width < Height) ? Width / 4 : Height / 4;
		}
	}


	public class DiskStorageSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new DiskStorageSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				int arcRadius = GetArcRadius();
				if (arcRadius > 0) {
					int left = X + (int)Math.Round(-Width / 2f);
					int top = Y + (int)Math.Round(-Height / 2f);
					int right = left + Width;
					int bottom = top + Height;
					float angle = Geometry.TenthsOfDegreeToDegrees(Angle);
					int halfLineWidth = (int)Math.Round(LineStyle.LineWidth / 2f, MidpointRounding.AwayFromZero);

					Point topCenter = Geometry.RotatePoint(X, Y, angle, X, top + arcRadius);
					Point bottomCenter = Geometry.RotatePoint(X, Y, angle, X, bottom - arcRadius);
					Rectangle topBounds;
					Geometry.CalcBoundingRectangleEllipse(topCenter.X, topCenter.Y, Width, 2 * arcRadius, angle, out topBounds);
					ShapeUtils.InflateBoundingRectangle(ref topBounds, LineStyle);
					Rectangle bottomBounds;
					Geometry.CalcBoundingRectangleEllipse(bottomCenter.X, bottomCenter.Y, Width, 2 * arcRadius, angle, out bottomBounds);
					ShapeUtils.InflateBoundingRectangle(ref bottomBounds, LineStyle);

					return Geometry.UniteRectangles(topBounds, bottomBounds);
				} else return base.CalculateBoundingRectangle(tight);
			} else return base.CalculateBoundingRectangle(tight);
		}


		protected internal DiskStorageSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal DiskStorageSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Height = 40;
			Width = 40;
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			base.CalcCaptionBounds(index, out captionBounds);
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int arcRadius = GetArcRadius();
			captionBounds.X = left + arcRadius;
			captionBounds.Y = top;
			captionBounds.Width = Width - arcRadius - arcRadius;
			captionBounds.Height = Height;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int arcRadius = GetArcRadius();
				int arcSpacing = arcRadius;
				if (Height / arcRadius < 6)
					arcSpacing = Height / 6;

				Path.StartFigure();
				// Disk #1
				Path.AddArc(left, top, Width, arcRadius + arcRadius, 180, -180);
				Path.AddArc(left, top, Width, arcRadius + arcRadius, 0, 180);
				// Disk #2
				Path.AddArc(left, top + arcSpacing, Width, arcRadius + arcRadius, 180, -180);
				Path.AddArc(left, top + arcSpacing, Width, arcRadius + arcRadius, 0, 180);
				// Disk #3
				Path.AddArc(left, top + arcSpacing + arcSpacing, Width, arcRadius + arcRadius, 180, -180);
				Path.AddArc(left, top + arcSpacing + arcSpacing, Width, arcRadius + arcRadius, 0, 180);
				// Outline
				Path.AddLine(left, top + arcRadius, left, bottom - arcRadius);
				Path.AddArc(left, bottom - arcRadius - arcRadius, Width, arcRadius + arcRadius, 180, -180);
				Path.AddLine(right, bottom - arcRadius, right, top + arcRadius);
				Path.AddArc(left, top, Width, arcRadius + arcRadius, 0, -180);
				Path.CloseFigure();
				Path.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
				return true;
			} else return false;
		}


		private int GetArcRadius() {
			return (int)Math.Round(Math.Min(Height / 4f, Width / 4f));
		}
	}


	public class TapeStorageSymbol : FlowChartCircleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new TapeStorageSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				Rectangle result = base.CalculateBoundingRectangle(tight);
				Rectangle r = Rectangle.Empty;
				r.Location = Point.Round(Geometry.RotatePoint(X, Y, Geometry.TenthsOfDegreeToDegrees(Angle), X + (DiameterInternal / 2f), Y + (DiameterInternal / 2f)));
				ShapeUtils.InflateBoundingRectangle(ref r, LineStyle);
				return Geometry.UniteRectangles(r, result);
			} else return base.CalculateBoundingRectangle(tight);
		}


		protected internal TapeStorageSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal TapeStorageSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			if (base.IntersectsWithCore(x, y, width, height))
				return true;
			int bottom = (Y - (DiameterInternal / 2) + DiameterInternal);
			int right = (X - (DiameterInternal / 2) + DiameterInternal);
			if (Geometry.RectangleIntersectsWithLine(x, y, x + width, y + height, Center.X, bottom, right, bottom, true))
				return true;
			return false;
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			base.CalcControlPoints();
			int right = (int)Math.Round(Diameter / 2f);
			int bottom = right;
			ControlPoints[7].X = right;
			ControlPoints[7].Y = bottom;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Diameter / 2f);
				int top = (int)Math.Round(-Diameter / 2f);
				int right = left + Diameter;
				int bottom = top + Diameter;

				Path.StartFigure();
				Path.AddLine(0, bottom, right, bottom);
				Path.AddEllipse(left, top, Diameter, Diameter);
				Path.CloseFigure();
				return true;
			}
			return false;
		}

	}


	public class PreparationSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new PreparationSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				CalcShapePoints(ref shapePoints);

				Matrix.Reset();
				Matrix.Translate(X, Y);
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center, MatrixOrder.Append);
				Matrix.TransformPoints(shapePoints);

				Rectangle result;
				Geometry.CalcBoundingRectangle(shapePoints, out result);
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		protected internal PreparationSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal PreparationSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		private int CalcEdgeInset() { return Height / 6; }


		/// <override></override>
		protected override void CalcControlPoints() {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;
			int edgeInset = CalcEdgeInset();

			// top row
			ControlPoints[0].X = left + edgeInset;
			ControlPoints[0].Y = top;
			ControlPoints[1].X = 0;
			ControlPoints[1].Y = top;
			ControlPoints[2].X = right - edgeInset;
			ControlPoints[2].Y = top;

			// middle row
			ControlPoints[3].X = left;
			ControlPoints[3].Y = 0;
			ControlPoints[4].X = right;
			ControlPoints[4].Y = 0;

			// bottom row
			ControlPoints[5].X = left + edgeInset;
			ControlPoints[5].Y = bottom;
			ControlPoints[6].X = 0;
			ControlPoints[6].Y = bottom;
			ControlPoints[7].X = right - edgeInset;
			ControlPoints[7].Y = bottom;

			ControlPoints[8].X = 0;
			ControlPoints[8].Y = 0;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				CalcShapePoints(ref shapePoints);

				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				return true;
			} else return false;
		}


		private void CalcShapePoints(ref Point[] points) {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;
			int edgeInset = CalcEdgeInset();

			points[0].X = left + edgeInset;
			points[0].Y = top;
			points[1].X = right - edgeInset;
			points[1].Y = top;
			points[2].X = right;
			points[2].Y = 0;
			points[3].X = right - edgeInset;
			points[3].Y = bottom;
			points[4].X = left + edgeInset;
			points[4].Y = bottom;
			points[5].X = left;
			points[5].Y = 0;
		}


		#region Fields
		private Point[] shapePoints = new Point[6];
		#endregion
	}


	public class ManualInputSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new ManualInputSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				int left = X+(int)Math.Round(-Width / 2f);
				int top = Y + (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);

				Point tl = Geometry.RotatePoint(X, Y, angle, left, top + (Height / 4));
				Point tr = Geometry.RotatePoint(X, Y, angle, right, top);
				Point bl = Geometry.RotatePoint(X, Y, angle, left, bottom);
				Point br = Geometry.RotatePoint(X, Y, angle, right, bottom);

				Rectangle result;
				Geometry.CalcBoundingRectangle(tl, tr, bl, br, out result);
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		protected internal ManualInputSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal ManualInputSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;

				shapePoints[0].X = left;
				shapePoints[0].Y = top + (Height / 4);
				shapePoints[1].X = right;
				shapePoints[1].Y = top;
				shapePoints[2].X = right;
				shapePoints[2].Y = bottom;
				shapePoints[3].X = left;
				shapePoints[3].Y = bottom;

				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		#region Fields
		private Point[] shapePoints = new Point[4];
		#endregion
	}


	public class CoreSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new CoreSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal CoreSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal CoreSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();

				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int sideBarSize = CalcSideBarSize();
				Rectangle rect = Rectangle.Empty;

				Path.StartFigure();
				rect.X = left;
				rect.Y = top;
				rect.Width = Width;
				rect.Height = Height;
				Path.AddRectangle(rect);

				rect.Width = sideBarSize;
				Path.AddRectangle(rect);

				rect.Width = Width;
				rect.Height = sideBarSize;
				Path.AddRectangle(rect);
				Path.CloseFigure();
				Path.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
				return true;
			}
			return false;
		}


		private int CalcSideBarSize() { return (int)Math.Round(Math.Min(Width / 8f, Height / 8f)); }
	}


	public class DisplaySymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new DisplaySymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			Rectangle result = Geometry.InvalidRectangle;
			if (tight) {
				int left = X + (int)Math.Round(-Width / 2f);
				int top = Y + (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);
				int arcRadius = CalcArcRadius();

				Point tl = Geometry.RotatePoint(X, Y, angle, left + arcRadius, top);
				Point ml = Geometry.RotatePoint(X, Y, angle, left, Y);
				Point bl = Geometry.RotatePoint(X, Y, angle, left + arcRadius, bottom);
				Point arcCenter = Geometry.RotatePoint(X, Y, angle, right - arcRadius, Y);

				Rectangle leftBounds;
				Geometry.CalcBoundingRectangle(tl, ml, bl, arcCenter, out leftBounds);
				if (arcRadius > 0) {
					Rectangle rightBounds;
					Geometry.CalcBoundingRectangleEllipse(arcCenter.X, arcCenter.Y, arcRadius + arcRadius, Height, angle, out rightBounds);
					result = Geometry.UniteRectangles(leftBounds, rightBounds);
				} else {
					Point tr = Geometry.RotatePoint(X, Y, angle, right, top);
					Point br = Geometry.RotatePoint(X, Y, angle, right, bottom);
					result = Geometry.UniteRectangles(tr.X, tr.Y, br.X, br.Y, leftBounds);
				}
			} else result =  base.CalculateBoundingRectangle(tight);
			if (Geometry.IsValid(result))
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
			return result;
		}


		protected internal DisplaySymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal DisplaySymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int arcRadius = CalcArcRadius();

				shapePoints[0].X = left;
				shapePoints[0].Y = 0;
				shapePoints[1].X = left + arcRadius;
				shapePoints[1].Y = top;
				shapePoints[2].X = right;
				shapePoints[2].Y = top;
				shapePoints[3].X = right;
				shapePoints[3].Y = bottom;
				shapePoints[4].X = left + arcRadius;
				shapePoints[4].Y = bottom;

				Path.Reset();
				Path.StartFigure();
				Path.AddLine(shapePoints[0], shapePoints[1]);
				Path.AddLine(shapePoints[1].X, shapePoints[1].Y, shapePoints[2].X - arcRadius, shapePoints[2].Y);
				if (arcRadius > 0)
					Path.AddArc(shapePoints[2].X - arcRadius - arcRadius, shapePoints[2].Y, arcRadius + arcRadius, Height, -90, 180);
				else Path.AddLine(shapePoints[2], shapePoints[3]);
				Path.AddLine(shapePoints[3].X - arcRadius, shapePoints[3].Y, shapePoints[4].X, shapePoints[4].Y);
				Path.AddLine(shapePoints[4].X, shapePoints[4].Y, shapePoints[0].X, shapePoints[0].Y);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		private int CalcArcRadius() {
			return (int)Math.Round(Math.Min(Width / 4f, Height / 4f));
		}


		#region Fields
		private Point[] shapePoints = new Point[5];
		#endregion
	}


	public class TapeSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new TapeSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal TapeSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal TapeSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Width = 60;
			Height = 40;
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			base.CalcCaptionBounds(index, out captionBounds);
			int tearOffSize = CalcTearOffSize();
			captionBounds.Y += tearOffSize;
			captionBounds.Height -= (2 * tearOffSize);
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int tearOffSize = CalcTearOffSize();
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round((-Height / 2f) + tearOffSize);
				int right = left + Width;
				int bottom = (int)Math.Round((-Height / 2f) + Height - tearOffSize);

				Path.StartFigure();
				Path.AddBezier(left, top,
					0, top + (3 * tearOffSize),
					0, top - (3 * tearOffSize),
					right, top);
				Path.AddLine(right, top, right, bottom);
				Path.AddBezier(right, bottom,
					0, bottom - (3 * tearOffSize),
					0, bottom + (3 * tearOffSize),
					left, bottom);
				Path.AddLine(left, bottom, left, top);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		private int CalcTearOffSize() {
			return (int)Math.Round(Math.Min(10, Height / 8f));
		}
	}


	public class ManualOperationSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new ManualOperationSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				int left = X + (int)Math.Round(-Width / 2f);
				int top = Y + (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int edgeInset = CalcEdgeInset();
				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);

				Point tl = Geometry.RotatePoint(X, Y, angle, left, top);
				Point tr = Geometry.RotatePoint(X, Y, angle, right, top);
				Point bl = Geometry.RotatePoint(X, Y, angle, left + edgeInset, bottom);
				Point br = Geometry.RotatePoint(X, Y, angle, right - edgeInset, bottom);

				Rectangle result;
				Geometry.CalcBoundingRectangle(tl, tr, bl, br, out result);
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		/// <override></override>
		protected internal ManualOperationSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected internal ManualOperationSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int edgeInset = CalcEdgeInset();

				shapePoints[0].X = left;
				shapePoints[0].Y = top;
				shapePoints[1].X = right;
				shapePoints[1].Y = top;
				shapePoints[2].X = right - edgeInset;
				shapePoints[2].Y = bottom;
				shapePoints[3].X = left + edgeInset;
				shapePoints[3].Y = bottom;

				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		private int CalcEdgeInset() { return (int)Math.Round(Width / 4f); }


		#region Fields
		private Point[] shapePoints = new Point[4];
		#endregion
	}


	public class SortSymbol : FlowChartDiamondBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new SortSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal SortSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal SortSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Width = 40;
			Height = 40;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				if (Width > 0 && Height > 0) {
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

					Path.StartFigure();
					Path.AddPolygon(pointBuffer);
					Path.CloseFigure();

					// refresh edge positions
					Path.StartFigure();
					Path.AddLine(left, 0, right, 0);
					Path.CloseFigure();
					Path.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
					return true;
				} else return false;
			} else return false;
		}
	}


	public class CollateSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new CollateSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal CollateSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal CollateSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			shapePointBuffer[0].X = left;
			shapePointBuffer[0].Y = top;
			shapePointBuffer[1].X = right;
			shapePointBuffer[1].Y = top;
			shapePointBuffer[2].X = right;
			shapePointBuffer[2].Y = bottom;
			shapePointBuffer[3].X = left;
			shapePointBuffer[3].Y = bottom;
			Matrix.Reset();
			Matrix.Translate(X, Y);
			if (Angle != 0) Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center, MatrixOrder.Append);
			Matrix.TransformPoints(shapePointBuffer);

			if (Geometry.TriangleContainsPoint(shapePointBuffer[0], shapePointBuffer[1], Center, x, y)
				|| Geometry.TriangleContainsPoint(Center, shapePointBuffer[2], shapePointBuffer[3], x, y))
				return true;
			else return false;
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Height = 40;
			Width = 40;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;

				Path.AddLine(left, top, right, top);
				Path.AddLine(right, top, left, bottom);
				Path.AddLine(left, bottom, right, bottom);
				Path.AddLine(right, bottom, left, top);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		private Point[] shapePointBuffer = new Point[4];
	}


	public class CardSymbol : FlowChartRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new CardSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				int left = X + (int)Math.Round(-Width / 2f);
				int top = Y + (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int edgeSize = CalcEdgeSize();
				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);

				Point itl = Geometry.RotatePoint(X, Y, angle, left + edgeSize, top);
				Point tr = Geometry.RotatePoint(X, Y, angle, right, top);
				Point ibl = Geometry.RotatePoint(X, Y, angle, left + edgeSize, bottom);
				Point br = Geometry.RotatePoint(X, Y, angle, right, bottom);

				Point otl = Geometry.RotatePoint(X, Y, angle, left, top + edgeSize);
				Point obl = Geometry.RotatePoint(X, Y, angle, left, bottom);

				Rectangle result;
				Geometry.CalcBoundingRectangle(itl, tr, ibl, br, out result); 
				result = Geometry.UniteWithRectangle(otl, result);
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				return Geometry.UniteWithRectangle(obl, result);
			} else return base.CalculateBoundingRectangle(tight);
		}


		protected internal CardSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal CardSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		private int CalcEdgeSize() { return (int)Math.Round(Math.Min(Width / 4f, Height / 4f)); }


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				if (Width > 0 && Height > 0) {
					int left = (int)Math.Round(-Width / 2f);
					int top = (int)Math.Round(-Height / 2f);
					int right = left + Width;
					int bottom = top + Height;
					int edgeSize = CalcEdgeSize();

					Path.AddLine(left + edgeSize, top, right, top);
					Path.AddLine(right, top, right, bottom);
					Path.AddLine(right, bottom, left, bottom);
					Path.AddLine(left, bottom, left, top + edgeSize);
					Path.CloseFigure();
					return true;
				} else return false;
			} else return false;
		}
	}


	public class CommLinkSymbol : FlowChartDiamondBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new CommLinkSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopCenterControlPoint:
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
				case BottomCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
				case MiddleCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Reference) != 0 || (controlPointCapability & ControlPointCapabilities.Rotate) != 0 || ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId)));
				case ControlPointId.Reference:
					return ((controlPointCapability & ControlPointCapabilities.Reference) != 0 || (controlPointCapability & ControlPointCapabilities.Rotate) != 0);
				default:
					return false;
			}
		}


		protected internal CommLinkSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
			pointBuffer = new Point[6];
		}


		protected internal CommLinkSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
			pointBuffer = new Point[6];
		}


		/// <override></override>
		protected override int ControlPointCount {
			get { return 5; }
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			pointBuffer = new Point[6];
			Height = 60;
			Width = 20;
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers) {
			bool result = true;
			int dx = 0, dy = 0;
			int width = Width;
			int height = Height;
			switch ((int)pointId) {
				case TopCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleTop(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
						result = false;
					break;
				case MiddleLeftControlPoint:
					result = (transformedDeltaY == 0);
					if (!Geometry.MoveRectangleLeft(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
						result = false;
					break;
				case MiddleRightControlPoint:
					result = (transformedDeltaY == 0);
					if (!Geometry.MoveRectangleRight(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
						result = false;
					break;
				case BottomCenterControlPoint:
					result = (transformedDeltaX == 0);
					if (!Geometry.MoveRectangleBottom(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
						result = false;
					break;
				default:
					break;
			}
			if (result) {
				Width = width;
				Height = height;
				X += dx;
				Y += dy;
			}
			ControlPointsHaveMoved();
			return result;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			Rectangle result;
			CalculalteTranslatedShapePoints();
			Geometry.CalcBoundingRectangle(pointBuffer, out result);
			ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
			return result;
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			CalculalteTranslatedShapePoints();
			bool result = Geometry.PolygonContainsPoint(pointBuffer, x, y);
			if (!result) {
				pt.X = x;
				pt.Y = y;
				if (DisplayService != null)
					DisplayService.Invalidate(new Rectangle(short.MinValue, short.MinValue, short.MaxValue * 2, short.MaxValue * 2));
			} else pt = Point.Empty;
			return result;
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			ControlPoints[0].X = 0;
			ControlPoints[0].Y = top;
			ControlPoints[1].X = left;
			ControlPoints[1].Y = 0;
			ControlPoints[2].X = right;
			ControlPoints[2].Y = 0;
			ControlPoints[3].X = 0;
			ControlPoints[3].Y = bottom;
			ControlPoints[4].X = 0;
			ControlPoints[4].Y = 0;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				CalculateShapePoints();

				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(pointBuffer);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		private void CalculateShapePoints() {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;
			int w = (int)Math.Round(Width / 8f);
			int h = (int)Math.Round(Height / 16f);

			pointBuffer[0].X = 0;
			pointBuffer[0].Y = top;
			pointBuffer[1].X = right;
			pointBuffer[1].Y = 0 + h;
			pointBuffer[2].X = 0 - w;
			pointBuffer[2].Y = 0 + (int)Math.Round(h / 2f);
			pointBuffer[3].X = 0;
			pointBuffer[3].Y = bottom;
			pointBuffer[4].X = left;
			pointBuffer[4].Y = 0 - h;
			pointBuffer[5].X = 0 + w;
			pointBuffer[5].Y = 0 - (int)Math.Round(h / 2f);
		}


		private void CalculalteTranslatedShapePoints() {
			CalculateShapePoints();
			Matrix.Reset();
			Matrix.Translate(X, Y, MatrixOrder.Prepend);
			Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center, MatrixOrder.Append);
			Matrix.TransformPoints(pointBuffer);
		}


		private Point pt;

		#region Fields
		private const int TopCenterControlPoint = 1;
		private const int MiddleLeftControlPoint = 2;
		private const int MiddleRightControlPoint = 3;
		private const int BottomCenterControlPoint = 4;
		private const int MiddleCenterControlPoint = 5;
		#endregion
	}


	public static class NShapeLibraryInitializer {

		public static void Initialize(IRegistrar registrar) {
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);
			registrar.RegisterShapeType(new ShapeType("Terminator", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new TerminatorSymbol(shapeType, t); },
				TerminatorSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Process", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new ProcessSymbol(shapeType, t); },
				ProcessSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Decision", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new DecisionSymbol(shapeType, t); },
				DecisionSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("InputOutput", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new InputOutputSymbol(shapeType, t); },
				InputOutputSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Document", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new DocumentSymbol(shapeType, t); },
				DocumentSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("OffpageConnector", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new OffpageConnectorSymbol(shapeType, t); },
				OffpageConnectorSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Connector", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new ConnectorSymbol(shapeType, t); },
				ConnectorSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("PredefinedProcess", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new PredefinedProcessSymbol(shapeType, t); },
				PredefinedProcessSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Extract", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new ExtractSymbol(shapeType, t); },
				ExtractSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Merge", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new MergeSymbol(shapeType, t); },
				MergeSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("OnlineStorage", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new OnlineStorageSymbol(shapeType, t); },
				OnlineStorageSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("OfflineStorage", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new OfflineStorageSymbol(shapeType, t); },
				OfflineStorageSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("DrumStorage", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new DrumStorageSymbol(shapeType, t); },
				DrumStorageSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("DiskStorage", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new DiskStorageSymbol(shapeType, t); },
				DiskStorageSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("TapeStorage", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new TapeStorageSymbol(shapeType, t); },
				TapeStorageSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Preparation", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new PreparationSymbol(shapeType, t); },
				PreparationSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("ManualInput", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new ManualInputSymbol(shapeType, t); },
				ManualInputSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Core", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new CoreSymbol(shapeType, t); },
				CoreSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Display", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new DisplaySymbol(shapeType, t); },
				DisplaySymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Tape", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new TapeSymbol(shapeType, t); },
				TapeSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("ManualOperation", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new ManualOperationSymbol(shapeType, t); },
				ManualOperationSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Sort", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new SortSymbol(shapeType, t); },
				SortSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Collate", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new CollateSymbol(shapeType, t); },
				CollateSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Card", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new CardSymbol(shapeType, t); },
				CardSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("CommLink", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return new CommLinkSymbol(shapeType, t); },
				CommLinkSymbol.GetPropertyDefinitions));
		}


		private const string namespaceName = "FlowChartShapes";
		private const int preferredRepositoryVersion = 3;
	}
}