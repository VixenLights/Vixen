using System.Xml.Serialization;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.TimingTrackBrowser.Model.ExternalVendorInventory
{
	public class Vendor : BindableBase
	{
		[XmlElement("name")]
		public string Name { get; set; }

        [XmlElement("website")]
		public string Website { get; set; }
        
		[XmlElement("logolink")]
		public string LogoLink { get; set; }
		
	}
}
