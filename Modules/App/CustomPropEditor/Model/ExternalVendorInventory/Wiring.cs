using System.Collections.Generic;
using System.Xml.Serialization;

namespace VixenModules.App.CustomPropEditor.Model.ExternalVendorInventory
{
	public class Wiring
	{
		[XmlElement("name")]
		public string Name { get; set; }

		[XmlElement("description")]
		public string Description { get; set; }

		[XmlElement("xmodellink")]
		public string XModelLink { get; set; }

		[XmlElement("imageFile")]
		public List<string> Images { get; set; }
	}
}
