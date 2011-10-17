using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

//TODO: Distribute among individual classes
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

		private const string ELEMENT_CONTROLLERS = "Controllers";
		private const string ELEMENT_CONTROLLER = "Controller";
		private const string ELEMENT_OUTPUTS = "Outputs";
		private const string ELEMENT_OUTPUT = "Output";
		private const string ELEMENT_TRANSFORMS = "Transforms";
		private const string ELEMENT_TRANSFORM = "Transform";
		private const string ELEMENT_MODULE_DATA = "ModuleData";
		private const string ATTR_COMB_STRATEGY = "strategy";
		private const string ATTR_LINKED_TO = "linkedTo";
		private const string ATTR_OUTPUT_COUNT = "outputCount";
		private const string ATTR_HARDWARE_ID = "hardwareId";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";

		override protected XElement _CreateContent(SystemConfig obj) {
			return new XElement(ELEMENT_ROOT,
				_WriteContextFlag(obj),
				_WriteIdentity(obj),
				_WriteAlternateDataDirectory(obj),
				_WriteChannels(obj),
				_WriteBranchNodes(obj),
				_WriteControllers(obj));
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
			IEnumerable<XElement> elements = obj.Channels.Select(_WriteOutputChannel);
			return new XElement(ELEMENT_CHANNELS, elements);
		}

		private XElement _WriteBranchNodes(SystemConfig obj) {
			IEnumerable<XElement> elements = obj.Nodes.Select(_WriteChannelNode);
			return new XElement(ELEMENT_NODES, elements);
		}

		private XElement _WriteControllers(SystemConfig obj) {
			IEnumerable<XElement> elements = obj.Controllers.Select(_WriteController);
			return new XElement(ELEMENT_CONTROLLERS, elements);
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

		private XElement _WriteController(OutputController controller) {
			controller.Commit();

			XElement element = new XElement(ELEMENT_CONTROLLER,
				new XAttribute(ATTR_NAME, controller.Name),
				new XAttribute(ATTR_HARDWARE_ID, controller.OutputModuleId),
				new XAttribute(ATTR_OUTPUT_COUNT, controller.OutputCount),
				new XAttribute(ATTR_ID, controller.Id),
				new XAttribute(ATTR_LINKED_TO, controller.LinkedTo),

				new XElement(ELEMENT_MODULE_DATA, _CreateModuleDataContent(controller)),
				new XElement(ELEMENT_OUTPUTS,
					controller.Outputs.Select((x, index) =>
						new XElement(ELEMENT_OUTPUT,
							new XAttribute(ATTR_NAME, x.Name),
							new XElement(ELEMENT_TRANSFORMS,
								_CreateOutputTransformContent(controller, index))))));

			return element;
		}

		private XElement _CreateModuleDataContent(OutputController controller) {
			if(controller.OutputModule != null) {
				return controller.OutputModule.ModuleDataSet.ToXElement();
			}
			return null;
		}

		private IEnumerable<XElement> _CreateOutputTransformContent(OutputController controller, int outputIndex) {
			if(controller.OutputModule != null) {
				return controller.OutputModule.GetTransforms(outputIndex).Select(x =>
					new XElement(ELEMENT_TRANSFORM,
						new XAttribute(ATTR_TYPE_ID, x.Descriptor.TypeId),
						new XAttribute(ATTR_INSTANCE_ID, x.InstanceId)));

			}
			return null;
		}

		private XElement _WriteControllerReference(ControllerReference controllerReference) {
			XElement element = new XElement("ControllerReference",
				new XAttribute("controllerId", controllerReference.ControllerId),
				new XAttribute("outputIndex", controllerReference.OutputIndex));
			return element;
		}
	}
}
