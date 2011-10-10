using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Commands {
	public abstract class Command {
		protected Command() { }

		abstract public CommandIdentifier Identifier { get; }

		// Required for transforms.
		abstract public CommandParameterSignature Signature { get; }
		// Required for transforms.
		abstract public object GetParameterValue(int index);
		// Required for transforms.
		abstract public void SetParameterValue(int index, object value);
		abstract public Command Clone();

		virtual public bool CanCombine(Command other) {
			return this.Identifier.Equals(other.Identifier);
		}

		virtual public Command Combine(Command other) {
			return this;
		}

		static public Command Combine(IEnumerable<Command> commands) {
			int count = commands.Count();
			if(count == 0) return null;
			Command firstCommand = commands.First();
			if(count == 1) return firstCommand;

			return commands.Aggregate((command1, command2) => {
					if(command1 != null) {
						return command1.Combine(command2);
					} else if(command2 != null) {
						return command2.Combine(command1);
					}
					// Both are null
					return null;
				});
		}
	}
}
