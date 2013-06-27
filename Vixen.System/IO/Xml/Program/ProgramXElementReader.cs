using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.Program
{
	using Vixen.Sys;

	internal class ProgramXElementReader : IObjectContentReader<XElement, Program>
	{
		public XElement ReadContentFromObject(Program obj)
		{
			XElement content = new XElement("Program");
			XmlRootAttributeVersion.SetVersion(content, ObjectVersion.Program);
			XmlProgramFilePolicy xmlFilePolicy = new XmlProgramFilePolicy(obj, content);
			xmlFilePolicy.Write();
			return content;
		}

		object IObjectContentReader.ReadContentFromObject(object obj)
		{
			if (!(obj is Program)) throw new InvalidOperationException("Object must be a Program.");
			return ReadContentFromObject((Program) obj);
		}
	}
}