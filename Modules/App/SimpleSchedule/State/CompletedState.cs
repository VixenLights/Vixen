using System;
using System.Collections.Generic;
using Common.StateMach;

namespace VixenModules.App.SimpleSchedule.State {
	class CompletedState : IState<IScheduledItemStateObject> {
		private ITransition<IScheduledItemStateObject>[] _transitions;
		private IList<IScheduledItemStateObject> _executingCollection;

		public CompletedState(IList<IScheduledItemStateObject> executingCollection) {
			if(executingCollection == null) throw new ArgumentNullException("executingCollection");

			_transitions = new ITransition<IScheduledItemStateObject>[0];
			_executingCollection = executingCollection;
		}

		public string Name {
			get { return "Completed"; }
		}

		public IEnumerable<ITransition<IScheduledItemStateObject>> Transitions {
			get { return _transitions; }
		}

		public void Entering(IScheduledItemStateObject obj) {
			if(_executingCollection.Remove(obj)) {
				obj.ReleaseContext();
				obj.Context = null;
			}
		}

		public void Leaving(IScheduledItemStateObject obj) { }
	}
}
