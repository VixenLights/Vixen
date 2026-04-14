using System.Xml.Linq;

namespace Vixen.IO.Xml.ModuleStore
{
	internal class ModuleStoreXElementReader : IObjectContentReader<XElement, Sys.ModuleStore>
	{
		public XElement ReadContentFromObject(Sys.ModuleStore obj)
		{
			XElement content = new XElement("ModuleStore");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.ModuleStore);
			XmlModuleStoreFilePolicy xmlFilePolicy = new XmlModuleStoreFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj)
		{
			if (!(obj is Sys.ModuleStore)) throw new InvalidOperationException("Object must be a ModuleStore.");
			return ReadContentFromObject((Sys.ModuleStore) obj);
		}
	}
}