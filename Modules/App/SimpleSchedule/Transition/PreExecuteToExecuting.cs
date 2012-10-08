using System;
using Common.StateMach;

namespace VixenModules.App.SimpleSchedule.Transition {
	class PreExecuteToExecuting : ITransition<IScheduledItemStateObject> {
		public Predicate<IScheduledItemStateObject> Condition {
			get { return _TransitionCondition; }
		}

		public IState<IScheduledItemStateObject> TargetState {
			get { return States.ExecutingState; }
		}

		private bool _TransitionCondition(IScheduledItemStateObject item) {
			return item.Context != null;
		}
	}
}
