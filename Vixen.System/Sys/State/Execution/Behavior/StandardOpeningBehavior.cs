using System.Collections.Generic;
using System.Linq;
using Vixen.Sys.Output;

namespace Vixen.Sys.State.Execution.Behavior {
	//class StandardOpeningBehavior<T>
	//    where T : IEnumerator<CommandNode[]> {
	class StandardOpeningBehavior {
		// No state needed for this, so going to make it static.
		static public void Run() {
			WindowsMultimedia wm = new WindowsMultimedia();
			wm.BeginEnhancedResolution();

			// Have the outputs draw their data from the channels.
			IOutputSourceCollection channelSources = VixenSystem.Channels.GetSources();
			VixenSystem.Controllers.AddSources(channelSources);

			//// Open the channels.
			//VixenSystem.Channels.OpenChannels<T>();

			// Enabled/disabled list is going to be an opt-in list of disabled controllers
			// so that new controllers don't need to be added to it in order to be enabled.
			IEnumerable<IOutputDevice> enabledControllers = VixenSystem.SystemConfig.Controllers.Except(VixenSystem.SystemConfig.DisabledControllers);
			VixenSystem.Controllers.StartAll(enabledControllers);

			Sys.Execution.Startup();
		}
	}
}
