using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml.SystemConfig
{
	using Vixen.Sys;

	internal class SystemConfigXElementWriter : IObjectContentWriter<XElement, SystemConfig>
	{
		public void WriteContentToObject(XElement content, SystemConfig obj)
		{
			XmlSystemConfigFilePolicy xmlFilePolicy = new XmlSystemConfigFilePolicy(obj, content);
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
			if (!(obj is SystemConfig)) throw new InvalidOperationException("Object must be a SystemConfig.");

			WriteContentToObject((XElement) content, (SystemConfig) obj);
		}

		int IObjectContentWriter.GetContentVersion(object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");
			return GetContentVersion((XElement) content);
		}
	}
}