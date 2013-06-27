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


namespace Dataweb.NShape.Advanced
{
	/// <ToBeCompleted></ToBeCompleted>
	public class RegularPolygoneBase : CaptionedShapeBase
	{
		/// <ToBeCompleted></ToBeCompleted>
		public int VertexCount
		{
			get { return pointCount; }
			set
			{
				if (value < 3) throw new ArgumentOutOfRangeException("VertexCount has to be >= 3");
				if (pointCount != value) {
					BeginResize();

					pointCount = value;
					Array.Resize(ref controlPoints, ControlPointCount);
					UpdateShapePoints();

					EndResize(1, 1);
				}
			}
		}


		/// <override></override>
		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is RegularPolygoneBase) {
				RegularPolygoneBase src = (RegularPolygoneBase) source;
				VertexCount = src.VertexCount;
				diameter = src.diameter;
				if (pointCount != src.pointCount)
					Array.Resize(ref shapePoints, src.pointCount);
				Array.Copy(src.shapePoints, shapePoints, shapePoints.Length);
				for (int i = shapePoints.Length - 1; i >= 0; --i)
					shapePoints[i] = src.shapePoints[i];
				CalcControlPoints();
				drawCacheIsInvalid = true;
			}
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new RegularPolygoneBase(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override IEnumerable<ControlPointId> GetControlPointIds(ControlPointCapabilities controlPointCapability)
		{
			return base.GetControlPointIds(controlPointCapability);
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId,
		                                               ControlPointCapabilities controlPointCapability)
		{
			if (controlPointId == controlPoints.Length) {
				return ((controlPointCapability & ControlPointCapabilities.Rotate) > 0
				        || (controlPointCapability & ControlPointCapabilities.Reference) > 0
				        ||
				        ((controlPointCapability & ControlPointCapabilities.Connect) > 0 && IsConnectionPointEnabled(controlPointId)));
			}
			else if (controlPointId >= 1) {
				return ((controlPointCapability & ControlPointCapabilities.Resize) > 0
				        ||
				        ((controlPointCapability & ControlPointCapabilities.Connect) > 0 && IsConnectionPointEnabled(controlPointId)));
			}
			else
				return base.HasControlPointCapability(controlPointId, controlPointCapability);
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY)
		{
			if (X == startX && Y == startY)
				return Center;
			else {
				Array.Copy(shapePoints, pointBuffer, pointBuffer.Length);
				Matrix.Reset();
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Point.Empty, MatrixOrder.Prepend);
				Matrix.Translate(X, Y, MatrixOrder.Append);
				Matrix.TransformPoints(pointBuffer);

				Point result = Geometry.GetNearestPoint(startX, startY,
				                                        Geometry.IntersectPolygonLine(pointBuffer, X, Y, startX, startY, false));
				return result;
			}
		}


		/// <override></override>
		public override RelativePosition CalculateRelativePosition(int x, int y)
		{
			// Definition of the relative position:
			// Let posA be thwe intersection point of A|xy and B|C
			// Let posB be the intersection point of B|xy and A|C
			// The relative position is the intersection point of A|posA and B|posB
			// Storage in the relative position's fields:
			// RelativePosition.A = Tenths of percentage of BC
			// RelativePosition.B = Tenths of percentage of AC

			float angle = Geometry.RadiansToDegrees(Geometry.Angle(X, Y, x, y));
			float dist = Geometry.DistancePointPoint(X, Y, x, y)/diameter;

			RelativePosition result = RelativePosition.Empty;
			result.A = (int) Math.Round(angle*1000);
			result.B = (int) Math.Round(dist*1000);
			return result;
		}


