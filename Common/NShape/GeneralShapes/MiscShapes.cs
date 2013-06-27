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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.GeneralShapes
{
	public class ThickArrow : RectangleBase
	{
		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);
			bodyHeightRatio = 1d/3d;
			headWidth = (int) Math.Round(Width/2f);
		}


		/// <override></override>
		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is ThickArrow) {
				this.headWidth = ((ThickArrow) source).headWidth;
				this.bodyHeightRatio = ((ThickArrow) source).bodyHeightRatio;
			}
			InvalidateDrawCache();
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new ThickArrow(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}


		public int BodyWidth
		{
			get { return Width - headWidth; }
		}


		[Category("Layout")]
		[Description("The height of the arrow's body.")]
		[PropertyMappingId(PropertyIdBodyHeight)]
		[RequiredPermission(Permission.Layout)]
		public int BodyHeight
		{
			get { return (int) Math.Round(Height*bodyHeightRatio); }
			set
			{
				Invalidate();
				if (value > Height) throw new ArgumentOutOfRangeException("BodyHeight");

				if (Height == 0) bodyHeightRatio = 0;
				else bodyHeightRatio = value/(float) Height;

				InvalidateDrawCache();
				Invalidate();
			}
		}


		[Category("Layout")]
		[Description("The width of the arrow's tip.")]
		[PropertyMappingId(PropertyIdHeadWidth)]
		[RequiredPermission(Permission.Layout)]
		public int HeadWidth
		{
			get { return headWidth; }
			set
			{
				Invalidate();
				headWidth = value;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		/// <override></override>
		protected override int ControlPointCount
		{
			get { return 7; }
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId,
		                                               ControlPointCapabilities controlPointCapability)
		{
			switch (controlPointId) {
				case ArrowTipControlPoint:
				case BodyEndControlPoint:
					// ToDo: Implement GluePoint behavior for ThickArrows
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0
					        //|| (controlPointCapability & ControlPointCapabilities.Glue) != 0
					        ||
					        ((controlPointCapability & ControlPointCapabilities.Connect) != 0 &&
					         IsConnectionPointEnabled(controlPointId)));
				case ArrowTopControlPoint:
				case BodyTopControlPoint:
				case BodyBottomControlPoint:
				case ArrowBottomControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0
					        || ((controlPointCapability & ControlPointCapabilities.Connect) != 0
					            && IsConnectionPointEnabled(controlPointId)));

				case RotateControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Reference) != 0
					        || (controlPointCapability & ControlPointCapabilities.Rotate) != 0
					        || ((controlPointCapability & ControlPointCapabilities.Connect) != 0
					            && IsConnectionPointEnabled(controlPointId)));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		public override void Fit(int x, int y, int width, int height)
		{
			float headWidthRatio = this.HeadWidth/(float) Width;
			HeadWidth = (int) Math.Round(width*headWidthRatio);
			base.Fit(x, y, width, height);
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY)
		{
			CalcShapePoints();
			PointF rotationCenter = PointF.Empty;
			rotationCenter.X = X;
			rotationCenter.Y = Y;
			Matrix.Reset();
			Matrix.Translate(X, Y, MatrixOrder.Prepend);
			Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), rotationCenter, MatrixOrder.Append);
			Matrix.TransformPoints(shapePoints);
			Matrix.Reset();

			Point startPoint = Point.Empty;
			startPoint.X = startX;
			startPoint.Y = startY;
			Point result = Geometry.GetNearestPoint(startPoint,
			                                        Geometry.IntersectPolygonLine(shapePoints, startX, startY, X, Y, true));
			if (!Geometry.IsValid(result)) result = Center;
			return result;
		}

		#region IEntity Members

		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version)
		{
			base.LoadFieldsCore(reader, version);
			HeadWidth = reader.ReadInt32();
			BodyHeight = reader.ReadInt32();
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version)
		{
			base.SaveFieldsCore(writer, version);
			writer.WriteInt32(HeadWidth);
			writer.WriteInt32(BodyHeight);
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.ThickArrow" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in RectangleBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("HeadWidth", typeof (int));
			yield return new EntityFieldDefinition("BodyHeight", typeof (int));
		}

		#endregion

		protected internal ThickArrow(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		protected internal ThickArrow(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds)
		{
			if (index != 0) throw new IndexOutOfRangeException();
			captionBounds = Rectangle.Empty;
			captionBounds.Width = (int) Math.Round(Width - (HeadWidth/3f));
			captionBounds.Height = BodyHeight;
			captionBounds.X = -(int) Math.Round((Width/2f) - (HeadWidth/3f));
			captionBounds.Y = -(int) Math.Round(captionBounds.Height/2f);
			if (ParagraphStyle != null) {
				captionBounds.X += ParagraphStyle.Padding.Left;
				captionBounds.Y += ParagraphStyle.Padding.Top;
				captionBounds.Width -= ParagraphStyle.Padding.Horizontal;
				captionBounds.Height -= ParagraphStyle.Padding.Vertical;
			}
		}


		/// <override></override>
		protected override bool CalculatePath()
		{
			if (base.CalculatePath()) {
				Path.Reset();
				CalcShapePoints();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				return true;
			}
			else return false;
		}


		protected override bool ContainsPointCore(int x, int y)
		{
			if (base.ContainsPointCore(x, y)) {
				CalcShapePoints();
				// Transform points
				Matrix.Reset();
				Matrix.Translate(X, Y, MatrixOrder.Prepend);
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center, MatrixOrder.Append);
				Matrix.TransformPoints(shapePoints);

				return Geometry.PolygonContainsPoint(shapePoints, x, y);
			}
			return false;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight)
		{
			Rectangle result = Geometry.InvalidRectangle;
			if (Width >= 0 && Height >= 0) {
				CalcShapePoints();
				Geometry.CalcBoundingRectangle(shapePoints, 0, 0, Geometry.TenthsOfDegreeToDegrees(Angle), out result);
				if (Geometry.IsValid(result)) {
					result.Offset(X, Y);
					ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				}
			}
			return result;
		}


		/// <override></override>
		protected override int GetControlPointIndex(ControlPointId id)
		{
			switch (id) {
				case ArrowTipControlPoint:
					return 0;
				case ArrowTopControlPoint:
					return 1;
				case BodyTopControlPoint:
					return 2;
				case BodyEndControlPoint:
					return 3;
				case BodyBottomControlPoint:
					return 4;
				case ArrowBottomControlPoint:
					return 5;
				case RotateControlPoint:
					return 6;
				default:
					return base.GetControlPointIndex(id);
			}
		}


		/// <override></override>
		protected override void CalcControlPoints()
		{
			int left = -(int) Math.Round(Width/2f);
			int right = left + Width;
			int top = -(int) Math.Round(Height/2f);
			int bottom = top + Height;
			int halfBodyWidth = (int) Math.Round(BodyWidth/2f);
			int halfBodyHeight = (int) Math.Round(BodyHeight/2f);

			int i = 0;
			controlPoints[i].X = left;
			controlPoints[i].Y = 0;
			++i;
			controlPoints[i].X = right - BodyWidth;
			controlPoints[i].Y = top;
			++i;
			controlPoints[i].X = right - halfBodyWidth;
			controlPoints[i].Y = -halfBodyHeight;
			++i;
			controlPoints[i].X = right;
			controlPoints[i].Y = 0;
			++i;
			controlPoints[i].X = right - halfBodyWidth;
			controlPoints[i].Y = halfBodyHeight;
			++i;
			controlPoints[i].X = right - BodyWidth;
			controlPoints[i].Y = bottom;
			++i;
			controlPoints[i].X = 0;
			controlPoints[i].Y = 0;
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers)
		{
			if (pointId == ArrowTipControlPoint || pointId == BodyEndControlPoint) {
				bool result = true;
				int dx = 0, dy = 0;
				int width = Width;
				int angle = Angle;
				Point tipPt = GetControlPointPosition(ArrowTipControlPoint);
				Point endPt = GetControlPointPosition(BodyEndControlPoint);

				if (pointId == ArrowTipControlPoint)
					result = Geometry.MoveArrowPoint(Center, tipPt, endPt, angle, headWidth, 0.5f, deltaX, deltaY, modifiers, out dx,
					                                 out dy, out width, out angle);
				else
					result = Geometry.MoveArrowPoint(Center, endPt, tipPt, angle, headWidth, 0.5f, deltaX, deltaY, modifiers, out dx,
					                                 out dy, out width, out angle);

				RotateCore(angle - Angle, X, Y);
				MoveByCore(dx, dy);
				Width = width;
				return result;
			}
			else return base.MovePointByCore(pointId, deltaX, deltaY, modifiers);
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY,
		                                        float sin, float cos, ResizeModifiers modifiers)
		{
			bool result = true;
			int dx = 0, dy = 0;
			int width = Width;
			int height = Height;
			switch (pointId) {
				case ArrowTopControlPoint:
				case ArrowBottomControlPoint:
					if (pointId == ArrowTopControlPoint) {
						//result = (transformedDeltaX == 0);
						if (
							!Geometry.MoveRectangleTop(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx,
							                           out dy, out width, out height))
							result = false;
					}
					else {
						//result = (transformedDeltaX == 0);
						if (
							!Geometry.MoveRectangleBottom(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx,
							                              out dy, out width, out height))
							result = false;
					}
					int newHeadWidth = HeadWidth + (int) Math.Round(transformedDeltaX);
					if (newHeadWidth < 0) {
						newHeadWidth = 0;
						result = false;
					}
					else if (newHeadWidth > Width) {
						newHeadWidth = Width;
						result = false;
					}
					HeadWidth = newHeadWidth;
					break;
				case BodyTopControlPoint:
				case BodyBottomControlPoint:
					result = (transformedDeltaX == 0);
					int newBodyHeight = 0;
					if (pointId == BodyTopControlPoint)
						newBodyHeight = (int) Math.Round(BodyHeight - (transformedDeltaY*2));
					else
						newBodyHeight = (int) Math.Round(BodyHeight + (transformedDeltaY*2));
					if (newBodyHeight > Height) {
						newBodyHeight = Height;
						result = false;
					}
					else if (newBodyHeight < 0) {
						newBodyHeight = 0;
						result = false;
					}
					BodyHeight = newBodyHeight;
					break;
				default:
					return base.MovePointByCore(pointId, transformedDeltaX, transformedDeltaY, sin, cos, modifiers);
			}
			if (width < headWidth) {
				width = headWidth;
				result = false;
			}
			MoveByCore(dx, dy);
			Width = width;
			Height = height;
			return result;
		}


		/// <override></override>
		protected override void ProcessExecModelPropertyChange(IModelMapping propertyMapping)
		{
			switch (propertyMapping.ShapePropertyId) {
				case PropertyIdBodyHeight:
					BodyHeight = propertyMapping.GetInteger();
					break;
				case PropertyIdHeadWidth:
					HeadWidth = propertyMapping.GetInteger();
					break;
				default:
					base.ProcessExecModelPropertyChange(propertyMapping);
					break;
			}
		}


		private void CalcShapePoints()
		{
			int left = -(int) Math.Round(Width/2f);
			int right = left + Width;
			int top = -(int) Math.Round(Height/2f);
			int bottom = top + Height;
			int halfBodyHeight = (int) Math.Round(BodyHeight/2f);

			// head tip
			shapePoints[0].X = left;
			shapePoints[0].Y = 0;

			// head side tip (top)
			shapePoints[1].X = right - BodyWidth;
			shapePoints[1].Y = top;

			// head / body connection point
			shapePoints[2].X = right - BodyWidth;
			shapePoints[2].Y = -halfBodyHeight;

			// body corner (top)
			shapePoints[3].X = right;
			shapePoints[3].Y = -halfBodyHeight;

			// body corner (bottom)
			shapePoints[4].X = right;
			shapePoints[4].Y = halfBodyHeight;

			// head / body connection point
			shapePoints[5].X = right - BodyWidth;
			shapePoints[5].Y = halfBodyHeight;

			// head side tip (bottom)
			shapePoints[6].X = right - BodyWidth;
			shapePoints[6].Y = bottom;
		}

		#region Fields

		protected const int PropertyIdBodyHeight = 9;
		protected const int PropertyIdHeadWidth = 10;

		private Point newTipPos = Point.Empty;
		private Point oldTipPos = Point.Empty;

		private const int ArrowTipControlPoint = 1;
		private const int ArrowTopControlPoint = 2;
		private const int ArrowBottomControlPoint = 3;
		private const int BodyTopControlPoint = 4;
		private const int BodyBottomControlPoint = 5;
		private const int BodyEndControlPoint = 6;
		private const int RotateControlPoint = 7;

		private Point[] shapePoints = new Point[7];
		private int headWidth;
		private double bodyHeightRatio;

		#endregion
	}


	public class Picture : PictureBase
	{
		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Picture(shapeType, template);
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Picture(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}


		protected internal Picture(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			Construct();
		}


		protected internal Picture(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			Construct();
		}


		private void Construct()
		{
			//Image = new NamedImage();
		}
	}


	public class RegularPolygone : RegularPolygoneBase
	{
		/// <ToBeCompleted></ToBeCompleted>
		public static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new RegularPolygone(shapeType, template);
		}


		public override Shape Clone()
		{
			Shape result = new RegularPolygone(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}


		protected RegularPolygone(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		protected RegularPolygone(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}
	}
}