using System.Collections.Generic;
using System.Xml.Serialization;

namespace VixenModules.App.CustomPropEditor.Model.ExternalVendorInventory
{
	public class Model 
	{
		[XmlElement("id")]
		public uint Id { get; set; }

		[XmlElement("categoryid")]
		public List<uint> CategoryIds { get; set; }

		[XmlElement("name")]
		public string Name { get; set; }

		[XmlElement("type")]
		public string Type { get; set; }

		[XmlElement("weblink")]
		public string Weblink { get; set; }

		[XmlElement("material")]
		public string Material { get; set; }

		[XmlElement("width")]
		public string Width { get; set; }

		[XmlElement("height")]
		public string Height { get; set; }

		[XmlElement("thickness")]
		public string Thickness { get; set; }

		[XmlElement("pixelcount")]
		public uint PixelCount { get; set; }

		[XmlElement("pixeldescription")]
		public string PixelDescription { get; set; }

		[XmlElement("pixelspacing")]
		public string PixelSpacing { get; set; }

		[XmlElement("imagefile")]
		public List<string> ImageFile { get; set; }

		[XmlElement("notes")]
		public string Notes { get; set; }

		[XmlElement("wiring", typeof(Wiring))]
		public List<Wiring> Wiring { get; set; }

	}
}
