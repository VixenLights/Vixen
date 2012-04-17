using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.Effect;

namespace Vixen.Sys {
	/// <summary>
	/// Qualifies a Command with a start time and length.
	/// </summary>
	public class EffectNode : IEffectNode {
		public EffectNode()
			: this(null, TimeSpan.Zero) {
			// Default instance is empty.
		}

		public EffectNode(IEffectModuleInstance effect, TimeSpan startTime) {
			Effect = effect;

			TimeSpan timeSpan = (Effect != null) ? Effect.TimeSpan : TimeSpan.Zero;
			TimeNode = new TimeNode(startTime, timeSpan);

			if(!IsEmpty) {
				// If the effect requires any properties, make sure the target nodes have those
				// properties.
				EffectModuleDescriptorBase effectDescriptor = Modules.GetDescriptorById<EffectModuleDescriptorBase>(effect.Descriptor.TypeId);
				if(!effect.TargetNodes.All(x => x.Properties.Select(y => y.Descriptor.TypeId).Intersect(effectDescriptor.PropertyDependencies).Count() == effectDescriptor.PropertyDependencies.Length)) {

					List<string> message = new List<string>();
					message.Add("The \"" + effectDescriptor.TypeName + "\" effect has property requirements that are missing:");
					message.Add("");
					foreach(ChannelNode channelNode in effect.TargetNodes) {
						Guid[] missingPropertyIds = effectDescriptor.PropertyDependencies.Except(channelNode.Properties.Select(x => x.Descriptor.TypeId)).ToArray();
						if(missingPropertyIds.Length > 0) {
							message.Add((channelNode.Children.Count() > 0 ? "Group " : "Channel ") + channelNode.Name);
							message.AddRange(missingPropertyIds.Select(x => " - Property " + Modules.GetDescriptorById(x).TypeName));
						}
					}
					throw new InvalidOperationException(string.Join(Environment.NewLine, message));
				}
			}
		}

		public IEffectModuleInstance Effect { get; private set; }

		public TimeNode TimeNode { get; private set; }

		public TimeSpan StartTime {
			get { return TimeNode.StartTime; }
			set { TimeNode = new TimeNode(value, TimeSpan); }
		}

		public TimeSpan TimeSpan {
			get { return TimeNode.TimeSpan; }
		}

		public TimeSpan EndTime {
			get { return TimeNode.EndTime; }
		}
		
		public bool IsEmpty {
			get { return Effect == null; }
		}

		static public readonly EffectNode Empty = new EffectNode();

		#region IComparable<IEffectNode>
		public int CompareTo(IEffectNode other) {
			return CompareTo((IDataNode)other);
		}
		#endregion

		#region IComparable<IDataNode>
		public int CompareTo(IDataNode other) {
			return TimeNode.CompareTo(other.TimeNode);
		}
		#endregion
	}

	public interface IEffectNode : IDataNode, IComparable<IEffectNode> {
		IEffectModuleInstance Effect { get; }
		bool IsEmpty { get; }
	}
}
