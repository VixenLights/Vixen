using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;

namespace Vixen.Sys {
	/// <summary>
	/// An instance of a low-level command, with the parameter values and time frame needed
	/// for execution by an Output module.
	/// </summary>
	public class Command : ITimed, IComparable<Command> {
		public readonly CommandIdentifier CommandIdentifier;
		public readonly object[] ParameterValues;

		public Command()
			: this(TimeSpan.Zero, TimeSpan.Zero, null, null) {
			// Default instance is empty.
		}

		public Command(TimeSpan startTime, TimeSpan endTime, CommandIdentifier commandIdentifier, object[] parameterValues) {
			StartTime = startTime;
			EndTime = endTime;
			this.CommandIdentifier = commandIdentifier;
			ParameterValues = parameterValues ?? new object[] { };
		}

		public Command(TimeSpan startTime, TimeSpan endTime, string name, byte platform, byte category, byte commandIndex, object[] parameterValues)
			: this(startTime, endTime, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		public Command(string name, byte platform, byte category, byte commandIndex, object[] parameterValues)
			: this(TimeSpan.Zero, TimeSpan.Zero, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }

		static public readonly Command Empty = new Command(TimeSpan.Zero, TimeSpan.Zero, null, null);

		public bool IsEmpty {
			get { return CommandIdentifier == null; }
		}

		static public Command Combine(IEnumerable<Command> commands) {
			int count = commands.Count();
			if(count == 0) return Command.Empty;
			Command firstCommand = commands.First();
			if(count == 1) return firstCommand;

			TimeSpan startTime = firstCommand.StartTime;
			TimeSpan endTime = firstCommand.EndTime;
			// First command wins.
			CommandStandard.CommandIdentifier commandIdentifier = firstCommand.CommandIdentifier;
			object[] parameters = firstCommand.ParameterValues;

			foreach(Command command in commands.Skip(1)) {
				startTime = startTime < command.StartTime ? startTime : command.StartTime;
				endTime = endTime > command.EndTime ? endTime : command.EndTime;
				parameters = CommandStandard.Standard.Combine(commandIdentifier, parameters, command.ParameterValues);
			}

			return new Command(startTime, endTime, commandIdentifier, parameters);
		}

		public int CompareTo(Command other) {
			return this.StartTime.CompareTo(other.StartTime);
		}
	}
}
