using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Serializer;
using Vixen.Sys;

namespace Vixen.IO.Xml.ElementNodeTemplate
{
	internal class XmlElementNodeTemplateFilePolicy : ElementNodeTemplateFilePolicy
	{
		private Sys.ElementNodeTemplate _template;
		private XElement _content;

		public XmlElementNodeTemplateFilePolicy(Sys.ElementNodeTemplate template, XElement content)
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