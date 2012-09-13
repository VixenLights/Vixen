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
using System.Drawing.Drawing2D;


namespace Dataweb.NShape.Advanced {

	// FreeTriangle as base for the FreePolygon
	/// <ToBeCompleted></ToBeCompleted>
	public class TriangleBase : CaptionedShapeBase {

		/// <override></override>
		public override void CopyFrom(Shape source) {
			base.CopyFrom(source);
			if (source is TriangleBase) {
				TriangleBase src = (TriangleBase)source;
				this.shapePoints[0] = src.shapePoints[0];
				this.shapePoints[1] = src.shapePoints[1];
				this.shapePoints[2] = src.shapePoints[2];
			}
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new TriangleBase(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override IEnumerable<ControlPointId> GetControlPointIds(ControlPointCapabilities controlPointCapability) {
			return base.GetControlPointIds(controlPointCapability);
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case ControlPoint1:
				case ControlPoint2:
				case ControlPoint3:
					return ((controlPointCapability & ControlPointCapabilities.Resize) > 0
								|| ((controlPointCapability & ControlPointCapabilities.Connect) > 0 && IsConnectionPointEnabled(controlPointId)));
				case RotateControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Rotate) > 0
								|| (controlPointCapability & ControlPointCapabilities.Reference) > 0
								|| ((controlPointCapability & ControlPointCapabilities.Connect) > 0 && IsConnectionPointEnabled(controlPointId)));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			if (X == startX && Y == startY)
				return Center;
			else {
				Array.Copy(shapePoints, pointBuffer, pointBuffer.Length);
				Matrix.Reset();
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Point.Empty, MatrixOrder.Prepend);
				Matrix.Translate(X, Y, MatrixOrder.Append);
				Matrix.TransformPoints(pointBuffer);

				Point result = Geometry.GetNearestPoint(startX, startY, Geometry.IntersectPolygonLine(pointBuffer, X, Y, startX, startY, false));
				return result;
			}
		}


		/// <override></override>
		public override RelativePosition CalculateRelativePosition(int x, int y) {
			// Definition of the relative position:
			// Let posA be thwe intersection point of A|xy and B|C
			// Let posB be the intersection point of B|xy and A|C
			// The relative position is the intersection point of A|posA and B|posB
			// Storage in the relative position's fields:
			// RelativePosition.A = Tenths of percentage of BC
			// RelativePosition.B = Tenths of percentage of AC
			int sX = X, sY = Y;
			float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
			Point p1 = Geometry.RotatePoint(sX, sY, angleDeg, shapePoints[0].X + sX, shapePoints[0].Y + sY);
			Point p2 = Geometry.RotatePoint(sX, sY, angleDeg, shapePoints[1].X + sX, shapePoints[1].Y + sY);
			Point p3 = Geometry.RotatePoint(sX, sY, angleDeg, shapePoints[2].X + sX, shapePoints[2].Y + sY);
			Point posA, posB;
			if (p1.X == x && p1.Y == y)
				posA = Geometry.VectorLinearInterpolation(p2, p3, 0.5f);
			else posA = Geometry.IntersectLines(p1.X, p1.Y, x, y, p2.X, p2.Y, p3.X, p3.Y);
			if (p2.X == x && p2.Y == y)
				posB = Geometry.VectorLinearInterpolation(p1, p3, 0.5f);
			else posB = Geometry.IntersectLines(p2.X, p2.Y, x, y, p1.X, p1.Y, p3.X, p3.Y);

			float distA = Geometry.DistancePointPoint(p2, posA) / Geometry.DistancePointPoint(p2, p3);
			float distB = Geometry.DistancePointPoint(p1, posB) / Geometry.DistancePointPoint(p1, p3);

			RelativePosition result = RelativePosition.Empty;
			result.A = (int)Math.Round(distA * 1000);
			result.B = (int)Math.Round(distB * 1000);
			return result;
		}


		/// <override></override>
		public override Point CalculateAbsolutePosition(RelativePosition relativePosition) {
			// Definition of the relative position:
			// Let posA be thwe intersection point of A|xy and B|C
			// Let posB be the intersection point of B|xy and A|C
			// The relative position is the intersection point of A|posA and B|posB
			// Storage in the relative position's fields:
			// RelativePosition.A = Tenths of percentage of BC
			// RelativePosition.B = Tenths of percentage of AC
			int sX = X, sY = Y;
			float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
			Point p1 = Geometry.RotatePoint(sX, sY, angleDeg, shapePoints[0].X + sX, shapePoints[0].Y + sY);
			Point p2 = Geometry.RotatePoint(sX, sY, angleDeg, shapePoints[1].X + sX, shapePoints[1].Y + sY);
			Point p3 = Geometry.RotatePoint(sX, sY, angleDeg, shapePoints[2].X + sX, shapePoints[2].Y + sY);
			float distA = relativePosition.A / 1000f;
			float distB = relativePosition.B / 1000f;
			
			Point posA = Geometry.VectorLinearInterpolation(p2, p3, distA);
			Point posB = Geometry.VectorLinearInterpolation(p1, p3, distB);

			Point result = Geometry.IntersectLines(p1.X, p1.Y, posA.X, posA.Y, p2.X, p2.Y, posB.X, posB.Y);
			return result;
		}


