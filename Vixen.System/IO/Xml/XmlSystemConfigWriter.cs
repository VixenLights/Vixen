using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemConfigWriter : XmlWriterBase<SystemConfig> {
		private const string ELEMENT_ROOT = "SystemConfig";
		private const string ELEMENT_DATA_DIRECTORY = "DataDirectory";
		private const string ELEMENT_CHANNELS = "Channels";
		private const string ELEMENT_NODES = "Nodes";
		private const string ELEMENT_CHANNEL = "Channel";
		private const string ELEMENT_PATCH = "Patch";
		private const string ELEMENT_NODE = "Node";
		private const string ELEMENT_PROPERTIES = "Properties";
		private const string ELEMENT_PROPERTY = "Property";
		private const string ELEMENT_PROPERTY_DATA = "PropertyData";
		private const string ELEMENT_IDENTITY = "Identity";
		private const string ATTR_ID = "id";
		private const string ATTR_NAME = "name";
		private const string ATTR_CHANNEL_ID = "channelId";
		private const string ATTR_IS_CONTEXT = "isContext";

		override protected XElement _CreateContent(SystemConfig obj) {
			return new XElement(ELEMENT_ROOT,
				_WriteContextFlag(obj),
				_WriteIdentity(obj),
				_WriteAlternateDataDirectory(obj),
				_WriteChannels(obj),
				_WriteBranchNodes(obj));
		}

		private XAttribute _WriteContextFlag(SystemConfig obj) {
			if(obj.IsContext) {
				return new XAttribute(ATTR_IS_CONTEXT, true);
			}
			// If not a context, don't include the flag.
			return null;
		}

		private XElement _WriteIdentity(SystemConfig obj) {
			return new XElement(ELEMENT_IDENTITY, obj.Identity);
		}

		private XElement _WriteAlternateDataDirectory(SystemConfig obj) {
			if(!string.IsNullOrWhiteSpace(obj.AlternateDataPath)) {
				return new XElement(ELEMENT_DATA_DIRECTORY, obj.AlternateDataPath);
			}
			return null;
		}

		private XElement _WriteChannels(SystemConfig obj) {
			IEnumerable<XElement> elements = Vixen.Sys.Execution.Channels.Select(_WriteOutputChannel);
			return new XElement(ELEMENT_CHANNELS, elements);
		}

		private XElement _WriteBranchNodes(SystemConfig obj) {
			IEnumerable<XElement> elements = Vixen.Sys.Execution.Nodes.RootNodes.Select(_WriteChannelNode);
			return new XElement(ELEMENT_NODES, elements);
		}

		private XElement _WriteOutputChannel(Channel outputChannel) {
			XElement element = new XElement(ELEMENT_CHANNEL,
				new XAttribute(ATTR_ID, outputChannel.Id),
				new XAttribute(ATTR_NAME, outputChannel.Name),
				new XElement(ELEMENT_PATCH,
					outputChannel.Patch.ControllerReferences.Select(_WriteControllerReference)));
			return element;
		}

		private XElement _WriteChannelNode(ChannelNode channelNode) {
			object channelElements = null;

			if(channelNode.IsLeaf) {
				// Leaf - reference the single channel by id
				if(channelNode.Channel != null) {
					channelElements = new XAttribute(ATTR_CHANNEL_ID, channelNode.Channel.Id.ToString());
				}
			} else {
				// Branch - include the child nodes inline
				channelElements = channelNode.Children.Select(_WriteChannelNode);
			}

			return new XElement(ELEMENT_NODE,
				new XAttribute(ATTR_NAME, channelNode.Name),
				new XAttribute(ATTR_ID, channelNode.Id),
				new XElement(ELEMENT_PROPERTIES, channelNode.Properties.Select(x => new XElement(ELEMENT_PROPERTY, x.Descriptor.TypeId))),
				new XElement(ELEMENT_PROPERTY_DATA, channelNode.Properties.PropertyData.ToXElement()), channelElements);
		}

		private XElement _WriteControllerReference(ControllerReference controllerReference) {
			XElement element = new XElement("ControllerReference",
				new XAttribute("controllerId", controllerReference.ControllerId),
				new XAttribute("outputIndex", controllerReference.OutputIndex));
			return element;
		}
	}
}
