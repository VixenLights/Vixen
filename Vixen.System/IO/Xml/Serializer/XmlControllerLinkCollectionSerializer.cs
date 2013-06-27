using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlControllerLinkCollectionSerializer : IXmlSerializer<IEnumerable<ControllerLink>>
	{
		private const string ELEMENT_CONTROLLER_LINKS = "ControllerLinks";
		private const string ELEMENT_CONTROLLER_LINK = "ControllerLink";
		private const string ATTR_ID = "id";
		private const string ATTR_PRIOR_ID = "priorId";
		private const string ATTR_NEXT_ID = "nextId";

		public XElement WriteObject(IEnumerable<ControllerLink> value)
		{
			IEnumerable<XElement> elements = value.Select(_WriteControllerLink);
			return new XElement(ELEMENT_CONTROLLER_LINKS, elements);
		}

		public IEnumerable<ControllerLink> ReadObject(XElement element)
		{
			List<ControllerLink> controllerLinks = new List<ControllerLink>();

			XElement parentNode = element.Element(ELEMENT_CONTROLLER_LINKS);
			if (parentNode != null) {
				controllerLinks.AddRange(parentNode.Elements().Select(_ReadControllerLink).Where(x => x != null));
			}

			return controllerLinks;
		}

		private XElement _WriteControllerLink(ControllerLink controllerLink)
		{
			XElement element = new XElement(ELEMENT_CONTROLLER_LINK,
			                                new XAttribute(ATTR_ID, controllerLink.ControllerId),
			                                new XAttribute(ATTR_PRIOR_ID, controllerLink.PriorId ?? Guid.Empty),
			                                new XAttribute(ATTR_NEXT_ID, controllerLink.NextId ?? Guid.Empty));
			return element;
		}

		private ControllerLink _ReadControllerLink(XElement element)
		{
			Guid? controllerId = XmlHelper.GetGuidAttribute(element, ATTR_ID);
			if (controllerId == null) return null;

			Guid? priorId = XmlHelper.GetGuidAttribute(element, ATTR_PRIOR_ID);
			Guid? nextId = XmlHelper.GetGuidAttribute(element, ATTR_NEXT_ID);

			if (priorId == Guid.Empty) priorId = null;
			if (nextId == Guid.Empty) nextId = null;

			return new ControllerLink(controllerId.Value) {PriorId = priorId, NextId = nextId};
		}
	}
}