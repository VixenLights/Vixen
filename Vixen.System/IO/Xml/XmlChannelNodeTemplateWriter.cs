using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlChannelNodeTemplateWriter : XmlWriterBase<ChannelNode> {
		private const string ELEMENT_NODE = "Node";
		private const string ELEMENT_PROPERTIES = "Properties";
		private const string ELEMENT_PROPERTY = "Property";
		private const string ELEMENT_PROPERTY_DATA = "PropertyData";
		private const string ATTR_NAME = "name";
		private const string ATTR_ID = "id";
		
		override protected XElement _CreateContent(ChannelNode node) {
			object channelElements = null;
			if(!node.IsLeaf) {
				// Branch - include the child nodes inline
				channelElements = node.Children.Select(x => _CreateContent(x));
			}
			return new XElement(ELEMENT_NODE,
				new XAttribute(ATTR_NAME, node.Name),
				new XAttribute(ATTR_ID, node.Id),
				new XElement(ELEMENT_PROPERTIES, node.Properties.Select(x => 
					new XElement(ELEMENT_PROPERTY, x.Descriptor.TypeId))),
				new XElement(ELEMENT_PROPERTY_DATA, node.Properties.PropertyData.ToXElement()), 
				channelElements);
		}
	}
}
