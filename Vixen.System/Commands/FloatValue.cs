using Vixen.Sys;

namespace Vixen.Commands {
	public class FloatValue : Dispatchable<FloatValue>, ICommand<float> {
		public FloatValue(float value) {
			CommandValue = value;
		}

		public float CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (float)value; }
		}
	}
}
