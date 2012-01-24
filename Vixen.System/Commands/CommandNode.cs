using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Commands {
	public class CommandNode : ITimed, IComparable<CommandNode> {
		public CommandNode()
			: this(null, TimeSpan.Zero, TimeSpan.Zero) {
			// Default instance is empty.
		}

		public CommandNode(Command command, TimeSpan startTime, TimeSpan timeSpan) {
			this.Command = command;
			StartTime = startTime;
			TimeSpan = timeSpan;
		}

		public Command Command { get; private set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan TimeSpan { get; set; }

		public TimeSpan EndTime {
			get { return StartTime + TimeSpan; }
		}

		public bool IsEmpty {
			get { return Command == null; }
		}

		public int CompareTo(CommandNode other) {
			return StartTime.CompareTo(other.StartTime);
		}

		static public CommandNode Empty = new CommandNode();
	}
}
