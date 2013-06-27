using System;
using Common.StateMach;

namespace VixenModules.App.SimpleSchedule.Transition
{
	internal class PreExecuteToExecuting : ITransition<IScheduledItemStateObject>
	{
		public Predicate<IScheduledItemStateObject> Condition
		{
			get { return _TransitionCondition; }
		}

		public IState<IScheduledItemStateObject> TargetState
		{
			get { return States.ExecutingState; }
		}

		private bool _TransitionCondition(IScheduledItemStateObject item)
		{
			if (item.Context != null) return true;

			// The context was not available when entering the PreExecute state, so
			// try to get it again in anticipation of the next schedule poll.
			item.RequestContext();
			return false;
		}
	}
}