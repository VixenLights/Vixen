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
using System.Text;
using System.Diagnostics;
using System.Drawing;

using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.Layouters {

	/// <summary>
	/// Orders the shapes into layers, such that most of the arrows point downwards and if possible also 
	/// only to the next layer.
	/// </summary>
	/// <remarks>
	/// All lines that have non-identical caps count as arrows from their first to their last vertex.
	/// Lines with idential caps count as arrows in both directions. Lines without caps do not count at all.
	/// We only regard point-to-shape connections. Point-to-point connections would required rotating 
	/// the shapes what we should/want/must not do in a layouter.
	///
	/// The algorithm consists of three phases:
	/// Optimize Levels: Moves single shapes up or down one layer if it improves its characteristics.
	/// Ordering: Orders the shapes within one layer such that lines do not cross so much.
	/// Positioning: Positions shapes within one layer such that there connected shapes are nearer.
	/// These phases are repeated until a stable state or the maximum number of steps is reached.
	/// </remarks>
	public class FlowLayouter : LayouterBase, ILayouter {

		/// <ToBeCompleted></ToBeCompleted>
		public enum FlowDirection {
			/// <ToBeCompleted></ToBeCompleted>
			BottomUp,
			/// <ToBeCompleted></ToBeCompleted>
			LeftToRight,
			/// <ToBeCompleted></ToBeCompleted>
			TopDown,
			/// <ToBeCompleted></ToBeCompleted>
			RightToLeft
		};

		/// <ToBeCompleted></ToBeCompleted>
		public FlowLayouter(Project project)
			: base(project) {
			shapeByXComparer = new ShapeByXComparer(this);
			shapeByValueComparer = new ShapeByValueComparer();
		}


		/// <ToBeCompleted></ToBeCompleted>
		public FlowDirection Direction {
			get { return flowDirection; }
			set { flowDirection = value; }
		}


		/// <summary>Indicates the distance between layers.</summary>
		public int LayerDistance {
			get { return layerDistance; }
			set {
				if (value <= 0) throw new ArgumentException("Layer distance must be greater than zero");
				layerDistance = value; 
			}
		}


		/// <summary>Indicates the distance between shapes within a layer.</summary>
		public int RowDistance {
			get { return rowDistance; }
			set {
				if (value <= 0) throw new ArgumentException("Row distance must be greater than zero");
				rowDistance = value;
			}
		}


		/// <override></override>
		public override string InvariantName {
			get { return "Flow"; }
		}


		/// <override></override>
		public override string Description {
			get { return "Orders the shapes such that the majority of the arrows points in a given direction."; }
		}


		/// <override></override>
		public override void Prepare() {
			base.Prepare();
			PrepareLayering();
			phase = Phase.OptimizeLevels;
		}


		/// <override></override>
		public override void Unprepare() {
			foreach (Shape s in selectedShapes)
				s.InternalTag = null;
		}


		/// <override></override>
		protected override bool ExecuteStepCore() {
			bool result = true;
			switch (phase) {
				case Phase.OptimizeLevels:
					if (!ImproveShapeDistribution()) {
						phase = Phase.Ordering;
						PrepareOrdering();
					}
					break;
				case Phase.Ordering:
					if (!OrderShapesWithinLayers()) {
						phase = Phase.Positioning;
						PreparePositioning();
					}
					break;
				case Phase.Positioning:
					result = PositionShapesWithinLayers();
					break;
				default:
					Debug.Fail("Unsupported phase in FlowLayouter.ExecuteStep");
					break;
			}
			return result;
		}


		// Calculates the fields firstLayerPos, layerDistance, layerCount and initially assigns shapes 
		// to layers.
		private void PrepareLayering() {
			Rectangle layoutRect = CalcLayoutArea();
			switch (flowDirection) {
				case FlowDirection.TopDown:
					firstLayerPos = layoutRect.Top;
					layerCount = layoutRect.Height / layerDistance;
					break;
				case FlowDirection.LeftToRight:
					firstLayerPos = layoutRect.Left;
					layerCount = layoutRect.Width / layerDistance;
					break;
				case FlowDirection.BottomUp:
					firstLayerPos = -layoutRect.Bottom;
					layerCount = layoutRect.Height / layerDistance;
					break;
				case FlowDirection.RightToLeft:
					firstLayerPos = -layoutRect.Right;
					layerCount = layoutRect.Width / layerDistance;
					break;
				default:
					Debug.Fail("Unexpected flow direction in PrepareLayering");
					break;
			}
			disturbanceLevel = layerCount / 2;
			disturbanceCount = 0;
			// Now assign initial layers
			foreach (Shape s in selectedShapes) {
				s.InternalTag = new LayerInfo();
				int fy = GetFlowY(s);
				int layerIndex = (fy - firstLayerPos + layerDistance / 2) / layerDistance;
				Debug.Assert(layerIndex >= 0);
				((LayerInfo)s.InternalTag).layer = layerIndex;
				if (layerIndex >= layerCount) layerCount = layerIndex + 1;
				MoveShapeFlowY(s, firstLayerPos + ((LayerInfo)s.InternalTag).layer * layerDistance);
			}
			// All direct neighbors of the selected shapes need a layer info too
			foreach (Shape s in selectedShapes) {
				foreach (Shape os in GetConnectedShapes(s, Orientation.None))
					if (os.InternalTag == null) {
						os.InternalTag = new LayerInfo();
						int fy = GetFlowY(s);
						int layerIndex = (fy - firstLayerPos + layerDistance / 2) / layerDistance;
						Debug.Assert(layerIndex >= 0);
						((LayerInfo)s.InternalTag).layer = layerIndex;
						if (layerIndex >= layerCount) layerCount = layerIndex + 1;
					}
			}
		}


		/// <summary>
		/// Uses gradient descent to distribute shapes over the layers in the best way.
		/// </summary>
		/// <returns>True, if the at least one shape has been moved from a layer to another.</returns>
		private bool ImproveShapeDistribution() {
			bool result = false;
			foreach (Shape s in selectedShapes) {
				if (ExcludeFromFitting(s)) continue;
				//
				// We calculate the evaluation function and check whether we can improve it by moving the shape up or down.
				float ovb = CalcOptimizationValue(s, 0);
				float ovdu = CalcOptimizationValue(s, -1);
				float ovdd = CalcOptimizationValue(s, +1);
				if (ovdu < ovb && ovdu < ovdd) {
					MoveShapeLayerFlow(s, -1);
					result = true;
				} else if (ovdd < ovb && ovdd < ovdu) {
					MoveShapeLayerFlow(s, +1);
					result = true;
				}
			}
			if (!result) result = PerformRandomMove();
			return result;
		}


		/// <summary>
		/// Returns true, if a move could be performed.
		/// </summary>
		private bool PerformRandomMove() {
			bool result;
			if (disturbanceLevel > 0) { 
				// Determine the number of layers (+1..+maximumLayerDelta)
				int layerDelta = random.Next(disturbanceLevel) + 1;
				// Determine the sign of the move
				if (random.Next(2) > 0) layerDelta = -layerDelta;
				// Determine a shape
				Shape shape;
				do {
					shape = selectedShapes[random.Next(selectedShapes.Count)];
				} while (ExcludeFromFitting(shape));
				// Check it is not moved out of the current layers.
				int shapeLayer = ((LayerInfo)shape.InternalTag).layer;
				if (shapeLayer + layerDelta < 0) layerDelta = -shapeLayer;
				else if (shapeLayer + layerDelta >= layerCount) layerDelta = layerCount - 1 - shapeLayer;
				MoveShapeLayerFlow(shape, layerDelta);
				// We perform a disturbance for every four shapes before we reduce the disturbance level.
				++disturbanceCount;
				if (disturbanceCount > selectedShapes.Count / 4) {
					--disturbanceLevel;
					disturbanceCount = 0;
				}
				result = true;
			} else result = false;
			return result;
		}


		// Creates the data structures for the layout on layer level and already sorts the top layer.
		private void PrepareOrdering() {
			layers.Clear();
			foreach (Shape s in selectedShapes) {
				if (ExcludeFromFitting(s)) continue;

				int li = ((LayerInfo)s.InternalTag).layer;
				while (layers.Count <= li)
					layers.Add(new List<Shape>());
				int i = layers[li].BinarySearch(s, shapeByXComparer);
				if (i < 0) i = ~i;
				layers[li].Insert(i, s);
			}
			// We to always calculated with positive row indexes especially in PositionShapesWithLayers.
			firstRowPos = -10000;
			phase = Phase.Ordering;
			positioningRun = 0;
			currentLayer = 1;
		}


		// Uses Sugiyamas idea of barycenters to sort shapes within the layer such that crossings are minimized.
		private bool OrderShapesWithinLayers() {
			// Do nothing if there is only one layer.
			if (currentLayer >= layers.Count) return false;
			int layerDelta = positioningRun % 2 == 0 ? -1 : +1;
			// Calculate the barycenter for the current layer. The barycenter is defined here as the average 
			// index of connected shapes. If a shape has no connection to the other layer, its barycenter is 
			// set to the joker value -1.
			foreach (Shape shape in layers[currentLayer]) {
				int bc = 0;
				int n = 0;
				foreach (Shape s in GetConnectedShapes(shape, Orientation.None)) {
					if (((LayerInfo)s.InternalTag).layer != currentLayer + layerDelta) continue;
					++n;
					bc += layers[currentLayer + layerDelta].IndexOf(s) + 1;
				}
				if (n == 0) ((LayerInfo)shape.InternalTag).value = -1;
				else ((LayerInfo)shape.InternalTag).value = (float)bc / n;
			}
			// Sort shapes after ascending barycenter values. In order to not move well positioned shapes 
			// unnecessarily, we insert the shapes geometrically.
			float mbc = 0; // Maximum barycenter
			for (int i = 0; i < layers[currentLayer].Count; ++i) {
				Shape shape = layers[currentLayer][i];
				if (((LayerInfo)shape.InternalTag).value < 0) {
					// Its a joker, set to fitting value.
					((LayerInfo)shape.InternalTag).value = mbc;
				} else if (((LayerInfo)shape.InternalTag).value < mbc) {
					// Sorted in the wrong order
					int index = layers[currentLayer].BinarySearch(0, i, shape, shapeByValueComparer);
					if (index < 0) index = ~index;
					layers[currentLayer].Remove(shape);
					layers[currentLayer].Insert(index, shape);
					if (index == 0) MoveShapeFlowX(shape, GetFlowX(layers[currentLayer][0]) - 10);
					else MoveShapeFlowX(shape, (GetFlowX(layers[currentLayer][index - 1]) + GetFlowX(layers[currentLayer][index])) / 2);
				} else {
					mbc = ((LayerInfo)shape.InternalTag).value;
				}
			}
			// Determine next layer to process
			if (layerDelta < 0) {
				++currentLayer;
				if (currentLayer >= layers.Count) {
					++positioningRun;
					currentLayer = layers.Count - 2;
				}
			} else {
				--currentLayer;
				if (currentLayer < 0) {
					++positioningRun;
					currentLayer = 1;
				}
			}
			// We are doing down - up - down
			return positioningRun < positioningRunCount;
		}


		// During positioning we assume that the row index corresponds directly to the shape index
		// through rowIdx == leastRowIdx + shapeIdx. This method ensures this constraint.
		// The order of shapes within the layer is not changed.
		private void PreparePositioning() {
			// First find the least row index. At the end rowIdx == leastRowIdx + shapeIdx will be valid for all shapes.
			int leastRowIdx = int.MaxValue;
			for (int layerIdx = 0; layerIdx < layers.Count; ++layerIdx) {
				for (int shapeIdx = 0; shapeIdx < layers[layerIdx].Count; ++shapeIdx) {
					Shape shape = layers[layerIdx][shapeIdx];
					int rowIdx = (GetFlowX(shape) - firstRowPos + rowDistance / 2) / rowDistance;
					if (rowIdx < leastRowIdx) leastRowIdx = rowIdx;
				}
			}
			// Now we insert as many null values at the beginning of each layer shape list such that the first
			// shape position corresponds to least row index.
			for (int layerIdx = 0; layerIdx < layers.Count; ++layerIdx) {
				if (layers[layerIdx].Count <= 0) continue;
				Shape firstShape = layers[layerIdx][0];
				int firstRowIdx = (GetFlowX(firstShape) - firstRowPos + rowDistance / 2) / rowDistance;
				for (int j = 0; j < firstRowIdx - leastRowIdx; ++j)
					layers[layerIdx].Insert(0, null);
				// All shapes in the layer have to be aligned to the row distance
				int nextFreeRowIdx = leastRowIdx;
				for (int shapeIdx = 0; shapeIdx < layers[layerIdx].Count; ++shapeIdx) {
					Shape shape = layers[layerIdx][shapeIdx];
					if (shape == null) continue;
					int rowIdx = (GetFlowX(shape) - firstRowPos + rowDistance / 2) / rowDistance;
					// If the natural row index is alreay occupied we assign the next free row index.
					if (rowIdx < nextFreeRowIdx) {
						rowIdx = nextFreeRowIdx;
						++nextFreeRowIdx;
					} else nextFreeRowIdx = rowIdx + 1;
					MoveShapeFlowX(shape, firstRowPos + rowIdx * rowDistance);
				}
			}
			// First positioning is in layer 1 with respect to layer 0.
			phase = Phase.Positioning;
			positioningRun = 0;
			currentLayer = 1;
		}


		/// <summary>
		/// Moves the shapes in the layers such the distances to their connected shapes in neighbored layers 
		/// are small.
		/// </summary>
		/// <returns></returns>
		private bool PositionShapesWithinLayers() {
			// If there is only one layer, nothing to do.
			if (currentLayer >= layers.Count) return false;
			//
			int layerDelta = positioningRun % 2 == 0 ? -1 : +1;
			List<Shape> shapeList = layers[currentLayer];
			// Loop from left to right and move shapes to the correct place if possible.
			for (int si = 0; si < shapeList.Count; ++si) {
				if (shapeList[si] == null) continue;
				int m = 0;
				int n = 0;
				foreach (Shape s in GetConnectedShapes(shapeList[si], Orientation.None)) {
					if (((LayerInfo)s.InternalTag).layer != currentLayer + layerDelta) continue;
					// Make sure to perform integer division in the positive numbers, otherwise it is rounded towards 0.
					m += (GetFlowX(s) - firstRowPos)/rowDistance - (GetFlowX(shapeList[si]) - firstRowPos)/rowDistance;
					++n;
				}
				// Move the shape if there is a free row.
				if (n > 0 && m / n != 0) TryMoveShapeRow(shapeList, si, m / n, false);
			}
			// Determine next layer to process
			if (layerDelta < 0) {
				++currentLayer;
				if (currentLayer >= layers.Count) {
					++positioningRun;
					currentLayer = layers.Count - 2;
				}
			} else {
				--currentLayer;
				if (currentLayer < 0) {
					++positioningRun;
					currentLayer = 1;
				}
			}
			// We do down - up - down 
			return positioningRun < positioningRunCount;
		}


		// Returns the shape's coordinate orthogonal to the flow (called to the right)
		private int GetFlowX(Shape s) {
			switch (flowDirection) {
				case FlowDirection.TopDown: return s.X;
				case FlowDirection.LeftToRight: return -s.Y;
				case FlowDirection.BottomUp: return -s.X;
				case FlowDirection.RightToLeft: return s.Y;
				default: Debug.Fail("Unexpected flow direction"); return 0;
			}
		}


		// Returns the coordinate in the direction of the flow (called downwards)
		private int GetFlowY(Shape s) {
			switch (flowDirection) {
				case FlowDirection.TopDown: return s.Y;
				case FlowDirection.LeftToRight: return s.X;
				case FlowDirection.BottomUp: return -s.Y;
				case FlowDirection.RightToLeft: return -s.X;
				default: Debug.Fail("Unexpected flow direction"); return 0;
			}
		}


		// Moves the shape in the direction orthogonal to the flow (called to the right)
		private void MoveShapeFlowX(Shape shape, int x) {
			switch (flowDirection) {
				case FlowDirection.TopDown: shape.X = x; break;
				case FlowDirection.LeftToRight: shape.Y = -x; break;
				case FlowDirection.BottomUp: shape.X = -x; break;
				case FlowDirection.RightToLeft: shape.Y = x; break;
				default: Debug.Fail("Unexpected flow direction"); break;
			}
		}


		private void MoveShapeFlowY(Shape shape, int y) {
			switch (flowDirection) {
				case FlowDirection.TopDown: shape.Y = y; break;
				case FlowDirection.LeftToRight: shape.X = y; break;
				case FlowDirection.BottomUp: shape.Y = -y; break;
				case FlowDirection.RightToLeft: shape.X = -y; break;
				default: Debug.Fail("Unexpected flow direction"); break;
			}
		}


		/// <summary>
		/// Moves a shape by a given number of layers
		/// </summary>
		/// <param name="shape">shape to move</param>
		/// <param name="layerDelta">number of layers to move in flow direction, negative to move "upwards"</param>
		private void MoveShapeLayerFlow(Shape shape, int layerDelta) {
			// Do we need a new layer
			if (((LayerInfo)shape.InternalTag).layer + layerDelta < 0)
				InsertLayer(0);
			else if (((LayerInfo)shape.InternalTag).layer + layerDelta >= layerCount)
				InsertLayer(layerCount);
			// Move shape to its new layer
			((LayerInfo)shape.InternalTag).layer += layerDelta;
			MoveShapeFlowY(shape, firstLayerPos + ((LayerInfo)shape.InternalTag).layer * layerDistance);
		}


		/// <summary>
		/// Used to attach layer information to a shape (via InternalTag)
		/// </summary>
		private class LayerInfo {
			// Index of the layer (starting at 0)
			public int layer;
			// Current value. When sorting Bary center.
			public float value;
		}


		/// <summary>
		/// Returns all shapes that have an arrow from or to this shape.
		/// </summary>
		private IEnumerable<Shape> GetConnectedShapes(Shape shape, Orientation orientation) {
			foreach (ShapeConnectionInfo sci in shape.GetConnectionInfos(ControlPointId.Reference, null)) {
				if (!(sci.OtherShape is LineShapeBase)) continue;
				Orientation arrowOrientation;
				ControlPointId arrowEndId;
				if (sci.OtherPointId == ControlPointId.FirstVertex) {
					arrowOrientation = Orientation.Outgoing;
					arrowEndId = ControlPointId.LastVertex;
				} else if (sci.OtherPointId == ControlPointId.LastVertex) {
					arrowOrientation = Orientation.Incoming;
					arrowEndId = ControlPointId.FirstVertex;
				} else continue;
				if (orientation == Orientation.None || arrowOrientation == orientation) {
					foreach (ShapeConnectionInfo aci in sci.OtherShape.GetConnectionInfos(arrowEndId, null)) {
						if (aci.OtherShape != null)
							yield return aci.OtherShape;
					}
				}
			}
		}


		// Tries to move the shape from index by delta rows. In this task existing shapes may be pushed 
		// together. The function does that as far as possible and returns the number of performed moves.
		private int TryMoveShapeRow(List<Shape> shapeList, int index, int delta, bool allowPushing) {
			int newIndex = index;
			while (shapeList.Count <= index + delta) shapeList.Add(null);
			// 
			int inc = delta > 0 ? +1 : -1;
			while (delta != 0 && shapeList[newIndex + inc] == null) {
				newIndex += inc;
				delta -= inc;
			}
			// If there is already a shape, push it (if we may).
			if (delta != 0 && allowPushing)
				newIndex += TryMoveShapeRow(shapeList, newIndex + inc, delta, true);
			// Now do the move
			if (newIndex != index) {
				shapeList[newIndex] = shapeList[index];
				shapeList[index] = null;
				MoveShapeFlowX(shapeList[newIndex], GetFlowX(shapeList[newIndex]) + (newIndex - index) * rowDistance);
			}
			return newIndex - index;
		}


		/// <summary>
		/// Inserts a layer before the indicated one.
		/// </summary>
		private void InsertLayer(int layerIndex) {
			foreach (Shape s in selectedShapes) {
				if (ExcludeFromFitting(s)) continue;
				if (((LayerInfo)s.InternalTag).layer >= layerIndex) {
					++((LayerInfo)s.InternalTag).layer;
				}
			}
			// Currently we only insert layers at the top and at the bottom.
			if (layerIndex == 0) firstLayerPos -= layerDistance;
			++layerCount;
		}


		private class ShapeByXComparer : IComparer<Shape> {

			public ShapeByXComparer(FlowLayouter flowLayouter) {
				this.flowLayouter = flowLayouter;
			}


			public int Compare(Shape s1, Shape s2) {
				int x1 = flowLayouter.GetFlowX(s1);
				int x2 = flowLayouter.GetFlowX(s2);
				return x1 > x2 ? +1 : x2 > x1 ? -1 : 0;
			}


			private FlowLayouter flowLayouter;
		}


		private class ShapeByValueComparer : IComparer<Shape> {

			public int Compare(Shape s1, Shape s2) {
				float v1 = ((LayerInfo)s1.InternalTag).value;
				float v2 = ((LayerInfo)s2.InternalTag).value;
				return v1 > v2 ? +1 : v2 > v1 ? -1 : 0;
			}

		}


		private enum Phase { OptimizeLevels, Ordering, Positioning };


		/// <summary>
		/// Calculates the optimizing value for the given shape assumed to be moved by layerDelta layers downwards.
		/// </summary>
		/// <remarks>
		/// The optimization value is calculated as follows:
		/// - Directed arrow one level down: 0 points
		/// - Directed arrow more than one level down: 1 point per level
		/// - Directed arrow on same level: 1 point
		/// - Directed arrow one level up: 3 points
		/// - Directed arrow more than one level up: 1 additional point per level
		/// - Bidirectional arrow on same level: 0 points
		/// - Bidirectional arrow up or down multiple levels: 1 point for each level
		/// </remarks>
		private float CalcOptimizationValue(Shape shape, int layerDelta) {
			float result = 0;
			int shapeLayer = ((LayerInfo)shape.InternalTag).layer + layerDelta;
			//
			// Loop through outgoing arrows and count points
			foreach (Shape targetShape in GetConnectedShapes(shape, Orientation.Outgoing)) {
				result += CalcDirectedArrowPenalty(shapeLayer, ((LayerInfo)targetShape.InternalTag).layer);
			}
			// Loop through bidirectional arrows and count points
			foreach (Shape otherShape in GetConnectedShapes(shape, Orientation.Both)) {
				int sourceLayer = ((LayerInfo)shape.InternalTag).layer;
				int targetLayer = ((LayerInfo)otherShape.InternalTag).layer;
				result += Math.Abs(targetLayer - shapeLayer);
			}
			// Loop through incoming arrows and count points
			foreach (Shape sourceShape in GetConnectedShapes(shape, Orientation.Incoming)) {
				result += CalcDirectedArrowPenalty(((LayerInfo)sourceShape.InternalTag).layer, shapeLayer);
			}
			return result;
		}


		private float CalcDirectedArrowPenalty(int sourceLayer, int targetLayer) {
			const float penaltyOneLevelUp = 3.0f;
			const float penaltyMoreLevelUp = 1.0f;
			const float penaltyMoreLevelDown = 1.0f;
			const float penaltySameLevel = 1.0f;
			//
			float result = 0;
			if (targetLayer < sourceLayer)
				result += penaltyOneLevelUp + penaltyMoreLevelUp * (sourceLayer - targetLayer - 1);
			else if (targetLayer == sourceLayer)
				result += penaltySameLevel;
			else
				result += penaltyMoreLevelDown * (targetLayer - sourceLayer - 1);
			return result;
		}


		private enum Orientation { None, Incoming, Outgoing, Both };

		private ShapeByXComparer shapeByXComparer;
		private ShapeByValueComparer shapeByValueComparer;

		// --- Configuration data ---

		// Desired direction of the flow
		private FlowDirection flowDirection = FlowDirection.TopDown;

		// Distance of rows
		private int rowDistance = 50;
		//
		// Distance between layers in pixels
		private int layerDistance = 100;
		//
		private Random random = new Random();

		// --- Dynamic Processing Data ---
		//
		// Depending on the flow direction, we use a flow coordinate system, where the direction of the
		// flow is always the positive x-axis
		// Position of first layer from top in flow coordinates
		private int firstLayerPos = 100;
		// Position of first row from left in flow coordinates
		private int firstRowPos = -10000; // So we can always calculated with positive row indices.
		// Number of runs for ordering and positioning
		private int positioningRunCount = 3;
		//
		// Currently executed phase
		private Phase phase;
		// Index of current run through the positioning or ordering algorithm
		private int positioningRun;
		// 
		// Current number of layers
		private int layerCount = 10;
		//
		// Currently processed layer
		private int currentLayer = 0;
		//
		// Maximum number of layers to jump for a disturbance
		private int disturbanceLevel = 10;
		//
		// Number of disturbances already performed on the current level.
		private int disturbanceCount = 0;

		List<List<Shape>> layers = new List<List<Shape>>();
		List<Shape> layerShapes = new List<Shape>();
	}

}