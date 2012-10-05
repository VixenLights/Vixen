using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Serializer;

namespace Vixen.IO.Xml.ChannelNodeTemplate {
	using Vixen.Sys;

	class XmlChannelNodeTemplateFilePolicy : ChannelNodeTemplateFilePolicy {
		private ChannelNodeTemplate _template;
		private XElement _content;

		public XmlChannelNodeTemplateFilePolicy(ChannelNodeTemplate template, XElement content) {
			_template = template;
			_content = content;
		}

		protected override void WriteChannelNode() {
			XmlChannelNodeSerializer serializer = new XmlChannelNodeSerializer(null);
			XElement element = serializer.WriteObject(_template.ChannelNode);
			_content.Add(element);
		}

		protected override void ReadChannelNode() {
			XmlChannelNodeSerializer serializer = new XmlChannelNodeSerializer(VixenSystem.Channels);
			_template.ChannelNode = serializer.ReadObject(_content);
		}
	}
}
