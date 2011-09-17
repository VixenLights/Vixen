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
	public class Command : ITimed {
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
	}
}
