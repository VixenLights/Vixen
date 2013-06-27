using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input
{
	internal interface IPovBehavior
	{
		double GetValue(JoystickState joystickState, int povIndex);
	}
}