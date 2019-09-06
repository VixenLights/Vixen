using System.IO;
using System.Threading.Tasks;
using Catel.Data;
using Newtonsoft.Json;
using Vixen.IO.JSON;
using Vixen.Sys;

namespace VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Services
{
	public class ModelPersistenceService<T> : IModelPersistenceService<T> where T:ModelBase
	{
		private readonly JsonSerializer _serializer;
		public ModelPersistenceService()
		{
			_serializer = JsonSerializer.CreateDefault();
			_serializer.NullValueHandling = NullValueHandling.Ignore;
			_serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
			_serializer.Formatting = Formatting.Indented;
			_serializer.ContractResolver = new CustomContractResolver();
		}

		#region Implementation of IModelPersistenceService<T>

		/// <inheritdoc />
		public async Task<T> LoadModelAsync(string path)
		{
			return await Task.Factory.StartNew(() =>
			{
				if (File.Exists(path))
				{
					using (StreamReader sw = new StreamReader(path))
					using (JsonReader reader = new JsonTextReader(sw))
					{
						return _serializer.Deserialize<T>(reader);
					}
				}
				
				return null;
			});
		}

		/// <inheritdoc />
		public async Task<bool> SaveModelAsync(T model, string path)
		{
			return await Task.Factory.StartNew(() =>
			{
				using (StreamWriter sw = new StreamWriter(path))
				using (JsonWriter writer = new JsonTextWriter(sw))
				{
					writer.Formatting = _serializer.Formatting;
					_serializer.Serialize(writer, model, typeof(ElementNode));
				}
			
				return true;

			});
		}

		/// <inheritdoc />
		public async Task<ElementNodeProxy> LoadElementNodeProxyAsync(string path)
		{
			return await VixenSystem.Nodes.ImportElementNodeProxy(path);
		}

		#endregion
	}
}