using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module.SequenceFilter;
using Vixen.Sys;

namespace Vixen.Module.SequenceType.Surrogate {
	[DataContract(Namespace = "")]
	class FilterNodeSurrogate : NodeSurrogate {
		public FilterNodeSurrogate(ISequenceFilterNode filterNode) {
			StartTime = filterNode.StartTime;
			TypeId = filterNode.Filter.Descriptor.TypeId;
			InstanceId = filterNode.Filter.InstanceId;
			TimeSpan = filterNode.Filter.TimeSpan;
			TargetNodes = filterNode.Filter.TargetNodes.Select(x => new ChannelNodeReferenceSurrogate(x)).ToArray();
		}

		public ISequenceFilterNode CreateFilterNode() {
			var elementNodes = VixenSystem.Nodes.Distinct().ToDictionary(x => x.Id);

			IEnumerable<Guid> targetNodeIds = TargetNodes.Select(x => x.NodeId);
			IEnumerable<Guid> validElementIds = targetNodeIds.Intersect(elementNodes.Keys);

			ISequenceFilterModuleInstance filter = Modules.ModuleManagement.GetSequenceFilter(TypeId);
			filter.InstanceId = InstanceId;
			filter.TimeSpan = TimeSpan;
			filter.TargetNodes = validElementIds.Select(x => elementNodes[x]).ToArray();

			return new SequenceFilterNode(filter, StartTime);
		}
	}
}
