using System;
using System.Collections.Generic;
using System.Linq;
using Common.StateMach;
using VixenModules.App.SimpleSchedule.Service;
using VixenModules.App.SimpleSchedule.State;

namespace VixenModules.App.SimpleSchedule {
	class ScheduleStates : IItemScheduler {
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
			if(item == null) throw new ArgumentNullException("item");

			IScheduledItemStateObject scheduledSequence = ScheduledItemService.Instance.CreateSequenceStateObject(item);
			_AddItem(scheduledSequence);
		}

		public void AddProgram(IScheduledItem item) {
			if(item == null) throw new ArgumentNullException("item");

			IScheduledItemStateObject scheduledProgram = ScheduledItemService.Instance.CreateProgramStateObject(item);
			_AddItem(scheduledProgram);
		}

		public void UpdateSequence(IScheduledItem item) {
			if(item == null) throw new ArgumentNullException("item");

			_UpdateItem(item, ScheduledItemService.Instance.CreateSequenceStateObject);
		}

		public void UpdateProgram(IScheduledItem item) {
			if(item == null) throw new ArgumentNullException("item");

			_UpdateItem(item, ScheduledItemService.Instance.CreateSequenceStateObject);
		}

		public void RemoveSequence(IScheduledItem item) {
			if(item == null) throw new ArgumentNullException("item");

			_RemoveItem(_FindStateObject(item));
		}

		public void RemoveProgram(IScheduledItem item) {
			if(item == null) throw new ArgumentNullException("item");

			_RemoveItem(_FindStateObject(item));
		}

		public void Update() {
			_stateMachine.Update();
		}

		private IScheduledItemStateObject _FindStateObject(IScheduledItem item) {
			return _stateMachine.GetStatedObjects().FirstOrDefault(x => x.Id.Equals(item.Id));
		}

		private void _AddItem(IScheduledItemStateObject item) {
			if(item.ItemIsValid) {
				_stateMachine.AddStatedObject(item, States.WaitingState);
			}
		}

		private void _UpdateItem(IScheduledItem item, Func<IScheduledItem, IScheduledItemStateObject> newStatedObjectCreator) {
			IScheduledItemStateObject scheduledItemStateObject = _FindStateObject(item);
			IScheduledItemStateObject newScheduledItemStateObject = newStatedObjectCreator(item);

			if(scheduledItemStateObject == null || newScheduledItemStateObject == null) return;

			if(ScheduledItemService.Instance.ScheduledItemQualifiesForExecution(newScheduledItemStateObject)) {
				_ReplaceItem(scheduledItemStateObject, newScheduledItemStateObject);
			} else {
				_RemoveItem(scheduledItemStateObject);
				_AddItem(newScheduledItemStateObject);
			}
		}

		private void _RemoveItem(IScheduledItemStateObject item) {
			if(item != null) {
				_stateMachine.SetState(item, States.CompletedState);
				_stateMachine.RemoveStatedObject(item);
			}
		}

		private void _ReplaceItem(IScheduledItemStateObject oldItem, IScheduledItemStateObject newItem) {
			_stateMachine.ReplaceStatedObject(oldItem, newItem);
		}
	}
}
