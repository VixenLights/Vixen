using Vixen.Execution;
using Vixen.Sys.State.Execution.Behavior;

namespace Vixen.Sys.State.Execution {
	public class OpeningState : State {
		public const string StateName = "Opening";

		public OpeningState(ExecutionStateEngine engine)
			: base(engine) {
		}

		public override string Name {
			get { return StateName; }
		}

		public override void Enter() {
			VixenSystem.Logging.Info("Vixen execution engine entering the open state...");

			//StandardOpeningBehavior<SystemChannelEnumerator>.Run();
			StandardOpeningBehavior.Run();

			Engine.SetState(Engine.OpenState);
		}
	}
}
