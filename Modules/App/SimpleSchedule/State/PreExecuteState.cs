using System;
using System.Collections.Generic;
using Common.StateMach;
using VixenModules.App.SimpleSchedule.Transition;

namespace VixenModules.App.SimpleSchedule.State {
	class PreExecuteState : IState<IScheduledItemStateObject> {
		private ITransition<IScheduledItemStateObject>[] _transitions;
		private Machine<IScheduledItemStateObject> _stateMachine;
		private HashSet<IScheduledItemStateObject> _waitingCollection;
		private HashSet<IScheduledItemStateObject> _executingCollection;

		public PreExecuteState(Machine<IScheduledItemStateObject> stateMachine, HashSet<IScheduledItemStateObject> waitingCollection, HashSet<IScheduledItemStateObject> executingCollection) {
			if(stateMachine == null) throw new ArgumentNullException("stateMachine");
			if(waitingCollection == null) throw new ArgumentNullException("waitingCollection");
			if(executingCollection == null) throw new ArgumentNullException("executingCollection");

			_stateMachine = stateMachine;
			_waitingCollection = waitingCollection;
			_executingCollection = executingCollection;
			_transitions = new ITransition<IScheduledItemStateObject>[] {
				new PreExecuteToExecuting()
			};
		}

		public string Name {
			get { return "PreExecute"; }
		}

		public IEnumerable<ITransition<IScheduledItemStateObject>> Transitions {
			get { return _transitions; }
		}

		public void Entering(IScheduledItemStateObject obj) {
			obj.RequestContext();
			if(obj.Context != null) {
				_MoveObjectFromWaitingToExecuting(obj);
				// We need to immediately transition to the Executing state to get this thing running.
				_stateMachine.Update();
			}
		}

		public void Leaving(IScheduledItemStateObject obj) {
			if(obj.Context != null) {
				_MoveObjectFromWaitingToExecuting(obj);
			}
		}

		private void _MoveObjectFromWaitingToExecuting(IScheduledItemStateObject obj) {
			_waitingCollection.Remove(obj);
			_executingCollection.Add(obj);
		}
	}
}
