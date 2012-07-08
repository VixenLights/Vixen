using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input {
	class ZAxis : JoystickInput {
		public ZAxis(string name)
			: base(name) {
		}

		protected override double _GetValue(JoystickState joystickState) {
			return joystickState.Z;
		}
	}
}
