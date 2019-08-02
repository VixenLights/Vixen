using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class XModelInventoryImporter
	{
		public async Task<Model.InternalVendorInventory.ModelInventory> Import(string xml)
		{
			XmlRootAttribute xRoot = new XmlRootAttribute();
			xRoot.ElementName = "modelinventory";
			xRoot.IsNullable = true;

			Model.ExternalVendorInventory.XModelInventory xModelInventory = DeserializeObject<Model.ExternalVendorInventory.XModelInventory>(xml, xRoot);

			XModelInventoryMapper mapper = new XModelInventoryMapper();
			return await Task.FromResult(mapper.Map(xModelInventory));
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
