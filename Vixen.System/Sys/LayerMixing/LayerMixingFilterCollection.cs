using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	[KnownType(typeof(DefaultLayerMixingDefinition))]
	[KnownType(typeof(LayerMixingDefinition))]
	public class LayerMixingFilterCollection
	{
		[DataMember]
		private readonly List<BaseLayerMixingDefinition> _definitions = new List<BaseLayerMixingDefinition>();
		public LayerMixingFilterCollection()
		{
			Id = Guid.NewGuid();
			//Add the default layer
			Add(new DefaultLayerMixingDefinition());
		}
		
		[DataMember]
		public Guid Id { get; set; }

		public void Update(List<BaseLayerMixingDefinition> definitions)
		{
			_definitions.Clear();
			for (int i = 0; i < definitions.Count; i++)
			{
				_definitions[i] = definitions[i];
			}
			UpdateLevels();
		}

		public void ReplaceAt(int index, BaseLayerMixingDefinition item)
		{
			if (index >= Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			_definitions[index] = item;
			UpdateLevels();
		}

		public List<BaseLayerMixingDefinition> GetLayerDefinitions()
		{
			return _definitions.ToList();
		} 

		public void Add(BaseLayerMixingDefinition item)
		{
			_definitions.Insert(Count, item);
			UpdateLevels();
		}

		public int Count
		{
			get { return _definitions.Count; } 
		}

		public void Move(int indexFrom, int indexTo)
		{
			if (indexFrom == 0 || indexTo == 0) throw new ArgumentOutOfRangeException();
			var itemToMove = _definitions[indexFrom];
			_definitions.RemoveAt(indexFrom);
			if (indexTo > indexFrom) indexTo--;
			_definitions.Insert(indexTo, itemToMove);
			UpdateLevels();
		}

		public int IndexOf(BaseLayerMixingDefinition item)
		{
			return _definitions.IndexOf(item);
		}

		public void Insert(int index, BaseLayerMixingDefinition item)
		{
			if (index == 0) throw new ArgumentOutOfRangeException();
			_definitions.Insert(index, item);
			UpdateLevels();
		}

		public void RemoveAt(int index)
		{
			if (index == 0) throw new ArgumentOutOfRangeException();
			_definitions.RemoveAt(index);
			UpdateLevels();
		}

		private void UpdateLevels()
		{
			for (int i = 0; i < _definitions.Count; i++)
			{
				_definitions[i].LayerLevel = i;
			}
		}
	}
}
