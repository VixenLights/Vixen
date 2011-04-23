using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;

namespace Vixen.Common {
	/// <summary>
	/// An instance of a command, meaning the command id and the parameter values needed
	/// to fulfill the command.
	/// </summary>
	public class CommandData : ITimed {
		public readonly CommandIdentifier CommandIdentifier;
		public readonly object[] ParameterValues;

		public CommandData()
			: this(null, null) {
			// Default instance is empty.
		}

		public CommandData(long startTime, long endTime, CommandIdentifier commandIdentifier, params object[] parameterValues) {
			StartTime = startTime;
			EndTime = endTime;
			this.CommandIdentifier = commandIdentifier;
			ParameterValues = parameterValues ?? new object[] { };
		}

		public CommandData(CommandIdentifier commandIdentifier, params object[] parameterValues)
			: this(0L, 0L, commandIdentifier, parameterValues) {
		}

		public CommandData(long startTime, long endTime, string name, byte platform, byte category, byte commandIndex, params object[] parameterValues)
			: this(startTime, endTime, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		public CommandData(string name, byte platform, byte category, byte commandIndex, params object[] parameterValues)
			: this(0L, 0L, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		public long StartTime { get; private set; }
		public long EndTime { get; private set; }

		static public readonly CommandData Empty = new CommandData(null, null);

		public bool IsEmpty {
			get { return CommandIdentifier == null; }
		}
	}
}
