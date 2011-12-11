namespace Vixen.Sys.State.Execution {
	public class ClosingState : State {
		public const string StateName = "Closing";

		public ClosingState(ExecutionStateEngine engine)
			: base(engine) {
		}

		public override string Name {
			get { return StateName; }
		}

		public override void Enter() {
			VixenSystem.Logging.Info("Vixen Execution Engine stopping...");

			Sys.Execution.Shutdown();

			// Close the channels.
			VixenSystem.Channels.CloseChannels();
			// Stop the controllers.
			VixenSystem.Controllers.CloseControllers();

			Engine.SetState(Engine.ClosedState);
		}
	}
}
