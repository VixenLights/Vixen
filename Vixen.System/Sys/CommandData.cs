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
	public class CommandData : ITimed {
		public readonly CommandIdentifier CommandIdentifier;
		public readonly object[] ParameterValues;

		public CommandData()
			: this(TimeSpan.Zero, TimeSpan.Zero, null, null) {
			// Default instance is empty.
		}

		public CommandData(TimeSpan startTime, TimeSpan endTime, CommandIdentifier commandIdentifier, object[] parameterValues) {
			StartTime = startTime;
			EndTime = endTime;
			this.CommandIdentifier = commandIdentifier;
			ParameterValues = parameterValues ?? new object[] { };
		}

		public CommandData(TimeSpan startTime, TimeSpan endTime, string name, byte platform, byte category, byte commandIndex, object[] parameterValues)
			: this(startTime, endTime, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		public CommandData(string name, byte platform, byte category, byte commandIndex, object[] parameterValues)
			: this(TimeSpan.Zero, TimeSpan.Zero, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }

		static public readonly CommandData Empty = new CommandData(TimeSpan.Zero, TimeSpan.Zero, null, null);

		public bool IsEmpty {
			get { return CommandIdentifier == null; }
		}
	}
}
