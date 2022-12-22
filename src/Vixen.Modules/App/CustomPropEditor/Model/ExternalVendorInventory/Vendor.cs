using System.Xml.Serialization;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model.ExternalVendorInventory
{
	public class Vendor : BindableBase
	{
		[XmlElement("name")]
		public string Name { get; set; }
		
		[XmlElement("contact")]
		public string Contact { get; set; }
		
		[XmlElement("email")]
		public string Email { get; set; }
		
		[XmlElement("phone")]
		public string Phone { get; set; }
		
		[XmlElement("website")]
		public string Website { get; set; }
		
		[XmlElement("facebook")]
		public string Facebook { get; set; }

		[XmlElement("notes")]
		public string Notes { get; set; }
		
		[XmlElement("logolink")]
		public string LogoLink { get; set; }
		
	}
}
