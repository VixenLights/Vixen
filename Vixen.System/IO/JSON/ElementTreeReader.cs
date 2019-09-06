using System;
using System.IO;
using Newtonsoft.Json;
using Vixen.Sys;

namespace Vixen.IO.JSON
{
	public class ElementTreeReader: IFileReader<ElementNodeProxy>
	{
		#region Implementation of IFileReader

		/// <inheritdoc />
		public ElementNodeProxy ReadFile(string filePath)
		{
			ElementNodeProxy proxy = null;
			JsonSerializer serializer = JsonSerializer.CreateDefault();
			serializer.NullValueHandling = NullValueHandling.Ignore;
			serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
			serializer.Formatting = Formatting.Indented;
			serializer.ContractResolver = new CustomContractResolver();

			using (StreamReader sw = new StreamReader(filePath))
			using (JsonReader reader = new JsonTextReader(sw))
			{
				 proxy = serializer.Deserialize<ElementNodeProxy>(reader);
			}

			return proxy;
		}

		/// <inheritdoc />
		object IFileReader.ReadFile(string filePath)
		{
			return ReadFile(filePath);
		}

		#endregion
	}
}
