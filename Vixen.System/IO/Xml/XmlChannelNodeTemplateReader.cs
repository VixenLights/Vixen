using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.IO.Xml {
	class XmlChannelNodeTemplateReader : XmlReaderBase<ChannelNodeDefinition> {
		private const string ELEMENT_NODE = "Node";
		private const string ELEMENT_PROPERTIES = "Properties";
		private const string ELEMENT_PROPERTY = "Property";
		private const string ELEMENT_PROPERTY_DATA = "PropertyData";
		private const string ATTR_NAME = "name";
		private const string ATTR_ID = "id";
		
		override protected ChannelNodeDefinition _CreateObject(XElement element, string filePath) {
			ChannelNodeDefinition template = new ChannelNodeDefinition();
			template.FilePath = filePath;
			return template;
		}

		private ChannelNode _ReadNode(XElement element) {
			string name = element.Attribute(ATTR_NAME).Value;
			Guid id = Guid.Parse(element.Attribute(ATTR_ID).Value);

			// check if we have already loaded the node with this GUID (ie. if it's a node that's referenced twice,
			// in different groups). If we have, return that node instead, and don't go any deeper into child nodes.
			// (we'll just assume that the XML data for this one is identical to the earlier XML node that was written
			// out. To be a bit more proper, we should probably change the WriteXML() to not fully write out repeat
			// ChannelNodes, and instead do some sort of soft reference to the first one (ie. GUID only). )
			ChannelNode existingNode = ChannelNode.GetChannelNode(id);
			if(existingNode != null) {
				return existingNode;
			}

			// Children or channel reference
			ChannelNode node = null;
			// Branch
			IEnumerable<ChannelNode> children = element.Elements(ELEMENT_NODE).Select(_ReadNode);
			node = new ChannelNode(id, name, null, children);

			if(node != null) {
				// Property data
				// It's not necessary to load the data before the properties, but it will
				// save it from creating data for each module and then dumping it.
				string moduleDataString = element.Element(ELEMENT_PROPERTY_DATA).InnerXml();
				node.Properties.PropertyData.Deserialize(moduleDataString);

				// Properties
				foreach(XElement propertyElement in element.Element(ELEMENT_PROPERTIES).Elements(ELEMENT_PROPERTY)) {
					node.Properties.Add(new Guid(propertyElement.Value));
				}
			}

			return node;
		}

		protected override void _PopulateObject(ChannelNodeDefinition obj, XElement element) {
		}

		protected override IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			return new Func<XElement, XElement>[] { };
		}
	}
}
