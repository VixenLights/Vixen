using System;
using System.Collections.Generic;
using System.Linq;
using Common.StateMach;
using Vixen.Sys;
using VixenModules.App.SimpleSchedule.Service;
using VixenModules.App.SimpleSchedule.State;

namespace VixenModules.App.SimpleSchedule
{
	internal class ScheduleStates : IItemScheduler
	{
		private Machine<IScheduledItemStateObject> _stateMachine;
		private HashSet<IScheduledItemStateObject> _waitingItems;
		private HashSet<IScheduledItemStateObject> _executingItems;

		public ScheduleStates()
		{
			_waitingItems = new HashSet<IScheduledItemStateObject>();
			_executingItems = new HashSet<IScheduledItemStateObject>();

			_stateMachine = new Machine<IScheduledItemStateObject>();

			States.WaitingState = new WaitingState(_waitingItems);
			States.PreExecuteState = new PreExecuteState(_waitingItems, _executingItems);
			States.ExecutingState = new ExecutingState();
			States.PostExecuteState = new PostExecuteState();
			States.CompletedState = new CompletedState(_executingItems);

			_stateMachine.AddState(States.WaitingState);
			_stateMachine.AddState(States.PreExecuteState);
			_stateMachine.AddState(States.ExecutingState);
			_stateMachine.AddState(States.PostExecuteState);
			_stateMachine.AddState(States.CompletedState);
		}

		public void AddSequence(IScheduledItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			IScheduledItemStateObject scheduledSequence = ScheduledItemService.Instance.CreateSequenceStateObject(item);
			_AddItem(scheduledSequence);
		}

		public void AddProgram(IScheduledItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			IScheduledItemStateObject scheduledProgram = ScheduledItemService.Instance.CreateProgramStateObject(item);
			_AddItem(scheduledProgram);
		}

		public void UpdateSequence(IScheduledItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			_UpdateItem(item);
		}

		public void UpdateProgram(IScheduledItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			_UpdateItem(item);
		}

		public void RemoveSequence(IScheduledItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			_RemoveItem(_FindStateObject(item));
		}

		public void RemoveProgram(IScheduledItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			_RemoveItem(_FindStateObject(item));
		}

		public void Update()
		{
			_stateMachine.Update();
		}

		public void TerminateAll()
		{
			foreach (IScheduledItemStateObject item in _executingItems.ToArray()) {
				_stateMachine.SetState(item, States.CompletedState);
				_stateMachine.SetState(item, States.WaitingState);
			}
		}

		public void Refresh(IEnumerable<IScheduledItem> items)
		{
			_TerminateDeletedItems(items);
			_UpdateExecutingItems(items);

			IScheduledItemStateObject[] statedObjects = _stateMachine.GetStatedObjects().ToArray();

			var itemsToAdd = items.Where(x => _FindStateObject(x) == null);
			var itemsToRemove = statedObjects.Where(x => !items.Any(y => y.Id == x.Id));
			var itemsToUpdate = items.Where(x => statedObjects.FirstOrDefault(y => y.Id == x.Id) != null);

			foreach (var item in itemsToAdd) {
				_AddItem(item);
			}

			foreach (var item in itemsToRemove) {
				_RemoveItem(item);
			}

			foreach (var item in itemsToUpdate) {
				_UpdateItem(item);
			}
		}

		private void _TerminateDeletedItems(IEnumerable<IScheduledItem> items)
		{
			IEnumerable<IScheduledItemStateObject> currentItems = items.Select(_FindStateObject).Where(x => x != null);
			IEnumerable<IScheduledItemStateObject> deletedItems = _executingItems.Except(currentItems);
			foreach (IScheduledItemStateObject item in deletedItems) {
				_RemoveItem(item);
			}
		}

		private void _UpdateExecutingItems(IEnumerable<IScheduledItem> items)
		{
			foreach (var item in items) {
				_UpdateItem(item);
			}
		}

		private bool _IsItemAProgram(IScheduledItem item)
		{
			return item.ItemFilePath.EndsWith(Program.Extension);
		}

		private IScheduledItemStateObject _FindStateObject(IScheduledItem item)
		{
			return _stateMachine.GetStatedObjects().FirstOrDefault(x => x.Id.Equals(item.Id));
		}

		private void _AddItem(IScheduledItemStateObject item)
		{
			_stateMachine.AddStatedObject(item, States.WaitingState);
		}

		private void _AddItem(IScheduledItem item)
		{
			if (_IsItemAProgram(item)) {
				AddProgram(item);
			}
			else {
				AddSequence(item);
			}
		}

		private void _UpdateItem(IScheduledItem item)
		{
			Func<IScheduledItem, IScheduledItemStateObject> newStatedObjectCreator;

			if (_IsItemAProgram(item)) {
				newStatedObjectCreator = ScheduledItemService.Instance.CreateProgramStateObject;
			}
			else {
				newStatedObjectCreator = ScheduledItemService.Instance.CreateSequenceStateObject;
			}

			IScheduledItemStateObject scheduledItemStateObject = _FindStateObject(item);
			IScheduledItemStateObject newScheduledItemStateObject = newStatedObjectCreator(item);

			if (scheduledItemStateObject == null || newScheduledItemStateObject == null) return;

			if (ScheduledItemService.Instance.ScheduledItemQualifiesForExecution(newScheduledItemStateObject)) {
				_ReplaceItem(scheduledItemStateObject, newScheduledItemStateObject);
			}
			else {
				_RemoveItem(scheduledItemStateObject);
				_AddItem(newScheduledItemStateObject);
			}
		}

		private void _RemoveItem(IScheduledItemStateObject item)
		{
			if (item != null) {
				_stateMachine.SetState(item, States.CompletedState);
				_stateMachine.RemoveStatedObject(item);
			}
		}

		private void _ReplaceItem(IScheduledItemStateObject oldItem, IScheduledItemStateObject newItem)
		{
			_stateMachine.ReplaceStatedObject(oldItem, newItem);
		}
	}
}