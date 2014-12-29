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
using System.Collections;
using System.Collections.Generic;
using Dataweb.NShape.Advanced;
using System.Drawing;
using Dataweb.Utilities;
using System.Threading;
using System.Diagnostics;


namespace Dataweb.NShape
{
	/// <summary>
	/// Defines the directions in which a shape is lifted.
	/// </summary>
	/// <status>Reviewed</status>
	public enum ZOrderDestination
	{
		/// <summary>Send to the back.</summary>
		ToBottom,

		/// <summary>Send one backwards.</summary>
		Downwards,

		/// <summary>Bring upwards by one.</summary>
		Upwards,

		/// <summary>Bring to the top.</summary>
		ToTop
	}


	/// <summary>
	/// A read-only collection of shapes sorted by z-order.
	/// </summary>
	/// <remarks>Also providing methods to find shapes and process the collection 
	/// items in every direction.</remarks>
	/// <status>Reviewed</status>
	public interface IReadOnlyShapeCollection : Dataweb.NShape.Advanced.IReadOnlyCollection<Shape>
	{
		/// <summary>
		/// Retrieves the greatest z-order within the collection.
		/// </summary>
		/// <returns></returns>
		int MaxZOrder { get; }

		/// <summary>
		/// Retrieves the least z-order within the collection.
		/// </summary>
		/// <returns></returns>
		int MinZOrder { get; }

		/// <summary>
		/// Retrieves the top most shape.
		/// </summary>
		Shape TopMost { get; }

		/// <summary>
		/// Retrieves the bottom shape.
		/// </summary>
		Shape Bottom { get; }

		/// <summary>
		/// Enumerates shapes from the highest z-orders to the lowest.
		/// </summary>
		IEnumerable<Shape> TopDown { get; }

		/// <summary>
		/// Enumerates shapes from the lowest z-orders to the highest.
		/// </summary>
		IEnumerable<Shape> BottomUp { get; }

		/// <summary>
		/// Finds a shape within the given rectangle.
		/// </summary>
		Shape FindShape(int x, int y, int width, int height, bool completelyInside, Shape startShape);

		/// <summary>
		/// Finds all shapes within the given rectangle. Note: Some shapes may be returned multiple times.
		/// </summary>
		IEnumerable<Shape> FindShapes(int x, int y, int width, int height, bool completelyInside);

		/// <summary>
		/// Finds a shape, which has control points near a given position.
		/// </summary>
		Shape FindShape(int x, int y, ControlPointCapabilities controlPointCapabilities, int distance, Shape startShape);

		/// <summary>
		/// Finds all shapes, which have control points near the given position. Note: Some shapes may be returned multiple times.
		/// </summary>
		IEnumerable<Shape> FindShapes(int x, int y, ControlPointCapabilities controlPointCapabilities, int distance);
	}


	/// <summary>
	/// A modifyable collection of shapes.
	/// </summary>
	/// <status>Reviewed</status>
	public interface IShapeCollection : IReadOnlyShapeCollection
	{
		/// <ToBeCompleted></ToBeCompleted>
		void Add(Shape shape);

		/// <ToBeCompleted></ToBeCompleted>
		void Add(Shape shape, int zOrder);

		/// <ToBeCompleted></ToBeCompleted>
		bool Contains(Shape shape);

		/// <ToBeCompleted></ToBeCompleted>
		bool Remove(Shape shape);

		/// <ToBeCompleted></ToBeCompleted>
		void Clear();

		/// <ToBeCompleted></ToBeCompleted>
		void CopyTo(Shape[] array, int arrayIndex);

		/// <ToBeCompleted></ToBeCompleted>
		void AddRange(IEnumerable<Shape> shapes);

		/// <ToBeCompleted></ToBeCompleted>
		void Replace(Shape oldShape, Shape newShape);

		/// <ToBeCompleted></ToBeCompleted>
		void ReplaceRange(IEnumerable<Shape> oldShapes, IEnumerable<Shape> newShapes);

		/// <ToBeCompleted></ToBeCompleted>
		bool RemoveRange(IEnumerable<Shape> shapes);

		/// <ToBeCompleted></ToBeCompleted>
		Rectangle GetBoundingRectangle(bool tight);

		/// <ToBeCompleted></ToBeCompleted>
		bool ContainsAll(IEnumerable<Shape> shapes);

		/// <ToBeCompleted></ToBeCompleted>
		bool ContainsAny(IEnumerable<Shape> shapes);

		/// <summary>
		/// Assigns a z-order to a shape.
		/// </summary>
		/// <param name="shape"></param>
		/// <param name="zOrder"></param>
		/// <remarks>This method modifies the shape. The caller must make sure that the
		/// repository is informed about the modification.</remarks>
		void SetZOrder(Shape shape, int zOrder);
	}


