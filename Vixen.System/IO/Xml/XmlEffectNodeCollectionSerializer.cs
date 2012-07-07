using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlEffectNodeCollectionSerializer : IXmlSerializer<IEnumerable<IEffectNode>> {
		private const string ELEMENT_EFFECT_NODES = "EffectNodes";
		private const string ELEMENT_EFFECT_NODE = "EffectNode";
		private const string ELEMENT_TARGET_NODES = "TargetNodes";
		private const string ELEMENT_TARGET_NODE = "TargetNode";
		private const string ATTR_START_TIME = "startTime";
		private const string ATTR_TIME_SPAN = "timeSpan";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";
		private const string ATTR_ID = "id";

		public XElement WriteObject(IEnumerable<IEffectNode> value) {
			return new XElement(ELEMENT_EFFECT_NODES,
				value.Select(x =>
					new XElement(ELEMENT_EFFECT_NODE,
						new XAttribute(ATTR_TYPE_ID, x.Effect.Descriptor.TypeId),
						new XAttribute(ATTR_INSTANCE_ID, x.Effect.InstanceId),
						new XAttribute(ATTR_START_TIME, x.StartTime.Ticks),
						new XAttribute(ATTR_TIME_SPAN, x.TimeSpan.Ticks),
						new XElement(ELEMENT_TARGET_NODES,
							x.Effect.TargetNodes.Select(y =>
								new XElement(ELEMENT_TARGET_NODE,
									new XAttribute(ATTR_ID, y.Id)))))));
		}

		public IEnumerable<IEffectNode> ReadObject(XElement element) {
			// Create a channel node lookup of channels that are currently valid.
			var channelNodes = VixenSystem.Nodes.Distinct().ToDictionary(x => x.Id);

			List<EffectNode> effectNodes = new List<EffectNode>();

			element = element.Element(ELEMENT_EFFECT_NODES);
			if(element != null) {
				foreach(XElement effectNodeElement in element.Elements(ELEMENT_EFFECT_NODE)) {
					Guid? typeId = XmlHelper.GetGuidAttribute(effectNodeElement, ATTR_TYPE_ID);
					if(typeId == null) continue;

					Guid? instanceId = XmlHelper.GetGuidAttribute(effectNodeElement, ATTR_INSTANCE_ID);
					if(instanceId == null) continue;

					TimeSpan? startTime = Helper.GetXmlTimeValue(effectNodeElement, ATTR_START_TIME);
					if(startTime == null) continue;

					TimeSpan? timeSpan = Helper.GetXmlTimeValue(effectNodeElement, ATTR_TIME_SPAN);
					if(timeSpan == null) continue;

					XElement targetNodesElement = effectNodeElement.Element(ELEMENT_TARGET_NODES);
					if(targetNodesElement == null) continue;

					IEnumerable<Guid> targetNodeIds = targetNodesElement
						.Elements(ELEMENT_TARGET_NODE)
						.Select(x => XmlHelper.GetGuidAttribute(x, ATTR_ID))
						.Where(x => x.HasValue)
						.Select(x => x.Value);

					IEffectModuleInstance effect = Modules.ModuleManagement.GetEffect(typeId);
					IEnumerable<Guid> validChannelIds = targetNodeIds.Intersect(channelNodes.Keys);
					effect.InstanceId = instanceId.Value;
					effect.TimeSpan = timeSpan.Value;
					effect.TargetNodes = validChannelIds.Select(x => channelNodes[x]).ToArray();

					EffectNode effectNode = new EffectNode(effect, startTime.Value);

					effectNodes.Add(effectNode);
				}
			}

			return effectNodes;
		}
	}
}
