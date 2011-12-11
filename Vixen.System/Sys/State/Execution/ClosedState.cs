namespace Vixen.Sys.State.Execution {
	public class ClosedState : State {
		public const string StateName = "Closed";

		public ClosedState(ExecutionStateEngine engine)
			: base(engine) {
		}

		public override string Name {
			get { return StateName; }
		}


		public override void Enter() {
			Vixen.Sys.Execution.SystemTime.Stop();
			VixenSystem.Logging.Info("Vixen Execution Engine stopped.");
		}


		public override void OnOpen() {
			Engine.SetState(Engine.OpeningState);
		}

		public override void OnOutputTest() {
			Engine.SetState(Engine.OutputTestState);
		}
	}
}
