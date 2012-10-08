using System;
using Common.StateMach;

namespace VixenModules.App.SimpleSchedule.Transition {
	class ExecutingToPostExecute : ITransition<IScheduledItemStateObject> {
		public Predicate<IScheduledItemStateObject> Condition {
			get { return _TransitionCondition; }
		}

		public IState<IScheduledItemStateObject> TargetState {
			get { return States.PostExecuteState; }
		}

		private bool _TransitionCondition(IScheduledItemStateObject item) {
			return !item.Context.IsRunning;
		}
	}
}
