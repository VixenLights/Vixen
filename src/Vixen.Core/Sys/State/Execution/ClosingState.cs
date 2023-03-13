using Vixen.Sys.State.Execution.Behavior;

namespace Vixen.Sys.State.Execution
{
	public class ClosingState : State
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public const string StateName = "Closing";

		public ClosingState(ExecutionStateEngine engine)
			: base(engine)
		{
		}

		public override string Name
		{
			get { return StateName; }
		}

		public override void Enter()
		{
			Logging.Info("Vixen execution engine entering the closing state...");

			StandardClosingBehavior.Run();

			Engine.SetState(Engine.ClosedState);
		}
	}
}