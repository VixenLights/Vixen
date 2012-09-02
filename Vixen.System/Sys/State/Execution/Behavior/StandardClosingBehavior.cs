namespace Vixen.Sys.State.Execution.Behavior {
	class StandardClosingBehavior {
		static public void Run() {
			Sys.Execution.Shutdown();

			WindowsMultimedia wm = new WindowsMultimedia();
			wm.EndEnhancedResolution();

			// Release all contexts.
			VixenSystem.Contexts.ReleaseContexts();

			// Stop all output devices.
			VixenSystem.OutputDeviceManagement.StopAll();
		}
	}
}
