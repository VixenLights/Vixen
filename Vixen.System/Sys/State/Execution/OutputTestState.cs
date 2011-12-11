namespace Vixen.Sys.State.Execution {
	class OutputTestState : State {
		public const string StateName = "OutputTest";

		public OutputTestState(ExecutionStateEngine engine)
			: base(engine) {
		}

		public override string Name {
			get { return StateName; }
		}

		public override void Enter() {
			//*** setup 1:1 patch source
		}

		public override void Leave() {
			//*** restore channel patch sources
		}

		// Cannot go from OutputTest to Open.
		// Has to be done as two steps from the driver: Close, Open

		public override void OnClose() {
			Engine.SetState(Engine.ClosingState);
		}
	}
}
