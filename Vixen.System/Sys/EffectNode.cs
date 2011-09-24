using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Effect;

namespace Vixen.Sys {
	/// <summary>
	/// Qualifies a Command with a start time and length.
	/// </summary>
	public class EffectNode : ITimed {
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

		public ChannelData RenderEffectData() {
			if(!IsEmpty) {
				return Effect.Render();
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="renderStartTime">Relative to the start of the CommandNode.</param>
		/// <param name="renderTimeSpan">Length of time to render.</param>
		/// <returns></returns>
		public ChannelData RenderEffectData(TimeSpan renderStartTime, TimeSpan renderTimeSpan) {
			if(!IsEmpty) {
				// We're providing the length of the desired effect and a relative start time for rendering.
				TimeSpan startTime = (renderStartTime > StartTime && renderStartTime < TimeSpan) ? renderStartTime : StartTime;
				TimeSpan timeSpan = (renderTimeSpan + StartTime < TimeSpan) ? renderTimeSpan : TimeSpan - StartTime;
				return Effect.Render(startTime, timeSpan);
			}
			return null;
		}

		static public readonly EffectNode Empty = new EffectNode();

		#region IComparer<CommandNode>
		public class Comparer : IComparer<EffectNode> {
			public int Compare(EffectNode x, EffectNode y) {
				return x.StartTime.CompareTo(y.StartTime);
			}
		}
		#endregion
	}
}