		/// <override></override>
		public override Point CalculateAbsolutePosition(RelativePosition relativePosition)
		{
			// Definition of the relative position:
			// Let posA be thwe intersection point of A|xy and B|C
			// Let posB be the intersection point of B|xy and A|C
			// The relative position is the intersection point of A|posA and B|posB
			// Storage in the relative position's fields:
			// RelativePosition.A = Tenths of percentage of BC
			// RelativePosition.B = Tenths of percentage of AC
			float angle = Geometry.TenthsOfDegreeToDegrees(relativePosition.A);
			float dist = (relativePosition.B/1000f)*diameter;

			Point result = Point.Round(Geometry.CalcPoint(X, Y, angle, dist));
			return result;
		}


		/// <override></override>
		public override ControlPointId HitTest(int x, int y, ControlPointCapabilities controlPointCapability, int range)
		{
			ControlPointId result = base.HitTest(x, y, controlPointCapability, range);
			if (result != ControlPointId.None)
				return result;

			UpdateDrawCache();
			for (int i = 0; i < pointCount; ++i) {
				int j = i < 2 ? i + 1 : 0;
				int x1 = controlPoints[i].X + X;
				int y1 = controlPoints[i].Y + Y;
				if (Geometry.DistancePointPoint(x, y, x1, y1) <= range)
					return i + 1;
			}
			if ((controlPointCapability & ControlPointCapabilities.Rotate) > 0)
				if (Geometry.DistancePointPoint(X, Y, x, y) <= range)
					return controlPoints.Length;
			return ControlPointId.None;
		}


