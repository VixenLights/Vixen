using System.Xml.Linq;

namespace Vixen.IO.Xml.SystemContext
{
	internal class SystemContextXElementReader : IObjectContentReader<XElement, Sys.SystemContext>
	{
		public XElement ReadContentFromObject(Sys.SystemContext obj)
		{
			XElement content = new XElement("SystemContext");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.SystemContext);
			XmlSystemContextFilePolicy xmlFilePolicy = new XmlSystemContextFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj)
		{
			if (!(obj is Sys.SystemContext)) throw new InvalidOperationException("Object must be a SystemContext.");
			return ReadContentFromObject((Sys.SystemContext) obj);
		}
	}
}