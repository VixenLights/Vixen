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
		private Channel[] _channels;

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
		private const string ATTR_IS_CONTEXT = "isContext";

		override protected UserData _CreateObject(XElement element, string filePath) {
			UserData userData = new UserData() { FilePath = filePath };

			return userData;
		}

		protected override void _PopulateObject(UserData obj, XElement element) {
			bool isContext = _ReadContextFlag(element);
			Guid identity = _ReadIdentity(element);
			// Alternate data path handled by VixenSystem.
			ModuleStaticDataSet moduleData = _ReadModuleData(element);
			_channels = _ReadChannels(element);
			ChannelNode[] nodes = _ReadNodes(element);

			obj.IsContext = isContext;
			obj.Identity = identity;
			obj.ModuleData = moduleData;
			obj.Channels = _channels;
			obj.Nodes = nodes;
		}

		protected override IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			if(versionAt < 2 && targetVersion >= 2) yield return _Version_1_to_2;
		}

		private bool _ReadContextFlag(XElement element) {
			// The presence of the flag is enough.  The value is immaterial.
			return element.Attribute(ATTR_IS_CONTEXT) != null;
		}

		private Guid _ReadIdentity(XElement element) {
			element = element.Element(ELEMENT_IDENTITY);
			return Guid.Parse(element.Value);
		}

		private ModuleStaticDataSet _ReadModuleData(XElement element) {
			XElement moduleDataElement = element.Element(ELEMENT_MODULE_DATA);
			ModuleStaticDataSet moduleData = new ModuleStaticDataSet();

			if(moduleDataElement != null) {
				moduleData.Deserialize(moduleDataElement.ToString());
			}

			return moduleData;
		}

		private Channel[] _ReadChannels(XElement element) {
			XElement parentNode = element.Element(ELEMENT_CHANNELS);
			Channel[] channels = parentNode.Elements().Select(_ReadOutputChannel).ToArray();
			return channels;
		}

		private ChannelNode[] _ReadNodes(XElement element) {
			// Any references to non-existent channels will be pruned by this operation.
			XElement parentNode = element.Element(ELEMENT_NODES);
			ChannelNode[] rootNodes = parentNode.Elements().Select(_ReadChannelNode).ToArray();
			return rootNodes;
		}

		private Channel _ReadOutputChannel(XElement element) {
			Guid id = new Guid(element.Attribute(ATTR_ID).Value);
			string name = element.Attribute(ATTR_NAME).Value;
			Patch patch = new Patch(
					element
					.Element(ELEMENT_PATCH)
					.Elements()
					.Select(_ReadControllerReference)
					);
			return new Channel(id, name, patch);
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
				Channel channel =_channels.FirstOrDefault(x => x.Id == channelId);
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

		private XElement _Version_1_to_2(XElement element) {
			element.Add(new XElement(ELEMENT_IDENTITY, Guid.NewGuid().ToString()));
			return element;
		}
	}
}