		/// <override></override>
		public override ControlPointId HitTest(int x, int y, ControlPointCapabilities controlPointCapability, int range) {
			ControlPointId result = base.HitTest(x, y, controlPointCapability, range);
			if (result != ControlPointId.None)
				return result;

			UpdateDrawCache();
			for (int i = 0; i < 3; ++i) {
				int j = i < 2 ? i + 1 : 0;
				int x1 = controlPoints[i].X + X;
				int y1 = controlPoints[i].Y + Y;
				if (Geometry.DistancePointPoint(x, y, x1, y1) <= range)
					return i + 1;
			}
			if ((controlPointCapability & ControlPointCapabilities.Rotate) > 0)
				if (Geometry.DistancePointPoint(X, Y, x, y) <= range)
					return RotateControlPoint;
			return ControlPointId.None;
		}


		/// <override></override>
		public override void Draw(Graphics graphics) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			DrawPath(graphics, LineStyle, FillStyle);
			DrawCaption(graphics);
			base.Draw(graphics);	// Draw children and outline
		}


		/// <override></override>
		public override void Fit(int x, int y, int width, int height) {
			Rectangle dstBounds = Rectangle.Empty;
			dstBounds.Offset(x, y);
			dstBounds.Width = width; dstBounds.Height = height;

			// Get current positions of control points
			Rectangle srcBounds = CalculateBoundingRectangle(true);
			Point p1 = GetControlPointPosition(1);
			Point p2 = GetControlPointPosition(2);
			Point p3 = GetControlPointPosition(3);
			Point pos = GetControlPointPosition(ControlPointId.Reference);
			
			float scaleX = dstBounds.Width / (float)srcBounds.Width;
			float scaleY = dstBounds.Height / (float)srcBounds.Height;

			// Scale shape to the desired size
			int dx, dy;
			dx = (int)Math.Round(scaleX * (p1.X - srcBounds.X));
			dy = (int)Math.Round(scaleY * (p1.Y - srcBounds.Y));
			MoveControlPointTo(1, dstBounds.X + dx, dstBounds.Y + dy, ResizeModifiers.None);
			dx = (int)Math.Round(scaleX * (p2.X - srcBounds.X));
			dy = (int)Math.Round(scaleY * (p2.Y - srcBounds.Y));
			MoveControlPointTo(2, dstBounds.X + dx, dstBounds.Y + dy, ResizeModifiers.None);
			dx = (int)Math.Round(scaleX * (p3.X - srcBounds.X));
			dy = (int)Math.Round(scaleY * (p3.Y - srcBounds.Y));
			MoveControlPointTo(3, dstBounds.X + dx, dstBounds.Y + dy, ResizeModifiers.None);
			dx = (int)Math.Round(scaleX * (pos.X - srcBounds.Left));
			dy = (int)Math.Round(scaleY * (pos.Y - srcBounds.Top));
			MoveTo(dstBounds.X + dx, dstBounds.Y + dy);
		}


		#region IEntity Members

		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version) {
			base.LoadFieldsCore(reader, version);
			shapePoints[0].X = reader.ReadInt32();
			shapePoints[0].Y = reader.ReadInt32();
			shapePoints[1].X = reader.ReadInt32();
			shapePoints[1].Y = reader.ReadInt32();
			shapePoints[2].X = reader.ReadInt32();
			shapePoints[2].Y = reader.ReadInt32();
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version) {
			base.SaveFieldsCore(writer, version);
			writer.WriteInt32(shapePoints[0].X);
			writer.WriteInt32(shapePoints[0].Y);
			writer.WriteInt32(shapePoints[1].X);
			writer.WriteInt32(shapePoints[1].Y);
			writer.WriteInt32(shapePoints[2].X);
			writer.WriteInt32(shapePoints[2].Y);
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.TriangleBase" />.
		/// </summary>
		new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
			foreach (EntityPropertyDefinition pi in CaptionedShapeBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("A_X", typeof(int));
			yield return new EntityFieldDefinition("A_Y", typeof(int));
			yield return new EntityFieldDefinition("B_X", typeof(int));
			yield return new EntityFieldDefinition("B_Y", typeof(int));
			yield return new EntityFieldDefinition("C_X", typeof(int));
			yield return new EntityFieldDefinition("C_Y", typeof(int));
		}

		#endregion


		/// <override></override>
		protected internal override int ControlPointCount {
			get { return 4; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal TriangleBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
			Construct();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal TriangleBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
			Construct();
		}


		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			pointBuffer[0].X = 0;
			pointBuffer[0].Y = -20;
			pointBuffer[1].X = -20;
			pointBuffer[1].Y = 20;
			pointBuffer[2].X = 20;
			pointBuffer[2].Y = 20;
			int x, y;
			Geometry.CalcPolygonBalancePoint(pointBuffer, out x, out y);

			for (int i = shapePoints.Length - 1; i >= 0; --i) {
				shapePoints[i].X = pointBuffer[i].X - x;
				shapePoints[i].Y = pointBuffer[i].Y - y;
			}
		}


		/// <override></override>
		protected override int DivFactorX {
			get { return 1; }
		}


		/// <override></override>
		protected override int DivFactorY {
			get { return 1; }
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			Rectangle result;
			Array.Copy(shapePoints, pointBuffer, pointBuffer.Length);
			Matrix.Reset();
			Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Point.Empty);
			Matrix.TransformPoints(pointBuffer);

			Geometry.CalcBoundingRectangle(pointBuffer, out result);
			result.Offset(X, Y);
			ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);

			return result;
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			// Transform x|y to 0|0 before comparing with (the untransformed) triangle.
			// Instead of rotating the triangle, we rotate the point in the opposite direction.
			if (Geometry.IsValid(x, y)) {
				Point p = Point.Empty;
				p.Offset(x - X, y - Y);
				p = Geometry.RotatePoint(Point.Empty, Geometry.TenthsOfDegreeToDegrees(-Angle), p);
				return Geometry.TriangleContainsPoint(shapePoints[0], shapePoints[1], shapePoints[2], p.X, p.Y);
			} else return false;
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			// Transform the rectangle 0|0 before comparing it with the (untransformed) shapePoints
			int rX = x - X; int rY = y - Y;
			float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
			pointBuffer[0] = Geometry.RotatePoint(Center, angleDeg, shapePoints[0]);
			pointBuffer[1] = Geometry.RotatePoint(Center, angleDeg, shapePoints[1]);
			pointBuffer[2] = Geometry.RotatePoint(Center, angleDeg, shapePoints[2]);
			return Geometry.PolygonIntersectsWithRectangle(pointBuffer, rX, rY, rX + width, rY + height);
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers) {
			int idx = GetControlPointIndex(pointId);
			Debug.Assert(idx >= 0);

			int oldPosX, oldPosY;
			Geometry.CalcPolygonBalancePoint(shapePoints, out oldPosX, out oldPosY);

			shapePoints[idx].X += (int)Math.Round(transformedDeltaX);
			shapePoints[idx].Y += (int)Math.Round(transformedDeltaY);

			int newPosX, newPosY;
			Geometry.CalcPolygonBalancePoint(shapePoints, out newPosX, out newPosY);

			// Update (relative) vertex positions before moving the shape to the new balance point
			int dX = newPosX - oldPosX;
			int dY = newPosY - oldPosY;
			for (int i = shapePoints.Length - 1; i >= 0; --i) {
				shapePoints[i].X -= dX;
				shapePoints[i].Y -= dY;
			}
			MoveByCore((int)Math.Round((dX * cos) - (dY * sin)), (int)Math.Round((dX * sin) + (dY * cos)));
			
			return true;
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			captionBounds = Geometry.InvalidRectangle;
			//Geometry.CalcBoundingRectangle(shapePoints, out captionBounds);
			// Calculate the inner circle
			int circleRadius = int.MinValue;
			int j;
			for (int i = shapePoints.Length - 1; i >= 0; --i) {
				j = (i == 0) ? shapePoints.Length - 1 : i - 1;
				int x1 = shapePoints[i].X; int y1 = shapePoints[i].Y;
				int x2 = shapePoints[j].X; int y2 = shapePoints[j].Y;
				Point p = Geometry.CalcPointOnLine(x1, y1, x2, y2, Geometry.DistancePointPoint(x1, y1, x2, y2) / 2f);
				float dist = Geometry.DistancePointPoint(Point.Empty, p);
				if (dist > circleRadius) circleRadius = (int)Math.Round(dist);
			}
			captionBounds.X = -circleRadius;
			captionBounds.Y = -circleRadius;
			captionBounds.Width = captionBounds.Height = circleRadius + circleRadius;
			if (ParagraphStyle != null) {
				captionBounds.X += ParagraphStyle.Padding.Left;
				captionBounds.Y += ParagraphStyle.Padding.Top;
				captionBounds.Width -= ParagraphStyle.Padding.Horizontal;
				captionBounds.Height -= ParagraphStyle.Padding.Vertical;
			}
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			int cnt = shapePoints.Length;
			for (int i = 0; i < cnt; ++i) {
				controlPoints[i].X = shapePoints[i].X;
				controlPoints[i].Y = shapePoints[i].Y;
			}
			controlPoints[ControlPointCount - 1] = Point.Empty;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				return true;
			} else return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void Construct() {
		}


		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected const int ControlPoint1 = 1;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int ControlPoint2 = 2;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int ControlPoint3 = 3;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int RotateControlPoint = 4;

		// The vertices contain the (unrotated) points of the triangle relative to 0|0.
		private Point[] shapePoints = new Point[3];
		private Point[] pointBuffer = new Point[3];
		
		#endregion
	}

}
