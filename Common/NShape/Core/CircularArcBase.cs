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
using Dataweb.NShape.Commands;

namespace Dataweb.NShape.Advanced
{
	/// <summary>
	/// Abstract base class for circular arcs.
	/// </summary>
	/// <remarks>RequiredPermissions set</remarks>
	public abstract class CircularArcBase : LineShapeBase
	{
		#region Shape Members

		/// <override></override>
		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is CircularArcBase) {
				// Vertices and CapStyles will be copied by the base class
				// so there's nothing left to do here...
			}
		}


		/// <override></override>
		public override Point CalculateAbsolutePosition(RelativePosition relativePosition)
		{
			if (relativePosition == RelativePosition.Empty) throw new ArgumentOutOfRangeException("relativePosition");
			// The relative Position of an arc is defined by...
			// A: Tenths of percentage of the distance between StartPoint and EndPoint
			// B: Radius offset (tenths)
			// C: Not used
			Point result = Geometry.InvalidPoint;
			if (IsLine)
				result = Geometry.VectorLinearInterpolation(StartPoint, EndPoint, relativePosition.A/1000f);
			else result = CalcPointFromRelativeAngle(relativePosition.A, relativePosition.B);
			if (!Geometry.IsValid(result)) System.Diagnostics.Debug.Fail("Unable to calculate glue point position!");
			return result;
		}


		/// <override></override>
		public override RelativePosition CalculateRelativePosition(int x, int y)
		{
			if (!Geometry.IsValid(x, y)) throw new ArgumentOutOfRangeException("x / y");
			// The relative Position of an arc is defined by...
			// A: Tenths of percentage of the distance between StartPoint and EndPoint (== tenths of percentage of the sweep angle)
			// B: Radius offset: Distance from arc's outline
			// C: Not used
			RelativePosition result = RelativePosition.Empty;
			Point p = Point.Empty;
			p.Offset(x, y);
			if (IsLine) {
				float length = Geometry.DistancePointPoint(StartPoint, EndPoint);
				if (length == 0) {
					result.A = result.B = result.C = 0;
				}
				else {
					float distToPt = Geometry.DistancePointPoint(StartPoint, p);
					result.A = (int) Math.Round((distToPt/(length/100))*10);
					result.B = (int) Math.Round(Geometry.DistancePointLineSegment2(p, StartPoint, EndPoint)*10);
				}
			}
			else {
				// Encode angle to the given point as percentage of the current sweepangle 
				// (which is *always* from start point to end point)
				result.A = CalcRelativeAngleFromPoint(p);
				result.B = (int) Math.Round((Geometry.DistancePointPoint(Center, p) - Radius)*10);
				//if (result.A < 0 && result.A > 1000) { }
			}
			return result;
		}


		/// <override></override>
		public override Point CalcNormalVector(Point point)
		{
			Point result = Point.Empty;
			if (IsLine) {
				float lineAngle = Geometry.RadiansToDegrees(Geometry.Angle(StartPoint, EndPoint));
				int x = point.X + 100;
				int y = point.Y;
				Geometry.RotatePoint(point.X, point.Y, lineAngle + 90, ref x, ref y);
				result.X = x;
				result.Y = y;
			}
			else {
				if (drawCacheIsInvalid) UpdateDrawCache();
				float angleDeg = Geometry.RadiansToDegrees(Geometry.Angle(Center.X, Center.Y, point.X, point.Y));
				result = Geometry.CalcPoint(point.X, point.Y, angleDeg, 100);
			}
			return result;
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int fromX, int fromY)
		{
			Point result = Geometry.InvalidPoint;
			if (IsLine) {
				float aSeg, bSeg, cSeg;
				// Calculate line equation for the line segment
				Geometry.CalcLine(StartPoint, EndPoint, out aSeg, out bSeg, out cSeg);
				// Translate the line to point fromX/fromY:
				float cFrom = -((aSeg*fromX) + (bSeg*fromY));
				// Calculate perpendicular line through fromX/fromY
				float aPer, bPer, cPer;
				Geometry.CalcPerpendicularLine(fromX, fromY, aSeg, bSeg, cFrom, out aPer, out bPer, out cPer);
				// intersect perpendicular line with line segment

				float resX, resY;
				if (Geometry.IntersectLineWithLineSegment(aPer, bPer, cPer, StartPoint, EndPoint, out resX, out resY)) {
					result.X = (int) Math.Round(resX);
					result.Y = (int) Math.Round(resY);
				}
				else {
					// if the lines do not intersect, return the nearest point of the line segment
					result = Geometry.GetNearestPoint(fromX, fromY, StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);
				}
			}
			else {
				// Calculate intersection point(s)
				PointF p = Geometry.GetNearestPoint(fromX, fromY,
				                                    Geometry.IntersectArcLine(
				                                    	StartPoint.X, StartPoint.Y,
				                                    	RadiusPoint.X, RadiusPoint.Y,
				                                    	EndPoint.X, EndPoint.Y,
				                                    	Center.X, Center.Y,
				                                    	fromX, fromY, false));
				if (Geometry.IsValid(p)) {
					result.X = (int) Math.Round(p.X);
					result.Y = (int) Math.Round(p.Y);
				}
				else result = Geometry.GetNearestPoint(fromX, fromY, StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);
			}
			return result;
		}


		///// <override></override>
		//public override Point CalculateConnectionFoot(int x1, int y1, int x2, int y2) {
		//    Point result = Point.Empty;
		//    Point linePt = Geometry.GetFurthestPoint((int)Math.Round(Center.X), (int)Math.Round(Center.Y), x1, y1, x2, y2);
		//    Point intersectionPt = Geometry.GetNearestPoint(linePt, Geometry.IntersectArcLine(StartPoint.X, StartPoint.Y, RadiusPoint.X, RadiusPoint.Y, EndPoint.X, EndPoint.Y, x1, y1, x2, y2, false));
		//    if (Geometry.IsValid(intersectionPt))
		//        return intersectionPt;
		//    else
		//        return Geometry.GetNearestPoint(linePt, Geometry.CalcArcTangentThroughPoint(StartPoint.X, StartPoint.Y, RadiusPoint.X, RadiusPoint.Y, EndPoint.X, EndPoint.Y, linePt.X, linePt.Y));
		//}


		/// <override></override>
		protected internal override IEnumerable<Point> CalculateCells(int cellSize)
		{
			// Calculate cells occupied by children
			if (ChildrenCollection != null) {
				foreach (Shape shape in ChildrenCollection.BottomUp)
					foreach (Point cell in shape.CalculateCells(cellSize))
						yield return cell;
			}

			int lineRadius = (int) Math.Ceiling(LineStyle.LineWidth/2f);
			Rectangle firstVertexCells = Geometry.InvalidRectangle, lastVertexCells = Geometry.InvalidRectangle;
			CheckCellOccupationDelegate cellOccupied =
				delegate(Point cell)
					{
						return (IsCellOccupied(firstVertexCells, cell)
						        || IsCellOccupied(lastVertexCells, cell)
						       );
					};

			Rectangle cells = GetCapVertexCellArea(ControlPointId.FirstVertex, lineRadius, cellSize);
			foreach (Point cell in GetAreaCells(cells.Left, cells.Top, cells.Right, cells.Bottom, null))
				yield return cell;
			firstVertexCells = cells;
			cells = GetCapVertexCellArea(ControlPointId.LastVertex, lineRadius, cellSize);
			foreach (Point cell in GetAreaCells(cells.Left, cells.Top, cells.Right, cells.Bottom, null))
				yield return cell;
			lastVertexCells = cells;

			if (IsLine) {
				float angle = Geometry.RadiansToDegrees(Geometry.Angle(StartPoint, EndPoint));
				Point[] segmentBounds = new Point[4];
				segmentBounds[0] = Geometry.RotatePoint(StartPoint.X, StartPoint.Y, angle - 90, StartPoint.X + lineRadius,
				                                        StartPoint.Y);
				segmentBounds[1] = Geometry.RotatePoint(StartPoint.X, StartPoint.Y, angle + 90, StartPoint.X + lineRadius,
				                                        StartPoint.Y);
				segmentBounds[2] = Geometry.RotatePoint(EndPoint.X, EndPoint.Y, angle - 90, EndPoint.X + lineRadius, EndPoint.Y);
				segmentBounds[3] = Geometry.RotatePoint(EndPoint.X, EndPoint.Y, angle + 90, EndPoint.X + lineRadius, EndPoint.Y);

				Rectangle cellBounds = Rectangle.Empty;
				Geometry.CalcBoundingRectangle(segmentBounds, out cellBounds);
				Point cellsLT = ShapeUtils.CalcCell(cellBounds.Left, cellBounds.Top, cellSize);
				Point cellsRB = ShapeUtils.CalcCell(cellBounds.Right, cellBounds.Bottom, cellSize);
				foreach (Point cell in GetAreaCells(cellsLT.X, cellsLT.Y, cellsRB.X, cellsRB.Y, cellOccupied)) {
					if (Geometry.PolygonIntersectsWithRectangle(segmentBounds, cell.X*cellSize, cell.Y*cellSize, (cell.X + 1)*cellSize,
					                                            (cell.Y + 1)*cellSize))
						yield return cell;
				}
			}
			else {
				if (arcIsInvalid) RecalculateArc();
				// The outer bounding rectangle (including the control points) is required here
				Rectangle bounds = CalculateBoundingRectangle(false);
				int cellsLeft, cellsTop, cellsRight, cellsBottom;
				ShapeUtils.CalcCell(bounds.Left, bounds.Top, cellSize, out cellsLeft, out cellsTop);
				ShapeUtils.CalcCell(bounds.Right, bounds.Bottom, cellSize, out cellsRight, out cellsBottom);
				Rectangle cellBounds = Rectangle.Empty;
				cellBounds.Width = cellBounds.Height = cellSize;

				// Calculate Points for outer and inner arc (bounds of the arc's line)
				int innerRadius = (int) Math.Floor(Radius - lineRadius);
				int outerRadius = (int) Math.Ceiling(Radius + lineRadius);
				Point innerStartPt = Point.Round(Geometry.IntersectCircleWithLine(Center, innerRadius, StartPoint, Center, false));
				Point innerRadPt = Point.Round(Geometry.IntersectCircleWithLine(Center, innerRadius, RadiusPoint, Center, false));
				Point innerEndPt = Point.Round(Geometry.IntersectCircleWithLine(Center, innerRadius, EndPoint, Center, false));
				Point outerStartPt = Point.Round(Geometry.IntersectCircleWithLine(Center, outerRadius, StartPoint, Center, false));
				Point outerRadPt = Point.Round(Geometry.IntersectCircleWithLine(Center, outerRadius, RadiusPoint, Center, false));
				Point outerEndPt = Point.Round(Geometry.IntersectCircleWithLine(Center, outerRadius, EndPoint, Center, false));

				Point cell = Point.Empty;
				for (cell.X = cellsLeft; cell.X <= cellsRight; cell.X += 1) {
					for (cell.Y = cellsTop; cell.Y <= cellsBottom; cell.Y += 1) {
						if (cellOccupied(cell)) continue;
						cellBounds.X = cell.X*cellSize;
						cellBounds.Y = cell.Y*cellSize;
						if (Geometry.CircleIntersectsWithRectangle(cellBounds, StartPoint, lineRadius)
						    || Geometry.CircleIntersectsWithRectangle(cellBounds, RadiusPoint, lineRadius)
						    || Geometry.CircleIntersectsWithRectangle(cellBounds, EndPoint, lineRadius))
							yield return cell;
						else if (Geometry.ArcIntersectsWithRectangle(innerStartPt.X, innerStartPt.Y, innerRadPt.X, innerRadPt.Y, innerEndPt.X,
						                                             innerEndPt.Y, Center.X, Center.Y, innerRadius, cellBounds)
						         ||
						         Geometry.ArcIntersectsWithRectangle(outerStartPt.X, outerStartPt.Y, outerRadPt.X, outerRadPt.Y, outerEndPt.X,
						                                             outerEndPt.Y, Center.X, Center.Y, outerRadius, cellBounds))
							yield return cell;
					}
				}
			}
		}


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs(int mouseX, int mouseY, int range)
		{
			// Return actions of base class. Use private method to avoid compiler warning
			IEnumerator<MenuItemDef> enumerator = GetBaseActions(mouseX, mouseY, range);
			while (enumerator.MoveNext()) yield return enumerator.Current;
			// return own actions
			ControlPointId clickedPointId = FindNearestControlPoint(mouseX, mouseY, range, ControlPointCapabilities.Resize);

			bool isFeasible;
			string description;

			isFeasible = (clickedPointId == ControlPointId.None || clickedPointId == ControlPointId.Reference) &&
			             ContainsPoint(mouseX, mouseY) && VertexCount < 3;
			description = "You have to click on the line in order to insert new points";
			yield return new CommandMenuItemDef("Insert Point", null, description, isFeasible,
			                                    new AddVertexCommand(this, mouseX, mouseY));

			isFeasible = !HasControlPointCapability(clickedPointId, ControlPointCapabilities.Glue) && VertexCount > 2;
			if (clickedPointId == ControlPointId.None || clickedPointId == ControlPointId.Reference)
				description = "No control point was clicked";
			else description = "Glue control points may not be removed.";
			yield return new CommandMenuItemDef("Remove Point", null, description, isFeasible,
			                                    new RemoveVertexCommand(this, clickedPointId));
		}


		/// <override></override>
		public override ControlPointId HitTest(int x, int y, ControlPointCapabilities controlPointCapability, int range)
		{
			// First, check control points
			foreach (ControlPointId pointId in GetControlPointIds(ControlPointCapabilities.All)) {
				Point p = GetControlPointPosition(pointId);
				if (Geometry.DistancePointPoint(x, y, p.X, p.Y) <= range)
					if (HasControlPointCapability(pointId, controlPointCapability) && IsConnectionPointEnabled(pointId))
						return pointId;
			}
			// Afterwards, check shape outline
			Point startPos = StartPoint;
			Point endPos = EndPoint;
			if (IsLine) {
				if (Geometry.DistancePointLine(x, y, startPos.X, startPos.Y, endPos.X, endPos.Y, true) <= range) {
					if (HasControlPointCapability(ControlPointId.Reference, controlPointCapability)
					    && !(Geometry.DistancePointPoint(x, y, startPos.X, startPos.Y) <= range)
					    && !(Geometry.DistancePointPoint(x, y, endPos.X, endPos.Y) <= range)
					    && IsConnectionPointEnabled(ControlPointId.Reference))
						return ControlPointId.Reference;
				}
			}
			else {
				Point radPos = RadiusPoint;
				float hitTestRange = (int) Math.Ceiling(LineStyle.LineWidth/2f) + 1;
				if (Geometry.ArcContainsPoint(startPos.X, startPos.Y, radPos.X, radPos.Y, endPos.X, endPos.Y, Center.X, Center.Y,
				                              Radius, x, y, hitTestRange)) {
					if (HasControlPointCapability(ControlPointId.Reference, controlPointCapability)
					    && !(Geometry.DistancePointPoint(x, y, startPos.X, startPos.Y) <= range)
					    && !(Geometry.DistancePointPoint(x, y, endPos.X, endPos.Y) <= range)
					    && IsConnectionPointEnabled(ControlPointId.Reference))
						return ControlPointId.Reference;
				}
			}
			return ControlPointId.None;
		}


		///// <override></override>
		//public override void Fit(int x, int y, int width, int height) {
		//    if (IsLine) {
		//        MoveControlPointTo(ControlPointId.FirstVertex, x, y, ResizeModifiers.None);
		//        MoveControlPointTo(ControlPointId.LastVertex, x + width, y + height, ResizeModifiers.None);
		//    } else {
		//        float radius = Math.Min(width / 2f, height / 2f);
		//        PointF c = PointF.Empty;
		//        c.X = x + (width / 2f);
		//        c.Y = y + (height / 2f);

		//        // Calculate new start point
		//        PointF s = PointF.Empty;
		//        s.X = c.X + radius;
		//        s.Y = c.Y;
		//        s = Geometry.RotatePoint(c, StartAngle, s);
		//        // Calculate new end point
		//        PointF e = PointF.Empty;
		//        e = Geometry.RotatePoint(c, StartAngle + SweepAngle, s);
		//        // calculate new radius point
		//        PointF r = PointF.Empty;
		//        r = Geometry.RotatePoint(c, Geometry.RadiansToDegrees(Geometry.Angle(Center.X, Center.Y, RadiusPoint.X, RadiusPoint.Y)), s);
		//        foreach (ControlPointId id in GetControlPointIds(ControlPointCapabilities.Resize)) {
		//            if (IsFirstVertex(id)) MoveControlPointTo(id, (int)Math.Round(s.X), (int)Math.Round(s.Y), ResizeModifiers.None);
		//            else if (IsLastVertex(id)) MoveControlPointTo(id, (int)Math.Round(e.X), (int)Math.Round(e.Y), ResizeModifiers.None);
		//            else MoveControlPointTo(id, (int)Math.Round(r.X), (int)Math.Round(r.Y), ResizeModifiers.None);
		//        }
		//    }
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
		public override int MinVertexCount
		{
			get { return 2; }
		}


		/// <override></override>
		public override int MaxVertexCount
		{
			get { return 3; }
		}


		/// <override></override>
		public override ControlPointId InsertVertex(ControlPointId beforePointId, int x, int y)
		{
			if (IsFirstVertex(beforePointId) || beforePointId == ControlPointId.Reference || beforePointId == ControlPointId.None)
				throw new NShapeException("{0} is not a valid {1} for this operation.", beforePointId, typeof (ControlPointId).Name);
			if (VertexCount >= MaxVertexCount) throw new NShapeException("Number of maximum vertices reached.");

			radiusPointId = GetNewControlPointId();
			return InsertVertex(beforePointId, radiusPointId, x, y);
		}


		/// <override></override>
		public override ControlPointId InsertVertex(ControlPointId beforePointId, ControlPointId newVertexId, int x, int y)
		{
			if (IsFirstVertex(beforePointId) || beforePointId == ControlPointId.Reference || beforePointId == ControlPointId.None)
				throw new NShapeException("{0} is not a valid {1} for this operation.", beforePointId, typeof (ControlPointId).Name);
			if (VertexCount >= MaxVertexCount) throw new NShapeException("Number of maximum vertices reached.");

			// Create new LineControlPoint
			LineControlPoint ctrlPoint = new VertexControlPoint(newVertexId, x, y);
			if (VertexCount < MaxVertexCount) {
				int pointIndex = GetControlPointIndex(beforePointId);
				if (pointIndex < 0 || pointIndex > ControlPointCount)
					throw new IndexOutOfRangeException();
				// Insert Radius Point
				InsertControlPoint(pointIndex, ctrlPoint);
			}
			else {
				int pointIndex = GetControlPointIndex(GetPreviousVertexId(beforePointId));
				if (pointIndex < 0 || pointIndex > VertexCount || pointIndex > MaxVertexCount)
					throw new IndexOutOfRangeException();
				// replace Radius Point
				SetControlPoint(pointIndex, ctrlPoint);
			}

			return newVertexId;
		}


		/// <override></override>
		public override ControlPointId AddVertex(int x, int y)
		{
			if (VertexCount >= MaxVertexCount) throw new InvalidOperationException("Number of maximum vertices reached.");
			return InsertVertex(ControlPointId.LastVertex, x, y);
		}


		/// <override></override>
		public override void RemoveVertex(ControlPointId controlPointId)
		{
			if (IsFirstVertex(controlPointId) || IsLastVertex(controlPointId))
				throw new InvalidOperationException("Start- and end pioints of linear shapes cannot be removed.");
			Debug.Assert(controlPointId == radiusPointId);

			int idx = GetControlPointIndex(controlPointId);
			RemoveControlPoint(idx);

			radiusPointId = ControlPointId.None;
		}


		/// <override></override>
		public override ControlPointId AddConnectionPoint(int x, int y)
		{
			if (!ContainsPoint(x, y)) throw new NShapeException("Coordinates {0},{1} are not part of this shape.", x, y);

			ControlPointId pointId = GetNewControlPointId();
			RelativePosition relPos = CalculateRelativePosition(x, y);
			relPos.B = 0;
			LineControlPoint ctrlPoint = new DynamicConnectionPoint(this, pointId, relPos);

			// Insert ConnectionPoint 
			InsertControlPoint(ControlPointCount - 1, ctrlPoint);

			return pointId;
		}


		/// <override></override>
		public override void Invalidate()
		{
			if (DisplayService != null) {
				base.Invalidate();

				int margin = 1;
				if (LineStyle != null) margin = (int) Math.Ceiling(LineStyle.LineWidth/2f) + 1;
				//DisplayService.Invalidate((int)Math.Floor(arcBounds.X - margin), (int)Math.Floor(arcBounds.Y - margin), (int) Math.Ceiling(arcBounds.Width + margin + margin), (int)Math.Ceiling(arcBounds.Height + margin + margin));

				int left, right, top, bottom;
				left = Math.Min(StartPoint.X, EndPoint.X);
				right = Math.Max(StartPoint.X, EndPoint.X);
				top = Math.Min(StartPoint.Y, EndPoint.Y);
				bottom = Math.Max(StartPoint.Y, EndPoint.Y);
				if (!IsLine) {
					if (arcIsInvalid) RecalculateArc();
					if (!arcIsInvalid) {
						if (arcBounds.Left < left)
							left = (int) Math.Floor(arcBounds.Left);
						if (arcBounds.Top < top)
							top = (int) Math.Floor(arcBounds.Top);
						if (arcBounds.Right > right)
							right = (int) Math.Ceiling(arcBounds.Right);
						if (arcBounds.Bottom > bottom)
							bottom = (int) Math.Ceiling(arcBounds.Bottom);
					}
				}
				DisplayService.Invalidate(left - margin, top - margin, right - left + margin + margin,
				                          bottom - top + margin + margin);
			}
		}


		/// <override></override>
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) throw new ArgumentNullException("graphics");
			UpdateDrawCache();
			if (VertexCount > 1) {
				// draw caps interior
				DrawStartCapBackground(graphics, shapePoints[0].X, shapePoints[0].Y);
				int endPtIdx = shapePoints.Length - 1;
				DrawEndCapBackground(graphics, shapePoints[endPtIdx].X, shapePoints[endPtIdx].Y);

				// draw line
				Pen pen = ToolCache.GetPen(LineStyle, StartCapStyleInternal, EndCapStyleInternal);
				DrawOutline(graphics, pen);

#if DEBUG_DIAGNOSTICS

				#region Visualize arc definition

				//// Draw bounding rectangle / line cap bounds
				//graphics.DrawRectangle(Pens.Red, GetBoundingRectangle(true));
				//if (StartCapBounds != Geometry.InvalidRectangle)
				//    graphics.DrawRectangle(Pens.Red, StartCapBounds);
				//if (EndCapBounds != Geometry.InvalidRectangle)
				//    graphics.DrawRectangle(Pens.Red, EndCapBounds);


				//if (!IsLine) {
				//    // Draw start- and sweep angle of the arc
				//    using (Brush sweepAngBrush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
				//        GdiHelpers.DrawAngle(graphics, sweepAngBrush, Center, arcStartAngle, arcSweepAngle, (int)Math.Round(3 * (Radius / 4)));
				//    using (Brush startAngBrush = new SolidBrush(Color.FromArgb(128, Color.Navy)))
				//        GdiHelpers.DrawAngle(graphics, startAngBrush, Center, arcStartAngle, (int)Math.Round(Radius / 2));

				//    // Draw the arc's vertices
				//    GdiHelpers.DrawPoint(graphics, Pens.Lime, StartPoint.X, StartPoint.Y, 10);
				//    GdiHelpers.DrawPoint(graphics, Pens.Blue, RadiusPoint.X, RadiusPoint.Y, 10);
				//    GdiHelpers.DrawPoint(graphics, Pens.Green, EndPoint.X, EndPoint.Y, 10);

				//    // Draw the arc's shapePoints
				//    for (int i = 0; i < shapePoints.Length; ++i)
				//        GdiHelpers.DrawPoint(graphics, Pens.Red, shapePoints[i].X, shapePoints[i].Y, 3);
				//    GdiHelpers.DrawPoint(graphics, Pens.Red, Center.X, Center.Y, 3);
				//    graphics.DrawRectangle(Pens.Red, arcBounds.X, arcBounds.Y, arcBounds.Width, arcBounds.Height);
				//}

				#endregion

				#region Visualize absolute/relative positions

				//// Draw relative positions from 0% - 100%
				//RelativePosition relPos = RelativePosition.Empty;
				//Point p = Point.Empty;
				//int size = 10;
				//relPos.B = 0;
				//relPos.A = 0;
				//int cnt = 10;
				//for (int i = 0; i <= cnt; ++i) {
				//   relPos.A = i * (1000 / cnt);
				//   p = CalculateAbsolutePosition(relPos);
				//   GdiHelpers.DrawPoint(graphics, Pens.Green, p.X, p.Y, size);
				//   relPos = CalculateRelativePosition(p.X, p.Y);
				//   GdiHelpers.DrawPoint(graphics, Pens.Red, p.X, p.Y, size);
				//}

				//// Draw angles to connected glue points
				//if (!IsLine) {
				//   foreach (ShapeConnectionInfo ci in GetConnectionInfos(ControlPointId.Reference, null)) {
				//      Point pt = ci.OtherShape.GetControlPointPosition(ci.OtherPointId);
				//      float angleDeg = Geometry.RadiansToDegrees(Geometry.Angle(Center.X, Center.Y, StartPoint.X, StartPoint.Y, pt.X, pt.Y));
				//      if (angleDeg < 0) angleDeg += 360;
				//      GdiHelpers.DrawAngle(graphics, Brushes.Red, Center, arcStartAngle + angleDeg, (int)(Radius / 2));
				//      GdiHelpers.DrawPoint(graphics, Pens.Blue, pt.X, pt.Y, 3);
				//   }
				//}

				//if (!IsLine) {
				//   // Draw angle to point 0 and shapeAngle to point 2
				//   float arcRadius;
				//   PointF arcCenter = Geometry.CalcArcCenterAndRadius((float)StartPoint.X, (float)StartPoint.Y, (float)RadiusPoint.X, (float)RadiusPoint.Y, (float)EndPoint.X, (float)EndPoint.Y, out arcRadius);

				//   SolidBrush startAngleBrush = new SolidBrush(Color.FromArgb(96, Color.Red));
				//   SolidBrush sweepAngleBrush = new SolidBrush(Color.FromArgb(96, Color.Blue));
				//   SolidBrush ptAngleBrush = new SolidBrush(Color.FromArgb(96, Color.Green));

				//   // Draw StartAngle
				//   float angleToStartPt = (360 + Geometry.RadiansToDegrees(Geometry.Angle(Center.X, Center.Y, StartPoint.X, StartPoint.Y))) % 360;
				//   GdiHelpers.DrawAngle(graphics, startAngleBrush, arcCenter, angleToStartPt, (int)(Radius / 2));

				//   // Draw relative Position of connected shape
				//   foreach (ShapeConnectionInfo ci in GetConnectionInfos(ControlPointId.Reference, null)) {
				//      if (ci.OwnPointId != ControlPointId.Reference) continue;
				//      Point oPt = ci.OtherShape.GetControlPointPosition(ci.OtherPointId);

				//      RelativePosition relativePosition = CalculateRelativePosition(oPt.X, oPt.Y);
				//      float arcLength = Radius * Geometry.DegreesToRadians(SweepAngle);
				//      float resAngleToPt = Geometry.RadiansToDegrees((arcLength * relativePosition.A / 1000f) / Radius);

				//      // Draw relative position
				//      GdiHelpers.DrawAngle(graphics, sweepAngleBrush, Center, angleToStartPt, resAngleToPt, (int)(Radius - (Radius / 4)));

				//      // Draw absolute position
				//      Point absPtPos = CalculateAbsolutePosition(relativePosition);
				//      GdiHelpers.DrawPoint(graphics, Pens.Red, absPtPos.X, absPtPos.Y, 13);
				//      break;
				//   }

				//   startAngleBrush.Dispose();
				//   sweepAngleBrush.Dispose();
				//}

				//if (!IsLine) {
				//   // Draw start angle to point 0 and sweep angle to point 2
				//   float arcRadius;
				//   PointF arcCenter = Geometry.CalcArcCenterAndRadius((float)StartPoint.X, (float)StartPoint.Y, (float)RadiusPoint.X, (float)RadiusPoint.Y, (float)EndPoint.X, (float)EndPoint.Y, out arcRadius);

				//   GdiHelpers.DrawPoint(graphics, Pens.Red, shapePoints[0].X, shapePoints[0].Y, 4);
				//   GdiHelpers.DrawPoint(graphics, Pens.Purple, shapePoints[1].X, shapePoints[1].Y, 4);
				//   GdiHelpers.DrawPoint(graphics, Pens.Blue, shapePoints[2].X, shapePoints[2].Y, 4);

				//   SolidBrush startAngleBrush = new SolidBrush(Color.FromArgb(96, Color.Red));
				//   SolidBrush sweepAngleBrush = new SolidBrush(Color.FromArgb(96, Color.Blue));

				//   float startAngle = Geometry.RadiansToDegrees(Geometry.Angle(arcCenter.X, arcCenter.Y, shapePoints[0].X, shapePoints[0].Y));
				//   float endAngle = Geometry.RadiansToDegrees(Geometry.Angle(arcCenter.X, arcCenter.Y, shapePoints[2].X, shapePoints[2].Y));
				//   if (!float.IsNaN(startAngle))
				//      GdiHelpers.DrawAngle(graphics, startAngleBrush, Center, startAngle, (int)(Radius / 2));
				//   if (!float.IsNaN(endAngle))
				//      GdiHelpers.DrawAngle(graphics, sweepAngleBrush, Center, endAngle, (int)(Radius / 2));

				//   startAngleBrush.Dispose();
				//   sweepAngleBrush.Dispose();
				//}

				#endregion

				#region Visualize dynamic connection point calculation

				//ControlPointId gluePointId = ControlPointId.LastVertex;
				//if (!IsLine && IsConnected(gluePointId, null) != ControlPointId.None) {
				//   ShapeConnectionInfo ci = GetConnectionInfo(gluePointId, null);
				//   Shape shape = ci.OtherShape;
				//   Point ptPos = Geometry.InvalidPoint;
				//   // Calculate desired arc: 
				//   // Start point, end point, center and radius
				//   ShapeConnectionInfo startCi = GetConnectionInfo(ControlPointId.FirstVertex, null);
				//   ShapeConnectionInfo endCi = GetConnectionInfo(ControlPointId.LastVertex, null);
				//   Point startPt = Point.Empty, endPt = Point.Empty;
				//   if (startCi.IsEmpty) startPt.Offset(StartPoint.X, StartPoint.Y);
				//   else startPt.Offset(startCi.OtherShape.X, startCi.OtherShape.Y);
				//   if (endCi.IsEmpty) endPt.Offset(EndPoint.X, EndPoint.Y);
				//   else endPt.Offset(endCi.OtherShape.X, endCi.OtherShape.Y);
				//   float r;
				//   PointF centerPt = Geometry.CalcArcCenterAndRadius(startPt, RadiusPoint, endPt, out r);
				//   //
				//   // Draw the base circle of the new arc
				//   Pen tmpPen = Pens.Yellow;
				//   GdiHelpers.DrawLine(graphics, tmpPen, centerPt, startPt);
				//   GdiHelpers.DrawLine(graphics, tmpPen, centerPt, endPt);
				//   GdiHelpers.DrawPoint(graphics, tmpPen, centerPt.X, centerPt.Y, 3);
				//   graphics.DrawEllipse(tmpPen, centerPt.X - radius, centerPt.Y - radius, radius + radius, radius + radius);

				//   //
				//   // Calculate tangent on the desired arc through the other shape's center
				//   Point tangentPt = IsFirstVertex(gluePointId) ? startPt : endPt;
				//   float a, b, c;
				//   Geometry.CalcPerpendicularLine(centerPt.X, centerPt.Y, tangentPt.X, tangentPt.Y, out a, out b, out c);
				//   int aT, bT, cT;
				//   Geometry.TranslateLine((int)a, (int)b, (int)c, tangentPt, out aT, out bT, out cT);
				//   // Draw Tangent
				//   tmpPen = Pens.Orange;
				//   GdiHelpers.DrawLine(graphics, tmpPen, aT, bT, cT);
				//   GdiHelpers.DrawPoint(graphics, tmpPen, tangentPt.X, tangentPt.Y, 3);

				//   //
				//   // Calculate intersection point of the calculated tangent and the perpendicular bisector 
				//   // of the line through startPt and endPt
				//   Geometry.CalcPerpendicularBisector(startPt.X, startPt.Y, endPt.X, endPt.Y, out a, out b, out c);
				//   PointF pT = Geometry.IntersectLines(aT, bT, cT, a, b, c);
				//   //
				//   // Draw the calculated intersection point and the perpendicular bisector
				//   tmpPen = Pens.OrangeRed;
				//   GdiHelpers.DrawLine(graphics, tmpPen, a, b, c);
				//   GdiHelpers.DrawPoint(graphics, tmpPen, (endPt.X - ((endPt.X - startPt.X) / 2)), (endPt.Y - ((endPt.Y - startPt.Y) / 2)), 3);

				//   if (pT != Geometry.InvalidPointF) {
				//      PointF pB = Geometry.VectorLinearInterpolation(startPt, endPt, 0.5);
				//      ptPos = Point.Round(Geometry.VectorLinearInterpolation(pB, pT, 0.75));

				//      graphics.DrawLine(Pens.DarkRed, ptPos, centerPt);
				//      // Check if the calculated point is on the right side
				//      bool chk = Geometry.ArcIntersectsWithLine(startPt.X, startPt.Y, RadiusPoint.X, RadiusPoint.Y, endPt.X, endPt.Y, ptPos.X, ptPos.Y, centerPt.X, centerPt.Y, true);

				//      bool intersectswith = false;
				//      List<PointF> chkPts = new List<PointF>(Geometry.IntersectArcLine(startPt, RadiusPoint, endPt, ptPos, centerPt, true));
				//      for (int i = chkPts.Count - 1; i >= 0; --i) {
				//         if (chkPts[i] != Geometry.InvalidPointF) {
				//            intersectswith = true;
				//            break;
				//         }
				//      }
				//      if (!intersectswith)
				//         ptPos = Geometry.VectorLinearInterpolation(ptPos, tangentPt, 2);

				//      //
				//      // Draw the calculated point
				//      GdiHelpers.DrawPoint(graphics, Pens.Red, ptPos.X, ptPos.Y, 3);
				//   }
				//   // If the arc only has 2 points or something went wrong while calculating the desired arc
				//   if (ptPos == Geometry.InvalidPoint)
				//      ptPos = Geometry.VectorLinearInterpolation(StartPoint, EndPoint, 0.5d);
				//   Point result = CalcGluePointFromPosition(gluePointId, shape, ptPos.X, ptPos.Y);
				//   //
				//   // Draw resulting intersection point
				//   GdiHelpers.DrawPoint(graphics, Pens.Lime, result.X, result.Y, 3);
				//}

				#endregion

				#region Visualize simultaneous dynamic calculation of two connected glue points

				//// Draw Calculating both ends simultaneously
				//if (!IsLine
				//   && IsConnected(ControlPointId.FirstVertex, null) == ControlPointId.Reference
				//   && IsConnected(ControlPointId.LastVertex, null) == ControlPointId.Reference) {
				//   // Get partner shapes and current glue point positions
				//   Shape shapeA = GetConnectionInfo(ControlPointId.FirstVertex, null).OtherShape;
				//   Shape shapeB = GetConnectionInfo(ControlPointId.LastVertex, null).OtherShape;
				//   Point currGluePtAPos = GetControlPointPosition(ControlPointId.FirstVertex);
				//   Point currGluePtBPos = GetControlPointPosition(ControlPointId.LastVertex);

				//   float sPtAngle = Geometry.RadiansToDegrees(Geometry.Angle(shapeA.X, shapeA.Y, RadiusPoint.X, RadiusPoint.Y));
				//   float ePtAngle = Geometry.RadiansToDegrees(Geometry.Angle(shapeB.X, shapeB.Y, RadiusPoint.X, RadiusPoint.Y));
				//   float dist = Geometry.DistancePointPoint(shapeA.X, shapeA.Y, shapeB.X, shapeB.Y) * 2;
				//   Point tmpPtS = Geometry.CalcPoint(shapeA.X, shapeA.Y, sPtAngle, dist);
				//   Point tmpPtE = Geometry.CalcPoint(shapeB.X, shapeB.Y, ePtAngle, dist);
				//   Point newRadiusPtPos = Geometry.IntersectLines(
				//      shapeA.X, shapeA.Y, tmpPtS.X, tmpPtS.Y,
				//      shapeB.X, shapeB.Y, tmpPtE.X, tmpPtE.Y);
				//   if (newRadiusPtPos == Geometry.InvalidPoint)
				//      newRadiusPtPos = Geometry.VectorLinearInterpolation(shapeA.X, shapeA.Y, shapeB.X, shapeB.Y, 0.5);

				//   // Calculate a common base point for connection foot calculation
				//   Point calcBasePtA = CalcGluePointCalculationBase(ControlPointId.FirstVertex, shapeA);
				//   Point calcBasePtB = CalcGluePointCalculationBase(ControlPointId.LastVertex, shapeB);
				//   Point calcBasePt = Geometry.VectorLinearInterpolation(calcBasePtA, calcBasePtB, 0.5);
				//   // Calc new glue point positions from the common calculation base
				//   Point newGluePtAPos = CalcGluePointFromPosition(ControlPointId.FirstVertex, shapeA, calcBasePt.X, calcBasePt.Y);
				//   Point newGluePtBPos = CalcGluePointFromPosition(ControlPointId.LastVertex, shapeB, calcBasePt.X, calcBasePt.Y);
				//   // Move both glue points to their final destination

				//   Pen tmpPen;
				//   tmpPen = Pens.Red;
				//   GdiHelpers.DrawLine(graphics, tmpPen, StartPoint, tmpPtS);
				//   tmpPen = Pens.Blue;
				//   GdiHelpers.DrawLine(graphics, tmpPen, EndPoint, tmpPtE);
				//   tmpPen = Pens.Lime;
				//   GdiHelpers.DrawPoint(graphics, tmpPen, newRadiusPtPos, 3);
				//   GdiHelpers.DrawPoint(graphics, tmpPen, newGluePtAPos, 3);
				//   GdiHelpers.DrawPoint(graphics, tmpPen, newGluePtBPos, 3);
				//}

				#endregion

#endif
			}
			base.Draw(graphics);
		}


		/// <override></override>
		public override void DrawOutline(Graphics graphics, Pen pen)
		{
			base.DrawOutline(graphics, pen);
			if (IsLine) graphics.DrawLine(pen, StartPoint, EndPoint);
			else {
				Debug.Assert(Geometry.IsValid(arcBounds));
				Debug.Assert(!float.IsNaN(StartAngle));
				Debug.Assert(!float.IsNaN(SweepAngle));
				graphics.DrawArc(pen, arcBounds, StartAngle, SweepAngle);
			}
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
				using (CircularArcBase arc = (CircularArcBase) this.Clone()) {
					// Prepare shape
					if (VertexCount < 3) arc.InsertVertex(ControlPointId.LastVertex, 0, 0);
					ControlPointId radPtId = arc.GetNextVertexId(ControlPointId.FirstVertex);
					Point p = new Point(r.Right, r.Top + r.Height/2);
					PointF c = new PointF(r.Left + (r.Width/2f), r.Top + (r.Height/2f));
					// Move control points
					Point newPos;
					newPos = Point.Round(Geometry.RotatePoint(c, 135, p));
					arc.MoveControlPointTo(ControlPointId.FirstVertex, newPos.X, newPos.Y, ResizeModifiers.None);
					newPos = Point.Round(Geometry.RotatePoint(c, 270, p));
					arc.MoveControlPointTo(radPtId, newPos.X, newPos.Y, ResizeModifiers.None);
					newPos = Point.Round(Geometry.RotatePoint(c, 45, p));
					arc.MoveControlPointTo(ControlPointId.LastVertex, newPos.X, newPos.Y, ResizeModifiers.None);
					// Draw shape
					arc.Draw(g);
				}
			}
		}

		#endregion

		#region [Protected] Methods

		/// <ToBeCompleted></ToBeCompleted>
		protected internal CircularArcBase(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal CircularArcBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}


		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);
			for (int i = ControlPointCount - 1; i >= 0; --i) {
				LineControlPoint ctrlPoint = GetControlPoint(i);
				if (ctrlPoint is VertexControlPoint)
					ctrlPoint.SetPosition(i*10, i*10);
			}
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangleCore(bool tight)
		{
			// Calcualte line caps' bounds
			Rectangle result = Rectangle.Empty;
			float halfLineWidth = LineStyle.LineWidth/2f;
			float delta = halfLineWidth + 0.2f;
			if (IsLine) {
				result.X = (int) Math.Floor(Math.Min(StartPoint.X, EndPoint.X) - halfLineWidth);
				result.Y = (int) Math.Floor(Math.Min(StartPoint.Y, EndPoint.Y) - halfLineWidth);
				result.Width = (int) Math.Ceiling(Math.Max(StartPoint.X, EndPoint.X) + halfLineWidth) - result.X;
				result.Height = (int) Math.Ceiling(Math.Max(StartPoint.Y, EndPoint.Y) + halfLineWidth) - result.Y;
			}
			else {
				if (arcIsInvalid) RecalculateArc();

				float left = Center.X - Radius - halfLineWidth;
				float top = Center.Y - Radius - halfLineWidth;
				float right = Center.X + Radius + halfLineWidth;
				float bottom = Center.Y + Radius + halfLineWidth;

				if (Geometry.ArcContainsPoint(StartPoint.X, StartPoint.Y, RadiusPoint.X, RadiusPoint.Y, EndPoint.X, EndPoint.Y,
				                              Center.X, Center.Y, Radius, left, Center.Y, delta))
					result.X = (int) Math.Floor(left);
				else result.X = (int) Math.Floor(Math.Min(StartPoint.X, EndPoint.X) - halfLineWidth);

				if (Geometry.ArcContainsPoint(StartPoint.X, StartPoint.Y, RadiusPoint.X, RadiusPoint.Y, EndPoint.X, EndPoint.Y,
				                              Center.X, Center.Y, Radius, Center.X, top, delta))
					result.Y = (int) Math.Floor(top);
				else result.Y = (int) Math.Floor(Math.Min(StartPoint.Y, EndPoint.Y) - halfLineWidth);

				if (Geometry.ArcContainsPoint(StartPoint.X, StartPoint.Y, RadiusPoint.X, RadiusPoint.Y, EndPoint.X, EndPoint.Y,
				                              Center.X, Center.Y, Radius, right, Center.Y, delta))
					result.Width = (int) Math.Ceiling(right) - result.X;
				else result.Width = (int) Math.Ceiling(Math.Max(StartPoint.X, EndPoint.X) + halfLineWidth) - result.X;

				if (Geometry.ArcContainsPoint(StartPoint.X, StartPoint.Y, RadiusPoint.X, RadiusPoint.Y, EndPoint.X, EndPoint.Y,
				                              Center.X, Center.Y, Radius, Center.X, bottom, delta))
					result.Height = (int) Math.Ceiling(bottom) - result.Y;
				else result.Height = (int) Math.Ceiling(Math.Max(StartPoint.Y, EndPoint.Y) + halfLineWidth) - result.Y;
			}
			return result;
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y)
		{
			if (base.ContainsPointCore(x, y))
				return true;
			float lineContainsDelta = (int) Math.Ceiling(LineStyle.LineWidth/2f) + 2;
			if (IsLine)
				return Geometry.LineContainsPoint(StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y, true, x, y, lineContainsDelta);
			else {
				if (Geometry.IsValid(Center))
					return Geometry.ArcContainsPoint(StartPoint.X, StartPoint.Y, RadiusPoint.X, RadiusPoint.Y, EndPoint.X, EndPoint.Y,
					                                 Center.X, Center.Y, Radius, x, y, lineContainsDelta);
				else
					return Geometry.ArcContainsPoint(
						StartPoint.X, StartPoint.Y,
						RadiusPoint.X, RadiusPoint.Y,
						EndPoint.X, EndPoint.Y,
						lineContainsDelta, x, y);
			}
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height)
		{
			Rectangle rectangle = Rectangle.Empty;
			rectangle.X = x;
			rectangle.Y = y;
			rectangle.Width = width;
			rectangle.Height = height;
			if (IsLine) {
				if (Geometry.RectangleIntersectsWithLine(rectangle,
				                                         StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y, true))
					return true;
			}
			else {
				if (arcIsInvalid) RecalculateArc();
				// Calculate Points for outer and inner arc (bounds of the arc's line)
				float lineRadius = LineStyle.LineWidth/2f;
				int innerRadius = (int) Math.Floor(Radius - lineRadius);
				int outerRadius = (int) Math.Ceiling(Radius + lineRadius);
				Point innerStartPt = Point.Round(Geometry.IntersectCircleWithLine(Center, innerRadius, StartPoint, Center, false));
				Point innerRadPt = Point.Round(Geometry.IntersectCircleWithLine(Center, innerRadius, RadiusPoint, Center, false));
				Point innerEndPt = Point.Round(Geometry.IntersectCircleWithLine(Center, innerRadius, EndPoint, Center, false));
				Point outerStartPt = Point.Round(Geometry.IntersectCircleWithLine(Center, outerRadius, StartPoint, Center, false));
				Point outerRadPt = Point.Round(Geometry.IntersectCircleWithLine(Center, outerRadius, RadiusPoint, Center, false));
				Point outerEndPt = Point.Round(Geometry.IntersectCircleWithLine(Center, outerRadius, EndPoint, Center, false));
				return (Geometry.ArcIntersectsWithRectangle(innerStartPt.X, innerStartPt.Y, innerRadPt.X, innerRadPt.Y, innerEndPt.X,
				                                            innerEndPt.Y, Center.X, Center.Y, innerRadius, rectangle)
				        ||
				        Geometry.ArcIntersectsWithRectangle(outerStartPt.X, outerStartPt.Y, outerRadPt.X, outerRadPt.Y, outerEndPt.X,
				                                            outerEndPt.Y, Center.X, Center.Y, outerRadius, rectangle));
			}
			return false;
		}


		/// <override></override>
		protected override bool MoveByCore(int deltaX, int deltaY)
		{
			// move cap bounds and cap points			
			if (base.MoveByCore(deltaX, deltaY)) {
				if (Geometry.IsValid(center)) {
					center.X += deltaX;
					center.Y += deltaY;
				}
				return true;
			}
			else {
				InvalidateDrawCache();
				return false;
			}
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers)
		{
			int pointIndex = GetControlPointIndex(pointId);

			float radiusPtAngle = 0;
			bool maintainAspect = false;
			ControlPointId otherGluePtId = ControlPointId.None;
			ControlPointId nextPtId = ControlPointId.None;
			if ((modifiers & ResizeModifiers.MaintainAspect) == ResizeModifiers.MaintainAspect
			    && (IsFirstVertex(pointId) || IsLastVertex(pointId))) {
				maintainAspect = true;
				// Get opposite glue point and the glue point next to the moved glue point
				// (may be identical to the opposite glue point)
				if (IsFirstVertex(pointId)) {
					otherGluePtId = ControlPointId.LastVertex;
					nextPtId = GetPreviousVertexId(otherGluePtId);
				}
				else {
					otherGluePtId = ControlPointId.FirstVertex;
					nextPtId = GetNextVertexId(otherGluePtId);
				}
				// Calculate the original angle for later use
				radiusPtAngle = Geometry.RadiansToDegrees(Geometry.Angle(
					GetControlPointPosition(pointId),
					GetControlPointPosition(otherGluePtId),
					GetControlPointPosition(nextPtId)));
			}

			// Assign new position to vertex
			GetControlPoint(pointIndex).Offset(deltaX, deltaY);

			if (maintainAspect) {
				int radPointIdx = GetControlPointIndex(radiusPointId);
				if (IsLine) {
					MaintainGluePointPosition(otherGluePtId, nextPtId);
					if (VertexCount > 2)
						GetControlPoint(radPointIdx).SetPosition(Geometry.VectorLinearInterpolation(StartPoint, EndPoint, 0.5f));
				}
				else {
					// Try to maintain angle between StartPoint and RadiusPoint
					Point movedPtPos = GetControlPointPosition(pointId);
					Point otherGluePtPos = GetControlPointPosition(otherGluePtId);
					int hX = otherGluePtPos.X;
					int hY = otherGluePtPos.Y;
					Geometry.RotatePoint(movedPtPos.X, movedPtPos.Y, radiusPtAngle, ref hX, ref hY);

					int aPb, bPb, cPb; // perpendicular bisector
					int aR, bR, cR; // line through start point and radius point
					Geometry.CalcPerpendicularBisector(movedPtPos.X, movedPtPos.Y, otherGluePtPos.X, otherGluePtPos.Y, out aPb, out bPb,
					                                   out cPb);
					Geometry.CalcLine(movedPtPos.X, movedPtPos.Y, hX, hY, out aR, out bR, out cR);

					Point newPos = Geometry.IntersectLines(aPb, bPb, cPb, aR, bR, cR);
					Debug.Assert(Geometry.IsValid(newPos));
					if (Geometry.IsValid(newPos))
						GetControlPoint(radPointIdx).SetPosition(newPos);

					// After moving the point between the glue points, we have to recalculate the glue point
					// positions again
					MaintainGluePointPosition(ControlPointId.LastVertex, GetPreviousVertexId(ControlPointId.LastVertex));
					MaintainGluePointPosition(ControlPointId.FirstVertex, GetNextVertexId(ControlPointId.FirstVertex));
				}
			}
			else {
				MaintainGluePointPosition(ControlPointId.FirstVertex, pointId);
				MaintainGluePointPosition(ControlPointId.LastVertex, pointId);
			}
			return true;
		}


		/// <override></override>
		protected override Point CalcGluePoint(ControlPointId gluePointId, Shape shape)
		{
			if (IsLine) {
				ControlPointId secondPtId = (gluePointId == ControlPointId.FirstVertex)
				                            	? ControlPointId.LastVertex
				                            	: ControlPointId.FirstVertex;
				// If the line only has 2 vertices and both are connected via Point-To-Shape connection...
				if (IsConnected(ControlPointId.FirstVertex, null) == ControlPointId.Reference
				    && IsConnected(ControlPointId.LastVertex, null) == ControlPointId.Reference) {
					// ... calculate new point position from the position of the second shape:
					Shape secondShape = GetConnectionInfo(secondPtId, null).OtherShape;
					return CalcGluePointFromPosition(gluePointId, shape, secondShape.X, secondShape.Y);
				}
				else {
					Point p = GetControlPointPosition(secondPtId);
					Point result = CalcGluePointFromPosition(gluePointId, shape, p.X, p.Y);
					return result;
				}
			}
			else {
				Point pos = CalcGluePointCalculationBase(gluePointId, shape);
				return CalcGluePointFromPosition(gluePointId, shape, pos.X, pos.Y);
			}
		}


		/// <override></override>
		protected override float CalcCapAngle(ControlPointId pointId)
		{
			if (drawCacheIsInvalid) RecalcShapePoints();
			if (IsLine) {
				Pen pen = ToolCache.GetPen(LineStyle, StartCapStyleInternal, EndCapStyleInternal);
				return ShapeUtils.CalcLineCapAngle(shapePoints, pointId, pen);
			}
			else {
				if (pointId == ControlPointId.FirstVertex)
					return CalcArcCapAngle(GetControlPointIndex(pointId), StartCapStyleInternal.CapSize);
				else if (pointId == ControlPointId.LastVertex)
					return CalcArcCapAngle(GetControlPointIndex(pointId), EndCapStyleInternal.CapSize);
				else throw new NotSupportedException();
			}
		}


		/// <override></override>
		protected override void InvalidateDrawCache()
		{
			base.InvalidateDrawCache();
			arcIsInvalid = true;
			center = Geometry.InvalidPointF;
			radius = float.NaN;
			arcStartAngle = float.NaN;
			arcSweepAngle = float.NaN;
			arcBounds = Geometry.InvalidRectangleF;
		}


		/// <override></override>
		protected override void UpdateDrawCache()
		{
			base.UpdateDrawCache();
			//if (arcIsInvalid) RecalcDrawCache();
		}


		/// <override></override>
		protected override void RecalcDrawCache()
		{
			RecalcShapePoints();
			// Recalculate arc parameters (RecalculateArc does nothing if IsLine is true)
			if (arcIsInvalid) RecalculateArc();
			// Calculate boundingRectangle of the arc (required for drawing and invalidating)
			arcBounds.X = center.X - X - radius;
			arcBounds.Y = center.Y - Y - radius;
			arcBounds.Width = arcBounds.Height = Math.Max(0.1f, radius + radius);

			base.RecalcDrawCache();
		}


		/// <override></override>
		protected override void TransformDrawCache(int deltaX, int deltaY, int deltaAngle, int rotationCenterX,
		                                           int rotationCenterY)
		{
			base.TransformDrawCache(deltaX, deltaY, deltaAngle, rotationCenterX, rotationCenterY);
			if (Geometry.IsValid(arcBounds)) arcBounds.Offset(deltaX, deltaY);
		}


		/// <override></override>
		protected override void InsertControlPoint(int index, LineShapeBase.LineControlPoint controlPoint)
		{
			if (controlPoint is RelativeLineControlPoint) {
				RelativePosition relPos = ((RelativeLineControlPoint) controlPoint).RelativePosition;
				relPos.B = 0;
				((RelativeLineControlPoint) controlPoint).RelativePosition = relPos;
			}
			base.InsertControlPoint(index, controlPoint);
		}


		//protected override int CalcRelativeConnectionPointPosition(int x, int y) {
		//    int result = int.MinValue;

		//    float range = (int)Math.Ceiling(LineStyle.LineWidth / 2f);
		//    float ptRelPos = 0;
		//    float lineLength = 0;
		//    int maxPtIdx = VertexCount - 1;
		//    for (int i = 0; i < maxPtIdx; ++i) {
		//        if (Geometry.LineContainsPoint(vertices[i].X, vertices[i].X, vertices[i + 1].X, vertices[i + 1].Y, true, x, y, range)) {
		//            float ptDist = Geometry.DistancePointPoint(vertices[i].X, vertices[i].Y, x, y);
		//            ptRelPos = lineLength + ptDist;
		//        }
		//        lineLength += Geometry.DistancePointPoint(vertices[i], vertices[i + 1]);
		//    }
		//    result = (int)Math.Round((lineLength / ptRelPos) * 1000);
		//    return result;
		//}


		//protected override Point CalcAbsoluteConnectionPointPosition(LineControlPoint connectionPointDef) {
		//    Point result = Geometry.InvalidPoint;
		//    // Calculate line's length
		//    float lineLength = 0;
		//    int maxPtIdx = VertexCount - 1;
		//    for (int i = 0; i < maxPtIdx; ++i)
		//        lineLength += Geometry.DistancePointPoint(vertices[i], vertices[i + 1]);
		//    // Calculate absolute Position from relative position
		//    float relPos = lineLength * (connectionPointDef.Position / 1000f);
		//    for (int i = 0; i < maxPtIdx; ++i) {
		//        float segmentLength = Geometry.DistancePointPoint(vertices[i], vertices[i + 1]);
		//        if (segmentLength < relPos)
		//            relPos -= segmentLength;
		//        else {
		//            float t = segmentLength / relPos;
		//            result = Geometry.VectorLinearInterpolation(vertices[i], vertices[i + 1], t);
		//            break;
		//        }
		//    }
		//    // Validate result
		//    if (!Geometry.IsValid(result)) {
		//        Debug.Fail("Failed to calculate a valid ConntectionPoint position! Using fallback value.");
		//        result.X = X; result.Y = Y;
		//    }
		//    return result;
		//}

		#endregion

		#region [Private] Properties

		/// <summary>
		/// returns the coordinates of the arc's start point. 
		/// The start point is defined as the first point of the arc (clockwise) and the underlying point id changes depending on the position of the other points.
		/// </summary>
		private Point StartPoint
		{
			get { return GetControlPointPosition(ControlPointId.FirstVertex); }
		}


		/// <summary>
		/// returns the coordinates of the arc's end point. 
		/// The end point is defined as the first point of the arc (clockwise) and the underlying point id changes depending on the position of the other points.
		/// </summary>
		private Point EndPoint
		{
			get { return GetControlPointPosition(ControlPointId.LastVertex); }
		}


		/// <summary>
		/// This point defines the radius of the arc and the start point and end points. 
		/// The radius point is located between start point and end point of the arc per definition. 
		/// Which of the points are regarded as start point or end point depends on the position of the radius point.
		/// </summary>
		private Point RadiusPoint
		{
			get
			{
				if (VertexCount == 2)
					return Geometry.VectorLinearInterpolation(StartPoint, EndPoint, 0.5f);
				else if (VertexCount == 3) {
					if (radiusPointId == ControlPointId.None) {
						// If radiusPointId is not set, find it now.
						for (int i = ControlPointCount - 2; i > 0; --i) {
							LineControlPoint ctrlPoint = GetControlPoint(i);
							if (ctrlPoint is VertexControlPoint) {
								radiusPointId = ctrlPoint.Id;
								break;
							}
						}
					}
					return GetControlPointPosition(radiusPointId);
				}
				else throw new IndexOutOfRangeException();
			}
		}


		private PointF Center
		{
			get
			{
				if (IsLine) center = Geometry.VectorLinearInterpolation((PointF) StartPoint, (PointF) EndPoint, 0.5f);
				else if (arcIsInvalid) RecalculateArc();
				return center;
			}
		}


		private float StartAngle
		{
			get
			{
				if (arcIsInvalid) RecalculateArc();
				return arcStartAngle;
			}
		}


		private float SweepAngle
		{
			get
			{
				if (arcIsInvalid) RecalculateArc();
				return arcSweepAngle;
			}
		}


		private float Radius
		{
			get
			{
				if (IsLine) return float.PositiveInfinity;
				else if (arcIsInvalid) RecalculateArc();
				return radius;
			}
		}


		private bool IsLine
		{
			get
			{
				return (VertexCount == 2
				        || StartPoint == EndPoint
				        || Geometry.LineContainsPoint(StartPoint, EndPoint, false, RadiusPoint, 0.1f));
			}
		}

		#endregion

		#region [Private] Methods

		private IEnumerator<MenuItemDef> GetBaseActions(int mouseX, int mouseY, int range)
		{
			return base.GetMenuItemDefs(mouseX, mouseY, range).GetEnumerator();
		}


		private void RecalcShapePoints()
		{
			// Reset draw cache to origin (for later transformation)
			if (shapePoints.Length != VertexCount) Array.Resize<Point>(ref shapePoints, VertexCount);
			int i = -1;
			foreach (ControlPointId pointId in GetControlPointIds(ControlPointCapabilities.Resize)) {
				Point p = GetControlPointPosition(pointId);
				p.Offset(-X, -Y);
				shapePoints[++i] = p;
			}
		}


		private void RecalculateArc()
		{
			if (!IsLine) {
				Point startPt = StartPoint;
				Point radiusPt = RadiusPoint;
				Point endPt = EndPoint;
				Geometry.CalcCircumCircle(startPt.X, startPt.Y, radiusPt.X, radiusPt.Y, endPt.X, endPt.Y, out center, out radius);
				if (Geometry.IsValid(center)) {
					// Calculate center point and radius
					CalculateAngles(center, startPt, radiusPt, endPt, out arcStartAngle, out arcSweepAngle);
					// Calculate arc's bounds
					arcBounds.X = center.X - radius;
					arcBounds.Y = center.Y - radius;
					arcBounds.Width = arcBounds.Height = radius + radius;
					arcIsInvalid = false;
				}
			}
		}


		private bool CalculateAngles(PointF centerPt, Point startPt, Point radiusPt, Point endPt, out float startAngle,
		                             out float sweepAngle)
		{
			if (!Geometry.IsValid(centerPt)) throw new ArgumentException("centerPt");
			if (!Geometry.IsValid(startPt)) throw new ArgumentException("startPt");
			if (!Geometry.IsValid(endPt)) throw new ArgumentException("endPt");
			if (!Geometry.IsValid(radiusPt)) throw new ArgumentException("radiusPt");
			startAngle = sweepAngle = float.NaN;

			// Get sorted id's and positions of vertices
			ControlPointId startPtId, endPtId;
			Point arcStartPt, arcEndPt;
			GetAngleCalculationStartAndEndPointId(centerPt, startPt, radiusPt, endPt, out startPtId, out arcStartPt, out endPtId,
			                                      out arcEndPt);

			// Calculate angles
			sweepAngle =
				Geometry.RadiansToDegrees(Geometry.Angle(centerPt.X, centerPt.Y, arcStartPt.X, arcStartPt.Y, arcEndPt.X, arcEndPt.Y));
			if (sweepAngle < 0) sweepAngle = (360 + sweepAngle)%360;
			if (startPtId == ControlPointId.FirstVertex) {
				startAngle = Geometry.RadiansToDegrees(Geometry.Angle(centerPt.X, centerPt.Y, arcStartPt.X, arcStartPt.Y));
				if (startAngle < 0) startAngle = (360 + startAngle)%360;
			}
			else {
				// if startPt and endPt were swapped, invert the sweepAngle - otherwise the line cap will be drawn on the wrong side.
				startAngle = Geometry.RadiansToDegrees(Geometry.Angle(centerPt.X, centerPt.Y, arcEndPt.X, arcEndPt.Y));
				if (startAngle < 0) startAngle = (360 + startAngle)%360;
				sweepAngle = -sweepAngle;
			}
			return true;
		}


		private void GetAngleCalculationStartAndEndPointId(PointF centerPt, Point startPt, Point radiusPt, Point endPt,
		                                                   out ControlPointId startPtId, out Point startPtPos,
		                                                   out ControlPointId endPtId, out Point endPtPos)
		{
			startPtId = ControlPointId.None;
			endPtId = ControlPointId.None;
			startPtPos = Geometry.InvalidPoint;
			endPtPos = Geometry.InvalidPoint;

			// Calculate vertex angles
			float p0Angle = Geometry.Angle(centerPt.X, centerPt.Y, startPt.X, startPt.Y);
			float p1Angle = Geometry.Angle(centerPt.X, centerPt.Y, radiusPt.X, radiusPt.Y);
			float p2Angle = Geometry.Angle(centerPt.X, centerPt.Y, endPt.X, endPt.Y);

			// Sort vertices in order to calculate start- and sweep angle
			if (p0Angle >= 0 && p1Angle >= 0 && p2Angle >= 0) {
				//===============================================================================
				// Case 1: All angles positive
				//
				if (p0Angle < p1Angle && p1Angle < p2Angle) {
					startPtId = ControlPointId.FirstVertex;
					startPtPos = startPt;
					endPtId = ControlPointId.LastVertex;
					endPtPos = endPt;
				}
				else if (p0Angle >= p1Angle && p1Angle >= p2Angle) {
					startPtId = ControlPointId.LastVertex;
					startPtPos = endPt;
					endPtId = ControlPointId.FirstVertex;
					endPtPos = startPt;
				}
				else {
					if (p0Angle < p2Angle) {
						startPtId = ControlPointId.LastVertex;
						startPtPos = endPt;
						endPtId = ControlPointId.FirstVertex;
						endPtPos = startPt;
					}
					else {
						startPtId = ControlPointId.FirstVertex;
						startPtPos = startPt;
						endPtId = ControlPointId.LastVertex;
						endPtPos = endPt;
					}
				}
			}
			else if (p0Angle <= 0 && p1Angle <= 0 && p2Angle <= 0) {
				//===============================================================================
				// Case 2: All angles negative
				//
				if (p0Angle < p1Angle && p1Angle < p2Angle) {
					startPtId = ControlPointId.FirstVertex;
					startPtPos = startPt;
					endPtId = ControlPointId.LastVertex;
					endPtPos = endPt;
				}
				else if (p0Angle >= p1Angle && p1Angle >= p2Angle) {
					startPtId = ControlPointId.LastVertex;
					startPtPos = endPt;
					endPtId = ControlPointId.FirstVertex;
					endPtPos = startPt;
				}
				else {
					if (p0Angle > p2Angle) {
						startPtId = ControlPointId.FirstVertex;
						startPtPos = startPt;
						endPtId = ControlPointId.LastVertex;
						endPtPos = endPt;
					}
					else {
						startPtId = ControlPointId.LastVertex;
						startPtPos = endPt;
						endPtId = ControlPointId.FirstVertex;
						endPtPos = startPt;
					}
				}
			}
			else if (p0Angle >= 0 && p1Angle >= 0 && p2Angle < 0) {
				//===============================================================================
				// Case 3: startPt's angle positive, radiusPt's angle positive, endPt's angle negative
				//
				if (p0Angle < p1Angle) {
					startPtId = ControlPointId.FirstVertex;
					startPtPos = startPt;
					endPtId = ControlPointId.LastVertex;
					endPtPos = endPt;
				}
				else {
					startPtId = ControlPointId.LastVertex;
					startPtPos = endPt;
					endPtId = ControlPointId.FirstVertex;
					endPtPos = startPt;
				}
			}
			else if (p0Angle >= 0 && p1Angle < 0 && p2Angle < 0) {
				//===============================================================================
				// Case 4: startPt's angle positive, radiusPt's angle negative, endPt's angle negative
				//
				if (p1Angle < p2Angle) {
					startPtId = ControlPointId.FirstVertex;
					startPtPos = startPt;
					endPtId = ControlPointId.LastVertex;
					endPtPos = endPt;
				}
				else {
					startPtId = ControlPointId.LastVertex;
					startPtPos = endPt;
					endPtId = ControlPointId.FirstVertex;
					endPtPos = startPt;
				}
			}
			else if (p0Angle < 0 && p1Angle < 0 && p2Angle >= 0) {
				//===============================================================================
				// Case 5: startPt's angle negative, radiusPt's angle negative, endPt's angle positive
				//
				if (p0Angle < p1Angle) {
					startPtId = ControlPointId.FirstVertex;
					startPtPos = startPt;
					endPtId = ControlPointId.LastVertex;
					endPtPos = endPt;
				}
				else {
					startPtId = ControlPointId.LastVertex;
					startPtPos = endPt;
					endPtId = ControlPointId.FirstVertex;
					endPtPos = startPt;
				}
			}
			else if (p0Angle < 0 && p1Angle >= 0 && p2Angle >= 0) {
				//===============================================================================
				// Case 6: startPt's angle negative, radiusPt's angle positive, endPt's angle positive
				//
				if (p1Angle < p2Angle) {
					startPtId = ControlPointId.FirstVertex;
					startPtPos = startPt;
					endPtId = ControlPointId.LastVertex;
					endPtPos = endPt;
				}
				else {
					startPtId = ControlPointId.LastVertex;
					startPtPos = endPt;
					endPtId = ControlPointId.FirstVertex;
					endPtPos = startPt;
				}
			}
			else if (p0Angle >= 0 && p1Angle < 0 && p2Angle >= 0) {
				//===============================================================================
				// Case 7: startPt's angle positive, radiusPt's angle negative, endPt's angle positive
				//
				if (p0Angle < p2Angle) {
					startPtId = ControlPointId.LastVertex;
					startPtPos = endPt;
					endPtId = ControlPointId.FirstVertex;
					endPtPos = startPt;
				}
				else {
					startPtId = ControlPointId.FirstVertex;
					startPtPos = startPt;
					endPtId = ControlPointId.LastVertex;
					endPtPos = endPt;
				}
			}
			else if (p0Angle < 0 && p1Angle >= 0 && p2Angle < 0) {
				//===============================================================================
				// Case 8: startPt's angle negative, radiusPt's angle positive, endPt's angle negative
				//
				if (p0Angle < p2Angle) {
					startPtId = ControlPointId.LastVertex;
					startPtPos = endPt;
					endPtId = ControlPointId.FirstVertex;
					endPtPos = startPt;
				}
				else {
					startPtId = ControlPointId.FirstVertex;
					startPtPos = startPt;
					endPtId = ControlPointId.LastVertex;
					endPtPos = endPt;
				}
			}
			else if (float.IsNaN(p0Angle) && float.IsNaN(p1Angle) && float.IsNaN(p2Angle)) {
				//===============================================================================
				// Case 9: No Solution: Arc is not defined
				//
				startPtId = ControlPointId.FirstVertex;
				startPtPos = startPt;
				endPtId = ControlPointId.LastVertex;
				endPtPos = endPt;
				throw new NShapeInternalException("Unable to calculate drawCache.");
			}
			else throw new NShapeInternalException("Unable to calculate drawCache.");
		}


		private float CalcArcCapAngle(int pointIndex, float capSize)
		{
			Debug.Assert(!IsLine);
			if (Geometry.DistancePointPoint(StartPoint, EndPoint) < capSize)
				capSize = Geometry.DistancePointPoint(StartPoint, EndPoint);
			LineControlPoint ctrlPoint = GetControlPoint(pointIndex);
			foreach (
				PointF p in
					Geometry.IntersectCircleArc((float) ctrlPoint.GetPosition().X, (float) ctrlPoint.GetPosition().Y, capSize,
					                            StartPoint.X, StartPoint.Y, RadiusPoint.X, RadiusPoint.Y, EndPoint.X, EndPoint.Y,
					                            Center.X, Center.Y, Radius)) {
				if (Geometry.ArcContainsPoint(StartPoint.X, StartPoint.Y, RadiusPoint.X, RadiusPoint.Y, EndPoint.X, EndPoint.Y,
				                              Center.X, Center.Y, Radius, p.X, p.Y, 1)) {
					float angle = Geometry.RadiansToDegrees(Geometry.Angle(ctrlPoint.GetPosition(), p));
					return angle;
				}
			}
			return 0;
		}


		private Point CalcGluePointCalculationBase(ControlPointId gluePointId, Shape shape)
		{
			Point result = Geometry.InvalidPoint;
			if (!IsLine) {
				// Calculate a new radius for the arc:
				// A circle through the 2 end points of the arc (or the centers of the shapes they are connected to)
				// and the arc's radius point.
				ShapeConnectionInfo startCi = GetConnectionInfo(ControlPointId.FirstVertex, null);
				ShapeConnectionInfo endCi = GetConnectionInfo(ControlPointId.LastVertex, null);
				Point startPt = Point.Empty, endPt = Point.Empty;
				if (startCi.IsEmpty) startPt.Offset(StartPoint.X, StartPoint.Y);
				else startPt.Offset(startCi.OtherShape.X, startCi.OtherShape.Y);
				if (endCi.IsEmpty) endPt.Offset(EndPoint.X, EndPoint.Y);
				else endPt.Offset(endCi.OtherShape.X, endCi.OtherShape.Y);
				float r;
				PointF centerPt;
				Geometry.CalcCircumCircle(startPt, RadiusPoint, endPt, out centerPt, out r);
				//
				// Calculate tangent on the desired arc through the other shape's center
				Point tangentPt = IsFirstVertex(gluePointId) ? startPt : endPt;
				float a, b, c;
				Geometry.CalcPerpendicularLine(centerPt.X, centerPt.Y, tangentPt.X, tangentPt.Y, out a, out b, out c);
				float aT, bT, cT;
				Geometry.TranslateLine(a, b, c, tangentPt, out aT, out bT, out cT);
				//
				// Calculate intersection point of the calculated tangent and the perpendicular bisector 
				// of the line through startPt and endPt
				Geometry.CalcPerpendicularBisector(startPt.X, startPt.Y, endPt.X, endPt.Y, out a, out b, out c);
				PointF pT = Geometry.IntersectLines(aT, bT, cT, a, b, c);
				if (Geometry.IsValid(pT)) {
					PointF pB = Geometry.VectorLinearInterpolation(startPt, endPt, 0.5f);
					result = Point.Round(Geometry.VectorLinearInterpolation(pB, pT, 0.75f));
					// Check if the calculated point is on the right side
					if (
						!Geometry.ArcIntersectsWithLine(startPt.X, startPt.Y, RadiusPoint.X, RadiusPoint.Y, endPt.X, endPt.Y, result.X,
						                                result.Y, centerPt.X, centerPt.Y, true))
						result = Geometry.VectorLinearInterpolation(result, tangentPt, 2);
				}
			}
			// If the arc only has 2 points or something went wrong while calculating the desired arc
			if (!Geometry.IsValid(result)) result = Geometry.VectorLinearInterpolation(StartPoint, EndPoint, 0.5f);
			return result;
		}


		private int CalcRelativeAngleFromPoint(Point p)
		{
			if (arcIsInvalid) RecalculateArc();
			// Calculate a new radius point between the StartPoint and p
			PointF tmpP = Geometry.VectorLinearInterpolation(StartPoint.X, StartPoint.Y, p.X, p.Y, 0.5f);
			Point rp = Point.Round(Geometry.CalcPointOnLine(Center.X, Center.Y, tmpP.X, tmpP.Y, Radius));
			float pointStartAngle, pointSweepAngle;
			CalculateAngles(Center, StartPoint, rp, p, out pointStartAngle, out pointSweepAngle);
			// Encode angle to the given point as percentage of the current sweepangle 
			int result = (int) Math.Round(Math.Abs(pointSweepAngle)/(Math.Abs(SweepAngle)/100f))*10;
			return result;
		}


		private Point CalcPointFromRelativeAngle(int relativeAngle)
		{
			return CalcPointFromRelativeAngle(relativeAngle, 0);
		}


		private Point CalcPointFromRelativeAngle(int relativeAngle, int radiusOffset)
		{
			// Ensure that all arc parameters are calculated properly
			if (arcIsInvalid) RecalculateArc();

			float startAng, sweepAng;
			CalculateAngles(Center, StartPoint, RadiusPoint, EndPoint, out startAng, out sweepAng);

			// Calculate absolute sweep angle from relative position (percentate of sweep angle)
			float angleToPt = sweepAng*(relativeAngle/1000f);

			// Calculate absolute position on the arc
			float ptRadius = Radius + (radiusOffset/10f);
			float x = Center.X + ptRadius;
			float y = Center.Y;
			Geometry.RotatePoint(Center.X, Center.Y, startAng + angleToPt, ref x, ref y);
			PointF p = Geometry.RotatePoint(Center, angleToPt, StartPoint);
			//if (Math.Abs(p.X - x) > 0.1f || Math.Abs(p.Y- y) > 0.1f) { }

			Point result = Point.Empty;
			result.Offset((int) Math.Round(x), (int) Math.Round(y));
#if DEBUG
			RelativePosition relPos = CalculateRelativePosition(result.X, result.Y);
			if (!(Math.Abs(relPos.A - relativeAngle) < 10 && Math.Abs(relPos.B - radiusOffset) < 10)) {
			}
#endif
			return result;
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

			Rectangle cells = Geometry.InvalidRectangle;
			if (Geometry.IsValid(capBounds) && !capBounds.IsEmpty) {
				cells.Location = ShapeUtils.CalcCell(capBounds.Location, cellSize);
				cells.Width = (capBounds.Right/cellSize) - cells.X;
				cells.Height = (capBounds.Bottom/cellSize) - cells.Y;
			}
			else {
				Point p = GetControlPointPosition(pointId);
				cells.Location = ShapeUtils.CalcCell(p.X - lineRadius, p.Y - lineRadius, cellSize);
				p = ShapeUtils.CalcCell(p.X + lineRadius, p.Y + lineRadius, cellSize);
				cells.Width = p.X - cells.X;
				cells.Height = p.Y - cells.Y;
			}
			return cells;
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


		private bool IsCellOccupied(Rectangle occupiedCells, Point cell)
		{
			return ((occupiedCells.Width >= 0 && occupiedCells.Height >= 0)
			        && (occupiedCells.Location == cell || Geometry.RectangleContainsPoint(occupiedCells, cell.X, cell.Y)));
		}

		#endregion

		#region Fields

		// Property buffers
		private bool arcIsInvalid = true;
		private PointF center = Geometry.InvalidPointF;
		private float radius = float.NaN;
		private ControlPointId radiusPointId = ControlPointId.None;

		// Draw cache
		private float arcStartAngle = float.NaN;
		private float arcSweepAngle = float.NaN;
		private RectangleF arcBounds = Geometry.InvalidRectangleF;

		#endregion
	}
}