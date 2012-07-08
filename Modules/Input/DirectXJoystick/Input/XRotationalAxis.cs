using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input {
	class XRotationalAxis : JoystickInput {
		public XRotationalAxis(string name)
			: base(name) {
		}

		protected override double _GetValue(JoystickState joystickState) {
			return joystickState.RotationX;
		}
	}
}
