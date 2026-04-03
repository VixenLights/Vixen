using System.Xml.Linq;

namespace Vixen.IO.Xml.ElementNodeTemplate
{
	internal class ElementNodeTemplateXElementReader : IObjectContentReader<XElement, Sys.ElementNodeTemplate>
	{
		public XElement ReadContentFromObject(Sys.ElementNodeTemplate obj)
		{
			XElement content = new XElement("ElementNodeTemplate");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.ElementNodeTemplate);
			XmlElementNodeTemplateFilePolicy xmlFilePolicy = new XmlElementNodeTemplateFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj)
		{
			if (!(obj is Sys.ElementNodeTemplate)) throw new InvalidOperationException("Object must be a ElementNodeTemplate.");
			return ReadContentFromObject((Sys.ElementNodeTemplate) obj);
		}
	}
}