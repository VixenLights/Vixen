using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Execution;
using Vixen.Sys.State.Execution.Behavior;

namespace Vixen.Sys.State.Execution {
	public class TestOpeningState : State {
		public const string StateName = "Opening for testing";

		public TestOpeningState(ExecutionStateEngine engine)
			: base(engine) {
		}

		public override string Name {
			get { return StateName; }
		}

		public override void Enter() {
			VixenSystem.Logging.Info("Vixen execution engine entering the testing state...");

			//StandardOpeningBehavior<NonExpiringChannelEnumerator>.Run();
			StandardOpeningBehavior.Run();

			Engine.SetState(Engine.TestOpenState);
		}
	}
}
