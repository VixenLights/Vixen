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
using System.ComponentModel;
using System.Drawing;

using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.ElectricalShapes {

	public abstract class ElectricalRectangleBase : RectangleBase {

		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopCenterControlPoint:
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
				case BottomCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0 || (controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId));
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


		protected internal ElectricalRectangleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal ElectricalRectangleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override int ControlPointCount { get { return 9; } }


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


	public abstract class ElectricalSquareBase : SquareBase {

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


		protected internal ElectricalSquareBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal ElectricalSquareBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override int ControlPointCount { get { return 9; } }


		// ControlPoint Id Constants
		private const int TopCenterControlPoint = 2;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomCenterControlPoint = 7;
	}


	public abstract class ElectricalEllipseBase : EllipseBase {

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


		protected internal ElectricalEllipseBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal ElectricalEllipseBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		// ControlPoint Id Constants
		private const int TopCenterControlPoint = 2;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomCenterControlPoint = 7;
	}


	public abstract class ElectricalCircleBase : CircleBase {

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


		protected internal ElectricalCircleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal ElectricalCircleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override int ControlPointCount { get { return 9; } }


		// ControlPoint Id Constants
		private const int TopLeftControlPoint = 1;
		private const int TopRightControlPoint = 3;
		private const int BottomLeftControlPoint = 6;
		private const int BottomRightControlPoint = 8;
	}


	public abstract class ElectricalTriangleBase : IsoscelesTriangleBase {

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


		protected internal ElectricalTriangleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal ElectricalTriangleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override int ControlPointCount { get { return 5; } }


		// ControlPoint Id Constants
		private const int TopCenterControlPoint = 1;
		private const int BottomLeftControlPoint = 2;
		private const int BottomCenterControlPoint = 3;
		private const int BottomRightControlPoint = 4;
		private const int MiddleCenterControlPoint = 5;
	}


	public class DisconnectorSymbol : ElectricalCircleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new DisconnectorSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Diameter / 2f);
				int top = (int)Math.Round(-Diameter / 2f);

				Path.StartFigure();
				Path.AddEllipse(left, top, Diameter, Diameter);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		protected internal DisconnectorSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal DisconnectorSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}
	}


	public class AutoDisconnectorSymbol : ElectricalCircleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new AutoDisconnectorSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Diameter / 2f);
				int top = (int)Math.Round(-Diameter / 2f);
				int right = left + Diameter;

				Path.StartFigure();
				Path.AddLine(left, top, right, top);
				Path.AddEllipse(left, top, Diameter, Diameter);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				Rectangle result = Rectangle.Empty;
				result.X = X - (int)Math.Round(Diameter / 2f);
				result.Y = Y - (int)Math.Round(Diameter / 2f);
				result.Width = result.Height = Diameter;
				// Rotate top line
				if (Angle != 0) {
					float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
					Point tl = Point.Empty;
					tl.Offset(result.Left, result.Top);
					tl = Geometry.RotatePoint(X, Y, angleDeg, tl);
					Point tr = Point.Empty;
					tr.Offset(result.Right, result.Top);
					tr = Geometry.RotatePoint(X, Y, angleDeg, tr);
					Geometry.UniteRectangles(tl.X, tl.Y, tr.X, tr.Y, result);
					ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				}
				return result;
			} return base.CalculateBoundingRectangle(tight);
		}


		protected internal AutoDisconnectorSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal AutoDisconnectorSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}
	}


	public class AutoSwitchSymbol : ElectricalSquareBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new AutoSwitchSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				CalcShapeRectangles(out smallRectBuffer, out largeRectBuffer);

				Path.Reset();
				Path.StartFigure();
				Path.AddRectangle(smallRectBuffer);
				Path.AddRectangle(largeRectBuffer);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				CalcShapeRectangles(out smallRectBuffer, out largeRectBuffer);
				largeRectBuffer.Offset(X, Y);
				smallRectBuffer.Offset(X, Y);

				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);
				Point tl = Geometry.RotatePoint(X, Y, angle, largeRectBuffer.Left, largeRectBuffer.Top);
				Point tr = Geometry.RotatePoint(X, Y, angle, largeRectBuffer.Right, largeRectBuffer.Top);
				Point bl1 = Geometry.RotatePoint(X, Y, angle, largeRectBuffer.Left, largeRectBuffer.Bottom);
				Point br1 = Geometry.RotatePoint(X, Y, angle, largeRectBuffer.Right, largeRectBuffer.Bottom);
				Point bl2 = Geometry.RotatePoint(X, Y, angle, smallRectBuffer.Left, smallRectBuffer.Bottom);
				Point br2 = Geometry.RotatePoint(X, Y, angle, smallRectBuffer.Right, smallRectBuffer.Bottom);

				Rectangle result = Rectangle.Empty;
				Geometry.CalcBoundingRectangle(tl, tr, bl1, br1, out result);
				result = Geometry.UniteWithRectangle(bl2, result);
				result = Geometry.UniteWithRectangle(br2, result);

				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		protected internal AutoSwitchSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal AutoSwitchSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		private void CalcShapeRectangles(out Rectangle smallRectangle, out Rectangle largeRectangle) {
			smallRectangle = largeRectangle = Rectangle.Empty;

			int left = (int)Math.Round(-Size / 2f);
			int top = (int)Math.Round(-Size / 2f);
			int bottom = top + Size;

			smallRectangle.Width = (Size / 6) * 3;
			smallRectangle.Height = Size / 6;
			smallRectangle.X = left + ((Size - smallRectangle.Width) / 2);
			smallRectangle.Y = bottom - smallRectangle.Height;

			largeRectangle.Width = Size - (smallRectangle.Width / 4);
			largeRectangle.Height = Size - smallRectangle.Height;
			largeRectangle.X = left + ((Size - largeRectangle.Width) / 2);
			largeRectangle.Y = top;
		}


		private Rectangle smallRectBuffer;
		private Rectangle largeRectBuffer;
	}


	public class SwitchSymbol : ElectricalSquareBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new SwitchSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Size / 2f);
				int top = (int)Math.Round(-Size / 2f);

				shapeRect.X = left;
				shapeRect.Y = top;
				shapeRect.Width = Size;
				shapeRect.Height = Size;

				Path.StartFigure();
				Path.AddRectangle(shapeRect);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		protected internal SwitchSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal SwitchSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		#region Fields
		System.Drawing.Rectangle shapeRect;
		#endregion
	}


	public class BusBarSymbol : PolylineBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new BusBarSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override int MaxVertexCount {
			get { return 2; }
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			if ((controlPointCapability & ControlPointCapabilities.Connect) != 0)
				return true;
			if ((controlPointCapability & ControlPointCapabilities.Glue) != 0) {
				// always false
			}
			if ((controlPointCapability & ControlPointCapabilities.Reference) != 0) {
				if (controlPointId == ControlPointId.Reference || controlPointId == 1) return true;
			}
			if ((controlPointCapability & ControlPointCapabilities.Rotate) != 0) {
				// always false
			}
			if ((controlPointCapability & ControlPointCapabilities.Resize) != 0)
				return true;
			return false;
		}


		protected internal BusBarSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal BusBarSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}
	}


	public class TransformerSymbol : ElectricalRectangleBase {

		[Browsable(false)]
		public override IFillStyle FillStyle {
			get { return base.FillStyle; }
			set { base.FillStyle = value; }
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new TransformerSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			if ((controlPointCapability & ControlPointCapabilities.Connect) != 0) {
				if (controlPointId == TopCenterControlPoint || controlPointId == BottomCenterControlPoint)
					return true;
			}
			if ((controlPointCapability & ControlPointCapabilities.Glue) != 0) {
				// always false
			}
			if ((controlPointCapability & ControlPointCapabilities.Reference) != 0) {
				if (controlPointId == ControlPointId.Reference || controlPointId == MiddleCenterControlPoint)
					return true;
			}
			if ((controlPointCapability & ControlPointCapabilities.Rotate) != 0) {
				if (controlPointId == MiddleCenterControlPoint)
					return true;
			}
			if ((controlPointCapability & ControlPointCapabilities.Resize) != 0)
				if (controlPointId != -1 && controlPointId != MiddleCenterControlPoint)
					return true;
			return false;
		}


		protected override bool ContainsPointCore(int x, int y) {
			Rectangle upperRing, lowerRing;
			CalcRingBounds(out upperRing, out lowerRing);
			upperRing.Offset(X, Y);
			lowerRing.Offset(X, Y);
			float radius = upperRing.Width/2f;
			PointF upperRingCenter = Geometry.RotatePoint(X, Y, Geometry.TenthsOfDegreeToDegrees(Angle),upperRing.X + radius, upperRing.Y + radius);
			PointF lowerRingCenter = Geometry.RotatePoint(X, Y, Geometry.TenthsOfDegreeToDegrees(Angle),lowerRing.X + radius, lowerRing.Y + radius);
			return Geometry.CircleContainsPoint(upperRingCenter.X, upperRingCenter.Y, radius, x, y, 0)
				|| Geometry.CircleContainsPoint(lowerRingCenter.X, lowerRingCenter.Y, radius, x, y, 0);
		}


		public override void Draw(Graphics graphics) {
			Pen pen = ToolCache.GetPen(LineStyle, null, null);
			DrawOutline(graphics, pen);
		}


		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				CalcRingBounds(out upperRingBounds, out lowerRingBounds);
				float angle = Geometry.TenthsOfDegreeToDegrees(Angle);
				float ringRadius = upperRingBounds.Width / 2f;

				PointF upperCenter = PointF.Empty;
				upperCenter.X = X + (upperRingBounds.Left + upperRingBounds.Width / 2f);
				upperCenter.Y = Y + (upperRingBounds.Top + upperRingBounds.Height / 2f);
				PointF lowerCenter = PointF.Empty;
				lowerCenter.X = X + (lowerRingBounds.Left + lowerRingBounds.Width / 2f);
				lowerCenter.Y = Y + (lowerRingBounds.Top + lowerRingBounds.Height / 2f);

				upperCenter = Geometry.RotatePoint(X, Y, angle, upperCenter);
				lowerCenter = Geometry.RotatePoint(X, Y, angle, lowerCenter);

				Rectangle result = Rectangle.Empty;
				result.X = (int)Math.Round(Math.Min(upperCenter.X - ringRadius, lowerCenter.X - ringRadius));
				result.Y = (int)Math.Round(Math.Min(upperCenter.Y - ringRadius, lowerCenter.Y - ringRadius));
				result.Width = (int)Math.Round(Math.Max(upperCenter.X + ringRadius, lowerCenter.X + ringRadius)) - result.X;
				result.Height = (int)Math.Round(Math.Max(upperCenter.Y + ringRadius, lowerCenter.Y + ringRadius)) - result.Y;

				result = Geometry.UniteWithRectangle(Geometry.RotatePoint(X, Y, angle, X, Y - (Height / 2)), result);
				result = Geometry.UniteWithRectangle(Geometry.RotatePoint(X, Y, angle, X, Y - (Height / 2) + Height), result);

				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		protected internal TransformerSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal TransformerSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			FillStyle = styleSet.FillStyles.Transparent;
			Width = 40;
			Height = 70;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			Path.Reset();

			int top = -Height / 2;
			int bottom = -(Height / 2) + Height;
			CalcRingBounds(out upperRingBounds, out lowerRingBounds);

			// Add the lines between connection point and circles only if 
			// necessary. Otherwise, DrawPath() throws an OutOfMemoryException 
			// on Windows XP/Vista when drawing with a pen thicker than 1 pixel...
			Path.StartFigure();
			if (top < upperRingBounds.Top)
				Path.AddLine(0, top, 0, upperRingBounds.Top);
			Path.AddEllipse(upperRingBounds);
			Path.AddEllipse(lowerRingBounds);
			if (bottom > lowerRingBounds.Bottom)
				Path.AddLine(0, lowerRingBounds.Bottom, 0, bottom);
			Path.CloseFigure();

			return true;
		}


		private void CalcRingBounds(out Rectangle upperRingBounds, out Rectangle lowerRingBounds) {
			upperRingBounds = lowerRingBounds = Rectangle.Empty;

			float ringDiameter = Math.Min(5 * (Height / 8f), Width);
			float d = Math.Min(Height / 8f, Width / 4f);
			int left = (int)Math.Round(-ringDiameter / 2f);

			upperRingBounds.X = left;
			upperRingBounds.Y = (int)Math.Round(-ringDiameter + d);
			upperRingBounds.Width = (int)Math.Round(ringDiameter);
			upperRingBounds.Height = (int)Math.Round(ringDiameter);

			lowerRingBounds.X = left;
			lowerRingBounds.Y = (int)Math.Round(- d);
			lowerRingBounds.Width = (int)Math.Round(ringDiameter);
			lowerRingBounds.Height = (int)Math.Round(ringDiameter);
		}


		#region Fields

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

		Rectangle upperRingBounds;
		Rectangle lowerRingBounds;

		#endregion
	}


	public class EarthSymbol : ElectricalRectangleBase {

		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Width = 40;
			Height = 40;
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new EarthSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0
							|| ((controlPointCapability & ControlPointCapabilities.Connect) != 0
								&& IsConnectionPointEnabled(controlPointId)));
				case TopLeftControlPoint:
				case TopRightControlPoint:
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
				case BottomLeftControlPoint:
				case BottomCenterControlPoint:
				case BottomRightControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
				case MiddleCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Reference) != 0
							|| (controlPointCapability & ControlPointCapabilities.Rotate) != 0);
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
				//return false;
			}
		}


		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				CalcShapePoints(ref shapeBuffer);
				
				Matrix.Reset();
				Matrix.Translate(X, Y);
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center, System.Drawing.Drawing2D.MatrixOrder.Append);
				Matrix.TransformPoints(shapeBuffer);

				Rectangle result ;
				Geometry.CalcBoundingRectangle(shapeBuffer, out result);
				ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				return result;
			} else return base.CalculateBoundingRectangle(tight);
		}


		protected internal EarthSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal EarthSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				CalcShapePoints(ref shapeBuffer);

				Path.Reset();
				Path.StartFigure();
				Path.AddLines(shapeBuffer);
				Path.CloseFigure();

				return true;
			}
			return false;
		}


		private void CalcShapePoints(ref Point[] points) {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			int lineWidth = LineStyle.LineWidth;
			int lineX = -lineWidth;
			int largeAntennaTop = -lineWidth;
			int largeAntennaBottom = lineWidth;
			int mediumAntennaTop = bottom - (int)Math.Round(Height / 4f) - lineWidth;
			int mediumAntennaBottom = Math.Min(bottom, bottom - (int)Math.Round(Height / 4f) + lineWidth);
			int smallAntennaTop = bottom - lineWidth - lineWidth;
	
			if (points == null) points = new Point[25];

			// downward line from top to large 'antenna', left side
			points[0].X = lineX;
			points[0].Y = top;
			points[1].X = lineX;
			points[1].Y = largeAntennaTop;

			// large 'antenna', left side
			points[2].X = left;
			points[2].Y = largeAntennaTop;
			points[3].X = left;
			points[3].Y = largeAntennaBottom;
			points[4].X = lineX;
			points[4].Y = largeAntennaBottom;

			// downward line from large 'antenna' to medium 'antenna', left side
			points[5].X = lineX;
			points[5].Y = mediumAntennaTop;
			
			// medium 'antenna', left side
			int antennaLeft = left + (int)Math.Round(Width / 6f);
			points[6].X = antennaLeft;
			points[6].Y = mediumAntennaTop;
			points[7].X = antennaLeft;
			points[7].Y = mediumAntennaBottom;
			points[8].X = lineX;
			points[8].Y = mediumAntennaBottom;
			
			// downward line from medium 'antenna' to small 'antenna', left side				
			points[9].X = lineX;
			points[9].Y = smallAntennaTop;
			
			// small 'antenna', complete
			antennaLeft = left + (int)Math.Round(Width / 3f);
			int antennaRight = right - (int)Math.Round(Width / 3f);
			points[10].X = antennaLeft;
			points[10].Y = smallAntennaTop;
			points[11].X = antennaLeft;
			points[11].Y = bottom;
			points[12].X = antennaRight;
			points[12].Y = bottom;
			lineX = lineWidth;
			points[13].X = antennaRight;
			points[13].Y = smallAntennaTop;
			points[14].X = lineX;
			points[14].Y = smallAntennaTop;

			// upward line from small 'antenna' to medium 'antenna', right side
			points[15].X = lineX;
			points[15].Y = mediumAntennaBottom;

			// medium 'antenna', right side
			antennaRight = right - (int)Math.Round(Width / 6f);
			points[16].X = antennaRight;
			points[16].Y = mediumAntennaBottom;
			points[17].X = antennaRight;
			points[17].Y = mediumAntennaTop;
			points[18].X = lineX;
			points[18].Y = mediumAntennaTop;

			// upward line from medium 'antenna' to large 'antenna', right side
			points[19].X = lineX;
			points[19].Y = largeAntennaBottom;

			// large 'antenna', right side
			points[20].X = right;
			points[20].Y = largeAntennaBottom;
			points[21].X = right;
			points[21].Y = largeAntennaTop;
			points[22].X = lineX;
			points[22].Y = largeAntennaTop;

			// upward line from large 'antenna' to top, right side
			points[23].X = lineX;
			points[23].Y = top;
			points[24].X =-lineWidth ;
			points[24].Y = top;
		}


		#region Fields

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

		//private Rectangle shapeBuffer = Rectangle.Empty;
		private Point[] shapeBuffer = null;

		#endregion
	}


	public class FeederSymbol : ElectricalTriangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new FeederSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal FeederSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal FeederSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}
	}


	public class RectifierSymbol : ElectricalTriangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new RectifierSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width * CenterPosFactorX);
				int top = (int)Math.Round(-Height * CenterPosFactorY);
				int right = left + Width;
				int bottom = top + Height;

				Path.StartFigure();
				Path.AddLine(left, top, right, top);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			// Tight and loose bounding rectangles are equal
			Rectangle result = Geometry.InvalidRectangle;
			if (Width >= 0 && Height >= 0) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height * CenterPosFactorY);
				int right = left + Width;
				int bottom = (int)Math.Round(Height - (Height * CenterPosFactorY));

				if (Angle == 0 || Angle == 1800) {
					result.X = X + left;
					result.Y = (Angle == 0) ? Y + top : Y - bottom;
					result.Width = right - left;
					result.Height = bottom - top;
				} else if (Angle == 900 || Angle == 2700) {
					result.X = (Angle == 900) ? X - bottom : result.X = X + top;
					result.Y = Y + left;
					result.Width = bottom - top;
					result.Height = right - left;
				} else {
					float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
					int x1, y1, x2, y2, x3, y3, x4, y4;

					x1 = X + left; y1 = Y + top;
					x2 = X + right; y2 = Y + top;
					x3 = X + right; y3 = Y + bottom;
					x4 = X + left; y4 = Y + bottom;
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


		protected internal RectifierSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal RectifierSymbol(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}
	}


	public class DisconnectingPoint : ElectricalRectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new DisconnectingPoint(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0
							|| ((controlPointCapability & ControlPointCapabilities.Connect) != 0
								&& IsConnectionPointEnabled(controlPointId)));
				case TopLeftControlPoint:
				case TopCenterControlPoint:
				case TopRightControlPoint:
				case BottomLeftControlPoint:
				case BottomCenterControlPoint:
				case BottomRightControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
				case MiddleCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Reference) != 0
							|| (controlPointCapability & ControlPointCapabilities.Rotate) != 0);
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
				//return false;
			}
		}


		protected internal DisconnectingPoint(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal DisconnectingPoint(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;

				int offsetX = (int)Math.Round(Width / 6f);
				int offsetY = (int)Math.Round(Height / 3f);

				Path.StartFigure();
				Path.AddLine(left, top, left, bottom);
				Path.CloseFigure();
				Path.StartFigure();
				Path.AddLine(left, top, left, bottom);
				Path.CloseFigure();
				Path.StartFigure();
				Path.AddLine(left + offsetX, top + offsetY, left + offsetX, bottom - offsetY);
				Path.CloseFigure();
				Path.StartFigure();
				Path.AddLine(left + offsetX + offsetX, 0, right - offsetX - offsetX, 0);
				Path.CloseFigure();
				Path.StartFigure();
				Path.AddLine(right - offsetX, top + offsetY, right - offsetX, bottom - offsetY);
				Path.CloseFigure();
				Path.StartFigure();
				Path.AddLine(right, top, right, bottom);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		#region Fields

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

		private Rectangle shapeBuffer = Rectangle.Empty;
		#endregion
	}


	public static class NShapeLibraryInitializer {

		public static void Initialize(IRegistrar registrar) {
			registrar.RegisterLibrary(libraryName, preferredRepositoryVersion);
			registrar.RegisterShapeType(new ShapeType("BusBar", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new BusBarSymbol(shapeType, t); },
				BusBarSymbol.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceHorizontalBar));
			registrar.RegisterShapeType(new ShapeType("Disconnector", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new DisconnectorSymbol(shapeType, t); },
				DisconnectorSymbol.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceCircleWithBar));
			registrar.RegisterShapeType(new ShapeType("AutoDisconnector", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new AutoDisconnectorSymbol(shapeType, t); },
				AutoDisconnectorSymbol.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceCircleWithBar));
			registrar.RegisterShapeType(new ShapeType("AutoSwitch", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new AutoSwitchSymbol(shapeType, t); },
				AutoSwitchSymbol.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("Switch", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new SwitchSymbol(shapeType, t); },
				SwitchSymbol.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("Transformer", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new TransformerSymbol(shapeType, t); },
				TransformerSymbol.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceDoubleCircle));
			registrar.RegisterShapeType(new ShapeType("Earth", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new EarthSymbol(shapeType, t); },
				EarthSymbol.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceEarthSymbol));
			registrar.RegisterShapeType(new ShapeType("Feeder", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new FeederSymbol(shapeType, t); },
				FeederSymbol.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceEarthSymbol));
			registrar.RegisterShapeType(new ShapeType("Rectifier", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new RectifierSymbol(shapeType, t); },
				RectifierSymbol.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceEarthSymbol));
			registrar.RegisterShapeType(new ShapeType("DisconnectingPoint", libraryName, libraryName,
				delegate(ShapeType shapeType, Template t) { return new DisconnectingPoint(shapeType, t); },
				DisconnectingPoint.GetPropertyDefinitions, Dataweb.NShape.ElectricalShapes.Properties.Resources.ShaperReferenceEarthSymbol));
		}


		private const string libraryName = "ElectricalShapes";
		private const int preferredRepositoryVersion = 3;
	}

}