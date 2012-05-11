using Vixen.Sys;

namespace Vixen.Commands {
	public class FloatValueCommand : Dispatchable<FloatValueCommand>, ICommand<float> {
		public FloatValueCommand(float value) {
			CommandValue = value;
		}

		public float CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (float)value; }
		}
	}
}
