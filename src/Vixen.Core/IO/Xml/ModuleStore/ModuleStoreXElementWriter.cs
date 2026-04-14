using System.Xml.Linq;

namespace Vixen.IO.Xml.ModuleStore
{
	internal class ModuleStoreXElementWriter : IObjectContentWriter<XElement, Sys.ModuleStore>
	{
		public void WriteContentToObject(XElement content, Sys.ModuleStore obj)
		{
			XmlModuleStoreFilePolicy xmlFilePolicy = new XmlModuleStoreFilePolicy(obj, content);
			xmlFilePolicy.Read();
		}

		public int GetContentVersion(XElement content)
		{
			if (content == null) throw new ArgumentNullException("content");

			return XmlRootAttributeVersion.GetVersion(content);
		}

		void IObjectContentWriter.WriteContentToObject(object content, object obj)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");
			if (!(obj is Sys.ModuleStore)) throw new InvalidOperationException("Object must be a ModuleStore.");

			WriteContentToObject((XElement) content, (Sys.ModuleStore) obj);
		}

		int IObjectContentWriter.GetContentVersion(object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");
			return GetContentVersion((XElement) content);
		}
	}
}