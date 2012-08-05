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


namespace Dataweb.NShape.Advanced {

	/// <summary>
	/// Base class for lines that connect their vertices with rectangular lines.
	/// </summary>
	public abstract class RectangularLineBase : LineShapeBase {

		#region Shape Members

		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override void CopyFrom(Shape source) {
			base.CopyFrom(source);
			if (source is PolylineBase) {
				// Vertices and CapStyles will be copied by the base class
				// so there's nothing left to do here...
			}
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override void Fit(int x, int y, int width, int height) {
			Rectangle bounds = GetBoundingRectangle(true);
			// First, scale to the desired size
			//float scale;
			//scale = Geometry.CalcScaleFactor(bounds.Width, bounds.Height, width, height);

			// Second, move to the desired location
			Point topLeft = Point.Empty;
			topLeft.Offset(x, y);
			Point bottomRight = Point.Empty;
			bottomRight.Offset(x + width, y + height);
			MoveControlPointTo(ControlPointId.FirstVertex, topLeft.X, topLeft.Y, ResizeModifiers.None);
			MoveControlPointTo(ControlPointId.LastVertex, bottomRight.X, bottomRight.Y, ResizeModifiers.None);

			int ptNr = 0;
			foreach (ControlPointId ptId in GetControlPointIds(ControlPointCapabilities.Resize)) {
				if (IsFirstVertex(ptId) || IsLastVertex(ptId)) continue;
				ptNr = 1;
				Point pos = GetControlPointPosition(ptId);
				pos = Geometry.VectorLinearInterpolation(topLeft, bottomRight, ptNr / (float)(VertexCount - 1));
				MoveControlPointTo(ptId, pos.X, pos.Y, ResizeModifiers.None);
			}
			InvalidateDrawCache();
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override int X {
			get { return GetControlPoint(0).GetPosition().X; }
			set {
				int origValue = GetControlPoint(0).GetPosition().X;
				if (!MoveTo(value, Y)) MoveTo(origValue, Y);
			}
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override int Y {
			get { return GetControlPoint(0).GetPosition().Y; }
			set {
				int origValue = GetControlPoint(0).GetPosition().Y;
				if (!MoveTo(X, value)) MoveTo(X, origValue);
			}
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		/// <remarks>See <see cref="CalculateRelativePosition">CalcRelativePosition</see> for definition of relative positions for rectangular lines.</remarks>
		public override Point CalculateAbsolutePosition(RelativePosition relativePosition) {
			if (relativePosition == RelativePosition.Empty) throw new ArgumentOutOfRangeException("relativePosition");
			Point result = Point.Empty;
			//
			// Find the relevant segment for this relative position.
			Point knee1, knee2;
			FindSegment(relativePosition.A, out knee1, out knee2);
			//
			// Calculate the perpendicular foot
			int fX = knee1.X + relativePosition.B * (knee2.X - knee1.X) / 100;
			int fY = knee1.Y + relativePosition.B * (knee2.Y - knee1.Y) / 100;
			// 
			// Calculate absolute position on the perpendicular in distance relativePosition.C
			int pX, pY;
			Geometry.CalcNormalVectorOfLine(knee1.X, knee1.Y, knee2.X, knee2.Y, fX, fY, relativePosition.C, out pX, out pY);
			//
			result.Offset(pX, pY);
			return result;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		/// <remarks>The relative position of a point P with respsect to a RectangularLine is defined as
		/// A = Index of the nearest line segment (FirstVertex = 0 -> LastVertex)
		/// B = Position of the base point of the perpendicular from the position onto the nearest line segment 
		///     as the percentage of the line segment length.
		/// C = Distance between the point and the base point.
		/// </remarks>
		public override RelativePosition CalculateRelativePosition(int x, int y) {
			if (!Geometry.IsValid(x, y)) throw new ArgumentOutOfRangeException("x / y");
			RelativePosition result = RelativePosition.Empty;
			Point p = Point.Empty;

			// Find the nearest line segment
			int segmentIndex;
			Point knee1, knee2;
			FindNearestLineSegment(x, y, out segmentIndex, out knee1, out knee2);

			// magicIdx now holds the index of knee2 of the nearest line segment and prevMagicIdx the index of knee1.
			// Calculate the base point of the perpendicular
			int fX;
			int fY;
			Geometry.CalcDroppedPerpendicularFoot(x, y, knee1.X, knee1.Y, knee2.X, knee2.Y, out fX, out fY);
			result.A = segmentIndex;
			// B is the distance of p to knee1 in percent of the total length
			result.B = 100 * Geometry.DistancePointPointFast(fX, fY, knee1.X, knee1.Y) / Geometry.DistancePointPointFast(knee2.X, knee2.Y, knee1.X, knee1.Y);
			// C is the distance of p to knee1 - knee2
			result.C = Geometry.DistancePointPointFast(x, y, fX, fY);
			return result;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		/// <remarks>Connection foot is simple the nearest point on the nearest segment</remarks>
		public override Point CalculateConnectionFoot(int fromX, int fromY) {
			Point result = Point.Empty;
			//
			// Find the nearest line segment
			int segmentIndex;
			Point knee1, knee2;
			FindNearestLineSegment(fromX, fromY, out segmentIndex, out knee1, out knee2);
			//
			int x, y;
			Geometry.CalcNearestPointOfLineSegment(knee1.X, knee1.Y, knee2.X, knee2.Y, fromX, fromY, out x, out y);
			result.Offset(x, y);
			return result;
		}


		///// <summary>Overriden method. Check base class for documentation.</summary>
		//public override Point CalculateConnectionFoot(int x1, int y1, int x2, int y2) {
		//    return CalculateConnectionFoot(x1, y1);
		//}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override ControlPointId HitTest(int x, int y, ControlPointCapabilities controlPointCapability, int range) {
			ControlPointId result = ControlPointId.None;
			//
			// We first search for a hit control point
			for (int cpIdx = 0; cpIdx < ControlPointCount; ++cpIdx) {
				// Get positions of the current control points and test on hit.
				LineControlPoint ctrlPoint = GetControlPoint(cpIdx);
				Point currPos = ctrlPoint.GetPosition();
				if (Geometry.DistancePointPoint(x, y, currPos.X, currPos.Y) <= range) {
					if (HasControlPointCapability(ctrlPoint.Id, controlPointCapability))
						result = GetControlPointId(cpIdx);
					break;
				}
			}
			if (result == ControlPointId.None) {
				int lineRange = (int)Math.Ceiling(LineStyle.LineWidth / 2f) + 1;
				Point lastKnee;
				int cpIdx = -1;
				FindNextKnee(ref cpIdx, out lastKnee);
				Point knee;
				while (FindNextKnee(ref cpIdx, out knee)) {
					if (Geometry.DistancePointLine(x, y, lastKnee.X, lastKnee.Y, knee.X, knee.Y, true) <= lineRange) {
						result = ControlPointId.Reference;
						break;
					}
					lastKnee = knee;
				}
			}
			return result;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		// TODO 2: This implementation might also be used by PolylineBase.
		public override Point CalcNormalVector(Point point) {
			Point result = Point.Empty;
			int segmentIndex;
			Point knee1, knee2;
			FindNearestLineSegment(point.X, point.Y, out segmentIndex, out knee1, out knee2);
			int rX, rY;
			Geometry.CalcNormalVectorOfLine(knee1.X, knee1.Y, knee2.X, knee2.Y, point.X, point.Y, 100, out rX, out rY);
			result.Offset(rX, rY);
			return result;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override void Invalidate() {
			base.Invalidate();
			if (DisplayService != null) {
				Point knee1, knee2;
				int cpIdx = -1;
				FindNextKnee(ref cpIdx, out knee1);
				while (FindNextKnee(ref cpIdx, out knee2)) {
					InvalidateLineSegment(knee1, knee2);
					knee1 = knee2;
				}
			}
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		// TODO 2: This function is identical to the one in Polyline.
		public override void Draw(Graphics graphics) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			UpdateDrawCache();
			int lastIdx = shapePoints.Length - 1;
			if (lastIdx > 0) {
				// Draw background of line caps
				DrawStartCapBackground(graphics, shapePoints[0].X, shapePoints[0].Y);
				DrawEndCapBackground(graphics, shapePoints[lastIdx].X, shapePoints[lastIdx].Y);
				// Draw Line
				Pen pen = ToolCache.GetPen(LineStyle, StartCapStyleInternal, EndCapStyleInternal);
				DrawOutline(graphics, pen);
				// ToDo: If the line is connected to another line, draw a connection indicator (ein Bommel oder so)
				// ToDo: Add a property for enabling/disabling this feature
			}
			base.Draw(graphics);
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override void DrawOutline(Graphics graphics, Pen pen) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			if (pen == null) throw new ArgumentNullException("pen");
			base.DrawOutline(graphics, pen);
			graphics.DrawLines(pen, shapePoints);
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override void DrawThumbnail(Image image, int margin, Color transparentColor) {
			if (image == null) throw new ArgumentNullException("image");
			using (Graphics g = Graphics.FromImage(image)) {
				GdiHelpers.ApplyGraphicsSettings(g, RenderingQuality.MaximumQuality);
				g.Clear(transparentColor);

				int startCapSize = IsShapedLineCap(StartCapStyleInternal) ? StartCapStyleInternal.CapSize : 0;
				int endCapSize = IsShapedLineCap(EndCapStyleInternal) ? EndCapStyleInternal.CapSize : 0;

				int s = (int)Math.Max(startCapSize, endCapSize) * 4;
				int width = Math.Max(image.Width * 2, s);
				int height = Math.Max(image.Height * 2, s);
				float scale = Math.Max((float)image.Width / width, (float)image.Height / height);
				g.ScaleTransform(scale, scale);

				int m = (int)Math.Round(Math.Max(margin / scale, Math.Max(startCapSize / 2f, endCapSize / 2f)));
				Rectangle r = Rectangle.Empty;
				r.Width = width; r.Height = height;
				r.Inflate(-m, -m);
				// Create and draw shape
				using (RectangularLineBase line = (RectangularLineBase)this.Clone()) {
					while (line.VertexCount > 2) line.RemoveVertex(line.GetPreviousVertexId(ControlPointId.LastVertex));
					// Move vertices
					int dh = r.Height / 8;
					line.MoveControlPointTo(ControlPointId.FirstVertex, r.Left, r.Top + dh, ResizeModifiers.None);
					line.MoveControlPointTo(ControlPointId.LastVertex, r.Right, r.Bottom - dh, ResizeModifiers.None);
					// Add vertices
					line.InsertVertex(ControlPointId.LastVertex, r.Left + r.Width / 2, r.Top + r.Height / 2);
					line.Draw(g);
				}
			}
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected internal override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected override void InvalidateDrawCache() {
			base.InvalidateDrawCache();
			shapePointsAreInvalid = drawCacheIsInvalid;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected internal override IEnumerable<Point> CalculateCells(int cellSize) {
			Point result = Point.Empty;
			// Base class call
			foreach (Point p in base.CalculateCommonCells(cellSize))
				yield return p;
			// Now add the cells of the lines. We do not care about duplicate entries. They are theoretically
			// possible but should be rare in real life.
			RecalcBendings();
			int range = (int)Math.Ceiling(LineStyle.LineWidth / 2f);
			int cpIdx = -1;
			Point knee1;
			FindNextKnee(ref cpIdx, out knee1);
			Point knee2;
			while (FindNextKnee(ref cpIdx, out knee2)) {
				// -- Adds the cells between knee1 and knee2 --
				// Starting point is one cell before the first to return.
				result = ShapeUtils.CalcCell(knee1, cellSize);
				result.Offset(-Math.Sign(knee2.X - knee1.X), -Math.Sign(knee2.Y - knee1.Y));

				// If the line lies on a cell border, we have to return two cells at each location.
				int otherDeltaX = 0;
				int otherDeltaY = 0;
				if (knee2.X == knee1.X) {
					// Vertical line
					int x = Math.Abs(knee1.X % cellSize);
					if (x - range < 0) otherDeltaX = -1;
					else if (x + range >= cellSize) otherDeltaX = +1;
				} else if (knee2.Y == knee1.Y) {
					// Horizontal line
					int y = Math.Abs(knee1.Y % cellSize);
					if (y - range < 0) otherDeltaY = -1;
					else if (y + range >= cellSize) otherDeltaY = +1;
				} else Debug.Assert(false);
				//
				// Now advance from the starting point to the end point return the cells.
				// If intersects with the neighbour cell (otherDelta != 0), return the additional cells.
				Point knee2Cell = ShapeUtils.CalcCell(knee2, cellSize);
				do {
					result.X += Math.Sign(knee2.X - knee1.X);
					result.Y += Math.Sign(knee2.Y - knee1.Y);
					yield return result;
					// If line on boundary add second cell
					if (otherDeltaX != 0 || otherDeltaY != 0)
						yield return new Point(result.X + otherDeltaX, result.Y + otherDeltaY);
				} while (result != knee2Cell);
				knee1 = knee2;
			}
		}

		#endregion


		#region ILinearShape Members

		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override int MinVertexCount {
			get { return 2; }
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override int MaxVertexCount {
			get { return int.MaxValue; }
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override ControlPointId InsertVertex(ControlPointId beforePointId, int x, int y) {
			ControlPointId newPointId = ControlPointId.None;
			if (IsFirstVertex(beforePointId) || beforePointId == ControlPointId.Reference || beforePointId == ControlPointId.None)
				throw new ArgumentException(string.Format("{0} is not a valid control point id for this operation.", beforePointId));

			// Find position where to insert the new point
			int pointIndex = GetControlPointIndex(beforePointId);
			if (pointIndex < 0 || pointIndex > ControlPointCount)
				throw new IndexOutOfRangeException();

			// Create new vertex
			newPointId = GetNewControlPointId();
			return InsertVertex(beforePointId, newPointId, x, y);
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override ControlPointId InsertVertex(ControlPointId beforePointId, ControlPointId newVertexId, int x, int y) {
			ControlPointId newPointId = ControlPointId.None;
			if (IsFirstVertex(beforePointId) || beforePointId == ControlPointId.Reference || beforePointId == ControlPointId.None)
				throw new ArgumentException(string.Format("{0} is not a valid control point id for this operation.", beforePointId));

			// Find position where to insert the new point
			int pointIndex = GetControlPointIndex(beforePointId);
			if (pointIndex < 0 || pointIndex > ControlPointCount)
				throw new IndexOutOfRangeException();

			// Create new vertex
			newPointId = GetNewControlPointId();
			Invalidate();
			InsertControlPoint(pointIndex, new RectVertexControlPoint(newPointId, x, y));
			RecalcBendings();

			// Update the reference vertices of the dynamic connection points.
			ControlPointId refVertexId = newPointId;
			for (int i = pointIndex; i < ControlPointCount; ++i) {
				LineControlPoint ctrlPoint = GetControlPoint(i);
				if (ctrlPoint is VertexControlPoint) {
					refVertexId = ctrlPoint.Id;
				} else if (ctrlPoint is DynamicConnectionPoint) {
					RelativePosition relPos = ((DynamicConnectionPoint)ctrlPoint).RelativePosition;
					relPos.A = refVertexId;
					((DynamicConnectionPoint)ctrlPoint).RelativePosition = relPos;
				}
			}

			InvalidateDrawCache();
			Invalidate();
			return newPointId;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override ControlPointId AddVertex(int x, int y) {
			Invalidate();

			// ToDo: Falls die Distanz des Punktes x|y > 0 ist: Ausrechnen wo der Punkt sein muss (entlang der Lotrechten durch den Punkt verschieben)
			ControlPointId insertBeforeId = FindInsertionPointId(x, y, true);
			ControlPointId result = InsertVertex(insertBeforeId, x, y);

			if (result == ControlPointId.None) throw new NShapeException("Cannot add vertex {0}.", new Point(x, y));
			Invalidate();
			return result;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override void RemoveVertex(ControlPointId controlPointId) {
			Invalidate();
			InvalidateDrawCache();

			if (controlPointId == ControlPointId.Any || controlPointId == ControlPointId.Reference || controlPointId == ControlPointId.None)
				throw new ArgumentException(string.Format("{0} is not a valid ControlPointId for this operation.", controlPointId));
			if (IsFirstVertex(controlPointId) || IsLastVertex(controlPointId))
				throw new InvalidOperationException(string.Format("ControlPoint {0} is a GluePoint and therefore must not be removed.", controlPointId));

			// Maintain relative positions of dynamic control points
			ControlPointId prevVertexId = GetPreviousVertexId(controlPointId);
			for (int i = ControlPointCount - 1; i >= 0; --i) {
				LineControlPoint ctrlPoint = GetControlPoint(i);
				if (ctrlPoint is DynamicConnectionPoint) {
					DynamicConnectionPoint dynPoint = (DynamicConnectionPoint)ctrlPoint;
					if (dynPoint.RelativePosition.A == controlPointId) {
						RelativePosition relPos = dynPoint.RelativePosition;
						relPos.A = prevVertexId;
						SetControlPoint(i, new DynamicConnectionPoint(this, dynPoint.Id, relPos));
					}
				}
			}
			// remove shape point
			int idx = GetControlPointIndex(controlPointId);
			RemoveControlPoint(idx);

			ControlPointsHaveMoved();
			Invalidate();
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		public override ControlPointId AddConnectionPoint(int x, int y) {
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


		#region Protected Types

		/// <summary>
		/// Defines how a segment of the rectangular line is bent.
		/// </summary>
		protected enum VertexBending {
			/// <summary>No bending defined</summary>
			None,
			/// <summary>Line segment goes upwards.</summary>
			Up,
			/// <summary>Line segment goes downwards.</summary>
			Down,
			/// <summary>Line segment goes to the right.</summary>
			Right,
			/// <summary>Line segment goes to the left.</summary>
			Left,
			/// <summary>Line segment is the base of an upwards open U.</summary>
			UpU,
			/// <summary>Line segment is the base of a downwards open U.</summary>
			DownU,
			/// <summary>Line segment is the base of a rightsided open U.</summary>
			RightU,
			/// <summary>Line segment is the base of a leftsided open U.</summary>
			LeftU
		}


		/// <summary>
		/// Defines a vertex with bending information.
		/// </summary>
		protected class RectVertexControlPoint : VertexControlPoint {
			/// <ToBeCompleted></ToBeCompleted>
			public RectVertexControlPoint(AbsoluteLineControlPoint source) : base(source) { }
			/// <ToBeCompleted></ToBeCompleted>
			public RectVertexControlPoint(ControlPointId pointId, Point position) : base(pointId, position) { }
			/// <ToBeCompleted></ToBeCompleted>
			public RectVertexControlPoint(ControlPointId pointId, int x, int y) : base(pointId, x, y) { }

			/// <ToBeCompleted></ToBeCompleted>
			public override LineControlPoint Clone() {
				return new RectVertexControlPoint(this);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override void CopyFrom(LineControlPoint source) {
				base.CopyFrom(source);
				if (source is RectVertexControlPoint)
					this.bending = ((RectVertexControlPoint)source).bending;
			}

			/// <summary>
			/// Indicates whether the vertex has a vertical or a horizontal line segment
			/// </summary>
			public bool IsVertical {
				get { return bending == VertexBending.Up || bending == VertexBending.Down || bending == VertexBending.RightU || bending == VertexBending.LeftU; }
			}

			/// <summary>
			/// Indicates the bending in this vertex.
			/// </summary>
			public VertexBending bending;

		}

		#endregion


		#region New Protected Methods

		/// <summary>
		/// A hopefully faster iterator over the vertices who does not need to create an object each time.
		/// </summary>
		protected bool FindNextVertex(ref int controlPointIndex, out RectVertexControlPoint vertex) {
			bool result = false;
			LineControlPoint lcp = null;
			while (controlPointIndex < ControlPointCount - 1) {
				++controlPointIndex;
				lcp = GetControlPoint(controlPointIndex);
				if (lcp is RectVertexControlPoint) {
					result = true;
					break;
				}
			}
			vertex = (RectVertexControlPoint)lcp;
			return result;
		}


		/// <summary>
		/// A hopefully faster iterator over the vertices who does not need to create an object each time.
		/// </summary>
		protected bool FindPrevVertex(ref int controlPointIndex, out RectVertexControlPoint vertex) {
			bool result = false;
			LineControlPoint lcp = null;
			while (controlPointIndex > 0) {
				--controlPointIndex;
				lcp = GetControlPoint(controlPointIndex);
				if (lcp is RectVertexControlPoint) {
					result = true;
					break;
				}
			}
			vertex = (RectVertexControlPoint)lcp;
			return result;
		}


		/// <summary>
		/// A hopefully faster iterator over the knee points.
		/// Start with -1 and make no assumptions regardings its meaning.
		/// </summary>
		// Currently the magicIndex is twice the controlPointIndex, but that might change.
		protected bool FindNextKnee(ref int magicIndex, out Point knee) {
			bool result = false;
			RectVertexControlPoint rvcp;
			if (magicIndex < 0) {
				FindNextVertex(ref magicIndex, out rvcp);
				// This must be successful
				knee = rvcp.GetPosition();
				magicIndex *= 2;
				result = true;
			} else if (magicIndex < 2 * ControlPointCount) {
				if (magicIndex % 2 != 0) {
					// The current vertex requires two knees. The second one is calculated here.
					int newIndex = magicIndex / 2;
					// This must succeed, it did the last time.
					FindNextVertex(ref newIndex, out rvcp);
					RectVertexControlPoint prevVertex = (RectVertexControlPoint)GetControlPoint(magicIndex / 2);
					knee = rvcp.GetPosition();
					if (rvcp.IsVertical)
						knee.Y = (prevVertex.GetPosition().Y + rvcp.GetPosition().Y) / 2;
					else knee.X = (prevVertex.GetPosition().X + rvcp.GetPosition().X) / 2;
					magicIndex = newIndex * 2;
					result = true;
				} else {
					// Finished with vertex, advance to next one.
					int newIndex = magicIndex / 2;
					if (FindNextVertex(ref newIndex, out rvcp)) {
						// If this vertex and the previous one have the same orientation, we must add two knees
						RectVertexControlPoint prevVertex = (RectVertexControlPoint)GetControlPoint(magicIndex / 2);
						if (rvcp.IsVertical == prevVertex.IsVertical) {
							// We need two knees, the first one is calculated here
							knee = prevVertex.GetPosition();
							if (rvcp.IsVertical)
								knee.Y = (prevVertex.GetPosition().Y + rvcp.GetPosition().Y) / 2;
							else knee.X = (prevVertex.GetPosition().X + rvcp.GetPosition().X) / 2;
							magicIndex += 1;
						} else {
							// One knee is sufficient
							if (rvcp.IsVertical) {
								knee = rvcp.GetPosition();
								knee.Y = GetControlPoint(magicIndex / 2).GetPosition().Y;
							} else {
								knee = rvcp.GetPosition();
								knee.X = GetControlPoint(magicIndex / 2).GetPosition().X;
							}
							magicIndex = newIndex * 2;
						}
						result = true;
					} else {
						// End point
						knee = GetControlPoint(magicIndex / 2).GetPosition();
						magicIndex = ControlPointCount * 2;
						result = true;
					}
				}
			} else knee = Geometry.InvalidPoint;
			return result;
		}


		/// <summary>
		/// A hopefully faster iterator over the line segments.
		/// </summary>
		protected bool FindNextSegment(ref int magicIndex, ref Point p1, ref Point p2) {
			if (magicIndex < 0)
				FindNextKnee(ref magicIndex, out p1);
			else p1 = p2;
			return FindNextKnee(ref magicIndex, out p2);
		}


		/// <summary>
		/// Finds the segment of the line, which is nearest to the point.
		/// </summary>
		protected void FindNearestLineSegment(int x, int y, out int segmentIndex, out Point knee1, out Point knee2) {
			// Search nearest segment
			int magicIdx = -1;
			FindNextKnee(ref magicIdx, out knee1);
			int bestMagicIdx = -1;
			int bestD = int.MaxValue;
			while (FindNextKnee(ref magicIdx, out knee2)) {
				int d = (int)Geometry.DistancePointLine(x, y, knee1.X, knee1.Y, knee2.X, knee2.Y, true);
				if (d < bestD) {
					bestMagicIdx = magicIdx;
					bestD = d;
				}
				knee1 = knee2;
			}
			// Find left and right point of best segment
			segmentIndex = -1;
			magicIdx = -1;
			FindNextKnee(ref magicIdx, out knee2);
			do {
				knee1 = knee2;
				FindNextKnee(ref magicIdx, out knee2);
				++segmentIndex;
			} while (magicIdx != bestMagicIdx);
		}


		/// <summary>
		/// Finds the segment with the given index.
		/// </summary>
		protected void FindSegment(int segmentIndex, out Point p1, out Point p2) {
			int segIdx = -1;
			p1 = Geometry.InvalidPoint;
			p2 = Geometry.InvalidPoint;
			int magicIdx = -1;
			do {
				FindNextSegment(ref magicIdx, ref p1, ref p2);
				++segIdx;
			} while (segIdx != segmentIndex);
		}


		/// <summary>
		/// Rotates the bending 90 degree clockwise.
		/// </summary>
		protected VertexBending RotateBending(VertexBending bending, int count) {
			VertexBending result = bending;
			for (int i = 0; i < count; ++i) {
				switch (result) {
					case VertexBending.None:
						result = VertexBending.None;
						break;
					case VertexBending.Up:
						result = VertexBending.Right;
						break;
					case VertexBending.Right:
						result = VertexBending.Down;
						break;
					case VertexBending.Down:
						result = VertexBending.Left;
						break;
					case VertexBending.Left:
						result = VertexBending.Up;
						break;
					case VertexBending.UpU:
						result = VertexBending.RightU;
						break;
					case VertexBending.RightU:
						result = VertexBending.DownU;
						break;
					case VertexBending.DownU:
						result = VertexBending.LeftU;
						break;
					case VertexBending.LeftU:
						result = VertexBending.UpU;
						break;
					default:
						Debug.Assert(false);
						result = VertexBending.None;
						break;
				}
			}
			return result;
		}


		/// <summary>
		/// Rotates the v1 around origin by 90 degree clockwise.
		/// </summary>
		protected void RotateVertex(RectVertexControlPoint v, RectVertexControlPoint origin, int count) {
			Point p = v.GetPosition();
			Point o = origin.GetPosition();
			for (int i = 0; i < count; ++i) {
				int x = p.X;
				p.X = o.X - (p.Y - o.Y);
				p.Y = o.Y + (x - o.X);
				v.bending = RotateBending(v.bending, 1);
			}
			v.SetPosition(p);
		}


		/// <summary>
		/// Rotates the vertics around v2 until v3 is in the first quadrant.
		/// </summary>
		/// <returns>Returns the number of clockwise rotations by 90 degree necessary.</returns>
		/// <remarks>Note that our coordinate system is mirrored:
		/// III | IV
		/// ----|----
		///  II | I
		/// </remarks>
		protected int RotateVertices(RectVertexControlPoint v1, RectVertexControlPoint v2, RectVertexControlPoint v3) {
			int result = 0;
			while (Geometry.CalcRelativeQuadrant(v3.GetPosition(), v2.GetPosition()) != 1) {
				RotateVertex(v3, v2, 1);
				if (v1 != null) RotateVertex(v1, v2, 1);
				v2.bending = RotateBending(v2.bending, 1);
				++result;
			}
			return result;
		}


		/// <summary>
		/// Rotates the vertices counterclockwise for the number of count quarter rotations.
		/// </summary>
		protected void UnrotateVertices(int count, RectVertexControlPoint v1, RectVertexControlPoint v2, RectVertexControlPoint v3) {
			// We rotate 4-count times clockwise
			for (int i = 0; i < (4 - count) % 4; ++i) {
				if (v1 != null) RotateVertex(v1, v2, 1);
				RotateVertex(v3, v2, 1);
				v2.bending = RotateBending(v2.bending, 1);
			}
		}


		/// <summary>
		/// Assignes the bending.
		/// </summary>
		protected void AssignBending(RectVertexControlPoint currentVertex, RectVertexControlPoint nextVertex) {
			int r = RotateVertices(null, currentVertex, nextVertex);
			switch (nextVertex.bending) {
				case VertexBending.None:
					if (currentVertex.IsVertical) nextVertex.bending = VertexBending.Right;
					else nextVertex.bending = VertexBending.Down;
					break;
				default:
					// We currently assume that this function is only called for unassigned bendings in nextVertex.
					Debug.Assert(false);
					break;
			}
			UnrotateVertices(r, null, currentVertex, nextVertex);
		}


		/// <summary>
		/// We assume that a valid bending has been assigned to all vertices from the start point up to and
		/// including v2
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="v3"></param>
		protected void AssignBending(RectVertexControlPoint v1, RectVertexControlPoint v2, RectVertexControlPoint v3) {
			// Rotate the points such that the connection from v2 to v3 is in the first quadrant
			int r = RotateVertices(v1, v2, v3);
			// 
			switch (Geometry.CalcRelativeQuadrant(v1.GetPosition(), v2.GetPosition())) {
				case 1: // this is forbidden
					// Debug.Assert(false);
					// If it happens nevertheless...
					switch (v3.bending) {
						case VertexBending.None:
							if (v2.IsVertical) v3.bending = VertexBending.Right;
							else v3.bending = VertexBending.Down;
							break;
						default:
							// We leave it as it is.
							break;
					}
					break;
				case 2: // 
					Debug.Assert(v2.bending == VertexBending.DownU);
					// If v3 is the end point, it could already have a bending
					switch (v3.bending) {
						case VertexBending.None:
							v3.bending = VertexBending.Down;
							break;
						case VertexBending.Down:
						case VertexBending.LeftU:
							// That is alright.
							break;
						case VertexBending.UpU:
						case VertexBending.Right:
							// This causes a double knee, but ok.
							break;
						case VertexBending.Left:
						case VertexBending.Up:
							// Conflict, we just override the bending
							v3.bending = VertexBending.Down;
							break;
						default:
							Debug.Assert(false);
							break;
					}
					break;
				case 3:
					// bending of v2 cannot be U
					switch (v3.bending) {
						case VertexBending.None:
							if (v2.IsVertical) v3.bending = VertexBending.Right;
							else v3.bending = VertexBending.Down;
							break;
						case VertexBending.Right:
						case VertexBending.Down:
							// That is alright.
							break;
						case VertexBending.LeftU:
							v2.bending = VertexBending.Right;
							break;
						case VertexBending.UpU:
							v2.bending = VertexBending.Down;
							break;
						case VertexBending.Up:
						case VertexBending.Left:
							// Conflict, we just override the bending
							if (v2.IsVertical) v3.bending = VertexBending.Right;
							else v3.bending = VertexBending.Down;
							break;
						default:
							Debug.Assert(false);
							break;
					}
					break;
				case 4:
					Debug.Assert(v2.bending == VertexBending.RightU);
					switch (v3.bending) {
						case VertexBending.None:
							v3.bending = VertexBending.Right;
							break;
						case VertexBending.Right:
						case VertexBending.Down:
						case VertexBending.LeftU:
						case VertexBending.UpU:
							// That is ok
							break;
						case VertexBending.Left:
						case VertexBending.Up:
							// Conflict, we just override the bending
							v3.bending = VertexBending.Right;
							break;
						default:
							Debug.Assert(false);
							break;
					}
					break;
				default:
					Debug.Assert(false);
					break;
			}
			UnrotateVertices(r, v1, v2, v3);
		}


		/// <summary>
		/// Calculates how the connections between the vertices bend.
		/// </summary>
		protected virtual void RecalcBendings() {
			// Initialize bendings
			int idx = -1;
			RectVertexControlPoint prevVertex1, prevVertex2;
			RectVertexControlPoint currentVertex;
			while (FindNextVertex(ref idx, out currentVertex))
				currentVertex.bending = VertexBending.None;
			//
			// If there is an invalid sequence (predecessor in same relative quadrant as successor),
			// we remove the point. This should happen in MoveControlPoint already.
			//idx = ControlPointCount;
			//FindPrevVertex(ref idx, out currentVertex);
			//FindPrevVertex(ref idx, out prevVertex2);
			//int middleIdx = idx;
			//while (FindPrevVertex(ref idx, out prevVertex1)) {
			//  if (Geometry.CalcRelativeQuadrant(prevVertex1, prevVertex2) == Geometry.CalcRelativeQuadrant(currentVertex, prevVertex2))
			//    RemoveControlPoint(middleIdx);
			//  currentVertex = prevVertex2;
			//  prevVertex2 = prevVertex1;
			//  middleIdx = idx;
			//}
			// Find the U-formed segment pairs, their bending is determined.
			int firstUIdx = -1;
			idx = -1;
			FindNextVertex(ref idx, out prevVertex1);
			FindNextVertex(ref idx, out prevVertex2);
			while (FindNextVertex(ref idx, out currentVertex)) {
				int quadrant1 = Geometry.CalcRelativeQuadrant(prevVertex1.GetPosition(), prevVertex2.GetPosition());
				int quadrant2 = Geometry.CalcRelativeQuadrant(currentVertex.GetPosition(), prevVertex2.GetPosition());
				// Note: Because y coordinate ascends downwards, quadrant I and II are below IV and III.
				if (quadrant1 + quadrant2 != 0) {
					// This means the relative quadrants are adjacent, i.e. prevVertex2 is a U base
					switch (quadrant1 + quadrant2) {
						case 3:
							prevVertex2.bending = VertexBending.DownU;
							if (firstUIdx < 0) firstUIdx = idx;
							break;
						case 5:
							// Could be 2 and 3 or 1 and 4
							if (quadrant1 == 2 || quadrant2 == 2)
								prevVertex2.bending = VertexBending.LeftU;
							else prevVertex2.bending = VertexBending.RightU;
							if (firstUIdx < 0) firstUIdx = idx;
							break;
						case 7:
							prevVertex2.bending = VertexBending.UpU;
							if (firstUIdx < 0) firstUIdx = idx;
							break;
						default:
							// Other situations do not determine the bending
							break;
					}
				}
				prevVertex1 = prevVertex2;
				prevVertex2 = currentVertex;
			}
			// If there is at least one U, all other directions follow. If there is none, we can choose one
			// for start point.
			if (firstUIdx >= 0) {
				// First we assign the directions from the first U backwards
				idx = firstUIdx;
				FindPrevVertex(ref idx, out prevVertex2);
				// prevVertex2 is the U vertex
				while (FindPrevVertex(ref idx, out prevVertex1)) {
					// If prevVertex2 is the U vertex, no conflict happens. If it is not, we can adjust prevVertex2
					AssignBending(prevVertex2, prevVertex1);
					// It is the other direction, so we have to flip the bendings
					prevVertex1.bending = RotateBending(prevVertex1.bending, 2);
					prevVertex2 = prevVertex1;
				}
			} else {
				// The first vertex's orientation is free. The following two vertices lie in the same relative quadrant.
				// We want to make the middle segment shorter than the other two together.
				idx = -1;
				FindNextVertex(ref idx, out prevVertex1);
				prevVertex1.bending = VertexBending.Right;
				FindNextVertex(ref idx, out prevVertex2);
				AssignBending(prevVertex1, prevVertex2);
			}
			// At this point at least the first two vertices have an orientation
			// Now we move forward through the line and assign an orientation to all vertices, solving conflicts
			// when we found them.
			idx = -1;
			FindNextVertex(ref idx, out prevVertex1);
			FindNextVertex(ref idx, out prevVertex2);
			// Since a U was found the first two vertices must be already defined
			Debug.Assert(prevVertex1.bending != VertexBending.None && prevVertex2.bending != VertexBending.None);
			while (FindNextVertex(ref idx, out currentVertex)) {
				AssignBending(prevVertex1, prevVertex2, currentVertex);
				prevVertex1 = prevVertex2;
				prevVertex2 = currentVertex;
			}
		}


		/// <summary>
		/// Invalidates a line segment between ptA and ptB
		/// </summary>
		protected void InvalidateLineSegment(Point ptA, Point ptB) {
			InvalidateLineSegment(ptA.X, ptA.Y, ptB.X, ptB.Y);
		}


		/// <summary>
		/// Invalidates a line segment between (x1, y1) and (x2, y2)
		/// </summary>
		protected void InvalidateLineSegment(int x1, int y1, int x2, int y2) {
			if (DisplayService != null) {
				int xMin, xMax, yMin, yMax;
				xMin = Math.Min(x1, x2);
				yMin = Math.Min(y1, y2);
				xMax = Math.Max(x1, x2);
				yMax = Math.Max(y1, y2);
				int margin = 1;
				if (LineStyle != null) margin = LineStyle.LineWidth + 1;
				DisplayService.Invalidate(xMin - margin, yMin - margin, (xMax - xMin) + (margin + margin), (yMax - yMin) + (margin + margin));
			}
		}

		#endregion


		#region Protected Constructors

		/// <ToBeCompleted></ToBeCompleted>
		protected internal RectangularLineBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
			// nothing to do
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal RectangularLineBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
			// nothing to do
		}

		#endregion


		#region protected LineShapeBase overrides

		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected override Point CalcGluePoint(ControlPointId gluePointId, Shape shape) {
			// We must calculate a start point where the last line segment into the shape begins.
			// This is the last knee of the rectangular line.
			RectVertexControlPoint endCtrlPt = (RectVertexControlPoint)GetControlPoint(GetControlPointIndex(gluePointId));
			ControlPointId prevCtrlPtId = ControlPointId.None;
			if (IsFirstVertex(gluePointId))
				prevCtrlPtId = GetNextVertexId(gluePointId);
			else if (IsLastVertex(gluePointId))
				prevCtrlPtId = GetPreviousVertexId(gluePointId);
			Debug.Assert(prevCtrlPtId != ControlPointId.None);
			RectVertexControlPoint rvcp = (RectVertexControlPoint)GetControlPoint(GetControlPointIndex(prevCtrlPtId));
			Point startPoint;
			if (endCtrlPt.IsVertical) {
				startPoint = rvcp.GetPosition();
				startPoint.X = shape.X;
			} else {
				startPoint = rvcp.GetPosition();
				startPoint.Y = shape.Y;
			}
			return CalcGluePointFromPosition(gluePointId, shape, startPoint.X, startPoint.Y);
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected override Rectangle CalculateBoundingRectangleCore(bool tight) {
			Rectangle result = Geometry.InvalidRectangle;
			result.Location = GetControlPointPosition(ControlPointId.FirstVertex);
			//
			// Add all vertices, they somtimes lie outside the line segments
			int cpIdx = -1;
			if (!tight) {
				RectVertexControlPoint rvcp;
				while (FindNextVertex(ref cpIdx, out rvcp))
					Geometry.IncludeRectanglePoint(ref result, rvcp.GetPosition());
			}
			//
			// Add all line segments
			RecalcBendings();
			Point knee;
			cpIdx = -1;
			while (FindNextKnee(ref cpIdx, out knee)) {
				if (!Geometry.IsValid(result)) {
					result.Location = knee;
					result.Size = Size.Empty;
				} else
					Geometry.IncludeRectanglePoint(ref result, knee);
			}
			// 
			ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
			return result;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected override bool ContainsPointCore(int x, int y) {
			return base.ContainsPointCore(x, y) || HitTest(x, y, ControlPointCapabilities.None, 0) != ControlPointId.None;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected override LineControlPoint CreateVertex(ControlPointId id, Point position) {
			return new RectVertexControlPoint(id, position);
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected override float CalcCapAngle(ControlPointId pointId) {
			float result = float.NaN;
			if (shapePointsAreInvalid) 
				RecalcShapePoints();
			Pen pen = ToolCache.GetPen(LineStyle, StartCapStyleInternal, EndCapStyleInternal);
			result = ShapeUtils.CalcLineCapAngle(shapePoints, pointId, pen);
			return result;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			Rectangle rectangle = Rectangle.Empty;
			rectangle.X = x;
			rectangle.Y = y;
			rectangle.Width = width;
			rectangle.Height = height;

			if (StartCapIntersectsWith(rectangle))
				return true;
			if (EndCapIntersectsWith(rectangle))
				return true;
			// Check all line segments
			int idx = -1;
			Point knee1 = Geometry.InvalidPoint;
			Point knee2 = Geometry.InvalidPoint;
			while (FindNextSegment(ref idx, ref knee1, ref knee2)) {
				if (Geometry.RectangleIntersectsWithLine(rectangle, knee1, knee2, true))
					return true;
			}
			return false;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected override bool MovePointByCore(ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers) {
			int idx = GetControlPointIndex(pointId);
			GetControlPoint(idx).Offset(deltaX, deltaY);
			return true;
		}


		/// <summary>Overriden method. Check base class for documentation.</summary>
		protected override void RecalcDrawCache() {
			RecalcShapePoints();
			base.RecalcDrawCache();
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Determines where an additional insertion point has to be added to the list
		/// </summary>
		/// <param name="x">X coordinate of new insertion point</param>
		/// <param name="y">Y coordinate of new insertion point</param>
		/// <param name="insertBefore">Indicates whether point is to inserted before or after the respective control point.</param>
		/// <returns></returns>
		private ControlPointId FindInsertionPointId(int x, int y, bool insertBefore) {
			// Find the point before which the new point has to be inserted
			ControlPointId result = ControlPointId.None;
			int range = (int)Math.Ceiling(LineStyle.LineWidth / 2f) + 1;
			int idx = -1;
			int prevIdx = idx;
			Point knee1;
			FindNextKnee(ref idx, out knee1);
			Point knee2;
			prevIdx = idx;
			while (FindNextKnee(ref idx, out knee2)) {
				if (Geometry.LineContainsPoint(knee1, knee2, true, x, y, range)) {
					// Index of control point that lies on the segment defined by knee1 and knee2
					int ctrlPtIdx = (idx + 1) / 2 - 1;
					// Decide whether (x, y) comes before or after the control point on this segement
					// Only a question if we are not on an intermediate double-knee
					if (idx - prevIdx == 2) {
						RectVertexControlPoint rvcp = (RectVertexControlPoint)GetControlPoint(ctrlPtIdx);
						if (rvcp.IsVertical) {
							if (Math.Sign(knee1.Y - rvcp.GetPosition().Y) == Math.Sign(y - rvcp.GetPosition().Y))
								--ctrlPtIdx;
						} else {
							if (Math.Sign(knee1.X - rvcp.GetPosition().X) == Math.Sign(x - rvcp.GetPosition().X))
								--ctrlPtIdx;
						}
					}
					if (insertBefore) ++ctrlPtIdx;
					result = GetControlPointId(ctrlPtIdx);
					break;
				}
				prevIdx = idx;
				knee1 = knee2;
			}
			if (result == ControlPointId.None) result = ControlPointId.LastVertex;
			return result;
		}


		private void RecalcShapePoints() {
			if (shapePointsAreInvalid) {
				RecalcBendings();
				//
				Point refPos = GetControlPointPosition(ControlPointId.Reference);
				int magicIdx = -1;
				int pointIdx = -1;
				Point knee;
				while (FindNextKnee(ref magicIdx, out knee)) {
					++pointIdx;
					if (pointIdx >= shapePoints.Length) Array.Resize(ref shapePoints, 2 * VertexCount + 1);
					knee.Offset(-refPos.X, -refPos.Y);
					shapePoints[pointIdx] = knee;
				}
				Array.Resize(ref shapePoints, pointIdx + 1);
				shapePointsAreInvalid = false;
			}
		}

		#endregion


		private bool shapePointsAreInvalid = true;
	}

}