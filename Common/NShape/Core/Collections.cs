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
using System.Diagnostics;


namespace Dataweb.NShape.Advanced {

	/// <summary>
	/// A generic readonly collection of items providing an enumerator and the total number of items in the collection
	/// </summary>
	public interface IReadOnlyCollection<T> : IEnumerable<T>, ICollection { }


	/// <summary>
	/// A list based class implementing the IReadOnlyCollection interface
	/// </summary>
	public class ReadOnlyList<T> : List<T>, IReadOnlyCollection<T> {
		
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.ReadOnlyList`1" />.
		/// </summary>
		public ReadOnlyList()
			: base() {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.ReadOnlyList`1" />.
		/// </summary>
		public ReadOnlyList(int capacity)
			: base(capacity) {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.ReadOnlyList`1" />.
		/// </summary>
		public ReadOnlyList(IEnumerable<T> collection)
			: base(collection) {
		}

	}


	/// <summary>
	/// Defines methods for an editable collection of layers.
	/// </summary>
	/// <status>reviewed</status>
	public interface ILayerCollection : ICollection<Layer> {

		/// <summary>
		/// Retrieve the <see cref="T:Dataweb.NShape.Layer" /> instance associated with the given <see cref="T:Dataweb.NShape.LayerIds" />.
		/// </summary>
		Layer this[LayerIds layerId] { get; }

		/// <summary>
		/// Retrieve the <see cref="T:Dataweb.NShape.Layer" /> instance with the given name.
		/// </summary>
		Layer this[string name] { get; }
		
		/// <summary>
		/// Retrieve the <see cref="T:Dataweb.NShape.Layer" /> instance associated with the given <see cref="T:Dataweb.NShape.LayerIds" />.
		/// </summary>
		Layer GetLayer(LayerIds layerId);

		/// <summary>
		/// Retrieve all <see cref="T:Dataweb.NShape.Layer" /> instances associated with the given combination of <see cref="T:Dataweb.NShape.LayerIds" />.
		/// </summary>
		IEnumerable<Layer> GetLayers(LayerIds layerIds);

		/// <summary>
		/// Retrieve the <see cref="T:Dataweb.NShape.Layer" /> instance with the given name.
		/// </summary>
		Layer FindLayer(string name);

		/// <summary>
		/// Rename the specified <see cref="T:Dataweb.NShape.Layer" />.
		/// </summary>
		bool RenameLayer(string previousName, string newName);

	}


	/// <summary>
	/// Holds a list of layers.
	/// </summary>
	internal class LayerCollection : ILayerCollection {

		internal LayerCollection(Diagram diagram) {
			this.diagram = diagram;
			// create an entry for each layer so that the layer can be addressed directly
			foreach (LayerIds layerId in Enum.GetValues(typeof(LayerIds))) {
				if (layerId == LayerIds.None || layerId == LayerIds.All) continue;
				layers.Add(null);
			}
		}


		#region ILayerCollection Members

		public Layer this[LayerIds layerId] {
			get { 
				Layer result = GetLayer(layerId);
				if (result == null) throw new ItemNotFoundException<Layer>(result);
				return result;
			}
		}


		public Layer this[string name] {
			get {
				Layer result = FindLayer(name);
				if (result == null) throw new ItemNotFoundException<Layer>(result);
				return result;
			}
		}

		
		public Layer FindLayer(string name) {
			if (name == null) throw new ArgumentNullException("name");
			int cnt = layers.Count;
			for (int i = 0; i < cnt; ++i) {
				if (layers[i] != null && string.Compare(layers[i].Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
					return layers[i];
			}
			return null;
		}


		public Layer GetLayer(LayerIds layerId) {
			int layerBit = GetLayerBit(layerId);
			if (layerBit < 0) throw new ItemNotFoundException<LayerIds>(layerId);
			return layers[layerBit];
		}


		public IEnumerable<Layer> GetLayers(LayerIds layerId) {
			foreach (int layerBit in GetLayerBits(layerId)) {
				if (layerBit == -1) continue;
				if (layers[layerBit] != null)
					yield return layers[layerBit];
			}
		}


		public bool RenameLayer(string previousName, string newName) {
			if (string.IsNullOrEmpty(previousName)) throw new ArgumentNullException("previousName");
			if (string.IsNullOrEmpty(newName)) throw new ArgumentNullException("newName");
			if (FindLayer(newName) != null) throw new ArgumentException(string.Format("A layer named '{0}' already exists.", newName));
			Layer layer = FindLayer(previousName);
			if (layer != null) {
				layer.Name = newName;
				return true;
			} else return false;
		}

		#endregion


		#region ICollection<Layer> Members

		public void Add(Layer item) {
			if (item == null) throw new ArgumentNullException("item");
			for (int i = 0; i < layers.Count; ++i) {
				if (layers[i] == null) {
					item.Id = (LayerIds)Math.Pow(2, i);
					layers[i] = item;
					++layerCount;
					break;
				}
			}
			Debug.Assert(item.Id != LayerIds.None);
		}

		public void Clear() {
			for (int i = 0; i < layers.Count; ++i)
				layers[i] = null;
			layerCount = 0;
		}

		public bool Contains(Layer item) {
			return layers.Contains(item);
		}

		public void CopyTo(Layer[] array, int arrayIndex) {
			if (array == null) throw new ArgumentNullException("array");
			layers.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return layerCount; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(Layer item) {
			if (item == null) throw new ArgumentNullException("item");
			int layerBit = GetLayerBit(item.Id);
			if (layerBit >= 0) {
				Debug.Assert(item.Id == layers[layerBit].Id);
				layers[layerBit] = null;
				--layerCount;
				return true;
			} else return false;
		}

		#endregion


		#region IEnumerable<Layer> Members

		public IEnumerator<Layer> GetEnumerator() {
			return Enumerator.Create(layers);
		}

		#endregion


		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return layers.GetEnumerator();
		}

		#endregion


		#region [Private] Methods

		private int GetLayerBit(LayerIds layerId) {
			int result = -1;
			foreach (int layerBit in GetLayerBits(layerId)) {
				if (result < 0) result = layerBit;
				else throw new ArgumentException(string.Format("{0} is not a valid LayerId for one single layer."));
			}
			return result;
		}


		private IEnumerable<int> GetLayerBits(LayerIds layerIds) {
			if (layerIds == LayerIds.None) yield break;
			int bitNo = 0;
			foreach (LayerIds id in Enum.GetValues(typeof(LayerIds))) {
				if (id == LayerIds.None || id == LayerIds.All) continue;
				if ((layerIds & id) != 0)
					yield return bitNo;
				++bitNo;
			}
		}

		#endregion


		#region [Private] Types and Fields

		private struct Enumerator : IEnumerator<Layer>, IEnumerator {

			public static Enumerator Create(List<Layer> layerList) {
				if (layerList == null) throw new ArgumentNullException("layerList");
				Enumerator result = Enumerator.Empty;
				result.layerList = layerList;
				result.layerCount = layerList.Count;
				result.currentIdx = -1;
				result.currentLayer = null;
				return result;
			}


			public static readonly Enumerator Empty;


			public Enumerator(List<Layer> layerList) {
				if (layerList == null) throw new ArgumentNullException("layerList");
				this.layerList = layerList;
				this.layerCount = layerList.Count;
				this.currentIdx = -1;
				this.currentLayer = null;
			}


			#region IEnumerator<Layer> Members

			public Layer Current { get { return currentLayer; } }

			#endregion


			#region IDisposable Members

			public void Dispose() {
				// nothing to do
			}

			#endregion


			#region IEnumerator Members

			object IEnumerator.Current { get { return currentLayer; } }

			public bool MoveNext() {
				bool result = false;
				currentLayer = null;
				while (currentIdx < layerCount - 1 && !result) {
					currentLayer = layerList[++currentIdx];
					if (currentLayer != null) result = true;
				}
				return result;
			}

			public void Reset() {
				currentIdx = -1;
				currentLayer = null;
			}

			#endregion


			static Enumerator() {
				Empty.layerList = null;
				Empty.layerCount = 0;
				Empty.currentIdx = -1;
				Empty.currentLayer = null;
			}

			#region Fields
			private List<Layer> layerList;
			private int layerCount;
			private int currentIdx;
			private Layer currentLayer;
			#endregion
		}


		private List<Layer> layers = new List<Layer>(31);
		private int layerCount = 0;
		private Diagram diagram = null;
		
		#endregion
	}

}
