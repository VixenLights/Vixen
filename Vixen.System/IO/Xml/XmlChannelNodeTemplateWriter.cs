using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.IO.Xml {
	class XmlChannelNodeTemplateWriter : IWriter {
		private const string ELEMENT_NODE = "Node";
		private const string ATTR_NAME = "name";
		private const string ATTR_ID = "id";
		private const string ELEMENT_PROPERTIES = "Properties";
		private const string ELEMENT_PROPERTY = "Property";
		private const string ELEMENT_PROPERTY_DATA = "PropertyData";
		
		//public void Write(string filePath, object value) {
		//    if(!(value is ChannelNodeDefinition)) throw new InvalidOperationException("Attempt to serialize a " + value.GetType().ToString() + " as a ChannelNodeDefinition.");

		//    ChannelNodeDefinition definition = (ChannelNodeDefinition)value;
		//    XElement doc = CreateContent(definition);
		//    doc.Save(filePath);
		//}
		public void Write(string filePath, object value) {
			if(!(value is ChannelNode)) throw new InvalidOperationException("Attempt to serialize a " + value.GetType().ToString() + " as a ChannelNode.");

			ChannelNode node = (ChannelNode)value;
			XElement doc = CreateContent(node);
			doc.Save(filePath);
		}

		public XElement CreateContent(ChannelNode node) {
			object channelElements = null;
			if(!node.IsLeaf) {
				// Branch - include the child nodes inline
				channelElements = node.Children.Select(x => CreateContent(x));
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
