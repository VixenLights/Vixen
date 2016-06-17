using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	[KnownType(typeof(DefaultLayer))]
	[KnownType(typeof(StandardLayer))]
	public class SequenceLayers
	{
		[DataMember]
		private readonly Dictionary<Guid, Guid> _effectLayerMap;

		[DataMember]
		private readonly List<ILayer> _layers = new List<ILayer>();
		private Dictionary<Guid, ILayer> _layerMap; 
		public SequenceLayers()
		{
			Id = Guid.NewGuid();
			_effectLayerMap = new Dictionary<Guid, Guid>();
			_layerMap = new Dictionary<Guid, ILayer>();
			//Add the default layer
			AddLayer(new DefaultLayer());
		}
		
		[DataMember]
		public Guid Id { get; set; }

		public void UpdateLayers(List<ILayer> layers)
		{
			_layers.Clear();
			for (int i = 0; i < layers.Count; i++)
			{
				_layers[i] = layers[i];
			}
			UpdateLevels();
		}

		public void ReplaceLayerAt(int index, ILayer layer)
		{
			if (index >= Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			_layers[index] = layer;
			UpdateLevels();
		}

		public List<ILayer> GetLayers()
		{
			return _layers.ToList();
		}

		public ILayer GetDefaultLayer()
		{
			return _layers[0];
		} 

		public void AddLayer(ILayer layer)
		{
			_layers.Insert(Count, layer);
			if (!_layerMap.ContainsKey(layer.Id))
			{
				_layerMap.Add(layer.Id, layer);
			}
			UpdateLevels();
		}

		public int Count
		{
			get { return _layers.Count; } 
		}

		public void MoveLayer(int indexFrom, int indexTo)
		{
			if (indexFrom == 0 || indexTo == 0) throw new ArgumentOutOfRangeException();
			var itemToMove = _layers[indexFrom];
			_layers.RemoveAt(indexFrom);
			if (indexTo > indexFrom) indexTo--;
			_layers.Insert(indexTo, itemToMove);
			UpdateLevels();
		}

		public int IndexOfLayer(ILayer item)
		{
			return _layers.IndexOf(item);
		}

		public void InsertLayer(int index, ILayer layer)
		{
			if (index == 0) throw new ArgumentOutOfRangeException();
			_layers.Insert(index, layer);
			if (!_layerMap.ContainsKey(layer.Id))
			{
				_layerMap.Add(layer.Id, layer);
			}
			UpdateLevels();
		}

		public void RemoveLayerAt(int index)
		{
			if (index == 0) throw new ArgumentOutOfRangeException();
			if (index >= _layers.Count) throw new ArgumentOutOfRangeException();

			//when we remove, we move all effects from the old layer into the default layer
			var layer = _layers[index];
			var defaultLayer = GetDefaultLayer();
			_effectLayerMap.Where(x => x.Value.Equals(layer.Id)).Select(x => x.Value == defaultLayer.Id);
			_layers.RemoveAt(index);
			_layerMap.Remove(layer.Id);
			UpdateLevels();
		}

		public ILayer GetLayer(IEffectNode node)
		{
			Guid layerId;
			if(_effectLayerMap.TryGetValue(node.Effect.InstanceId, out layerId))
			{
				ILayer layer;
				if (_layerMap.TryGetValue(layerId, out layer))
				{
					return layer;
				}
				
			}
			return _layers[0];

		}

		public void AssignEffectNodeToLayer(IEffectNode node, ILayer layer)
		{
			_effectLayerMap[node.Effect.InstanceId] = layer.Id;
		}

		public void AssignEffectNodeToDefaultLayer(IEffectNode node)
		{
			_effectLayerMap[node.Effect.InstanceId] = _layers[0].Id;
		}
		
		private void UpdateLevels()
		{
			for (int i = 0; i < _layers.Count; i++)
			{
				_layers[i].LayerLevel = i;
			}
		}

		[OnDeserialized]
		private void PopulateMap(StreamingContext ctx)
		{
			_layerMap = new Dictionary<Guid, ILayer>();
			if (_layers != null)
			{
				foreach (var layer in _layers)
				{
					_layerMap.Add(layer.Id, layer);
				}
			}
		}
	}
}
