using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Catel.Collections;
using Catel.Data;
using Vixen.Sys;

namespace VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Models
{
	public class ElementMap: SavableModelBase<ElementMap>
	{
		private readonly Dictionary<Guid, ElementMapping> _sourceIds = new Dictionary<Guid, ElementMapping>();
		public ElementMap()
		{
			Id= Guid.NewGuid();
			ElementMappings = new FastObservableCollection<ElementMapping>();
		}

		public ElementMap(Dictionary<Guid, string> elementSources):this()
		{
			CreateMapsForSources(elementSources);
		}

		#region Id property

		/// <summary>
		/// Gets or sets the Id value.
		/// </summary>
		public Guid Id
		{
			get { return GetValue<Guid>(IdProperty); }
			protected set { SetValue(IdProperty, value); }
		}

		/// <summary>
		/// Id property data.
		/// </summary>
		public static readonly PropertyData IdProperty = RegisterProperty("Id", typeof(Guid));

		#endregion

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string));

		#endregion

		#region ElementMappings property

		/// <summary>
		/// Gets or sets the ElementMappings value.
		/// </summary>
		public FastObservableCollection<ElementMapping> ElementMappings
		{
			get { return GetValue<FastObservableCollection<ElementMapping>>(ElementMappingsProperty); }
			set
			{
				SetValue(ElementMappingsProperty, value);
				UpdateSourceIdMap();
			}
		}

		/// <summary>
		/// ElementMappings property data.
		/// </summary>
		public static readonly PropertyData ElementMappingsProperty = RegisterProperty("ElementMappings", typeof(FastObservableCollection<ElementMapping>));

		#endregion

		#region SourceTree property

		/// <summary>
		/// Gets or sets the SourceTree value.
		/// </summary>
		public ElementNodeProxy SourceTree
		{
			get { return GetValue<ElementNodeProxy>(SourceTreeProperty); }
			set { SetValue(SourceTreeProperty, value); }
		}

		/// <summary>
		/// SourceTree property data.
		/// </summary>
		public static readonly PropertyData SourceTreeProperty = RegisterProperty("SourceTree", typeof(ElementNodeProxy));

		#endregion

		#region Overrides of ModelBase

		/// <inheritdoc />
		protected override void OnDeserialized()
		{
			UpdateSourceIdMap();
			base.OnDeserialized();
		}

		#endregion

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			OnDeserialized();
		}

		public void Clear()
		{
			ElementMappings.Clear();
			_sourceIds.Clear();
		}

		public void Add(ElementMapping map)
		{
			if (!_sourceIds.ContainsKey(map.SourceId))
			{
				_sourceIds.Add(map.SourceId, map);
				ElementMappings.Add(map);
				SortByName();
			}
		}

		public void AddRange(IEnumerable<ElementMapping> maps)
		{
			var mapsToAdd = maps.Where(x => !_sourceIds.ContainsKey(x.SourceId));
			_sourceIds.AddRange(mapsToAdd.Select(x => new KeyValuePair<Guid, ElementMapping>(x.SourceId, x)));
			ElementMappings.AddItems(mapsToAdd);
			SortByName();
		}

		private void SortByName()
		{
			ElementMappings.Sort(delegate (ElementMapping x, ElementMapping y)
			{
				if (x.SourceName == null && y.SourceName == null) return 0;
				if (x.SourceName == null) return -1;
				if (y.SourceName == null) return 1;
				return string.Compare(x.SourceName, y.SourceName, StringComparison.CurrentCulture);
			});
		}

		public bool Contains(Guid sourceId)
		{
			return _sourceIds.ContainsKey(sourceId);
		}

		public ElementMapping GetBySourceId(Guid id)
		{
			_sourceIds.TryGetValue(id, out var mapping);
			return mapping;
		}

		public bool GetBySourceId(Guid sourceId, out Guid targetId)
		{
			targetId = Guid.Empty;
			var b = _sourceIds.TryGetValue(sourceId, out var em);
			if (b)
			{
				targetId = em.TargetId;
			}

			return b;
		}

		public Dictionary<string, Guid> GetSourceNameToTargetIdMap()
		{
			return ElementMappings.ToDictionary(x => x.SourceName, x=> x.TargetId);
		}

		public IEnumerable<string> GetSourceNames(bool distinct = false)
		{
			if (distinct)
			{
				return ElementMappings.Select(x => x.SourceName).Distinct();
			}

			return ElementMappings.Select(x => x.SourceName);
		}

		public Dictionary<Guid, string> GetSourceNameIds()
		{
			return ElementMappings.ToDictionary(x => x.SourceId, x => x.SourceName);
		}

		public void CreateMapsForSources(Dictionary<Guid, string> elementSources)
		{
			ElementMappings.AddRange(elementSources.Select(x => new ElementMapping(x.Key, x.Value)).OrderBy(x => x.SourceName));
			UpdateSourceIdMap();
		}

		internal void UpdateSourceIdMap()
		{
			_sourceIds.Clear();
			_sourceIds.AddRange(ElementMappings.Select(x => new KeyValuePair<Guid, ElementMapping>(x.SourceId, x)));
		}
	}
}
