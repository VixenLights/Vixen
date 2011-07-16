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
			: this(0, 0, null, null) {
			// Default instance is empty.
		}

		public CommandData(long startTime, long endTime, CommandIdentifier commandIdentifier, object[] parameterValues) {
			StartTime = startTime;
			EndTime = endTime;
			this.CommandIdentifier = commandIdentifier;
			ParameterValues = parameterValues ?? new object[] { };
		}

		public CommandData(long startTime, long endTime, string name, byte platform, byte category, byte commandIndex, object[] parameterValues)
			: this(startTime, endTime, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		public CommandData(string name, byte platform, byte category, byte commandIndex, object[] parameterValues)
			: this(0L, 0L, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		public long StartTime { get; set; }
		public long EndTime { get; set; }

		static public readonly CommandData Empty = new CommandData(0, 0, null, null);

		public bool IsEmpty {
			get { return CommandIdentifier == null; }
		}
	}
}
