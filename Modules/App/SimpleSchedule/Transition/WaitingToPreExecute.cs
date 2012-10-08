using System;
using Common.StateMach;
using VixenModules.App.SimpleSchedule.Service;

namespace VixenModules.App.SimpleSchedule.Transition {
	class WaitingToPreExecute : ITransition<IScheduledItemStateObject> {
		public Predicate<IScheduledItemStateObject> Condition {
			get { return _TransitionCondition; }
		}

		public IState<IScheduledItemStateObject> TargetState {
			get { return States.PreExecuteState; }
		}

		private bool _TransitionCondition(IScheduledItemStateObject item) {
			return ScheduledItemService.ScheduledItemQualifiesForExecution(item);
		}
	}
}
