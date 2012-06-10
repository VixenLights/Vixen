using System.Drawing;
using Vixen.Sys;

namespace Vixen.Commands {
	public class LightingValueCommand : Dispatchable<LightingValueCommand>, ICommand<LightingValue>  {
		public LightingValueCommand(LightingValue value) {
			CommandValue = value;
		}

		public LightingValue CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (LightingValue)value; }
		}

		public Color GetIntensityAffectedValue() {
			return CommandValue.GetIntensityAffectedColor();
		}
	}
}
