namespace Vixen.Sys.State.Execution {
	public class OpenState : State {
		public const string StateName = "Open";

		public OpenState(ExecutionStateEngine engine)
			: base(engine) {
		}

		public override string Name {
			get { return StateName; }
		}

		
		public override void Enter() {
			Vixen.Sys.Execution.SystemTime.Start();
			VixenSystem.Logging.Info("Vixen Execution Engine started.");
		}

		public override void OnClose() {
			Engine.SetState(Engine.ClosingState);
		}
	}
}
