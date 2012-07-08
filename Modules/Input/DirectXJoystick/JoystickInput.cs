using SlimDX.DirectInput;
using Vixen.Module.Input;

namespace VixenModules.Input.DirectXJoystick {
	abstract class JoystickInput : InputInputBase {
		protected JoystickInput(string name)
			: base(name) {
		}

		virtual public void Update(JoystickState joystickState) {
			double inputValue = _GetValue(joystickState);
			Value = inputValue;
		}

		abstract protected double _GetValue(JoystickState joystickState);
	}
}
