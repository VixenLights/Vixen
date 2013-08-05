using Vixen.Sys.State.Execution.Behavior;

namespace Vixen.Sys.State.Execution
{
	public class OpenState : State
	{
		public const string StateName = "Open";
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public OpenState(ExecutionStateEngine engine)
			: base(engine)
		{
		}

		public override string Name
		{
			get { return StateName; }
		}

		public override void Enter()
		{
			StandardOpenBehavior.Run();
			Logging.Info("Vixen execution engine entered the open state.");
		}

		public override void OnClose()
		{
			Engine.SetState(Engine.ClosingState);
		}
	}
}