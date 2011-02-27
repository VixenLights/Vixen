using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class CommandNode {
		private int _startTime;
		private int _timeSpan;

		public CommandNode(Command command, OutputChannel[] targetChannels, int startTime, int timeSpan) {
			this.Command = command;
			TargetChannels = targetChannels;
			StartTime = startTime;
			TimeSpan = timeSpan;
		}

		public Command Command { get; private set; }
		
		public OutputChannel[] TargetChannels { get; private set; }

		// These need to be writable to facilitate resyncing data to new intervals.
		public int StartTime {
			get { return _startTime; }
			set {
				_startTime = value;
				EndTime = _startTime + _timeSpan;
			}
		}
		public int TimeSpan {
			get { return _timeSpan; }
			set {
				_timeSpan = value;
				EndTime = _startTime + _timeSpan;
			}
		}

		public int EndTime { get; private set; }
		
		public bool IsRequired;

		public class Comparer : IComparer<CommandNode> {
			public int Compare(CommandNode x, CommandNode y) {
				return x.StartTime.CompareTo(y.StartTime);
			}
		}
	}
}
