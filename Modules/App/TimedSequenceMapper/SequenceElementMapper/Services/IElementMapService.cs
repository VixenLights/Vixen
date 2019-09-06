using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Models;

namespace VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Services
{
	public interface IElementMapService
	{
		/// <summary>
		/// Creates a new Map from the specified sources.
		/// </summary>
		/// <param name="elementSources"></param>
		ElementMap InitializeMap(Dictionary<Guid, string> elementSources, string name);

		/// <summary>
		/// Creates a new empty map
		/// </summary>
		ElementMap InitializeMap(string name);

		/// <summary>
		/// Load map from file path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		Task<bool> LoadMapAsync(string path);

		/// <summary>
		/// Save Map to file path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		Task<bool> SaveMapAsync(string path);

		void EndEdit();

		void BeginEdit();

		void CancelEdit();

		/// <summary>
		/// The Element map
		/// </summary>
		ElementMap ElementMap { get;}

		Dictionary<Guid, string> SourceActiveElements { get; set; }

		void RegisterMapMessages(object recipient, Action<ElementMapService.MapMessage> action);

		void UnRegisterMapMessages(object recipient, Action<ElementMapService.MapMessage> action);
	}
}
