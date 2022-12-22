﻿using Vixen.Sys.State.Execution.Behavior;

namespace Vixen.Sys.State.Execution
{
	public class TestOpeningState : State
	{
		public const string StateName = "Opening for testing";
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public TestOpeningState(ExecutionStateEngine engine)
			: base(engine)
		{
		}

		public override string Name
		{
			get { return StateName; }
		}

		public override void Enter()
		{
			Logging.Info("Vixen execution engine entering the testing state...");

			//StandardOpeningBehavior<NonExpiringElementEnumerator>.Run();
			StandardOpeningBehavior.Run();

			Engine.SetState(Engine.TestOpenState);
		}
	}
}