using Vixen.Sys;

namespace Vixen.Commands {
	public class DoubleValue : Dispatchable<DoubleValue>, ICommand<double>{
		public DoubleValue(double value) {
			CommandValue = value;
		}

		public double CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (double)value; }
		}
	}
}
