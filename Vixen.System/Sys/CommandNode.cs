using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Sys {
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

		// These need to be writable to facilitate resyncing data to new intervals.
		public long StartTime {
			get { return _startTime; }
			set {
				_startTime = value;
				EndTime = _startTime + _timeSpan;
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

		static public readonly CommandNode Empty = new CommandNode();

		public class Comparer : IComparer<CommandNode> {
			public int Compare(CommandNode x, CommandNode y) {
				return x.StartTime.CompareTo(y.StartTime);
			}
		}
	}
}