	/// <summary>
	/// Manages a list of shapes.
	/// </summary>
	/// <status>Interface reviewed</status>
	public class ShapeCollection : IShapeCollection, IReadOnlyShapeCollection, ICollection
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.ShapeCollection" />.
		/// </summary>
		public ShapeCollection()
			: base()
		{
			Construct(10);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.ShapeCollection" />.
		/// </summary>
		public ShapeCollection(int capacity)
		{
			Construct(capacity);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.ShapeCollection" />.
		/// </summary>
		public ShapeCollection(IEnumerable<Shape> collection)
		{
			if (collection == null) throw new ArgumentNullException("collection");
			if (collection is ICollection)
				Construct(((ICollection) collection).Count);
			else Construct(0);
			AddRangeCore(collection);
		}


#if DEBUG_UI
		/// <ToBeCompleted></ToBeCompleted>
		~ShapeCollection()
		{
			if (occupiedBrush != null) occupiedBrush.Dispose();
			if (emptyBrush != null) emptyBrush.Dispose();
		}
#endif

		#region IShapeCollection Members

		/// <summary>
		/// Adds multiple shapes to the collection.
		/// </summary>
		/// <param name="shapes"></param>
		public void AddRange(IEnumerable<Shape> shapes)
		{
			if (shapes == null) throw new ArgumentNullException("shapes");
			AddRangeCore(shapes);
		}


		/// <summary>
		/// Replaces a shape by another.
		/// </summary>
		public void Replace(Shape oldShape, Shape newShape)
		{
			if (oldShape == null) throw new ArgumentNullException("oldShape");
			if (newShape == null) throw new ArgumentNullException("newShape");
			ReplaceCore(oldShape, newShape);
		}


		/// <summary>
		/// Replaces multiple shapes by others.
		/// </summary>
		public void ReplaceRange(IEnumerable<Shape> oldShapes, IEnumerable<Shape> newShapes)
		{
			if (oldShapes == null) throw new ArgumentNullException("oldShapes");
			if (newShapes == null) throw new ArgumentNullException("newShapes");
			ReplaceRangeCore(oldShapes, newShapes);
		}


		/// <summary>
		/// Removes multiple shapes.
		/// </summary>
		public bool RemoveRange(IEnumerable<Shape> shapes)
		{
			if (shapes == null) throw new ArgumentNullException("shapes");
			return RemoveRangeCore(shapes);
		}


		/// <summary>
		/// Gets the shape with the highest z-order value.
		/// </summary>
		public Shape TopMost
		{
			get
			{
				if (shapes.Count > 0) return shapes[shapes.Count - 1];
				else return null;
			}
		}


		/// <summary>
		/// Gets the shape with the lowest z-order value.
		/// </summary>
		public Shape Bottom
		{
			get
			{
				if (shapes.Count > 0) return shapes[0];
				else return null;
			}
		}


		/// <summary>
		/// Gets all shapes from the topmost to the bottom shape sorted by z-order values.
		/// </summary>
		public IEnumerable<Shape> TopDown
		{
			get { return Enumerator.CreateTopDown(shapes); }
		}


		/// <summary>
		/// Gets all shapes from the bottom to the topmost shape sorted by z-order values.
		/// </summary>
		public IEnumerable<Shape> BottomUp
		{
			get { return Enumerator.CreateBottomUp(shapes); }
		}

		#region ShapeCollection Methods called by the child shapes

		/// <summary>
		/// Notifies the owner that the child shape is going to move.
		/// </summary>
		/// <param name="shape">The shape trying to move</param>
		public virtual void NotifyChildMoving(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			MapRemove(shape);
			ResetBoundingRectangles();
		}


		/// <summary>
		/// Notifies the owner that the child shape is going to resize.
		/// </summary>
		/// <param name="shape">The shape trying to move</param>
		public virtual void NotifyChildResizing(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			MapRemove(shape);
			ResetBoundingRectangles();
		}


		/// <summary>
		/// Notifies the owner that the child shape is going to rotate.
		/// </summary>
		/// <param name="shape">The shape trying to move</param>
		public virtual void NotifyChildRotating(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			MapRemove(shape);
			ResetBoundingRectangles();
		}


		/// <summary>
		/// Notifies the parent that its child shape has moved.
		/// </summary>
		/// <param name="shape"></param>
		public virtual void NotifyChildMoved(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			MapInsert(shape);
		}


		/// <summary>
		/// Notifies the parent that its child shape has resized.
		/// </summary>
		public virtual void NotifyChildResized(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			MapInsert(shape);
		}


		/// <summary>
		/// Notifies the parent that its child shape has rotated.
		/// </summary>
		public virtual void NotifyChildRotated(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			MapInsert(shape);
		}

		#endregion

		/// <override></override>
		public Rectangle GetBoundingRectangle(bool tight)
		{
			if (tight) {
				if (!Geometry.IsValid(boundingRectangleTight))
					GetBoundingRectangleCore(tight, out boundingRectangleTight);
				return boundingRectangleTight;
			}
			else {
				if (!Geometry.IsValid(boundingRectangleLoose))
					GetBoundingRectangleCore(tight, out boundingRectangleLoose);
				return boundingRectangleLoose;
			}
		}


		/// <override></override>
		public bool ContainsAll(IEnumerable<Shape> shapes)
		{
			if (shapes == null) throw new ArgumentNullException("shape");
			bool isEmpty = true;
			foreach (Shape shape in shapes) {
				if (isEmpty) isEmpty = false;
				if (!Contains(shape)) return false;
			}
			return !isEmpty;
		}


		/// <override></override>
		public bool ContainsAny(IEnumerable<Shape> shapes)
		{
			if (shapes == null) throw new ArgumentNullException("shape");
			bool result = false;
			foreach (Shape shape in shapes) {
				if (Contains(shape)) {
					result = true;
					break;
				}
			}
			return result;
		}


		/// <override></override>
		public Shape FindShape(int x, int y, ControlPointCapabilities controlPointCapabilities, int range, Shape startShape)
		{
			return GetFirstShapeInArea(x - range, y - range, 2*range, 2*range, controlPointCapabilities, startShape,
			                           SearchMode.Near);
		}


		/// <override></override>
		public Shape FindShape(int x, int y, int width, int height, bool completelyInside, Shape startShape)
		{
			return GetFirstShapeInArea(x, y, width, height, ControlPointCapabilities.None,
			                           startShape, completelyInside ? SearchMode.Contained : SearchMode.Near);
		}


		/// <summary>
		/// Finds shapes that contain (x, y) or own a control point within the range from (x, y).
		/// </summary>
		/// <returns>Shapes founds. Shapes may occur more than once.</returns>
		public IEnumerable<Shape> FindShapes(int x, int y, ControlPointCapabilities capabilities, int range)
		{
			foreach (Shape s in GetShapesInArea(x - range, y - range, 2*range,
			                                    2*range, capabilities, SearchMode.Near))
				yield return s;
		}


		/// <summary>
		/// Finds shapes that intersect with or are contained within the given rectangle.
		/// </summary>
		/// <returns>Shapes found. Shapes may occur more than once.</returns>
		public IEnumerable<Shape> FindShapes(int x, int y, int width, int height, bool completelyInside)
		{
			foreach (Shape s in GetShapesInArea(x, y, width, height, ControlPointCapabilities.None,
			                                    completelyInside ? SearchMode.Contained : SearchMode.Intersects))
				yield return s;
		}


		/// <override></override>
		public int MinZOrder
		{
			get
			{
				if (shapes.Count <= 0) return 0;
				else return Bottom.ZOrder;
			}
		}


		/// <override></override>
		public int MaxZOrder
		{
			get
			{
				if (shapes.Count <= 0) return 0;
				else return TopMost.ZOrder;
			}
		}


		/// <override></override>
		public void SetZOrder(Shape shape, int zOrder)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			Remove(shape);
			shape.ZOrder = zOrder;
			Add(shape);
		}

		#endregion

		#region ICollection<Shape> Members

		/// <override></override>
		public void Add(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			AddCore(shape);
			// Reset bounding rectangles
			boundingRectangleTight = boundingRectangleLoose = Geometry.InvalidRectangle;
		}


		/// <override></override>
		public void Add(Shape shape, int zOrder)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			shape.ZOrder = zOrder;
			Add(shape);
		}


