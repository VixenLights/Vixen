using System;
using Vixen.Commands;

namespace Vixen.Sys.Output {
	public class CommandOutput : Output {
		private ICommand _command;

		internal CommandOutput(Guid id, string name, int index)
			: base(id, name, index) {
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
