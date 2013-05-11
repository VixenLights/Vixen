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


namespace Dataweb.NShape.Advanced {

	/// <ToBeCompleted></ToBeCompleted>
	public abstract class PathBasedPlanarShape : ShapeBase, IPlanarShape {

		#region Shape Members

		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			controlPoints = new Point[ControlPointCount];
			FillStyle = styleSet.FillStyles.Blue;
		}


		/// <override></override>
		public override void CopyFrom(Shape source) {
			base.CopyFrom(source);
			if (source is IPlanarShape) {
				IPlanarShape src = (IPlanarShape)source;
				// Copy regular properties
				this.angle = src.Angle;
				// Copy templated properties
				this.privateFillStyle = (Template != null && src.FillStyle == ((IPlanarShape)Template.Shape).FillStyle) ? null : src.FillStyle;
			}
		}


		/// <override></override>
		public override void MakePreview(IStyleSet styleSet) {
			base.MakePreview(styleSet);
			privateFillStyle = styleSet.GetPreviewStyle(FillStyle);
		}


		/// <override></override>
		public override bool HasStyle(IStyle style) {
			if (IsStyleAffected(FillStyle, style)) return true;
			else return base.HasStyle(style);
		}


		/// <override></override>
		public override int X {
			get { return location.X; }
			set {
				int origValue = location.X;
				if (!MoveBy(value - location.X, 0)) {
					MoveTo(origValue, Y);
					throw new InvalidOperationException(string.Format("Shape cannot move to {0}", new Point(value, location.Y)));
				}
			}
		}


		/// <override></override>
		public override int Y {
			get { return location.Y; }
			set {
				int origValue = location.Y;
				if (!MoveBy(0, value - location.Y)) {
					MoveTo(origValue, Y);
					throw new InvalidOperationException(string.Format("Shape cannot move to {0}", new Point(location.X, value)));
				}
			}
		}


