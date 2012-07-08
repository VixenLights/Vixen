using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input {
	class YAxis : JoystickInput {
		public YAxis(string name)
			: base(name) {
		}

		protected override double _GetValue(JoystickState joystickState) {
			return joystickState.Y;
		}
	}
}
