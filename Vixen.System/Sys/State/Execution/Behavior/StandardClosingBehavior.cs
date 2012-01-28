using Vixen.Execution;

namespace Vixen.Sys.State.Execution.Behavior {
	class StandardClosingBehavior {
		static public void Run() {
			Sys.Execution.Shutdown();

			// Release all contexts.
			VixenSystem.Contexts.ReleaseContexts();

			// Stop the controllers.
			VixenSystem.Controllers.CloseControllers();

			// Close the channels.
			VixenSystem.Channels.CloseChannels();

			// Remove the sources.
			IOutputSourceCollection channelSources = VixenSystem.Channels.GetSources();
			VixenSystem.Controllers.RemoveSources(channelSources);
		}
	}
}