		/// <override></override>
		public void Clear()
		{
			ClearCore();
		}


		/// <override></override>
		public bool Contains(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			return shapeDictionary.ContainsKey(shape);
		}


		/// <override></override>
		public void CopyTo(Shape[] array, int arrayIndex)
		{
			if (array == null) throw new ArgumentNullException("array");
			shapes.CopyTo(array, arrayIndex);
		}


		/// <override></override>
		public int Count
		{
			get { return shapes.Count; }
		}


		/// <override></override>
		public bool IsReadOnly
		{
			get { return false; }
		}


		/// <override></override>
		public bool Remove(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			return RemoveCore(shape);
		}

		#endregion

		#region IEnumerable<Shape> Members

		/// <override></override>
		public IEnumerator<Shape> GetEnumerator()
		{
			return Enumerator.CreateTopDown(shapes);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerator.CreateTopDown(shapes);
		}

		#endregion

		#region ICollection Members

		/// <override></override>
		public void CopyTo(Array array, int index)
		{
			if (array == null) throw new ArgumentNullException("array");
			Array.Copy(shapes.ToArray(), index, array, 0, array.Length);
		}


		int ICollection.Count
		{
			get { return Count; }
		}


		/// <override></override>
		public bool IsSynchronized
		{
			get { return false; }
		}


		/// <override></override>
		public object SyncRoot
		{
			get
			{
				if (syncRoot == null)
					Interlocked.CompareExchange(ref syncRoot, new object(), null);
				return syncRoot;
			}
		}

		#endregion

