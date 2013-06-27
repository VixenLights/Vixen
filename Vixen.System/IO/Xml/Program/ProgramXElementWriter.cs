using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.Program
{
	using Vixen.Sys;

	internal class ProgramXElementWriter : IObjectContentWriter<XElement, Program>
	{
		public void WriteContentToObject(XElement content, Program obj)
		{
			XmlProgramFilePolicy xmlFilePolicy = new XmlProgramFilePolicy(obj, content);
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
			if (!(obj is Program)) throw new InvalidOperationException("Object must be a Program.");

			WriteContentToObject((XElement) content, (Program) obj);
		}

		int IObjectContentWriter.GetContentVersion(object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");
			return GetContentVersion((XElement) content);
		}
	}
}