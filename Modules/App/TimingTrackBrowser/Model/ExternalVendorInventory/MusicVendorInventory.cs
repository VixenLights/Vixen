using System.Collections.Generic;
using System.Xml.Serialization;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.TimingTrackBrowser.Model.ExternalVendorInventory
{
	[XmlRoot("musicinventory")]
	public class MusicVendorInventory : BindableBase
	{
		[XmlElement("vendor", typeof(Vendor))]
		public Vendor Vendor { get; set; }

		[XmlArray("categories")]
		[XmlArrayItem("category", typeof(Category))]
		public List<Category> Categories { get; set; }

		[XmlArray("music")]
		[XmlArrayItem("song", typeof(Song))]
		public List<Song> Songs { get; set; }
	}
}
