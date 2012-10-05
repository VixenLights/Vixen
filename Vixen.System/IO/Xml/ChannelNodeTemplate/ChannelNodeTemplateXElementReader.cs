using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.ChannelNodeTemplate {
	using Vixen.Sys;

	class ChannelNodeTemplateXElementReader : IObjectContentReader<XElement, ChannelNodeTemplate> {
		public XElement ReadContentFromObject(ChannelNodeTemplate obj) {
			XElement content = new XElement("ChannelNodeTemplate");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.ChannelNodeTemplate);
			XmlChannelNodeTemplateFilePolicy xmlFilePolicy = new XmlChannelNodeTemplateFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj) {
			if(!(obj is ChannelNodeTemplate)) throw new InvalidOperationException("Object must be a ChannelNodeTemplate.");
			return ReadContentFromObject((ChannelNodeTemplate)obj);
		}
	}
}
