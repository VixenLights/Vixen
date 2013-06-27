using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input
{
	internal class ZRotationalAxis : JoystickInput
	{
		public ZRotationalAxis(string name)
			: base(name)
		{
		}

		protected override double _GetValue(JoystickState joystickState)
		{
			return joystickState.RotationZ;
		}
	}
}