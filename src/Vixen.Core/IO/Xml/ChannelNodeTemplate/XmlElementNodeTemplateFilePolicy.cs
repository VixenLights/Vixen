using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Serializer;

namespace Vixen.IO.Xml.ElementNodeTemplate
{
	using Vixen.Sys;

	internal class XmlElementNodeTemplateFilePolicy : ElementNodeTemplateFilePolicy
	{
		private ElementNodeTemplate _template;
		private XElement _content;

		public XmlElementNodeTemplateFilePolicy(ElementNodeTemplate template, XElement content)
		{
			_template = template;
			_content = content;
		}

		protected override void WriteElementNode()
		{
			XmlElementNodeSerializer serializer = new XmlElementNodeSerializer(null);
			XElement element = serializer.WriteObject(_template.ElementNode);
			_content.Add(element);
		}

		protected override void ReadElementNode()
		{
			XmlElementNodeSerializer serializer = new XmlElementNodeSerializer(VixenSystem.Elements);
			_template.ElementNode = serializer.ReadObject(_content);
		}
	}
}