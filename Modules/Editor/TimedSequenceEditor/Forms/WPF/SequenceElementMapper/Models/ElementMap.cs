using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Collections;
using Catel.Data;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Models
{
	public class ElementMap: SavableModelBase<ElementMap>
	{
		private readonly HashSet<Guid> _sourceIds = new HashSet<Guid>();
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
			set { SetValue(ElementMappingsProperty, value); }
		}

		/// <summary>
		/// ElementMappings property data.
		/// </summary>
		public static readonly PropertyData ElementMappingsProperty = RegisterProperty("ElementMappings", typeof(FastObservableCollection<ElementMapping>));

		#endregion

		public void Add(ElementMapping map)
		{
			if (!_sourceIds.Contains(map.SourceId))
			{
				ElementMappings.Add(map);
				_sourceIds.Add(map.SourceId);
				SortByName();
			}
		}

		public void AddRange(IEnumerable<ElementMapping> maps)
		{
			var mapsToAdd = maps.Where(x => !_sourceIds.Contains(x.SourceId));
			ElementMappings.AddItems(mapsToAdd);
			_sourceIds.AddRange(mapsToAdd.Select(x => x.SourceId));
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
			return _sourceIds.Contains(sourceId);
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
			_sourceIds.AddRange(GetSourceNameIds().Keys);
		}
	}
}
