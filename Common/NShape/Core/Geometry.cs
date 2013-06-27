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
	public static class Geometry
	{
		#region Calculations with Rectangles

		/// <summary>
		/// Merges two rectangles to a new rectangle that contains all the points of the two.
		/// </summary>
		public static Rectangle UniteRectangles(int left, int top, int right, int bottom, Rectangle rectangle)
		{
			AssertIsValidLTRB(left, top, right, bottom);
			AssertIsValid(rectangle);
			//
			Rectangle result = Rectangle.Empty;
			if (rectangle.IsEmpty) {
				result.X = left;
				result.Y = top;
				result.Width = right - left;
				result.Height = bottom - top;
			}
			else {
				result.X = Math.Min(left, rectangle.X);
				result.Y = Math.Min(top, rectangle.Y);
				result.Width = Math.Max(right, rectangle.Right) - result.X;
				result.Height = Math.Max(bottom, rectangle.Bottom) - result.Y;
			}
			return result;
		}


		///// <summary>
		///// Merges two rectangles to a new rectangle that contains all the points of the two.
		///// </summary>
		//public static Rectangle UniteRectangles(Rectangle a, Rectangle b) {
		//    const string excMsgFormat = "{0} is not a valid value for parameter '{1}'";
		//    if (!IsValid(a)) throw new ArgumentException(string.Format(excMsgFormat, a, "a"));
		//    if (!IsValid(b)) throw new ArgumentException(string.Format(excMsgFormat, b, "b"));
		//    //
		//    if (a.IsEmpty) return b;
		//    if (b.IsEmpty) return a;
		//    Rectangle result = Rectangle.Empty;
		//    result.X = Math.Min(a.X, b.X);
		//    result.Y = Math.Min(a.Y, b.Y);
		//    result.Width = Math.Max(a.Right, b.Right) - result.X;
		//    result.Height = Math.Max(a.Bottom, b.Bottom) - result.Y;
		//    return result;
		//}


		///// <summary>
		///// Merges two rectangles to a new rectangle that contains all the points of the two.
		///// </summary>
		//public static Rectangle UniteRectangles(RectangleF a, Rectangle b) {
		//    const string excMsgFormat = "{0} is not a valid value for parameter '{1}'";
		//    if (!IsValid(a)) throw new ArgumentException(string.Format(excMsgFormat, a, "a"));
		//    if (!IsValid(b)) throw new ArgumentException(string.Format(excMsgFormat, b, "b"));
		//    //
		//    if (a.IsEmpty) return b;
		//    if (b.IsEmpty) return Rectangle.Round(a);
		//    Rectangle result = Rectangle.Empty;
		//    result.X = Math.Min((int)Math.Floor(a.X), b.X);
		//    result.Y = Math.Min((int)Math.Floor(a.Y), b.Y);
		//    result.Width = Math.Max((int)Math.Ceiling(a.Right), b.Right) - result.X;
		//    result.Height = Math.Max((int)Math.Ceiling(a.Bottom), b.Bottom) - result.Y;
		//    return result;
		//}


		/// <summary>
		/// Merges two rectangles to a new rectangle that contains all the points of the two.
		/// </summary>
		public static Rectangle UniteRectangles(Rectangle a, Rectangle b)
		{
			if (!IsValid(a)) return b;
			if (!IsValid(b)) return a;
			Rectangle result = Rectangle.Empty;
			result.X = Math.Min(a.X, b.X);
			result.Y = Math.Min(a.Y, b.Y);
			result.Width = Math.Max(a.Right, b.Right) - result.X;
			result.Height = Math.Max(a.Bottom, b.Bottom) - result.Y;
			return result;
		}


		/// <summary>
		/// Merges two rectangles to a new rectangle that contains all the points of the two.
		/// </summary>
		public static Rectangle UniteRectangles(RectangleF a, Rectangle b)
		{
			if (!IsValid(a)) return b;
			if (!IsValid(b)) return Rectangle.Round(a);
			Rectangle result = Rectangle.Empty;
			result.X = Math.Min((int) Math.Floor(a.X), b.X);
			result.Y = Math.Min((int) Math.Floor(a.Y), b.Y);
			result.Width = Math.Max((int) Math.Ceiling(a.Right), b.Right) - result.X;
			result.Height = Math.Max((int) Math.Ceiling(a.Bottom), b.Bottom) - result.Y;
			return result;
		}


		/// <summary>
		/// Merges a point and a rectangle to a new rectangle that contains all the points of the two.
		/// </summary>
		public static Rectangle UniteWithRectangle(Point p, Rectangle rect)
		{
			AssertIsValid(p);
			AssertIsValid(rect);
			if (p.X < rect.X) {
				rect.Width += rect.X - p.X;
				rect.X = p.X;
			}
			else if (p.X > rect.Right)
				rect.Width = p.X - rect.X;
			if (p.Y < rect.Y) {
				rect.Height += rect.Y - p.Y;
				rect.Y = p.Y;
			}
			else if (p.Y > rect.Bottom)
				rect.Height = p.Y - rect.Y;
			return rect;
		}


		/// <summary>
		/// Enlarges the rectangle to include a given point.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="p"></param>
		public static void IncludeRectanglePoint(ref Rectangle r, Point p)
		{
			if (p.X < r.Left) {
				r.Width = r.Right - p.X;
				r.X = p.X;
			}
			if (p.X > r.Right) r.Width = p.X - r.Left;
			if (p.Y < r.Top) {
				r.Height = r.Bottom - p.Y;
				r.Y = p.Y;
			}
			if (p.Y > r.Bottom) r.Height = p.Y - r.Top;
		}

		#endregion

		#region Helpers for moving and resizing shapes

		/// <summary>
		/// Transforms the mouse movement into the coordinate system of the (rotated) shape.
		/// </summary>
		/// <param name="deltaX">Mouse movement on X axis</param>
		/// <param name="deltaY">Mouse movement on Y axis</param>
		/// <param name="angleTenthsOfDeg">Angle of the rotated shape in tenths of degrees</param>
		/// <param name="modifiers">Movement/Resizing modifiers</param>
		/// <param name="divFactorX">Ensures that transformedDeltaX is dividable by this factor without remainder</param>
		/// <param name="divFactorY">Ensures that transformedDeltaY is dividable by this factor without remainder</param>
		/// <param name="transformedDeltaX">Transformed mouse movement on X axis</param>
		/// <param name="transformedDeltaY">Transformed mouse movement on Y axis</param>
		/// <param name="sin">Sinus value of the given angle</param>
		/// <param name="cos">Cosinus value of the given angle</param>
		[Obsolete("Use the other version of TransformmouseMovement in conjunction with AlignMovement instead.")]
		public static bool TransformMouseMovement(int deltaX, int deltaY, int angleTenthsOfDeg, ResizeModifiers modifiers,
		                                          int divFactorX, int divFactorY,
		                                          out float transformedDeltaX, out float transformedDeltaY, out float sin,
		                                          out float cos)
		{
			TransformMouseMovement(deltaX, deltaY, angleTenthsOfDeg, out transformedDeltaX, out transformedDeltaY, out sin,
			                       out cos);
			return AlignMovement(modifiers, divFactorX, divFactorY, ref transformedDeltaX, ref transformedDeltaY);
		}


		/// <summary>
		/// Transforms the mouse movement into the coordinate system of the (rotated) shape.
		/// </summary>
		/// <param name="deltaX">Mouse movement on X axis</param>
		/// <param name="deltaY">Mouse movement on Y axis</param>
		/// <param name="angleTenthsOfDeg">Angle of the rotated shape in tenths of degrees</param>
		/// <param name="transformedDeltaX">Transformed mouse movement on X axis</param>
		/// <param name="transformedDeltaY">Transformed mouse movement on Y axis</param>
		/// <param name="sin">Sinus value of the given angle</param>
		/// <param name="cos">Cosinus value of the given angle</param>
		public static void TransformMouseMovement(int deltaX, int deltaY, int angleTenthsOfDeg, out float transformedDeltaX,
		                                          out float transformedDeltaY, out float sin, out float cos)
		{
			// Rotate the mouse movement
			float ang = TenthsOfDegreeToRadians(angleTenthsOfDeg);
			cos = (float) Math.Cos(ang);
			sin = (float) Math.Sin(ang);
			transformedDeltaX = (float) ((deltaX*cos) + (deltaY*sin));
			transformedDeltaY = (float) ((deltaY*cos) - (deltaX*sin));
		}


		/// <summary>
		/// Aligns the (transformed) movement to pixels in order to ensure that the shape or its control points are moved to full integer coordinates.
		/// Returns false if the movement was modified.
		/// </summary>
		/// <param name="modifiers">Movement/Resizing modifiers</param>
		/// <param name="divFactorX">Ensures that transformedDeltaX is dividable by this factor without remainder</param>
		/// <param name="divFactorY">Ensures that transformedDeltaY is dividable by this factor without remainder</param>
		/// <param name="deltaX">Mouse movement on X axis</param>
		/// <param name="deltaY">Mouse movement on Y axis</param>
		public static bool AlignMovement(ResizeModifiers modifiers, int divFactorX, int divFactorY, ref float deltaX,
		                                 ref float deltaY)
		{
			bool result = true;
			// Ensure that the mouse movement will be devidable without remainder 
			// when *not* resizing in both directions
			if ((modifiers & ResizeModifiers.MirroredResize) == 0) {
				if (deltaX%divFactorX != 0) {
					deltaX -= deltaX%divFactorX;
					result = false;
				}
				if (deltaY%divFactorY != 0) {
					deltaY -= deltaY%divFactorY;
					result = false;
				}
			}
			return result;
		}


		/// <summary>
		/// Corrects deltaX and deltaY in order to preserve the aspect ratio of the sources bounds when inflating by deltaX and deltaY.
		/// </summary>
		/// <param name="width">The current width</param>
		/// <param name="height">The current height</param>
		/// <param name="growDirectionX">Specifies if deltaX is added (1) or subtracted (-1) in order to grow the shape.</param>
		/// <param name="growDirectionY">Specifies if deltaY is added (1) or subtracted (-1) in order to grow the shape.</param>
		/// <param name="deltaX">Specifies the growth of the shape in X direction.</param>
		/// <param name="deltaY">Specifies the growth of the shape in Y direction.</param>
		public static bool MaintainAspectRatio(float width, float height, sbyte growDirectionX, sbyte growDirectionY,
		                                       ref float deltaX, ref float deltaY)
		{
			if (width == 0) throw new ArgumentException("width");
			if (height == 0) throw new ArgumentException("height");
			if (!(growDirectionX == 1 || growDirectionX == -1)) throw new ArgumentException("growDirectionX has to be 1 or -1.");
			if (!(growDirectionY == 1 || growDirectionY == -1)) throw new ArgumentException("growDirectionX has to be 1 or -1.");
			bool result = true;
			// Calculate aspect ratio and resulting rectangles
			float aspectRatio = width/(float) height;
			int dstWidthX = (int) Math.Round(width + (deltaX*growDirectionX), MidpointRounding.ToEven);
			int dstHeightX = (int) Math.Round(dstWidthX/aspectRatio, MidpointRounding.ToEven);
			float dstHeightY = (int) Math.Round(height + (deltaY*growDirectionY), MidpointRounding.ToEven);
			float dstWidthY = (int) Math.Round(dstHeightY*aspectRatio, MidpointRounding.ToEven);
			// 
			if (dstWidthX <= dstWidthY && dstHeightX <= dstHeightY) {
				float newDeltaY = ((dstWidthX/aspectRatio) - height)*growDirectionY;
				result = (deltaY == newDeltaY);
				deltaY = newDeltaY;
			}
			else if (dstWidthY <= dstWidthX && dstHeightY <= dstHeightX) {
				float newDeltaX = ((dstHeightY*aspectRatio) - width)*growDirectionX;
				result = (deltaX == newDeltaX);
				deltaX = newDeltaX;
			}
			else Debug.Fail("Undefined case!");

			//float dstWidth = width + (deltaX * growDirectionX);
			//float dstHeight = height + (deltaY * growDirectionY);
			//float newDeltaX = deltaX;
			//float newDeltaY = deltaY;
			//if (Math.Abs(deltaX) <= Math.Abs(deltaY)) {
			//    newDeltaY = ((dstWidth / aspectRatio) - height) * growDirectionY;
			//} else {
			//    newDeltaX = ((dstHeight * aspectRatio) - width) * growDirectionX;
			//}
			//deltaX = newDeltaX;
			//deltaY = newDeltaY;

			return result;
		}


		/// <summary>
		/// Moves the TopTeft corner of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because 
		/// of movement restrictions</returns>
		public static bool MoveRectangleTopLeft(int width, int height, float deltaX, float deltaY,
		                                        float cosAngle, float sinAngle, ResizeModifiers modifiers,
		                                        out int centerOffsetX,
		                                        out int centerOffsetY, out int newWidth, out int newHeight)
		{
			int minWidth = 0, minHeight = 0;
			GetMinValues(width, height, modifiers, out minWidth, out minHeight);
			return MoveRectangleTopLeft(width, height, minWidth, minHeight, 0.5f, 0.5f, 2, 2, deltaX,
			                            deltaY, cosAngle, sinAngle, modifiers, out centerOffsetX, out centerOffsetY, out newWidth,
			                            out newHeight);
		}


		/// <summary>
		/// Moves the TopTeft corner of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="minValueX">Specifies the minimum width.</param>
		/// <param name="minValueY">Specifies the minimum height.</param>
		/// <param name="centerPosFactorX">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="centerPosFactorY">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="divFactorX">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="divFactorY">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleTopLeft(int width, int height, int minValueX, int minValueY,
		                                        float centerPosFactorX, float centerPosFactorY, int divFactorX, int divFactorY,
		                                        float deltaX, float deltaY, float cosAngle, float sinAngle,
		                                        ResizeModifiers modifiers,
		                                        out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                        out int newHeight)
		{
			bool result = true;
			centerOffsetX = centerOffsetY = 0;
			newWidth = width;
			newHeight = height;
			// Maintain aspect (if needed)
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0)
				result = MaintainAspectRatio(width, height, -1, -1, ref deltaX, ref deltaY);
			// Align to integer coordinates
			Geometry.AlignMovement(modifiers, divFactorX, divFactorY, ref deltaX, ref deltaY);

			if ((modifiers & ResizeModifiers.MirroredResize) != 0) {
				if (newWidth - deltaX - deltaX >= minValueX)
					newWidth -= (int) Math.Round(deltaX + deltaX);
				else {
					newWidth = minValueX;
					result = false;
				}
				if (newHeight - deltaY - deltaY >= minValueY)
					newHeight -= (int) Math.Round(deltaY + deltaY);
				else {
					newHeight = minValueY;
					result = false;
				}
			}
			else {
				if (newWidth - deltaX >= minValueX) {
					newWidth -= (int) Math.Round(deltaX);
				}
				else {
					deltaX = newWidth;
					newWidth = minValueX;
					result = false;
				}
				if (newHeight - deltaY >= minValueY) {
					newHeight -= (int) Math.Round(deltaY);
				}
				else {
					deltaY = newHeight;
					newHeight = minValueY;
					result = false;
				}
				centerOffsetX = (int) Math.Round((deltaX*centerPosFactorX*cosAngle) - (deltaY*(1 - centerPosFactorY)*sinAngle));
				centerOffsetY = (int) Math.Round((deltaX*centerPosFactorX*sinAngle) + (deltaY*(1 - centerPosFactorY)*cosAngle));
			}
			return result;
		}


		/// <summary>
		/// Moves the top side of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleTop(int width, int height, float deltaX, float deltaY, float cosAngle, float sinAngle,
		                                    ResizeModifiers modifiers,
		                                    out int centerOffsetX, out int centerOffsetY, out int newWidth, out int newHeight)
		{
			int minWidth = 0, minHeight = 0;
			GetMinValues(width, height, modifiers, out minWidth, out minHeight);
			return MoveRectangleTop(width, height, minHeight, 0.5f, 0.5f, 2, 2, deltaX, deltaY, cosAngle, sinAngle, modifiers,
			                        out centerOffsetX, out centerOffsetY, out newWidth, out newHeight);
		}


		/// <summary>
		/// Moves the top side of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="minValueY">Specifies the minimum height.</param>
		/// <param name="centerPosFactorX">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="centerPosFactorY">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="divFactorX">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="divFactorY">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleTop(int width, int height, int minValueY,
		                                    float centerPosFactorX, float centerPosFactorY, int divFactorX, int divFactorY,
		                                    float deltaX, float deltaY, double cosAngle, double sinAngle,
		                                    ResizeModifiers modifiers,
		                                    out int centerOffsetX, out int centerOffsetY, out int newWidth, out int newHeight)
		{
			bool result = true;
			centerOffsetX = centerOffsetY = 0;
			newWidth = width;
			newHeight = height;

			if ((modifiers & ResizeModifiers.MaintainAspect) != 0) {
				//MaintainAspectRatio(width, height, 1, -1, ref deltaX, ref deltaY);
				float aspectRatio = width/(float) height;
				deltaX = deltaY*aspectRatio;
			}
			// Align to integer coordinates
			Geometry.AlignMovement(modifiers, divFactorX, divFactorY, ref deltaX, ref deltaY);

			if ((modifiers & ResizeModifiers.MirroredResize) != 0) {
				if (newHeight - deltaY - deltaY >= minValueY)
					newHeight -= (int) Math.Round(deltaY + deltaY);
				else {
					newHeight = minValueY;
					result = false;
				}
			}
			else {
				if (newHeight - deltaY >= minValueY)
					newHeight -= (int) Math.Round(deltaY);
				else {
					deltaY = newHeight;
					newHeight = minValueY;
					result = false;
				}
				centerOffsetX = (int) Math.Round(-(deltaY*(1 - centerPosFactorY)*sinAngle));
				centerOffsetY = (int) Math.Round((deltaY*(1 - centerPosFactorY)*cosAngle));
			}
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0)
				newWidth = (int) Math.Round(newHeight*(width/(float) height));
			return result;
		}


		/// <summary>
		/// Moves the TopRight corner of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleTopRight(int width, int height, float deltaX, float deltaY, float cosAngle,
		                                         float sinAngle, ResizeModifiers modifiers,
		                                         out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                         out int newHeight)
		{
			int minWidth = 0, minHeight = 0;
			GetMinValues(width, height, modifiers, out minWidth, out minHeight);
			return MoveRectangleTopRight(width, height, minWidth, minHeight, 0.5f, 0.5f, 2, 2, deltaX, deltaY, cosAngle, sinAngle,
			                             modifiers, out centerOffsetX, out centerOffsetY, out newWidth, out newHeight);
		}


		/// <summary>
		/// Moves the TopRight corner of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="minValueX">Specifies the minimum width.</param>
		/// <param name="minValueY">Specifies the minimum height.</param>
		/// <param name="centerPosFactorX">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="centerPosFactorY">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="divFactorX">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="divFactorY">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleTopRight(int width, int height, int minValueX, int minValueY,
		                                         float centerPosFactorX, float centerPosFactorY, int divFactorX,
		                                         int divFactorY,
		                                         float deltaX, float deltaY, float cosAngle, float sinAngle,
		                                         ResizeModifiers modifiers,
		                                         out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                         out int newHeight)
		{
			bool result = true;
			centerOffsetX = centerOffsetY = 0;
			newWidth = width;
			newHeight = height;
			// Aspect maintainance can be combined with both MirroredResizing and normal resizing
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0)
				MaintainAspectRatio(width, height, 1, -1, ref deltaX, ref deltaY);
			// Align to integer coordinates
			Geometry.AlignMovement(modifiers, divFactorX, divFactorY, ref deltaX, ref deltaY);

			if ((modifiers & ResizeModifiers.MirroredResize) != 0) {
				if (newWidth + deltaX + deltaX >= minValueX)
					newWidth += (int) Math.Round(deltaX + deltaX);
				else {
					newWidth = minValueX;
					result = false;
				}
				if (newHeight - deltaY - deltaY >= minValueY)
					newHeight -= (int) Math.Round(deltaY + deltaY);
				else {
					newHeight = minValueY;
					result = false;
				}
			}
			else {
				if (newWidth + deltaX >= minValueX)
					newWidth += (int) Math.Round(deltaX);
				else {
					deltaX = -newWidth;
					newWidth = minValueX;
					result = false;
				}
				if (newHeight - deltaY >= minValueY)
					newHeight -= (int) Math.Round(deltaY);
				else {
					deltaY = newHeight;
					newHeight = minValueY;
					result = false;
				}
				centerOffsetX = (int) Math.Round((deltaX*centerPosFactorX*cosAngle) - (deltaY*(1 - centerPosFactorY)*sinAngle));
				centerOffsetY = (int) Math.Round((deltaX*centerPosFactorX*sinAngle) + (deltaY*(1 - centerPosFactorY)*cosAngle));
			}
			return result;
		}


		/// <summary>
		/// Moves the left side of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. Returns 
		/// false if the movement could not be performed completely because of movement 
		/// restrictions</returns>
		public static bool MoveRectangleLeft(int width, int height, float deltaX, float deltaY, float cosAngle, float sinAngle,
		                                     ResizeModifiers modifiers,
		                                     out int centerOffsetX, out int centerOffsetY, out int newWidth, out int newHeight)
		{
			int minWidth = 0, minHeight = 0;
			GetMinValues(width, height, modifiers, out minWidth, out minHeight);
			return MoveRectangleLeft(width, height, minWidth, 0.5f, 0.5f, 2, 2, deltaX, deltaY,
			                         cosAngle, sinAngle, modifiers, out centerOffsetX, out centerOffsetY, out newWidth,
			                         out newHeight);
		}


		/// <summary>
		/// Moves the left side of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="minValueX">Specifies the minimum width.</param>
		/// <param name="centerPosFactorX">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="centerPosFactorY">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="divFactorX">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="divFactorY">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleLeft(int width, int height, int minValueX,
		                                     float centerPosFactorX, float centerPosFactorY, int divFactorX, int divFactorY,
		                                     float deltaX, float deltaY, double cosAngle, double sinAngle,
		                                     ResizeModifiers modifiers,
		                                     out int centerOffsetX, out int centerOffsetY, out int newWidth, out int newHeight)
		{
			bool result = true;
			centerOffsetX = centerOffsetY = 0;
			newWidth = width;
			newHeight = height;
			// aspect maintainence can be combined with both MirroredResizing and normal resizing
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0) {
				//MaintainAspectRatio(newWidth, height, -1, 1, ref deltaX, ref deltaY);
				float aspectRatio = width/(float) height;
				deltaY = deltaX/aspectRatio;
			}
			// Align to integer coordinates
			Geometry.AlignMovement(modifiers, divFactorX, divFactorY, ref deltaX, ref deltaY);

			if ((modifiers & ResizeModifiers.MirroredResize) != 0) {
				if (newWidth - deltaX - deltaX >= minValueX)
					newWidth -= (int) Math.Round(deltaX + deltaX);
				else {
					newWidth = minValueX;
					result = false;
				}
			}
			else {
				if (newWidth - deltaX >= minValueX)
					newWidth -= (int) Math.Round(deltaX);
				else {
					deltaX = newWidth;
					newWidth = minValueX;
					result = false;
				}
				centerOffsetX = (int) Math.Round(deltaX*centerPosFactorX*cosAngle);
				centerOffsetY = (int) Math.Round(deltaX*centerPosFactorX*sinAngle);
			}
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0)
				newHeight = (int) Math.Round(newWidth/(width/(float) height));
			return result;
		}


		/// <summary>
		/// Moves the right side of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleRight(int width, int height, float deltaX, float deltaY, float cosAngle,
		                                      float sinAngle, ResizeModifiers modifiers,
		                                      out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                      out int newHeight)
		{
			int minWidth = 0, minHeight = 0;
			GetMinValues(width, height, modifiers, out minWidth, out minHeight);
			return MoveRectangleRight(width, height, minWidth, 0.5f, 0.5f, 2, 2, deltaX, deltaY, cosAngle, sinAngle, modifiers,
			                          out centerOffsetX, out centerOffsetY, out newWidth, out newHeight);
		}


		/// <summary>
		/// Moves the right side of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="minValueX">Specifies the minimum width.</param>
		/// <param name="centerPosFactorX">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="centerPosFactorY">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="divFactorX">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="divFactorY">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleRight(int width, int height, int minValueX,
		                                      float centerPosFactorX, float centerPosFactorY, int divFactorX, int divFactorY,
		                                      float deltaX, float deltaY, double cosAngle, double sinAngle,
		                                      ResizeModifiers modifiers,
		                                      out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                      out int newHeight)
		{
			bool result = true;
			centerOffsetX = centerOffsetY = 0;
			newWidth = width;
			newHeight = height;
			// Aspect maintainence can be combined with both MirroredResizing and normal resizing
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0) {
				//MaintainAspectRatio(newWidth, height, 1, -1, ref deltaX, ref deltaY);
				float aspectRatio = width/(float) height;
				deltaY = -deltaX/aspectRatio;
			}
			// Align to integer coordinates
			Geometry.AlignMovement(modifiers, divFactorX, divFactorY, ref deltaX, ref deltaY);

			if ((modifiers & ResizeModifiers.MirroredResize) != 0) {
				if (newWidth + deltaX + deltaX >= minValueX)
					newWidth += (int) Math.Round(deltaX + deltaX);
				else {
					newWidth = minValueX;
					result = false;
				}
			}
			else {
				if (newWidth + deltaX >= minValueX)
					newWidth += (int) Math.Round(deltaX);
				else {
					deltaX = minValueX - newWidth;
					newWidth = minValueX;
					result = false;
				}
				centerOffsetX = (int) Math.Round(deltaX*centerPosFactorX*cosAngle);
				centerOffsetY = (int) Math.Round(deltaX*centerPosFactorX*sinAngle);
			}
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0)
				newHeight = (int) Math.Round(newWidth/(width/(float) height));
			return result;
		}


		/// <summary>
		/// Moves the BottomLeft corner of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleBottomLeft(int width, int height, float deltaX, float deltaY, float cosAngle,
		                                           float sinAngle, ResizeModifiers modifiers,
		                                           out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                           out int newHeight)
		{
			int minWidth = 0, minHeight = 0;
			GetMinValues(width, height, modifiers, out minWidth, out minHeight);
			return MoveRectangleBottomLeft(width, height, minWidth, minHeight, 0.5f, 0.5f, 2, 2, deltaX, deltaY, cosAngle,
			                               sinAngle, modifiers, out centerOffsetX, out centerOffsetY, out newWidth, out newHeight);
		}


		/// <summary>
		/// Moves the BottomLeft corner of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="minValueX">Specifies the minimum width.</param>
		/// <param name="minValueY">Specifies the minimum height.</param>
		/// <param name="centerPosFactorX">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="centerPosFactorY">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="divFactorX">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="divFactorY">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleBottomLeft(int width, int height, int minValueX, int minValueY,
		                                           float centerPosFactorX, float centerPosFactorY, int divFactorX,
		                                           int divFactorY,
		                                           float deltaX, float deltaY, double cosAngle, double sinAngle,
		                                           ResizeModifiers modifiers,
		                                           out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                           out int newHeight)
		{
			bool result = true;
			centerOffsetX = centerOffsetY = 0;
			newWidth = width;
			newHeight = height;

			// Aspect maintainence can be combined with both MirroredResizing and normal resizing
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0)
				MaintainAspectRatio(newWidth, newHeight, -1, 1, ref deltaX, ref deltaY);
			// Align to integer coordinates
			Geometry.AlignMovement(modifiers, divFactorX, divFactorY, ref deltaX, ref deltaY);

			if ((modifiers & ResizeModifiers.MirroredResize) != 0) {
				if (newWidth - deltaX - deltaX >= minValueX)
					newWidth -= (int) Math.Round(deltaX + deltaX);
				else {
					newWidth = minValueX;
					result = false;
				}
				if (newHeight + deltaY + deltaY >= minValueY)
					newHeight += (int) Math.Round(deltaY + deltaY);
				else {
					newHeight = minValueY;
					result = false;
				}
			}
			else {
				if (newWidth - deltaX >= minValueX) {
					newWidth -= (int) Math.Round(deltaX);
				}
				else {
					deltaX = newWidth;
					newWidth = minValueX;
					result = false;
				}
				if (newHeight + deltaY >= minValueY) {
					newHeight += (int) Math.Round(deltaY);
				}
				else {
					deltaY = -newHeight;
					newHeight = minValueY;
					result = false;
				}
				centerOffsetX = (int) Math.Round((deltaX*centerPosFactorX*cosAngle) - (deltaY*centerPosFactorY*sinAngle));
				centerOffsetY = (int) Math.Round((deltaX*centerPosFactorX*sinAngle) + (deltaY*centerPosFactorY*cosAngle));
			}
			return result;
		}


		/// <summary>
		/// Moves the bottom side of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleBottom(int width, int height, float deltaX, float deltaY, float cosAngle,
		                                       float sinAngle, ResizeModifiers modifiers, out int centerOffsetX,
		                                       out int centerOffsetY, out int newWidth, out int newHeight)
		{
			int minWidth = 0, minHeight = 0;
			GetMinValues(width, height, modifiers, out minWidth, out minHeight);
			return MoveRectangleBottom(width, height, minHeight, 0.5f, 0.5f, 2, 2, deltaX, deltaY, cosAngle, sinAngle, modifiers,
			                           out centerOffsetX, out centerOffsetY, out newWidth, out newHeight);
		}


		/// <summary>
		/// Moves the bottom side of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="minValueY">Specifies the minimum height.</param>
		/// <param name="centerPosFactorX">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="centerPosFactorY">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="divFactorX">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="divFactorY">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleBottom(int width, int height, int minValueY,
		                                       float centerPosFactorX, float centerPosFactorY, int divFactorX, int divFactorY,
		                                       float deltaX, float deltaY, double cosAngle, double sinAngle,
		                                       ResizeModifiers modifiers,
		                                       out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                       out int newHeight)
		{
			bool result = true;
			centerOffsetX = centerOffsetY = 0;
			newWidth = width;
			newHeight = height;

			// Aspect maintainence can be combined with both MirroredResizing and normal resizing
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0) {
				//MaintainAspectRatio(width, newHeight, 1, 1, ref deltaX, ref deltaY);
				float aspectRatio = width/(float) height;
				deltaX = -deltaY*aspectRatio;
			}
			// Align to integer coordinates
			Geometry.AlignMovement(modifiers, divFactorX, divFactorY, ref deltaX, ref deltaY);

			if ((modifiers & ResizeModifiers.MirroredResize) != 0) {
				if (newHeight + deltaY + deltaY >= minValueY)
					newHeight += (int) Math.Round(deltaY + deltaY);
				else {
					newHeight = minValueY;
					result = false;
				}
			}
			else {
				if (newHeight + deltaY >= minValueY)
					newHeight += (int) Math.Round(deltaY);
				else {
					deltaY = -newHeight;
					newHeight = minValueY;
					result = false;
				}
				centerOffsetX = (int) Math.Round(-deltaY*centerPosFactorY*sinAngle);
				centerOffsetY = (int) Math.Round(deltaY*centerPosFactorY*cosAngle);
			}
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0)
				newWidth = (int) Math.Round(newHeight*(width/(float) height));
			return result;
		}


		/// <summary>
		/// Moves the BottomRight corner of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleBottomRight(int width, int height, float deltaX, float deltaY, float cosAngle,
		                                            float sinAngle, ResizeModifiers modifiers,
		                                            out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                            out int newHeight)
		{
			int minWidth = 0, minHeight = 0;
			GetMinValues(width, height, modifiers, out minWidth, out minHeight);
			return MoveRectangleBottomRight(width, height, minWidth, minHeight, 0.5f, 0.5f, 2, 2, deltaX, deltaY, cosAngle,
			                                sinAngle, modifiers, out centerOffsetX, out centerOffsetY, out newWidth,
			                                out newHeight);
		}


		/// <summary>
		/// Moves the BottomRight corner of a (rotated) rectangle in order to resize it
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="minValueX">Specifies the minimum width.</param>
		/// <param name="minValueY">Specifies the minimum height.</param>
		/// <param name="centerPosFactorX">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="centerPosFactorY">Specifies where the center is located. Default value is 50% = 0.5</param>
		/// <param name="divFactorX">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="divFactorY">Specifies the factor through which the corrected and modified movement must be dividable without remainding.</param>
		/// <param name="deltaX">Movement on X axis</param>
		/// <param name="deltaY">Movement on Y axis</param>
		/// <param name="cosAngle">Cosinus value of the rectangle's rotation angle</param>
		/// <param name="sinAngle">Sinus value of the rectangle's rotation angle</param>
		/// <param name="modifiers">Movement modifiers</param>
		/// <param name="centerOffsetX">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="centerOffsetY">Specifies the movement the rectangle's center has to perform for resizing</param>
		/// <param name="newWidth">New width of the rectangle</param>
		/// <param name="newHeight">New height of the rectangle</param>
		/// <returns>Returns true if the movement could be performed as desired. 
		/// Returns false if the movement could not be performed completely because of movement restrictions</returns>
		public static bool MoveRectangleBottomRight(int width, int height, int minValueX, int minValueY,
		                                            float centerPosFactorX, float centerPosFactorY, int divFactorX,
		                                            int divFactorY,
		                                            float deltaX, float deltaY, double cosAngle, double sinAngle,
		                                            ResizeModifiers modifiers,
		                                            out int centerOffsetX, out int centerOffsetY, out int newWidth,
		                                            out int newHeight)
		{
			bool result = true;
			centerOffsetX = centerOffsetY = 0;
			newWidth = width;
			newHeight = height;

			// Aspect maintainence can be combined with both MirroredResizing and normal resizing
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0)
				MaintainAspectRatio(width, height, 1, 1, ref deltaX, ref deltaY);
			// Align to integer coordinates
			Geometry.AlignMovement(modifiers, divFactorX, divFactorY, ref deltaX, ref deltaY);

			if ((modifiers & ResizeModifiers.MirroredResize) != 0) {
				if (newWidth + deltaX + deltaX >= minValueX)
					newWidth += (int) Math.Round(deltaX + deltaX);
				else {
					newWidth = minValueX;
					result = false;
				}
				if (newHeight + deltaY + deltaY >= minValueY)
					newHeight += (int) Math.Round(deltaY + deltaY);
				else {
					newHeight = minValueY;
					result = false;
				}
			}
			else {
				if (newWidth + deltaX >= minValueX)
					newWidth += (int) Math.Round(deltaX);
				else {
					deltaX = -newWidth;
					newWidth = minValueX;
					result = false;
				}
				if (newHeight + deltaY >= minValueY)
					newHeight += (int) Math.Round(deltaY);
				else {
					deltaY = -newHeight;
					newHeight = minValueY;
					result = false;
				}
				centerOffsetX = (int) Math.Round((deltaX*centerPosFactorX*cosAngle) - (deltaY*centerPosFactorY*sinAngle));
				centerOffsetY = (int) Math.Round((deltaX*centerPosFactorX*sinAngle) + (deltaY*centerPosFactorY*cosAngle));
			}
			return result;
		}


		/// <summary>
		/// Moves the arrow tip of a rectangle based arrow
		/// </summary>
		public static bool MoveArrowPoint(Point center, Point movedPtPos, Point fixedPtPos, int angle, int minWidth,
		                                  float centerPosFactorX, int untransformedDeltaX, int untransformedDeltaY,
		                                  ResizeModifiers modifiers, out int centerOffsetX, out int centerOffsetY,
		                                  out int newWidth, out int newAngle)
		{
			centerOffsetX = centerOffsetY = 0;
			newAngle = angle;
			newWidth = 0;

			// calculate new position of moved point
			Point newPtPos = movedPtPos;
			newPtPos.Offset(untransformedDeltaX, untransformedDeltaY);

			// calculate new shape location
			PointF newCenter = VectorLinearInterpolation((float) fixedPtPos.X, (float) fixedPtPos.Y, (float) newPtPos.X,
			                                             (float) newPtPos.Y, centerPosFactorX);
			centerOffsetX = (int) Math.Round(newCenter.X - center.X);
			centerOffsetY = (int) Math.Round(newCenter.Y - center.Y);

			// calculate new angle
			float newAng = (360 + RadiansToDegrees(Geometry.Angle(fixedPtPos, newPtPos)))%360;
			float oldAng = (360 + RadiansToDegrees(Geometry.Angle(fixedPtPos, movedPtPos)))%360;
			newAngle = angle + DegreesToTenthsOfDegree(newAng - oldAng);

			// calculate new width
			newWidth = (int) Math.Round(DistancePointPoint(fixedPtPos, newPtPos));

			return (movedPtPos.X == newPtPos.X && movedPtPos.Y == newPtPos.Y);
		}

		#endregion

		#region Vector functions

		/// <summary>
		/// Calculates the dot product a * b
		/// </summary>
		public static int VectorDotProduct(int aX, int aY, int bX, int bY)
		{
			return aX*bX + aY*bY;
		}


		/// <summary>
		/// Calculates the dot product a * b
		/// </summary>
		public static float VectorDotProduct(float aX, float aY, float bX, float bY)
		{
			return aX*bX + aY*bY;
		}


		/// <summary>
		/// Calculates the dot product a * b
		/// </summary>
		public static int VectorDotProduct(Point a, Point b)
		{
			return VectorDotProduct(a.X, a.Y, b.X, b.Y);
		}


		/// <summary>
		/// Calculates the dot product a * b
		/// </summary>
		public static float VectorDotProduct(PointF a, PointF b)
		{
			return VectorDotProduct(a.X, a.Y, b.X, b.Y);
		}


		/// <summary>
		/// Calculates the dot product ab * bc
		/// </summary>
		public static float VectorDotProduct(PointF a, PointF b, PointF c)
		{
			return VectorDotProduct(a.X, a.Y, b.X, b.Y, c.X, c.Y);
		}


		/// <summary>
		/// Calculates the dot product ab * bc
		/// </summary>
		public static float VectorDotProduct(float aX, float aY, float bX, float bY, float cX, float cY)
		{
			float abX = bX - aX;
			float abY = bY - aY;
			float bcX = cX - bX;
			float bcY = cY - bY;
			return abX*bcX + abY*bcY;
		}


		/// <summary>
		/// Calculates the dot product ab * bc
		/// </summary>
		public static int VectorDotProduct(Point a, Point b, Point c)
		{
			return VectorDotProduct(a.X, a.Y, b.X, b.Y, c.X, c.Y);
		}


		/// <summary>
		/// Calculates the dot product ab * bc
		/// </summary>
		// Note: We should have calculated ba * bc instead, that would be more conformant to the expectations.
		// The result should be proportional to the cos of the angle between the two vectors.
		public static int VectorDotProduct(int aX, int aY, int bX, int bY, int cX, int cY)
		{
			int abX = bX - aX;
			int abY = bY - aY;
			int bcX = cX - bX;
			int bcY = cY - bY;
			return abX*bcX + abY*bcY;
			// Sometimes, the products get larger than int.MaxValue!
			//return (int)((float)abX * (float)bcX + (float)abY * (float)bcY);
		}


		/// <summary>
		/// Calculates the scalar/dot product between vectors A-B and C-D
		/// </summary>
		public static int CalcScalarProduct(int aX, int aY, int bX, int bY, int cX, int cY, int dX, int dY)
		{
			return (bX - aX)*(dX - cX) + (bY - aY)*(dY - cY);
		}


		/// <summary>
		/// Calculates the cross product a x b
		/// </summary>
		/// <remarks>What we actually calculate here is the z coordinate of the cross product vector in R³
		/// with aZ and bZ assumed to be zero. Result is the size of the area of the parallelogram A B A' B'
		/// </remarks>
		public static int VectorCrossProduct(int aX, int aY, int bX, int bY)
		{
			return aX*bY - aY*bX;
			// Sometimes, the products get larger than int.MaxValue!
			//return (int)((float)aX * (float)bY - (float)aY * (float)bX);
		}


		/// <summary>
		/// Calculates the cross product ab x ac
		/// </summary>
		public static float VectorCrossProduct(float aX, float aY, float bX, float bY)
		{
			return aX*bY - aY*bX;
		}


		/// <summary>
		/// Calculates the cross product ab x ac
		/// </summary>
		public static int VectorCrossProduct(Point a, Point b, Point c)
		{
			return VectorCrossProduct(a.X, a.Y, b.X, b.Y, c.X, c.Y);
		}


		/// <summary>
		/// Calculates the cross product ab x ac
		/// </summary>
		public static int VectorCrossProduct(int aX, int aY, int bX, int bY, int cX, int cY)
		{
			int abX = bX - aX;
			int abY = bY - aY;
			int acX = cX - aX;
			int acY = cY - aY;
			return (int) ((float) abX*(float) acY - (float) abY*(float) acX);
		}


		/// <summary>
		/// Calculates the cross product ab x ac
		/// </summary>
		public static float VectorCrossProduct(PointF a, PointF b, PointF c)
		{
			return VectorCrossProduct(a.X, a.Y, b.X, b.Y, c.X, c.Y);
		}


		/// <summary>
		/// Calculates the cross product ab x ac
		/// </summary>
		public static float VectorCrossProduct(float aX, float aY, float bX, float bY, float cX, float cY)
		{
			return ((aX - bX)*(aY - cY) - (aY - bY)*(aX - cX));
		}


		/// <summary>
		/// Computes a point on the line segment ab located db% away from a
		/// </summary>
		/// <param name="a">Point defining one end of the line segment</param>
		/// <param name="b">Point defining the other end of the line segment</param>
		/// <param name="t">Position of the calculated point. E.g. 0.5 returns the middle of a and b.</param>
		public static Point VectorLinearInterpolation(Point a, Point b, float t)
		{
			return VectorLinearInterpolation(a.X, a.Y, b.X, b.Y, t);
		}


		/// <summary>
		/// Computes a point on the line segment ab located db% away from a
		/// </summary>
		/// <param name="aX">X coordinate of the line's first point</param>
		/// <param name="aY">Y coordinate of the line's first point</param>
		/// <param name="bX">X coordinate of the line's second point</param>
		/// <param name="bY">Y coordinate of the line's second point</param>
		/// <param name="t">Position of the calculated point. E.g. 0.5 returns the middle of a and b.</param>
		public static Point VectorLinearInterpolation(int aX, int aY, int bX, int bY, float t)
		{
			Point result = Point.Empty;
			int dX = bX - aX;
			int dY = bY - aY;
			result.X = aX + (int) Math.Round(dX*t);
			result.Y = aY + (int) Math.Round(dY*t);
			return result;
		}


		/// <summary>
		/// Computes a point on the line segment ab located (t * 100)% away from a
		/// </summary>
		/// <param name="a">Point defining one end of the line segment</param>
		/// <param name="b">Point defining the other end of the line segment</param>
		/// <param name="t">Position of the calculated point. E.g. 0.5 returns the middle of a and b.</param>
		public static PointF VectorLinearInterpolation(PointF a, PointF b, float t)
		{
			return VectorLinearInterpolation(a.X, a.Y, b.X, b.Y, t);
		}


		/// <summary>
		/// Computes a point on the line segment ab located db% away from a
		/// </summary>
		/// <param name="aX">X coordinate of the line's first point</param>
		/// <param name="aY">Y coordinate of the line's first point</param>
		/// <param name="bX">X coordinate of the line's second point</param>
		/// <param name="bY">Y coordinate of the line's second point</param>
		/// <param name="t">Position of the calculated point. E.g. 0.5 returns the middle of a and b.</param>
		public static PointF VectorLinearInterpolation(float aX, float aY, float bX, float bY, float t)
		{
			PointF result = Point.Empty;
			float dX = bX - aX;
			float dY = bY - aY;
			result.X = (float) (aX + (dX*t));
			result.Y = (float) (aY + (dY*t));
			return result;
		}

		#endregion

		#region Calculation of normal vectors

		/// <summary>
		/// Calculates the normal vector of the line in foot F.
		/// </summary>
		/// <param name="x1">x coordinate of first line definition point</param>
		/// <param name="y1">y coordinate of first line definition point</param>
		/// <param name="x2">x coordinate of second line definition point</param>
		/// <param name="y2">y coordinate of second line definition point</param>
		/// <param name="fX">x coordinate of foot point</param>
		/// <param name="fY">y coordinate of foot point</param>
		/// <param name="vectorLength">desired length of normal vector</param>
		/// <param name="pX">x coordinate of normal vector end point</param>
		/// <param name="pY">y coordinate of normal vector end point</param>
		// The direction of the resulting vector should point towards the half-plane that does not contain the origin.
		public static void CalcNormalVectorOfLine(int x1, int y1, int x2, int y2, int fX, int fY, int vectorLength, out int pX,
		                                          out int pY)
		{
			int a, b, c;
			CalcLine(x1, y1, x2, y2, out a, out b, out c);
			double l;
			if (a == 0 && b == 0) l = 1;
			else l = Math.Sqrt(a*a + b*b);
			// Since a, b, c is (almost) the Hesse normal form, (a, b) is the normal vector of A-B.
			pX = (int) (fX + vectorLength*a/l);
			pY = (int) (fY + vectorLength*b/l);
		}


		/// <summary>
		/// Calculates the normal vector of a rectangle. 
		/// If the point p is not on the outline of the rectangle, the resulting normal vector will be translated to the outline.
		/// </summary>
		public static Point CalcNormalVectorOfRectangle(Rectangle rectangle, Point p, int vectorLength)
		{
			return CalcNormalVectorOfRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, p.X, p.Y,
			                                   vectorLength);
		}


		/// <summary>
		/// Calculates the normal vector of a rectangle. 
		/// If the point p is not on the outline of the rectangle, the resulting notmal vector will be translated to the outline.
		/// </summary>
		public static Point CalcNormalVectorOfRectangle(Rectangle rectangle, int ptX, int ptY, int vectorLength)
		{
			return CalcNormalVectorOfRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, ptX, ptY,
			                                   vectorLength);
		}


		/// <summary>
		/// Calculates the normal vector in (ptX, ptY) with respect to the bounds of a rectangle. 
		/// If the point (ptX,ptY) is not on the outline of the rectangle, the resulting normal vector will be translated to the outline.
		/// </summary>
		public static Point CalcNormalVectorOfRectangle(int x, int y, int width, int height, int ptX, int ptY,
		                                                int vectorLength)
		{
			Point normalVector = Point.Empty;
			normalVector.X = ptX;
			normalVector.Y = ptY;
			if (x <= ptX && ptX <= x + width && y <= ptY && ptY <= y + height) {
				Point topLeft = Point.Empty, topRight = Point.Empty, bottomLeft = Point.Empty, bottomRight = Point.Empty;
				topLeft.X = bottomLeft.X = y;
				topLeft.Y = topRight.Y = y;
				topRight.X = bottomRight.X = x + width;
				bottomLeft.Y = bottomRight.Y = y + height;
				Point center = Point.Empty, p = Point.Empty;
				center.X = x + (width/2);
				center.Y = y + (height/2);
				p.X = ptX;
				p.Y = ptY;
				if (TriangleContainsPoint(topLeft, topRight, center, p)) {
					normalVector.X = ptX;
					normalVector.Y = y - vectorLength;
				}
				else if (TriangleContainsPoint(bottomLeft, bottomRight, center, p)) {
					normalVector.X = ptX;
					normalVector.Y = y + height + vectorLength;
				}
				else if (TriangleContainsPoint(topLeft, bottomLeft, center, p)) {
					normalVector.X = x - vectorLength;
					normalVector.Y = ptY;
				}
				else if (TriangleContainsPoint(topRight, bottomRight, center, p)) {
					normalVector.X = x + width + vectorLength;
					normalVector.Y = ptY;
				}
				else Debug.Fail(string.Format("Unable to calculate normal vector of {0}", new Point(ptX, ptY)));
			}
			return normalVector;
		}


		/// <summary>
		/// Calculates the normal vector of a circle. 
		/// If the point ptX/ptY is not on the outline of the circle, the resulting notmal vector will be translated to the outline.
		/// </summary>
		public static Point CalcNormalVectorOfCircle(int centerX, int centerY, int radius, int ptX, int ptY, int vectorLength)
		{
			Point normalVector = Point.Empty;
			normalVector.Offset(ptX, ptY);
			if (CircleContainsPoint(centerX, centerY, radius, ptX, ptY, 0)) {
				Point intersectionPt = IntersectCircleWithLine(centerX, centerY, radius, ptX, ptY, centerX, centerY, false);
				if (IsValid(intersectionPt)) {
					float d = (float) radius/vectorLength;
					normalVector = VectorLinearInterpolation(centerX, centerY, intersectionPt.X, intersectionPt.Y, d);
				}
				else Debug.Fail("No intersection between circle and line");
			}
			return normalVector;
		}

		#endregion

		#region Hit test functions

		/// <summary>
		/// Returns true if point p is inside or on the bounds of the triangle a/b/c
		/// </summary>
		/// <param name="a">First point of the triangle</param>
		/// <param name="b">Second point of the triangle</param>
		/// <param name="c">Third point of the triangle</param>
		/// <param name="p">The testee</param>
		/// <returns></returns>
		public static bool TriangleContainsPoint(Point a, Point b, Point c, Point p)
		{
			return TriangleContainsPoint(a.X, a.Y, b.X, b.Y, c.X, c.Y, p.X, p.Y);
		}


		/// <summary>
		/// Returns true if point x/y is inside or on the bounds of the triangle a/b/c
		/// </summary>
		public static bool TriangleContainsPoint(Point a, Point b, Point c, int x, int y)
		{
			return TriangleContainsPoint(a.X, a.Y, b.X, b.Y, c.X, c.Y, x, y);
		}


		/// <summary>
		/// Returns true if point x/y is inside or on the bounds of the triangle a/b/c
		/// </summary>
		public static bool TriangleContainsPoint(int aX, int aY, int bX, int bY, int cX, int cY, int x, int y)
		{
			bool ab = VectorCrossProduct(aX, aY, bX, bY, x, y) < 0;
			bool bc = VectorCrossProduct(bX, bY, cX, cY, x, y) < 0;
			bool ca = VectorCrossProduct(cX, cY, aX, aY, x, y) < 0;
			return ((ab == bc) && (ab == ca));
		}


		/// <summary>
		/// Returns true if point p is inside or on the bounds of the triangle a/b/c
		/// </summary>
		/// <param name="a">First point of the triangle</param>
		/// <param name="b">Second point of the triangle</param>
		/// <param name="c">Third point of the triangle</param>
		/// <param name="p">The testee</param>
		/// <returns></returns>
		public static bool TriangleContainsPoint(PointF a, PointF b, PointF c, PointF p)
		{
			return TriangleContainsPoint(a.X, a.Y, b.X, b.Y, c.X, c.Y, p.X, p.Y);
		}


		/// <summary>
		/// Returns true if point x/y is inside or on the bounds of the triangle a/b/c
		/// </summary>
		public static bool TriangleContainsPoint(float aX, float aY, float bX, float bY, float cX, float cY, float x, float y)
		{
			bool ab = VectorCrossProduct(aX, aY, bX, bY, x, y) < 0;
			bool bc = VectorCrossProduct(bX, bY, cX, cY, x, y) < 0;
			bool ca = VectorCrossProduct(cX, cY, aX, aY, x, y) < 0;
			return ((ab == bc) && (ab == ca));
		}


		/// <summary>
		/// Returns true if point x/y is inside or on the bounds of the quadrangle a/b/c/d
		/// </summary>
		/// <param name="a">First point of the quadrangle</param>
		/// <param name="b">Second point of the quadrangle</param>
		/// <param name="c">Third point of the quadrangle</param>
		/// <param name="d">Fourth point of the quadrangle</param>
		/// <param name="x">X coordinate to test.</param>
		/// <param name="y">Y coordinate to test.</param>
		public static bool QuadrangleContainsPoint(Point a, Point b, Point c, Point d, int x, int y)
		{
			return QuadrangleContainsPoint(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y, x, y);
		}


		/// <summary>
		/// Returns true if point x/y is inside or on the bounds of the quadrangle a/b/c/d
		/// </summary>
		public static bool QuadrangleContainsPoint(int aX, int aY, int bX, int bY, int cX, int cY, int dX, int dY, int x,
		                                           int y)
		{
			// ToDo: Check whether shuffling points affects the result
			bool ab = VectorCrossProduct(aX, aY, bX, bY, x, y) < 0;
			bool bc = VectorCrossProduct(bX, bY, cX, cY, x, y) < 0;
			bool cd = VectorCrossProduct(cX, cY, dX, dY, x, y) < 0;
			bool da = VectorCrossProduct(dX, dY, aX, aY, x, y) < 0;
			return ((ab == bc) && (ab == cd) && (ab == da));
		}


		/// <summary>
		/// Returns true if point p is inside or on the bounds of the quadrangle a/b/c/d
		/// </summary>
		/// <param name="a">First point of the quadrangle</param>
		/// <param name="b">Second point of the quadrangle</param>
		/// <param name="c">Third point of the quadrangle</param>
		/// <param name="d">Fourth point of the quadrangle</param>
		/// <param name="x">X coordinate to test.</param>
		/// <param name="y">Y coordinate to test.</param>
		/// <returns></returns>
		public static bool QuadrangleContainsPoint(PointF a, PointF b, PointF c, PointF d, float x, float y)
		{
			return QuadrangleContainsPoint(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y, x, y);
		}


		/// <summary>
		/// Returns true if point x/y is inside or on the bounds of the quadrangle a/b/c/d
		/// </summary>
		public static bool QuadrangleContainsPoint(float aX, float aY, float bX, float bY, float cX, float cY, float dX,
		                                           float dY, float x, float y)
		{
			if ((x == aX || x == bX || x == cX || x == dX) && (y == aY || y == bY || y == cY || y == dY))
				return true;
			bool ab = VectorCrossProduct(aX, aY, bX, bY, x, y) < 0;
			bool bc = VectorCrossProduct(bX, bY, cX, cY, x, y) < 0;
			bool cd = VectorCrossProduct(cX, cY, dX, dY, x, y) < 0;
			bool da = VectorCrossProduct(dX, dY, aX, aY, x, y) < 0;
			return ((ab == bc) && (ab == cd) && (ab == da));
		}


		/// <summary>
		/// Tests if point p is inside or on the bounds of the given rectangle.
		/// </summary>
		public static bool RectangleContainsPoint(int rX, int rY, int rWidth, int rHeight, int pX, int pY)
		{
			return RectangleContainsPoint(rX, rY, rWidth, rHeight, pX, pY, true);
		}


		/// <summary>
		/// Tests if point x/y is inside or on the bounds of the given rectangle.
		/// </summary>
		public static bool RectangleContainsPoint(Rectangle r, Point p)
		{
			return RectangleContainsPoint(r.X, r.Y, r.Width, r.Height, p.X, p.Y, true);
		}


		/// <summary>
		/// Tests if point x/y is inside or on the bounds of the given rectangle.
		/// </summary>
		public static bool RectangleContainsPoint(Rectangle r, int x, int y)
		{
			return RectangleContainsPoint(r.X, r.Y, r.Width, r.Height, x, y, true);
		}


		/// <summary>
		/// Tests if point p is inside the given rectangle. The bounds of the rectangle can be excluded.
		/// </summary>
		public static bool RectangleContainsPoint(int rectX, int rectY, int rectWidth, int rectHeight, float rectRotationAngle,
		                                          int pX, int pY, bool withBounds)
		{
			if (rectRotationAngle != 0 && rectRotationAngle%180 != 0) {
				float x = pX;
				float y = pY;
				RotatePoint(rectX + (rectWidth/2f), rectY + (rectHeight/2f), -rectRotationAngle, ref x, ref y);
				return RectangleContainsPoint(rectX, rectY, rectWidth, rectHeight, x, y);
			}
			else return RectangleContainsPoint(rectX, rectY, rectWidth, rectHeight, pX, pY);
		}


		/// <summary>
		/// Tests if point p is inside the given rectangle. The bounds of the rectangle can be excluded.
		/// </summary>
		public static bool RectangleContainsPoint(int rectX, int rectY, int rWidth, int rHeight, int pX, int pY,
		                                          bool withBounds)
		{
			bool result = false;
			if (withBounds) {
				if ((pX >= rectX && pX <= rectX + rWidth) && (pY >= rectY && pY <= rectY + rHeight))
					result = true;
			}
			else {
				if ((pX > rectX && pX < rectX + rWidth) && (pY > rectY && pY < rectY + rHeight))
					result = true;
			}
			return result;
		}


		/// <summary>
		/// Tests if point p is inside the given rectangle
		/// </summary>
		public static bool RectangleContainsPoint(RectangleF r, PointF p)
		{
			return RectangleContainsPoint(r.X, r.Y, r.Width, r.Height, p.X, p.Y, true);
		}


		/// <summary>
		/// Tests if point p is inside the given rectangle
		/// </summary>
		public static bool RectangleContainsPoint(RectangleF r, float x, float y)
		{
			return RectangleContainsPoint(r.X, r.Y, r.Width, r.Height, x, y, true);
		}


		/// <summary>
		/// Tests if point p is inside the given rectangle
		/// </summary>
		public static bool RectangleContainsPoint(float rectX, float rectY, float rWidth, float rHeight, float pX, float pY)
		{
			return RectangleContainsPoint(rectX, rectY, rWidth, rHeight, pX, pY, true);
		}


		/// <summary>
		/// Tests if point p is inside the given rectangle. The bounds of the rectangle can be excluded.
		/// </summary>
		public static bool RectangleContainsPoint(float rectX, float rectY, float rWidth, float rHeight, float pX, float pY,
		                                          bool withBounds)
		{
			bool result = false;
			if (withBounds) {
				if ((pX >= rectX && pX <= rectX + rWidth) && (pY >= rectY && pY <= rectY + rHeight))
					result = true;
			}
			else {
				if ((pX > rectX && pX < rectX + rWidth) && (pY > rectY && pY < rectY + rHeight))
					result = true;
			}
			return result;
		}


		/// <summary>
		/// Tests if the given point array is a convex polygon.
		/// </summary>
		public static bool PolygonIsConvex(Point[] points)
		{
			if (points == null) throw new ArgumentNullException("points");
			int convex = 0;
			int n = points.Length - 1;
			float ai = (float) Math.Atan2(points[n].X - points[0].X, points[n].Y - points[0].Y);
			for (int i = 0; i < n; ++i) {
				int j = i + 1;
				float aj = (float) Math.Atan2(points[i].X - points[j].X, points[i].Y - points[j].Y);
				if (ai - aj < 0) convex++;
				ai = aj;
			}
			return (convex == 1 || convex == n - 1);
		}


		/// <summary>
		/// Tests if the given points define a convex polygon or not
		/// </summary>
		public static bool PolygonIsConvex(PointF[] points)
		{
			if (points == null) throw new ArgumentNullException("points");
			int convex = 0;
			int n = points.Length;
			float ai = (float) Math.Atan2(points[n - 1].X - points[0].X, points[n - 1].Y - points[0].Y);
			for (int i = 0; i < n; ++i) {
				int j = i + 1;
				float aj = (float) Math.Atan2(points[i].X - points[j].X, points[i].Y - points[j].Y);
				if (ai - aj < 0) convex++;
				ai = aj;
			}
			return (convex == 1 || convex == n - 1);
		}


		/// <summary>
		/// Tests if point x/y is inside or on the bounds of the given (convex) polygon
		/// </summary>
		public static bool ConvexPolygonContainsPoint(Point[] points, int x, int y)
		{
			if (points == null) throw new ArgumentNullException("points");
			// Store the cross product of the points.
			// If all the points have the same cross product (this means that they 
			// are on the same side), the point is inside the convex polygon.
			int maxIdx = points.Length - 1;
			bool z = VectorCrossProduct(points[maxIdx].X, points[maxIdx].Y, points[0].X, points[0].Y, x, y) < 0;
			int j;
			for (int i = 0; i < maxIdx; ++i) {
				j = i + 1;
				if (VectorCrossProduct(points[i].X, points[i].Y, points[j].X, points[j].Y, x, y) < 0 != z)
					return false;
			}
			return true;
		}


		/// <summary>
		/// Tests if point x/y is inside or on the bounds of the given (convex) polygon.
		/// PointF's of the polygon will be rounded to Point.
		/// </summary>
		public static bool ConvexPolygonContainsPoint(PointF[] points, int x, int y)
		{
			if (points == null) throw new ArgumentNullException("points");
			int maxIdx = points.Length - 1;
			if (points[0] == points[maxIdx]) --maxIdx;
			// Store the cross product of the points.
			// If all the points have the same cross product (this means that they 
			// are on the same side), the point is inside the convex polygon.
			bool z = VectorCrossProduct(
				(int) Math.Round(points[maxIdx].X),
				(int) Math.Round(points[maxIdx].Y),
				(int) Math.Round(points[0].X),
				(int) Math.Round(points[0].Y),
				x,
				y) < 0;
			int j;
			for (int i = 0; i < maxIdx; ++i) {
				j = i + 1;
				if (VectorCrossProduct(
					(int) Math.Round(points[i].X),
					(int) Math.Round(points[i].Y),
					(int) Math.Round(points[j].X),
					(int) Math.Round(points[j].Y),
					x,
					y) < 0 != z)
					return false;
			}
			return true;
		}


		/// <summary>
		/// Tests if point x/y is inside or on the bounds of the given (convex) polygon
		/// </summary>
		public static bool ConvexPolygonContainsPoint(PointF[] points, float x, float y)
		{
			if (points == null) throw new ArgumentNullException("points");
			// If vector a is less than 180 degrees clockwise from b, the value is positive
			// If all cross products are positive or all cross products are negative, this means 
			// that the point is always on the same side and this means that the point is inside
			int n = points.Length - 1;
			if (points[0] == points[n]) --n;
			bool z = VectorCrossProduct(points[n].X, points[n].Y, points[0].X, points[0].Y, x, y) < 0;
			int j;
			for (var i = 0; i < n; i++) {
				j = i + 1;
				if (VectorCrossProduct(points[i].X, points[i].Y, points[j].X, points[j].Y, x, y) < 0 != z)
					return false;
			}
			return true;
		}


		/// <summary>
		/// Tests if point x/y is inside or on the bounds of the given polygon
		/// </summary>
		public static bool PolygonContainsPoint(Point[] points, int x, int y)
		{
			int cnt = points.Length;
			if (cnt == 0) return false;
			else if (cnt == 1) return points[0].X == x && points[0].Y == y;
			else {
				// Test intersection of a line between the point and a point far, far away
				// If the number of intersections is even, the point must be outside, otherwise it is inside.
				int intersectionCnt = 0;
				long x2 = int.MaxValue, y2 = int.MaxValue;
				int j;
				for (int i = 0; i < cnt; ++i) {
					j = i + 1;
					if (j == cnt) j = 0;
					if (DistancePointLine(x, y, points[i].X, points[i].Y, points[j].X, points[j].Y, true) == 0)
						return true;
					else if (LineSegmentIntersectsWithLineSegment(x, y, x2, y2, points[i].X, points[i].Y, points[j].X, points[j].Y))
						intersectionCnt++;
				}
				return (intersectionCnt%2) != 0;
			}
		}

		#region Old version of ConvexPolygonContainsPoint (slower)

		//public static bool ConvexPolygonContainsPoint(PointF[] points, float x, float y) {
		//   // ist der Punkt immer auf der selben Seite der einzelnen Geraden, so ist er innerhalb der Figur
		//   int maxIdx = points.Length - 1;
		//   float r;
		//   float d = DistancePointLine2(x, y, points[0].X, points[0].Y, points[1].X, points[1].Y);
		//   for (int i = 1; i < maxIdx; ++i) {
		//      r = DistancePointLine2(x, y, points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y);
		//      if (d == 0) d = r;
		//      else if (d < 0) {
		//         if (r > 0) return false;
		//      }
		//      else if (d > 0) {
		//         if (r < 0) return false;
		//      }
		//   }
		//   r = DistancePointLine2(x, y, points[maxIdx].X, points[maxIdx].Y, points[0].X, points[0].Y);
		//   if (d < 0) { 
		//      if (r > 0) return false; 
		//   }
		//   else if (d > 0) { 
		//      if (r < 0) return false; 
		//   }
		//   return true;
		//}


		//public static bool ConvexPolygonContainsPoint(Point[] points, int x, int y) {
		//   // ist der Punkt immer auf der selben Seite der einzelnen Geraden, so ist er innerhalb der Figur
		//   float r;
		//   float d = DistancePointLine(x, y, points[0].X, points[0].Y, points[1].X, points[1].Y, false);
		//   for (int i = 1; i < points.Length - 1; ++i) {
		//      r = DistancePointLine(x, y, points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y, false);
		//      if (d == 0) d = r;
		//      else if (d < 0 && r > 0) 
		//         return false;
		//      else if (d > 0 && r < 0) 
		//         return false;
		//   }
		//   if (points[0] != points[points.Length - 1]) {
		//      r = DistancePointLine(x, y, points[points.Length - 1].X, points[points.Length - 1].Y, points[0].X, points[0].Y, false);
		//      if (d < 0 && r > 0) 
		//         return false;
		//      else if (d > 0 && r < 0) 
		//         return false;
		//   }
		//   return true;
		//}

		#endregion

		/// <summary>
		/// Determines if the point is on the line (segment).
		/// </summary>
		/// <param name="p1x">The x-coordinate of the first point that defines the line</param>
		/// <param name="p1y">The y-coordinate of the first point that defines the line</param>
		/// <param name="p2x">The x-coordinate of the second point that defines the line</param>
		/// <param name="p2y">The y-coordinate of the second point that defines the line</param>
		/// <param name="isSegment">Specifies if the line is a line or a line segment.</param>
		/// <param name="x">The x-coordinate of the point to test</param>
		/// <param name="y">The y-coordinate of the point to test</param>
		public static bool LineContainsPoint(int p1x, int p1y, int p2x, int p2y, bool isSegment, int x, int y)
		{
			return LineContainsPoint(p1x, p1y, p2x, p2y, isSegment, x, y, 0.1f);
		}


		/// <summary>
		/// Determines if the point is on the line (segment).
		/// </summary>
		/// <param name="p1">The first point that defines the line</param>
		/// <param name="p2">The second point that defines the line</param>
		/// <param name="isSegment">Specifies if the line is a line or a line segment</param>
		/// <param name="x">The x-coordinate of the point to test</param>
		/// <param name="y">The y-coordinate of the point to test</param>
		/// <param name="delta">Specifies the tolerance of the calculation</param>
		public static bool LineContainsPoint(Point p1, Point p2, bool isSegment, int x, int y, float delta)
		{
			return LineContainsPoint(p1.X, p1.Y, p2.X, p2.Y, isSegment, x, y, delta);
		}


		/// <summary>
		/// Determines if the point is on the line (segment).
		/// </summary>
		/// <param name="p1">The first point that defines the line</param>
		/// <param name="p2">The second point that defines the line</param>
		/// <param name="isSegment">Specifies if the line is a line or a line segment</param>
		/// <param name="p">The point to test</param>
		/// <param name="delta">Specifies the tolerance of the calculation</param>
		public static bool LineContainsPoint(Point p1, Point p2, bool isSegment, Point p, float delta)
		{
			return LineContainsPoint(p1.X, p1.Y, p2.X, p2.Y, isSegment, p.X, p.Y, delta);
		}


		/// <summary>
		/// Determines if the point is on the line (segment).
		/// </summary>
		/// <param name="p1">The first point that defines the line</param>
		/// <param name="p2">The second point that defines the line</param>
		/// <param name="isSegment">Specifies if the line is a line or a line segment</param>
		/// <param name="x">The x-coordinate of the point to test</param>
		/// <param name="y">The y-coordinate of the point to test</param>
		/// <param name="delta">Specifies the tolerance of the calculation</param>
		public static bool LineContainsPoint(PointF p1, PointF p2, bool isSegment, float x, float y, float delta)
		{
			return LineContainsPoint(p1.X, p1.Y, p2.X, p2.Y, isSegment, x, y, delta);
		}


		/// <summary>
		/// Determines if the point is on the line (segment).
		/// </summary>
		/// <param name="p1x">The x-coordinate of the first point that defines the line</param>
		/// <param name="p1y">The y-coordinate of the first point that defines the line</param>
		/// <param name="p2x">The x-coordinate of the second point that defines the line</param>
		/// <param name="p2y">The y-coordinate of the second point that defines the line</param>
		/// <param name="isSegment">Specifies if the line is a line or a line segment</param>
		/// <param name="x">The x-coordinate of the point to test</param>
		/// <param name="y">The y-coordinate of the point to test</param>
		/// <param name="delta">Specifies the tolerance of the calculation</param>
		public static bool LineContainsPoint(int p1x, int p1y, int p2x, int p2y, bool isSegment, int x, int y, float delta)
		{
			return Math.Abs(DistancePointLine(x, y, p1x, p1y, p2x, p2y, isSegment)) <= delta;
		}


		/// <summary>
		/// Determines if the point is on the line (segment).
		/// </summary>
		/// <param name="p1x">The x-coordinate of the first point that defines the line</param>
		/// <param name="p1y">The y-coordinate of the first point that defines the line</param>
		/// <param name="p2x">The x-coordinate of the second point that defines the line</param>
		/// <param name="p2y">The y-coordinate of the second point that defines the line</param>
		/// <param name="isSegment">Specifies if the line is a line or a line segment.</param>
		/// <param name="x">The x-coordinate of the point to test</param>
		/// <param name="y">The y-coordinate of the point to test</param>
		public static bool LineContainsPoint(float p1x, float p1y, float p2x, float p2y, bool isSegment, float x, float y)
		{
			if (p2x == p1x)
				return (p1x == x);
			// check if point is inside the bounds of the line segment
			if (isSegment) {
				if (!(Math.Min(p1x, p2x) <= x && x <= Math.Max(p1x, p2x)) ||
				    !(Math.Min(p1y, p2y) <= y && y <= Math.Max(p1y, p2y)))
					return false;
			}
			float m = (p2y - p1y)/(p2x - p1x);
			float c1 = m*p1x - p1y;
			float c = m*x - y;
			return (c1 == c);
		}


		/// <summary>
		/// Determines if the point is on the line (segment).
		/// </summary>
		/// <param name="p1x">The x-coordinate of the first point that defines the line</param>
		/// <param name="p1y">The y-coordinate of the first point that defines the line</param>
		/// <param name="p2x">The x-coordinate of the second point that defines the line</param>
		/// <param name="p2y">The y-coordinate of the second point that defines the line</param>
		/// <param name="isSegment">Specifies if the line is a line or a line segment</param>
		/// <param name="x">The x-coordinate of the point to test</param>
		/// <param name="y">The y-coordinate of the point to test</param>
		/// <param name="delta">Specifies the tolerance of the calculation</param>
		public static bool LineContainsPoint(float p1x, float p1y, float p2x, float p2y, bool isSegment, float x, float y,
		                                     float delta)
		{
			if (p2x == p1x)
				return (p1x - delta <= x && x <= p1x + delta);
			// check if point is inside the bounds of the line segment
			if (isSegment) {
				if (!(Math.Min(p1x - delta, p2x - delta) <= x && x <= Math.Max(p1x + delta, p2x + delta)) ||
				    !(Math.Min(p1y - delta, p2y - delta) <= y && y <= Math.Max(p1y + delta, p2y + delta)))
					return false;
			}
			float m = (p2y - p1y)/(p2x - p1x);
			float c1 = m*p1x - p1y;
			float c = m*x - y;

			return (c1 - delta <= c && c <= c1 + delta);
		}


		/// <summary>
		/// Returns true if rectangle rect1 contains rectangle rect2.
		/// </summary>
		public static bool RectangleContainsRectangle(Rectangle rect1, Rectangle rect2)
		{
			AssertIsValid(rect1);
			AssertIsValid(rect2);
			return rect2.Left >= rect1.Left
			       && rect2.Top >= rect1.Top
			       && rect2.Right <= rect1.Right
			       && rect2.Bottom <= rect1.Bottom;
		}


		/// <summary>
		/// Returns true if the rectanlge defined by x, y, width and height contains the given rectangle.
		/// </summary>
		public static bool RectangleContainsRectangle(int x, int y, int width, int height, Rectangle rectangle)
		{
			AssertIsValid(x, y, width, height);
			AssertIsValid(rectangle);
			return rectangle.Left >= x && rectangle.Top >= y && rectangle.Right <= x + width && rectangle.Bottom <= y + height;
		}


		/// <summary>
		/// Returns true if the given rectangle contains the rectanlge defined by x, y, width and height.
		/// </summary>
		public static bool RectangleContainsRectangle(Rectangle rectangle, int x, int y, int width, int height)
		{
			AssertIsValid(x, y, width, height);
			AssertIsValid(rectangle);
			return x >= rectangle.Left
			       && y >= rectangle.Top
			       && x + width <= rectangle.Right
			       && y + height <= rectangle.Bottom;
		}


		/// <summary>
		/// Returns true if rectangle rect1 contains rectangle rect2.
		/// </summary>
		public static bool RectangleContainsRectangle(int rect1X, int rect1Y, int rect1Width, int rect1Height, int rect2X,
		                                              int rect2Y, int rect2Width, int rect2Height)
		{
			if (rect1Width < 0)
				throw new ArgumentException(string.Format("{0} is not a valid value.", rect1Width), "rect1Width");
			if (rect1Height < 0)
				throw new ArgumentException(string.Format("{0} is not a valid value.", rect1Height), "rect1Height");
			if (rect2Width < 0)
				throw new ArgumentException(string.Format("{0} is not a valid value.", rect2Width), "rect2Width");
			if (rect2Height < 0)
				throw new ArgumentException(string.Format("{0} is not a valid value.", rect2Height), "rect2Height");
			return rect2X >= rect1X
			       && rect2Y >= rect1Y
			       && rect2X + rect2Width <= rect1X + rect1Width
			       && rect2Y + rect2Height <= rect1Y + rect1Height;
		}


		/// <summary>
		/// Determines if a rotated ellipse contains a point
		/// /// </summary>
		/// <param name="ellipseCenter">The center point of the ellipse and the rotation center</param>
		/// <param name="ellipseWidth">The width of the ellipse, equal to the doubled length of the major half axis</param>
		/// <param name="ellipseHeight">The height of the ellipse, equal to the doubled length of the minor half axis</param>
		/// <param name="ellipseAngleDeg">The rotation angle of the ellipse in degrees</param>
		/// <param name="p">The point to test</param>
		/// <returns></returns>
		public static bool EllipseContainsPoint(PointF ellipseCenter, float ellipseWidth, float ellipseHeight,
		                                        float ellipseAngleDeg, PointF p)
		{
			return EllipseContainsPoint(ellipseCenter.X, ellipseCenter.Y, ellipseWidth, ellipseHeight, ellipseAngleDeg, p.X, p.Y);
		}


		/// <summary>
		/// Determines if a rotated ellipse contains a point
		/// /// </summary>
		/// <param name="ellipseCenterX">The center point of the ellipse and the rotation center</param>
		/// <param name="ellipseCenterY">The center point of the ellipse and the rotation center</param>
		/// <param name="ellipseWidth">The width of the ellipse, equal to the doubled length of the major half axis</param>
		/// <param name="ellipseHeight">The height of the ellipse, equal to the doubled length of the minor half axis</param>
		/// <param name="ellipseAngleDeg">The rotation angle of the ellipse in degrees</param>
		/// <param name="x">The x-coordinate of the point to test</param>
		/// <param name="y">The y-coordinate of the point to test</param>
		/// <returns></returns>
		public static bool EllipseContainsPoint(float ellipseCenterX, float ellipseCenterY, float ellipseWidth,
		                                        float ellipseHeight, float ellipseAngleDeg, float x, float y)
		{
			// Standard ellipse formula:
			// (x² / a²) + (y² / b²) = 1
			// Where a = radiusX and b = radiusY
			float radiusX = ellipseWidth/2f;
			float radiusY = ellipseHeight/2f;
			// instead of rotating the ellipse, we rotate the line to the opposite side and then rotate intersection points back
			if (ellipseAngleDeg != 0) RotatePoint(ellipseCenterX, ellipseCenterY, -ellipseAngleDeg, ref x, ref y);
			// transform x/y to the origin of the coordinate system
			float x0 = x - ellipseCenterX;
			float y0 = y - ellipseCenterY;
			return ((x0*x0)/(radiusX*radiusX)) + ((y0*y0)/(radiusY*radiusY)) <= 1;
		}


		/// <summary>
		/// Determines if a rotated ellipse contains a point
		/// /// </summary>
		/// <param name="ellipseCenter">The center point of the ellipse and the rotation center</param>
		/// <param name="ellipseWidth">The width of the ellipse, equal to the doubled length of the major half axis</param>
		/// <param name="ellipseHeight">The height of the ellipse, equal to the doubled length of the minor half axis</param>
		/// <param name="ellipseAngleDeg">The rotation angle of the ellipse in degrees</param>
		/// <param name="p">The point to test</param>
		/// <returns></returns>
		public static bool EllipseContainsPoint(Point ellipseCenter, int ellipseWidth, int ellipseHeight,
		                                        float ellipseAngleDeg, Point p)
		{
			return EllipseContainsPoint(ellipseCenter.X, ellipseCenter.Y, ellipseWidth, ellipseHeight, ellipseAngleDeg, p.X, p.Y);
		}


		/// <summary>
		/// Determines if a rotated ellipse contains a point
		/// /// </summary>
		/// <param name="ellipseCenterX">The center point of the ellipse and the rotation center</param>
		/// <param name="ellipseCenterY">The center point of the ellipse and the rotation center</param>
		/// <param name="ellipseWidth">The width of the ellipse, equal to the doubled length of the major half axis</param>
		/// <param name="ellipseHeight">The height of the ellipse, equal to the doubled length of the minor half axis</param>
		/// <param name="ellipseAngleDeg">The rotation angle of the ellipse in degrees</param>
		/// <param name="x">The x-coordinate of the point to test</param>
		/// <param name="y">The y-coordinate of the point to test</param>
		/// <returns></returns>
		public static bool EllipseContainsPoint(int ellipseCenterX, int ellipseCenterY, int ellipseWidth, int ellipseHeight,
		                                        float ellipseAngleDeg, int x, int y)
		{
			// Standard ellipse formula:
			// (x² / a²) + (y² / b²) = 1
			// Where a = radiusX and b = radiusY
			float radiusX = ellipseWidth/2f;
			float radiusY = ellipseHeight/2f;
			// instead of rotating the ellipse, we rotate the line to the opposite side and then rotate intersection points back
			if (ellipseAngleDeg != 0) RotatePoint(ellipseCenterX, ellipseCenterY, -ellipseAngleDeg, ref x, ref y);
			// transform x/y to the origin of the coordinate system
			float x0 = x - ellipseCenterX;
			float y0 = y - ellipseCenterY;
			return ((x0*x0)/(radiusX*radiusX)) + ((y0*y0)/(radiusY*radiusY)) <= 1;
		}


		/// <summary>
		/// Determines if a circle represented by a center point and a radius contains a point
		/// </summary>
		/// <param name="centerX">The x-coordinate of the circle's center</param>
		/// <param name="centerY">The y-coordinate of the circle's center</param>
		/// <param name="radius">The radius of the circle</param>
		/// <param name="pointX">The x-coordinate of the point to test</param>
		/// <param name="pointY">The y-coordinate of the point to test</param>
		/// <param name="delta">The tolerance of the calculation</param>
		public static bool CircleContainsPoint(float centerX, float centerY, float radius, float pointX, float pointY,
		                                       float delta)
		{
			float pDistance = Math.Abs(DistancePointPoint(pointX, pointY, centerX, centerY));
			return pDistance <= radius + delta;
		}


		/// <summary>
		/// Determines if a circle represented by a center point and a radius contains a point
		/// </summary>
		/// <param name="centerX">The x-coordinate of the circle's center</param>
		/// <param name="centerY">The y-coordinate of the circle's center</param>
		/// <param name="radius">The radius of the circle</param>
		/// <param name="pointX">The x-coordinate of the point to test</param>
		/// <param name="pointY">The y-coordinate of the point to test</param>
		/// <param name="delta">The tolerance of the calculation</param>
		public static bool CircleContainsPoint(int centerX, int centerY, float radius, int pointX, int pointY, float delta)
		{
			float pDistance = Math.Abs(DistancePointPoint(pointX, pointY, centerX, centerY));
			return pDistance <= radius + delta;
		}


		/// <summary>
		/// Determines if a circle outline represented by a center point, a radius and the width of the outline contains a point
		/// </summary>
		/// <param name="centerX">The x-coordinate of the circle's center</param>
		/// <param name="centerY">The y-coordinate of the circle's center</param>
		/// <param name="radius">The radius of the circle</param>
		/// <param name="pX">The x-coordinate of the point to test</param>
		/// <param name="pY">The y-coordinate of the point to test</param>
		/// <param name="delta">The tolerance of the calculation</param>
		public static bool CircleOutlineContainsPoint(int centerX, int centerY, float radius, int pX, int pY, float delta)
		{
			float pDistance = Math.Abs(DistancePointPoint(pX, pY, centerX, centerY));
			if ((pDistance <= radius + delta) && (pDistance >= radius - delta))
				return true;
			else
				return false;
		}


		/// <summary>
		/// Returns true if the arc defined by arcCenter and the three points startPoint, endPoint and radiusPoint contains the given point.
		/// </summary>
		public static bool ArcContainsPoint(Point arcCenter, Point startPoint, Point endPoint, Point radiusPoint, Point point,
		                                    float delta)
		{
			return ArcContainsPoint(arcCenter.X, arcCenter.Y, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, delta, point.X,
			                        point.Y);
		}


		/// <summary>
		/// Determines if an arc, defined by three points, contains a point
		/// </summary>
		/// <param name="startPoint">The point defining the beginning of the arc</param>
		/// <param name="radiusPoint">The point defining the radius of the arc</param>
		/// <param name="endPoint">The point defining end of the arc</param>
		/// <param name="point">The point to test</param>
		/// <param name="delta">The tolerance of the calculation</param>
		public static bool ArcContainsPoint(PointF startPoint, PointF radiusPoint, PointF endPoint, PointF point, float delta)
		{
			return ArcContainsPoint(startPoint.X, startPoint.Y, radiusPoint.X, radiusPoint.Y, endPoint.X, endPoint.Y, delta,
			                        point.X, point.Y);
		}


		/// <summary>
		/// Determines if an arc, defined by three points, contains a point
		/// </summary>
		/// <param name="startPointX">The x-coordinate of the point defining the beginning of the arc</param>
		/// <param name="startPointY">The y-coordinate of the point defining  the beginning of the arc</param>
		/// <param name="radiusPointX">The x-coordinate of the point defining the radius of the arc</param>
		/// <param name="radiusPointY">The y-coordinate of the point defining the radius of the arc</param>
		/// <param name="endPointX">The x-coordinate of the point defining end of the arc</param>
		/// <param name="endPointY">The x-coordinate of the point defining end of the arc</param>
		/// <param name="delta">The tolerance of the calculation</param>
		/// <param name="pointX">The x-coordinate of the point to test</param>
		/// <param name="pointY">The y-coordinate of the point to test</param>
		public static bool ArcContainsPoint(float startPointX, float startPointY, float radiusPointX, float radiusPointY,
		                                    float endPointX, float endPointY, float delta, float pointX, float pointY)
		{
			float radius;
			PointF center;
			CalcCircumCircle(startPointX, startPointY, radiusPointX, radiusPointY, endPointX, endPointY, out center, out radius);
			return ArcContainsPoint(startPointX, startPointY, radiusPointX, radiusPointY, endPointX, endPointY, center.X,
			                        center.Y, radius, pointX, pointY, delta);
		}


		/// <summary>
		/// Determines if an arc contains a point
		/// </summary>
		/// <param name="arcCenterX">The x-coordinate of the arc's center point</param>
		/// <param name="arcCenterY">The y-coordinate of the arc's center point</param>
		/// <param name="startPointX">The x-coordinate of the point defining the beginning of the arc</param>
		/// <param name="startPointY">The y-coordinate of the point defining  the beginning of the arc</param>
		/// <param name="endPointX">The x-coordinate of the point defining end of the arc</param>
		/// <param name="endPointY">The x-coordinate of the point defining end of the arc</param>
		/// <param name="radiusPointX">The x-coordinate of the point defining the radius of the arc</param>
		/// <param name="radiusPointY">The y-coordinate of the point defining the radius of the arc</param>
		/// <param name="pointX">The x-coordinate of the point to test</param>
		/// <param name="pointY">The y-coordinate of the point to test</param>
		/// <param name="delta">The tolerance of the calculation</param>
		public static bool ArcContainsPoint(int arcCenterX, int arcCenterY, int startPointX, int startPointY, int endPointX,
		                                    int endPointY, int radiusPointX, int radiusPointY, int pointX, int pointY,
		                                    float delta)
		{
			//if the point is on the arc's circle...
			float distance = Math.Abs(DistancePointPoint(pointX, pointY, arcCenterX, arcCenterY));
			float radius = Math.Abs(DistancePointPoint(startPointX, startPointY, arcCenterX, arcCenterY));
			if (radius - delta <= distance && distance <= radius + delta) {
				// ... and if Point and RadiusPoint are on the same side -> arc contains point
				float distancePt = DistancePointLine(pointX, pointY, startPointX, startPointY, endPointX, endPointY, false);
				float distanceRadPt = DistancePointLine(radiusPointX, radiusPointY, startPointX, startPointY, endPointX, endPointY,
				                                        false);
				if (distancePt < 0 && distanceRadPt < 0 || distancePt >= 0 && distanceRadPt >= 0)
					return true;
			}
			return false;
		}


		/// <summary>
		/// Determines if an arc contains a point. This function takes all parameters of the arc so it does not have to perform redundant calculations.
		/// </summary>
		/// <param name="startPointX">The x-coordinate of the point defining the beginning of the arc</param>
		/// <param name="startPointY">The y-coordinate of the point defining  the beginning of the arc</param>
		/// <param name="radiusPointX">The x-coordinate of the point defining the radius of the arc</param>
		/// <param name="radiusPointY">The y-coordinate of the point defining the radius of the arc</param>
		/// <param name="endPointX">The x-coordinate of the point defining end of the arc</param>
		/// <param name="endPointY">The x-coordinate of the point defining end of the arc</param>
		/// <param name="arcCenterX">The x-coordinate of the arc's center point</param>
		/// <param name="arcCenterY">The y-coordinate of the arc's center point</param>
		/// <param name="arcRadius">The radius of the arc</param>
		/// <param name="pointX">The x-coordinate of the point to test</param>
		/// <param name="pointY">The y-coordinate of the point to test</param>
		/// <param name="delta">The tolerance of the calculation</param>
		public static bool ArcContainsPoint(float startPointX, float startPointY, float radiusPointX, float radiusPointY,
		                                    float endPointX, float endPointY, float arcCenterX, float arcCenterY,
		                                    float arcRadius, float pointX, float pointY, float delta)
		{
			// check if the point is on the arc's circle
			float distance = Math.Abs(DistancePointPoint(pointX, pointY, arcCenterX, arcCenterY));
			if (arcRadius - delta <= distance && distance <= arcRadius + delta) {
				float twoPi = (float) (Math.PI + Math.PI);

				// First, sort the angles
				float startPtAngle = Angle(arcCenterX, arcCenterY, startPointX, startPointY);
				if (startPtAngle < 0) startPtAngle += twoPi;
				float radiusPtAngle = Angle(arcCenterX, arcCenterY, radiusPointX, radiusPointY);
				if (radiusPtAngle < 0) radiusPtAngle += twoPi;
				float endPtAngle = Angle(arcCenterX, arcCenterY, endPointX, endPointY);
				if (endPtAngle < 0) endPtAngle += twoPi;
				float pointAngle = Angle(arcCenterX, arcCenterY, pointX, pointY);
				if (pointAngle < 0) pointAngle += twoPi;

				// Then compare the point's angle with the sorted angles of the arc
				if (startPtAngle <= radiusPtAngle && radiusPtAngle <= endPtAngle) {
					if (startPtAngle <= pointAngle && pointAngle <= endPtAngle)
						return true;
					else return false;
				}
				else if (endPtAngle <= radiusPtAngle && radiusPtAngle <= startPtAngle) {
					if (endPtAngle <= pointAngle && pointAngle <= startPtAngle)
						return true;
					else return false;
				}
				else if (startPtAngle <= radiusPtAngle && endPtAngle <= radiusPtAngle) {
					if (startPtAngle < endPtAngle) {
						if (startPtAngle < pointAngle && pointAngle < endPtAngle)
							return false;
						else return true;
					}
					else if (endPtAngle < startPtAngle) {
						if (endPtAngle < pointAngle && pointAngle < startPtAngle)
							return false;
						else return true;
					}
				}
				else if (radiusPtAngle <= startPtAngle && radiusPtAngle <= endPtAngle) {
					if (startPtAngle < endPtAngle) {
						if (startPtAngle < pointAngle && pointAngle < endPtAngle)
							return false;
						else return true;
					}
					else if (endPtAngle < startPtAngle) {
						if (endPtAngle < pointAngle && pointAngle < startPtAngle)
							return false;
						else return true;
					}
				}
			}
			return false;
		}

		#endregion

		#region Intersection test functions

		/// <ToBeCompleted></ToBeCompleted>
		public static bool PolygonIntersectsWithRectangle(Point[] points, Rectangle rectangle)
		{
			return PolygonIntersectsWithRectangle(points, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool PolygonIntersectsWithRectangle(Point[] points, int rectangleLeft, int rectangleTop,
		                                                  int rectangleRight, int rectangleBottom)
		{
			int left = int.MaxValue;
			int top = int.MaxValue;
			int right = int.MinValue;
			int bottom = int.MinValue;

			int rectangleWidth = rectangleRight - rectangleLeft;
			int rectangleHeight = rectangleBottom - rectangleTop;
			int maxIdx = points.Length - 1;
			for (int i = 0; i < maxIdx; ++i) {
				if (points[i].X < left) left = points[i].X;
				if (points[i].X > right) right = points[i].X;
				if (points[i].Y < top) top = points[i].Y;
				if (points[i].Y > bottom) bottom = points[i].Y;

				// The polygon intersects the Rectangle if the rectangle contains one point of the polygon...
				if (RectangleContainsPoint(rectangleLeft, rectangleTop, rectangleWidth, rectangleHeight, points[i].X, points[i].Y))
					return true;
				if (RectangleContainsPoint(rectangleLeft, rectangleTop, rectangleWidth, rectangleHeight, points[i + 1].X,
				                           points[i + 1].Y))
					return true;

				// ... or if one side of the polygon intersects one side of the rectangle ...
				if (RectangleIntersectsWithLine(rectangleLeft, rectangleTop, rectangleRight, rectangleBottom, points[i].X,
				                                points[i].Y, points[i + 1].X, points[i + 1].Y, true))
					return true;
			}
			if (RectangleIntersectsWithLine(rectangleLeft, rectangleTop, rectangleRight, rectangleBottom, points[maxIdx].X,
			                                points[maxIdx].Y, points[0].X, points[0].Y, true))
				return true;

			// ... or if the rectangle is inside of the polygon...
			//if (left <= rectangle.left && top <= rectangle.top && right >= rectangle.right && bottom >= rectangle.bottom)
			//   return true;
			if (ConvexPolygonContainsPoint(points, rectangleLeft, rectangleTop) &&
			    ConvexPolygonContainsPoint(points, rectangleRight, rectangleTop) &&
			    ConvexPolygonContainsPoint(points, rectangleRight, rectangleBottom) &&
			    ConvexPolygonContainsPoint(points, rectangleLeft, rectangleBottom))
				return true;
			return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool PolygonIntersectsWithRectangle(PointF[] pointFs, Rectangle rectangle)
		{
			return PolygonIntersectsWithRectangle(pointFs, (RectangleF) rectangle);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool PolygonIntersectsWithRectangle(PointF[] points, RectangleF rectangle)
		{
			if (points == null) throw new ArgumentNullException("points");
			if (points.Length <= 0) return false;
			int maxIdx = points.Length - 1;
			for (int i = 0; i < maxIdx; ++i) {
				// The polygon intersects the Rectangle if the rectangle contains one point of the polygon...
				if (RectangleContainsPoint(rectangle, points[i].X, points[i].Y))
					return true;
				if (RectangleContainsPoint(rectangle, points[i + 1].X, points[i + 1].Y))
					return true;
				// ... or if one side of the polygon intersects one side of the rectangle ...
				if (RectangleIntersectsWithLine(rectangle, points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y, true))
					return true;
			}
			if (RectangleIntersectsWithLine(rectangle, points[maxIdx].X, points[maxIdx].Y, points[0].X, points[0].Y, true))
				return true;

			// ... or if the rectangle is inside of the polygon...
			if (ConvexPolygonContainsPoint(points, rectangle.Left, rectangle.Top) &&
			    ConvexPolygonContainsPoint(points, rectangle.Right, rectangle.Top) &&
			    ConvexPolygonContainsPoint(points, rectangle.Right, rectangle.Bottom) &&
			    ConvexPolygonContainsPoint(points, rectangle.Left, rectangle.Bottom))
				return true;
			return false;
		}


		/// <summary>
		/// Bestimmt, ob die beiden Geraden sich schneiden.
		/// </summary>
		public static bool LineIntersectsWithLine(Point line1Start, Point line1End, Point line2Start, Point line2End)
		{
			return LineIntersectsWithLine(line1Start.X, line1Start.Y, line1End.X, line1End.Y, line2Start.X, line2Start.Y,
			                              line2End.X, line2End.Y);
		}


		/// <summary>
		/// Bestimmt, ob die beiden Geraden sich schneiden.
		/// </summary>
		public static bool LineIntersectsWithLine(Point line1Start, Point line1End, int line2StartX, int line2StartY,
		                                          int line2EndX, int line2EndY)
		{
			return LineIntersectsWithLine(line1Start.X, line1Start.Y, line1End.X, line1End.Y, line2StartX, line2StartY, line2EndX,
			                              line2EndY);
		}


		/// <summary>
		/// Bestimmt, ob die beiden Geraden sich schneiden.
		/// </summary>
		public static bool LineIntersectsWithLine(int line1StartX, int line1StartY, int line1EndX, int line1EndY,
		                                          int line2StartX, int line2StartY, int line2EndX, int line2EndY)
		{
			// Richtungsvectoren der beiden Geraden berechnen			
			Point line1Vec = Point.Empty; // Richtungsvektor Linie1
			line1Vec.X = line1EndX - line1StartX;
			line1Vec.Y = line1EndY - line1StartY;
			Point line2Vec = Point.Empty; // Richtungsvector Linie2
			line2Vec.X = line2EndX - line2StartX;
			line2Vec.Y = line2EndY - line2StartY;

			// Determinante det berechnen
			int det = line1Vec.Y*line2Vec.X - line1Vec.X*line2Vec.Y;

			// Wenn det == 0 ist, sind die Linien parallel. 
			if (det == 0) {
				// wenn die Linien gegensinnig laufen, also die Vektoren die Beträge [10|10] und [-10|-10] haben, schneiden sie sich zwangsläufig
				if (line1Vec.X == -line2Vec.X && line1Vec.Y == -line2Vec.Y)
					return true;
				else
					return false;
			}

			// Determinante det's berechnen 
			int ds = (line2StartY - line1StartY)*line2Vec.X - (line2StartX - line1StartX)*line2Vec.Y;

			// detdet / det berechnen und prüfen, ob s in den Grenzen liegt 
			float s = ds/(float) det;
			if (s < 0.0f || s > 1.0f)
				return false;

			// dt und db berechnen.
			float dt = (line2StartY - line1StartY)*line1Vec.X - (line2StartX - line1StartX)*line1Vec.Y;
			float t = dt/det;
			if (t < 0.0f || t > 1.0f)
				return false;

			return true;
		}


		/// <summary>
		/// Bestimmt, ob die beiden Geraden sich schneiden.
		/// </summary>
		public static bool LineIntersectsWithLine(PointF line1Start, PointF line1End, PointF line2Start, PointF line2End)
		{
			return LineIntersectsWithLine(line1Start.X, line1Start.Y, line1End.X, line1End.Y, line2Start.X, line2Start.Y,
			                              line2End.X, line2End.Y);
		}


		/// <summary>
		/// Bestimmt, ob die beiden Geraden sich schneiden.
		/// </summary>
		public static bool LineIntersectsWithLine(float line1StartX, float line1StartY, float line1EndX, float line1EndY,
		                                          float line2StartX, float line2StartY, float line2EndX, float line2EndY)
		{
			// Richtungsvectoren der beiden Geraden berechnen			
			PointF line1Vec = Point.Empty; // Richtungsvektor Linie1
			line1Vec.X = line1EndX - line1StartX;
			line1Vec.Y = line1EndY - line1StartY;
			PointF line2Vec = Point.Empty; // Richtungsvector Linie2
			line2Vec.X = line2EndX - line2StartX;
			line2Vec.Y = line2EndY - line2StartY;

			// Determinante det berechnen
			float det = line1Vec.Y*line2Vec.X - line1Vec.X*line2Vec.Y;

			// Wenn det == 0 ist, sind die Linien parallel. 
			if (det == 0) {
				// wenn die Linien gegensinnig laufen, also die Vektoren die Beträge [10|10] und [-10|-10] haben, schneiden sie sich zwangsläufig
				if (line1Vec.X == -line2Vec.X && line1Vec.Y == -line2Vec.Y)
					return true;
				else
					return false;
			}

			// Determinante det's berechnen 
			float detdet = (line2StartY - line1StartY)*line2Vec.X - (line2StartX - line1StartX)*line2Vec.Y;

			// detdet / det berechnen und prüfen, ob das Ergebnis in den Grenzen liegt
			float s = detdet/(float) det;
			if (s < 0.0f || s > 1.0f)
				return false;

			// dt und db berechnen.
			float dt = (line2StartY - line1StartY)*line1Vec.X - (line2StartX - line1StartX)*line1Vec.Y;
			float t = dt/det;
			if (t < 0.0f || t > 1.0f)
				return false;

			return true;
		}


		/// <summary>
		/// Bestimmt, ob die Gerade sich mit der Strecke schneidet.
		/// </summary>
		public static bool LineIntersectsWithLineSegment(PointF line1Start, PointF line1End, float line2StartX,
		                                                 float line2StartY, float line2EndX, float line2EndY)
		{
			return LineIntersectsWithLineSegment(line1Start.X, line1Start.Y, line1End.X, line1End.Y, line2StartX, line2StartY,
			                                     line2EndX, line2EndY);
		}


		/// <summary>
		/// Bestimmt, ob die Gerade sich mit der Strecke schneidet.
		/// </summary>
		public static bool LineIntersectsWithLineSegment(float line1StartX, float line1StartY, float line1EndX,
		                                                 float line1EndY, float line2StartX, float line2StartY,
		                                                 float line2EndX, float line2EndY)
		{
			return
				IsValid(IntersectLineWithLineSegment(line1StartX, line1StartY, line1EndX, line1EndY, line2StartX, line2StartY,
				                                     line2EndX, line2EndY));
		}


		/// <summary>
		/// Bestimmt, ob die Gerade sich mit der Strecke schneidet.
		/// </summary>
		public static bool LineIntersectsWithLineSegment(Point line1Start, Point line1End, Point line2Start, Point line2End)
		{
			return LineIntersectsWithLineSegment(line1Start.X, line1Start.Y, line1End.X, line1End.Y, line2Start.X, line2Start.Y,
			                                     line2End.X, line2End.Y);
		}


		/// <summary>
		/// Bestimmt, ob die Gerade sich mit der Strecke schneidet.
		/// </summary>
		public static bool LineIntersectsWithLineSegment(Point line1Start, Point line1End, int line2StartX, int line2StartY,
		                                                 int line2EndX, int line2EndY)
		{
			return LineIntersectsWithLineSegment(line1Start.X, line1Start.Y, line1End.X, line1End.Y, line2StartX, line2StartY,
			                                     line2EndX, line2EndY);
		}


		/// <summary>
		/// Bestimmt, ob die Gerade sich mit der Strecke schneidet.
		/// </summary>
		public static bool LineIntersectsWithLineSegment(int line1StartX, int line1StartY, int line1EndX, int line1EndY,
		                                                 int line2StartX, int line2StartY, int line2EndX, int line2EndY)
		{
			return
				IsValid(IntersectLineWithLineSegment(line1StartX, line1StartY, line1EndX, line1EndY, line2StartX, line2StartY,
				                                     line2EndX, line2EndY));
		}


		/// <summary>
		/// Bestimmt, ob die beiden Strecken sich schneiden.
		/// </summary>
		public static bool LineSegmentIntersectsWithLineSegment(int line1StartX, int line1StartY, int line1EndX, int line1EndY,
		                                                        int line2StartX, int line2StartY, int line2EndX, int line2EndY)
		{
			return
				IsValid(IntersectLineSegments(line1StartX, line1StartY, line1EndX, line1EndY, line2StartX, line2StartY, line2EndX,
				                              line2EndY));
		}


		/// <summary>
		/// Bestimmt, ob die beiden Strecken sich schneiden.
		/// </summary>
		public static bool LineSegmentIntersectsWithLineSegment(long line1StartX, long line1StartY, long line1EndX,
		                                                        long line1EndY, long line2StartX, long line2StartY,
		                                                        long line2EndX, long line2EndY)
		{
			return
				IsValid(IntersectLineSegments(line1StartX, line1StartY, line1EndX, line1EndY, line2StartX, line2StartY, line2EndX,
				                              line2EndY));
		}


		/// <summary>
		/// Bestimmt, ob die beiden Strecken sich schneiden.
		/// </summary>
		public static bool LineSegmentIntersectsWithLineSegment(float line1StartX, float line1StartY, float line1EndX,
		                                                        float line1EndY, float line2StartX, float line2StartY,
		                                                        float line2EndX, float line2EndY)
		{
			return
				IsValid(IntersectLineSegments(line1StartX, line1StartY, line1EndX, line1EndY, line2StartX, line2StartY, line2EndX,
				                              line2EndY));
		}


		/// <summary>
		/// Checks whether the rectangle area intersects with the line. 
		/// If isSegmnt is true, the line is treated as line segment.
		/// </summary>
		public static bool RectangleIntersectsWithLine(Rectangle rectangle, Point pt1, Point pt2, bool isSegment)
		{
			return RectangleIntersectsWithLine(rectangle, pt1.X, pt1.Y, pt2.X, pt2.Y, isSegment);
		}


		/// <summary>
		/// Checks whether the rectangle intersects with the line. 
		/// If isSegmnt is true, the line is treated as line segment.
		/// </summary>
		public static bool RectangleIntersectsWithLine(Rectangle rectangle, int pt1X, int pt1Y, int pt2X, int pt2Y,
		                                               bool isSegment)
		{
			// Strecke schneidet das Rechteck, wenn das Recteck einen der Linien-Endpunkte enthält 
			// oder wenn sie eine der Seiten schneidet
			if (RectangleContainsPoint(rectangle, pt1X, pt1Y) || RectangleContainsPoint(rectangle, pt2X, pt2Y))
				return true;
			else {
				// sind beide Punkte der Strecke auf einer Seite des Rechtecks kann es keinen Schnittpunkt geben
				if (isSegment && ((pt1X < rectangle.Left && pt2X < rectangle.Left)
				                  || (pt1X > rectangle.Right && pt2X > rectangle.Right)
				                  || (pt1Y < rectangle.Top && pt2Y < rectangle.Top)
				                  || (pt1Y > rectangle.Bottom && pt2Y > rectangle.Bottom)))
					return false;
				else {
					if (isSegment)
						return
							(LineSegmentIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Top, rectangle.Right,
							                                      rectangle.Top) ||
							 LineSegmentIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Right, rectangle.Top, rectangle.Right,
							                                      rectangle.Bottom) ||
							 LineSegmentIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Bottom, rectangle.Right,
							                                      rectangle.Bottom) ||
							 LineSegmentIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Top, rectangle.Left,
							                                      rectangle.Bottom));
					else
						return
							(LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Top, rectangle.Right,
							                               rectangle.Top) ||
							 LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Right, rectangle.Top, rectangle.Right,
							                               rectangle.Bottom) ||
							 LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Bottom, rectangle.Right,
							                               rectangle.Bottom) ||
							 LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Top, rectangle.Left,
							                               rectangle.Bottom));
				}
			}
		}


		/// <summary>
		/// Checks whether the rectangle intersects with the line. 
		/// If isSegmnt is true, the line is treated as line segment.
		/// </summary>
		public static bool RectangleIntersectsWithLine(RectangleF rectangle, PointF p1, PointF p2, bool isSegment)
		{
			return RectangleIntersectsWithLine(rectangle, p1.X, p1.Y, p2.X, p2.Y, isSegment);
		}


		/// <summary>
		/// Checks whether the rectangle intersects with the line. 
		/// If isSegmnt is true, the line is treated as line segment.
		/// </summary>
		public static bool RectangleIntersectsWithLine(RectangleF rectangle, float pt1X, float pt1Y, float pt2X, float pt2Y,
		                                               bool isSegment)
		{
			// Strecke schneidet das Rechteck, wenn das Recteck einen der Linien-Endpunkte enthält 
			// oder wenn sie eine der Seiten schneidet
			if (RectangleContainsPoint(rectangle, pt1X, pt1Y) || RectangleContainsPoint(rectangle, pt2X, pt2Y))
				return true;
			else {
				// sind beide Punkte der Strecke auf einer Seite des Rectecks kann es keinen Schnittpunkt geben
				if (isSegment && ((pt1X < rectangle.Left && pt2X < rectangle.Left)
				                  || (pt1X > rectangle.Right && pt2X > rectangle.Right)
				                  || (pt1Y < rectangle.Top && pt2Y < rectangle.Top)
				                  || (pt1Y > rectangle.Bottom && pt2Y > rectangle.Bottom)))
					return false;
				else {
					if (isSegment)
						return
							(LineSegmentIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Top, rectangle.Right,
							                                      rectangle.Top) ||
							 LineSegmentIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Right, rectangle.Top, rectangle.Right,
							                                      rectangle.Bottom) ||
							 LineSegmentIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Bottom, rectangle.Right,
							                                      rectangle.Bottom) ||
							 LineSegmentIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Top, rectangle.Left,
							                                      rectangle.Bottom));
					else
						return
							(LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Top, rectangle.Right,
							                               rectangle.Top) ||
							 LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Right, rectangle.Top, rectangle.Right,
							                               rectangle.Bottom) ||
							 LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Bottom, rectangle.Right,
							                               rectangle.Bottom) ||
							 LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, rectangle.Left, rectangle.Top, rectangle.Left,
							                               rectangle.Bottom));
				}
			}
		}


		/// <summary>
		/// Checks whether the rectangle intersects with the line. 
		/// If isSegmnt is true, the line is treated as line segment.
		/// </summary>
		public static bool RectangleIntersectsWithLine(int left, int top, int right, int bottom, int pt1X, int pt1Y, int pt2X,
		                                               int pt2Y, bool isSegment)
		{
			return RectangleIntersectsWithLine(Rectangle.FromLTRB(left, top, right, bottom), pt1X, pt1Y, pt2X, pt2Y, isSegment);
		}


		/// <summary>
		/// Checks if there is an intersection between the given rectangle (rotated around its center point) and the given line.
		/// </summary>
		/// <param name="rectangleCenterX">X coordinate of the rectangle's center</param>
		/// <param name="rectangleCenterY">Y coordinate of the rectangle's center</param>
		/// <param name="rectangleWidth">Width of the rectangle</param>
		/// <param name="rectangleHeight">Height of the rectangle</param>
		/// <param name="rectangleAngleDeg">Rotation of the rectangle</param>
		/// <param name="x1">X coordinate of the line's first point</param>
		/// <param name="y1">Y coordinate of the line's first point</param>
		/// <param name="x2">X coordinate of the line's second point</param>
		/// <param name="y2">Y coordinate of the line's second point</param>
		/// <param name="isSegment">Specifies whether the line is a line segment</param>
		/// <returns></returns>
		public static bool RectangleIntersectsWithLine(int rectangleCenterX, int rectangleCenterY, int rectangleWidth,
		                                               int rectangleHeight, float rectangleAngleDeg, int x1, int y1, int x2,
		                                               int y2, bool isSegment)
		{
			// Instead of rotating the rectangle, we translate both rectangle and line to the origin of the coordinate 
			// system and rotate the line's points in the opposite direction:
			//
			// Calc unrotated but translated rectangle
			RectangleF rect = Rectangle.Empty;
			rect.X = -rectangleWidth/2f;
			rect.Y = -rectangleHeight/2f;
			rect.Width = rectangleWidth;
			rect.Height = rectangleHeight;
			// Calc rotated and translated first point of the line
			Point p1 = Point.Empty;
			p1.Offset(x1 - rectangleCenterX, y1 - rectangleCenterY);
			p1 = RotatePoint(Point.Empty, -rectangleAngleDeg, p1);
			// Calc rotated and translated second point of the line
			Point p2 = Point.Empty;
			p2.Offset(x2 - rectangleCenterX, y2 - rectangleCenterY);
			p2 = RotatePoint(Point.Empty, -rectangleAngleDeg, p2);
			// check for intersection
			return RectangleIntersectsWithLine(rect, p1, p2, isSegment);
		}


		/// <summary>
		/// Checks if there is an intersection between the given rectangle (rotated around its center point) and the given line.
		/// </summary>
		/// <param name="rectangleCenterX">X coordinate of the rectangle's center</param>
		/// <param name="rectangleCenterY">Y coordinate of the rectangle's center</param>
		/// <param name="rectangleWidth">Width of the rectangle</param>
		/// <param name="rectangleHeight">Height of the rectangle</param>
		/// <param name="rectangleAngleDeg">Rotation of the rectangle</param>
		/// <param name="x1">X coordinate of the line's first point</param>
		/// <param name="y1">Y coordinate of the line's first point</param>
		/// <param name="x2">X coordinate of the line's second point</param>
		/// <param name="y2">Y coordinate of the line's second point</param>
		/// <param name="isSegment">Specifies whether the line is a line segment</param>
		/// <returns></returns>
		public static bool RectangleIntersectsWithLine(float rectangleCenterX, float rectangleCenterY, float rectangleWidth,
		                                               float rectangleHeight, float rectangleAngleDeg, float x1, float y1,
		                                               float x2, float y2, bool isSegment)
		{
			// Instead of rotating the rectangle, we translate both rectangle and line to the origin of the coordinate 
			// system and rotate the line's points in the opposite direction:
			//
			// Calc unrotated but translated rectangle
			RectangleF rect = Rectangle.Empty;
			rect.X = -rectangleWidth/2f;
			rect.Y = -rectangleHeight/2f;
			rect.Width = rectangleWidth;
			rect.Height = rectangleHeight;
			// Calc rotated and translated first point of the line
			PointF p1 = Point.Empty;
			p1.X = x1 - rectangleCenterX;
			p1.Y = y1 - rectangleCenterY;
			p1 = RotatePoint(PointF.Empty, -rectangleAngleDeg, p1);
			// Calc rotated and translated second point of the line
			PointF p2 = Point.Empty;
			p2.X = x2 - rectangleCenterX;
			p2.Y = y2 - rectangleCenterY;
			p2 = RotatePoint(PointF.Empty, -rectangleAngleDeg, p2);
			// check for intersection
			return RectangleIntersectsWithLine(rect, p1, p2, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool CircleIntersectsWithLine(PointF center, float radius, PointF p1, PointF p2, bool isSegment)
		{
			return Math.Abs(DistancePointLine(center.X, center.Y, p1.X, p1.Y, p2.X, p2.Y, isSegment)) <= radius;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool CircleIntersectsWithLine(float centerX, float centerY, float radius, float x1, float y1, float x2,
		                                            float y2, bool isSegment)
		{
			return Math.Abs(DistancePointLine(centerX, centerY, x1, y1, x2, y2, isSegment)) <= radius;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool ArcIntersectsWithLine(float startPtX, float startPtY, float radiusPtX, float radiusPtY,
		                                         float endPtX, float endPtY, float x1, float y1, float x2, float y2,
		                                         bool isSegment)
		{
			foreach (
				PointF p in IntersectArcLine(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, x1, y1, x2, y2, isSegment))
				return true;
			return false;
		}


		/// <summary>
		/// Checks if an arc defined, by three points, intersects with a rectangle.
		/// </summary>
		/// <param name="startPt">The Arc's frist point</param>
		/// <param name="radiusPt">The arc's second point</param>
		/// <param name="endPt">The Arc's third point</param>
		/// <param name="rect">Rectangle to be checked for intersection</param>
		public static bool ArcIntersectsWithRectangle(PointF startPt, PointF radiusPt, PointF endPt, Rectangle rect)
		{
			return ArcIntersectsWithRectangle(startPt.X, startPt.Y, radiusPt.X, radiusPt.Y, endPt.X, endPt.Y, rect);
		}


		/// <summary>
		/// Checks if an arc defined, by three points, intersects with a (rotated) rectangle.
		/// </summary>
		/// <param name="startPtX">X Coordinate of the arc's first point</param>
		/// <param name="startPtY">Y Coordinate of the arc's first point</param>
		/// <param name="radiusPtX">X Coordinate of the arc's second point</param>
		/// <param name="radiusPtY">Y Coordinate of the arc's second point</param>
		/// <param name="endPtX">X Coordinate of the arc's third point</param>
		/// <param name="endPtY">Y Coordinate of the arc's third point</param>
		/// <param name="rect">Rectangle to be checked for intersection</param>
		/// <param name="angleDeg">Rotation angle of the rectangle in degrees</param>
		public static bool ArcIntersectsWithRectangle(float startPtX, float startPtY, float radiusPtX, float radiusPtY,
		                                              float endPtX, float endPtY, Rectangle rect, float angleDeg)
		{
			// rotate the points defining the arc in the opposite direction, then do the intersection test
			PointF[] pts = new PointF[3];
			pts[0].X = startPtX;
			pts[0].Y = startPtY;
			pts[1].X = radiusPtX;
			pts[1].Y = radiusPtY;
			pts[2].X = endPtX;
			pts[2].Y = endPtY;
			PointF rotationCenter = PointF.Empty;
			rotationCenter.X = rect.X + (rect.Width/2f);
			rotationCenter.Y = rect.Y + (rect.Height/2f);
			matrix.Reset();
			matrix.RotateAt(DegreesToRadians(-angleDeg), rotationCenter);
			matrix.TransformPoints(pts);
			// perform intersection test
			return ArcIntersectsWithRectangle(pts[0].X, pts[0].Y, pts[1].X, pts[1].Y, pts[2].X, pts[2].Y, rect);
		}


		/// <summary>
		/// Checks if an arc defined, by three points, intersects with a rectangle.
		/// </summary>
		/// <param name="startPtX">X Coordinate of the arc's first point</param>
		/// <param name="startPtY">Y Coordinate of the arc's first point</param>
		/// <param name="radiusPtX">X Coordinate of the arc's second point</param>
		/// <param name="radiusPtY">Y Coordinate of the arc's second point</param>
		/// <param name="endPtX">X Coordinate of the arc's third point</param>
		/// <param name="endPtY">Y Coordinate of the arc's third point</param>
		/// <param name="rect">Rectangle to be checked for intersection</param>
		public static bool ArcIntersectsWithRectangle(float startPtX, float startPtY, float radiusPtX, float radiusPtY,
		                                              float endPtX, float endPtY, Rectangle rect)
		{
			float radius;
			PointF center;
			CalcCircumCircle(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, out center, out radius);
			return ArcIntersectsWithRectangle(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, center.X, center.Y,
			                                  radius, rect);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool ArcIntersectsWithRectangle(float startPtX, float startPtY, float radiusPtX, float radiusPtY,
		                                              float endPtX, float endPtY, float centerX, float centerY, float radius,
		                                              Rectangle rect)
		{
			float left = rect.Left;
			float top = rect.Top;
			float right = rect.Right;
			float bottom = rect.Bottom;
			// Check if the rectangle contains any of the arc's points...
			if (left <= startPtX && startPtX <= right && top <= startPtY && startPtY <= bottom)
				return true;
			if (left <= radiusPtX && radiusPtX <= right && top <= radiusPtY && radiusPtY <= bottom)
				return true;
			if (left <= endPtX && endPtX <= right && top <= endPtY && endPtY <= bottom)
				return true;
			// check the sides of the rectangle if one one of then intersects with the arc
			PointF p;
			p = IntersectCircleWithLine(centerX, centerY, radius, left, top, right, top, true);
			if (IsValid(p) &&
			    ArcContainsPoint(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, centerX, centerY, radius, p.X, p.Y,
			                     0.01f))
				return true;
			p = IntersectCircleWithLine(centerX, centerY, radius, right, top, right, bottom, true);
			if (IsValid(p) &&
			    ArcContainsPoint(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, centerX, centerY, radius, p.X, p.Y,
			                     0.01f))
				return true;
			p = IntersectCircleWithLine(centerX, centerY, radius, right, bottom, left, bottom, true);
			if (IsValid(p) &&
			    ArcContainsPoint(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, centerX, centerY, radius, p.X, p.Y,
			                     0.01f))
				return true;
			p = IntersectCircleWithLine(centerX, centerY, radius, left, bottom, left, top, true);
			if (IsValid(p) &&
			    ArcContainsPoint(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, centerX, centerY, radius, p.X, p.Y,
			                     0.01f))
				return true;
			return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool CircleIntersectsWithCircle(PointF center1, float radius1, PointF center2, float radius2)
		{
			float distance = DistancePointPoint(center1, center2);
			return (distance <= radius1 || distance <= radius2);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool CircleIntersectsWithRectangle(Rectangle rect, Point center, float radius)
		{
			return CircleIntersectsWithRectangle(rect, center.X, center.Y, radius);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool CircleIntersectsWithRectangle(Rectangle rect, int centerX, int centerY, float radius)
		{
			float radiusSq = radius*radius;

			// Translate coordinates, placing center at the origin.
			int left = rect.Left - centerX;
			int top = rect.Top - centerY;
			int right = left + rect.Width;
			int bottom = top + rect.Height;

			// rect to left of circle center
			if (right < 0) {
				// rect in lower left corner
				if (bottom < 0)
					return ((right*right + bottom*bottom) < radiusSq);
					// rect in upper left corner
				else if (top > 0)
					return ((right*right + top*top) < radiusSq);
					// rect due West of circle
				else
					return (Math.Abs(right) < radius);
			}
				// rect to right of circle center
			else if (left > 0) {
				// rect in lower right corner
				if (bottom < 0)
					return ((left*left + bottom*bottom) < radiusSq);
					// rect in upper right corner
				else if (top > 0)
					return ((left*left + top*top) < radiusSq);
					// rect due East of circle
				else
					return (left < radius);
			}
				// rect on circle vertical centerline
			else
				// rect due South of circle
				if (bottom < 0)
					return (Math.Abs(bottom) < radius);
					// rect due North of circle
				else if (top > 0)
					return (top < radius);
					// rect contains circle centerpoint
				else
					return true;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool CircleIntersectsWithRectangle(Rectangle rect, PointF center, float radius)
		{
			return CircleIntersectsWithRectangle(rect, center.X, center.Y, radius);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool CircleIntersectsWithRectangle(Rectangle rect, float centerX, float centerY, float radius)
		{
			float radiusSq = radius*radius;

			// Translate coordinates, placing center at the origin.
			float left = rect.Left - centerX;
			float top = rect.Top - centerY;
			float right = left + rect.Width;
			float bottom = top + rect.Height;

			// rect to left of circle center
			if (right < 0) {
				// rect in lower left corner
				if (bottom < 0)
					return ((right*right + bottom*bottom) < radiusSq);
					// rect in upper left corner
				else if (top > 0)
					return ((right*right + top*top) < radiusSq);
					// rect due West of circle
				else
					return (Math.Abs(right) < radius);
			}
				// rect to right of circle center
			else if (left > 0) {
				// rect in lower right corner
				if (bottom < 0)
					return ((left*left + bottom*bottom) < radiusSq);
					// rect in upper right corner
				else if (top > 0)
					return ((left*left + top*top) < radiusSq);
					// rect due East of circle
				else
					return (left < radius);
			}
				// rect on circle vertical centerline
			else
				// rect due South of circle
				if (bottom < 0)
					return (Math.Abs(bottom) < radius);
					// rect due North of circle
				else if (top > 0)
					return (top < radius);
					// rect contains circle centerpoint
				else
					return true;
		}


		/// <summary>
		/// Check if the (rotated) rectangle intersects with the given circle
		/// </summary>
		public static bool CircleIntersectsWithRectangle(Rectangle rect, float angleDeg, Point center, float radius)
		{
			// rotate the circle in the opposite direction, then perform a check for unrotated objects
			PointF[] pt = new PointF[1] {center};
			PointF rotationCenter = PointF.Empty;
			rotationCenter.X = rect.X + (rect.Width/2f);
			rotationCenter.Y = rect.Y + (rect.Height/2f);
			matrix.Reset();
			matrix.RotateAt(DegreesToRadians(-angleDeg), rotationCenter);
			matrix.TransformPoints(pt);
			return CircleIntersectsWithRectangle(rect, pt[0], radius);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool EllipseIntersectsWithLine(Point center, int ellipseWidth, int ellipseHeight, Point p1, Point p2,
		                                             bool isSegment)
		{
			return EllipseIntersectsWithLine(center.X, center.Y, ellipseWidth, ellipseHeight, 0, p1.X, p1.Y, p2.X, p2.Y,
			                                 isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool EllipseIntersectsWithLine(Point center, int ellipseWidth, int ellipseHeight, float ellipseAngleDeg,
		                                             Point p1, Point p2, bool isSegment)
		{
			return EllipseIntersectsWithLine(center.X, center.Y, ellipseWidth, ellipseHeight, ellipseAngleDeg, p1.X, p1.Y, p2.X,
			                                 p2.Y, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool EllipseIntersectsWithLine(int centerX, int centerY, int ellipseWidth, int ellipseHeight, int x1,
		                                             int y1, int x2, int y2, bool isSegment)
		{
			return EllipseIntersectsWithLine(centerX, centerY, ellipseWidth, ellipseHeight, 0, x1, y1, x2, y2, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool EllipseIntersectsWithLine(int centerX, int centerY, int ellipseWidth, int ellipseHeight,
		                                             float ellipseAngleDeg, int x1, int y1, int x2, int y2, bool isSegment)
		{
			Point result = Point.Empty;
			float radiusX = ellipseWidth/2f;
			float radiusY = ellipseHeight/2f;
			float rrx = radiusX*radiusX;
			float rry = radiusY*radiusY;
			if (ellipseAngleDeg != 0) {
				RotatePoint(centerX, centerY, -ellipseAngleDeg, ref x1, ref y1);
				RotatePoint(centerX, centerY, -ellipseAngleDeg, ref x2, ref y2);
			}
			int x21 = x2 - x1;
			int y21 = y2 - y1;
			int x10 = x1 - centerX;
			int y10 = y1 - centerY;
			double a = x21*x21/(double) rrx + y21*y21/(double) rry;
			double b = x21*x10/(double) rrx + y21*y10/(double) rry;
			double c = x10*x10/(double) rrx + y10*y10/(double) rry;
			double d = b*b - a*(c - 1);
			if (d < 0)
				return false;
			else {
				if (isSegment) {
					double rd = Math.Sqrt(d);
					double u1 = (-b - rd)/a;
					double u2 = (-b + rd)/a;
					return ((0 <= u1 && u1 <= 1) || (0 <= u2 && u2 <= 1));
				}
				else return true;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool EllipseIntersectsWithLine(Point center, int ellipseWidth, int ellipseHeight, float ellipseAngleDeg,
		                                             PointF p1, PointF p2, bool isSegment)
		{
			return EllipseIntersectsWithLine(center.X, center.Y, ellipseWidth, ellipseHeight, ellipseAngleDeg, p1.X, p1.Y, p2.X,
			                                 p2.Y, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool EllipseIntersectsWithLine(Point center, int ellipseWidth, int ellipseHeight, PointF p1, PointF p2,
		                                             bool isSegment)
		{
			return EllipseIntersectsWithLine(center.X, center.Y, ellipseWidth, ellipseHeight, 0, p1.X, p1.Y, p2.X, p2.Y,
			                                 isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool EllipseIntersectsWithLine(int centerX, int centerY, int ellipseWidth, int ellipseHeight, float x1,
		                                             float y1, float x2, float y2, bool isSegment)
		{
			// instead of rotating the ellipse, we rotate the line in the opposite direction
			return EllipseIntersectsWithLine(centerX, centerY, ellipseWidth, ellipseHeight, 0, x1, y1, x2, y2, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool EllipseIntersectsWithLine(int centerX, int centerY, int ellipseWidth, int ellipseHeight,
		                                             float ellipseAngleDeg, float x1, float y1, float x2, float y2,
		                                             bool isSegment)
		{
			Point result = Point.Empty;
			float radiusX = ellipseWidth/2f;
			float radiusY = ellipseHeight/2f;
			float rrx = radiusX*radiusX;
			float rry = radiusY*radiusY;
			if (ellipseAngleDeg != 0) {
				RotatePoint(centerX, centerY, -ellipseAngleDeg, ref x1, ref y1);
				RotatePoint(centerX, centerY, -ellipseAngleDeg, ref x2, ref y2);
			}
			float x21 = x2 - x1;
			float y21 = y2 - y1;
			float x10 = x1 - centerX;
			float y10 = y1 - centerY;
			double a = x21*x21/(double) rrx + y21*y21/(double) rry;
			double b = x21*x10/(double) rrx + y21*y10/(double) rry;
			double c = x10*x10/(double) rrx + y10*y10/(double) rry;
			double d = b*b - a*(c - 1);
			if (d < 0)
				return false;
			else {
				if (isSegment) {
					double rd = Math.Sqrt(d);
					double u1 = (-b - rd)/a;
					double u2 = (-b + rd)/a;
					if (double.IsNaN(u1) && (0 <= u1 && u1 <= 1))
						return true;
					else if (!double.IsNaN(u2) && (0 <= u2 && u2 <= 1))
						return true;
					else return false;
				}
				else return true;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool EllipseIntersectsWithRectangle(int ellipseCenterX, int ellipseCenterY, int ellipseWidth,
		                                                  int ellipseHeight, float ellipseAngleDeg, Rectangle rectangle)
		{
			// At least one point of the rectangle is inside the ellipse
			if (EllipseContainsPoint(ellipseCenterX, ellipseCenterY, ellipseWidth, ellipseHeight, ellipseAngleDeg, rectangle.Left,
			                         rectangle.Top)
			    ||
			    EllipseContainsPoint(ellipseCenterX, ellipseCenterY, ellipseWidth, ellipseHeight, ellipseAngleDeg,
			                         rectangle.Right, rectangle.Top)
			    ||
			    EllipseContainsPoint(ellipseCenterX, ellipseCenterY, ellipseWidth, ellipseHeight, ellipseAngleDeg,
			                         rectangle.Right, rectangle.Bottom)
			    ||
			    EllipseContainsPoint(ellipseCenterX, ellipseCenterY, ellipseWidth, ellipseHeight, ellipseAngleDeg, rectangle.Left,
			                         rectangle.Bottom))
				return true;

			// At least one point of the ellipse is inside the rectangle
			Point ellipseLeft = Point.Empty,
			      ellipseTop = Point.Empty,
			      ellipseRight = Point.Empty,
			      ellipseBottom = Point.Empty;
			ellipseLeft.X = (int) Math.Round(ellipseCenterX - (ellipseWidth/2f));
			ellipseRight.X = ellipseLeft.X + ellipseWidth;
			ellipseLeft.Y = ellipseRight.Y = ellipseCenterY;
			ellipseTop.X = ellipseBottom.X = ellipseCenterX;
			ellipseTop.Y = (int) Math.Round(ellipseCenterY - (ellipseHeight/2f));
			ellipseBottom.Y = ellipseTop.Y + ellipseHeight;
			if (ellipseAngleDeg%180 != 0) {
				// No need to rotate any points if the ellipse is upside down or rotated by 360°
				Point ellipseCenter = Point.Empty;
				ellipseCenter.X = ellipseCenterX;
				ellipseCenter.Y = ellipseCenterY;
				ellipseLeft = RotatePoint(ellipseCenter, ellipseAngleDeg, ellipseLeft);
				ellipseTop = RotatePoint(ellipseCenter, ellipseAngleDeg, ellipseTop);
				ellipseRight = RotatePoint(ellipseCenter, ellipseAngleDeg, ellipseRight);
				ellipseBottom = RotatePoint(ellipseCenter, ellipseAngleDeg, ellipseBottom);
			}
			if (RectangleContainsPoint(rectangle, ellipseLeft) || RectangleContainsPoint(rectangle, ellipseRight)
			    || RectangleContainsPoint(rectangle, ellipseTop) || RectangleContainsPoint(rectangle, ellipseBottom))
				return true;

			PointF[] rectPoints = new PointF[4];
			rectPoints[0].X = rectangle.Left;
			rectPoints[0].Y = rectangle.Top;
			rectPoints[1].X = rectangle.Right;
			rectPoints[1].Y = rectangle.Top;
			rectPoints[2].X = rectangle.Right;
			rectPoints[2].Y = rectangle.Bottom;
			rectPoints[3].X = rectangle.Left;
			rectPoints[3].Y = rectangle.Bottom;
			return PolygonIntersectsWithEllipse(ellipseCenterX, ellipseCenterY, ellipseWidth, ellipseHeight, ellipseAngleDeg,
			                                    rectPoints);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool PolygonIntersectsWithEllipse(int ellipseCenterX, int ellipseCenterY, int ellipseWidth,
		                                                int ellipseHeight, float ellipseAngleDeg, PointF[] polygon)
		{
			if (ellipseAngleDeg != 0) {
				// instead of rotating the ellipse, we rotate the polygon in the opposite direction and calculate 
				// intersection of the unrotated ellipse with the rotated polygon
				float x, y;
				for (int i = polygon.Length - 1; i >= 0; --i) {
					x = polygon[i].X;
					y = polygon[i].Y;
					RotatePoint(ellipseCenterX, ellipseCenterY, -ellipseAngleDeg, ref x, ref y);
					polygon[i].X = x;
					polygon[i].Y = y;
				}
			}

			int maxIdx = polygon.Length - 1;
			for (int i = 0; i < maxIdx; ++i) {
				if (EllipseIntersectsWithLine(ellipseCenterX, ellipseCenterY, ellipseWidth, ellipseHeight, polygon[i].X,
				                              polygon[i].Y, polygon[i + 1].X, polygon[i + 1].Y, true))
					return true;
			}
			if (EllipseIntersectsWithLine(ellipseCenterX, ellipseCenterY, ellipseWidth, ellipseHeight, polygon[0].X, polygon[0].Y,
			                              polygon[maxIdx].X, polygon[maxIdx].Y, true))
				return true;

			return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool PolygonIntersectsWithEllipse(int ellipseCenterX, int ellipseCenterY, int ellipseWidth,
		                                                int ellipseHeight, float ellipseAngleDeg, Point[] polygon)
		{
			if (ellipseAngleDeg != 0) {
				// instead of rotating the ellipse, we rotate the polygon in the opposite direction and calculate 
				// intersection of the unrotated ellipse with the rotated polygon
				int x, y;
				for (int i = polygon.Length - 1; i >= 0; --i) {
					x = polygon[i].X;
					y = polygon[i].Y;
					RotatePoint(ellipseCenterX, ellipseCenterY, -ellipseAngleDeg, ref x, ref y);
					polygon[i].X = x;
					polygon[i].Y = y;
				}
			}

			int maxIdx = polygon.Length - 1;
			for (int i = 0; i < maxIdx; ++i) {
				if (EllipseIntersectsWithLine(ellipseCenterX, ellipseCenterY, ellipseWidth, ellipseHeight, polygon[i].X,
				                              polygon[i].Y, polygon[i + 1].X, polygon[i + 1].Y, true))
					return true;
			}
			if (EllipseIntersectsWithLine(ellipseCenterX, ellipseCenterY, ellipseWidth, ellipseHeight, polygon[0].X, polygon[0].Y,
			                              polygon[maxIdx].X, polygon[maxIdx].Y, true))
				return true;
			return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool RectangleIntersectsWithRectangle(Rectangle rect1, Rectangle rect2)
		{
			return ((((rect2.X <= rect1.Right) && (rect1.X <= rect2.Right)) && (rect2.Y <= rect1.Bottom)) &&
			        (rect1.Y <= rect2.Bottom));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool RectangleIntersectsWithRectangle(Rectangle rect, int x, int y, int width, int height)
		{
			Rectangle rect2 = Rectangle.Empty;
			rect2.Offset(x, y);
			rect2.Width = width;
			rect2.Height = height;
			return RectangleIntersectsWithRectangle(rect, rect2);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool RectangleIntersectsWithRectangle(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2)
		{
			Rectangle r1 = Rectangle.Empty, r2 = Rectangle.Empty;
			r1.Offset(x1, y1);
			r1.Width = w1;
			r1.Height = h1;
			r2.Offset(x2, y2);
			r2.Width = w2;
			r2.Height = h2;
			return RectangleIntersectsWithRectangle(r1, r2);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool RectangleIntersectsWithRectangle(RectangleF rect1, RectangleF rect2)
		{
			return ((((rect2.X <= rect1.Right) && (rect1.X <= rect2.Right)) && (rect2.Y <= rect1.Bottom)) &&
			        (rect1.Y <= rect2.Bottom));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool RectangleIntersectsWithRectangle(RectangleF rect, float x, float y, float width, float height)
		{
			RectangleF rect2 = RectangleF.Empty;
			rect2.Offset(x, y);
			rect2.Width = width;
			rect2.Height = height;
			return RectangleIntersectsWithRectangle(rect, rect2);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool RectangleIntersectsWithRectangle(float x1, float y1, float w1, float h1, float x2, float y2,
		                                                    float w2, float h2)
		{
			RectangleF r1 = RectangleF.Empty, r2 = RectangleF.Empty;
			r1.Offset(x1, y1);
			r1.Width = w1;
			r1.Height = h1;
			r2.Offset(x2, y2);
			r2.Width = w2;
			r2.Height = h2;
			return RectangleIntersectsWithRectangle(r1, r2);
		}

		#endregion

		#region Intersection calculation functions

		/// <ToBeCompleted></ToBeCompleted>
		public static bool IntersectLineWithLineSegment(int aLine, int bLine, int cLine, Point p1, Point p2, out int x,
		                                                out int y)
		{
			int aSegment, bSegment, cSegment;
			CalcLine(p1.X, p1.Y, p2.X, p2.Y, out aSegment, out bSegment, out cSegment);
			if (IntersectLines(aLine, bLine, cLine, aSegment, bSegment, cSegment, out x, out y)) {
				return (Math.Min(p1.X, p2.X) <= x && x <= Math.Max(p1.X, p2.X)
				        && Math.Min(p1.Y, p2.Y) <= y && y <= Math.Max(p1.Y, p2.Y));
			}
			else return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool IntersectLineWithLineSegment(float aLine, float bLine, float cLine, PointF p1, PointF p2,
		                                                out float x, out float y)
		{
			float aSegment, bSegment, cSegment;
			CalcLine(p1.X, p1.Y, p2.X, p2.Y, out aSegment, out bSegment, out cSegment);
			if (IntersectLines(aLine, bLine, cLine, aSegment, bSegment, cSegment, out x, out y)) {
				return (Math.Min(p1.X, p2.X) <= x && x <= Math.Max(p1.X, p2.X)
				        && Math.Min(p1.Y, p2.Y) <= y && y <= Math.Max(p1.Y, p2.Y));
			}
			else return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Point IntersectLineWithLineSegment(int aLine, int bLine, int cLine, Point p1, Point p2)
		{
			Point result = InvalidPoint;
			int x, y, aSegment, bSegment, cSegment;
			CalcLine(p1.X, p1.Y, p2.X, p2.Y, out aSegment, out bSegment, out cSegment);
			if (IntersectLines(aLine, bLine, cLine, aSegment, bSegment, cSegment, out x, out y)) {
				if (Math.Min(p1.X, p2.X) <= x && x <= Math.Max(p1.X, p2.X)
				    && Math.Min(p1.Y, p2.Y) <= y && y <= Math.Max(p1.Y, p2.Y)) {
					result.X = x;
					result.Y = y;
				}
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static PointF IntersectLineWithLineSegment(float aLine, float bLine, float cLine, PointF p1, PointF p2)
		{
			PointF result = InvalidPointF;
			float x, y, aSegment, bSegment, cSegment;
			CalcLine(p1.X, p1.Y, p2.X, p2.Y, out aSegment, out bSegment, out cSegment);
			if (IntersectLines(aLine, bLine, cLine, aSegment, bSegment, cSegment, out x, out y)) {
				if (Math.Min(p1.X, p2.X) <= x && x <= Math.Max(p1.X, p2.X)
				    && Math.Min(p1.Y, p2.Y) <= y && y <= Math.Max(p1.Y, p2.Y)) {
					result.X = x;
					result.Y = y;
				}
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Point IntersectLineSegments(Point p1, Point p2, Point p3, Point p4)
		{
			return IntersectLineSegments(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Point IntersectLineSegments(int p1X, int p1Y, int p2X, int p2Y, int p3X, int p3Y, int p4X, int p4Y)
		{
			Point result = InvalidPoint;
			int x21 = p2X - p1X;
			int y21 = p2Y - p1Y;
			int x13 = p1X - p3X;
			int y13 = p1Y - p3Y;
			int x43 = p4X - p3X;
			int y43 = p4Y - p3Y;
			float d = y43*x21 - x43*y21;
			if (d != 0) {
				float u2 = ((x21*y13 - y21*x13)/d);
				if (!float.IsNaN(u2) && 0 <= u2 && u2 <= 1) {
					float u1 = (x43*y13 - y43*x13)/d;
					if (!float.IsNaN(u1) && 0 <= u1 && u1 <= 1) {
						result.X = (int) Math.Round(p1X + x21*u1);
						result.Y = (int) Math.Round(p1Y + y21*u1);
					}
				}
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Point IntersectLineSegments(long p1X, long p1Y, long p2X, long p2Y, long p3X, long p3Y, long p4X,
		                                          long p4Y)
		{
			Point result = InvalidPoint;
			long x21 = p2X - p1X;
			long y21 = p2Y - p1Y;
			long x13 = p1X - p3X;
			long y13 = p1Y - p3Y;
			long x43 = p4X - p3X;
			long y43 = p4Y - p3Y;
			double d = y43*x21 - x43*y21;
			if (d != 0) {
				double u2 = ((x21*y13 - y21*x13)/d);
				if (!double.IsNaN(u2) && 0 <= u2 && u2 <= 1) {
					double u1 = (x43*y13 - y43*x13)/d;
					if (!double.IsNaN(u1) && 0 <= u1 && u1 <= 1) {
						result.X = (int) Math.Round(p1X + x21*u1);
						result.Y = (int) Math.Round(p1Y + y21*u1);
					}
				}
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static PointF IntersectLineSegments(PointF p1, PointF p2, PointF p3, PointF p4)
		{
			return IntersectLineSegments(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static PointF IntersectLineSegments(float p1X, float p1Y, float p2X, float p2Y, float p3X, float p3Y, float p4X,
		                                           float p4Y)
		{
			float x21 = p2X - p1X;
			float y21 = p2Y - p1Y;
			float x13 = p1X - p3X;
			float y13 = p1Y - p3Y;
			float x43 = p4X - p3X;
			float y43 = p4Y - p3Y;
			float d = y43*x21 - x43*y21;
			PointF result = InvalidPointF;
			if (d != 0) {
				float u2 = (x21*y13 - y21*x13)/d;
				if (!float.IsNaN(u2) && 0 <= u2 && u2 <= 1) {
					float u1 = (x43*y13 - y43*x13)/d;
					if (!float.IsNaN(u1) && 0 <= u1 && u1 <= 1) {
						result.X = p1X + x21*u1;
						result.Y = p1Y + y21*u1;
					}
				}
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Point IntersectLineWithLineSegment(int lineStartX, int lineStartY, int lineEndX, int lineEndY,
		                                                 int lineSegmentStartX, int lineSegmentStartY, int lineSegmentEndX,
		                                                 int lineSegmentEndY)
		{
			Point result = InvalidPoint;
			float a1, b1, c1, a2, b2, c2;
			CalcLine(lineStartX, lineStartY, lineEndX, lineEndY, out a1, out b1, out c1);
			CalcLine(lineSegmentStartX, lineSegmentStartY, lineSegmentEndX, lineSegmentEndY, out a2, out b2, out c2);
			float x, y;
			if (IntersectLines(a1, b1, c1, a2, b2, c2, out x, out y)) {
				if (Math.Min(lineSegmentStartX, lineSegmentEndX) <= x && x <= Math.Max(lineSegmentStartX, lineSegmentEndX)
				    && Math.Min(lineSegmentStartY, lineSegmentEndY) <= y && y <= Math.Max(lineSegmentStartY, lineSegmentEndY)) {
					result.X = (int) Math.Round(x);
					result.Y = (int) Math.Round(y);
				}
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static PointF IntersectLineWithLineSegment(float lineStartX, float lineStartY, float lineEndX, float lineEndY,
		                                                  float lineSegmentStartX, float lineSegmentStartY,
		                                                  float lineSegmentEndX, float lineSegmentEndY)
		{
			PointF result = InvalidPointF;
			float a1, b1, c1, a2, b2, c2;
			CalcLine(lineStartX, lineStartY, lineEndX, lineEndY, out a1, out b1, out c1);
			CalcLine(lineSegmentStartX, lineSegmentStartY, lineSegmentEndX, lineSegmentEndY, out a2, out b2, out c2);
			float x, y;
			//if (SolveLinear22System(a1, b1, a2, b2, c1, c2, out x, out y)) {
			if (IntersectLines(a1, b1, c1, a2, b2, c2, out x, out y)) {
				if (Math.Min(lineSegmentStartX, lineSegmentEndX) <= x && x <= Math.Max(lineSegmentStartX, lineSegmentEndX)
				    && Math.Min(lineSegmentStartY, lineSegmentEndY) <= y && y <= Math.Max(lineSegmentStartY, lineSegmentEndY)) {
					result.X = x;
					result.Y = y;
				}
			}
			return result;
		}


		/// <summary>
		/// Calculates the intersection point of two lines given in the form ax + bx + c = 0
		/// </summary>
		// Algorithm Explaination:
		// a1 * x + b1 * y = -c1   multiplied by b2
		// and
		// a2 * x + b2 * y = -c2   multiplied by b1
		// results in 
		// a1 b2 x + b1 b2 y = -(b2 c1)          [1]
		// a2 b1 x + b1 b2 y = -(b1 c2)          [2]
		// [1] - [2] results in
		// a1 b2 x - a2 b1 x = - b2 c1 - b1 c2   [3]
		// [3] divided through a1 b2 - a2 b1 results in the equation for x (analog for y)
		public static bool IntersectLines(float a1, float b1, float c1, float a2, float b2, float c2, out int x, out int y)
		{
			double det = (float) a1*(float) b2 - (float) a2*(float) b1;
			if (det == 0) {
				x = InvalidPoint.X;
				y = InvalidPoint.Y;
				return false;
			}
			else {
				x = (int) Math.Round(((float) b2*(float) -c1 - (float) b1*(float) -c2)/det);
				y = (int) Math.Round(((float) a1*(float) -c2 - (float) a2*(float) -c1)/det);
				return true;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool IntersectLines(int a1, int b1, int c1, int a2, int b2, int c2, out int x, out int y)
		{
			x = InvalidPoint.X;
			y = InvalidPoint.Y;
			double det = (float) a1*(float) b2 - (float) a2*(float) b1;
			if (det == 0) return false;
			else {
				x = (int) Math.Round(((float) b2*(float) -c1 - (float) b1*(float) -c2)/det);
				y = (int) Math.Round(((float) a1*(float) -c2 - (float) a2*(float) -c1)/det);
				return true;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool IntersectLines(float a1, float b1, float c1, float a2, float b2, float c2, out float x, out float y)
		{
			x = InvalidPointF.X;
			y = InvalidPointF.Y;
			double det = (float) a1*(float) b2 - (float) a2*(float) b1;
			if (det == 0) return false;
			else {
				x = (float) ((b2*-c1 - b1*-c2)/det);
				y = (float) ((a1*-c2 - a2*-c1)/det);
				return true;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Point IntersectLines(int a1, int b1, int c1, int a2, int b2, int c2)
		{
			Point result = InvalidPoint;
			int x, y;
			if (IntersectLines(a1, b1, c1, a2, b2, c2, out x, out y)) {
				result.X = x;
				result.Y = y;
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Point IntersectLines(int a1, int b1, int c1, Point a, Point b)
		{
			Point result = InvalidPoint;
			int a2, b2, c2;
			CalcLine(a.X, a.Y, b.X, b.Y, out a2, out b2, out c2);
			int x, y;
			if (IntersectLines(a1, b1, c1, a2, b2, c2, out x, out y)) {
				result.X = x;
				result.Y = y;
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static PointF IntersectLines(float a1, float b1, float c1, PointF a, PointF b)
		{
			PointF result = InvalidPointF;
			float a2, b2, c2;
			CalcLine(a.X, a.Y, b.X, b.Y, out a2, out b2, out c2);
			float x, y;
			if (IntersectLines(a1, b1, c1, a2, b2, c2, out x, out y)) {
				result.X = x;
				result.Y = y;
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static PointF IntersectLines(float a1, float b1, float c1, float a2, float b2, float c2)
		{
			PointF result = InvalidPointF;
			float x, y;
			if (IntersectLines(a1, b1, c1, a2, b2, c2, out x, out y)) {
				result.X = x;
				result.Y = y;
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static PointF IntersectLines(float line1StartX, float line1StartY, float line1EndX, float line1EndY,
		                                    float line2StartX, float line2StartY, float line2EndX, float line2EndY)
		{
			PointF result = InvalidPointF;
			float a1, b1, c1, a2, b2, c2;
			CalcLine(line1StartX, line1StartY, line1EndX, line1EndY, out a1, out b1, out c1);
			CalcLine(line2StartX, line2StartY, line2EndX, line2EndY, out a2, out b2, out c2);
			float x, y;
			if (IntersectLines(a1, b1, c1, a2, b2, c2, out x, out y)) {
				result.X = x;
				result.Y = y;
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Point IntersectLines(int line1StartX, int line1StartY, int line1EndX, int line1EndY, int line2StartX,
		                                   int line2StartY, int line2EndX, int line2EndY)
		{
			Point result = InvalidPoint;
			int a1, b1, c1, a2, b2, c2;
			CalcLine(line1StartX, line1StartY, line1EndX, line1EndY, out a1, out b1, out c1);
			CalcLine(line2StartX, line2StartY, line2EndX, line2EndY, out a2, out b2, out c2);
			int x, y;
			if (IntersectLines(a1, b1, c1, a2, b2, c2, out x, out y)) {
				result.X = x;
				result.Y = y;
			}
			return result;
		}


		/// <summary>
		/// Calculates the intersection point of the line with the rectangle that is nearer to point 1 of the line.
		/// </summary>
		public static Point IntersectLineWithRectangle(int lineX1, int lineY1, int lineX2, int lineY2, int rectX1, int rectY1,
		                                               int rectX2, int rectY2)
		{
			Point result = InvalidPoint;
			if (lineX1 == lineX2 && lineY1 == lineY2)
				return result;

			if (lineY1 <= rectY1) {
				if (lineY2 > rectY1) {
					if (lineX1 <= rectX1) {
						// links oben
						result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX1, rectY1, rectX1, rectY2);
						if (!IsValid(result))
							result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX1, rectY1, rectX2, rectY1);
					}
					else if (lineX1 >= rectX2) {
						// rechts oben
						result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX2, rectY1, rectX2, rectY2);
						if (!IsValid(result))
							result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX1, rectY1, rectX2, rectY1);
					}
					else {
						// Mitte oben
						result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX1, rectY1, rectX2, rectY1);
					}
				}
			}
			else if (lineY1 >= rectY2) {
				if (lineY2 < rectY2) {
					if (lineX1 <= rectX1) {
						// links unten
						result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX1, rectY1, rectX1, rectY2);
						if (!IsValid(result))
							result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX1, rectY2, rectX2, rectY2);
					}
					else if (lineX1 >= rectX2) {
						// rechts unten
						result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX1, rectY2, rectX2, rectY2);
						if (!IsValid(result))
							result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX2, rectY1, rectX2, rectY2);
					}
					else {
						// Mitte unten
						result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX1, rectY2, rectX2, rectY2);
					}
				}
			}
			else if (lineX1 <= rectX1) {
				// links mitte
				result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX1, rectY1, rectX1, rectY2);
			}
			else if (lineX1 >= rectX2) {
				// rechts mitte
				result = IntersectLineWithLineSegment(lineX1, lineY1, lineX2, lineY2, rectX2, rectY1, rectX2, rectY2);
			}
			else if (lineY2 <= rectY1 || lineY2 >= rectY2 || lineX2 <= rectX1 || lineX2 >= rectX2) {
				result = IntersectLineWithRectangle(lineX2, lineY2, lineX1, lineY1, rectX1, rectY1, rectX2, rectY2);
			}
			else {
				// Beide sind innen, wir verlängern die Gerade über X2, Y2 hinaus
				int newLineX1, newLineY1;
				if (Math.Abs(lineX2 - lineX1) >= Math.Abs(lineY2 - lineY1)) {
					// X in den Nenner, Y durch Rechteck bestimmen
					if (lineX2 >= lineX1) newLineX1 = lineX1 + rectX2 - rectX1;
					else newLineX1 = lineX1 + rectX1 - rectX2;
					newLineY1 = lineY2 + (newLineX1 - lineX2)*(lineY1 - lineY2)/(lineX1 - lineX2);
				}
				else {
					// Y in den Nenner, X durch Rechteckbreite bestimmen
					if (lineY2 >= lineY1) newLineY1 = lineY1 + rectY2 - rectY1;
					else newLineY1 = lineY1 + rectY1 - rectY2;
					newLineX1 = lineX2 + (newLineY1 - lineY2)*(lineX1 - lineX2)/(lineY1 - lineY2);
				}
				result = IntersectLineWithRectangle(newLineX1, newLineY1, lineX2, lineY2, rectX1, rectY1, rectX2, rectY2);
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Point IntersectCircleWithLine(Point center, int radius, Point p1, Point p2, bool isSegment)
		{
			return IntersectCircleWithLine(center.X, center.Y, radius, p1.X, p1.Y, p2.X, p2.Y, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static PointF IntersectCircleWithLine(PointF center, float radius, PointF p1, PointF p2, bool isSegment)
		{
			return IntersectCircleWithLine(center.X, center.Y, radius, p1.X, p1.Y, p2.X, p2.Y, isSegment);
		}


		/// <summary>
		/// Calculates and returns the intersection point of the given circle with the given line (segment) that is nearest to point x1/y1.
		/// </summary>
		/// <param name="centerX">X Coordinate of the circle's center</param>
		/// <param name="centerY">Y Coordinate of the circle's center</param>
		/// <param name="radius">Radius of the circle</param>
		/// <param name="x1">X coordinate of the first point of the line (segment)</param>
		/// <param name="y1">Y coordinate of the first point of the line (segment)</param>
		/// <param name="x2">X coordinate of the second point of the line (segment)</param>
		/// <param name="y2">Y coordinate of the second point of the line (segment)</param>
		/// <param name="isSegment">Specifies if the given line should be treated as a (endable) line segment instead of a (endless) line</param>
		public static Point IntersectCircleWithLine(int centerX, int centerY, int radius, int x1, int y1, int x2, int y2,
		                                            bool isSegment)
		{
			Point result = InvalidPoint;
			if (x1 == x2 && y1 == y2)
				return result;
			float rr = radius*radius;
			float x21 = x2 - x1;
			float y21 = y2 - y1;
			float x10 = x1 - centerX;
			float y10 = y1 - centerY;
			float a = (x21*x21 + y21*y21)/rr;
			float b = (x21*x10 + y21*y10)/rr;
			float c = (x10*x10 + y10*y10)/rr;
			float d = b*b - a*(c - 1);
			if (d >= 0) {
				float e = (float) Math.Sqrt(d);
				float u1 = (-b - e)/a;
				float u2 = (-b + e)/a;
				Point pt1 = InvalidPoint;
				Point pt2 = InvalidPoint;
				if (!isSegment || (!float.IsNaN(u1) && 0 <= u1 && u1 <= 1)) {
					Point p = Point.Empty;
					p.X = (int) Math.Round(x1 + x21*u1);
					p.Y = (int) Math.Round(y1 + y21*u1);
					pt1 = p;
				}
				if (!isSegment || (!float.IsNaN(u2) && 0 <= u2 && u2 <= 1)) {
					Point p = Point.Empty;
					p.X = (int) Math.Round(x1 + x21*u2);
					p.Y = (int) Math.Round(y1 + y21*u2);
					pt2 = p;
				}
				if (IsValid(pt1) && IsValid(pt2))
					result = GetNearestPoint(x1, y1, pt1.X, pt1.Y, pt2.X, pt2.Y);
				else if (IsValid(pt1)) result = pt1;
				else if (IsValid(pt2)) result = pt2;
			}
			return result;
		}


		/// <summary>
		/// Calculates and returns the intersection point of the given circle with the given line (segment) that is nearest to point x1/y1.
		/// </summary>
		/// <param name="centerX">X Coordinate of the circle's center</param>
		/// <param name="centerY">Y Coordinate of the circle's center</param>
		/// <param name="radius">Radius of the circle</param>
		/// <param name="x1">X coordinate of the first point of the line (segment)</param>
		/// <param name="y1">Y coordinate of the first point of the line (segment)</param>
		/// <param name="x2">X coordinate of the second point of the line (segment)</param>
		/// <param name="y2">Y coordinate of the second point of the line (segment)</param>
		/// <param name="isSegment">Specifies if the given line should be treated as a (endable) line segment instead of a (endless) line</param>
		public static PointF IntersectCircleWithLine(float centerX, float centerY, float radius, float x1, float y1, float x2,
		                                             float y2, bool isSegment)
		{
			PointF p = Point.Empty;
			p.X = x1;
			p.Y = y1;
			return GetNearestPoint(p, GetAllCircleLineIntersections(centerX, centerY, radius, x1, y1, x2, y2, isSegment));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> GetAllCircleLineIntersections(PointF center, float radius, PointF pt1, PointF pt2,
		                                                                bool isSegment)
		{
			return GetAllCircleLineIntersections(center.X, center.Y, radius, pt1.X, pt1.Y, pt2.X, pt2.Y, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> GetAllCircleLineIntersections(float centerX, float centerY, float radius, float x1,
		                                                                float y1, float x2, float y2, bool isSegment)
		{
			PointF result = PointF.Empty;
			double rr = radius*radius;
			float x21 = x2 - x1;
			float y21 = y2 - y1;
			float x10 = x1 - centerX;
			float y10 = y1 - centerY;
			double a = (x21*x21 + y21*y21)/rr;
			double b = (x21*x10 + y21*y10)/rr;
			double c = (x10*x10 + y10*y10)/rr;
			double d = b*b - a*(c - 1);
			if (d >= 0) {
				double e = Math.Sqrt(d);
				double u1 = (-b - e)/a;
				double u2 = (-b + e)/a;
				if (!isSegment || (0 <= u1 && u1 <= 1)) {
					result.X = (float) (x1 + x21*u1);
					result.Y = (float) (y1 + y21*u1);
					yield return result;
				}
				if (!isSegment || (0 <= u2 && u2 <= 1)) {
					result.X = (float) (x1 + x21*u2);
					result.Y = (float) (y1 + y21*u2);
					yield return result;
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> GetAllCircleLineIntersections(Point center, float radius, Point pt1, Point pt2,
		                                                               bool isSegment)
		{
			return GetAllCircleLineIntersections(center.X, center.Y, radius, pt1.X, pt1.Y, pt2.X, pt2.Y, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> GetAllCircleLineIntersections(int centerX, int centerY, float radius, int x1, int y1,
		                                                               int x2, int y2, bool isSegment)
		{
			foreach (
				PointF p in
					GetAllCircleLineIntersections((float) centerX, (float) centerY, (float) radius, (float) x1, (float) y1, (float) x2,
					                              (float) y2, isSegment))
				yield return Point.Round(p);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> IntersectCircles(int center1X, int center1Y, int radius1, int center2X, int center2Y,
		                                                   int radius2)
		{
			PointF result = Point.Empty;
			double a1, a2, b1, b2, c1, c2, d, e, f;

			a1 = 2*center1X;
			b1 = 2*center1Y;
			c1 = (radius1*radius1) - (center1X*center1X) - (center1Y*center1Y);

			a2 = 2*center2X;
			b2 = 2*center2Y;
			c2 = (radius2*radius2) - (center2X*center2X) - (center2Y*center2Y);

			if ((a2 - a1) > (b2 - b1)) {
				d = (c1 - c2)/(a2 - a1);
				e = (b1 - b2)/(a2 - a1);
				f = ((2*d*e) - b1 - (a1*e))/(2*(e*e) + 2);

				result.Y = (float) Math.Round(Math.Sqrt((f*f) - (((d*d) - (a1*d) - c1)/((e*e) + 1))) - f, 6);
				result.X = (float) Math.Round(d + e*result.Y, 6);
				yield return result;

				result.Y = (float) Math.Round(-Math.Sqrt((f*f) - (((d*d) - (a1*d) - c1)/((e*e) + 1))) - f, 6);
				result.X = (float) Math.Round(d + e*result.Y);
				yield return result;
			}
			else {
				d = (c1 - c2)/(b2 - b1);
				e = (a1 - a2)/(b2 - b1);
				f = ((2*d*e) - a1 - (b1*e))/(2*(e*e) + 2);

				result.X = (float) Math.Round(Math.Sqrt((f*f) - (((d*d) - (b1*d) - c1)/((e*e) + 1))) - f, 6);
				result.Y = (float) Math.Round(d + e*result.X, 6);
				yield return result;

				result.X = (float) Math.Round(-Math.Sqrt((f*f) - (((d*d) - (b1*d) - c1)/((e*e) + 1))) - f, 6);
				result.Y = (float) Math.Round(d + e*result.X, 6);
				yield return result;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> IntersectCircles(float center1X, float center1Y, float radius1, float center2X,
		                                                   float center2Y, float radius2)
		{
			PointF result = Point.Empty;
			double a1, a2, b1, b2, c1, c2, d, e, f;

			a1 = 2*center1X;
			b1 = 2*center1Y;
			c1 = (radius1*radius1) - (center1X*center1X) - (center1Y*center1Y);

			a2 = 2*center2X;
			b2 = 2*center2Y;
			c2 = (radius2*radius2) - (center2X*center2X) - (center2Y*center2Y);

			if ((a2 - a1) > (b2 - b1) || (b2 - b1 == 0)) {
				d = (c1 - c2)/(a2 - a1);
				e = (b1 - b2)/(a2 - a1);
				f = ((2*d*e) - b1 - (a1*e))/(2*(e*e) + 2);

				result.Y = (float) (Math.Sqrt((f*f) - (((d*d) - (a1*d) - c1)/((e*e) + 1))) - f);
				result.X = (float) (d + e*result.Y);
				yield return result;

				result.Y = (float) (-Math.Sqrt((f*f) - (((d*d) - (a1*d) - c1)/((e*e) + 1))) - f);
				result.X = (float) (d + e*result.Y);
				yield return result;
			}
			else {
				d = (c1 - c2)/(b2 - b1);
				e = (a1 - a2)/(b2 - b1);
				f = ((2*d*e) - a1 - (b1*e))/(2*(e*e) + 2);

				result.X = (float) (Math.Sqrt((f*f) - (((d*d) - (b1*d) - c1)/((e*e) + 1))) - f);
				result.Y = (float) (d + e*result.X);
				yield return result;

				result.X = (float) (-Math.Sqrt((f*f) - (((d*d) - (b1*d) - c1)/((e*e) + 1))) - f);
				result.Y = (float) (d + e*result.X);
				yield return result;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> IntersectCircleArc(int circleCenterX, int circleCenterY, int circleRadius,
		                                                    int arcStartPtX, int arcStartPtY, int arcRadiusPtX,
		                                                    int arcRadiusPtY, int arcEndPtX, int arcEndPtY)
		{
			float arcRadius;
			PointF arcCenter;
			CalcCircumCircle(arcStartPtX, arcStartPtY, arcRadiusPtX, arcRadiusPtY, arcEndPtX, arcEndPtY, out arcCenter,
			                 out arcRadius);
			return IntersectCircleArc(circleCenterX, circleCenterY, circleRadius, arcStartPtX, arcStartPtY, arcRadiusPtX,
			                          arcRadiusPtY, arcEndPtX, arcEndPtY, arcCenter.X, arcCenter.Y, arcRadius);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> IntersectCircleArc(int circleCenterX, int circleCenterY, int circleRadius,
		                                                    int arcStartPtX, int arcStartPtY, int arcRadiusPtX,
		                                                    int arcRadiusPtY, int arcEndPtX, int arcEndPtY, float arcCenterX,
		                                                    float arcCenterY, float arcRadius)
		{
			foreach (PointF p in IntersectCircles(circleCenterX, circleCenterY, circleRadius, arcCenterX, arcCenterY, arcRadius)) {
				if (ArcContainsPoint(arcStartPtX, arcStartPtY, arcRadiusPtX, arcRadiusPtY, arcEndPtX, arcEndPtY, arcCenterX,
				                     arcCenterY, arcRadius, p.X, p.Y, 0.1f))
					yield return Point.Round(p);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> IntersectCircleArc(float circleCenterX, float circleCenterY, float circleRadius,
		                                                     float arcStartPtX, float arcStartPtY, float arcRadiusPtX,
		                                                     float arcRadiusPtY, float arcEndPtX, float arcEndPtY)
		{
			float arcRadius;
			PointF arcCenter;
			CalcCircumCircle(arcStartPtX, arcStartPtY, arcRadiusPtX, arcRadiusPtY, arcEndPtX, arcEndPtY, out arcCenter,
			                 out arcRadius);
			return IntersectCircleArc(circleCenterX, circleCenterY, circleRadius, arcStartPtX, arcStartPtY, arcRadiusPtX,
			                          arcRadiusPtY, arcEndPtX, arcEndPtY, arcCenter.X, arcCenter.Y, arcRadius);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> IntersectCircleArc(float circleCenterX, float circleCenterY, float circleRadius,
		                                                     float arcStartPtX, float arcStartPtY, float arcRadiusPtX,
		                                                     float arcRadiusPtY, float arcEndPtX, float arcEndPtY,
		                                                     float arcCenterX, float arcCenterY, float arcRadius)
		{
			foreach (PointF p in IntersectCircles(circleCenterX, circleCenterY, circleRadius, arcCenterX, arcCenterY, arcRadius)) {
				if (ArcContainsPoint(arcStartPtX, arcStartPtY, arcRadiusPtX, arcRadiusPtY, arcEndPtX, arcEndPtY, arcCenterX,
				                     arcCenterY, arcRadius, p.X, p.Y, 0.1f))
					yield return p;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> IntersectArcLine(Point startPt, Point radiusPt, Point endPt, Point p1, Point p2,
		                                                  bool isSegment)
		{
			return IntersectArcLine(startPt.X, startPt.Y, radiusPt.X, radiusPt.Y, endPt.X, endPt.Y, p1.X, p1.Y, p2.X, p2.Y,
			                        isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> IntersectArcLine(int startPtX, int startPtY, int radiusPtX, int radiusPtY, int endPtX,
		                                                  int endPtY, int x1, int y1, int x2, int y2, bool isSegment)
		{
			// calculate center point and radius
			float radius;
			PointF center;
			CalcCircumCircle((float) startPtX, (float) startPtY, (float) radiusPtX, (float) radiusPtY, (float) endPtX,
			                 (float) endPtY, out center, out radius);
			foreach (PointF p in GetAllCircleLineIntersections(center.X, center.Y, radius, x1, y1, x2, y2, isSegment))
				if (ArcContainsPoint(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, center.X, center.Y, radius, p.X, p.Y,
				                     0.1f))
					yield return Point.Round(p);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> IntersectArcLine(PointF startPt, PointF radiusPt, PointF endPt, PointF p1, PointF p2,
		                                                   bool isSegment)
		{
			return IntersectArcLine(startPt.X, startPt.Y, radiusPt.X, radiusPt.Y, endPt.X, endPt.Y, p1.X, p1.Y, p2.X, p2.Y,
			                        isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> IntersectArcLine(float startPtX, float startPtY, float radiusPtX, float radiusPtY,
		                                                   float endPtX, float endPtY, float x1, float y1, float x2, float y2,
		                                                   bool isSegment)
		{
			// calculate center point and radius
			float radius;
			PointF center;
			CalcCircumCircle(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, out center, out radius);
			foreach (PointF p in GetAllCircleLineIntersections(center.X, center.Y, radius, x1, y1, x2, y2, isSegment))
				if (ArcContainsPoint(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, center.X, center.Y, radius, p.X, p.Y,
				                     0.1f))
					yield return p;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> IntersectPolygonLine(Point[] points, Point p1, Point p2, bool isSegment)
		{
			return IntersectPolygonLine(points, p1.X, p1.Y, p2.X, p2.Y, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> IntersectPolygonLine(Point[] points, int pt1X, int pt1Y, int pt2X, int pt2Y,
		                                                      bool isSegment)
		{
			if (points == null) throw new ArgumentNullException("points");
			Point result = Point.Empty;
			Point polyPt1 = Point.Empty;
			Point polyPt2 = Point.Empty;

			int a, b, c;
			int aLine, bLine, cLine;
			CalcLine(pt1X, pt1Y, pt2X, pt2Y, out aLine, out bLine, out cLine);

			int maxIdx = points.Length - 1;
			for (int i = 0; i < maxIdx; ++i) {
				polyPt1.X = points[i].X;
				polyPt1.Y = points[i].Y;
				polyPt2.X = points[i + 1].X;
				polyPt2.Y = points[i + 1].Y;
				if (isSegment
				    	? LineSegmentIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y)
				    	: LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y)) {
					CalcLine(polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y, out a, out b, out c);
					int x, y;
					if (IntersectLines(a, b, c, aLine, bLine, cLine, out x, out y)) {
						result.X = x;
						result.Y = y;
						yield return result;
					}
				}
			}
			if (points[0] != points[maxIdx]) {
				polyPt1.X = points[0].X;
				polyPt1.Y = points[0].Y;
				polyPt2.X = points[maxIdx].X;
				polyPt2.Y = points[maxIdx].Y;
				if (isSegment
				    	? LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y,
				    	                                polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y)
				    	: LineIntersectsWithLine(pt1X, pt1Y, pt2X, pt2Y,
				    	                         polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y)) {
					CalcLine(polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y, out a, out b, out c);
					int x, y;
					if (IntersectLines(a, b, c, aLine, bLine, cLine, out x, out y)) {
						result.X = x;
						result.Y = y;
						yield return result;
					}
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> IntersectPolygonLine(PointF[] points, PointF p1, PointF p2, bool isSegment)
		{
			return IntersectPolygonLine(points, p1, p2, isSegment);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> IntersectPolygonLine(PointF[] points, float pt1X, float pt1Y, float pt2X, float pt2Y,
		                                                      bool isSegment)
		{
			if (points == null) throw new ArgumentNullException("points");
			Point result = Point.Empty;
			PointF polyPt1 = Point.Empty;
			PointF polyPt2 = Point.Empty;

			float a, b, c;
			float aLine, bLine, cLine;
			CalcLine(pt1X, pt1Y, pt2X, pt2Y, out aLine, out bLine, out cLine);

			int maxIdx = points.Length - 1;
			for (int i = 0; i < maxIdx; ++i) {
				polyPt1.X = points[i].X;
				polyPt1.Y = points[i].Y;
				polyPt2.X = points[i + 1].X;
				polyPt2.Y = points[i + 1].Y;
				bool intersection;
				if (isSegment)
					intersection = LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y);
				else intersection = LineIntersectsWithLine(pt1X, pt1Y, pt2X, pt2Y, polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y);
				if (intersection) {
					CalcLine(polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y, out a, out b, out c);
					float x, y;
					if (IntersectLines(a, b, c, aLine, bLine, cLine, out x, out y)) {
						result.X = (int) Math.Round(x);
						result.Y = (int) Math.Round(y);
						yield return result;
					}
				}
			}
			if (points[0] != points[maxIdx]) {
				polyPt1.X = points[0].X;
				polyPt1.Y = points[0].Y;
				polyPt2.X = points[maxIdx].X;
				polyPt2.Y = points[maxIdx].Y;
				bool intersection;
				if (isSegment)
					intersection = LineIntersectsWithLineSegment(pt1X, pt1Y, pt2X, pt2Y, polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y);
				else intersection = LineIntersectsWithLine(pt1X, pt1Y, pt2X, pt2Y, polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y);
				if (intersection) {
					CalcLine(polyPt1.X, polyPt1.Y, polyPt2.X, polyPt2.Y, out a, out b, out c);
					float x, y;
					if (IntersectLines(a, b, c, aLine, bLine, cLine, out x, out y)) {
						result.X = (int) Math.Round(x);
						result.Y = (int) Math.Round(y);
						yield return result;
					}
				}
			}
		}


		/// <summary>
		/// Calculates the intersection points of the line segment from p1 to p2 with the ellipse.
		/// </summary>
		/// <param name="center">Center point of the ellipse.</param>
		/// <param name="ellipseWidth">Width of the ellipse.</param>
		/// <param name="ellipseHeight">Height of the ellipse.</param>
		/// <param name="ellipseAngleDeg">Rotation angle of the ellipse in degrees.</param>
		/// <param name="p1">First point of the line segment.</param>
		/// <param name="p2">Second point of the line segment.</param>
		/// <param name="isSegment">Specifies whether the tested line should be treated as line or as line segment.</param>
		/// <returns>Intersection points of the line with the ellipse.</returns>		
		public static IEnumerable<Point> IntersectEllipseLine(Point center, int ellipseWidth, int ellipseHeight,
		                                                      float ellipseAngleDeg, Point p1, Point p2, bool isSegment)
		{
			return IntersectEllipseLine(center.X, center.Y, ellipseWidth, ellipseHeight, ellipseAngleDeg, p1.X, p1.Y, p2.X, p2.Y,
			                            isSegment);
		}


		/// <summary>
		/// Calculates the intersection points of the line segment from p1 to p2 with the ellipse.
		/// </summary>
		/// <param name="center">Center point of the ellipse.</param>
		/// <param name="ellipseWidth">Width of the ellipse.</param>
		/// <param name="ellipseHeight">Height of the ellipse.</param>
		/// <param name="p1">First point of the line segment.</param>
		/// <param name="p2">Second point of the line segment.</param>
		/// <param name="isSegment">Specifies whether the tested line should be treated as line or as line segment.</param>
		/// <returns>Intersection points of the line with the ellipse.</returns>		
		public static IEnumerable<Point> IntersectEllipseLine(Point center, int ellipseWidth, int ellipseHeight, Point p1,
		                                                      Point p2, bool isSegment)
		{
			return IntersectEllipseLine(center.X, center.Y, ellipseWidth, ellipseHeight, 0, p1.X, p1.Y, p2.X, p2.Y, isSegment);
		}


		/// <summary>
		/// Calculates the intersection points of the line segment from p1 to p2 with the ellipse.
		/// </summary>
		/// <param name="centerX">X coordinate of the ellipse center point.</param>
		/// <param name="centerY">Y coordinate of the ellipse center point.</param>
		/// <param name="ellipseWidth">Width of the ellipse.</param>
		/// <param name="ellipseHeight">Height of the ellipse.</param>
		/// <param name="x1">X coordinate of the line segments first point.</param>
		/// <param name="y1">Y coordinate of the line segments first point.</param>
		/// <param name="x2">X coordinate of the line segments second point.</param>
		/// <param name="y2">Y coordinate of the line segments second point.</param>
		/// <param name="isSegment">Specifies whether the tested line should be treated as line or as line segment.</param>
		/// <returns>Intersection points of the line with the ellipse.</returns>		
		public static IEnumerable<Point> IntersectEllipseLine(int centerX, int centerY, int ellipseWidth, int ellipseHeight,
		                                                      int x1, int y1, int x2, int y2, bool isSegment)
		{
			return IntersectEllipseLine(centerX, centerY, ellipseWidth, ellipseHeight, 0, x1, y1, x2, y2, isSegment);
		}


		/// <summary>
		/// Calculates the intersection points of the line segment from p1 to p2 with the ellipse.
		/// </summary>
		/// <param name="centerX">X coordinate of the ellipse center point.</param>
		/// <param name="centerY">Y coordinate of the ellipse center point.</param>
		/// <param name="ellipseWidth">Width of the ellipse.</param>
		/// <param name="ellipseHeight">Height of the ellipse.</param>
		/// <param name="ellipseAngleDeg">Specifies the rotation angle of the ellipse in degrees.</param>
		/// <param name="x1">X coordinate of the line segments first point.</param>
		/// <param name="y1">Y coordinate of the line segments first point.</param>
		/// <param name="x2">X coordinate of the line segments second point.</param>
		/// <param name="y2">Y coordinate of the line segments second point.</param>
		/// <param name="isSegment">Specifies whether the tested line should be treated as line or as line segment.</param>
		/// <returns>Intersection points of the line with the ellipse.</returns>		
		public static IEnumerable<Point> IntersectEllipseLine(int centerX, int centerY, int ellipseWidth, int ellipseHeight,
		                                                      float ellipseAngleDeg, int x1, int y1, int x2, int y2,
		                                                      bool isSegment)
		{
			Point result = Point.Empty;
			float x, y;
			float radiusX = ellipseWidth/2f;
			float radiusY = ellipseHeight/2f;
			float rrx = radiusX*radiusX;
			float rry = radiusY*radiusY;
			// instead of rotating the ellipse, we rotate the line to the opposite side and then rotate intersection points back
			if (ellipseAngleDeg != 0) {
				RotatePoint(centerX, centerY, -ellipseAngleDeg, ref x1, ref y1);
				RotatePoint(centerX, centerY, -ellipseAngleDeg, ref x2, ref y2);
			}
			int dXLine = x2 - x1;
			int dYLine = y2 - y1;
			int dXCenter = x1 - centerX;
			int dYCenter = y1 - centerY;
			float a = (((float) dXLine*(float) dXLine)/rrx) + ((float) dYLine*(float) dYLine/rry);
			float b = (((float) dXLine*(float) dXCenter)/rrx) + ((float) dYLine*(float) dYCenter/rry);
			float c = (((float) dXCenter*(float) dXCenter)/rrx) + ((float) dYCenter*(float) dYCenter/rry);
			float d = b*b - a*(c - 1);
			if (d >= 0) {
				float rd = (float) Math.Sqrt(d);
				float u1 = (-b - rd)/a;
				float u2 = (-b + rd)/a;
				if (!float.IsNaN(u1) && (0 <= u1 && u1 <= 1 || !isSegment)) {
					x = (float) (x1 + dXLine*u1);
					y = (float) (y1 + dYLine*u1);
					if (ellipseAngleDeg != 0) RotatePoint(centerX, centerY, ellipseAngleDeg, ref x, ref y);
					result.X = (int) Math.Round(x);
					result.Y = (int) Math.Round(y);
					yield return result;
				}
				if (!float.IsNaN(u2) && (0 <= u2 && u2 <= 1 || !isSegment)) {
					x = (float) (x1 + dXLine*u2);
					y = (float) (y1 + dYLine*u2);
					if (ellipseAngleDeg != 0) RotatePoint(centerX, centerY, ellipseAngleDeg, ref x, ref y);
					result.X = (int) Math.Round(x);
					result.Y = (int) Math.Round(y);
					yield return result;
				}
			}
		}


		/// <summary>
		/// Calculates the intersection points of the line segment from p1 to p2 with the ellipse.
		/// </summary>
		/// <param name="centerX">X coordinate of the ellipse center point.</param>
		/// <param name="centerY">Y coordinate of the ellipse center point.</param>
		/// <param name="ellipseWidth">Width of the ellipse.</param>
		/// <param name="ellipseHeight">Height of the ellipse.</param>
		/// <param name="ellipseAngleDeg">Rotation angle of the ellipse.</param>
		/// <param name="x1">X coordinate of the line segments first point.</param>
		/// <param name="y1">Y coordinate of the line segments first point.</param>
		/// <param name="x2">X coordinate of the line segments second point.</param>
		/// <param name="y2">Y coordinate of the line segments second point.</param>
		/// <param name="isSegment">Specifies whether the tested line should be treated as line or as line segment.</param>
		/// <returns>Intersection points of the line with the ellipse.</returns>		
		public static IEnumerable<PointF> IntersectEllipseLine(float centerX, float centerY, float ellipseWidth,
		                                                       float ellipseHeight, float ellipseAngleDeg, float x1, float y1,
		                                                       float x2, float y2, bool isSegment)
		{
			PointF result = PointF.Empty;
			float x, y;
			float radiusX = ellipseWidth/2f;
			float radiusY = ellipseHeight/2f;
			float rrx = radiusX*radiusX;
			float rry = radiusY*radiusY;
			// Instead of rotating the ellipse, we rotate the line to the opposite side and then rotate intersection points back
			if (ellipseAngleDeg != 0) {
				RotatePoint(centerX, centerY, -ellipseAngleDeg, ref x1, ref y1);
				RotatePoint(centerX, centerY, -ellipseAngleDeg, ref x2, ref y2);
			}
			float dXLine = x2 - x1;
			float dYLine = y2 - y1;
			float dXCenter = x1 - centerX;
			float dYCenter = y1 - centerY;
			double a = ((dXLine*dXLine)/(double) rrx) + (dYLine*dYLine/(double) rry);
			double b = ((dXLine*dXCenter)/(double) rrx) + (dYLine*dYCenter/(double) rry);
			double c = ((dXCenter*dXCenter)/(double) rrx) + (dYCenter*dYCenter/(double) rry);
			double d = b*b - a*(c - 1);
			if (d >= 0) {
				double rd = Math.Sqrt(d);
				double u1 = (-b - rd)/a;
				double u2 = (-b + rd)/a;
				if (!double.IsNaN(u1) && (0 <= u1 && u1 <= 1 || !isSegment)) {
					x = (float) (x1 + dXLine*u1);
					y = (float) (y1 + dYLine*u1);
					if (ellipseAngleDeg != 0) RotatePoint(centerX, centerY, ellipseAngleDeg, ref x, ref y);
					result.X = x;
					result.Y = y;
					yield return result;
				}
				if (!double.IsNaN(u2) && (0 <= u2 && u2 <= 1 || !isSegment)) {
					x = (float) (x1 + dXLine*u2);
					y = (float) (y1 + dYLine*u2);
					if (ellipseAngleDeg != 0) RotatePoint(centerX, centerY, ellipseAngleDeg, ref x, ref y);
					result.X = x;
					result.Y = y;
					yield return result;
				}
			}
		}


		/// <summary>
		/// Calculates the intersection points of the line segment from p1 to p2 with the (rotated) rectangle.
		/// </summary>
		public static IEnumerable<Point> IntersectRectangleLine(Rectangle rectangle, float angleDeg, Point p1, Point p2,
		                                                        bool isSegment)
		{
			// Calc unrotated but translated rectangle
			float centerX = rectangle.X + (rectangle.Width/2f);
			float centerY = rectangle.Y + (rectangle.Height/2f);

			PointF tl = RotatePoint(centerX, centerY, angleDeg, rectangle.Left, rectangle.Top);
			PointF tr = RotatePoint(centerX, centerY, angleDeg, rectangle.Right, rectangle.Top);
			PointF bl = RotatePoint(centerX, centerY, angleDeg, rectangle.Left, rectangle.Bottom);
			PointF br = RotatePoint(centerX, centerY, angleDeg, rectangle.Right, rectangle.Bottom);
			if (isSegment) {
				PointF p;
				p = IntersectLineWithLineSegment(p1.X, p1.Y, p2.X, p2.Y, tl.X, tl.Y, tr.X, tr.Y);
				if (IsValid(p)) yield return Point.Round(p);
				p = IntersectLineWithLineSegment(p1.X, p1.Y, p2.X, p2.Y, tr.X, tr.Y, br.X, br.Y);
				if (IsValid(p)) yield return Point.Round(p);
				p = IntersectLineWithLineSegment(p1.X, p1.Y, p2.X, p2.Y, bl.X, bl.Y, br.X, br.Y);
				if (IsValid(p)) yield return Point.Round(p);
				p = IntersectLineWithLineSegment(p1.X, p1.Y, p2.X, p2.Y, tl.X, tl.Y, br.X, br.Y);
				if (IsValid(p)) yield return Point.Round(p);
			}
			else {
				PointF p;
				p = IntersectLineSegments(p1.X, p1.Y, p2.X, p2.Y, tl.X, tl.Y, tr.X, tr.Y);
				if (IsValid(p)) yield return Point.Round(p);
				p = IntersectLineSegments(p1.X, p1.Y, p2.X, p2.Y, tr.X, tr.Y, br.X, br.Y);
				if (IsValid(p)) yield return Point.Round(p);
				p = IntersectLineSegments(p1.X, p1.Y, p2.X, p2.Y, bl.X, bl.Y, br.X, br.Y);
				if (IsValid(p)) yield return Point.Round(p);
				p = IntersectLineSegments(p1.X, p1.Y, p2.X, p2.Y, tl.X, tl.Y, br.X, br.Y);
				if (IsValid(p)) yield return Point.Round(p);
			}
		}

		#endregion

		#region Distance calculation functions

		/// <summary>
		/// Calculate the distance between the two points {x1 | y1} and {x2 | y2}
		/// </summary>
		public static float DistancePointPoint(int x1, int y1, int x2, int y2)
		{
			int d1 = x1 - x2;
			int d2 = y1 - y2;
			return (float) Math.Sqrt((float) d1*(float) d1 + (float) d2*(float) d2);
		}


		/// <summary>
		/// Calculate the distance between the two points a and b
		/// </summary>
		public static float DistancePointPoint(Point a, Point b)
		{
			int d1 = a.X - b.X;
			int d2 = a.Y - b.Y;
			return (float) Math.Sqrt((float) d1*(float) d1 + (float) d2*(float) d2);
		}


		/// <summary>
		/// Calculate the distance between the two points a and b
		/// </summary>
		public static float DistancePointPoint(float x1, float y1, float x2, float y2)
		{
			float d1 = x1 - x2;
			float d2 = y1 - y2;
			return (float) Math.Sqrt(d1*d1 + d2*d2);
		}


		/// <summary>
		/// Calculate the distance between the two points a and b
		/// </summary>
		public static float DistancePointPoint(PointF a, PointF b)
		{
			float d1 = a.X - b.X;
			float d2 = a.Y - b.Y;
			return (float) Math.Sqrt(d1*d1 + d2*d2);
		}


		/// <summary>
		/// Calculates the approximate distance between the two points {x1 | y1} and {x2 | y2}
		/// </summary>
		public static int DistancePointPointFast(int x1, int y1, int x2, int y2)
		{
			int dx = x2 - x1;
			int dy = y2 - y1;
			int min, max;
			if (dx < 0) dx = -dx;
			if (dy < 0) dy = -dy;

			if (dx < dy) {
				min = dx;
				max = dy;
			}
			else {
				min = dy;
				max = dx;
			}
			// coefficients equivalent to ( 123/128 * max ) and ( 51/128 * min )
			int result = (((max << 8) + (max << 3) - (max << 4) - (max << 1) +
			               (min << 7) - (min << 5) + (min << 3) - (min << 1)) >> 8);
			return result;
		}


		/// <summary>
		/// Calculates the approximate distance between the two points {x1 | y1} and {x2 | y2}
		/// </summary>
		public static int DistancePointPointFast(int x, int y, Point p)
		{
			return DistancePointPointFast(x, y, p.X, p.Y);
		}


		/// <summary>
		/// Calculates the approximate distance betwenn the two points.
		/// </summary>
		public static int DistancePointPointFast(Point a, Point b)
		{
			int dx = b.X - a.X;
			int dy = b.Y - a.Y;
			int min, max;
			if (dx < 0) dx = -dx;
			if (dy < 0) dy = -dy;
			if (dx < dy) {
				min = dx;
				max = dy;
			}
			else {
				min = dy;
				max = dx;
			}
			// coefficients equivalent to ( 123/128 * max ) and ( 51/128 * min )
			return (((max << 8) + (max << 3) - (max << 4) - (max << 1) +
			         (min << 7) - (min << 5) + (min << 3) - (min << 1)) >> 8);
		}


		/// <summary>
		/// Calculates the distance of point p from the line through a and b. The result can be positive or negative.
		/// </summary>
		public static float DistancePointLine2(Point p, Point a, Point b)
		{
			return DistancePointLine2(p.X, p.Y, a.X, a.Y, b.X, b.Y);
		}


		/// <summary>
		/// Calculates the distance of point p from the line through a and b. The result can be positive or negative.
		/// </summary>
		public static float DistancePointLine2(int pX, int pY, int aX, int aY, int bX, int bY)
		{
			return DistancePointPoint(pX, pY, aX, aY)*(float) Math.Sin(Angle(aX, aY, pX, pY, bX, bY));
		}


		/// <summary>
		/// Berechnet den Abstand des Punktes p von der Geraden linePt1 - linePt2
		/// </summary>
		public static float DistancePointLine2(float pX, float pY, float aX, float aY, float bX, float bY)
		{
			return DistancePointPoint(pX, pY, aX, aY)*(float) Math.Sin(Angle(aX, aY, pX, pY, bX, bY));
		}


		/// <summary>
		/// Calculates the distance of point p from the line segment a to b. The result is always >= 0.
		/// </summary>
		public static float DistancePointLineSegment2(Point p, Point a, Point b)
		{
			float result;
			// Liegt der Lotpunkt auf der Strecke?
			// Abstand des Lotpunktes von linePt1 in Richtung linePt2
			float d = DistancePointPoint(p, a)*(float) Math.Cos(Angle(a, p, b));
			// Falls ja ist der Abstand gleich dem Abstand von der Geraden
			// Falls nein ist der Abstand der zum näher liegenden Strecken-Endpunkt.
			if (d < 0)
				result = DistancePointPoint(p, a);
			else if (d > DistancePointPoint(b, a))
				result = DistancePointPoint(p, b);
			else
				result = Math.Abs(DistancePointLine2(p, a, b));
			return result;
		}


		/// <summary>
		/// Calculates the distance of point p from the line segment a to b. The result is always >= 0.
		/// </summary>
		public static float DistancePointLineLineSegment2(int pX, int pY, int aX, int aY, int bX, int bY)
		{
			float result;
			// Liegt der Lotpunkt auf der Strecke?
			// Abstand des Lotpunktes von linePt1 in Richtung linePt2
			float d = DistancePointPoint(pX, pY, aX, aY)*(float) Math.Cos(Angle(aX, aY, pX, pY, bX, bY));
			// Falls ja ist der Abstand gleich dem Abstand von der Geraden
			// Falls nein ist der Abstand der zum näher liegenden Strecken-Endpunkt.
			if (d < 0)
				result = DistancePointPoint(pX, pY, aX, aY);
			else if (d > DistancePointPoint(bX, bY, aX, aY))
				result = DistancePointPoint(pX, pY, bX, bY);
			else
				result = Math.Abs(DistancePointLine2(pX, pY, aX, aY, bX, bY));
			return result;
		}


		/// <summary>
		/// Calculate the distance from p to ab. 
		/// If isSegment is true, ab is not a line but a line segment. This also means that the calculated value is always >= 0.
		/// </summary>
		public static float DistancePointLine(Point p, Point a, Point b, bool isSegment)
		{
			return DistancePointLine(p.X, p.Y, a.X, a.Y, b.X, b.Y, isSegment);
		}


		/// <summary>
		/// Calculate the distance from p to ab. 
		/// If isSegment is true, ab is not a line but a line segment. This also means that the calculated value is always >= 0.
		/// </summary>
		public static float DistancePointLine(int pX, int pY, int aX, int aY, int bX, int bY, bool isSegment)
		{
			if ((pX == aX && pY == aY) || (pX == bX && pY == bY)) return 0;
			if (isSegment) {
				float l = DistancePointPoint(aX, aY, bX, bY);
				if (l == 0) return DistancePointPoint(pX, pY, aX, aY);
			}
			// Vector cross product is the size of the parallelogram A, B, P, A'. Dived by the length of 
			// the base line, the result is the requested distance.
			float dist = VectorCrossProduct(aX, aY, bX, bY, pX, pY)/DistancePointPoint(aX, aY, bX, bY);
			if (!isSegment) return dist;
			else {
				// If one of the angles is larger than 90 degree, there is no perpendicular and we must calculate
				// the distance to the nearer point of the two.
				int dot1 = VectorDotProduct(aX, aY, bX, bY, pX, pY);
				if (dot1 > 0) return DistancePointPoint(bX, bY, pX, pY);
				int dot2 = VectorDotProduct(bX, bY, aX, aY, pX, pY);
				if (dot2 > 0) return DistancePointPoint(aX, aY, pX, pY);
				return Math.Abs(dist);
			}
		}


		/// <summary>
		/// Calculate the distance from p to ab. 
		/// If isSegment is true, ab is not a line but a line segment. This also means that the calculated value is always >= 0.
		/// </summary>
		public static float DistancePointLine(PointF p, PointF a, PointF b, bool isSegment)
		{
			if (p == a || p == b)
				return 0;
			float dist = VectorCrossProduct(a, b, p)/DistancePointPoint(a, b);
			if (isSegment) return dist;
			else {
				float dot1 = VectorDotProduct(a, b, p);
				if (dot1 > 0) return DistancePointPoint(b, p);
				float dot2 = VectorDotProduct(b, a, p);
				if (dot2 > 0) return DistancePointPoint(a, p);
				return Math.Abs(dist);
			}
		}


		///<summary>
		///Calculate the distance from p to ab. If isSegment is true, ab is not not a line but a line segment.
		///If isSegment is true, ab is not not a line but a line segment. This also means that the calculated value is always >= 0.
		///</summary>
		public static float DistancePointLine(float pX, float pY, float aX, float aY, float bX, float bY, bool isSegment)
		{
			if ((pX == aX && pY == aY) || (pX == bX && pY == bY))
				return 0;
			float dist = VectorCrossProduct(aX, aY, bX, bY, pX, pY)/DistancePointPoint(aX, aY, bX, bY);
			if (isSegment) return dist;
			else {
				float dot1 = VectorDotProduct(aX, aY, bX, bY, pX, pY);
				if (dot1 > 0) return DistancePointPoint(bX, bY, pX, pY);
				float dot2 = VectorDotProduct(bX, bY, aX, aY, pX, pY);
				if (dot2 > 0) return DistancePointPoint(aX, aY, pX, pY);
				return Math.Abs(dist);
			}
		}

		#endregion

		#region Rotation and angle calculation functions

		/// <summary>
		/// Berechnet den Winkel zwischen x1,y1 und x2,y2 mit Scheitel x0,y0 in Radians
		/// </summary>
		public static float Angle(int x0, int y0, int x1, int y1, int x2, int y2)
		{
			return (float) (Math.Atan2(x1 - x0, y1 - y0) - Math.Atan2(x2 - x0, y2 - y0));
		}


		/// <summary>
		/// Berechnet den Winkel zwischen x1,y1 und x2,y2 mit Scheitel x0,y0 in Radians
		/// </summary>
		public static float Angle(float x0, float y0, float x1, float y1, float x2, float y2)
		{
			return (float) (Math.Atan2(x1 - x0, y1 - y0) - Math.Atan2(x2 - x0, y2 - y0));
		}


		/// <summary>
		/// Berechnet den Winkel zwischen x1,y1 und x2,y2 mit Scheitel x0,y0 in Radians
		/// </summary>
		public static float Angle(Point vertex, Point p1, Point p2)
		{
			return Angle(vertex.X, vertex.Y, p1.X, p1.Y, p2.X, p2.Y);
		}


		/// <summary>
		/// Berechnet den Winkel (in Radians) von linePt1 zum Scheitelpunkt center.
		/// </summary>
		public static float Angle(Point p0, Point p1)
		{
			return (float) Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
		}


		/// <summary>
		/// Berechnet den Winkel zwischen x1,y1 und x2,y2 mit Scheitel x0,y0 in Radians
		/// </summary>
		public static float Angle(PointF vertex, PointF p1, PointF p2)
		{
			return Angle(vertex.X, vertex.Y, p1.X, p1.Y, p2.X, p2.Y);
		}


		/// <summary>
		/// Calculates the angle (in radians) between p1 and the center point p0.
		/// </summary>
		public static float Angle(PointF p0, PointF p1)
		{
			return (float) Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
		}


		/// <summary>
		/// Calculates the angle (in radians) between p1 and the center point p0.
		/// </summary>
		public static float Angle(int p0x, int p0y, int p1x, int p1y)
		{
			return (float) Math.Atan2(p1y - p0y, p1x - p0x);
		}


		/// <summary>
		/// Calculates the angle (in radians) between p1 and the center point p0.
		/// </summary>
		public static float Angle(float p0x, float p0y, float p1x, float p1y)
		{
			return (float) Math.Atan2(p1y - p0y, p1x - p0x);
		}


		/// <summary>
		/// Performas a rotation of a point around a center point.
		/// </summary>
		public static PointF RotatePoint(PointF rotationCenter, float angleDeg, PointF point)
		{
			return RotatePoint(rotationCenter.X, rotationCenter.Y, angleDeg, point);
		}


		/// <summary>
		/// Performas a rotation of a point around a center point.
		/// </summary>
		public static PointF RotatePoint(float rotationCenterX, float rotationCenterY, float angleDeg, PointF point)
		{
			double a = angleDeg*RadiansFactor;
			double cos = Math.Cos(a);
			double sin = Math.Sin(a);

			point.X = point.X - rotationCenterX;
			point.Y = point.Y - rotationCenterY;
			double dX = (point.X*cos) - (point.Y*sin);
			double dY = (point.X*sin) + (point.Y*cos);
			point.X = (float) (dX + rotationCenterX);
			point.Y = (float) (dY + rotationCenterY);

			return point;
		}


		/// <summary>
		/// Rotate the given Point around the given center
		/// </summary>
		public static Point RotatePoint(Point rotationCenter, float angleDeg, Point point)
		{
			return RotatePoint(rotationCenter.X, rotationCenter.Y, angleDeg, point);
		}


		/// <summary>
		/// Rotate the given Point around the given center
		/// </summary>
		public static Point RotatePoint(int rotationCenterX, int rotationCenterY, float angleDeg, int x, int y)
		{
			Point p = Point.Empty;
			p.Offset(x, y);
			return RotatePoint(rotationCenterX, rotationCenterY, angleDeg, p);
		}


		/// <summary>
		/// Rotate the given Point around the given center
		/// </summary>
		public static PointF RotatePoint(float rotationCenterX, float rotationCenterY, float angleDeg, float x, float y)
		{
			PointF p = PointF.Empty;
			p.X = x;
			p.Y = y;
			return RotatePoint(rotationCenterX, rotationCenterY, angleDeg, p);
		}


		/// <summary>
		/// Rotate the given Point around the given center
		/// </summary>
		public static Point RotatePoint(int rotationCenterX, int rotationCenterY, float angleDeg, Point point)
		{
			double a = angleDeg*RadiansFactor;
			double cos = Math.Cos(a);
			double sin = Math.Sin(a);

			point.X = point.X - rotationCenterX;
			point.Y = point.Y - rotationCenterY;
			int dX = (int) Math.Round((point.X*cos) - (point.Y*sin));
			int dY = (int) Math.Round((point.X*sin) + (point.Y*cos));
			point.X = dX + rotationCenterX;
			point.Y = dY + rotationCenterY;

			return point;
		}


		/// <summary>
		/// Rotate the given Point around the given center
		/// </summary>
		public static void RotatePoint(float rotationCenterX, float rotationCenterY, float angleDeg, ref float x, ref float y)
		{
			double a = angleDeg*RadiansFactor;
			double cos = Math.Cos(a);
			double sin = Math.Sin(a);

			x = x - rotationCenterX;
			y = y - rotationCenterY;
			double dX = (x*cos) - (y*sin);
			double dY = (x*sin) + (y*cos);
			x = (float) (dX + rotationCenterX);
			y = (float) (dY + rotationCenterY);
		}


		/// <summary>
		/// Rotate the given Point around the given center
		/// </summary>
		public static void RotatePoint(int rotationCenterX, int rotationCenterY, float angleDeg, ref int x, ref int y)
		{
			double a = angleDeg*RadiansFactor;
			double cos = Math.Cos(a);
			double sin = Math.Sin(a);

			x = x - rotationCenterX;
			y = y - rotationCenterY;
			int dX = (int) Math.Round((x*cos) - (y*sin));
			int dY = (int) Math.Round((x*sin) + (y*cos));
			x = dX + rotationCenterX;
			y = dY + rotationCenterY;
		}


		/// <summary>
		/// Rotates the given line around the given center
		/// </summary>
		public static void RotateLine(Point center, float angleDeg, ref Point a, ref Point b)
		{
			RotateLine(center.X, center.Y, angleDeg, ref a, ref b);
		}


		/// <summary>
		/// Rotates the given line around the given center
		/// </summary>
		public static void RotateLine(int centerX, int centerY, float angleDeg, ref Point a, ref Point b)
		{
			a = RotatePoint(centerX, centerY, angleDeg, a);
			b = RotatePoint(centerX, centerY, angleDeg, b);
		}


		/// <summary>
		/// Rotates the given rectangle around the given center
		/// </summary>
		/// <param name="rectangle">Rectangle to rotate</param>
		/// <param name="rotationCenter">Center of rotation</param>
		/// <param name="angleDeg">Rotation angle</param>
		/// <param name="topLeft">Rotated top left corner of the rectangle</param>
		/// <param name="topRight">Rotated top right corner of the rectangle</param>
		/// <param name="bottomRight">Rotated bottom right corner of the rectangle</param>
		/// <param name="bottomLeft">Rotated bottom left corner of the rectangle</param>
		public static void RotateRectangle(Rectangle rectangle, Point rotationCenter, float angleDeg, out Point topLeft,
		                                   out Point topRight, out Point bottomRight, out Point bottomLeft)
		{
			RotateRectangle(rectangle, rotationCenter.X, rotationCenter.Y, angleDeg, out topLeft, out topRight, out bottomRight,
			                out bottomLeft);
		}


		/// <summary>
		/// Rotates the given rectangle around the given center
		/// </summary>
		/// <param name="rectangle">Rectangle to rotate</param>
		/// <param name="rotationCenterX">X coordinate of the rotation center</param>
		/// <param name="rotationCenterY">Y coordinate of the rotation center</param>
		/// <param name="angleDeg">Rotation angle</param>
		/// <param name="topLeft">Rotated top left corner of the rectangle</param>
		/// <param name="topRight">Rotated top right corner of the rectangle</param>
		/// <param name="bottomRight">Rotated bottom right corner of the rectangle</param>
		/// <param name="bottomLeft">Rotated bottom left corner of the rectangle</param>
		public static void RotateRectangle(Rectangle rectangle, int rotationCenterX, int rotationCenterY, float angleDeg,
		                                   out Point topLeft, out Point topRight, out Point bottomRight, out Point bottomLeft)
		{
			topLeft = RotatePoint(rotationCenterX, rotationCenterY, angleDeg, rectangle.Left, rectangle.Top);
			topRight = RotatePoint(rotationCenterX, rotationCenterY, angleDeg, rectangle.Right, rectangle.Top);
			bottomRight = RotatePoint(rotationCenterX, rotationCenterY, angleDeg, rectangle.Right, rectangle.Bottom);
			bottomLeft = RotatePoint(rotationCenterX, rotationCenterY, angleDeg, rectangle.Left, rectangle.Bottom);
		}


		/// <summary>
		/// Rotates the given rectangle around the given center
		/// </summary>
		/// <param name="rectangle">Rectangle to rotate</param>
		/// <param name="rotationCenter">Center of rotation</param>
		/// <param name="angleDeg">Rotation angle</param>
		/// <param name="topLeft">Rotated top left corner of the rectangle</param>
		/// <param name="topRight">Rotated top right corner of the rectangle</param>
		/// <param name="bottomRight">Rotated bottom right corner of the rectangle</param>
		/// <param name="bottomLeft">Rotated bottom left corner of the rectangle</param>
		public static void RotateRectangle(RectangleF rectangle, PointF rotationCenter, float angleDeg, out PointF topLeft,
		                                   out PointF topRight, out PointF bottomRight, out PointF bottomLeft)
		{
			RotateRectangle(rectangle, rotationCenter.X, rotationCenter.Y, angleDeg, out topLeft, out topRight, out bottomRight,
			                out bottomLeft);
		}


		/// <summary>
		/// Rotates the given rectangle around the given center
		/// </summary>
		/// <param name="rectangle">Rectangle to rotate</param>
		/// <param name="rotationCenterX">X coordinate of the rotation center</param>
		/// <param name="rotationCenterY">Y coordinate of the rotation center</param>
		/// <param name="angleDeg">Rotation angle</param>
		/// <param name="topLeft">Rotated top left corner of the rectangle</param>
		/// <param name="topRight">Rotated top right corner of the rectangle</param>
		/// <param name="bottomRight">Rotated bottom right corner of the rectangle</param>
		/// <param name="bottomLeft">Rotated bottom left corner of the rectangle</param>
		public static void RotateRectangle(RectangleF rectangle, float rotationCenterX, float rotationCenterY, float angleDeg,
		                                   out PointF topLeft, out PointF topRight, out PointF bottomRight,
		                                   out PointF bottomLeft)
		{
			topLeft = RotatePoint(rotationCenterX, rotationCenterY, angleDeg, rectangle.Left, rectangle.Top);
			topRight = RotatePoint(rotationCenterX, rotationCenterY, angleDeg, rectangle.Right, rectangle.Top);
			bottomRight = RotatePoint(rotationCenterX, rotationCenterY, angleDeg, rectangle.Right, rectangle.Bottom);
			bottomLeft = RotatePoint(rotationCenterX, rotationCenterY, angleDeg, rectangle.Left, rectangle.Bottom);
		}


		/// <summary>
		/// Translates and rotates the given caption bounds.
		/// </summary>
		/// <param name="captionCenter">Coordinates of the caption's center.</param>
		/// <param name="angle">Rotation angle in tenths of degree.</param>
		/// <param name="captionBounds">Untransformed caption bounds (relative to origin of coordinates).</param>
		/// <param name="topLeft">Transformed top left corner of the given caption bounds.</param>
		/// <param name="topRight">Transformed top right corner of the given caption bounds.</param>
		/// <param name="bottomRight">Transformed bottom right corner of the given caption bounds.</param>
		/// <param name="bottomLeft">Transformed bottom left corner of the given caption bounds.</param>
		public static void TransformRectangle(Point captionCenter, int angle, Rectangle captionBounds,
		                                      out Point topLeft, out Point topRight, out Point bottomRight,
		                                      out Point bottomLeft)
		{
			// translate text bounds
			captionBounds.Offset(captionCenter.X, captionCenter.Y);
			// rotate text bounds
			RotateRectangle(captionBounds, captionCenter, Geometry.TenthsOfDegreeToDegrees(angle),
			                out topLeft, out topRight, out bottomRight, out bottomLeft);
		}


		/// <summary>
		/// Translates and rotates the given caption bounds.
		/// </summary>
		/// <param name="captionCenter">Coordinates of the caption's center.</param>
		/// <param name="rotationCenter">Coordinates of the rotation center.</param>
		/// <param name="angle">Rotation angle in tenths of degree.</param>
		/// <param name="captionBounds">Untransformed caption bounds (relative to origin of coordinates).</param>
		/// <param name="topLeft">Transformed top left corner of the given caption bounds.</param>
		/// <param name="topRight">Transformed top right corner of the given caption bounds.</param>
		/// <param name="bottomRight">Transformed bottom right corner of the given caption bounds.</param>
		/// <param name="bottomLeft">Transformed bottom left corner of the given caption bounds.</param>
		public static void TransformRectangle(Point captionCenter, Point rotationCenter, int angle, Rectangle captionBounds,
		                                      out Point topLeft, out Point topRight, out Point bottomRight,
		                                      out Point bottomLeft)
		{
			// translate text bounds
			captionBounds.Offset(captionCenter.X, captionCenter.Y);
			// rotate text bounds
			RotateRectangle(captionBounds, rotationCenter, Geometry.TenthsOfDegreeToDegrees(angle),
			                out topLeft, out topRight, out bottomRight, out bottomLeft);
		}


		/// <summary>
		/// Translates and rotates the given caption bounds.
		/// </summary>
		/// <param name="captionCenter">Coordinates of the caption's center.</param>
		/// <param name="angle">Rotation angle in tenths of degree.</param>
		/// <param name="captionBounds">Untransformed caption bounds (relative to origin of coordinates).</param>
		/// <param name="topLeft">Transformed top left corner of the given caption bounds.</param>
		/// <param name="topRight">Transformed top right corner of the given caption bounds.</param>
		/// <param name="bottomRight">Transformed bottom right corner of the given caption bounds.</param>
		/// <param name="bottomLeft">Transformed bottom left corner of the given caption bounds.</param>
		public static void TransformRectangle(Point captionCenter, int angle, Rectangle captionBounds,
		                                      out PointF topLeft, out PointF topRight, out PointF bottomRight,
		                                      out PointF bottomLeft)
		{
			// translate text bounds
			captionBounds.Offset(captionCenter.X, captionCenter.Y);
			// rotate text bounds
			RotateRectangle(captionBounds, captionCenter, Geometry.TenthsOfDegreeToDegrees(angle),
			                out topLeft, out topRight, out bottomRight, out bottomLeft);
		}


		/// <summary>
		/// Translates and rotates the given caption bounds.
		/// </summary>
		/// <param name="captionCenter">Coordinates of the caption's center.</param>
		/// <param name="rotationCenter">Coordinates of the rotation center.</param>
		/// <param name="angle">Rotation angle in tenths of degree.</param>
		/// <param name="captionBounds">Untransformed caption bounds (relative to origin of coordinates).</param>
		/// <param name="topLeft">Transformed top left corner of the given caption bounds.</param>
		/// <param name="topRight">Transformed top right corner of the given caption bounds.</param>
		/// <param name="bottomRight">Transformed bottom right corner of the given caption bounds.</param>
		/// <param name="bottomLeft">Transformed bottom left corner of the given caption bounds.</param>
		public static void TransformRectangle(Point captionCenter, Point rotationCenter, int angle, Rectangle captionBounds,
		                                      out PointF topLeft, out PointF topRight, out PointF bottomRight,
		                                      out PointF bottomLeft)
		{
			// translate text bounds
			captionBounds.Offset(captionCenter.X, captionCenter.Y);
			// rotate text bounds
			RotateRectangle(captionBounds, rotationCenter, Geometry.TenthsOfDegreeToDegrees(angle),
			                out topLeft, out topRight, out bottomRight, out bottomLeft);
		}


		/// <summary>
		/// Converts and angle measured in tenths of degrees to an angle measured in radians.
		/// </summary>
		/// <param name="angle">Angle measured in tenths of degree.</param>
		/// <returns>Angle measured in radians</returns>
		public static float TenthsOfDegreeToRadians(int angle)
		{
			return (float) (angle*(RadiansFactor/10));
		}


		/// <summary>
		/// Converts and angle measured in tenths of degrees to an angle measured in degrees.
		/// </summary>
		/// <param name="angle">Angle measured in tenths of degree.</param>
		/// <returns>Angle measured in degrees</returns>
		public static float TenthsOfDegreeToDegrees(int angle)
		{
			return angle/10f;
		}


		/// <summary>
		/// Converts and angle measured in degrees to an angle measured in radians.
		/// </summary>
		/// <param name="angle">Angle measured in degrees</param>
		/// <returns>Angle measured in radians</returns>
		public static float DegreesToRadians(float angle)
		{
			return (float) (angle*RadiansFactor);
		}


		/// <summary>
		/// Converts and angle measured in degrees to an angle measured in tenths of degree.
		/// </summary>
		/// <param name="angle">Angle measured in degrees</param>
		/// <returns>Angle measured in tenths of degree</returns>
		public static int DegreesToTenthsOfDegree(float angle)
		{
			return (int) Math.Round(angle*10);
		}


		/// <summary>
		/// Converts and angle measured in radians to an angle measured in degrees.
		/// </summary>
		/// <param name="angle">Angle measured in radians</param>
		/// <returns>Angle measured in degrees</returns>
		public static float RadiansToDegrees(float angle)
		{
			return (float) (angle/RadiansFactor);
		}


		/// <summary>
		/// Converts and angle measured in radians to an angle measured in tenths of degree.
		/// </summary>
		/// <param name="angle">Angle measured in radians</param>
		/// <returns>Angle measured in tenths of degree</returns>
		public static int RadiansToTenthsOfDegree(float angle)
		{
			return (int) Math.Round(angle/(RadiansFactor/10));
		}

		#endregion

		/// <summary>
		/// Calculates the scale factor that has to be applied in order to fit the source rectangle into the destination rectangle.
		/// </summary>
		public static float CalcScaleFactor(int srcWidth, int srcHeight, int dstWidth, int dstHeight)
		{
			return CalcScaleFactor(srcWidth, srcHeight, dstWidth, dstHeight, true);
		}


		/// <summary>
		/// Calculates the scale factor that has to be applied in order to fit the source rectangle into the destination rectangle.
		/// </summary>
		public static float CalcScaleFactor(float srcWidth, float srcHeight, float dstWidth, float dstHeight)
		{
			return CalcScaleFactor(srcWidth, srcHeight, dstWidth, dstHeight, true);
		}


		/// <summary>
		/// Calculates the scale factor that has to be applied in order to fit the source rectangle into the destination rectangle.
		/// </summary>
		public static float CalcScaleFactor(int srcWidth, int srcHeight, int dstWidth, int dstHeight, bool minimum)
		{
			return CalcScaleFactor((float) srcWidth, (float) srcHeight, (float) dstWidth, (float) dstHeight, minimum);
		}


		/// <summary>
		/// Calculates the scale factor that has to be applied in order to fit the source rectangle into the destination rectangle.
		/// </summary>
		public static float CalcScaleFactor(float srcWidth, float srcHeight, float dstWidth, float dstHeight, bool minimum)
		{
			float scaleX, scaleY;
			CalcScaleFactor(srcWidth, srcHeight, dstWidth, dstHeight, out scaleX, out scaleY);
			return minimum ? Math.Min(scaleX, scaleY) : Math.Max(scaleX, scaleY);
		}


		/// <summary>
		/// Calculates the scale factor that has to be applied in order to fit the source rectangle into the destination rectangle
		/// </summary>
		/// <param name="srcWidth">Width of the source rectangle</param>
		/// <param name="srcHeight">Height of the source rectangle</param>
		/// <param name="dstWidth">Width of the destinalion rectangle</param>
		/// <param name="dstHeight">Height of the destinalion rectangle</param>
		/// <param name="scaleX">Scale factor in X direction</param>
		/// <param name="scaleY">Scale factor in X direction</param>
		public static void CalcScaleFactor(int srcWidth, int srcHeight, int dstWidth, int dstHeight, out float scaleX,
		                                   out float scaleY)
		{
			CalcScaleFactor((float) srcWidth, (float) srcHeight, (float) dstWidth, (float) dstHeight, out scaleX, out scaleY);
		}


		/// <summary>
		/// Calculates the scale factor that has to be applied in order to fit the source rectangle into the destination rectangle
		/// </summary>
		/// <param name="srcWidth">Width of the source rectangle</param>
		/// <param name="srcHeight">Height of the source rectangle</param>
		/// <param name="dstWidth">Width of the destinalion rectangle</param>
		/// <param name="dstHeight">Height of the destinalion rectangle</param>
		/// <param name="scaleX">Scale factor in X direction</param>
		/// <param name="scaleY">Scale factor in X direction</param>
		public static void CalcScaleFactor(float srcWidth, float srcHeight, float dstWidth, float dstHeight, out float scaleX,
		                                   out float scaleY)
		{
			scaleX = (float) dstWidth/(float) (srcWidth != 0 ? srcWidth : 1);
			scaleY = (float) dstHeight/(float) (srcHeight != 0 ? srcHeight : 1);
		}


		/// <summary>
		/// Calculates a point on a cubic bezier curve
		/// </summary>
		/// <param name="A">The first interpolation point of the bezier curve</param>
		/// <param name="B">The second interpolation point of the bezier curve</param>
		/// <param name="C">The third interpolation point of the bezier curve</param>
		/// <param name="D">The fourth interpolation point of the bezier curve</param>
		/// <param name="distance">the distance of the point to calculate</param>
		public static PointF BezierPoint(PointF A, PointF B, PointF C, PointF D, float distance)
		{
			PointF p = Point.Empty;
			int n = 200;
			float a = distance*(1f/n);
			float b = 1.0f - a;
			p.X = A.X*a*a*a + B.X*3*a*a*b + C.X*3*a*b*b + D.X*b*b*b;
			p.Y = A.Y*a*a*a + B.Y*3*a*a*b + C.Y*3*a*b*b + D.Y*b*b*b;
			return p;
		}


		/// <summary>
		/// Calculates the balance point of a polygon
		/// </summary>
		public static PointF CalcPolygonBalancePoint(IEnumerable<PointF> points)
		{
			if (points == null) throw new ArgumentNullException("points");
			PointF result = PointF.Empty;
			int pointCnt = 0;
			foreach (PointF p in points) {
				result.X += p.X;
				result.Y += p.Y;
				++pointCnt;
			}
			result.X = result.X/pointCnt;
			result.Y = result.Y/pointCnt;
			return result;
		}


		/// <summary>
		/// Calculates the balance point of a polygon
		/// </summary>
		public static void CalcPolygonBalancePoint(IEnumerable<PointF> points, out float x, out float y)
		{
			if (points == null) throw new ArgumentNullException("points");
			x = 0;
			y = 0;
			int pointCnt = 0;
			foreach (PointF p in points) {
				x += p.X;
				y += p.Y;
				++pointCnt;
			}
			x = x/pointCnt;
			y = y/pointCnt;
		}


		/// <summary>
		/// Calculates the balance point of a polygon
		/// </summary>
		public static Point CalcPolygonBalancePoint(IEnumerable<Point> points)
		{
			if (points == null) throw new ArgumentNullException("points");
			Point result = Point.Empty;
			int pointCnt = 0;
			foreach (Point p in points) {
				result.Offset(p.X, p.Y);
				++pointCnt;
			}
			result.X = (int) Math.Round(result.X/(float) pointCnt);
			result.Y = (int) Math.Round(result.Y/(float) pointCnt);
			return result;
		}


		/// <summary>
		/// Calculates the balance point of a polygon
		/// </summary>
		public static void CalcPolygonBalancePoint(IEnumerable<Point> points, out int x, out int y)
		{
			if (points == null) throw new ArgumentNullException("points");
			x = 0;
			y = 0;
			int pointCnt = 0;
			foreach (Point p in points) {
				x += p.X;
				y += p.Y;
				++pointCnt;
			}
			x = (int) Math.Round(x/(float) pointCnt);
			y = (int) Math.Round(y/(float) pointCnt);
		}


		/// <summary>
		/// Returns the boundingRectangle of a rotated Point collection. 
		/// The Vertices in the collections are neither copied nor changed.
		/// </summary>
		public static void CalcBoundingRectangle(IEnumerable<Point> points, int rotationCenterX, int rotationCenterY,
		                                         float angleDeg, out Rectangle rectangle)
		{
			if (points == null) throw new ArgumentNullException("points");
			rectangle = Rectangle.Empty;
			int left = int.MaxValue;
			int top = int.MaxValue;
			int right = int.MinValue;
			int bottom = int.MinValue;
			int pX, pY;
			foreach (Point p in points) {
				pX = p.X;
				pY = p.Y;
				if (angleDeg != 0) RotatePoint(rotationCenterX, rotationCenterY, angleDeg, ref pX, ref pY);

				if (pX < left) left = pX;
				if (pX > right) right = pX;
				if (pY < top) top = pY;
				if (pY > bottom) bottom = pY;
			}
			rectangle.X = left;
			rectangle.Y = top;
			rectangle.Width = right - left;
			rectangle.Height = bottom - top;
		}


		/// <summary>
		/// Returns the bounding rectangle of a point array.
		/// </summary>
		/// <param name="points">The point array</param>
		/// <param name="rectangle">The resulting bounding rectangle</param>
		public static void CalcBoundingRectangle(IEnumerable<Point> points, out Rectangle rectangle)
		{
			if (points == null) throw new ArgumentNullException("points");
			rectangle = Rectangle.Empty;
			int left = int.MaxValue;
			int top = int.MaxValue;
			int right = int.MinValue;
			int bottom = int.MinValue;
			foreach (Point p in points) {
				if (p.X < left) left = p.X;
				if (p.X > right) right = p.X;
				if (p.Y < top) top = p.Y;
				if (p.Y > bottom) bottom = p.Y;
			}
			rectangle.X = left;
			rectangle.Y = top;
			rectangle.Width = right - left;
			rectangle.Height = bottom - top;
		}


		/// <summary>
		/// Calculates the axis-aligned bounding rectangle of the given point collection. 
		/// </summary>
		/// <param name="points">Collection of points</param>
		/// <param name="rectangle">The resulting bounding rectangle</param>
		/// <param name="floor">Specifies if PointF values should be floored or ceiled</param>
		public static void CalcBoundingRectangle(IEnumerable<PointF> points, out Rectangle rectangle, bool floor)
		{
			if (points == null) throw new ArgumentNullException("points");
			rectangle = Rectangle.Empty;
			float left = int.MaxValue;
			float top = int.MaxValue;
			float right = int.MinValue;
			float bottom = int.MinValue;
			foreach (PointF p in points) {
				if (p.X < left) left = p.X;
				else if (p.X > right) right = p.X;
				if (p.Y < top) top = p.Y;
				else if (p.Y > bottom) bottom = p.Y;
			}
			rectangle.X = floor ? (int) Math.Ceiling(left) : (int) Math.Floor(left);
			rectangle.Y = floor ? (int) Math.Ceiling(top) : (int) Math.Floor(top);
			rectangle.Width = (floor ? (int) Math.Floor(right) : (int) Math.Ceiling(right)) - rectangle.Left;
			rectangle.Height = (floor ? (int) Math.Floor(bottom) : (int) Math.Ceiling(bottom)) - rectangle.Top;
		}


		/// <summary>
		/// Calculates the axis-aligned bounding rectangle of the given point collection
		/// </summary>
		public static void CalcBoundingRectangle(IEnumerable<PointF> points, out Rectangle rectangle)
		{
			CalcBoundingRectangle(points, out rectangle, false);
		}


		/// <summary>
		/// Calculates the x axis aligned bounding rectangle of a point array.
		/// </summary>
		/// <param name="p1">The top left corner of the rectangle.</param>
		/// <param name="p2">The top right corner of the rectangle.</param>
		/// <param name="p3">The bottom right corner of the rectangle.</param>
		/// <param name="p4">The bottom left corner of the rectangle.</param>
		/// <param name="rectangle">The resulting bounding rectangle</param>
		public static void CalcBoundingRectangle(Point p1, Point p2, Point p3, Point p4, out Rectangle rectangle)
		{
			int left = Math.Min(Math.Min(p1.X, p2.X), Math.Min(p3.X, p4.X));
			int top = Math.Min(Math.Min(p1.Y, p2.Y), Math.Min(p3.Y, p4.Y));
			int right = Math.Max(Math.Max(p1.X, p2.X), Math.Max(p3.X, p4.X));
			int bottom = Math.Max(Math.Max(p1.Y, p2.Y), Math.Max(p3.Y, p4.Y));
			rectangle = Rectangle.Empty;
			rectangle.Offset(left, top);
			rectangle.Width = right - left;
			rectangle.Height = bottom - top;
		}


		/// <summary>
		/// Calculates the x axis aligned bounding rectangle of a point array.
		/// </summary>
		/// <param name="p1">The top left corner of the rectangle.</param>
		/// <param name="p2">The top right corner of the rectangle.</param>
		/// <param name="p3">The bottom right corner of the rectangle.</param>
		/// <param name="p4">The bottom left corner of the rectangle.</param>
		/// <param name="rectangle">The resulting bounding rectangle</param>
		public static void CalcBoundingRectangle(PointF p1, PointF p2, PointF p3, PointF p4, out Rectangle rectangle)
		{
			float left = Math.Min(Math.Min(p1.X, p2.X), Math.Min(p3.X, p4.X));
			float top = Math.Min(Math.Min(p1.Y, p2.Y), Math.Min(p3.Y, p4.Y));
			float right = Math.Max(Math.Max(p1.X, p2.X), Math.Max(p3.X, p4.X));
			float bottom = Math.Max(Math.Max(p1.Y, p2.Y), Math.Max(p3.Y, p4.Y));
			rectangle = Rectangle.Empty;
			rectangle.Offset((int) Math.Floor(left), (int) Math.Floor(top));
			rectangle.Width = (int) Math.Ceiling(right) - rectangle.X;
			rectangle.Height = (int) Math.Ceiling(bottom) - rectangle.Y;
		}


		/// <summary>
		/// Calculates the x axis aligned bounding rectangle of a rotated ellipse.
		/// </summary>
		public static void CalcBoundingRectangleEllipse(int centerX, int centerY, int width, int height, float angleDeg,
		                                                out Rectangle rectangle)
		{
			rectangle = Geometry.InvalidRectangle;
			// a is the major half axis
			// b is the minor half axis
			// phi is the ratation angle of the ellipse
			// t1/t2 are the angles where to find the maxima:
			// The formulas how to calculate the maxima:
			//	   x = centerX + a * cos(t) * cos(phi) - b * sin(t) * sin(phi)  [1]
			//	   y = centerY + b * sin(t) * cos(phi) + a * cos(t) * sin(phi)  [2]
			// The formula how to calculate the angle t:
			//    tan(t) = -b * tan(phi) / a   [3]
			//    tan(t) = b * cot(phi) / a  [4]
			float a = width/2f;
			float b = height/2f;
			float phi = Geometry.DegreesToRadians(angleDeg);
			double tanPhi = Math.Tan(phi);
			double sinPhi = Math.Sin(phi);
			double cosPhi = Math.Cos(phi);
			float t1 = (float) Math.Round(Math.Atan(-b*tanPhi/a), 7, MidpointRounding.ToEven);
			float t2 = (float) Math.Round(Math.Atan(b*(1/tanPhi)/a), 7, MidpointRounding.ToEven);
			double sinT1 = Math.Sin(t1);
			double cosT1 = Math.Cos(t1);
			double sinT2 = Math.Sin(t2);
			double cosT2 = Math.Cos(t2);

			float x1 = (float) Math.Abs(a*cosT1*cosPhi - b*sinT1*sinPhi);
			float x2 = (float) Math.Abs(a*cosT2*cosPhi - b*sinT2*sinPhi);
			float y1 = (float) Math.Abs(b*sinT1*cosPhi + a*cosT1*sinPhi);
			float y2 = (float) Math.Abs(b*sinT2*cosPhi + a*cosT2*sinPhi);

			rectangle.X = (int) Math.Floor(centerX - Math.Max(x1, x2));
			rectangle.Y = (int) Math.Floor(centerY - Math.Max(y1, y2));
			rectangle.Width = (int) Math.Ceiling(centerX + Math.Max(x1, x2)) - rectangle.X;
			rectangle.Height = (int) Math.Ceiling(centerY + Math.Max(y1, y2)) - rectangle.Y;
		}


		/// <summary>
		/// Converts font size in points to font size in pixels
		/// </summary>
		public static int PointToPixel(float sizeInPoints, float dpiY)
		{
			return (int) Math.Ceiling((sizeInPoints/72)*dpiY);
		}


		/// <summary>
		/// Converts font size in points to font size in pixels
		/// </summary>
		public static float PixelToPoint(int sizeInPixel, float dpiY)
		{
			return (sizeInPixel*72)/dpiY;
		}


		/// <summary>
		/// Calculates the coordinates of a point from another point with angle and distance;
		/// </summary>
		public static Point CalcPoint(int x, int y, float angleDeg, float distance)
		{
			return Point.Round(CalcPoint((float) x, (float) y, angleDeg, distance));
		}


		/// <summary>
		/// Calculates the coordinates of a point from another point with angle and distance;
		/// </summary>
		public static PointF CalcPoint(float x, float y, float angleDeg, float distance)
		{
			float angle = DegreesToRadians(angleDeg);
			PointF result = PointF.Empty;
			result.X = (float) (x + distance*Math.Cos(angle));
			result.Y = (float) (y + distance*Math.Sin(angle));
			return result;
		}


		/// <summary>
		/// Calculates the position of the point on the line. 
		/// </summary>
		public static Point CalcPointOnLine(int startX, int startY, int endX, int endY, float distanceFromStart)
		{
			return Point.Round(CalcPointOnLine((float) startX, (float) startY, (float) endX, (float) endY, distanceFromStart));
		}


		/// <summary>
		/// Calculates the position of the point on the line.
		/// </summary>
		public static PointF CalcPointOnLine(float startX, float startY, float endX, float endY, float distanceFromStart)
		{
			//float angle = RadiansToDegrees(Angle(startX, startY, endX, endY));
			//float ptX = startX + distance;
			//float ptY = startY;
			//RotatePoint(startX, startY, angle, ref ptX, ref ptY);
			//PointF result = PointF.Empty;
			//result.X = ptX;
			//result.Y = ptY;
			//return result;
			float dist = DistancePointPoint(startX, startY, endX, endY);
			if (dist == 0) return new PointF(startX, startY);
			return VectorLinearInterpolation(startX, startY, endX, endY, distanceFromStart/dist);
		}


		/// <summary>
		/// Calculates the parameters a, b and c of the line formula ax + by + c = 0 from two given points
		/// </summary>
		public static void CalcLine(Point p1, Point p2, out int a, out int b, out int c)
		{
			CalcLine(p1.X, p1.Y, p2.X, p2.Y, out a, out b, out c);
		}


		/// <summary>
		/// Calculates the parameters a, b and c of the line formula ax + by + c = 0 from two given points.
		/// In order to be compatible with the Hesse normal form, c is always negative, -c is the distance 
		/// of the line from the origin. The vector A-B points from the origin towards the line.
		/// </summary>
		public static void CalcLine(int x1, int y1, int x2, int y2, out int a, out int b, out int c)
		{
			a = y2 - y1;
			b = x2 - x1;
			c = a*x1 - b*y1;
			if (c > 0) {
				b = -b;
				c = -c;
			}
			else a = -a;
		}


		/// <summary>
		/// Calculates the parameters a, b and c of the line formula ax + by + c = 0 from two given points
		/// </summary>
		public static void CalcLine(PointF p1, PointF p2, out float a, out float b, out float c)
		{
			CalcLine(p1.X, p1.Y, p2.X, p2.Y, out a, out b, out c);
		}


		/// <summary>
		/// Calculates the parameters a, b and c of the line formula ax + by + c = 0 from two given points
		/// In order to be compatible with the Hesse normal form, c is always negative, -c is the distance 
		/// of the line from the origin. The vector A-B points from the origin towards the line.
		/// </summary>
		public static void CalcLine(float x1, float y1, float x2, float y2, out float a, out float b, out float c)
		{
			a = y2 - y1;
			b = x2 - x1;
			c = a*x1 - b*y1;
			if (c > 0) {
				b = -b;
				c = -c;
			}
			else a = -a;
		}


		/// <summary>
		/// Calculates the parameters m and c of the line formula y = mx + c from two given points
		/// </summary>
		public static void CalcLine(Point p1, Point p2, out float m, out float c)
		{
			CalcLine(p1.X, p1.Y, p2.X, p2.Y, out m, out c);
		}


		/// <summary>
		/// Calculates the parameters m and c of the line formula y = mx + c from two given points
		/// </summary>
		public static void CalcLine(int x1, int y1, int x2, int y2, out float m, out float c)
		{
			m = 0;
			c = 0;
			int m1 = y1 - y2;
			int m2 = x1 - x2;
			if (m2 != 0)
				m = m1/(float) m2;
			else {
				if (m1 < 0) m = float.NegativeInfinity;
				else m = float.PositiveInfinity;
			}
			c = y1 - (m*x1);
		}


		/// <summary>
		/// Calculates the parameters m and c of the line formula y = mx + c from two given points
		/// </summary>
		public static void CalcLine(float x1, float y1, float x2, float y2, out float m, out float c)
		{
			m = 0;
			c = 0;
			float m1 = y1 - y2;
			float m2 = x1 - x2;
			if (m2 != 0)
				m = m1/m2;
			else {
				if (m1 < 0) m = float.NegativeInfinity;
				else m = float.PositiveInfinity;
			}
			c = y1 - (m*x1);
		}


		/// <summary>
		/// Calculates the center point and the radius of an arc defined by start point, end point and a third point on the arc ('radius point')
		/// </summary>
		[Obsolete("Use Geometry.CalcCircumCircle instead.")]
		public static PointF CalcArcCenterAndRadius(PointF startPt, PointF radiusPt, PointF endPt, out float radius)
		{
			PointF result;
			CalcCircumCircle(startPt.X, startPt.Y, radiusPt.X, radiusPt.Y, endPt.X, endPt.Y, out result, out radius);
			return result;
		}


		/// <summary>
		/// Calculates the center point and the radius of an arc defined by start point, end point and a third point on the arc ('radius point')
		/// </summary>
		[Obsolete("Use Geometry.CalcCircumCircle instead.")]
		public static PointF CalcArcCenterAndRadius(float startPtX, float startPtY, float radiusPtX, float radiusPtY,
		                                            float endPtX, float endPtY, out float radius)
		{
			PointF result;
			CalcCircumCircle(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, out result, out radius);
			return result;
		}


		/// <summary>
		/// Calculates the center point and the radius of an arc defined by start point, end point and a third point on the arc ('radius point')
		/// </summary>
		public static void CalcCircumCircle(PointF startPt, PointF radiusPt, PointF endPt, out PointF center, out float radius)
		{
			CalcCircumCircle(startPt.X, startPt.Y, radiusPt.X, radiusPt.Y, endPt.X, endPt.Y, out center, out radius);
		}


		/// <summary>
		/// Calculates the center point and the radius of an arc defined by start point, end point and a third point on the arc ('radius point')
		/// </summary>
		public static void CalcCircumCircle(float startPtX, float startPtY, float radiusPtX, float radiusPtY, float endPtX,
		                                    float endPtY, out PointF center, out float radius)
		{
			center = Geometry.InvalidPointF;
			radius = 0;

			// Geradengleichung der Kreissehne StartPoint/RadiusPoint
			float a1s, b1s, c1s;
			CalcLine(startPtX, startPtY, radiusPtX, radiusPtY, out a1s, out b1s, out c1s);
			// Geradengleichung der Mittelsenkrechten der Kresissehne StartPoint/RadiusPoint
			float a1m, b1m, c1m;
			CalcPerpendicularBisector(startPtX, startPtY, radiusPtX, radiusPtY, out a1m, out b1m, out c1m);
			// Schnittpunkt der Mittelsenkrechten mit der Kreissehne berechnen berechnen
			float pt1X, pt1Y;
			IntersectLines(a1s, b1s, c1s, a1m, b1m, c1m, out pt1X, out pt1Y);

			// Geradengleichung der Kreissehne EndPoint/RadiusPoint
			float a2s, b2s, c2s;
			CalcLine(endPtX, endPtY, radiusPtX, radiusPtY, out a2s, out b2s, out c2s);
			// Geradengleichung der Mittelsenkrechten der Strecke EndPoint-RadiusPoint
			float a2m, b2m, c2m;
			CalcPerpendicularBisector(endPtX, endPtY, radiusPtX, radiusPtY, out a2m, out b2m, out c2m);
			// Schnittpunkt der Mittelsenkrechten mit der Kreissehne berechnen berechnen
			float pt2X, pt2Y;
			IntersectLines(a2s, b2s, c2s, a2m, b2m, c2m, out pt2X, out pt2Y);

			// Schnittpunkte der Mittelsenkrechten berechnen
			float cX, cY;
			IntersectLines(a1m, b1m, c1m, a2m, b2m, c2m, out cX, out cY);

			radius = DistancePointPoint(cX, cY, radiusPtX, radiusPtY);
			center.X = cX;
			center.Y = cY;
		}


		// Berechnet die Lotrechte durch den Punkt x1/y1 
		// Das Ergebnis ist die Normalform der Geraden ax + by + c = 0
		/// <ToBeCompleted></ToBeCompleted>
		public static void CalcPerpendicularLine(int x1, int y1, int x2, int y2, out int a, out int b, out int c)
		{
			// Steigung
			a = x1 - x2;
			b = y1 - y2;
			// Konstante
			c = -a*x1 - b*y1;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void CalcPerpendicularLine(float x1, float y1, float x2, float y2, out float a, out float b, out float c)
		{
			// Steigung
			a = x1 - x2;
			b = y1 - y2;
			// Konstante
			c = -a*x1 - b*y1;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void CalcPerpendicularLine(int x, int y, int a, int b, int c, out int aP, out int bP, out int cP)
		{
			aP = b;
			bP = -a;
			cP = -(aP*x + bP*y);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void CalcPerpendicularLine(float x, float y, float a, float b, float c, out float aP, out float bP,
		                                         out float cP)
		{
			aP = b;
			bP = -a;
			cP = -(aP*x + bP*y);
		}


		// Berechnet die Gerade (2D), welche im Mittelpunkt zwischen linePt1 und linePt2 senkrecht 
		// auf der Verbindungsstrecke steht. Das Ergebnis ist die Normalform der Geraden
		// ax + by + c = 0
		/// <ToBeCompleted></ToBeCompleted>
		public static void CalcPerpendicularBisector(int x1, int y1, int x2, int y2, out int a, out int b, out int c)
		{
			// Steigung
			a = x1 - x2;
			b = y1 - y2;
			// Konstante
			c = (int) Math.Round((-a*(x1 + x2))/2f - (b*(y1 + y2))/2f);
		}


		// Berechnet die Gerade (2D), welche im Mittelpunkt zwischen linePt1 und linePt2 senkrecht 
		// auf der Verbindungsstrecke steht. Das Ergebnis ist die Normalform der Geraden
		// ax + by + c = 0
		/// <ToBeCompleted></ToBeCompleted>
		public static void CalcPerpendicularBisector(float x1, float y1, float x2, float y2, out float a, out float b,
		                                             out float c)
		{
			// Steigung
			a = x1 - x2;
			b = y1 - y2;
			// Konstante
			c = (-a*(x1 + x2))/2f - (b*(y1 + y2))/2f;
		}


		/// <summary>
		/// Calculates the coordinates of the base point (fX, fY) of the perpendicular from point (pX, pY) 
		/// to line (aX, aY) - (bX, bY).
		/// </summary>
		/// <remarks>The foot is the solution of equation (p - d) . (a - b) = 0</remarks>
		public static void CalcDroppedPerpendicularFoot(int pX, int pY, int aX, int aY, int bX, int bY, out int fX, out int fY)
		{
			int a1, b1, c1;
			// Calculate line formula parameters for the line a - b.
			CalcLine(aX, aY, bX, bY, out a1, out b1, out c1);
			// Calculate perpendicular through p
			int a2, b2, c2;
			a2 = aX - bX;
			b2 = aY - bY;
			c2 = bX*pX - aX*pX + bY*pY - aY*pY;
			// Now intersect the two lines
			IntersectLines(a1, b1, c1, a2, b2, c2, out fX, out fY);
		}


		/// <summary>
		/// Translates the given line in Hesse normal form to point p.
		/// </summary>
		public static void TranslateLine(int a, int b, int c, Point p, out int aT, out int bT, out int cT)
		{
			aT = a;
			bT = b;
			cT = -((a*p.X) + (b*p.Y));
		}


		/// <summary>
		/// Translates the given line in Hesse normal form to point p.
		/// </summary>
		public static void TranslateLine(float a, float b, float c, Point p, out float aT, out float bT, out float cT)
		{
			aT = a;
			bT = b;
			cT = -((a*p.X) + (b*p.Y));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> CalcCircleTangentThroughPoint(float centerX, float centerY, float radius, int ptX,
		                                                                int ptY)
		{
			float distance = (float) Math.Abs(DistancePointPoint(centerX, centerY, ptX, ptY));
			PointF p = VectorLinearInterpolation(centerX, centerY, ptX, ptY, 0.5f);
			return IntersectCircles(centerX, centerY, radius, p.X, p.Y, distance/2f);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<PointF> CalcArcTangentThroughPoint(float startPtX, float startPtY, float radiusPtX,
		                                                             float radiusPtY, float endPtX, float endPtY, int ptX,
		                                                             int ptY)
		{
			float radius;
			PointF center;
			CalcCircumCircle(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, out center, out radius);
			float distance = (float) Math.Abs(DistancePointPoint(center.X, center.Y, ptX, ptY));
			PointF pC = VectorLinearInterpolation(center.X, center.Y, ptX, ptY, 0.5f);
			foreach (PointF pT in IntersectCircles(center.X, center.Y, radius, pC.X, pC.Y, distance/2f)) {
				if (ArcContainsPoint(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, center.X, center.Y, radius, pT.X,
				                     pT.Y, 0.1f))
					yield return pT;
				else {
					PointF result = PointF.Empty;
					float startPtDist = DistancePointPoint(pT.X, pT.Y, startPtX, startPtY);
					float endPtDist = DistancePointPoint(pT.X, pT.Y, endPtX, endPtY);
					if (startPtDist < endPtDist) {
						result.X = startPtX;
						result.Y = startPtY;
						yield return result;
					}
					else if (endPtDist < startPtDist) {
						result.X = endPtX;
						result.Y = endPtY;
						yield return result;
					}
					else {
						startPtDist = DistancePointLine(startPtX, startPtY, pT.X, pT.Y, ptX, ptY, true);
						endPtDist = DistancePointLine(endPtX, endPtY, pT.X, pT.Y, ptX, ptY, true);
						if (startPtDist < endPtDist) {
							result.X = startPtX;
							result.Y = startPtY;
							yield return result;
						}
						else if (endPtDist < startPtDist) {
							result.X = endPtX;
							result.Y = endPtY;
							yield return result;
						}
					}
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Point> CalcArcTangentThroughPoint(int startPtX, int startPtY, int radiusPtX, int radiusPtY,
		                                                            int endPtX, int endPtY, int ptX, int ptY)
		{
			float radius;
			PointF center;
			CalcCircumCircle(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, out center, out radius);
			float distance = Math.Abs(DistancePointPoint(center.X, center.Y, ptX, ptY));
			PointF pC = VectorLinearInterpolation(center.X, center.Y, ptX, ptY, 0.5f);
			foreach (PointF pT in IntersectCircles(center.X, center.Y, radius, pC.X, pC.Y, distance/2f)) {
				if (ArcContainsPoint(startPtX, startPtY, radiusPtX, radiusPtY, endPtX, endPtY, center.X, center.Y, radius, pT.X,
				                     pT.Y, 0.1f))
					yield return Point.Round(pT);
				else {
					Point result = Point.Empty;
					float startPtDist = DistancePointPoint(pT.X, pT.Y, startPtX, startPtY);
					float endPtDist = DistancePointPoint(pT.X, pT.Y, endPtX, endPtY);
					if (startPtDist < endPtDist) {
						result.X = startPtX;
						result.Y = startPtY;
						yield return result;
					}
					else if (endPtDist < startPtDist) {
						result.X = endPtX;
						result.Y = endPtY;
						yield return result;
					}
					else {
						startPtDist = DistancePointLine(startPtX, startPtY, pT.X, pT.Y, ptX, ptY, true);
						endPtDist = DistancePointLine(endPtX, endPtY, pT.X, pT.Y, ptX, ptY, true);
						if (startPtDist < endPtDist) {
							result.X = startPtX;
							result.Y = startPtY;
							yield return result;
						}
						else if (endPtDist < startPtDist) {
							result.X = endPtX;
							result.Y = endPtY;
							yield return result;
						}
					}
				}
			}
		}


		// Löst das lineare Gleichungssystem Ax + b = 0
		/// <ToBeCompleted></ToBeCompleted>
		public static bool SolveLinear22System(int a11, int a12, int a21, int a22, int b1, int b2, out int x, out int y)
		{
			bool result = false;
			x = InvalidPoint.X;
			y = InvalidPoint.Y;
			// if det == 0, there is no solution
			int det = (a11*a22 - a12*a21);
			if (det != 0) {
				result = true;
				x = (int) Math.Round((b2*a12 - b1*a22)/(float) det);
				y = (int) Math.Round((b1*a21 - b2*a11)/(float) det);
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool SolveLinear22System(float a11, float a12, float a21, float a22, float b1, float b2, out float x,
		                                       out float y)
		{
			bool result = false;
			x = InvalidPointF.X;
			y = InvalidPointF.Y;
			// if det == 0, there is no solution
			float det = ((a11*a22) - (a12*a21));
			if (det != 0) {
				result = true;
				x = (b2*a12 - b1*a22)/det;
				y = (b1*a21 - b2*a11)/det;
			}
			return result;
		}


		/// <summary>
		/// Returns the sign of the given value: -1, 0 (if value is 0) or +1
		/// </summary>
		public static int Signum(int value)
		{
			return (value > 0) ? 1 : (value < 0) ? -1 : 0;
		}


		/// <summary>
		/// Returns the sign of the given value: -1, 0 (if value is 0) or +1
		/// </summary>
		public static int Signum(long value)
		{
			return (value > 0) ? 1 : (value < 0) ? -1 : 0;
		}


		/// <summary>
		/// Returns the sign of the given value: -1, 0 (if value is 0) or +1
		/// </summary>
		public static int Signum(float value)
		{
			return (value > 0) ? 1 : (value < 0) ? -1 : 0;
		}


		/// <summary>
		/// Returns the sign of the given value: -1, 0 (if value is 0) or +1
		/// </summary>
		public static int Signum(double value)
		{
			return (value > 0) ? 1 : (value < 0) ? -1 : 0;
		}

		#region Functions to determine the orientation of points and lines

		/// <summary>
		/// Determines the quadrant within the coordinate system of point with respect to origin.
		/// </summary>
		// For symmetry and uniqueness, we include the right-sided axis into the respective quadrant.
		public static int CalcRelativeQuadrant(Point point, Point origin)
		{
			int result;
			// Not sure whether this special case is correct.
			if (point == origin)
				result = 1;
			else if (point.Y - origin.Y >= 0 && point.X - origin.X > 0)
				result = 1;
			else if (point.Y - origin.Y > 0 && point.X - origin.X <= 0)
				result = 2;
			else if (point.Y - origin.Y <= 0 && point.X - origin.X < 0)
				result = 3;
			else if (point.Y - origin.Y < 0 && point.X - origin.X >= 0)
				result = 4;
			else {
				Debug.Fail("Geometry.CalcRelativeQuadrant");
				result = 0;
			}
			return result;
		}

		#endregion

		#region Retrieving nearest / farest point and compare point distance methods

		/// <summary>
		/// Returns p1 or p2 dending on which one is nearer to p.
		/// </summary>
		public static Point GetNearestPoint(Point p, Point p1, Point p2)
		{
			return GetNearestPoint(p.X, p.Y, p1.X, p1.Y, p2.X, p2.Y);
		}


		/// <summary>
		/// Returns p1 or p2 dending on which one is nearer to p.
		/// </summary>
		public static Point GetNearestPoint(int pX, int pY, int p1X, int p1Y, int p2X, int p2Y)
		{
			Point result = InvalidPoint;
			//// VectorCrossProduct liefert leider manchmal abweichende Ergebnisse...
			//int d1 = Math.Abs(VectorCrossProduct(pX, pY, p1X, p1Y));
			//int d2 = Math.Abs(VectorCrossProduct(pX, pY, p2X, p2Y));

			float d1 = DistancePointPoint(pX, pY, p1X, p1Y);
			float d2 = DistancePointPoint(pX, pY, p2X, p2Y);
			if (d1 <= d2) {
				result.X = p1X;
				result.Y = p1Y;
			}
			else {
				result.X = p2X;
				result.Y = p2Y;
			}
			return result;
		}


		/// <summary>
		/// Returns the nearest point to p (if there is one) or null
		/// </summary>
		public static Point GetNearestPoint(Point p, IEnumerable<Point> points)
		{
			return GetNearestPoint(p.X, p.Y, points);
		}


		/// <summary>
		/// Returns the nearest point to x/y (if there is one) or null
		/// </summary>
		public static Point GetNearestPoint(int x, int y, IEnumerable<Point> points)
		{
			if (points == null) throw new ArgumentNullException("points");
			Point result = InvalidPoint;
			float d, lowest = float.MaxValue;
			foreach (Point point in points) {
				d = DistancePointPoint(x, y, point.X, point.Y);
				if (d < lowest) {
					lowest = d;
					result = point;
				}
			}
			return result;
		}


		/// <summary>
		/// Returns the nearest point to p (if there is one) or null
		/// </summary>
		public static PointF GetNearestPoint(PointF p, IEnumerable<PointF> points)
		{
			return GetNearestPoint(p.X, p.Y, points);
		}


		/// <summary>
		/// Returns the nearest point to x/y (if there is one) or null
		/// </summary>
		public static PointF GetNearestPoint(float x, float y, IEnumerable<PointF> points)
		{
			if (points == null) throw new ArgumentNullException("points");
			PointF result = InvalidPointF;
			float d, lowest = float.MaxValue;
			foreach (PointF point in points) {
				d = DistancePointPoint(x, y, point.X, point.Y);
				if (d < lowest) {
					lowest = d;
					result = point;
				}
			}
			return result;
		}


		/// <summary>
		/// Returns the furthest point from p (if one exists) or null.
		/// </summary>
		public static Point GetFurthestPoint(Point p, IEnumerable<Point> points)
		{
			if (points == null) throw new ArgumentNullException("points");
			Point result = InvalidPoint;
			float d, greatest = int.MinValue;
			foreach (Point point in points) {
				d = DistancePointPoint(p, point);
				if (d > greatest) {
					greatest = d;
					result = point;
				}
			}
			return result;
		}


		/// <summary>
		/// Returns the furthest point from p (if one exists) or null.
		/// </summary>
		public static PointF GetFurthestPoint(PointF p, IEnumerable<PointF> points)
		{
			if (points == null) throw new ArgumentNullException("points");
			PointF result = InvalidPointF;
			float d, greatest = int.MinValue;
			foreach (PointF point in points) {
				d = DistancePointPoint(p, point);
				if (d > greatest) {
					greatest = d;
					result = point;
				}
			}
			return result;
		}


		/// <summary>
		/// Returns Point p1 or Point p2 dpending on the distance to p.
		/// </summary>
		public static Point GetFurthestPoint(int pX, int pY, int p1X, int p1Y, int p2X, int p2Y)
		{
			Point result = Point.Empty;
			float d1 = DistancePointPoint(pX, pY, p1X, p1Y);
			float d2 = DistancePointPoint(pX, pY, p2X, p2Y);
			if (d1 >= d2) {
				result.X = p1X;
				result.Y = p1Y;
			}
			else {
				result.X = p2X;
				result.Y = p2Y;
			}
			return result;
		}


		/// <summary>
		/// Returns Point p1 or Point p2 dpending on the distance to p.
		/// </summary>
		public static Point GetFurthestPoint(Point p, Point p1, Point p2)
		{
			return GetFurthestPoint(p.X, p.Y, p1.X, p1.Y, p2.X, p2.Y);
		}


		/// <summary>
		/// Returns Point p1 or Point p2 dpending on the distance to p.
		/// </summary>
		public static PointF GetFurthestPoint(float pX, float pY, float p1X, float p1Y, float p2X, float p2Y)
		{
			PointF result = PointF.Empty;
			float d1 = DistancePointPoint(pX, pY, p1X, p1Y);
			float d2 = DistancePointPoint(pX, pY, p2X, p2Y);
			if (d1 >= d2) {
				result.X = p1X;
				result.Y = p1Y;
			}
			else {
				result.X = p2X;
				result.Y = p2Y;
			}
			return result;
		}


		/// <summary>
		/// Calculates the point on the line segment with the smallest absolute distance to p.
		/// </summary>
		/// <param name="aX"></param>
		/// <param name="aY"></param>
		/// <param name="bX"></param>
		/// <param name="bY"></param>
		/// <param name="pX"></param>
		/// <param name="pY"></param>
		/// <param name="nX"></param>
		/// <param name="nY"></param>
		public static void CalcNearestPointOfLineSegment(int aX, int aY, int bX, int bY, int pX, int pY, out int nX,
		                                                 out int nY)
		{
			CalcDroppedPerpendicularFoot(pX, pY, aX, aY, bX, bY, out nX, out nY);
			if (Math.Sign(nX - aX) != Math.Sign(bX - aX)) {
				// Point is on the other side of A
				nX = aX;
				nY = aY;
			}
			else if (nX - aX >= bX - aX) {
				// Point is further away than B
				nX = bX;
				nY = bY;
			}
		}


		/// <summary>
		/// Returns Point p1 or Point p2 dpending on the distance to p.
		/// </summary>
		public static PointF GetFurthestPoint(PointF p, PointF p1, PointF p2)
		{
			return GetFurthestPoint(p.X, p.Y, p1.X, p1.Y, p2.X, p2.Y);
		}


		/// <summary>
		/// Compares the distances of originalPoint to p and comparedPoint to p.
		/// Returns true if comparedPoint is nearer to p than originalPoint
		/// </summary>
		public static bool IsNearer(PointF p, PointF originalPt, PointF comparedPt)
		{
			return DistancePointPoint(p, comparedPt) < DistancePointPoint(p, originalPt);
		}


		/// <summary>
		/// Compares the distances of originalPoint to p and comparedPoint to p.
		/// Returns true if comparedPoint is farer away from p than originalPoint
		/// </summary>
		public static bool IsFarther(PointF p, PointF originalPt, PointF comparedPt)
		{
			return DistancePointPoint(p, comparedPt) < DistancePointPoint(p, originalPt);
		}

		#endregion

		#region Check methods if a geometric struct has valid values

		/// <summary>
		/// Tests if the given point has valid coordinates.
		/// </summary>
		public static bool IsValid(Point p)
		{
			return IsValidCoordinate(p.X) && IsValidCoordinate(p.Y);
		}


		/// <summary>
		/// Tests if the given point has valid coordinates.
		/// </summary>
		public static bool IsValid(PointF p)
		{
			return IsValidCoordinate(p.X) && IsValidCoordinate(p.Y);
		}


		/// <summary>
		/// Tests if the given coordinates are considered as valid.
		/// </summary>
		public static bool IsValid(int x, int y)
		{
			return IsValidCoordinate(ref x) && IsValidCoordinate(ref y);
		}


		/// <summary>
		/// Tests if the given coordinates are considered as valid.
		/// </summary>
		public static bool IsValid(float x, float y)
		{
			return IsValidCoordinate(ref x) && IsValidCoordinate(ref y);
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		public static bool IsValid(Rectangle r)
		{
			return IsValidCoordinate(r.X) && IsValidCoordinate(r.Y) && IsValidSize(r.Width) && IsValidSize(r.Height);
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		public static bool IsValid(RectangleF r)
		{
			return IsValidCoordinate(r.X) && IsValidCoordinate(r.Y) && IsValidSize(r.Width) && IsValidSize(r.Height);
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		public static bool IsValid(int x, int y, int width, int height)
		{
			return IsValidCoordinate(ref x) && IsValidCoordinate(ref y) && IsValidSize(ref width) && IsValidSize(ref height);
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		public static bool IsValid(float x, float y, float width, float height)
		{
			return IsValidCoordinate(ref x) && IsValidCoordinate(ref y) && IsValidSize(ref width) && IsValidSize(ref height);
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// </summary>
		public static bool IsValid(Size s)
		{
			return IsValidSize(s.Width) && IsValidSize(s.Height);
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// </summary>
		public static bool IsValid(SizeF s)
		{
			return IsValidSize(s.Width) && IsValidSize(s.Height);
		}


		/// <summary>
		/// Tests if the given point has valid coordinates. 
		/// The parameter will not be changed, ref is used for improved performance only.
		/// </summary>
		public static bool IsValid(ref Point p)
		{
			return IsValidCoordinate(p.X) && IsValidCoordinate(p.Y);
		}


		/// <summary>
		/// Tests if the given point has valid coordinates.
		/// The parameter will not be changed, ref is used for improved performance only.
		/// </summary>
		public static bool IsValid(ref PointF p)
		{
			return IsValidCoordinate(p.X) && IsValidCoordinate(p.Y);
		}


		/// <summary>
		/// Tests if the given coordinates are considered as valid.
		/// The parameter will not be changed, ref is used for improved performance only.
		/// </summary>
		public static bool IsValid(ref int x, ref int y)
		{
			return IsValidCoordinate(ref x) && IsValidCoordinate(ref y);
		}


		/// <summary>
		/// Tests if the given coordinates are considered as valid.
		/// The parameter will not be changed, ref is used for improved performance only.
		/// </summary>
		public static bool IsValid(ref float x, ref float y)
		{
			return IsValidCoordinate(ref x) && IsValidCoordinate(ref y);
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// The parameter will not be changed, ref is used for improved performance only.
		/// </summary>
		public static bool IsValid(ref Rectangle r)
		{
			return IsValidCoordinate(r.X) && IsValidCoordinate(r.Y) && IsValidSize(r.Width) && IsValidSize(r.Height);
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// The parameter will not be changed, ref is used for improved performance only.
		/// </summary>
		public static bool IsValid(ref RectangleF r)
		{
			return IsValidCoordinate(r.X) && IsValidCoordinate(r.Y) && IsValidSize(r.Width) && IsValidSize(r.Height);
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// The parameter will not be changed, ref is used for improved performance only.
		/// </summary>
		public static bool IsValid(ref int x, ref int y, ref int width, ref int height)
		{
			return IsValidCoordinate(ref x) && IsValidCoordinate(ref y) && IsValidSize(ref width) && IsValidSize(ref height);
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// The parameter will not be changed, ref is used for improved performance only.
		/// </summary>
		public static bool IsValid(ref float x, ref float y, ref float width, ref float height)
		{
			return IsValidCoordinate(ref x) && IsValidCoordinate(ref y) && IsValidSize(ref width) && IsValidSize(ref height);
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// The parameter will not be changed, ref is used for improved performance only.
		/// </summary>
		public static bool IsValid(ref Size s)
		{
			return IsValidSize(s.Width) && IsValidSize(s.Height);
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// </summary>
		public static bool IsValid(ref SizeF s)
		{
			return IsValidSize(s.Width) && IsValidSize(s.Height);
		}

		#endregion

		#region Check funktions (debug mode only)

		/// <summary>
		/// Tests if the given point has valid coordinates.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(Point p)
		{
			if (!IsValidCoordinate(p.X) || !IsValidCoordinate(p.Y))
				throw new ArgumentException(string.Format("{0} is not a valid point.", p));
		}


		/// <summary>
		/// Tests if the given point has valid coordinates.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(PointF p)
		{
			if (!IsValidCoordinate(p.X) || !IsValidCoordinate(p.Y))
				throw new ArgumentException(string.Format("{0} is not a valid point.", p));
		}


		/// <summary>
		/// Tests if the given coordinates are considered as valid.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(int x, int y)
		{
			if (!IsValidCoordinate(x)) throw new ArgumentException(string.Format("{0} is not a valid coordinate.", x));
			if (!IsValidCoordinate(y)) throw new ArgumentException(string.Format("{0} is not a valid coordinate.", y));
		}


		/// <summary>
		/// Tests if the given coordinates are considered as valid.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(float x, float y)
		{
			if (!IsValidCoordinate(x)) throw new ArgumentException(string.Format("{0} is not a valid coordinate.", x));
			if (!IsValidCoordinate(y)) throw new ArgumentException(string.Format("{0} is not a valid coordinate.", y));
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(Rectangle r)
		{
			if (!IsValidCoordinate(r.X) || !IsValidCoordinate(r.Y) || !IsValidSize(r.Width) || !IsValidSize(r.Height))
				throw new ArgumentException(string.Format("{0} is not a valid rectangle.", r));
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(RectangleF r)
		{
			if (!IsValidCoordinate(r.X) || !IsValidCoordinate(r.Y) || !IsValidSize(r.Width) || !IsValidSize(r.Height))
				throw new ArgumentException(string.Format("{0} is not a valid rectangle.", r));
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(int x, int y, int width, int height)
		{
			if (!IsValidCoordinate(x) || !IsValidCoordinate(y) || !IsValidSize(width) || !IsValidSize(height))
				throw new ArgumentException(string.Format("{0} is not a valid rectangle.", new Rectangle(x, y, width, height)));
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(float x, float y, float width, float height)
		{
			if (!IsValidCoordinate(x) || !IsValidCoordinate(y) || !IsValidSize(width) || !IsValidSize(height))
				throw new ArgumentException(string.Format("{0} is not a valid rectangle.", new RectangleF(x, y, width, height)));
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValidLTRB(int left, int top, int right, int bottom)
		{
			if (left == InvalidRectangle.Left && top == InvalidRectangle.Top && right == InvalidRectangle.Right &&
			    bottom == InvalidRectangle.Bottom)
				throw new ArgumentException(string.Format("{0} is not a valid rectangle.",
				                                          Rectangle.FromLTRB(left, top, right, bottom)));
		}


		/// <summary>
		/// Tests if the given rectangle has valid coordinates and a valid size.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValidLTRB(float left, float top, float right, float bottom)
		{
			if (left == InvalidRectangleF.Left && top == InvalidRectangleF.Top && right == InvalidRectangleF.Right &&
			    bottom == InvalidRectangleF.Bottom)
				throw new ArgumentException(string.Format("{0} is not a valid rectangle.",
				                                          RectangleF.FromLTRB(left, top, right, bottom)));
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(Size s)
		{
			if (!IsValidSize(s.Width) || !IsValidSize(s.Height))
				throw new ArgumentException(string.Format("{0} is not a valid size.", s));
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// </summary>
		[Conditional("DEBUG")]
		public static void AssertIsValid(SizeF s)
		{
			if (!IsValidSize(s.Width) || !IsValidSize(s.Height))
				throw new ArgumentException(string.Format("{0} is not a valid size.", s));
		}

		#endregion

		#region Definitions for invalid geometric structs

		/// <summary>Defines a point considered as invalid.</summary>
		public static readonly Point InvalidPoint;

		/// <summary>Defines a point considered as invalid.</summary>
		public static readonly PointF InvalidPointF;

		/// <summary>Defines a size considered as invalid.</summary>
		public static readonly Size InvalidSize;

		/// <summary>Defines a size considered as invalid.</summary>
		public static readonly SizeF InvalidSizeF;

		/// <summary>Defines a rectangle considered as invalid.</summary>
		public static readonly Rectangle InvalidRectangle;

		/// <summary>Defines a rectangle considered as invalid.</summary>
		public static readonly RectangleF InvalidRectangleF;

		#endregion

		#region Methods (private)

		static Geometry()
		{
			InvalidPoint.X =
				InvalidPoint.Y = InvalidCoordinateValue;

			InvalidPointF.X =
				InvalidPointF.Y = InvalidCoordinateValue;

			InvalidSize.Width =
				InvalidSize.Height = InvalidSizeValue;

			InvalidSizeF.Width =
				InvalidSizeF.Height = InvalidSizeValue;

			InvalidRectangle.Location = Point.Empty;
			InvalidRectangle.Width = -1;
			InvalidRectangle.Height = -1;

			InvalidRectangleF.Location = PointF.Empty;
			InvalidRectangleF.Width = -1;
			InvalidRectangleF.Height = -1;
		}


		/// <summary>
		/// Tests if the given coordinate is considered as valid.
		/// </summary>
		private static bool IsValidCoordinate(int c)
		{
			return c > InvalidCoordinateValue;
		}


		/// <summary>
		/// Tests if the given coordinate is considered as valid.
		/// </summary>
		private static bool IsValidCoordinate(float c)
		{
			return !float.IsNaN(c) && !float.IsInfinity(c) && c > InvalidCoordinateValue;
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// </summary>
		private static bool IsValidSize(int s)
		{
			return s >= 0;
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// </summary>
		private static bool IsValidSize(float s)
		{
			return !float.IsNaN(s) && !float.IsInfinity(s) && s >= 0;
		}


		/// <summary>
		/// Tests if the given coordinate is considered as valid.
		/// Parameter will not be modified, ref is for improved performance only.
		/// </summary>
		private static bool IsValidCoordinate(ref int c)
		{
			return c > InvalidCoordinateValue;
		}


		/// <summary>
		/// Tests if the given coordinate is considered as valid.
		/// Parameter will not be modified, ref is for improved performance only.
		/// </summary>
		private static bool IsValidCoordinate(ref float c)
		{
			return !float.IsNaN(c) && !float.IsInfinity(c) && c > InvalidCoordinateValue;
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// Parameter will not be modified, ref is for improved performance only.
		/// </summary>
		private static bool IsValidSize(ref int s)
		{
			return s >= 0;
		}


		/// <summary>
		/// Tests if the given size is considered as valid.
		/// Parameter will not be modified, ref is for improved performance only.
		/// </summary>
		private static bool IsValidSize(ref float s)
		{
			return !float.IsNaN(s) && !float.IsInfinity(s) && s >= 0;
		}


		/// <summary>
		/// Calculates the greatest common factor with the algorithm of Euklid.
		/// </summary>
		private static int CalcGreatestCommonFactor(int valueA, int valueB)
		{
			int result = 1;
			int rem;
			int a = Math.Max(valueA, valueB);
			int b = Math.Min(valueA, valueB);
			do {
				rem = a%b;
				if (rem != 0) b = rem;
			} while (rem != 0);
			Debug.Assert(valueA%b == 0 && valueB%b == 0);
			result = b;
			return result;
		}


		private static void GetMinValues(int width, int height, ResizeModifiers modifiers, out int minWidth, out int minHeight)
		{
			if ((modifiers & ResizeModifiers.MaintainAspect) != 0) {
				//if (width == height || width == 0 || height == 0) {
				//    minWidth = minHeight = 1;
				//} else {
				//    int gcf = CalcGreatestCommonFactor(width, height);
				//    minHeight = height / gcf;
				//    minWidth = width / gcf;
				//}

				// A simpler and less acurate but much faster alternative:
				float aspect = width/(float) height;
				minHeight = 1;
				minWidth = (int) Math.Round(1*aspect);
			}
			else minWidth = minHeight = 0;
		}

		#endregion

		private static Matrix matrix = new Matrix();

		private const int InvalidCoordinateValue = int.MinValue;
		private const int InvalidSizeValue = int.MinValue;
		private const double RadiansFactor = 0.017453292519943295769236907684886d; // = Math.PI / 180
	}
}