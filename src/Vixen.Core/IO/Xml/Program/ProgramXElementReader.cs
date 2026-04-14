using System.Xml.Linq;

namespace Vixen.IO.Xml.Program
{
	internal class ProgramXElementReader : IObjectContentReader<XElement, Sys.Program>
	{
		public XElement ReadContentFromObject(Sys.Program obj)
		{
			XElement content = new XElement("Program");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.Program);
			XmlProgramFilePolicy xmlFilePolicy = new XmlProgramFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj)
		{
			if (!(obj is Sys.Program)) throw new InvalidOperationException("Object must be a Program.");
			return ReadContentFromObject((Sys.Program) obj);
		}
	}
}