		/// <summary>
		/// Clones all shapes (including model objects) in the shapeCollection. Connections between shapes in the shape collection will be maintained.
		/// </summary>
		public ShapeCollection Clone()
		{
			return Clone(true);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ShapeCollection Clone(bool withModelObjects)
		{
#if DEBUG_DIAGNOSTICS
			Stopwatch w = new Stopwatch();
			w.Start();
#endif
			ShapeCollection result = new ShapeCollection(Count);

			Dictionary<Shape, Shape> shapeDict = new Dictionary<Shape, Shape>(shapes.Count);
			Dictionary<Shape, List<ShapeConnectionInfo>> connections = new Dictionary<Shape, List<ShapeConnectionInfo>>();
			// clone from last to first shape in order to maintain the ZOrder
			foreach (Shape shape in BottomUp) {
				// Clone shape (and model)
				Shape clone = null;
				if (withModelObjects)
					clone = ShapeDuplicator.CloneShapeAndModelObject(shape);
				else clone = ShapeDuplicator.CloneShapeOnly(shape);
				//Shape clone = shape.Clone();
				//if (withModelObjects && clone.ModelObject != null)
				//   clone.ModelObject = shape.ModelObject.Clone();

				// Register original shape and clone in the dictionary for fast searching 
				// when restoring connections later
				shapeDict.Add(shape, clone);
				foreach (ShapeConnectionInfo ci in shape.GetConnectionInfos(ControlPointId.Any, null)) {
					if (shape.HasControlPointCapability(ci.OwnPointId, ControlPointCapabilities.Glue)) {
						if (!connections.ContainsKey(shape))
							connections.Add(shape, new List<ShapeConnectionInfo>(2)); // typically, a line has 2 connections
						connections[shape].Add(ci);
					}
				}
				result.Add(clone);
			}

			// Restore connections between copied shapes
			foreach (KeyValuePair<Shape, List<ShapeConnectionInfo>> item in connections) {
				int cnt = item.Value.Count;
				for (int i = 0; i < cnt; ++i) {
					// If the passive shape was among the copied shapes, restore the connection
					if (!shapeDict.ContainsKey(item.Value[i].OtherShape)) continue;
					Shape activeShape = shapeDict[item.Key];
					Shape passiveShape = shapeDict[item.Value[i].OtherShape];
					Debug.Assert(result.Contains(passiveShape));
					activeShape.Connect(item.Value[i].OwnPointId, passiveShape, item.Value[i].OtherPointId);
				}
			}
			shapeDict.Clear();
			connections.Clear();
#if DEBUG_DIAGNOSTICS
			w.Stop();
			Console.WriteLine("Cloning ShapeCollection with {0} elements: {1}", result.Count, w.Elapsed);
#endif
			return result;
		}

		#region [Protected] Methods

		/// <summary>
		/// Adds the given shape to the collection.
		/// </summary>
		/// <param name="shape"></param>
		protected virtual int AddCore(Shape shape)
		{
			if (shapeDictionary.ContainsKey(shape))
				throw new ArgumentException("The shape item already exists in the collection.");
			int idx = FindInsertPosition(shape.ZOrder);
			return InsertCore(idx, shape);
		}


		/// <summary>
		/// Adds the given shape to the collection. 
		/// The shape is inserted into the collection at the correct position, depending on the z-order.
		/// </summary>
		/// <returns>Index where the shape has been inserted. The shape indexes can vary.</returns>
		protected virtual int InsertCore(int index, Shape shape)
		{
			if (shapeDictionary.ContainsKey(shape))
				throw new ArgumentException("The given shape is already part of the collection.");
			int result = -1;
			if (shapes.Count == 0 || index == shapes.Count || shape.ZOrder >= this.TopMost.ZOrder) {
				if (shapes.Count > 0) Debug.Assert(shape.ZOrder >= shapes[shapes.Count - 1].ZOrder);

				// append shape
				shapes.Add(shape);
				result = shapes.Count - 1;
			}
			else if (shape.ZOrder < shapes[0].ZOrder) {
				Debug.Assert(shape.ZOrder <= shapes[0].ZOrder);

				shapes.Insert(0, shape);
				result = index;
			}
			else {
				// prepend shape
				if (index > 0) Debug.Assert(shapes[index - 1].ZOrder <= shape.ZOrder);
				Debug.Assert(shape.ZOrder <= shapes[index].ZOrder);

				shapes.Insert(index, shape);
				result = index;
			}
			AddShapeToIndex(shape);
			return result;
		}


		/// <summary>
		/// Inserts the elements of the collection into the ShapeCollection. 
		/// This method inserts each element into the correct position, sorted by z-order.
		/// </summary>
		protected virtual void AddRangeCore(IEnumerable<Shape> collection)
		{
			int lastInsertPos = -1;
			foreach (Shape shape in collection) {
				// Check if the next shape can be inserted at the last position
				// This is the case if the shapes are pre-sorted by ZOrder (upper shapes first)
				if (IndexIsValid(lastInsertPos, shape)) {
					if (shapes[shapes.Count - 1].ZOrder <= shape.ZOrder)
						lastInsertPos = InsertCore(shapes.Count, shape);
					else {
						AssertIndexIsValid(lastInsertPos, shape);
						InsertCore(lastInsertPos, shape);
					}
				}
				else {
					int index = FindInsertPosition(shape.ZOrder);
					lastInsertPos = InsertCore(index, shape);
				}
			}
			// Reset BoundingRectangle
			boundingRectangleTight = boundingRectangleLoose = Geometry.InvalidRectangle;
		}


		/// <summary>
		/// Replaces the old shape with the new shape.
		/// </summary>
		protected virtual void ReplaceCore(Shape oldShape, Shape newShape)
		{
			if (shapeDictionary.ContainsKey(newShape))
				throw new InvalidOperationException("The value to be inserted does already exist in the collection.");
			if (!shapeDictionary.ContainsKey(oldShape))
				throw new InvalidOperationException("The value to be replaced does not exist in the colection.");
			int idx = FindShapeIndex(oldShape);
			if (idx < 0) throw new InvalidOperationException("The given shape does not exist in the collection.");
			// Copy DiagramShape properties
			newShape.ZOrder = oldShape.ZOrder;
			newShape.Layers = oldShape.Layers;
			// Remove old shape from search dictionary and spacial index
			RemoveShapeFromIndex(oldShape);
			// Replace oldShape with newShape
			shapes[idx] = newShape;
			// Add newShape in search dictionary and spacial index
			AddShapeToIndex(newShape);
			// Reset BoundingRectangle
			boundingRectangleTight = boundingRectangleLoose = Geometry.InvalidRectangle;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void ReplaceRangeCore(IEnumerable<Shape> oldShapes, IEnumerable<Shape> newShapes)
		{
			IList<Shape> oldShapesList, newShapesList;
			if (oldShapes is IList<Shape>) oldShapesList = (IList<Shape>) oldShapes;
			else oldShapesList = new List<Shape>(oldShapes);
			if (newShapes is IList<Shape>) newShapesList = (IList<Shape>) newShapes;
			else newShapesList = new List<Shape>(newShapes);

			if (oldShapesList.Count != newShapesList.Count)
				throw new NShapeInternalException("Numer of elements in the given collections differ from each other.");
			for (int i = 0; i < oldShapesList.Count; ++i)
				ReplaceCore(oldShapesList[i], newShapesList[i]);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual bool RemoveCore(Shape shape)
		{
			if (!shapeDictionary.ContainsKey(shape)) return false;
			int idx = FindShapeIndex(shape);
			Debug.Assert(idx >= 0);
			if (idx >= 0) {
				shapes.RemoveAt(idx);
				RemoveShapeFromIndex(shape);
			}
			// Reset BoundingRectangle
			boundingRectangleTight = boundingRectangleLoose = Geometry.InvalidRectangle;
			return true;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual bool RemoveRangeCore(IEnumerable<Shape> shapes)
		{
			bool result = true;

			// Convert IEnumerable<Shapes> to a list type providing an indexer
			IList<Shape> shapeList;
			if (shapes is IList<Shape>)
				shapeList = (IList<Shape>) shapes;
			else shapeList = new List<Shape>(shapes);

			// Remove shapes
			for (int i = shapeList.Count - 1; i >= 0; --i) {
				if (!RemoveCore(shapeList[i]))
					result = false;
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void ClearCore()
		{
			shapes.Clear();
			shapeDictionary.Clear();
			if (shapeMap != null) shapeMap.Clear();
			// Reset BoundingRectangle
			boundingRectangleTight = boundingRectangleLoose = Geometry.InvalidRectangle;
		}


		/// <summary>
		/// Adds the given shape into the collection's indexes.
		/// </summary>
		protected void AddShapeToIndex(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			shapeDictionary.Add(shape, null);
			MapInsert(shape);
		}


		/// <summary>
		/// removes the given shape from the collection's indexes.
		/// </summary>
		protected void RemoveShapeFromIndex(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			MapRemove(shape);
			shapeDictionary.Remove(shape);
		}

		#endregion

		internal void SetDisplayService(IDisplayService displayService)
		{
			// set displayService for all shapes - the shapes set their children's DisplayService themselfs
			for (int i = shapes.Count - 1; i >= 0; --i)
				shapes[i].DisplayService = displayService;
		}

		#region [Private] Methods

		private void Construct(int capacity)
		{
			if (capacity > 0) {
				shapes = new ReadOnlyList<Shape>(capacity);
				shapeDictionary = new Dictionary<Shape, object>(capacity);
			}
			else {
				shapes = new ReadOnlyList<Shape>();
				shapeDictionary = new Dictionary<Shape, object>();
			}
			if (capacity >= 1000) shapeMap = new MultiHashList<Shape>(capacity);
		}


		private uint CalcMapHashCode(Point cellIndex)
		{
			unchecked {
				uint x = (uint) (cellIndex.X);
				uint y = (uint) (cellIndex.Y);
				y = (y & 0x000000ffu) << 24 | (y & 0x0000ff00u) << 8 | (y & 0x00ff0000u) >> 8 | (y & 0xff000000u) >> 24;
				return x ^ y;
			}
		}


		/// <summary>
		/// Inserts the given shape into the shape map (spatial index).
		/// </summary>
		private void MapInsert(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			if (shapeMap != null) {
				//Debug.Assert(CellsAreValid(shape));
				foreach (Point p in shape.CalculateCells(Diagram.CellSize)) {
					shapeMap.Add(CalcMapHashCode(p), shape);
				}
			}
		}


		/// <summary>
		/// Removes the given shape into the shape map (spatial index).
		/// </summary>
		private void MapRemove(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			if (shapeMap != null) {
				//Debug.Assert(CellsAreValid(shape));
				foreach (Point p in shape.CalculateCells(Diagram.CellSize))
					shapeMap.Remove(CalcMapHashCode(p), shape);
			}
		}


#if DEBUG_UI
		/// <ToBeCompleted></ToBeCompleted>
		public void DrawOccupiedCells(Graphics graphics, int x, int y, int width, int height)
		{
			Point p = Point.Empty;
			int minCellX = x/Diagram.CellSize;
			if (x < 0) --minCellX;
			int minCellY = y/Diagram.CellSize;
			if (y < 0) --minCellY;
			int maxCellX = width/Diagram.CellSize;
			int maxCellY = height/Diagram.CellSize;
			for (p.X = minCellX; p.X <= maxCellX; ++p.X) {
				for (p.Y = minCellY; p.Y <= maxCellY; ++p.Y) {
					foreach (Shape s in shapeMap[CalcMapHashCode(p)]) {
						int left = p.X*Diagram.CellSize;
						int top = p.Y*Diagram.CellSize;
						if (s.IntersectsWith(left, top, Diagram.CellSize, Diagram.CellSize)
						    ||
						    Geometry.RectangleContainsRectangle(left, top, Diagram.CellSize, Diagram.CellSize,
						                                        s.GetBoundingRectangle(false))) {
							graphics.FillRectangle(occupiedBrush, left, top, Diagram.CellSize, Diagram.CellSize);
						}
						else graphics.FillRectangle(emptyBrush, left, top, Diagram.CellSize, Diagram.CellSize);
					}
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public int GetShapeCount(int x, int y)
		{
			Point p = Point.Empty;
			p.Offset(x/Diagram.CellSize, y/Diagram.CellSize);
			return GetShapeCount(p);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public int GetShapeCount(Point cellIndex)
		{
			int result = 0;
			foreach (Shape s in shapeMap[CalcMapHashCode(cellIndex)])
				++result;
			return result;
		}
#endif


		private bool CellsAreValid(Shape shape)
		{
			int left = int.MaxValue, top = int.MaxValue;
			int right = int.MinValue, bottom = int.MinValue;
			foreach (Point p in shape.CalculateCells(Diagram.CellSize)) {
				if (p.X < left) left = p.X;
				if (p.X > right) right = p.X;
				if (p.Y < top) top = p.Y;
				if (p.Y > bottom) bottom = p.Y;
			}
			// If there are no cells at all, this is considered as "valid".
			if (left == int.MaxValue && top == int.MaxValue && right == int.MinValue && bottom == int.MinValue)
				return true;

			Rectangle cellBounds = Rectangle.FromLTRB(left*Diagram.CellSize, top*Diagram.CellSize,
			                                          (right*Diagram.CellSize) + Diagram.CellSize,
			                                          (bottom*Diagram.CellSize) + Diagram.CellSize);
			Rectangle shapeBounds = shape.GetBoundingRectangle(true);

			if (!Geometry.RectangleContainsRectangle(cellBounds, shapeBounds))
				return false;
			int doubleCellSize = Diagram.CellSize*2;
			if (Math.Abs(cellBounds.Left - shapeBounds.Left) >= doubleCellSize
			    || Math.Abs(cellBounds.Top - shapeBounds.Top) >= doubleCellSize
			    || Math.Abs(shapeBounds.Right - cellBounds.Right) >= doubleCellSize
			    || Math.Abs(shapeBounds.Bottom - cellBounds.Bottom) >= doubleCellSize)
				return false;
			return true;
		}


		private void ReindexShape(Shape shape)
		{
			MapRemove(shape);
			MapInsert(shape);
		}


		private void AssertIndexIsValid(int index, Shape shape)
		{
			if (Count > 0) {
				if (index > Count)
					throw new NShapeInternalException("Index {0} is out of range: The collection contains only {1} element.", index,
					                                  Count);
				if (index > 0 && index <= Count && shapes[index - 1].ZOrder > shape.ZOrder)
					throw new NShapeInternalException("ZOrder value {0} cannot be inserted after ZOrder value {1}.", shape.ZOrder,
					                                  shapes[index - 1].ZOrder);
				if (index < Count && shapes[index].ZOrder < shape.ZOrder)
					throw new NShapeInternalException("ZOrder value {0} cannot be inserted before ZOrder value {1}.", shape.ZOrder,
					                                  shapes[index].ZOrder);
			}
		}


		// Tests whether the shape can be inserted at the given index without violating
		// the z-order sorting.
		private bool IndexIsValid(int index, Shape shape)
		{
			if (index >= 0 && index <= shapes.Count) {
				// Insert into an empty list
				if (shapes.Count == 0)
					return (index == 0);
					// Insert elsewhere (placed here because it is the most likely case)
				else if (index > 0 && index < shapes.Count)
					return (shapes[index].ZOrder >= shape.ZOrder && shapes[index - 1].ZOrder <= shape.ZOrder);
					// Prepend
				else if (index == 0)
					return (shapes[0].ZOrder >= shape.ZOrder);
					// Append
				else if (index == shapes.Count)
					return (shapes[index - 1].ZOrder <= shape.ZOrder);
			}
			return false;
		}


		private bool IsShapeInRange(Shape shape, int x, int y, int distance, ControlPointCapabilities controlPointCapabilities)
		{
			// if no ControlPoints are needed, we use functions that ignore ControlPoint positions...
			if (controlPointCapabilities == ControlPointCapabilities.None) {
				if (distance == 0) return shape.ContainsPoint(x, y);
				else return shape.IntersectsWith(x - distance, y - distance, distance + distance, distance + distance);
			}
			else {
				// ...otherwise we use ContainsPoint which also checks the ControlPoints
				return shape.HitTest(x, y, controlPointCapabilities, distance) != ControlPointId.None;
			}
		}


		private IEnumerable<Shape> GetShapesInArea(int x, int y, int w, int h, ControlPointCapabilities capabilities,
		                                           SearchMode mode)
		{
			bool tightBounds = (capabilities == ControlPointCapabilities.None);
			int range = w/2;
			if (shapeMap != null) {
				int fromX, fromY, toX, toY;
				ShapeUtils.CalcCell(x, y, Diagram.CellSize, out fromX, out fromY);
				ShapeUtils.CalcCell(x + w, y + h, Diagram.CellSize, out toX, out toY);
				Point p = Point.Empty;
				for (p.X = fromX; p.X <= toX; p.X += 1)
					for (p.Y = fromY; p.Y <= toY; p.Y += 1)
						foreach (Shape s in shapeMap[CalcMapHashCode(p)])
							if (mode == SearchMode.Contained &&
							    Geometry.RectangleContainsRectangle(x, y, w, h, s.GetBoundingRectangle(tightBounds))
							    || mode == SearchMode.Intersects && s.IntersectsWith(x, y, w, h)
							    || mode == SearchMode.Near && IsShapeInRange(s, x + range, y + range, range, capabilities))
								yield return s;
			}
			else {
				for (int i = shapes.Count - 1; i >= 0; --i) {
					if (mode == SearchMode.Contained &&
					    Geometry.RectangleContainsRectangle(x, y, w, h, shapes[i].GetBoundingRectangle(tightBounds))
					    || mode == SearchMode.Intersects && shapes[i].IntersectsWith(x, y, w, h)
					    || mode == SearchMode.Near && IsShapeInRange(shapes[i], x + range, y + range, range, capabilities))
						yield return shapes[i];
				}
			}
		}


		private Shape GetFirstShapeInArea(int x, int y, int w, int h, ControlPointCapabilities capabilities,
		                                  Shape startShape, SearchMode mode)
		{
			Shape result = null;
			if (shapeMap != null) {
				int startZOrder = startShape == null ? int.MaxValue : startShape.ZOrder;
				bool skipChildren = (startShape != null && startShape.Parent == null);
				int maxZOrder = int.MinValue;
				foreach (Shape s in GetShapesInArea(x, y, w, h, capabilities, mode)) {
					if (skipChildren && s.Parent != null) continue;
					if (s.ZOrder <= startZOrder && s.ZOrder > maxZOrder && s != startShape) {
						maxZOrder = s.ZOrder;
						result = s;
					}
				}
			}
			else {
				bool startShapeFound = (startShape == null);
				foreach (Shape s in GetShapesInArea(x, y, w, h, capabilities, mode)) {
					if (startShapeFound) {
						result = s;
						break;
					}
					else if (s == startShape) startShapeFound = true;
				}
			}
			if (result == null && startShape != null)
				result = GetFirstShapeInArea(x, y, w, h, capabilities, null, mode);
			return result;
		}


		private int FindShapeIndex(Shape shape)
		{
			if (shapeDictionary.ContainsKey(shape)) {
				int shapeCnt = shapes.Count;
				int zOrder = shape.ZOrder;
				// Search with binary search style...
				int sPos = 0;
				int ePos = shapeCnt;
				while (ePos > sPos + 1) {
					int searchIdx = (ePos + sPos)/2;
					// Find position where prevZOrder < zOrder < nextZOrder
					// If the zorder is equal, we approach the last shape with an equal zOrder.
					// If the correct shape is found while searching, return its position
					if (shapes[searchIdx] == shape)
						return searchIdx;
					if (shapes[searchIdx].ZOrder == zOrder) {
						// If there are shapes with identical zOrders, search them all
						int i;
						i = searchIdx;
						while (i < ePos && shapes[i].ZOrder == zOrder)
							if (shapes[i] == shape) return i;
							else ++i;
						i = searchIdx;
						while (i >= sPos && shapes[i].ZOrder == zOrder)
							if (shapes[i] == shape) return i;
							else --i;
					}
					else if (shapes[searchIdx].ZOrder > zOrder)
						sPos = searchIdx;
					else ePos = searchIdx;
				}
				// If the shape was not found, there are duplicate ZOrders 
				// which have to be searched sequently
				int foundIndex = sPos;
				if (shapes[foundIndex].ZOrder < zOrder) {
					for (int i = foundIndex; i < shapeCnt; ++i)
						if (shapes[i] == shape) return i;
				}
				else {
					for (int i = foundIndex; i >= 0; --i)
						if (shapes[i] == shape) return i;
				}
				Debug.Fail("The shape was not found although it exists in the list!");
			}
			return -1;
		}


		private int FindInsertPosition(int zOrder)
		{
			// zOrder is lower than the lowest zOrder in the list
			int shapeCnt = shapes.Count;
			if (shapeCnt == 0 || zOrder < shapes[0].ZOrder)
				return 0;

				// zOrder is higher than the highest ZOrder in the list
			else if (zOrder >= shapes[shapeCnt - 1].ZOrder)
				return shapeCnt;

				// search correct position 
			else {
				int sPos = 0;
				int ePos = shapeCnt;
				while (ePos > sPos + 1) {
					int searchIdx = (ePos + sPos)/2;
					// Find position where prevZOrder < zOrder < nextZOrder
					// If the zorder is equal, we approach the last shape with an equal zOrder.
					if (shapes[searchIdx].ZOrder <= zOrder)
						sPos = searchIdx;
					else ePos = searchIdx;
				}
				int foundIndex = sPos;
				if (foundIndex >= shapeCnt)
					return shapeCnt;

				if (shapes[foundIndex].ZOrder <= zOrder) {
					for (int i = foundIndex; i < shapeCnt; ++i) {
						if (shapes[i].ZOrder > zOrder)
							return i;
					}
					return shapeCnt;
				}
				else {
					for (int i = foundIndex; i >= 0; --i) {
						if (shapes[i].ZOrder < zOrder)
							return i + 1;
					}
					return 0;
				}
			}
		}


		private void ResetBoundingRectangles()
		{
			if (Geometry.IsValid(boundingRectangleTight))
				boundingRectangleTight = Geometry.InvalidRectangle;
			if (Geometry.IsValid(boundingRectangleLoose))
				boundingRectangleLoose = Geometry.InvalidRectangle;
		}


		private void GetBoundingRectangleCore(bool tight, out Rectangle boundingRectangle)
		{
			// return an empty rectangle if the collection has no children
			boundingRectangle = Geometry.InvalidRectangle;
			if (shapes.Count > 0) {
				int left, right, top, bottom;
				left = top = int.MaxValue;
				right = bottom = int.MinValue;
				foreach (Shape shape in shapes) {
					Rectangle r = shape.GetBoundingRectangle(tight);
					Debug.Assert(Geometry.IsValid(r));
					if (left > r.Left) left = r.Left;
					if (top > r.Top) top = r.Top;
					if (right < r.Right) right = r.Right;
					if (bottom < r.Bottom) bottom = r.Bottom;
				}
				if (left != int.MaxValue && top != int.MaxValue && right != int.MinValue && bottom != int.MinValue) {
					boundingRectangle.Offset(left, top);
					boundingRectangle.Width = right - left;
					boundingRectangle.Height = bottom - top;
				}
			}
		}

		#endregion

		#region [Private] Types

		/// <summary>
		/// Enumerates the elements of a shape collection.
		/// </summary>
		/// <status>reviewed</status>
		private struct Enumerator : IEnumerable<Shape>, IEnumerator<Shape>, IEnumerator
		{
			public static Enumerator CreateBottomUp(ReadOnlyList<Shape> shapeList)
			{
				Enumerator result;
				result.shapeList = shapeList;
				result.count = shapeList.Count;
				result.startIdx = result.currIdx = -1;
				result.step = 1;
				return result;
			}


			public static Enumerator CreateTopDown(ReadOnlyList<Shape> shapeList)
			{
				Enumerator result;
				result.shapeList = shapeList;
				result.count = shapeList.Count;
				result.startIdx = result.currIdx = shapeList.Count;
				result.step = -1;
				return result;
			}

			#region IEnumerable<Shape> Members

			public IEnumerator<Shape> GetEnumerator()
			{
				return this;
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}

			#endregion

			#region IEnumerator<Shape> Members

			public Shape Current
			{
				get
				{
					if (currIdx >= 0 && currIdx < count)
						return shapeList[currIdx];
					else return null;
				}
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get { return Current; }
			}


			public bool MoveNext()
			{
				currIdx += step;
				return (currIdx >= 0 && currIdx < count);
			}


			public void Reset()
			{
				currIdx = startIdx;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				shapeList = null;
			}

			#endregion

			static Enumerator()
			{
				Empty.shapeList = null;
				Empty.count = 0;
				Empty.startIdx = -1;
				Empty.step = 1;
			}

			#region Fields

			public static readonly Enumerator Empty;

			private sbyte step;
			private int currIdx;
			private int startIdx;
			private int count;
			private ReadOnlyList<Shape> shapeList;

			#endregion
		}


		private enum SearchMode
		{
			Contained,
			Intersects,
			Near
		};

		#endregion

		#region Fields

		// List of shapes in collection
		/// <ToBeCompleted></ToBeCompleted>
		protected ReadOnlyList<Shape> shapes;

		// Last calculated bounding rectangle
		/// <ToBeCompleted></ToBeCompleted>
		protected Rectangle boundingRectangleTight = Geometry.InvalidRectangle;

		/// <ToBeCompleted></ToBeCompleted>
		protected Rectangle boundingRectangleLoose = Geometry.InvalidRectangle;

		//// Dictionary of contained shapes used for fast searching
		//// Key / Value: Shape reference / Index of shape in the internal list of shapes
		private Dictionary<Shape, object> shapeDictionary;
		// Hashtable to quickly find shapes given a coordinate.
		private MultiHashList<Shape> shapeMap = null;

		private object syncRoot = null;

		#endregion

#if DEBUG_UI
		private SolidBrush occupiedBrush = new SolidBrush(Color.FromArgb(32, Color.Green));
		private SolidBrush emptyBrush = new SolidBrush(Color.FromArgb(32, Color.Red));
#endif
	}
}