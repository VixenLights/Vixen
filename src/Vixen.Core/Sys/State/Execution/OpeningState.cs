﻿using Vixen.Sys.State.Execution.Behavior;

namespace Vixen.Sys.State.Execution
{
	public class OpeningState : State
	{
		public const string StateName = "Opening";
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public OpeningState(ExecutionStateEngine engine)
			: base(engine)
		{
		}

		public override string Name
		{
			get { return StateName; }
		}

		public override void Enter()
		{
			Logging.Info("Vixen execution engine entering the open state...");

			//StandardOpeningBehavior<SystemElementEnumerator>.Run();
			StandardOpeningBehavior.Run();

			Engine.SetState(Engine.OpenState);
		}
	}
}