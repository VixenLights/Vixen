using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlUserDataWriter : XmlWriterBase<UserData> {
		private const string ELEMENT_ROOT = "UserData";
		private const string ELEMENT_MODULE_DATA = "ModuleData";
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

		override protected XElement _CreateContent(UserData userData) {
			return new XElement(ELEMENT_ROOT,
				new XElement(ELEMENT_IDENTITY, userData.Identity),
				_WriteAlternateDataDirectory(userData),
				_WriteModuleData(userData),
				_WriteChannels(userData),
				_WriteBranchNodes(userData));
		}

		private XElement _WriteAlternateDataDirectory(UserData userData) {
			if(!string.IsNullOrWhiteSpace(userData.AlternateDataPath)) {
				return new XElement(ELEMENT_DATA_DIRECTORY, userData.AlternateDataPath);
			}
			return null;
		}

		private XElement _WriteModuleData(UserData userData) {
			return new XElement(ELEMENT_MODULE_DATA, userData.ModuleData.ToXElement());
		}

		private XElement _WriteChannels(UserData userData) {
			IEnumerable<XElement> elements = Vixen.Sys.Execution.Channels.Select(_WriteOutputChannel);
			return new XElement(ELEMENT_CHANNELS, elements);
		}

		private XElement _WriteBranchNodes(UserData userData) {
			IEnumerable<XElement> elements = Vixen.Sys.Execution.Nodes.RootNodes.Select(_WriteChannelNode);
			return new XElement(ELEMENT_NODES, elements);
		}

		private XElement _WriteOutputChannel(OutputChannel outputChannel) {
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