		/// <override></override>
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) throw new ArgumentNullException("graphics");
			DrawPath(graphics, LineStyle, FillStyle);
			DrawCaption(graphics);
			base.Draw(graphics); // Draw children and outline
		}


		/// <override></override>
		public override void Fit(int x, int y, int width, int height)
		{
			Rectangle dstBounds = Rectangle.Empty;
			dstBounds.Offset(x, y);
			dstBounds.Width = width;
			dstBounds.Height = height;

			// Get current bounds
			Rectangle srcBounds = CalculateBoundingRectangle(true);

			InvalidateDrawCache();
			float scaleFactor = Math.Min(dstBounds.Width/(float) srcBounds.Width, dstBounds.Height/(float) srcBounds.Height);
			diameter = (int) Math.Round(diameter*scaleFactor);
			UpdateShapePoints();

			// Scale shape to the desired size
			MoveTo(dstBounds.X + (int) Math.Round(dstBounds.Width/2f), dstBounds.Y + (int) Math.Round(dstBounds.Height/2f));
		}

		#region IEntity Members

		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version)
		{
			base.LoadFieldsCore(reader, version);
			diameter = reader.ReadFloat();
			VertexCount = reader.ReadInt32();
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version)
		{
			base.SaveFieldsCore(writer, version);
			writer.WriteFloat(diameter);
			writer.WriteInt32(VertexCount);
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.RegularPolygonBase" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in CaptionedShapeBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("Diameter", typeof (float));
			yield return new EntityFieldDefinition("VertexCount", typeof (int));
		}

		#endregion

		/// <override></override>
		protected internal override int ControlPointCount
		{
			get { return pointCount + 1; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal RegularPolygoneBase(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			Construct();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal RegularPolygoneBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			Construct();
		}


		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);
			FillStyle = styleSet.FillStyles.Blue;
		}


		/// <override></override>
		protected override int DivFactorX
		{
			get
			{
				if (pointCount%2 == 0) return 2;
				else if (pointCount%3 == 0) return 3;
				else if (pointCount%5 == 0) return 5;
				else if (pointCount%7 == 0) return 7;
				else return 1;
			}
		}


		/// <override></override>
		protected override int DivFactorY
		{
			get { return DivFactorX; }
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight)
		{
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
		protected override bool ContainsPointCore(int x, int y)
		{
			// Transform x|y to 0|0 before comparing with (the untransformed) polygon.
			// Instead of rotating the triangle, we rotate the point in the opposite direction.
			if (Geometry.IsValid(x, y)) {
				Point p = Point.Empty;
				p.Offset(x - X, y - Y);
				p = Geometry.RotatePoint(Point.Empty, Geometry.TenthsOfDegreeToDegrees(-Angle), p);
				return Geometry.PolygonContainsPoint(shapePoints, p.X, p.Y);
			}
			else return false;
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height)
		{
			// Transform the rectangle 0|0 before comparing it with the (untransformed) shapePoints
			float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
			Debug.Assert(pointCount == pointBuffer.Length);
			Array.Copy(shapePoints, pointBuffer, pointCount);
			Matrix.Reset();
			Matrix.RotateAt(angleDeg, Point.Empty, MatrixOrder.Prepend);
			Matrix.Translate(X, Y);
			Matrix.TransformPoints(pointBuffer);
			return Geometry.PolygonIntersectsWithRectangle(pointBuffer, x, y, x + width, y + height);
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY,
		                                        float sin, float cos, ResizeModifiers modifiers)
		{
			int idx = GetControlPointIndex(pointId);
			Debug.Assert(idx >= 0);

			// Normalize the movement to the first vector
			float segmentAngle = 360/(float) pointCount;
			float ptAngle = (pointId - 1)*segmentAngle;
			float normalizedDeltaX = transformedDeltaX;
			float normalizedDeltaY = transformedDeltaY;
			Geometry.RotatePoint(0, 0, -ptAngle, ref normalizedDeltaX, ref normalizedDeltaY);

			//int tmpY = (int)diameter + 100;
			//Point p = Geometry.GetNearestPoint(0, tmpY, Geometry.IntersectPolygonLine(shapePoints, 0, 0, 0, tmpY, false));
			//float dist = diameter - Geometry.DistancePointPoint(0, 0, p.X, p.Y);
			//float movementCorrectionFactor = (float)Math.Round(Geometry.DistancePointPoint(0, 0, p.X, p.Y) / diameter, 1);

			float dx = 0;
			float dy = (normalizedDeltaY/2); // * movementCorrectionFactor;
			diameter = (int) Math.Round(diameter - (normalizedDeltaY - dy));
			UpdateShapePoints();

			// Update shape position
			Geometry.RotatePoint(0, 0, ptAngle, ref dx, ref dy);
			MoveByCore((int) Math.Round((dx*cos) - (dy*sin)), (int) Math.Round((dx*sin) + (dy*cos)));

			return (normalizedDeltaX == 0);
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds)
		{
			captionBounds = Geometry.InvalidRectangle;
			//Geometry.CalcBoundingRectangle(shapePoints, out captionBounds);
			// Calculate the inner circle
			int circleRadius = int.MinValue;
			int j;
			for (int i = shapePoints.Length - 1; i >= 0; --i) {
				j = (i == 0) ? shapePoints.Length - 1 : i - 1;
				int x1 = shapePoints[i].X;
				int y1 = shapePoints[i].Y;
				int x2 = shapePoints[j].X;
				int y2 = shapePoints[j].Y;
				Point p = Geometry.CalcPointOnLine(x1, y1, x2, y2, Geometry.DistancePointPoint(x1, y1, x2, y2)/2f);
				float dist = Geometry.DistancePointPoint(Point.Empty, p);
				if (dist > circleRadius) circleRadius = (int) Math.Round(dist);
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
		protected override void CalcControlPoints()
		{
			if (controlPoints.Length != ControlPointCount)
				Array.Resize(ref controlPoints, ControlPointCount);
			int cnt = pointCount;
			for (int i = 0; i < cnt; ++i) {
				controlPoints[i].X = shapePoints[i].X;
				controlPoints[i].Y = shapePoints[i].Y;
			}
			controlPoints[cnt] = Point.Empty;
		}


		/// <override></override>
		protected override bool CalculatePath()
		{
			if (base.CalculatePath()) {
				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				return true;
			}
			else return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void Construct()
		{
			VertexCount = 6;
			UpdateShapePoints();
		}


		private void UpdateShapePoints()
		{
			if (shapePoints.Length != pointCount)
				Array.Resize(ref shapePoints, pointCount);
			if (pointBuffer.Length != pointCount)
				Array.Resize(ref pointBuffer, pointCount);

			float ptAngle = 360/(float) pointCount;
			for (int i = 0; i < pointCount; ++i)
				shapePoints[i] = Point.Round(Geometry.RotatePoint(0, 0, -90 + (i*ptAngle), diameter, 0));
		}

		#region Fields

		private int pointCount = 3;
		private float diameter = 20;
		// The vertices contain the (unrotated) points of the triangle relative to 0|0.
		private Point[] shapePoints = new Point[0];
		private Point[] pointBuffer = new Point[0];

		#endregion
	}


	//public abstract class PolygonBase : CaptionedShapeBase {

	//   protected PolygonBase(ShapeType shapeType, IStyleSet styleSet)
	//      : base(shapeType, styleSet) {
	//      throw new NotImplementedException();
	//   }


	//   protected PolygonBase(ShapeType shapeType, Template template) : base(shapeType, template) {
	//      throw new NotImplementedException();
	//   }


	//   protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
	//      throw new NotImplementedException();
	//   }

	//   protected override int DivFactorX {
	//      get { throw new NotImplementedException(); }
	//   }

	//   protected override int DivFactorY {
	//      get { throw new NotImplementedException(); }
	//   }

	//   protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers) {
	//      throw new NotImplementedException();
	//   }

	//   protected override void CalcControlPoints() {
	//      throw new NotImplementedException();
	//   }

	//   protected internal override int ControlPointCount {
	//      get { throw new NotImplementedException(); }
	//   }

	//   protected override bool IntersectsWithCore(int x, int y, int width, int height) {
	//      throw new NotImplementedException();
	//   }

	//   public override Shape Clone() {
	//      throw new NotImplementedException();
	//   }

	//   public override void Fit(int x, int y, int width, int height) {
	//      throw new NotImplementedException();
	//   }

	//   public override RelativePosition CalculateRelativePosition(int x, int y) {
	//      throw new NotImplementedException();
	//   }

	//   public override Point CalculateAbsolutePosition(RelativePosition relativePosition) {
	//      throw new NotImplementedException();
	//   }
	//}


	//public override void CopyFrom(Shape source) {
	//   base.CopyFrom(source);
	//   if (source is PolygonBase) {
	//      shapePoints = new Point[((PolygonBase)source).PointCount];
	//      for (int i = 0; i < shapePoints.Length; ++i) {
	//         shapePoints[i].X = ((PolygonBase)source).shapePoints[i].X;
	//         shapePoints[i].Y = ((PolygonBase)source).shapePoints[i].Y;
	//      }
	//      center = Geometry.CalcPolygonBalancePoint(shapePoints);
	//   }
	//}


	//#region IEntity Members

	//protected override void LoadFieldsCore(IRepositoryReader reader, int version) {
	//   base.LoadFieldsCore(reader, version);
	//   int pointCount = reader.ReadInt32();
	//   if (pointCount != shapePoints.Length)
	//      Array.Resize(ref shapePoints, pointCount);
	//}


	//protected override void LoadInnerObjectsCore(string propertyName, IRepositoryReader reader, int version) {
	//   switch (propertyName) {
	//      case "Vertices":
	//         // load Vertices
	//         reader.BeginReadInnerObjects();
	//         while (reader.BeginReadInnerObject()) {
	//            int ptIdx = reader.ReadInt32();
	//            int ptId = reader.ReadInt32();
	//            int x = reader.ReadInt32();
	//            int y = reader.ReadInt32();
	//            MoveControlPointTo(ptId, x, y, ResizeModifiers.None);
	//            reader.EndReadInnerObject();
	//         }
	//         reader.EndReadInnerObjects();
	//         break;
	//      default:
	//         base.LoadInnerObjectsCore(propertyName, reader, version);
	//         break;
	//   }
	//}


	//protected override void SaveFieldsCore(IRepositoryWriter writer, int version) {
	//   base.SaveFieldsCore(writer, version);
	//}


	//protected override void SaveInnerObjectsCore(string propertyName, IRepositoryWriter writer, int version) {
	//   switch (propertyName) {
	//      case "Vertices":
	//         // save Vertices
	//         writer.BeginWriteInnerObjects();
	//         foreach (ControlPointId pointId in GetControlPointIds(ControlPointCapabilities.All)) {
	//            Point p = GetControlPointPosition(pointId);
	//            writer.BeginWriteInnerObject();
	//            writer.WriteInt32(pointId - 1);
	//            writer.WriteInt32(pointId);
	//            writer.WriteInt32(p.X);
	//            writer.WriteInt32(p.Y);
	//            writer.EndWriteInnerObject();
	//         }
	//         writer.EndWriteInnerObjects();
	//         break;
	//      default:
	//         base.SaveInnerObjectsCore(propertyName, writer, version);
	//         break;
	//   }
	//}


	//new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
	//   foreach (EntityPropertyDefinition pi in CaptionedShapeBase.GetPropertyDefinitions(version))
	//      yield return pi;
	//   yield return new EntityFieldDefinition("PointCount", typeof(int));
	//   yield return new EntityInnerObjectsDefinition(attrNameVertices, pointTypeName, pointAttrNames, pointAttrTypes);
	//}

	//#endregion


	//public virtual int PointCount {
	//   get { return shapePoints.Length; }
	//}


	//protected internal override int ControlPointCount {
	//   get { return shapePoints.Length + 1; }
	//}


	//public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
	//   if (controlPointId == 1)
	//      return ((controlPointCapability & ControlPointCapabilities.Rotate) != 0 || (controlPointCapability & ControlPointCapabilities.Reference) != 0);
	//   else if (controlPointId > 1 && controlPointId <= ControlPointCount)
	//      return ((controlPointCapability & ControlPointCapabilities.Resize) != 0) || ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId));
	//   else return base.HasControlPointCapability(controlPointId, controlPointCapability);
	//}


	//public override Point CalculateAbsolutePosition(RelativePosition relativePosition) {
	//   throw new NotImplementedException();
	//}


	//public override RelativePosition CalculateRelativePosition(int x, int y) {
	//   throw new NotImplementedException();
	//}


	//public override void Draw(Graphics graphics) {
	//   if (graphics == null) throw new ArgumentNullException("graphics");
	//   DrawPath(graphics, LineStyle, FillStyle);
	//   DrawCaption(graphics);

	//   //graphics.DrawLine(Pens.Red, X - 3, Y, X + 3, Y);
	//   //graphics.DrawLine(Pens.Red, X, Y - 3, X, Y + 3);


	//   //Point[] pts = new Point[shapePoints.Length];
	//   //int width = (int)Math.Round(0.7f * Bounds.Width);
	//   //int height = (int)Math.Round(0.7f * Bounds.Height);
	//   //int x = (int)Math.Round(0.7f * Bounds.X);
	//   //int y = (int)Math.Round(0.7f * Bounds.Y);
	//   //UpdateGraphicalObjects();
	//   //float scaleFactor = Geometry.CalcScaleFactor(Bounds.Width, Bounds.Height, width, height);
	//   //for (int i = 0; i < shapePoints.Length; ++i) {
	//   //   float deltaX = (shapePoints[i].X - Bounds.left) * scaleFactor;
	//   //   float deltaY = (shapePoints[i].Y - Bounds.top) * scaleFactor;
	//   //   pts[i].X = (int)Math.Round(x + deltaX);
	//   //   pts[i].Y = (int)Math.Round(y + deltaY);
	//   //}
	//   //Point c = Geometry.CalcPolygonBalancePoint(pts);

	//   //graphics.DrawRectangle(Pens.Red, Bounds);

	//   //graphics.DrawPolygon(Pens.Green, pts);
	//   //graphics.DrawLine(Pens.Red, c.X - 3, c.Y, c.X + 3, c.Y);
	//   //graphics.DrawLine(Pens.Red, c.X, c.Y - 3, c.X, c.Y + 3);

	//   base.Draw(graphics);
	//}


	//protected internal override void InitializeToDefault(IStyleSet styleSet) {
	//   base.InitializeToDefault(styleSet);
	//   ControlPoints = new Point[ControlPointCount];
	//   shapePoints[0].X = 0;
	//   shapePoints[0].Y = -20;
	//   shapePoints[1].X = -20;
	//   shapePoints[1].Y = 20;
	//   shapePoints[2].X = 20;
	//   shapePoints[2].Y = 20;
	//   Center = Geometry.CalcPolygonBalancePoint(shapePoints);
	//}


	//protected internal PolygonBase(ShapeType shapeType, Template template)
	//   : base(shapeType, template) {
	//}


	//protected internal PolygonBase(ShapeType shapeType, IStyleSet styleSet)
	//   : base(shapeType, styleSet) {
	//}


	//protected Point[] ShapePoints {
	//   get { return shapePoints; }
	//   set { shapePoints = value; }
	//}


	//protected override bool ContainsPointCore(int x, int y) {
	//   if (Path.PointCount == 0)
	//      return Geometry.ConvexPolygonContainsPoint(shapePoints, x, y);
	//   else
	//      return Geometry.ConvexPolygonContainsPoint(Path.PathPoints, x, y);
	//}


	//protected override bool IntersectsWithCore(int x, int y, int width, int height) {
	//   Rectangle rectangle = Rectangle.Empty;
	//   rectangle.X = x;
	//   rectangle.Y = y;
	//   rectangle.Width = width;
	//   rectangle.Height = height;
	//   if (Path.PointCount == 0) {
	//      if (Geometry.PolygonIntersectsWithRectangle(shapePoints, rectangle))
	//         return true;
	//   } else {
	//      if (Geometry.PolygonIntersectsWithRectangle(Path.PathPoints, rectangle))
	//         return true;
	//   }
	//   return false;
	//}


	//protected override bool MoveByCore(int deltaX, int deltaY) {
	//   return base.MoveByCore(deltaX, deltaY);
	//}


	//protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers) {
	//   bool result = true;
	//   switch (pointId) {
	//      case 1:
	//         result = false;
	//         break;
	//      default:
	//         shapePoints[pointId - 2].X += (int)Math.Round(transformedDeltaX);
	//         shapePoints[pointId - 2].Y += (int)Math.Round(transformedDeltaY);

	//         Center = Geometry.CalcPolygonBalancePoint(shapePoints);
	//         InvalidateDrawCache();

	//         ControlPointsHaveMoved();
	//         break;
	//   }
	//   return result;
	//}


	//protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
	//   if (index != 0) throw new IndexOutOfRangeException();
	//   Geometry.CalcBoundingRectangle(shapePoints, out captionBounds);
	//}


	//protected override void CalcControlPoints() {
	//   // rotation handle
	//   Center = Geometry.CalcPolygonBalancePoint(shapePoints);

	//   ControlPoints[0].X = Center.X;
	//   ControlPoints[0].Y = Center.Y;
	//   // resize handles
	//   for (int i = 0; i < shapePoints.Length; ++i) {
	//      ControlPoints[i + 1].X = shapePoints[i].X;
	//      ControlPoints[i + 1].Y = shapePoints[i].Y;
	//   }
	//}


	//protected override bool CalculatePath() {
	//   if (base.CalculatePath()) {
	//      Path.StartFigure();
	//      Path.AddPolygon(shapePoints);
	//      Path.CloseFigure();
	//      return true;
	//   } else return false;
	//}


	//#region Fields

	//private static string pointTypeName = "Point";
	//private static string[] pointAttrNames = new string[] { "PointIndex", "PointId", "X", "Y" };
	//private static Type[] pointAttrTypes = new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) };

	//private Point[] shapePoints = new Point[3];
	//private Point center = Point.Empty;

	//#endregion
}