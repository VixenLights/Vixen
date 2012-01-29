using System;
using Vixen.Sys;

namespace Vixen.Commands {
	public class CommandNode : IDataNode, IComparable<CommandNode> {
		public CommandNode(Command command, TimeSpan startTime, TimeSpan timeSpan) {
			Command = command;
			StartTime = startTime;
			TimeSpan = timeSpan;
		}

		public Command Command { get; private set; }

		public TimeSpan StartTime { get; private set; }

		public TimeSpan TimeSpan { get; private set; }

		public TimeSpan EndTime {
			get { return StartTime + TimeSpan; }
		}

		public int CompareTo(CommandNode other) {
			return StartTime.CompareTo(other.StartTime);
		}
	}
}
