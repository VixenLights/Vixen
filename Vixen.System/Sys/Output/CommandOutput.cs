using System;
using Vixen.Commands;

namespace Vixen.Sys.Output {
	public class CommandOutput : Output {
		private ICommand _command;

		internal CommandOutput(Guid id, string name)
			: base(id, name) {
		}

		public ICommand Command {
			get { return _command; }
			set {
				if(!Equals(_command, value)) {
					_command = value;
				}
			}
		}
	}
}
