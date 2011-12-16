using Vixen.Sys.State.Execution.Behavior;

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
			VixenSystem.Logging.Info("Vixen execution engine entering the closing state...");

			StandardClosingBehavior.Run();

			Engine.SetState(Engine.ClosedState);
		}
	}
}
