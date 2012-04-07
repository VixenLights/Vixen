namespace Vixen.Sys.State.Execution.Behavior {
	class StandardClosingBehavior {
		static public void Run() {
			Sys.Execution.Shutdown();

			WindowsMultimedia wm = new WindowsMultimedia();
			wm.EndEnhancedResolution();

			// Release all contexts.
			VixenSystem.Contexts.ReleaseContexts();

			// Stop the controllers.
			VixenSystem.Controllers.StopAll();

			// Stop the previews.
			VixenSystem.Previews.StopAll();

			//// Close the channels.
			//VixenSystem.Channels.CloseChannels();

			// Remove the sources.
			IOutputSourceCollection channelSources = VixenSystem.Channels.GetSources();
			VixenSystem.Controllers.RemoveSources(channelSources);
		}
	}
}
