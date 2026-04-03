using System.Xml.Linq;

namespace Vixen.IO.Xml.ElementNodeTemplate
{
	internal class ElementNodeTemplateXElementWriter : IObjectContentWriter<XElement, Sys.ElementNodeTemplate>
	{
		public void WriteContentToObject(XElement content, Sys.ElementNodeTemplate obj)
		{
			XmlElementNodeTemplateFilePolicy xmlFilePolicy = new XmlElementNodeTemplateFilePolicy(obj, content);
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
			if (!(obj is Sys.ElementNodeTemplate)) throw new InvalidOperationException("Object must be a ElementNodeTemplate.");

			WriteContentToObject((XElement) content, (Sys.ElementNodeTemplate) obj);
		}

		int IObjectContentWriter.GetContentVersion(object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");
			return GetContentVersion((XElement) content);
		}
	}
}