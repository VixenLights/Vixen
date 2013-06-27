using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input
{
	internal class PolarPovBehavior : IPovBehavior
	{
		public double GetValue(JoystickState joystickState, int povIndex)
		{
			// Position is degrees from North.
			return joystickState.GetPointOfViewControllers()[povIndex]/100d;
		}
	}
}