		// Default implementation assuming a rectangular outline.
		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			Point result = Geometry.InvalidPoint;
			Rectangle boundingRect = GetBoundingRectangle(true);
			result = Geometry.IntersectLineWithRectangle(startX, startY, X, Y, boundingRect.X, boundingRect.Y, boundingRect.Right, boundingRect.Bottom);
			if (!Geometry.IsValid(result)) result = Center;
			return result;
		}


		/// <override></override>
		public override void DrawOutline(Graphics graphics, Pen pen) {
			base.DrawOutline(graphics, pen);
			graphics.DrawPath(pen, Path);
		}


		/// <override></override>
		public override Point GetControlPointPosition(ControlPointId controlPointId) {
			if (controlPointId == ControlPointId.Reference) {
				Point center = Point.Empty;
				center.X = X;
				center.Y = Y;
				return center;
			} else if (controlPointId == ControlPointId.None)
				throw new NShapeException("NotSupported PointId.");
			if (drawCacheIsInvalid) UpdateDrawCache(); 

			int index = GetControlPointIndex(controlPointId);
			return controlPoints[index];
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			if (controlPointId == ControlPointId.Reference)
				return (controlPointCapability & ControlPointCapabilities.Connect) != 0;
			else return base.HasControlPointCapability(controlPointId, controlPointCapability);
		}


		/// <override></override>
		public override void Invalidate() {
			base.Invalidate();
			if (DisplayService != null) DisplayService.Invalidate(GetBoundingRectangle(false));
		}

		#endregion


		#region IEntity Members

		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version) {
			base.LoadFieldsCore(reader, version);
			angle = reader.ReadInt32();
			privateFillStyle = reader.ReadFillStyle();
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version) {
			base.SaveFieldsCore(writer, version);
			writer.WriteInt32(Angle);
			writer.WriteStyle(privateFillStyle);
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.PathBasedPlanarShape" />.
		/// </summary>
		new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
			foreach (EntityPropertyDefinition pi in ShapeBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("Angle", typeof(int));
			yield return new EntityFieldDefinition("FillStyle", typeof(object));
		}

		#endregion


		#region IPlanarShape Members

		/// <ToBeCompleted></ToBeCompleted>
		[Category("Layout")]
		[Description("Rotation angle of the Shape in tenths of degree.")]
		[PropertyMappingId(PropertyIdAngle)]
		[RequiredPermission(Permission.Layout)]
		public virtual int Angle {
			get { return angle; }
			set { Rotate(value - angle, X, Y); }
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("Defines the appearence of the shape's interior.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdFillStyle)]
		[RequiredPermission(Permission.Present)]
		public virtual IFillStyle FillStyle {
			get { return privateFillStyle ?? ((IPlanarShape)Template.Shape).FillStyle; }
			set {
				privateFillStyle = (Template != null && value == ((IPlanarShape)Template.Shape).FillStyle) ? null : value;
				Invalidate();
			}
		}

		#endregion


		/// <ToBeCompleted></ToBeCompleted>
		protected internal PathBasedPlanarShape(ShapeType shapeType, Template template)
			: base(shapeType, template) {
			Construct();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal PathBasedPlanarShape(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
			Construct();
		}


		private void Construct() {
			controlPoints = new Point[ControlPointCount];
		}


		/// <override></override>
		protected override void ProcessExecModelPropertyChange(IModelMapping propertyMapping) {
			switch (propertyMapping.ShapePropertyId) {
				case PropertyIdAngle: 
					Angle = propertyMapping.GetInteger();
					break;
				case PropertyIdFillStyle:
					// assign private stylebecause if the style matches the template's style, it would not be assigned.
					privateFillStyle = (IFillStyle)propertyMapping.GetStyle();
					Invalidate();
					break;
				default: 
					base.ProcessExecModelPropertyChange(propertyMapping); 
					break;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Browsable(false)]
		protected virtual Point Center {
			get { return location; }
			set {
				Point origValue = location;
				if (MoveTo(value.X, value.Y)) {
					MoveTo(origValue.X, origValue.Y);
					throw new InvalidOperationException(string.Format("Shape cannot move to {0}", value));
				}
			}
		}


		/// <override></override>
		protected override bool RotateCore(int deltaAngle, int x, int y) {
			bool result = true;
			// first, perform rotation around the center point...
			angle = (3600 + angle + deltaAngle) % 3600;
			// ...then, rotate the shape's center around the given rotation center and 
			// move the shape (including its children) to this point
			if (x != X || y != Y) {
				int toX = X;
				int toY = Y;
				Geometry.RotatePoint(x, y, Geometry.TenthsOfDegreeToDegrees(deltaAngle), ref toX, ref toY);
				result = MoveTo(toX, toY);
			}
			return result;
		}


		/// <summary>
		/// Retuirns the BoundingRectangle of the shape. 
		/// This default implementation uses the GetBounds() method provided by the GraphicsPath class.
		/// It is recommended to override this method with more shape specific versions that do not call base.
		/// </summary>
		/// <param name="tight">
		/// If true, a thight fitting x axis aligned bounding rectangle will be returned.
		/// If false, a x axis aligned bounding rectangle also containing all control points will be returned.
		/// </param>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			if (tight) {
				Rectangle result = Rectangle.Empty;
				if (drawCacheIsInvalid) {
					CalculatePath();
					result = Rectangle.Ceiling(path.GetBounds());
					if (Angle != 0) {
						Point tl, tr, bl, br;
						Geometry.RotateRectangle(result, Point.Empty, Geometry.TenthsOfDegreeToDegrees(Angle), out tl, out tr, out bl, out br);
						Geometry.CalcBoundingRectangle(tl, tr, bl, br, out result);
					}
					result.Offset(X, Y);
					return result;
				} else return Rectangle.Ceiling(path.GetBounds());
			} else {
				Rectangle result = Rectangle.Empty;
				if (drawCacheIsInvalid) {
					CalcControlPoints();
					Point tl = Point.Empty, tr = Point.Empty, bl = Point.Empty, br = Point.Empty;
					for (int i = ControlPoints.Length - 1; i >= 0; --i) {
						if (ControlPoints[i].X <= tl.X && ControlPoints[i].Y <= tl.Y)
							tl = ControlPoints[i];
						if (ControlPoints[i].X >= tr.X && ControlPoints[i].Y <= tr.Y)
							tr = ControlPoints[i];
						if (ControlPoints[i].X <= bl.X && ControlPoints[i].Y >= bl.Y)
							bl = ControlPoints[i];
						if (ControlPoints[i].X >= br.X && ControlPoints[i].Y >= br.Y)
							br = ControlPoints[i];
					}
					if (Angle != 0) {
						tl = Geometry.RotatePoint(Point.Empty, Geometry.TenthsOfDegreeToDegrees(Angle), tl);
						tr = Geometry.RotatePoint(Point.Empty, Geometry.TenthsOfDegreeToDegrees(Angle), tr);
						bl = Geometry.RotatePoint(Point.Empty, Geometry.TenthsOfDegreeToDegrees(Angle), bl);
						br = Geometry.RotatePoint(Point.Empty, Geometry.TenthsOfDegreeToDegrees(Angle), br);
					}
					Geometry.CalcBoundingRectangle(tl, tr, bl, br, out result);
					result.Offset(X, Y);
				} else Geometry.CalcBoundingRectangle(ControlPoints, out result);
				return result;
			}
		}


		/// <override></override>
		/// <remarks>
		/// This is the default implementation.
		/// We strongly recommend to override this method without calling the base 
		/// method because the base implementation is rather slow.
		/// </remarks>
		protected override bool ContainsPointCore(int x, int y) {
			UpdateDrawCache();
			return path.IsVisible(x, y);
		}


		/// <override></override>
		protected override bool EndMove(int deltaX, int deltaY) {
			if (!drawCacheIsInvalid) TransformDrawCache(deltaX, deltaY, 0, X, Y);
			return base.EndMove(deltaX, deltaY);
		}


		/// <override></override>
		protected override bool MoveByCore(int deltaX, int deltaY) {
			location.Offset(deltaX, deltaY);
			return true;
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers) {
			float transformedDeltaX, transformedDeltaY, sin, cos;
			Geometry.TransformMouseMovement(deltaX, deltaY, Angle, out transformedDeltaX, out transformedDeltaY, out sin, out cos);
			return MovePointByCore(pointId, transformedDeltaX, transformedDeltaY, sin, cos, modifiers);
		}


		/// <summary>
		/// When resizing shapes, this factor is used to ensure that the movement is 
		/// dividable by this factor without remainder so the center of the shape does 
		/// not have to be rounded. E.g. a rectangle shape has a DivFactor of 2 (because 
		/// the center is located at 1/2 of the Height/Width).
		/// </summary>
		protected abstract int DivFactorX { get; }


		/// <summary>
		/// When resizing shapes, this factor is used to ensure that the movement is 
		/// dividable by this factor without remainder so the center of the shape does 
		/// not have to be rounded. E.g. a rectangle shape has a DivFactor of 2 (because 
		/// the center is located at 1/2 of the Height/Width).
		/// </summary>
		protected abstract int DivFactorY { get; }


		/// <ToBeCompleted></ToBeCompleted>
		protected abstract bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, 
			float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers);


		/// <ToBeCompleted></ToBeCompleted>
		protected GraphicsPath Path {
			get { return path; }
			set { path = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Point[] ControlPoints {
			get { return controlPoints; }
			//set { controlPoints = value; }
		}


		/// <override></override>
		protected override void InvalidateDrawCache() {
			base.InvalidateDrawCache();
			Path.Reset();
			boundingRectangleUnrotated = Geometry.InvalidRectangle;
		}


		/// <override></override>
		protected override void UpdateDrawCache() {
			if (drawCacheIsInvalid) {
				Debug.Assert(path != null);
				Debug.Assert(controlPoints != null);
				RecalcDrawCache();
				TransformDrawCache(X, Y, Angle, X, Y);
			}
		}


		/// <summary>
		/// Recalculate all objects that define the shape's outline and appearance, 
		/// such as ControlPoints and GraphicsPath. The objects have to be calculated 
		/// at the correct position and with the correct size but unrotated.
		/// </summary>
		protected override void RecalcDrawCache() {
			// calculate unrotated path on the current position with the current size
			CalcControlPoints();
			if (CalculatePath()) {
				// store unrotated bounding rectangle for transforming brushes to the right size.
				// rotated bounding rectangles have usually not the correct size for this purpose
				boundingRectangleUnrotated = Rectangle.Round(Path.GetBounds());
				drawCacheIsInvalid = false;
			}
		}


		/// <summary>
		/// Transforms all objects that need to be transformed, such as Point-Arrays, GraphicsPaths or Brushes
		/// </summary>
		/// <param name="deltaX">Translation on X axis</param>
		/// <param name="deltaY">Translation on Y axis</param>
		/// <param name="deltaAngle">Rotation angle in tenths of degrees</param>
		/// <param name="rotationCenterX">X coordinate of the rotation center</param>
		/// <param name="rotationCenterY">Y coordinate of the rotation center</param>
		protected override void TransformDrawCache(int deltaX, int deltaY, int deltaAngle, int rotationCenterX, int rotationCenterY) {
			// transform DrawCache only if the drawCache is valid, otherwise it will be recalculated
			// at the correct position/size
			if (!drawCacheIsInvalid) {
				Matrix.Reset();
				if (deltaX != 0 || deltaY != 0 || deltaAngle != 0) {
					Matrix.Translate(deltaX, deltaY, MatrixOrder.Prepend);
					if (deltaAngle != 0) {
						PointF rotationCenter = PointF.Empty;
						rotationCenter.X = rotationCenterX;
						rotationCenter.Y = rotationCenterY;
						Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(deltaAngle), rotationCenter, MatrixOrder.Append);
					}
					// transform controlPoints
					if (controlPoints != null) Matrix.TransformPoints(controlPoints);
					// transform GraphicsPath
					if (path != null) path.Transform(Matrix);
				}
				// transform unrotated boundingRectangle
				boundingRectangleUnrotated.Offset(deltaX, deltaY);
			}
		}


		/// <summary>
		/// Calculates the coordinates of the control points.
		/// </summary>
		/// <remarks>
		/// The control point positions have to be calculated assuming X|Y at the coordinate origin 0|0 because it will be translated to the current position X|Y after calculating.
		/// (unrotated) shape's control points.		/// 
		/// </remarks>
		protected abstract void CalcControlPoints();


		/// <summary>
		/// Calculate the shape's (unrotated) GraphicsPath. 
		/// The GraphicsPath has to be calculated assuming X|Y at the coordinate origin 0|0 because it will be translated to the current position X|Y after calculating.
		/// </summary>
		/// <returns>Returns true if the path was successfully calculated. Otherwise, the return value will be false.</returns>
		protected abstract bool CalculatePath();


		/// <summary>
		/// Draws the calculated GraphicsPath. If the GaphicsPath is not calculated yet, UpdateDrawCache will be called.
		/// </summary>
		protected void DrawPath(Graphics graphics, ILineStyle lineStyle, IFillStyle fillStyle) {
			UpdateDrawCache();
			if (fillStyle != null) {
				Brush brush = ToolCache.GetTransformedBrush(FillStyle, boundingRectangleUnrotated, Center, Angle);
				graphics.FillPath(brush, Path);
			}
			if (lineStyle != null) {
				Pen pen = ToolCache.GetPen(lineStyle, null, null);
				graphics.DrawPath(pen, Path);
			}

		}


		#region Fields
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdAngle = 2;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdFillStyle = 3;

		/// <ToBeCompleted></ToBeCompleted>
		protected Point[] controlPoints;
		/// <summary>Tight fitting BoundingRectangle of the unrotated shape, used for transforming brushes</summary>
		protected Rectangle boundingRectangleUnrotated = Geometry.InvalidRectangle;

		// Location of the Shape (usually the BalancePoint of the shape)
		private Point location = Point.Empty;
		private int angle = 0;
		// Transformation Matrix for transformation of Vertices, GraphicsPath and Brushes		
		private Matrix matrix = new Matrix();				
		// GraphicsPath that will define the appearance of the shape
		private GraphicsPath path = new GraphicsPath();	
		private IFillStyle privateFillStyle = null;
		#endregion
	}

}
