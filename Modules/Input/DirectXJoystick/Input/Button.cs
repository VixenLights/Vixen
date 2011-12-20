using SlimDX.DirectInput;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.Input.DirectXJoystick.Input {
	class Button : JoystickInput {
		public Button(string name, int index)
			: base(name) {
			ButtonIndex = index;
		}

		public int ButtonIndex { get; private set; }

		protected override double _GetValue(JoystickState joystickState) {
			return joystickState.GetButtons()[ButtonIndex] ? 
				Position.MaxValue :
				Position.MinValue;
		}
	}
}
