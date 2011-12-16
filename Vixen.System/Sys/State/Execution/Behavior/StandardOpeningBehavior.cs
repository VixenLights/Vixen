using System.Collections.Generic;
using System.Linq;
using Vixen.Commands;

namespace Vixen.Sys.State.Execution.Behavior {
	class StandardOpeningBehavior<T>
		where T : IEnumerator<CommandNode[]> {
		// No state needed for this, so going to make it static.
		static public void Run() {
			// Have the outputs draw their data from the channels.
			IOutputSourceCollection channelSources = VixenSystem.Channels.GetSources();
			VixenSystem.Controllers.AddSources(channelSources);

			// Open the channels.
			VixenSystem.Channels.OpenChannels<T>();

			// Enabled/disabled list is going to be an opt-in list of disabled controllers
			// so that new controllers don't need to be added to it in order to be enabled.
			IEnumerable<OutputController> enabledControllers = VixenSystem.SystemConfig.Controllers.Except(VixenSystem.SystemConfig.DisabledControllers);
			VixenSystem.Controllers.OpenControllers(enabledControllers);

			Sys.Execution.Startup();
		}
	}
}
