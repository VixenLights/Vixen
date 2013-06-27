using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.SystemConfig
{
	using Vixen.Sys;

	internal class SystemConfigXElementReader : IObjectContentReader<XElement, SystemConfig>
	{
		public XElement ReadContentFromObject(SystemConfig obj)
		{
			XElement content = new XElement("SystemConfig");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.SystemConfig);
			XmlSystemConfigFilePolicy xmlFilePolicy = new XmlSystemConfigFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj)
		{
			if (!(obj is SystemConfig)) throw new InvalidOperationException("Object must be a SystemConfig.");
			return ReadContentFromObject((SystemConfig) obj);
		}
	}
}