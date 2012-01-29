using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.Effect;

namespace Vixen.Sys {
	/// <summary>
	/// Qualifies a Command with a start time and length.
	/// </summary>
	public class EffectNode : IDataNode, IComparable<EffectNode> {
		public EffectNode()
			: this(null, TimeSpan.Zero) {
			// Default instance is empty.
		}

		public EffectNode(IEffectModuleInstance effect, TimeSpan startTime) {
			Effect = effect;
			StartTime = startTime;

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

		public TimeSpan StartTime { get; set; }

		public TimeSpan TimeSpan {
			get { return (Effect != null) ? Effect.TimeSpan : TimeSpan.Zero; }
		}

		public TimeSpan EndTime { 
			get { return (Effect != null) ? StartTime + TimeSpan : StartTime; }
		}
		
		public bool IsEmpty {
			get { return Effect == null; }
		}

		public EffectIntents RenderEffectData() {
			return !IsEmpty ? Effect.Render() : null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="desiredStartTime">Relative to the start of the EffectNode.</param>
		/// <param name="desiredDuration">Duration of effect to render.</param>
		/// <returns></returns>
		public EffectIntents RenderEffectData(TimeSpan desiredStartTime, TimeSpan desiredDuration) {
			if(!IsEmpty) {
				// We're providing the length of the desired effect and a relative start time for rendering.
				TimeSpan renderStartTime = (desiredStartTime < TimeSpan) ? desiredStartTime : TimeSpan.Zero;
				TimeSpan renderTimeSpan = (renderStartTime + desiredDuration < TimeSpan) ? desiredDuration : TimeSpan - renderStartTime;
				return Effect.Render(renderStartTime, renderTimeSpan);
			}
			return null;
		}

		static public readonly EffectNode Empty = new EffectNode();

		#region IComparer<EffectNode>
		public class Comparer : IComparer<EffectNode> {
			public int Compare(EffectNode x, EffectNode y) {
				return x.StartTime.CompareTo(y.StartTime);
			}
		}
		#endregion

		#region IComparable<EffectNode>
		public int CompareTo(EffectNode other) {
			return StartTime.CompareTo(other.StartTime);
		}
		#endregion
	}
}
