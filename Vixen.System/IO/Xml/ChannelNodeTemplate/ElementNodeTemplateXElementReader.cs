using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.ElementNodeTemplate {
	using Vixen.Sys;

	class ElementNodeTemplateXElementReader : IObjectContentReader<XElement, ElementNodeTemplate> {
		public XElement ReadContentFromObject(ElementNodeTemplate obj) {
			XElement content = new XElement("ElementNodeTemplate");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.ElementNodeTemplate);
			XmlElementNodeTemplateFilePolicy xmlFilePolicy = new XmlElementNodeTemplateFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj) {
			if(!(obj is ElementNodeTemplate)) throw new InvalidOperationException("Object must be a ElementNodeTemplate.");
			return ReadContentFromObject((ElementNodeTemplate)obj);
		}
	}
}
