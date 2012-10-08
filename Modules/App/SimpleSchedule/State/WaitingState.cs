using System;
using System.Collections.Generic;
using Common.StateMach;
using VixenModules.App.SimpleSchedule.Transition;

namespace VixenModules.App.SimpleSchedule.State {
	class WaitingState : IState<IScheduledItemStateObject> {
		private ITransition<IScheduledItemStateObject>[] _transitions;
		private IList<IScheduledItemStateObject> _waitingCollection;

		public WaitingState(IList<IScheduledItemStateObject> waitingCollection) {
			if(waitingCollection == null) throw new ArgumentNullException("waitingCollection");

			_waitingCollection = waitingCollection;
			_transitions = new ITransition<IScheduledItemStateObject>[] {
				new WaitingToPreExecute()
			};
		}

		public string Name {
			get { return "Waiting"; }
		}

		public IEnumerable<ITransition<IScheduledItemStateObject>> Transitions {
			get { return _transitions; }
		}

		public void Entering(IScheduledItemStateObject obj) {
			if(!_waitingCollection.Contains(obj)) {
				_waitingCollection.Add(obj);
			}
		}

		public void Leaving(IScheduledItemStateObject obj) { }
	}
}
