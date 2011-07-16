using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
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
			this.Command = command;
			TargetNodes = targetNodes;
			StartTime = startTime;
			TimeSpan = timeSpan;
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
