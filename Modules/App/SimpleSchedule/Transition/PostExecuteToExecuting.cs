using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.StateMach;
using VixenModules.App.SimpleSchedule.Service;

namespace VixenModules.App.SimpleSchedule.Transition {
	class PostExecuteToExecuting : ITransition<IScheduledItemStateObject> {
		public Predicate<IScheduledItemStateObject> Condition {
			get { return _TransitionCondition; }
		}

		public IState<IScheduledItemStateObject> TargetState {
			get { return States.ExecutingState; }
		}

		private bool _TransitionCondition(IScheduledItemStateObject item) {
			return ScheduledItemService.ScheduledItemQualifiesForExecution(item);
		}
	}
}
