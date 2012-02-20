using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlChannelNodeCollectionSerializer : IXmlSerializer<IEnumerable<ChannelNode>> {
		private const string ELEMENT_NODES = "Nodes";

		public XElement WriteObject(IEnumerable<ChannelNode> value) {
			XmlChannelNodeSerializer channelNodeSerializer = new XmlChannelNodeSerializer();
			IEnumerable<XElement> elements = value.Select(channelNodeSerializer.WriteObject);
			return new XElement(ELEMENT_NODES, elements);
		}

		public IEnumerable<ChannelNode> ReadObject(XElement element) {
			// Any references to non-existent channels will be pruned by this operation.
			List<ChannelNode> channelNodes = new List<ChannelNode>();

			XElement parentNode = element.Element(ELEMENT_NODES);
			if(parentNode != null) {
				XmlChannelNodeSerializer channelNodeSerializer = new XmlChannelNodeSerializer();
				IEnumerable<ChannelNode> childNodes = parentNode.Elements().Select(channelNodeSerializer.ReadObject).NotNull();
				channelNodes.AddRange(childNodes);
			}

			return channelNodes;
		}

		//private XElement _WriteChannelNode(ChannelNode channelNode) {
		//    object channelElements = null;

		//    if(channelNode.IsLeaf) {
		//        // Leaf - reference the single channel by id
		//        if(channelNode.Channel != null) {
		//            channelElements = new XAttribute(ATTR_CHANNEL_ID, channelNode.Channel.Id.ToString());
		//        }
		//    } else {
		//        // Branch - include the child nodes inline
		//        channelElements = channelNode.Children.Select(_WriteChannelNode);
		//    }

		//    XmlPropertyCollectionSerializer propertyCollectionSerializer = new XmlPropertyCollectionSerializer();
		//    XElement propertyCollectionElement = propertyCollectionSerializer.WriteObject(channelNode.Properties.Select(x => x.Descriptor.TypeId));
		//    XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
		//    XElement propertyDataElement = dataSetSerializer.WriteObject(channelNode.Properties.PropertyData);
		//    return new XElement(ELEMENT_NODE,
		//        new XAttribute(ATTR_NAME, channelNode.Name),
		//        new XAttribute(ATTR_ID, channelNode.Id),
		//        propertyCollectionElement,
		//        propertyDataElement,
		//        //new XElement(ELEMENT_PROPERTIES, channelNode.Properties.Select(x => new XElement(ELEMENT_PROPERTY, x.Descriptor.TypeId))),
		//        //new XElement(ELEMENT_PROPERTY_DATA, channelNode.Properties.PropertyData.ToXElement()), 
		//        channelElements);
		//}

		//private ChannelNode _ReadChannelNode(XElement element) {
		//    string name = XmlHelper.GetAttribute(element, ATTR_NAME);
		//    if(name == null) return null;

		//    Guid? id = XmlHelper.GetGuidAttribute(element, ATTR_ID);
		//    if(id == null) return null;

		//    // check if we have already loaded the node with this GUID (ie. if it's a node that's referenced twice,
		//    // in different groups). If we have, return that node instead, and don't go any deeper into child nodes.
		//    // (we'll just assume that the XML data for this one is identical to the earlier XML node that was written
		//    // out. To be a bit more proper, we should probably change the WriteXML() to not fully write out repeat
		//    // ChannelNodes, and instead do some sort of soft reference to the first one (ie. GUID only). )
		//    ChannelNode existingNode = VixenSystem.Nodes.GetChannelNode(id.Value);
		//    if(existingNode != null) {
		//        return existingNode;
		//    }

		//    // Children or channel reference
		//    ChannelNode node = null;
		//    Guid? channelId = XmlHelper.GetGuidAttribute(element, ATTR_CHANNEL_ID);
		//    if(channelId == null) {
		//        // Branch
		//        IEnumerable<ChannelNode> childNodes = _ReadValidChannelNodes(element.Elements(ELEMENT_NODE));
		//        node = new ChannelNode(id.Value, name, null, childNodes);
		//    } else {
		//        // Leaf
		//        Channel channel = VixenSystem.Channels.FirstOrDefault(x => x.Id == channelId);
		//        if(channel != null) {
		//            node = new ChannelNode(id.Value, name, channel, null);
		//        }
		//    }

		//    if(node != null) {
		//        // Property data
		//        // It's not necessary to load the data before the properties, but it will
		//        // save it from creating data for each module and then dumping it.
		//        XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
		//        node.Properties.PropertyData = dataSetSerializer.ReadObject(element);
		//        //string moduleDataString = element.Element(ELEMENT_PROPERTY_DATA).InnerXml();
		//        //node.Properties.PropertyData.Deserialize(moduleDataString);

		//        // Properties
		//        XmlPropertyCollectionSerializer propertyCollectionSerializer = new XmlPropertyCollectionSerializer();
		//        foreach(Guid propertyTypeId in propertyCollectionSerializer.ReadObject(element)) {
		//            node.Properties.Add(propertyTypeId);
		//        }
		//        //foreach(XElement propertyElement in element.Element(ELEMENT_PROPERTIES).Elements(ELEMENT_PROPERTY)) {
		//        //    node.Properties.Add(new Guid(propertyElement.Value));
		//        //}
		//    }

		//    return node;
		//}

		//private IEnumerable<ChannelNode> _ReadValidChannelNodes(IEnumerable<XElement> elements) {
		//    return elements.Select(_ReadChannelNode).NotNull();
		//}
	}
}
