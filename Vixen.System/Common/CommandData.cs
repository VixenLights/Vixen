using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;

namespace Vixen.Common {
	//*** Argument for keeping as a struct?
	//-> Besides the pain in the ass of moving over and the use of thousands of these things.
	/// <summary>
	/// An instance of a command, meaning the command id and the parameter values needed
	/// to fulfill the command.
	/// </summary>
	public struct CommandData {
		public readonly CommandIdentifier CommandIdentifier;
		public readonly object[] ParameterValues;
		public readonly int StartTime;
		public readonly int EndTime;
		public readonly bool IsValid;
		public readonly bool IsRequired;

		public CommandData(int startTime, int endTime, CommandIdentifier commandIdentifier, params object[] parameterValues) {
			StartTime = startTime;
			EndTime = endTime;
			this.CommandIdentifier = commandIdentifier;
			ParameterValues = parameterValues ?? new object[] { };
			IsValid = true;
			IsRequired = false;
		}

		public CommandData(int startTime, int endTime, CommandIdentifier commandIdentifier, bool required, params object[] parameterValues) {
			StartTime = startTime;
			EndTime = endTime;
			this.CommandIdentifier = commandIdentifier;
			ParameterValues = parameterValues ?? new object[] { };
			IsValid = true;
			IsRequired = required;
		}

		public CommandData(CommandIdentifier commandIdentifier, params object[] parameterValues)
			: this(0, 0, commandIdentifier, parameterValues) {
		}

		public CommandData(int startTime, int endTime, string name, byte platform, byte category, byte commandIndex, params object[] parameterValues)
			: this(startTime, endTime, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		public CommandData(string name, byte platform, byte category, byte commandIndex, params object[] parameterValues)
			: this(0, 0, new CommandIdentifier(platform, category, commandIndex), parameterValues) {
		}

		static public readonly CommandData Empty = new CommandData(null, null);

		public bool IsEmpty {
			get { return CommandIdentifier == null; }
		}

		static public bool operator ==(CommandData left, CommandData right) {
			return left.CommandIdentifier == right.CommandIdentifier &&
				left.IsValid == right.IsValid &&
				left.StartTime == right.StartTime &&
				left.EndTime == right.EndTime;
		}

		static public bool operator !=(CommandData left, CommandData right) {
			return !(left == right);
		}

		public override bool Equals(object obj) {
			if(obj is CommandData) {
				return (CommandData)obj == this;
			} else {
				return base.Equals(obj);
			}
		}

		public override int GetHashCode() {
			if(CommandIdentifier != null) {
				return
					(CommandIdentifier.GetHashCode() << 8) |
					this.ParameterValues.GetHashCode();
			}
			return 0;
		}
	}
}
