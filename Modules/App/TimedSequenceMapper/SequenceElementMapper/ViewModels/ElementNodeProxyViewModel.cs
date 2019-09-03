using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Models;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Services;

namespace VixenModules.App.TimedSequenceMapper.SequenceElementMapper.ViewModels
{
	public class ElementNodeProxyViewModel : ViewModelBase
	{
		private ElementMap _map;
		private readonly IElementMapService _elementMapService;

		public ElementNodeProxyViewModel(ElementNodeProxy proxy, IElementMapService elementMapService)
		{
			_elementMapService = elementMapService;
			ElementNodeProxy = proxy;
			_map = elementMapService.ElementMap;
			ElementMapping = _map.GetBySourceId(proxy.Id);
			elementMapService.RegisterMapMessages(this, OnElementMapChanged);
			if (ElementMapping == null)
			{
				_elementMapService.ElementMap.ElementMappings.CollectionChanged += ElementMappings_CollectionChanged;
			}
		}

		private void ElementMappings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ElementMapping = _map.GetBySourceId(ElementNodeProxy.Id);
			if (ElementMapping != null)
			{
				_map.ElementMappings.CollectionChanged -= ElementMappings_CollectionChanged;
			}
		}

		protected override async Task CloseAsync()
		{
			// TODO: unsubscribe from events here
			_elementMapService.UnRegisterMapMessages(this, OnElementMapChanged);
			_elementMapService.ElementMap.ElementMappings.CollectionChanged -= ElementMappings_CollectionChanged;
			await base.CloseAsync();
		}

		private void OnElementMapChanged(ElementMapService.MapMessage obj)
		{
			if (obj.Data.Equals(ElementMapService.MapMessageType.New))
			{
				_map.ElementMappings.CollectionChanged -= ElementMappings_CollectionChanged;
				_map = _elementMapService.ElementMap;
			}
			ElementMapping = _map.GetBySourceId(ElementNodeProxy.Id);
			if (ElementMapping == null)
			{
				_elementMapService.ElementMap.ElementMappings.CollectionChanged += ElementMappings_CollectionChanged;
			}
		}

		#region ElementMapping property

		/// <summary>
		/// Gets or sets the ElementMapping value.
		/// </summary>
		[Model]
		public ElementMapping ElementMapping
		{
			get { return GetValue<ElementMapping>(ElementMappingProperty); }
			set { SetValue(ElementMappingProperty, value); }
		}

		/// <summary>
		/// ElementMapping property data.
		/// </summary>
		public static readonly PropertyData ElementMappingProperty = RegisterProperty("ElementMapping", typeof(ElementMapping));

		#endregion

		#region ElementNodeProxy property

		/// <summary>
		/// Gets or sets the ElementNodeProxy value.
		/// </summary>
		[Model]
		public ElementNodeProxy ElementNodeProxy
		{
			get { return GetValue<ElementNodeProxy>(ElementNodeProxyProperty); }
			set { SetValue(ElementNodeProxyProperty, value); }
		}

		/// <summary>
		/// ElementNodeProxy property data.
		/// </summary>
		public static readonly PropertyData ElementNodeProxyProperty = RegisterProperty("ElementNodeProxy", typeof(ElementNodeProxy), null);

		#endregion

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		[ViewModelToModel("ElementNodeProxy")]
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);

		#endregion

		#region Children property

		/// <summary>
		/// Gets or sets the Children value.
		/// </summary>
		[ViewModelToModel("ElementNodeProxy")]
		public List<ElementNodeProxy> Children
		{
			get { return GetValue<List<ElementNodeProxy>>(ChildrenProperty); }
			set { SetValue(ChildrenProperty, value); }
		}

		/// <summary>
		/// Children property data.
		/// </summary>
		public static readonly PropertyData ChildrenProperty = RegisterProperty("Children", typeof(List<ElementNodeProxy>), null);

		#endregion

		#region TargetName property

		/// <summary>
		/// Gets or sets the TargetName value.
		/// </summary>
		[ViewModelToModel("ElementMapping")]
		public string TargetName
		{
			get { return GetValue<string>(TargetNameProperty); }
			set { SetValue(TargetNameProperty, value); }
		}

		/// <summary>
		/// TargetName property data.
		/// </summary>
		public static readonly PropertyData TargetNameProperty = RegisterProperty("TargetName", typeof(string), null, (sender, e) => ((ElementNodeProxyViewModel)sender).OnTargetNameChanged());

		#endregion

		/// <summary>
		/// Called when the ElementMapping property has changed.
		/// </summary>
		private void OnTargetNameChanged()
		{
			RaisePropertyChanged(nameof(IsMapped));
		}

		public bool IsMapped => ElementMapping != null && ElementMapping.TargetId != Guid.Empty;

		public bool IsActive => _elementMapService.SourceActiveElements.ContainsKey(ElementNodeProxy.Id);

	}
}
