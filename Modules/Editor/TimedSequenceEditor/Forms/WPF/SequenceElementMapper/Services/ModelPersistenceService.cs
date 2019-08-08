using System.IO;
using System.Threading.Tasks;
using Catel;
using Catel.Data;
using Catel.Runtime.Serialization;
using Catel.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Services
{
	public class ModelPersistenceService<T> : IModelPersistenceService<T> where T:ModelBase
	{
		private readonly IJsonSerializer _serializer;
		private readonly ISerializationConfiguration _serializationConfiguration;

		public ModelPersistenceService(IJsonSerializer serializer)
		{
			Argument.IsNotNull(() => serializer);
			_serializer = serializer;
			_serializer.WriteTypeInfo = false;
			_serializer.PreserveReferences = false;
			_serializationConfiguration = new JsonSerializationConfiguration { Formatting = Formatting.Indented };
		}

		#region Implementation of IModelPersistenceService<T>

		/// <inheritdoc />
		public async Task<T> LoadModelAsync(string path)
		{
			return await Task.Factory.StartNew(() =>
			{
				using (var fileStream = File.Open(path, FileMode.Open))
				{
					return _serializer.Deserialize<T>(fileStream, _serializationConfiguration);
				}
			});
		}

		/// <inheritdoc />
		public async Task<bool> SaveModelAsync(T model, string path)
		{
			return await Task.Factory.StartNew(() =>
			{
				using (var fileStream = File.Open(path, FileMode.Create))
				{
					_serializer.Serialize(model, fileStream, _serializationConfiguration);
				}

				return true;

			});
		}

		#endregion
	}
}