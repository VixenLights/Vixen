using System.Xml.Linq;

namespace Vixen.IO.Xml.SystemContext
{
	internal class SystemContextXElementWriter : IObjectContentWriter<XElement, Sys.SystemContext>
	{
		public void WriteContentToObject(XElement content, Sys.SystemContext obj)
		{
			XmlSystemContextFilePolicy xmlFilePolicy = new XmlSystemContextFilePolicy(obj, content);
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
			if (!(obj is Sys.SystemContext)) throw new InvalidOperationException("Object must be a SystemContext.");

			WriteContentToObject((XElement) content, (Sys.SystemContext) obj);
		}

		int IObjectContentWriter.GetContentVersion(object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");
			return GetContentVersion((XElement) content);
		}
	}
}