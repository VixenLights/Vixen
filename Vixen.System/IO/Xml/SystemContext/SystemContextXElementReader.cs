using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.SystemContext
{
	using Vixen.Sys;

	internal class SystemContextXElementReader : IObjectContentReader<XElement, SystemContext>
	{
		public XElement ReadContentFromObject(SystemContext obj)
		{
			XElement content = new XElement("SystemContext");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.SystemContext);
			XmlSystemContextFilePolicy xmlFilePolicy = new XmlSystemContextFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj)
		{
			if (!(obj is SystemContext)) throw new InvalidOperationException("Object must be a SystemContext.");
			return ReadContentFromObject((SystemContext) obj);
		}
	}
}