using System.Collections.Generic;
using System.Xml.Serialization;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model.ExternalVendorInventory
{
	[XmlRoot("modelinventory")]
	public class XModelInventory : BindableBase
	{
		[XmlElement("vendor", typeof(Vendor))]
		public Vendor Vendor { get; set; }

		[XmlArray("categories")]
		[XmlArrayItem("category", typeof(Category))]
		public List<Category> Categories { get; set; }

		[XmlArray("models")]
		[XmlArrayItem("model", typeof(Model))]
		public List<Model> Models { get; set; }
	}
}
