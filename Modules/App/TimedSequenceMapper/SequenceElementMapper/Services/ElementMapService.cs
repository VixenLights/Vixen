using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Catel.IoC;
using Catel.Messaging;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Models;

namespace VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Services
{
	public class ElementMapService:IElementMapService
	{
		private readonly IModelPersistenceService<ElementMap> _modelPersistenceService;

		public ElementMapService()
		{
			_modelPersistenceService = ServiceLocator.Default.ResolveType<IModelPersistenceService<ElementMap>>();
		}

		#region Implementation of IElementMapService

		/// <inheritdoc />
		public ElementMap InitializeMap(Dictionary<Guid, string> elementSources, string name)
		{
			ElementMap = new ElementMap(elementSources){Name = name};
			BeginEdit();
			MapMessage.SendWith(MapMessageType.New);
			return ElementMap;
		}

		/// <inheritdoc />
		public ElementMap InitializeMap(string name)
		{
			ElementMap = new ElementMap { Name = name };
			BeginEdit();
			MapMessage.SendWith(MapMessageType.New);
			return ElementMap;
		}

		/// <inheritdoc />
		public async Task<bool> LoadMapAsync(string path)
		{
			var map = await _modelPersistenceService.LoadModelAsync(path);
			if (map != null)
			{
				ElementMap = map;
				BeginEdit();
				MapMessage.SendWith(MapMessageType.New);
				return true;
			}

			return false;
		}

		/// <inheritdoc />
		public async Task<bool> SaveMapAsync(string path)
		{
			EndEdit();
			if (await _modelPersistenceService.SaveModelAsync(ElementMap, path))
			{
				BeginEdit();
				return true;
			}

			return false;
		}

		/// <inheritdoc />
		public void EndEdit()
		{
			((IEditableObject)ElementMap).EndEdit();
		}

		/// <inheritdoc />
		public void BeginEdit()
		{
			((IEditableObject)ElementMap).BeginEdit();
		}
		
		/// <inheritdoc />
		public void CancelEdit()
		{
			((IEditableObject)ElementMap).CancelEdit();
		}

		/// <inheritdoc />
		public ElementMap ElementMap { get; protected set; }

		/// <inheritdoc />
		public Dictionary<Guid, string> SourceActiveElements { get; set; }

		/// <inheritdoc />
		public void RegisterMapMessages(object recipient, Action<MapMessage> action)
		{
			MapMessage.Register(recipient, action);
		}

		public void UnRegisterMapMessages(object recipient, Action<MapMessage> action)
		{
			MapMessage.Unregister(recipient, action);
		}

		#endregion

		public class MapMessage : MessageBase<MapMessage, MapMessageType>
		{
			public MapMessage()
			{
				
			}

			public MapMessage(MapMessageType content)
				: base(content) { }
		}

		public enum MapMessageType
		{
			New
		}
	}
}
