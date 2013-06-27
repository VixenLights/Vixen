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


namespace Dataweb.NShape.Advanced
{
	/// <summary>
	/// Abstract base class for polylines.
	/// </summary>
	/// <remarks>RequiredPermissions set</remarks>
	public abstract class PolylineBase : LineShapeBase
	{
		#region Shape Members

		/// <override></override>
		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is PolylineBase) {
				// Vertices and CapStyles will be copied by the base class
				// so there's nothing left to do here...
			}
		}


		///// <override></override>
		//public override void Fit(int x, int y, int width, int height) {
		//    Rectangle bounds = GetBoundingRectangle(true);
		//    // First, scale to the desired size
		//    //float scale;
		//    //scale = Geometry.CalcScaleFactor(bounds.Width, bounds.Height, width, height);

		//    // Second, move to the desired location
		//    Point topLeft = Point.Empty;
		//    topLeft.Offset(x, y);
		//    Point bottomRight = Point.Empty;
		//    bottomRight.Offset(x + width, y + height);
		//    MoveControlPointTo(ControlPointId.FirstVertex, topLeft.X, topLeft.Y, ResizeModifiers.None);
		//    MoveControlPointTo(ControlPointId.LastVertex, bottomRight.X, bottomRight.Y, ResizeModifiers.None);

		//    int ptNr = 0;
		//    foreach (ControlPointId ptId in GetControlPointIds(ControlPointCapabilities.Resize)) {
		//        if (IsFirstVertex(ptId) || IsLastVertex(ptId)) continue;
		//        ptNr = 1;
		//        Point pos = GetControlPointPosition(ptId);
		//        pos = Geometry.VectorLinearInterpolation(topLeft, bottomRight, ptNr / (float)(VertexCount - 1));
		//        MoveControlPointTo(ptId, pos.X, pos.Y, ResizeModifiers.None);

		//        //MoveControlPointTo(ptId, (int)Math.Round(pos.X * scale), (int)Math.Round(pos.Y * scale), ResizeModifiers.None);
		//    }
		//    InvalidateDrawCache();
		//}


		/// <override></override>
		public override int X
		{
			get { return GetControlPoint(0).GetPosition().X; }
			set
			{
				int origValue = GetControlPoint(0).GetPosition().X;
				if (!MoveTo(value, Y)) MoveTo(origValue, Y);
			}
		}


		/// <override></override>
		public override int Y
		{
			get { return GetControlPoint(0).GetPosition().Y; }
			set
			{
				int origValue = GetControlPoint(0).GetPosition().Y;
				if (!MoveTo(X, value)) MoveTo(X, origValue);
			}
		}


		/// <override></override>
		public override Point CalculateAbsolutePosition(RelativePosition relativePosition)
		{
			if (relativePosition == RelativePosition.Empty) throw new ArgumentOutOfRangeException("relativePosition");
			// The RelativePosition of a PolyLine is defined as
			// A = ControlPointId of the first vertex of the nearest line segment (FirstVertex -> LastVertex)
			// B = Angle between the line segment (A / next vertex of A) and the point
			// C = Distance of the point from A in percentage of the line segment's length
			Point result = Point.Empty;
			if (drawCacheIsInvalid)
				UpdateDrawCache();

			ControlPointId ptIdA = relativePosition.A;
			ControlPointId ptIdB = GetNextVertexId(relativePosition.A);
			if (ptIdB == ControlPointId.None)
				ptIdB = ControlPointId.LastVertex;
			Point posA = GetControlPointPosition(ptIdA);
			Point posB = GetControlPointPosition(ptIdB);

			float angle = Geometry.TenthsOfDegreeToDegrees(relativePosition.B);
			float distance = relativePosition.C/1000f;
			if (distance != 0) {
				float segmentLength = Geometry.DistancePointPoint(posA, posB);
				result = Geometry.VectorLinearInterpolation(posA, posB, distance);
				if (angle != 0)
					result = Geometry.RotatePoint(posA, angle, result);
				Debug.Assert(Geometry.IsValid(result));
			}
			else result = posA;
			return result;
		}


		/// <override></override>
		public override RelativePosition CalculateRelativePosition(int x, int y)
		{
			if (!Geometry.IsValid(x, y)) throw new ArgumentOutOfRangeException("x / y");
			// The RelativePosition of a PolyLine is defined as
			// A = ControlPointId of the first vertex of the nearest line segment (FirstVertex -> LastVertex)
			// B = Angle between the line segment (A / next vertex of A) and the point
			// C = Distance of the point from A in percentage of the line segment's length
			RelativePosition result = RelativePosition.Empty;
			Point p = Point.Empty;
			p.Offset(x, y);

			// Find the nearest line segment
			float angleFromA = float.NaN;
			float distanceFromA = float.NaN;
			float lowestAbsDistance = float.MaxValue;
			float lineWidth = LineStyle.LineWidth/2f + 2;

			LineSegment segment = LineSegment.Empty;
			foreach (LineSegment s in GetLineSegments()) {
				Point posA = s.VertexA.GetPosition();
				Point posB = s.VertexB.GetPosition();
				float dist = Geometry.DistancePointLine(p, posA, posB, true);
				if (dist < lowestAbsDistance) {
					lowestAbsDistance = dist;
					segment = s;
					if (dist > lineWidth)
						angleFromA = ((360 + Geometry.RadiansToDegrees(Geometry.Angle(posA, posB, p)))%360);
					else angleFromA = 0;
					distanceFromA = Geometry.DistancePointPoint(posA, p);
				}
			}

			if (segment != LineSegment.Empty) {
				result.A = segment.VertexA.Id;
				result.B = Geometry.DegreesToTenthsOfDegree(angleFromA);
				float segmentLength = Geometry.DistancePointPoint(segment.VertexA.GetPosition(), segment.VertexB.GetPosition());
				if (segmentLength != 0)
					result.C = (int) Math.Round((distanceFromA/(segmentLength/100))*10);
				else result.C = 0;
				Debug.Assert(result.B >= 0 && result.B <= 3600, "Calculated angle is out of range.");
			}
			if (result == RelativePosition.Empty) result.A = result.B = result.C = 0;
			return result;
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int fromX, int fromY)
		{
			// Tries to calculate a perpendicular intersection point
			Point result = Geometry.InvalidPoint;
			// find nearest line segment
			Point fromP = Point.Empty;
			fromP.Offset(fromX, fromY);
			float lowestDistance = float.MaxValue;

			LineSegment segment = LineSegment.Empty;
			foreach (LineSegment s in GetLineSegments()) {
				float distance = Geometry.DistancePointLine(fromP, s.VertexA.GetPosition(), s.VertexB.GetPosition(), true);
				if (distance < lowestDistance) {
					lowestDistance = distance;
					segment = s;
				}
			}
			if (segment != LineSegment.Empty) {
				Point p1 = segment.VertexA.GetPosition();
				Point p2 = segment.VertexB.GetPosition();
				float aSeg, bSeg, cSeg;
				// Calculate line equation for the line segment
				Geometry.CalcLine(p1, p2, out aSeg, out bSeg, out cSeg);
				// Translate the line to point fromX/fromY:
				float cFrom = -((aSeg*fromP.X) + (bSeg*fromP.Y));
				// Calculate perpendicular line through fromX/fromY
				float aPer, bPer, cPer;
				Geometry.CalcPerpendicularLine(fromX, fromY, aSeg, bSeg, cFrom, out aPer, out bPer, out cPer);
				// intersect perpendicular line with line segment

				float resX, resY;
				if (Geometry.IntersectLineWithLineSegment(aPer, bPer, cPer, p1, p2, out resX, out resY)) {
					result.X = (int) Math.Round(resX);
					result.Y = (int) Math.Round(resY);
				}
				else {
					// if the lines do not intersect, return the nearest point of the line segment
					result = Geometry.GetNearestPoint(fromP, p1, p2);
				}
			}
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId,
		                                               ControlPointCapabilities controlPointCapability)
		{
			return base.HasControlPointCapability(controlPointId, controlPointCapability);
		}


		/// <summary>
		/// Determines if the given point is inside the shape or near a control point having one of the given control point capabilities.
		/// </summary>
		public override ControlPointId HitTest(int x, int y, ControlPointCapabilities controlPointCapability, int range)
		{
			ControlPointId result = ControlPointId.None;

			int lineRange = (int) Math.Ceiling(LineStyle.LineWidth/2f) + 1;
			LineControlPoint lastVertex = null;
			int cnt = ControlPointCount;
			for (int i = 0; i < cnt; ++i) {
				// Get positions of the current control points and test on hit.
				LineControlPoint ctrlPoint = GetControlPoint(i);
				Point currPos = ctrlPoint.GetPosition();
				if (Geometry.DistancePointPoint(x, y, currPos.X, currPos.Y) <= lineRange + range) {
					if (HasControlPointCapability(ctrlPoint.Id, controlPointCapability))
						result = GetControlPointId(i);
				}

				// If the point was not hit, test the line between the current vertex and the last known vertex
				if (result == ControlPointId.None && HasControlPointCapability(ctrlPoint.Id, ControlPointCapabilities.Resize)) {
					if (lastVertex != null) {
						Point lastPos = lastVertex.GetPosition();
						float d = Geometry.DistancePointLine(x, y, lastPos.X, lastPos.Y, currPos.X, currPos.Y, true);
						if (d <= lineRange) {
							// Check if the Reference point matches the given capabilities and if any of the two points 
							// are inside the range 
							if (HasControlPointCapability(ControlPointId.Reference, controlPointCapability)
							    && !(Geometry.DistancePointPoint(x, y, lastPos.X, lastPos.Y) <= lineRange)
							    && !(Geometry.DistancePointPoint(x, y, currPos.X, currPos.Y) <= lineRange))
								result = ControlPointId.Reference;
						}
					}
					lastVertex = ctrlPoint;
				}
			}
			return result;
		}


		/// <override></override>
		public override Point CalcNormalVector(Point point)
		{
			Point result = Geometry.InvalidPoint;
			int range = (int) Math.Ceiling(LineStyle.LineWidth/2f) + 1;
			//for (int i = VertexCount - 1; i > 0; --i) {
			//    if (Geometry.LineContainsPoint(vertices[i].X, vertices[i].Y, vertices[i - 1].X, vertices[i - 1].Y, true, point.X, point.Y, range)) {
			//        float lineAngle = Geometry.RadiansToDegrees(Geometry.Angle(vertices[i - 1], vertices[i]));
			//        int x = point.X + 100;
			//        int y = point.Y;
			//        Geometry.RotatePoint(point.X, point.Y, lineAngle + 90, ref x, ref y);
			//        result.X = x;
			//        result.Y = y;
			//        return result;
			//    }
			//}
			//if (!Geometry.IsValid(result)) throw new NShapeException("The given Point is not part of the line shape.");
			//return result;

			foreach (LineSegment segment in GetLineSegments()) {
				Point posA = segment.VertexA.GetPosition();
				Point posB = segment.VertexB.GetPosition();
				if (Geometry.LineContainsPoint(posA, posB, true, point.X, point.Y, range)) {
					float lineAngle = Geometry.RadiansToDegrees(Geometry.Angle(posA, posB));
					int x = point.X + 100;
					int y = point.Y;
					Geometry.RotatePoint(point.X, point.Y, lineAngle + 90, ref x, ref y);
					result.X = x;
					result.Y = y;
					return result;
				}
			}
			if (!Geometry.IsValid(result)) throw new NShapeException("The given Point is not part of the line shape.");
			return result;
		}


		/// <override></override>
		public override void Invalidate()
		{
			base.Invalidate();
			if (DisplayService != null) {
				foreach (LineSegment segment in GetLineSegments())
					InvalidateSegment(segment.VertexA.GetPosition(), segment.VertexB.GetPosition());
			}
		}


		/// <override></override>
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) throw new ArgumentNullException("graphics");
			UpdateDrawCache();
			int lastIdx = shapePoints.Length - 1;
			if (lastIdx > 0) {
				// Draw interior of line caps
				DrawStartCapBackground(graphics, shapePoints[0].X, shapePoints[0].Y);
				DrawEndCapBackground(graphics, shapePoints[lastIdx].X, shapePoints[lastIdx].Y);

				// Draw line
				Pen pen = ToolCache.GetPen(LineStyle, StartCapStyleInternal, EndCapStyleInternal);
				graphics.DrawLines(pen, shapePoints);

				// ToDo: If the line is connected to another line, draw a connection indicator (ein Bommel oder so)
				// ToDo: Add a property for enabling/disabling this feature
			}
			base.Draw(graphics);
		}


		/// <override></override>
		public override void DrawOutline(Graphics graphics, Pen pen)
		{
			if (graphics == null) throw new ArgumentNullException("graphics");
			if (pen == null) throw new ArgumentNullException("pen");
			base.DrawOutline(graphics, pen);
			graphics.DrawLines(pen, shapePoints);
		}


		/// <override></override>
		public override void DrawThumbnail(Image image, int margin, Color transparentColor)
		{
			if (image == null) throw new ArgumentNullException("image");
			using (Graphics g = Graphics.FromImage(image)) {
				GdiHelpers.ApplyGraphicsSettings(g, RenderingQuality.MaximumQuality);
				g.Clear(transparentColor);

				int startCapSize = IsShapedLineCap(StartCapStyleInternal) ? StartCapStyleInternal.CapSize : 0;
				int endCapSize = IsShapedLineCap(EndCapStyleInternal) ? EndCapStyleInternal.CapSize : 0;

				int s = (int) Math.Max(startCapSize, endCapSize)*4;
				int width = Math.Max(image.Width*2, s);
				int height = Math.Max(image.Height*2, s);
				float scale = Math.Max((float) image.Width/width, (float) image.Height/height);
				g.ScaleTransform(scale, scale);

				int m = (int) Math.Round(Math.Max(margin/scale, Math.Max(startCapSize/2f, endCapSize/2f)));
				Rectangle r = Rectangle.Empty;
				r.Width = width;
				r.Height = height;
				r.Inflate(-m, -m);

				// Create and draw shape
				using (PolylineBase line = (PolylineBase) this.Clone()) {
					while (line.VertexCount > 2) line.RemoveVertex(line.GetPreviousVertexId(ControlPointId.LastVertex));

					int dh = r.Height/4;
					int dw = r.Width/3;
					// Move vertices
					line.MoveControlPointTo(ControlPointId.FirstVertex, r.Left, r.Top + dh, ResizeModifiers.None);
					line.MoveControlPointTo(ControlPointId.LastVertex, r.Right, r.Bottom - dh, ResizeModifiers.None);
					// Add vertices
					line.InsertVertex(ControlPointId.LastVertex, r.Left + dw, r.Bottom - dh);
					line.InsertVertex(ControlPointId.LastVertex, r.Right - dw, r.Top + dh);

					//int dh = r.Height / 8;
					//// Move vertices
					//line.MoveControlPointTo(ControlPointId.FirstVertex, r.Left, r.Bottom - dh, ResizeModifiers.None);
					//line.MoveControlPointTo(ControlPointId.LastVertex, r.Right, r.Bottom - dh, ResizeModifiers.None);
					//// Add vertices
					//line.InsertVertex(ControlPointId.LastVertex, r.Left + r.Width / 2, r.Top + dh);

					// Draw shape
					line.Draw(g);
				}
			}
		}


		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);
		}


		/// <override></override>
		protected internal override IEnumerable<Point> CalculateCells(int cellSize)
		{
			// Calculate cells occupied by children
			if (ChildrenCollection != null) {
				foreach (Shape shape in ChildrenCollection.BottomUp)
					foreach (Point cell in shape.CalculateCells(cellSize))
						yield return cell;
			}
			// Calculate the line radius
			int lineRadius = (int) Math.Ceiling(LineStyle.LineWidth/2f);

			Point p = Point.Empty;
			Rectangle firstVertexCells = Geometry.InvalidRectangle, lastVertexCells = Geometry.InvalidRectangle;
			Rectangle segmentStartCells = Geometry.InvalidRectangle, segmentEndCells = Geometry.InvalidRectangle;
			Rectangle lastStepCells = Geometry.InvalidRectangle, currentStepCells = Geometry.InvalidRectangle;
			// Instantiate the delegate using an anonymous method
			CheckCellOccupationDelegate cellOccupied =
				delegate(Point cell)
					{
						return (IsCellOccupied(firstVertexCells, cell)
						        || IsCellOccupied(lastVertexCells, cell)
						        || IsCellOccupied(segmentStartCells, cell)
						        || IsCellOccupied(segmentEndCells, cell)
						        || IsCellOccupied(lastStepCells, cell)
						       );
					};

			// Calculate cells occupied by the line's start cap
			Rectangle cells = Geometry.InvalidRectangle;
			cells = GetCapVertexCellArea(ControlPointId.FirstVertex, lineRadius, cellSize);
			foreach (Point cell in GetAreaCells(cells.Left, cells.Top, cells.Right, cells.Bottom, null))
				yield return cell;
			firstVertexCells = cells;
			// Calculate cells occupied by the line's end cap
			cells = GetCapVertexCellArea(ControlPointId.LastVertex, lineRadius, cellSize);
			foreach (Point cell in GetAreaCells(cells.Left, cells.Top, cells.Right, cells.Bottom, cellOccupied))
				yield return cell;
			lastVertexCells = cells;

			// Strategy:
			// We process each segment of the line from first to last vertex.
			// For each line segment, we calculate intersections with the cell borders along the line.
			// Beginning with the start vertex of a line segment, the next neighbour cell including the adjacent cells
			// are tested on intersection with the line borders.
			// For optimization resons, we calculate the cells of the vertices first and skip them later (if necessary).
			Point[] segmentBounds = new Point[4];
			Point startCell = Point.Empty;
			Point endCell = Point.Empty;
			ControlPointId lastVertexId = ControlPointId.None;
			foreach (ControlPointId vertexId in GetControlPointIds(ControlPointCapabilities.Resize)) {
				if (lastVertexId != ControlPointId.None) {
					Point posA = GetControlPointPosition(lastVertexId);
					Point posB = GetControlPointPosition(vertexId);
					float segmentAngle = Geometry.RadiansToDegrees(Geometry.Angle(posA, posB));
					segmentBounds[0] = Geometry.RotatePoint(posA.X, posA.Y, segmentAngle - 90, posA.X + lineRadius, posA.Y);
					segmentBounds[1] = Geometry.RotatePoint(posB.X, posB.Y, segmentAngle - 90, posB.X + lineRadius, posB.Y);
					segmentBounds[2] = Geometry.RotatePoint(posB.X, posB.Y, segmentAngle + 90, posB.X + lineRadius, posB.Y);
					segmentBounds[3] = Geometry.RotatePoint(posA.X, posA.Y, segmentAngle + 90, posA.X + lineRadius, posA.Y);

					// Calculate start cell return the occupied cells
					startCell = ShapeUtils.CalcCell(posA, cellSize);
					if (!Geometry.IsValid(segmentStartCells)) {
						// Return all cells occupied by the segment's start vertex
						cells = GetVertexCellArea(posA, lineRadius, cellSize, cellOccupied);
						foreach (Point cell in GetAreaCells(cells.Left, cells.Top, cells.Right, cells.Bottom, cellOccupied))
							yield return cell;
						segmentStartCells = cells;
					}

					// Calculate end cell return the occupied cells
					endCell = ShapeUtils.CalcCell(posB, cellSize);
					// If the segment's end is in the same cell, continue with the next segment...
					if (startCell == endCell) continue;
					// ...otherwise follow the line until the end cell is reached:

					// First, calculate processing direction...
					int stepX = (startCell.X == endCell.X) ? 0 : (startCell.X < endCell.X) ? 1 : -1;
					int stepY = (startCell.Y == endCell.Y) ? 0 : (startCell.Y < endCell.Y) ? 1 : -1;
					// Then, check if the end cell is on the cell boundaries ensure that the end cell is in the expected direction
					if (posB.X%cellSize == 0 && posB.Y%cellSize == 0) {
						// If the point is on the cell boundaries, e.g. (100,400), the cell will be calculated as (1,4)
						// This is correct if the processing direction is left-to-right and topdown, but for other processing 
						// directions the end cell will never be reached. So we have to correct this here.
						if (Geometry.Signum(endCell.X) != Geometry.Signum(stepX)) endCell.X += stepX;
						if (Geometry.Signum(endCell.Y) != Geometry.Signum(stepY)) endCell.Y += stepY;
					}
					// Afterwards, return all cells occupied by the segment's end vertex before processing...
					cells = GetVertexCellArea(posB, lineRadius, cellSize, cellOccupied);
					foreach (Point cell in GetAreaCells(cells.Left, cells.Top, cells.Right, cells.Bottom, cellOccupied))
						yield return cell;
					segmentEndCells = cells;
					// ... and follow the line cell by cell until the end cell is reached.
					Point currentCell = startCell;
					currentStepCells = lastStepCells = Geometry.InvalidRectangle;
					int xFrom, yFrom, xTo, yTo;
					bool endCellReached = false;
					do {
						// Calculate cell bounds
						if (stepX > 0) {
							xFrom = currentCell.X*cellSize;
							xTo = xFrom + cellSize;
						}
						else {
							xTo = currentCell.X*cellSize;
							xFrom = xTo + cellSize;
						}
						if (stepY > 0) {
							yFrom = currentCell.Y*cellSize;
							yTo = yFrom + cellSize;
						}
						else {
							yTo = currentCell.Y*cellSize;
							yFrom = yTo + cellSize;
						}
						// Check vertical and horizontal intersection with cell bounds
						int nextHStep = 0;
						if (Geometry.LineSegmentIntersectsWithLineSegment(xTo, yFrom, xTo, yTo, posA.X, posA.Y, posB.X, posB.Y))
							nextHStep = stepX;
						int nextVStep = 0;
						if (Geometry.LineSegmentIntersectsWithLineSegment(xFrom, yTo, xTo, yTo, posA.X, posA.Y, posB.X, posB.Y))
							nextVStep = stepY;
						if (nextHStep == 0 && nextVStep == 0) {
							Debug.Fail(string.Format("Got stuck in cell {0} while trying to process the line from {1} to {2}!", currentCell,
							                         posA, posB));
							endCellReached = true; // Prevent endless loop
						}
						// Get all 
						foreach (Point c in GetSegmentCells(segmentBounds, currentCell, cellSize, nextHStep, nextVStep, cellOccupied)) {
							yield return c;
							StoreOccupiedCell(c, ref currentStepCells);
						}
						currentCell.Offset(nextHStep, nextVStep);
						lastStepCells = currentStepCells;
						currentStepCells = Geometry.InvalidRectangle;
						if (currentCell == endCell) endCellReached = true;
						if (endCellReached) {
							// The segment's end cells become the segment start cells for the next vertex
							segmentStartCells = segmentEndCells;
							segmentEndCells = Geometry.InvalidRectangle;
						}
					} while (!endCellReached);
				}
				lastVertexId = vertexId;
			}
		}

		#endregion

		#region ILinearShape Members

		/// <override></override>
		public override int MinVertexCount
		{
			get { return 2; }
		}


		/// <override></override>
		public override int MaxVertexCount
		{
			get { return int.MaxValue; }
		}


		/// <override></override>
		public override ControlPointId InsertVertex(ControlPointId beforePointId, int x, int y)
		{
			ControlPointId newPointId = ControlPointId.None;
			if (IsFirstVertex(beforePointId) || beforePointId == ControlPointId.Reference || beforePointId == ControlPointId.None)
				throw new ArgumentException(string.Format("{0} is not a valid control point id for this operation.", beforePointId));

			// Create new vertex
			newPointId = GetNewControlPointId();
			return InsertVertex(beforePointId, newPointId, x, y);
		}


		/// <override></override>
		public override ControlPointId InsertVertex(ControlPointId beforePointId, ControlPointId newVertexId, int x, int y)
		{
			// Find position where to insert the new point
			int pointIndex = GetControlPointIndex(beforePointId);
			if (pointIndex < 0 || pointIndex > ControlPointCount)
				throw new IndexOutOfRangeException();

			InsertControlPoint(pointIndex, new VertexControlPoint(newVertexId, x, y));

			return newVertexId;
		}


		/// <override></override>
		public override ControlPointId AddVertex(int x, int y)
		{
			// ToDo: Falls die Distanz des Punktes x|y > 0 ist: Ausrechnen wo der Punkt sein muss (entlang der Lotrechten durch den Punkt verschieben)
			ControlPointId insertBeforeId = FindInsertionPointId(x, y, true);
			ControlPointId result = InsertVertex(insertBeforeId, x, y);
			if (result == ControlPointId.None) throw new NShapeException("Cannot add vertex {0}.", new Point(x, y));
			return result;
		}


		/// <override></override>
		public override void RemoveVertex(ControlPointId controlPointId)
		{
			if (controlPointId == ControlPointId.Any || controlPointId == ControlPointId.Reference ||
			    controlPointId == ControlPointId.None)
				throw new ArgumentException(string.Format("{0} is not a valid ControlPointId for this operation.", controlPointId));
			if (IsFirstVertex(controlPointId) || IsLastVertex(controlPointId))
				throw new InvalidOperationException(
					string.Format("ControlPoint {0} is a GluePoint and therefore must not be removed.", controlPointId));

			// Remove shape point
			int idx = GetControlPointIndex(controlPointId);
			RemoveControlPoint(idx);
		}


		/// <override></override>
		public override ControlPointId AddConnectionPoint(int x, int y)
		{
			if (!ContainsPoint(x, y)) throw new NShapeException("Coordinates {0},{1} are not on the shape.", x, y);

			ControlPointId pointId = GetNewControlPointId();
			RelativePosition relPos = CalculateRelativePosition(x, y);
			LineControlPoint ctrlPoint = new DynamicConnectionPoint(this, pointId, relPos);

			// Find insert position
			ControlPointId insertId = FindInsertionPointId(x, y, false);
			int idx = GetControlPointIndex(insertId);

			Debug.Assert(idx < ControlPointCount);
			InsertControlPoint(idx + 1, ctrlPoint);

			return pointId;
		}

		#endregion

		#region [Protected] Methods

		/// <ToBeCompleted></ToBeCompleted>
		protected internal PolylineBase(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			// nothing to do
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal PolylineBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			// nothing to do
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height)
		{
			Rectangle rectangle = Rectangle.Empty;
			rectangle.X = x;
			rectangle.Y = y;
			rectangle.Width = width;
			rectangle.Height = height;

			if (StartCapIntersectsWith(rectangle))
				return true;
			if (EndCapIntersectsWith(rectangle))
				return true;
			foreach (LineSegment segment in GetLineSegments()) {
				if (Geometry.RectangleIntersectsWithLine(rectangle, segment.VertexA.GetPosition(), segment.VertexB.GetPosition(),
				                                         true))
					return true;
			}
			return false;
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y)
		{
			if (base.ContainsPointCore(x, y))
				return true;
			int range = (int) Math.Ceiling(LineStyle.LineWidth/2f) + 1;
			foreach (LineSegment segment in GetLineSegments()) {
				if (Geometry.LineContainsPoint(segment.VertexA.GetPosition(), segment.VertexB.GetPosition(), true, x, y, range))
					return true;
			}
			return false;
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers)
		{
			if (deltaX == 0 && deltaY == 0) return true;

			Rectangle boundsBefore = Rectangle.Empty;
			if ((modifiers & ResizeModifiers.MaintainAspect) == ResizeModifiers.MaintainAspect)
				boundsBefore = GetBoundingRectangle(true);

			int idx = GetControlPointIndex(pointId);
			LineControlPoint ctrlPoint = GetControlPoint(idx);
			ctrlPoint.Offset(deltaX, deltaY);

			// Scale line if MaintainAspect flag is set and start- or endpoint was moved
			if ((modifiers & ResizeModifiers.MaintainAspect) == ResizeModifiers.MaintainAspect
			    && (IsFirstVertex(pointId) || IsLastVertex(pointId))) {
				// ToDo: Improve maintaining aspect of polylines
				if (deltaX != 0 || deltaY != 0) {
					int dx = (int) Math.Round(deltaX/(float) (VertexCount - 1));
					int dy = (int) Math.Round(deltaY/(float) (VertexCount - 1));
					// The first and the last points are glue points, so move only the points between
					foreach (LineSegment segment in GetLineSegments()) {
						if (!IsFirstVertex(segment.VertexA.Id) && !IsLastVertex(segment.VertexA.Id))
							segment.VertexA.Offset(dx, dy);
						if (!IsFirstVertex(segment.VertexB.Id) && !IsLastVertex(segment.VertexB.Id))
							segment.VertexB.Offset(dx, dy);
					}
					// After moving the vertices between the first and the last vertex, 
					// we have to maintain the glue point positions again
					MaintainGluePointPosition(ControlPointId.LastVertex, GetPreviousVertexId(ControlPointId.LastVertex));
					MaintainGluePointPosition(ControlPointId.FirstVertex, GetNextVertexId(ControlPointId.FirstVertex));
				}
			}
			else {
				// Maintain glue point positions (if connected via "Point-To-Shape"
				MaintainGluePointPosition(ControlPointId.FirstVertex, pointId);
				MaintainGluePointPosition(ControlPointId.LastVertex, pointId);
			}
			return true;
		}


		/// <override></override>
		protected override Point CalcGluePoint(ControlPointId gluePointId, Shape shape)
		{
			// Get the second point of the line segment that should intersect with the passive shape's outline
			ControlPointId secondPtId = gluePointId;
			if (IsFirstVertex(gluePointId))
				secondPtId = GetNextVertexId(gluePointId);
			else if (IsLastVertex(gluePointId))
				secondPtId = GetPreviousVertexId(gluePointId);
			//
			Point pointPosition = Geometry.InvalidPoint;
			// If the line only has 2 vertices and both are connected via Point-To-Shape connection...
			if (VertexCount == 2
			    && IsConnected(ControlPointId.FirstVertex, null) == ControlPointId.Reference
			    && IsConnected(ControlPointId.LastVertex, null) == ControlPointId.Reference) {
				// ... calculate new point position from the position of the second shape:
				Shape secondShape = GetConnectionInfo(secondPtId, null).OtherShape;
				if (secondShape is IPlanarShape) {
					pointPosition.X = secondShape.X;
					pointPosition.Y = secondShape.Y;
				}
			}
			if (!Geometry.IsValid(pointPosition))
				// Calculate new glue point position of the moved GluePoint by calculating the intersection point
				// of the passive shape's outline with the line segment from GluePoint to NextPoint/PrevPoint of GluePoint
				pointPosition = GetControlPointPosition(secondPtId);

			return CalcGluePointFromPosition(gluePointId, shape, pointPosition.X, pointPosition.Y);
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangleCore(bool tight)
		{
			// Calculate line caps' bounds
			Rectangle result = Rectangle.Empty;
			Geometry.CalcBoundingRectangle(GetVertexPositions(), out result);
			ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
			return result;
		}


		/// <override></override>
		protected override float CalcCapAngle(ControlPointId pointId)
		{
			float result = float.NaN;
			// Recalc shapePoints
			CalcShapePoints();
			Pen pen = ToolCache.GetPen(LineStyle, StartCapStyleInternal, EndCapStyleInternal);
			result = ShapeUtils.CalcLineCapAngle(shapePoints, pointId, pen);
			return result;
		}


		/// <override></override>
		protected override void RecalcDrawCache()
		{
			CalcShapePoints();
			// Calculate line caps and set drawCacheIsInvalid flag to false;
			base.RecalcDrawCache();
		}

		#endregion

		#region [Private] Methods

		private ControlPointId FindInsertionPointId(int x, int y, bool insertBefore)
		{
			// Find the point before which the new point has to be inserted
			ControlPointId result = ControlPointId.None;
			int range = (int) Math.Ceiling(LineStyle.LineWidth/2f) + 1;
			LineControlPoint lastPoint = null;
			for (int i = 0; i < ControlPointCount; ++i) {
				LineControlPoint currPoint = GetControlPoint(i);
				if (lastPoint != null) {
					Point posA = lastPoint.GetPosition();
					Point posB = currPoint.GetPosition();
					if (Geometry.LineContainsPoint(posA, posB, true, x, y, range))
						return insertBefore ? currPoint.Id : lastPoint.Id;
				}
				lastPoint = currPoint;
			}
			Debug.Assert(false, "No insert position found!");
			return ControlPointId.LastVertex;
		}


		private IEnumerable<LineSegment> GetLineSegments()
		{
			LineControlPoint lastVertex = null;
			int cnt = ControlPointCount;
			for (int i = 0; i < cnt; ++i) {
				LineControlPoint ctrlPoint = GetControlPoint(i);
				if (HasControlPointCapability(ctrlPoint.Id, ControlPointCapabilities.Resize)) {
					if (lastVertex != null) {
						LineSegment segment = LineSegment.Empty;
						segment.VertexA = lastVertex;
						segment.VertexB = ctrlPoint;
						yield return segment;
					}
					lastVertex = ctrlPoint;
				}
			}
		}


		private IEnumerable<Point> GetVertexPositions()
		{
			int cnt = ControlPointCount;
			for (int idx = 0; idx < cnt; ++idx) {
				LineControlPoint ptA = GetControlPoint(idx);
				if (!HasControlPointCapability(ptA.Id, ControlPointCapabilities.Resize))
					continue;
				yield return ptA.GetPosition();
			}
		}


		private void CalcShapePoints()
		{
			// Calculate shape points (relative to origin of coordinates)
			if (shapePoints.Length != VertexCount)
				Array.Resize(ref shapePoints, VertexCount);
			Point refPos = GetControlPointPosition(ControlPointId.Reference);
			// Do not use GetLineSegments here because it will call RecalcDrawCache
			for (int vIdx = VertexCount - 1, ptIdx = ControlPointCount - 1; ptIdx >= 0; --ptIdx) {
				LineControlPoint ctrlPoint = GetControlPoint(ptIdx);
				if (ctrlPoint is VertexControlPoint) {
					shapePoints[vIdx] = ctrlPoint.GetPosition();
					shapePoints[vIdx].Offset(-refPos.X, -refPos.Y);
					--vIdx;
				}
			}
		}


		private void InvalidatePointSegments(ControlPointId controlPointId)
		{
			Point ptPos = GetControlPointPosition(controlPointId);
			if (!IsFirstVertex(controlPointId)) {
				int prevIdx = GetControlPointIndex(GetPreviousVertexId(controlPointId));
				InvalidateSegment(GetControlPoint(prevIdx).GetPosition(), ptPos);
			}
			if (!IsLastVertex(controlPointId)) {
				int nextIdx = GetControlPointIndex(GetNextVertexId(controlPointId));
				InvalidateSegment(ptPos, GetControlPoint(nextIdx).GetPosition());
			}
		}


		private void InvalidateSegment(Point ptA, Point ptB)
		{
			InvalidateSegment(ptA.X, ptA.Y, ptB.X, ptB.Y);
		}


		private void InvalidateSegment(int x1, int y1, int x2, int y2)
		{
			if (DisplayService != null) {
				int xMin, xMax, yMin, yMax;
				xMin = Math.Min(x1, x2);
				yMin = Math.Min(y1, y2);
				xMax = Math.Max(x1, x2);
				yMax = Math.Max(y1, y2);
				int margin = 1;
				if (LineStyle != null) margin = LineStyle.LineWidth + 1;
				DisplayService.Invalidate(xMin - margin, yMin - margin, (xMax - xMin) + (margin + margin),
				                          (yMax - yMin) + (margin + margin));
			}
		}


		// Delegate for checking whether CalculateCells has processed a certain cell or not.
		private delegate bool CheckCellOccupationDelegate(Point p);


		private Rectangle GetCapVertexCellArea(ControlPointId pointId, int lineRadius, int cellSize)
		{
			Rectangle capBounds = Geometry.InvalidRectangle;
			if (pointId == ControlPointId.FirstVertex && IsShapedLineCap(StartCapStyleInternal))
				capBounds = StartCapBounds;
			else if (pointId == ControlPointId.LastVertex && IsShapedLineCap(EndCapStyleInternal))
				capBounds = EndCapBounds;

			Point p = Geometry.InvalidPoint;
			Rectangle cells = Geometry.InvalidRectangle;
			if (Geometry.IsValid(capBounds) && !capBounds.IsEmpty) {
				cells.Location = ShapeUtils.CalcCell(capBounds.Location, cellSize);
				p = ShapeUtils.CalcCell(capBounds.Right, capBounds.Bottom, cellSize);
				cells.Width = p.X - cells.X;
				cells.Height = p.Y - cells.Y;
			}
			else {
				p = GetControlPointPosition(pointId);
				cells.Location = ShapeUtils.CalcCell(p.X - lineRadius, p.Y - lineRadius, cellSize);
				p = ShapeUtils.CalcCell(p.X + lineRadius, p.Y + lineRadius, cellSize);
				cells.Width = p.X - cells.X;
				cells.Height = p.Y - cells.Y;
			}
			return cells;
		}


		private IEnumerable<Point> GetSegmentCells(Point[] segmentBounds, Point currentCell, int cellSize, int stepX,
		                                           int stepY, CheckCellOccupationDelegate cellOccupied)
		{
			// Calc bounds of the current cell
			Point cell = Point.Empty;
			// Step in horizontal direction: Test in vertical direction
			if (stepX != 0) {
				currentCell.X += stepX;
				int cellLeft = currentCell.X*cellSize;
				int cellTop = currentCell.Y*cellSize;
				int cellRight = cellLeft + cellSize;
				int cellBottom = cellTop + cellSize;
				// Test intersection of the cell above the current cell
				if (Geometry.PolygonIntersectsWithRectangle(segmentBounds, cellLeft, cellTop - cellSize, cellRight, cellTop)) {
					cell.X = currentCell.X;
					cell.Y = currentCell.Y - 1;
					if (!cellOccupied(cell)) yield return cell;
				}
				// Test intersection of the current cell
				if (Geometry.PolygonIntersectsWithRectangle(segmentBounds, cellLeft, cellTop, cellRight, cellBottom)) {
					if (!cellOccupied(currentCell)) yield return currentCell;
				}
				// Test intersection of the cell below the current cell
				if (Geometry.PolygonIntersectsWithRectangle(segmentBounds, cellLeft, cellBottom, cellRight, cellBottom + cellSize)) {
					cell.X = currentCell.X;
					cell.Y = currentCell.Y + 1;
					if (!cellOccupied(cell)) yield return cell;
				}
			}
			// Step in vertical direction: Test in horizontal direction
			if (stepY != 0) {
				currentCell.Y += stepY;
				int cellLeft = currentCell.X*cellSize;
				int cellTop = currentCell.Y*cellSize;
				int cellRight = cellLeft + cellSize;
				int cellBottom = cellTop + cellSize;
				// Test intersection of the cell above the current cell
				if (Geometry.PolygonIntersectsWithRectangle(segmentBounds, cellLeft - cellSize, cellTop, cellLeft, cellBottom)) {
					cell.X = currentCell.X - 1;
					cell.Y = currentCell.Y;
					if (!cellOccupied(cell)) yield return cell;
				}
				// Test intersection of the current cell
				if (Geometry.PolygonIntersectsWithRectangle(segmentBounds, cellLeft, cellTop, cellRight, cellBottom)) {
					if (!cellOccupied(currentCell)) yield return currentCell;
				}
				// Test intersection of the cell below the current cell
				if (Geometry.PolygonIntersectsWithRectangle(segmentBounds, cellRight, cellTop, cellRight + cellSize, cellBottom)) {
					cell.X = currentCell.X + 1;
					cell.Y = currentCell.Y;
					if (!cellOccupied(cell)) yield return cell;
				}
			}
		}


		private Rectangle GetVertexCellArea(Point vertexPos, int lineRadius, int cellSize,
		                                    CheckCellOccupationDelegate cellProcessed)
		{
			// Calculate the cells occupied by the vertex point considering its line radius
			// (for performance reasons, we assume the point to be a rectangle)
			Point cellLT = ShapeUtils.CalcCell(vertexPos.X - lineRadius, vertexPos.Y - lineRadius, cellSize);
			Point cellRB = ShapeUtils.CalcCell(vertexPos.X + lineRadius, vertexPos.Y + lineRadius, cellSize);
			Rectangle cells = Rectangle.Empty;
			cells.Location = cellLT;
			cells.Width = cellRB.X - cellLT.X;
			cells.Height = cellRB.Y - cellLT.Y;
			return cells;
		}


		private IEnumerable<Point> GetAreaCells(int cellsLeft, int cellsTop, int cellsRight, int cellsBottom,
		                                        CheckCellOccupationDelegate cellProcessed)
		{
			// Calculate the cells occupied by the vertex point considering its line radius
			// (for performance reasons, we assume the point to be a rectangle)
			Point p = Point.Empty;
			for (p.X = cellsLeft; p.X <= cellsRight; p.X += 1) {
				for (p.Y = cellsTop; p.Y <= cellsBottom; p.Y += 1) {
					if (cellProcessed == null) yield return p;
					else if (!cellProcessed(p)) yield return p;
				}
			}
		}


		private void StoreOccupiedCell(Point cell, ref Rectangle occupiedCells)
		{
			if (Geometry.IsValid(occupiedCells))
				occupiedCells = Geometry.UniteRectangles(cell.X, cell.Y, cell.X, cell.Y, occupiedCells);
			else {
				occupiedCells.Location = cell;
				occupiedCells.Width = occupiedCells.Height = 0;
			}
		}


		private bool IsCellOccupied(Rectangle occupiedCells, Point cell)
		{
			return ((occupiedCells.Width >= 0 && occupiedCells.Height >= 0)
			        && (occupiedCells.Location == cell || Geometry.RectangleContainsPoint(occupiedCells, cell.X, cell.Y)));
		}

		#endregion

		#region [Private] Debug Methods

#if DEBUG_UI

		private void DrawCell(Graphics gfx, Brush brush, Point cell, int cellSize)
		{
			DrawCell(gfx, brush, cell.X, cell.Y, cellSize);
		}


		private void DrawCell(Graphics gfx, Brush brush, int x, int y, int cellSize)
		{
			gfx.FillRectangle(brush, x*cellSize, y*cellSize, cellSize, cellSize);
		}

#endif

		#endregion

		private enum ProcessingDirection
		{
			Horizontal,
			Vertical
		}


		private struct LineSegment : IEquatable<LineSegment>
		{
			public static readonly LineSegment Empty;

			public static bool operator ==(LineSegment a, LineSegment b)
			{
				return (a.VertexA == b.VertexA && a.VertexB == b.VertexB);
			}

			public static bool operator !=(LineSegment a, LineSegment b)
			{
				return !(a == b);
			}

			public LineSegment(LineControlPoint pointA, int idxA, LineControlPoint pointB, int idxB)
			{
				this.VertexA = pointA;
				this.VertexB = pointB;
			}

			public override int GetHashCode()
			{
				int result = 0;
				result |= VertexA.GetHashCode();
				result |= VertexB.GetHashCode();
				return result;
			}

			public override bool Equals(object obj)
			{
				if (obj is LineSegment)
					return (this == (LineSegment) obj);
				else return false;
			}


			public bool Equals(LineSegment other)
			{
				return other == this;
			}

			static LineSegment()
			{
				Empty.VertexA = Empty.VertexB = null;
			}

			public LineControlPoint VertexA;
			public LineControlPoint VertexB;
		}
	}
}