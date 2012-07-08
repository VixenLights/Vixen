using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input {
	class XAxis : JoystickInput {
		public XAxis(string name)
			: base(name) {
		}

		protected override double _GetValue(JoystickState joystickState) {
			return joystickState.X;
		}
	}
}
