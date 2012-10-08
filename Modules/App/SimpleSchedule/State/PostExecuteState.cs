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

		public void Entering(IScheduledItemStateObject obj) { }

		public void Leaving(IScheduledItemStateObject obj) { }
	}
}
