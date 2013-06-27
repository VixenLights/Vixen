using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input
{
	internal class YRotationalAxis : JoystickInput
	{
		public YRotationalAxis(string name)
			: base(name)
		{
		}

		protected override double _GetValue(JoystickState joystickState)
		{
			return joystickState.RotationY;
		}
	}
}