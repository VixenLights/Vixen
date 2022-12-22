using System.IO;
using System.Xml.Serialization;
using VixenModules.App.TimingTrackBrowser.Model.ExternalVendorInventory;
using VixenModules.App.TimingTrackBrowser.Model.InternalVendorInventory;

namespace VixenModules.App.TimingTrackBrowser.Import.XLights
{
	public class XTimingLyricInventoryImporter
	{
		public async Task<SongInventory> Import(string xml)
		{
			XmlRootAttribute xRoot = new XmlRootAttribute();
			xRoot.ElementName = "musicinventory";
			xRoot.IsNullable = true;

			MusicVendorInventory musicVendorInventory = DeserializeObject<MusicVendorInventory>(xml, xRoot);

			XTimingSongInventoryMapper mapper = new XTimingSongInventoryMapper();
			return await Task.FromResult(mapper.Map(musicVendorInventory));
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
