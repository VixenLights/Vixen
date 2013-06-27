using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace Vixen.Module.SequenceType.Surrogate
{
	[DataContract(Namespace = "")]
	internal class EffectNodeSurrogate : NodeSurrogate
	{
		public EffectNodeSurrogate(IEffectNode effectNode)
		{
			StartTime = effectNode.StartTime;
			TypeId = effectNode.Effect.Descriptor.TypeId;
			InstanceId = effectNode.Effect.InstanceId;
			TimeSpan = effectNode.Effect.TimeSpan;
			TargetNodes = effectNode.Effect.TargetNodes.Select(x => new ChannelNodeReferenceSurrogate(x)).ToArray();
		}

		public IEffectNode CreateEffectNode()
		{
			// Create a element node lookup of elements that are currently valid.
			var elementNodes = VixenSystem.Nodes.Distinct().ToDictionary(x => x.Id);

			IEnumerable<Guid> targetNodeIds = TargetNodes.Select(x => x.NodeId);
			IEnumerable<Guid> validElementIds = targetNodeIds.Intersect(elementNodes.Keys);

			IEffectModuleInstance effect = Modules.ModuleManagement.GetEffect(TypeId);
			effect.InstanceId = InstanceId;
			effect.TimeSpan = TimeSpan;
			effect.TargetNodes = validElementIds.Select(x => elementNodes[x]).ToArray();

			return new EffectNode(effect, StartTime);
		}
	}
}