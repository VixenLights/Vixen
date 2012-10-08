using System.Collections.Generic;
using Common.StateMach;
using VixenModules.App.SimpleSchedule.State;
using VixenModules.App.SimpleSchedule.StateObject;

namespace VixenModules.App.SimpleSchedule {
	class ScheduleStates {
		private Machine<IScheduledItemStateObject> _stateMachine;
		private List<IScheduledItemStateObject> _waitingItems;
		private List<IScheduledItemStateObject> _executingItems;

		public ScheduleStates() {
			_waitingItems = new List<IScheduledItemStateObject>();
			_executingItems = new List<IScheduledItemStateObject>();

			_stateMachine = new Machine<IScheduledItemStateObject>();

			States.WaitingState = new WaitingState(_waitingItems);
			States.PreExecuteState = new PreExecuteState(_stateMachine, _waitingItems, _executingItems);
			States.ExecutingState = new ExecutingState();
			States.PostExecuteState = new PostExecuteState();
			States.CompletedState = new CompletedState(_executingItems);

			_stateMachine.AddState(States.WaitingState);
			_stateMachine.AddState(States.PreExecuteState);
			_stateMachine.AddState(States.ExecutingState);
			_stateMachine.AddState(States.PostExecuteState);
			_stateMachine.AddState(States.CompletedState);
		}

		public void AddSequence(IScheduledItem item) {
			ScheduledSequence scheduledSequence = new ScheduledSequence(item);
			if(scheduledSequence.ItemIsValid) {
				_stateMachine.AddStatedObject(scheduledSequence, States.WaitingState);
			}
		}

		public void AddProgram(IScheduledItem item) {
			ScheduledProgram scheduledProgram = new ScheduledProgram(item);
			if(scheduledProgram.ItemIsValid) {
				_stateMachine.AddStatedObject(scheduledProgram, States.WaitingState);
			}
		}

		public void Update() {
			_stateMachine.Update();
		}
	}
}
