using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module.PreFilter;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlPreFilterNodeCollectionSerializer : IXmlSerializer<IEnumerable<IPreFilterNode>> {
		private const string ELEMENT_FILTER_NODES = "FilterNodes";
		private const string ELEMENT_FILTER_NODE = "FilterNode";
		private const string ELEMENT_TARGET_NODES = "TargetNodes";
		private const string ELEMENT_TARGET_NODE = "TargetNode";
		private const string ATTR_START_TIME = "startTime";
		private const string ATTR_TIME_SPAN = "timeSpan";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";
		private const string ATTR_ID = "id";

		public XElement WriteObject(IEnumerable<IPreFilterNode> value) {
			return new XElement(ELEMENT_FILTER_NODES,
				value.Select(x =>
					new XElement(ELEMENT_FILTER_NODE,
						new XAttribute(ATTR_TYPE_ID, x.PreFilter.Descriptor.TypeId),
						new XAttribute(ATTR_INSTANCE_ID, x.PreFilter.InstanceId),
						new XAttribute(ATTR_START_TIME, x.StartTime.Ticks),
						new XAttribute(ATTR_TIME_SPAN, x.TimeSpan.Ticks),
						new XElement(ELEMENT_TARGET_NODES,
							x.PreFilter.TargetNodes.Select(y =>
								new XElement(ELEMENT_TARGET_NODE,
									new XAttribute(ATTR_ID, y.Id)))))));
		}

		public IEnumerable<IPreFilterNode> ReadObject(XElement element) {
			// Create a channel node lookup of channels that are currently valid.
			var channelNodes = VixenSystem.Nodes.Distinct().ToDictionary(x => x.Id);

			List<PreFilterNode> preFilterNodes = new List<PreFilterNode>();

			//*** call a method that reads a single node
			element = element.Element(ELEMENT_FILTER_NODES);
			if(element != null) {
				foreach(XElement filterNodeElement in element.Elements(ELEMENT_FILTER_NODE)) {
					Guid? typeId = XmlHelper.GetGuidAttribute(filterNodeElement, ATTR_TYPE_ID);
					if(typeId == null) continue;

					Guid? instanceId = XmlHelper.GetGuidAttribute(filterNodeElement, ATTR_INSTANCE_ID);
					if(instanceId == null) continue;

					TimeSpan? startTime = Helper.GetXmlTimeValue(filterNodeElement, ATTR_START_TIME);
					if(startTime == null) continue;

					TimeSpan? timeSpan = Helper.GetXmlTimeValue(filterNodeElement, ATTR_TIME_SPAN);
					if(timeSpan == null) continue;

					XElement targetNodesElement = filterNodeElement.Element(ELEMENT_TARGET_NODES);
					if(targetNodesElement == null) continue;

					IEnumerable<Guid> targetNodeIds = targetNodesElement
						.Elements(ELEMENT_TARGET_NODE)
						.Select(x => XmlHelper.GetGuidAttribute(x, ATTR_ID))
						.Where(x => x.HasValue)
						.Select(x => x.Value);

					IPreFilterModuleInstance filter = Modules.ModuleManagement.GetPreFilter(typeId);
					IEnumerable<Guid> validChannelIds = targetNodeIds.Intersect(channelNodes.Keys);
					filter.InstanceId = instanceId.Value;
					filter.TargetNodes = validChannelIds.Select(x => channelNodes[x]).ToArray();
					filter.TimeSpan = timeSpan.Value;
					PreFilterNode preFilterNode = new PreFilterNode(filter, startTime.Value);

					preFilterNodes.Add(preFilterNode);
				}
			}

			return preFilterNodes;
		}
	}
}
