using System;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlElementSerializer : IXmlSerializer<Element>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_ELEMENT = "Channel";
		private const string ATTR_ID = "id";
		private const string ATTR_NAME = "name";

		public XElement WriteObject(Element value)
		{
			XElement element = new XElement(ELEMENT_ELEMENT,
			                                new XAttribute(ATTR_ID, value.Id),
			                                new XAttribute(ATTR_NAME, value.Name));
			return element;
		}

		public Element ReadObject(XElement element)
		{
			try {
				Guid id = XmlHelper.GetGuidAttribute(element, ATTR_ID).GetValueOrDefault();
				string name = XmlHelper.GetAttribute(element, ATTR_NAME);
				return new Element(id, name);
			} catch (Exception e) {
				logging.Error(e, "Error loading Element from XML");
				return null;
			}
		}
	}
}