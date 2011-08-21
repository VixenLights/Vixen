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
	public class CommandNode : ITimed {
		private long _startTime;
		private long _timeSpan;

		public CommandNode()
			: this(null, null, 0, 0) {
			// Default instance is empty.
		}

		public CommandNode(Command command, ChannelNode[] targetNodes, long startTime, long timeSpan) {
			Command = command;
			TargetNodes = targetNodes;
			StartTime = startTime;
			TimeSpan = timeSpan;

			if(!IsEmpty) {
				// If the effect requires any properties, make sure the target nodes have those
				// properties.
				EffectModuleDescriptorBase effectDescriptor = Modules.GetDescriptorById<EffectModuleDescriptorBase>(command.EffectId);
				if(!targetNodes.All(x => x.Properties.Select(y => y.Descriptor.TypeId).Intersect(effectDescriptor.PropertyDependencies).Count() == effectDescriptor.PropertyDependencies.Length)) {
					List<string> message = new List<string>();
					message.Add("The \"" + effectDescriptor.TypeName + "\" effect has property requirements that are missing:");
					message.Add("");
					foreach(ChannelNode channelNode in targetNodes) {
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

		public Command Command { get; private set; }

		public ChannelNode[] TargetNodes { get; private set; }

		public long StartTime {
			get { return _startTime; }
			set {
				_startTime = value;
				EndTime = _startTime + _timeSpan;
				// Effect rendering is done from 0, so a start time is irrelevant.
			}
		}

		public long TimeSpan {
			get { return _timeSpan; }
			set {
				_timeSpan = value;
				EndTime = _startTime + _timeSpan;
			}
		}

		public long EndTime { get; private set; }
		
		public bool IsEmpty {
			get { return Command == null; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="renderStartTime">Relative to the start of the CommandNode.</param>
		/// <param name="renderTimeSpan">Length of time to render.</param>
		/// <returns></returns>
		public ChannelData RenderEffectData(long renderStartTime, long renderTimeSpan) {
			if(!IsEmpty) {
				// We're providing the length of the desired effect and a relative start time for rendering.
				return Command.Render(this, TimeSpan, renderStartTime - StartTime, Math.Min(TimeSpan, renderTimeSpan));
			}
			return null;
		}

		static public readonly CommandNode Empty = new CommandNode();

		#region IComparer<CommandNode>
		public class Comparer : IComparer<CommandNode> {
			public int Compare(CommandNode x, CommandNode y) {
				return x.StartTime.CompareTo(y.StartTime);
			}
		}
		#endregion
	}
}
