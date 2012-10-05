using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.ChannelNodeTemplate {
	using Vixen.Sys;

	class ChannelNodeTemplateXElementWriter : IObjectContentWriter<XElement, ChannelNodeTemplate> {
		public void WriteContentToObject(XElement content, ChannelNodeTemplate obj) {
			
			XmlChannelNodeTemplateFilePolicy xmlFilePolicy = new XmlChannelNodeTemplateFilePolicy(obj, content);
			xmlFilePolicy.Read();
		}

		public int GetContentVersion(XElement content) {
			if(content == null) throw new ArgumentNullException("content");

			return XmlRootAttributeVersion.GetVersion(content);
		}

		void IObjectContentWriter.WriteContentToObject(object content, object obj) {
			if(!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");
			if(!(obj is ChannelNodeTemplate)) throw new InvalidOperationException("Object must be a ChannelNodeTemplate.");

			WriteContentToObject((XElement)content, (ChannelNodeTemplate)obj);
		}

		int IObjectContentWriter.GetContentVersion(object content) {
			if(!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");
			return GetContentVersion((XElement)content);
		}
	}
}
