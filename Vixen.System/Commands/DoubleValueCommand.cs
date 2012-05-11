using Vixen.Sys;

namespace Vixen.Commands {
	public class DoubleValueCommand : Dispatchable<DoubleValueCommand>, ICommand<double>{
		public DoubleValueCommand(double value) {
			CommandValue = value;
		}

		public double CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (double)value; }
		}
	}
}
