using System;
using System.Collections.Generic;
using Common.StateMach;
using VixenModules.App.SimpleSchedule.Transition;

namespace VixenModules.App.SimpleSchedule.State {
	class PostExecuteState : IState<IScheduledItemStateObject> {
		private ITransition<IScheduledItemStateObject>[] _transitions;

		public PostExecuteState() {
			_transitions = new ITransition<IScheduledItemStateObject>[] {
				new PostExecuteToCompleted(),
				new PostExecuteToExecuting()
			};
		}

		public string Name {
			get { return "PostExecute"; }
		}

		public IEnumerable<ITransition<IScheduledItemStateObject>> Transitions {
			get { return _transitions; }
		}

		// Not immediately transitioning out of this state.
		// We're going to wait for the next schedule poll so there is a comfortable break between executions.
		public void Entering(IScheduledItemStateObject obj) { }

		public void Leaving(IScheduledItemStateObject obj) { }
	}
}
