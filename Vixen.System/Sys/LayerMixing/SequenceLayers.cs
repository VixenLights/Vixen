using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module.MixingFilter;
using Vixen.Services;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	[KnownType(typeof(DefaultLayer))]
	[KnownType(typeof(StandardLayer))]
	public class SequenceLayers 
	{
		/// <summary>
		/// Effects reside in a layer. This is the mapping of the effect id to the layer id.
		/// </summary>
		[DataMember]
		private readonly Dictionary<Guid, Guid> _effectLayerMap;

		private static readonly ILayer DefaultLayer = new DefaultLayer();

		/// <summary>
		/// This is a mapping of the layer id to the layer object for fast lookups.
		/// </summary>
		private Dictionary<Guid, ILayer> _layerMap;

		private ISequence _sequence;

		public SequenceLayers()
		{
			Layers = new ObservableCollection<ILayer>();
			_effectLayerMap = new Dictionary<Guid, Guid>();
			_layerMap = new Dictionary<Guid, ILayer>();
			//Add the default layer
			AddLayer(DefaultLayer);
		}

		public void ReplaceLayerAt(int index, ILayer layer)
		{
			if (index >= Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			Layers[index] = layer;
			UpdateLevels();
		}

		[DataMember]
		public ObservableCollection<ILayer> Layers { get; private set; }

		public static ILayer GetDefaultLayer()
		{
			return DefaultLayer;
		}

		public bool IsDefaultLayer(ILayer layer)
		{
			return layer.Type == LayerType.Default;
		}

		public void AddLayer(ILayer layer)
		{
			Layers.Insert(0, layer);
			if (!_layerMap.ContainsKey(layer.Id))
			{
				_layerMap.Add(layer.Id, layer);
			}
			UpdateLevels();
		}

		public void AddLayer(ILayerMixingFilterInstance mixer)
		{
			var layer = new StandardLayer(EnsureUniqueName("Default")) {LayerMixingFilter = mixer};
			AddLayer(layer);
		}

		public int Count
		{
			get { return Layers.Count; } 
		}

		public void MoveLayer(int indexFrom, int indexTo)
		{
			if (indexFrom == Layers.Count - 1 || indexTo == Layers.Count-1) throw new ArgumentOutOfRangeException();
			Layers.Move(indexFrom, indexTo);
			UpdateLevels();
		}

		public int IndexOfLayer(ILayer item)
		{
			return Layers.IndexOf(item);
		}

		public void InsertLayer(int index, ILayer layer)
		{
			if (index == Layers.Count - 1) throw new ArgumentOutOfRangeException();
			Layers.Insert(index, layer);
			if (!_layerMap.ContainsKey(layer.Id))
			{
				_layerMap.Add(layer.Id, layer);
			}
			UpdateLevels();
		}

		public void Remove(ILayer layer)
		{
			RemoveLayerAt(IndexOfLayer(layer));
		}

		public void RemoveLayerAt(int index)
		{
			
			if (index >= Layers.Count-1) throw new ArgumentOutOfRangeException();

			//when we remove, we move all effects from the old layer into the default layer
			//as default is the default we can just remove all the references to keep things neat.
			var id = Layers[index].Id;
			
			foreach (var item in _effectLayerMap.Where(kvp => kvp.Value == id).ToList())
			{
				_effectLayerMap.Remove(item.Key);
			}

			Layers.RemoveAt(index);
			_layerMap.Remove(id);
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
			return Layers[Layers.Count-1];

		}

		public bool ContainsLayer(Guid layerId)
		{
			return _layerMap.ContainsKey(layerId);
		}

		/// <summary>
		/// Get the layer with the name and type specified
		/// </summary>
		/// <param name="name"></param>
		/// <param name="typeId"></param>
		/// <returns>ILayer if it exists or null</returns>
		public ILayer GetLayer(string name, Guid typeId)
		{
			return _layerMap.Values.First(x => x.LayerName.Equals(name) && x.FilterTypeId.Equals(typeId));
		}

		private string EnsureUniqueName(string name)
		{
			if (Layers.Any(x => x.LayerName == name))
			{
				string originalName = name;
				bool unique;
				int counter = 2;
				do
				{
					name = string.Format("{0} - {1}", originalName, counter++);
					unique = Layers.All(x => x.LayerName != name);
				} while (!unique);
			}
			return name;
		}

		public void RemoveEffectNodeFromLayers(IEffectNode node)
		{
			_effectLayerMap.Remove(node.Effect.InstanceId);
		}

		public void AssignEffectNodeToLayer(IEffectNode node, Guid LayerId)
		{
			ILayer layer;
			if(_layerMap.TryGetValue(LayerId, out layer))
			{
				AssignEffectNodeToLayer(node, layer);
			}
		}

		public void AssignEffectNodeToLayer(IEffectNode node, ILayer layer)
		{
			//First remove it from any already existing layers
			RemoveEffectNodeFromLayers(node);
			if (!IsDefaultLayer(layer) && _layerMap.ContainsKey(layer.Id))
			{
				//if it is not the default then assign it to the new layer
				_effectLayerMap[node.Effect.InstanceId] = layer.Id;
			}
		}

		public void AssignEffectNodeToDefaultLayer(IEffectNode node)
		{
			//We jusr remove it because if it is not found it will be in the default.
			RemoveEffectNodeFromLayers(node);
		}
		
		private void UpdateLevels()
		{
			var count = Layers.Count;

			for (int i = 0; i < count; i++)
			{
				Layers[i].LayerLevel = count-i-1;
			}
		}

		[OnDeserialized]
		private void PopulateMap(StreamingContext ctx)
		{
			_layerMap = new Dictionary<Guid, ILayer>();
			if (Layers != null)
			{
				foreach (var layer in Layers)
				{
					_layerMap.Add(layer.Id, layer);
				}
			}
		}

	}
}
