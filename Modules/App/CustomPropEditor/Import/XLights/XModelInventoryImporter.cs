using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class XModelInventoryImporter
	{
		public async Task<Model.InternalVendorInventory.ModelInventory> Import(string url)
		{
			var xml = await RequestInventory(url);

			XmlRootAttribute xRoot = new XmlRootAttribute();
			xRoot.ElementName = "modelinventory";
			xRoot.IsNullable = true;

			Model.ExternalVendorInventory.XModelInventory xModelInventory = DeserializeObject<Model.ExternalVendorInventory.XModelInventory>(xml, xRoot);

			XModelInventoryMapper mapper = new XModelInventoryMapper();
			return mapper.Map(xModelInventory);
		}

		private async Task<string> RequestInventory(string url)
		{
			using (HttpClient wc = new HttpClient())
			{
				wc.Timeout = TimeSpan.FromMilliseconds(5000);
				//Get Latest inventory from the url.
				return await wc.GetStringAsync(url);
			}
		}

		private static T DeserializeObject<T>(string xml, XmlRootAttribute root)
		{

			var serializer = new XmlSerializer(typeof(T), root);
			using (var tr = new StringReader(xml))
			{
				return (T)serializer.Deserialize(tr);
			}
		}
	}
}
