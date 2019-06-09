using System.Xml.Serialization;

namespace VixenModules.App.CustomPropEditor.Model.ExternalVendorInventory
{
	public class Wiring
	{
		[XmlElement("xmodellink")]
		public string XModelLink { get; set; }
	}
}
