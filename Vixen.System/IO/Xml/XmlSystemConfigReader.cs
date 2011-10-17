using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Module;

namespace Vixen.IO.Xml {
	class XmlSystemConfigReader : XmlReaderBase<SystemConfig> {
		private Channel[] _channels;

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

		private const string ELEMENT_CONTROLLERS = "Controllers";
		private const string ELEMENT_CONTROLLER = "Controller";
		private const string ELEMENT_OUTPUTS = "Outputs";
		private const string ELEMENT_OUTPUT = "Output";
		private const string ELEMENT_TRANSFORMS = "Transforms";
		private const string ELEMENT_TRANSFORM = "Transform";
		private const string ELEMENT_TRANSFORM_DATA = "TransformData";
		private const string ELEMENT_MODULE_DATA = "ModuleData";
		private const string ATTR_COMB_STRATEGY = "strategy";
		private const string ATTR_LINKED_TO = "linkedTo";
		private const string ATTR_OUTPUT_COUNT = "outputCount";
		private const string ATTR_HARDWARE_ID = "hardwareId";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";

		override protected SystemConfig _CreateObject(XElement element, string filePath) {
			SystemConfig obj = new SystemConfig() { LoadedFilePath = filePath };

			return obj;
		}

		protected override void _PopulateObject(SystemConfig obj, XElement element) {
			bool isContext = _ReadContextFlag(element);
			Guid identity = _ReadIdentity(element);
			_channels = _ReadChannels(element);
			ChannelNode[] nodes = _ReadNodes(element);
			OutputController[] controllers = _ReadControllers(element);

			obj.IsContext = isContext;
			obj.Identity = identity;
			obj.Channels = _channels;
			obj.Nodes = nodes;
			obj.Controllers = controllers;
		}

		protected override IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			if(versionAt < 2 && targetVersion >= 2) yield return _Version_1_to_2;
			if(versionAt < 3 && targetVersion >= 3) yield return _Version_2_to_3;
			if(versionAt < 4 && targetVersion >= 4) yield return _Version_3_to_4;
		}

		private bool _ReadContextFlag(XElement element) {
			// The presence of the flag is enough.  The value is immaterial.
			return element.Attribute(ATTR_IS_CONTEXT) != null;
		}

		private Guid _ReadIdentity(XElement element) {
			element = element.Element(ELEMENT_IDENTITY);
			return Guid.Parse(element.Value);
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

		private OutputController[] _ReadControllers(XElement element) {
			XElement parentNode = element.Element(ELEMENT_CONTROLLERS);
			OutputController[] controllers = parentNode.Elements().Select(_ReadController).ToArray();
			return controllers;
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
				int.Parse(element.Attribute("outputIndex").Value));
		}

		private OutputController _ReadController(XElement element) {
			string name = element.Attribute(ATTR_NAME).Value;
			Guid outputModuleId = new Guid(element.Attribute(ATTR_HARDWARE_ID).Value);
			int outputCount = int.Parse(element.Attribute(ATTR_OUTPUT_COUNT).Value);
			Guid id = Guid.Parse(element.Attribute(ATTR_ID).Value);
			Guid instanceId = Guid.NewGuid();

			OutputController controller = new OutputController(id, instanceId, name, outputCount, outputModuleId);

			_PopulateController(controller, element);

			return controller;
		}

		private void _PopulateController(OutputController controller, XElement element) {
			controller.LinkedTo = Guid.Parse(element.Attribute(ATTR_LINKED_TO).Value);

			controller.ModuleDataSet = _GetModuleData(element);

			int outputIndex = 0;
			foreach(XElement outputElement in element.Element(ELEMENT_OUTPUTS).Elements(ELEMENT_OUTPUT)) {
				// Data persisted in the controller instance may exceed the
				// output count.
				if(outputIndex >= controller.OutputCount) break;

				// The outputs were created when the output count was set.
				OutputController.Output output = controller.Outputs[outputIndex];

				output.Name = outputElement.Attribute(ATTR_NAME).Value;

				ModuleInstanceSpecification<int> transformSpecSet = new ModuleInstanceSpecification<int>();
				IEnumerable<XElement> transformSpecElements = outputElement.Element(ELEMENT_TRANSFORMS).Elements(ELEMENT_TRANSFORM);
				foreach(XElement transformSpecElement in transformSpecElements) {
					Guid typeId = Guid.Parse(transformSpecElement.Attribute(ATTR_TYPE_ID).Value);
					Guid instanceId = Guid.Parse(transformSpecElement.Attribute(ATTR_INSTANCE_ID).Value);
					transformSpecSet.Add(outputIndex, typeId, instanceId);
				}
				controller.OutputTransforms = transformSpecSet;

				outputIndex++;
			}
		}

		private IModuleDataSet _GetModuleData(XElement element) {
			IModuleDataSet moduleDataSet = new ModuleLocalDataSet();

			element = element.Element(ELEMENT_MODULE_DATA);

			if(!element.IsEmpty) {
				string moduleDataString = element.InnerXml();
				moduleDataSet.Deserialize(moduleDataString);
			}

			return moduleDataSet;
		}

		private XElement _Version_1_to_2(XElement element) {
			element.Add(new XElement(ELEMENT_IDENTITY, Guid.NewGuid().ToString()));
			return element;
		}

		private XElement _Version_2_to_3(XElement element) {
			element.Add(new XElement(ELEMENT_CONTROLLERS));
			return element;
		}

		private XElement _Version_3_to_4(XElement element) {
			XElement controllersElement = element.Element(ELEMENT_CONTROLLERS);
			foreach(XElement controllerElement in controllersElement.Elements(ELEMENT_CONTROLLER)) {
				XElement transformDataElement = controllerElement.Element(ELEMENT_TRANSFORM_DATA);
				string content = transformDataElement.InnerXml();
				transformDataElement.Remove();
				XElement moduleDataElement = new XElement(ELEMENT_MODULE_DATA, XElement.Parse(content));
				controllerElement.Add(moduleDataElement);
			}
			return element;
		}
	}
}
