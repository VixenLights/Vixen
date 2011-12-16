using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.State.Execution.Behavior;

namespace Vixen.Sys.State.Execution {
	public class TestOpenState : State {
		public const string StateName = "Open for testing";

		public TestOpenState(ExecutionStateEngine engine)
			: base(engine) {
		}

		public override string Name {
			get { return StateName; }
		}

		public override void Enter() {
			// Could have transitioned to Open, but the user needs to know that it's in
			// the testing state and not the standard execution state.
			StandardOpenBehavior.Run();
			VixenSystem.Logging.Info("Vixen execution engine entered the testing state.");
		}

		public override void OnClose() {
			Engine.SetState(Engine.ClosingState);
		}
	}
}
