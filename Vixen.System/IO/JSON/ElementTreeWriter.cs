using System;
using System.IO;
using Newtonsoft.Json;
using Vixen.Sys;

namespace Vixen.IO.JSON
{
	public class ElementTreeWriter: IFileWriter<ElementNodeProxy>
	{
		#region Implementation of IFileWriter

		/// <inheritdoc />
		public void WriteFile(string filePath, ElementNodeProxy node)
		{
			JsonSerializer serializer = JsonSerializer.CreateDefault();
			serializer.NullValueHandling = NullValueHandling.Ignore;
			serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
			serializer.Formatting = Formatting.Indented;
			serializer.ContractResolver = new CustomContractResolver();

			using (StreamWriter sw = new StreamWriter(filePath))
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.Formatting = serializer.Formatting;
				serializer.Serialize(writer, node, typeof(ElementNode));
				// {"ExpiryDate":new Date(1230375600000),"Price":0}
			}
		}

		/// <inheritdoc />
		public void WriteFile(string filePath, object content)
		{

			if (content is ElementNode node)
			{
				WriteFile(filePath, node);
			}

			throw new InvalidOperationException("Content must be an ElementNode.");
		}

		#endregion
	}
}
