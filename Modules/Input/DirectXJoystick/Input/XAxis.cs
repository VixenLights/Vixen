using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input
{
	internal class XAxis : JoystickInput
	{
		public XAxis(string name)
			: base(name)
		{
		}

		protected override double _GetValue(JoystickState joystickState)
		{
			return joystickState.X;
		}
	}
}