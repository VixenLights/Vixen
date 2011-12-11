using System.Collections.Generic;
using System.Linq;

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
			VixenSystem.Logging.Info("Vixen Execution Engine starting...");

			// Open the channels.
			VixenSystem.Channels.OpenChannels();

			// Enabled/disabled list is going to be an opt-in list of disabled controllers
			// so that new controllers don't need to be added to it in order to be enabled.
			IEnumerable<OutputController> enabledControllers = VixenSystem.SystemConfig.Controllers.Except(VixenSystem.SystemConfig.DisabledControllers);
			VixenSystem.Controllers.OpenControllers(enabledControllers);

			Sys.Execution.Startup();

			Engine.SetState(Engine.OpenState);
		}
	}
}
