using System.Collections.Generic;
using System.Linq;
using Vixen.Sys.Output;

namespace Vixen.Sys.State.Execution.Behavior {
	class StandardOpeningBehavior {
		// No state needed for this, so going to make it static.
		static public void Run() {
			WindowsMultimedia wm = new WindowsMultimedia();
			wm.BeginEnhancedResolution();

			// Enabled/disabled list is going to be an opt-in list of disabled devices
			// so that new devices don't need to be added to it in order to be enabled.
			IEnumerable<IOutputDevice> enabledDevices = VixenSystem.OutputDeviceManagement.Devices.Except(VixenSystem.SystemConfig.DisabledDevices);
			VixenSystem.OutputDeviceManagement.StartOnly(enabledDevices);

			Sys.Execution.Startup();
		}
	}
}
