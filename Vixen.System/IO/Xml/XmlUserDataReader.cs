using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Module;

namespace Vixen.IO.Xml {
	class XmlUserDataReader : XmlReaderBase<UserData> {
		private OutputChannel[] _channels;

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
		private const string ATTR_ID = "id";
		private const string ATTR_NAME = "name";
		private const string ATTR_CHANNEL_ID = "channelId";

		override protected UserData _CreateObject(XElement element, string filePath) {
			UserData userData = new UserData();

			return userData;
		}

		private IModuleDataSet _ReadModuleData(XElement element) {
			XElement moduleDataElement = element.Element(ELEMENT_MODULE_DATA);
			IModuleDataSet moduleData = new ModuleDataSet();

			if(moduleDataElement != null) {
				moduleData.Deserialize(moduleDataElement.ToString());
			}

			return moduleData;
		}

		private OutputChannel[] _ReadChannels(XElement element) {
			XElement parentNode = element.Element(ELEMENT_CHANNELS);
			OutputChannel[] channels = parentNode.Elements().Select(_ReadOutputChannel).ToArray();
			return channels;
		}

		private ChannelNode[] _ReadNodes(XElement element) {
			// Any references to non-existent channels will be pruned by this operation.
			XElement parentNode = element.Element(ELEMENT_NODES);
			ChannelNode[] rootNodes = parentNode.Elements().Select(_ReadChannelNode).ToArray();
			return rootNodes;
		}

		private OutputChannel _ReadOutputChannel(XElement element) {
			Guid id = new Guid(element.Attribute("id").Value);
			string name = element.Attribute("name").Value;
			Patch patch = new Patch(
					element
					.Element("Patch")
					.Elements()
					.Select(_ReadControllerReference)
					);
			return new OutputChannel(id, name, patch);
		}

		private ChannelNode _ReadChannelNode(XElement element) {
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
			if(element.Attribute(ATTR_CHANNEL_ID) == null) {
				// Branch
				node = new ChannelNode(id, name, null, element.Elements(ELEMENT_NODE).Select(_ReadChannelNode));
			} else {
				// Leaf
				Guid channelId = Guid.Parse(element.Attribute(ATTR_CHANNEL_ID).Value);
				OutputChannel channel =_channels.FirstOrDefault(x => x.Id == channelId);
				if(channel != null) {
					node = new ChannelNode(id, name, channel, null);
				}
			}

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

		private ControllerReference _ReadControllerReference(XElement element) {
			return new ControllerReference(
				new Guid(element.Attribute("controllerId").Value),
				int.Parse(element.Attribute("outputIndex").Value)
				);
		}

		protected override void _PopulateObject(UserData obj, XElement element) {
			// Alternate data path handled by VixenSystem.
			IModuleDataSet moduleData = _ReadModuleData(element);
			_channels = _ReadChannels(element);
			ChannelNode[] nodes = _ReadNodes(element);

			obj.ModuleData = moduleData;
			obj.Channels = _channels;
			obj.Nodes = nodes;
		}

		protected override IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			return new Func<XElement, XElement>[] { };
		}
	}
}
