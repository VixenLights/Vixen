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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Dataweb.NShape.Advanced
{
	/// <summary>
	/// Helper methods for calculating shapes
	/// </summary>
	public static class ShapeUtils
	{
		/// <summary>
		/// Calculates the cell for a shape map (spacial index) from the given coordinates.
		/// </summary>
		public static Point CalcCell(Point pos, int cellSize)
		{
			return CalcCell(pos.X, pos.Y, cellSize);
		}


		/// <summary>
		/// Calculates the cell for a shape map (spacial index) from the given coordinates.
		/// </summary>
		public static Point CalcCell(int x, int y, int cellSize)
		{
			// Optimization:
			// Use integer division for values >= 0 (>20 times faster than floored float divisions)
			// Use integer division and subtract 1 for values < 0 (otherwise calculating intersection with cell 
			// bounds will not work as expected as 0.5f / 1 is 0 instead of the expected result 1)
			Point cell = Point.Empty;
			cell.Offset(
				(x >= 0) ? (x/cellSize) : (x/cellSize) - 1,
				(y >= 0) ? (y/cellSize) : (y/cellSize) - 1
				);
			return cell;
		}


		/// <summary>
		/// Calculates the cell for a shape map (spacial index) from the given coordinates.
		/// </summary>
		public static void CalcCell(int x, int y, int cellSize, out int cellX, out int cellY)
		{
			// Optimization:
			// Use integer division for values >= 0 (>20 times faster than floored float divisions)
			// Use integer division and subtract 1 for values < 0 (otherwise calculating intersection with cell 
			// bounds will not work as expected as 0.5f / 1 is 0 instead of the expected result 1)
			cellX = (x >= 0) ? (x/cellSize) : (x/cellSize) - 1;
			cellY = (y >= 0) ? (y/cellSize) : (y/cellSize) - 1;
		}


		/// <summary>
		/// Inflates the bounding rectangle by the width of the given line style.
		/// </summary>
		public static void InflateBoundingRectangle(ref Rectangle boundingRectangle, ILineStyle lineStyle)
		{
			if (lineStyle == null) throw new ArgumentNullException("lineStyle");
			Geometry.AssertIsValid(boundingRectangle);
			if (lineStyle.LineWidth > 2) {
				int halfLineWidth = (int) Math.Ceiling(lineStyle.LineWidth/2f) - 1;
				boundingRectangle.Inflate(halfLineWidth, halfLineWidth);
			}
		}


		/// <summary>
		/// Calculates the angle of the line cap (this is usually the angle between the line's end vertex the its predecessor).
		/// If the predecessor of the line's end vertex is inside the line cap, the intersection point between the line cap and
		/// the rest of the line is calculated, like GDI+ does.
		/// </summary>
		public static float CalcLineCapAngle(Point[] points, ControlPointId pointId, Pen pen)
		{
			if (points == null) throw new ArgumentNullException("points");
			if (points.Length < 2) throw new ArgumentException("Parameter points must have at least 2 elements.");
			if (pointId != ControlPointId.FirstVertex && pointId != ControlPointId.LastVertex)
				throw new ArgumentException("pointId");
			float result = float.NaN;

			// Get required infos: Size of the cap, start point and point processing direction
			int startIdx = -1, step = 0;
			float capInset = 0;
			if (pointId == ControlPointId.FirstVertex) {
				startIdx = 0;
				step = 1;
				if (pen.StartCap == LineCap.Custom)
					capInset = pen.CustomStartCap.BaseInset*pen.Width;
			}
			else if (pointId == ControlPointId.LastVertex) {
				startIdx = points.Length - 1;
				step = -1;
				if (pen.EndCap == LineCap.Custom)
					capInset = pen.CustomEndCap.BaseInset*pen.Width;
			}
			else throw new NotSupportedException();
			// Store point where the line cap is located
			Point capPoint = points[startIdx];
			int maxIdx = points.Length - 1;
			int destIdx = startIdx + step;
			// Find first point *outside* the cap's area
			while (Geometry.DistancePointPoint(capPoint, points[destIdx]) < capInset && (destIdx > 0 && destIdx < maxIdx)) {
				startIdx = destIdx;
				destIdx += step;
			}
			// Calculate cap angle
			PointF p = Geometry.IntersectCircleWithLine(capPoint, capInset, points[startIdx], points[destIdx], true);
			if (Geometry.IsValid(p) && Geometry.LineContainsPoint(points[startIdx], points[destIdx], false, p.X, p.Y, 1))
				result = Geometry.RadiansToDegrees(Geometry.Angle(capPoint, p));
			if (float.IsNaN(result))
				result = Geometry.RadiansToDegrees(Geometry.Angle(points[startIdx], points[destIdx]));
			Debug.Assert(!float.IsNaN(result));
			return result;
		}
	}
}