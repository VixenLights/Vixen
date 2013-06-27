using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.ModuleStore
{
	using Vixen.Sys;

	internal class ModuleStoreXElementReader : IObjectContentReader<XElement, ModuleStore>
	{
		public XElement ReadContentFromObject(ModuleStore obj)
		{
			XElement content = new XElement("ModuleStore");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.ModuleStore);
			XmlModuleStoreFilePolicy xmlFilePolicy = new XmlModuleStoreFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj)
		{
			if (!(obj is ModuleStore)) throw new InvalidOperationException("Object must be a ModuleStore.");
			return ReadContentFromObject((ModuleStore) obj);
		}
	}
}