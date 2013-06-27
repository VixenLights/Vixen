using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.SystemContext
{
	using Vixen.Sys;

	internal class SystemContextXElementWriter : IObjectContentWriter<XElement, SystemContext>
	{
		public void WriteContentToObject(XElement content, SystemContext obj)
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
			if (!(obj is SystemContext)) throw new InvalidOperationException("Object must be a SystemContext.");

			WriteContentToObject((XElement) content, (SystemContext) obj);
		}

		int IObjectContentWriter.GetContentVersion(object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");
			return GetContentVersion((XElement) content);
		}
	}
}