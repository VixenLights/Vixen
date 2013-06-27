using SlimDX.DirectInput;
using Vixen.Module.Input;

namespace VixenModules.Input.DirectXJoystick
{
	internal abstract class JoystickInput : InputInputBase
	{
		protected JoystickInput(string name)
			: base(name)
		{
		}

		public virtual void Update(JoystickState joystickState)
		{
			double inputValue = _GetValue(joystickState);
			Value = inputValue;
		}

		protected abstract double _GetValue(JoystickState joystickState);
	}
}