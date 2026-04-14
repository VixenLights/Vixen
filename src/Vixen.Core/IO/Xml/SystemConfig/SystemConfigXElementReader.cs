using System.Xml.Linq;

namespace Vixen.IO.Xml.SystemConfig
{
	internal class SystemConfigXElementReader : IObjectContentReader<XElement, Sys.SystemConfig>
	{
		public XElement ReadContentFromObject(Sys.SystemConfig obj)
		{
			XElement content = new XElement("SystemConfig");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.SystemConfig);
			XmlSystemConfigFilePolicy xmlFilePolicy = new XmlSystemConfigFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj)
		{
			if (!(obj is Sys.SystemConfig)) throw new InvalidOperationException("Object must be a SystemConfig.");
			return ReadContentFromObject((Sys.SystemConfig) obj);
		}
	}
}