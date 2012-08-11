using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlChannelNodeSerializer : IXmlSerializer<ChannelNode> {
		private IEnumerable<Channel> _underlyingChannels;

		private const string ELEMENT_NODE = "Node";
		private const string ATTR_ID = "id";
		private const string ATTR_NAME = "name";
		private const string ATTR_CHANNEL_ID = "channelId";

		public XmlChannelNodeSerializer(IEnumerable<Channel> underlyingChannelsForRead) {
			// VixenSystem.Channels has not yet been populated because the file is in the
			// middle of being read/written.  This serializer has a dependency on the
			// newly read channel collection.
			_underlyingChannels = underlyingChannelsForRead;
		}

		public XElement WriteObject(ChannelNode value) {
			object channelElements = null;

			if(value.IsLeaf) {
				// Leaf - reference the single channel by id
				if(value.Channel != null) {
					channelElements = new XAttribute(ATTR_CHANNEL_ID, value.Channel.Id.ToString());
				}
			} else {
				// Branch - include the child nodes inline
				channelElements = value.Children.Select(WriteObject);
			}

			XmlPropertyCollectionSerializer propertyCollectionSerializer = new XmlPropertyCollectionSerializer();
			XElement propertyCollectionElement = propertyCollectionSerializer.WriteObject(value.Properties.Select(x => x.Descriptor.TypeId));
			//XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			//XElement propertyDataElement = dataSetSerializer.WriteObject(value.Properties.PropertyData);
			return new XElement(ELEMENT_NODE,
				new XAttribute(ATTR_NAME, value.Name),
				new XAttribute(ATTR_ID, value.Id),
				propertyCollectionElement,
				//propertyDataElement,
				channelElements);
		}

		public ChannelNode ReadObject(XElement element) {
			string name = XmlHelper.GetAttribute(element, ATTR_NAME);
			if(name == null) return null;

			Guid? id = XmlHelper.GetGuidAttribute(element, ATTR_ID);
			if(id == null) return null;

			// check if we have already loaded the node with this GUID (ie. if it's a node that's referenced twice,
			// in different groups). If we have, return that node instead, and don't go any deeper into child nodes.
			// (we'll just assume that the XML data for this one is identical to the earlier XML node that was written
			// out. To be a bit more proper, we should probably change the WriteXML() to not fully write out repeat
			// ChannelNodes, and instead do some sort of soft reference to the first one (ie. GUID only). )
			ChannelNode existingNode = VixenSystem.Nodes.GetChannelNode(id.Value);
			if(existingNode != null) {
				return existingNode;
			}

			// Children or channel reference
			ChannelNode node = null;
			Guid? channelId = XmlHelper.GetGuidAttribute(element, ATTR_CHANNEL_ID);
			if(channelId == null) {
				// Branch
				IEnumerable<ChannelNode> childNodes = element.Elements(ELEMENT_NODE).Select(ReadObject).NotNull();
				node = new ChannelNode(id.Value, name, null, childNodes);
			} else {
				// Leaf
				Channel channel = _underlyingChannels.FirstOrDefault(x => x.Id == channelId);
				if(channel != null) {
					node = new ChannelNode(id.Value, name, channel, null);
				}
			}

			if(node != null) {
				//// Property data
				//// It's not necessary to load the data before the properties, but it will
				//// save it from creating data for each module and then dumping it.
				//XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
				//node.Properties.PropertyData = dataSetSerializer.ReadObject(element);

				// Properties
				XmlPropertyCollectionSerializer propertyCollectionSerializer = new XmlPropertyCollectionSerializer();
				foreach(Guid propertyTypeId in propertyCollectionSerializer.ReadObject(element)) {
					node.Properties.Add(propertyTypeId);
				}
			}

			return node;
		}
	}
}
