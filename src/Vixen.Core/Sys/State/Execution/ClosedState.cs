﻿using Vixen.Sys.State.Execution.Behavior;

namespace Vixen.Sys.State.Execution
{
	public class ClosedState : State
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public const string StateName = "Closed";

		public ClosedState(ExecutionStateEngine engine)
			: base(engine)
		{
		}

		public override string Name
		{
			get { return StateName; }
		}

		public override void Enter()
		{
			StandardClosedBehavior.Run();
			Logging.Info("Vixen execution engine entered the closed state.");
		}

		public override void OnOpen()
		{
			Engine.SetState(Engine.OpeningState);
		}

		public override void OnTest()
		{
			Engine.SetState(Engine.TestOpeningState);
		}
	}
}