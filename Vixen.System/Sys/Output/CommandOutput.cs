using Vixen.Commands;
using Vixen.Data.Policy;

namespace Vixen.Sys.Output {
	public class CommandOutput : Output {
		private ICommand _command;

		public ICommand Command {
			get { return _command; }
			set {
				if(!Equals(_command, value)) {
					_command = value;
				}
			}
		}

		//*** Not yet any way to set this for an output.
		//    It is intended to allow an output to override the controller's data policy.
		public OutputDataPolicy DataPolicy { get; set; }
	}
}
