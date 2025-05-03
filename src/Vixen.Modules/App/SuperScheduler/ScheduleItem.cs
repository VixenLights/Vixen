﻿using Common.Broadcast;
using System.Runtime.Serialization;

using VixenModules.App.Shows;
using VixenModules.App.WebServer.Model;
using Action = VixenModules.App.Shows.Action;

namespace VixenModules.App.SuperScheduler
{
	public enum StateType
	{
		Waiting,
		Startup,
		Running,
		Shutdown,
		Paused,
		Changed
	}

	[DataContract]
	public class ScheduleItem: IDisposable
	{
		public ScheduleItem()
		{
			_actionLock = new object();
		}
		#region Data Members

		[DataMember]
		public Guid ShowID { get; set; }

		[DataMember]
		public bool Monday = true;
		[DataMember]
		public bool Tuesday = true;
		[DataMember]
		public bool Wednesday = true;
		[DataMember]
		public bool Thursday = true;
		[DataMember]
		public bool Friday = true;
		[DataMember]
		public bool Saturday = true;
		[DataMember]
		public bool Sunday = true;
		[DataMember]
		public bool Enabled = true;

		#endregion // Data Members

		#region Variables

		private Object _actionLock;
		Shows.ShowItem _currentItem = null;
		CancellationTokenSource tokenSourcePreProcess;
		//CancellationToken tokenPreProcess;
		CancellationTokenSource tokenSourcePreProcessAll;
		//CancellationToken tokenPreProcessAll;
		// Specifies the number of loops of a Show to run, regardless of the time window.
		// -1 specifies repetition managed by the time window. 
		private int _runLoops = -1;

		#endregion // Variables

		#region Properties

		private List<Shows.Action> runningActions = null;
		public List<Shows.Action> RunningActions => runningActions ?? (runningActions = new List<Shows.Action>());

		private StateType _state = StateType.Waiting;
		public StateType State
		{
			get { return _state; }
			set { _state = value; }
		}

		private Shows.Show _show = null;
		public Shows.Show Show {
			get 
			{
				if (_show == null)
					_show = Shows.ShowsData.GetShow(ShowID);
				return _show;
			}
		}

		private DateTime _startDate;
		[DataMember]
		public DateTime StartDate
		{
			get
			{
				return _startDate;
			}
			set
			{
				_startDate = new DateTime(value.Year, value.Month, value.Day);
			}
		}

		private DateTime _endDate;
		[DataMember]
		public DateTime EndDate
		{
			get
			{
				return _endDate;
			}
			set
			{
				_endDate = new DateTime(value.Year, value.Month, value.Day);
			}
		}

		private DateTime _startTime;
		[DataMember]
		public DateTime StartTime
		{
			get
			{
				DateTime result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
										 _startTime.Hour, _startTime.Minute, _startTime.Second);
				return result;
			}
			set
			{
				_startTime = value;
			}
		}

		private DateTime _endTime;
		[DataMember]
		public DateTime EndTime
		{
			get
			{
				DateTime result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
										 _endTime.Hour, _endTime.Minute, _endTime.Second);

				if (result < StartTime)
				{
					result = result.AddDays(1);
				}
				return result;
			}
			set { _endTime = value; }
		}

		private bool DaySupported(System.DayOfWeek dayOfWeek)
		{
			bool daySupported = false;

			switch (dayOfWeek)
			{
				case System.DayOfWeek.Sunday:
					daySupported = Sunday;
					break;
				case System.DayOfWeek.Monday:
					daySupported = Monday;
					break;
				case System.DayOfWeek.Tuesday:
					daySupported = Tuesday;
					break;
				case System.DayOfWeek.Wednesday:
					daySupported = Wednesday;
					break;
				case System.DayOfWeek.Thursday:
					daySupported = Thursday;
					break;
				case System.DayOfWeek.Friday:
					daySupported = Friday;
					break;
				case System.DayOfWeek.Saturday:
					daySupported = Saturday;
					break;
			}

			return daySupported;
		}

		/// <summary>
		/// Returns true if the item is schedulable in the future.
		/// </summary>
		/// <param name="nextDateTime">Date and time it should run next</param>
		/// <returns>true if the item is schedulable in the future</returns>
		public bool IsSchedulableInFuture(out DateTime nextDateTime)
		{
			// Default to the item NOT running in the future
			bool isSchedulableInFuture = false;

			// Default the next time to run to the current date/time
			nextDateTime = DateTime.Now;

			// Save off the current day of week
			DayOfWeek initialDayOfWeek = nextDateTime.DayOfWeek;

			// Add days until we find one supported
			while (!DaySupported(nextDateTime.DayOfWeek))
			{
				// Advance one day
				nextDateTime = nextDateTime.AddDays(1);

				// If we have looped around to the day we started on then...
				if (nextDateTime.DayOfWeek == initialDayOfWeek)
				{
					// Indicate the item is not scheduable
					return false;
				}
			}

			// Make sure the item is still within the schedule window
			if (_startDate < nextDateTime && _endDate > nextDateTime)
			{
				// Indicate the item is scheduable in the future
				isSchedulableInFuture = true;	
			}

			// Return whether the item is schedulable in the future
			return isSchedulableInFuture;
		}

		/// <summary>
		/// Returns the DateTime the item should run next or null if it shouldn't run anymore.
		/// </summary>		
		public DateTime? NextStartDateTime
		{
			get
			{			
				// Default the next time to run to the current date / time
				DateTime dateTime = DateTime.Now;

				// Check to see if this item is still scheduable
				if (!IsSchedulableInFuture(out dateTime))
				{
					// Otherwise return null
					return null;
				}
				
				// Verify that the start time is in the future
				if (_startTime.TimeOfDay > dateTime.TimeOfDay)
				{ 															
					return new DateTime(
						dateTime.Year, 
						dateTime.Month, 
						dateTime.Day,
						_startTime.Hour, 
						_startTime.Minute, 
						_startTime.Second);					
				}												
				else
				{
					// Otherwise return null
					// I don't think the code should ever get here because the show should already be running if the current
					// time is within the item's window.  Returning null is for defensive purposes.
					return null;
				}
			}
		}

		private DateTime InProcessEndTime { get; set; }

		private Queue<Shows.ShowItem> itemQueue;
		public Queue<Shows.ShowItem> ItemQueue
		{
			get
			{
				if (itemQueue == null)
					itemQueue = new Queue<Shows.ShowItem>();
				return itemQueue;
			}
			set
			{
				itemQueue = value;
			}
		}

		private List<Shows.Action> backgroundActions;
		public List<Shows.Action> BackgroundActions
		{
			get
			{
				if (backgroundActions == null)
					backgroundActions = new List<Shows.Action>();
				return backgroundActions;
			}
			set
			{
				backgroundActions = value;
			}
		}

		#endregion //Properties

		#region Utilities

		private int CompareTodaysDate(DateTime date) 
		{
			DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
			return now.CompareTo(date);
		}

		#endregion

		#region Show Control

		public void ProcessSchedule()
		{
			if (Show != null)
			{
				switch (State)
				{
					case StateType.Waiting:
						CheckForStartup();
						break;
					//case StateType.Changed:
					//	Restart();
					//	break;
					case StateType.Paused:
						//Pause();
						break;
					case StateType.Startup:
						//ProcessStartup();
						break;
					case StateType.Shutdown:
						//ProcessShutdown();
						//Shutdown();
						break;
					//default:
					//	Process();
					//	break;
				}
			}
		}

		public void CheckForStartup()
		{
			if (
				Enabled
				&& CompareTodaysDate(StartDate) >= 0
				&& CompareTodaysDate(EndDate) <= 0
				&& DateTime.Now.CompareTo(StartTime) >= 0
				&& DateTime.Now.CompareTo(EndTime) <= 0
				&& (
					   (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && Saturday)
					|| (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && Sunday)
					|| (DateTime.Now.DayOfWeek == DayOfWeek.Monday && Monday)
					|| (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday && Tuesday)
					|| (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && Wednesday)
					|| (DateTime.Now.DayOfWeek == DayOfWeek.Thursday && Thursday)
					|| (DateTime.Now.DayOfWeek == DayOfWeek.Friday && Friday)
				   )
			   )
			{
				Start(false);
			}
		}

		private bool CheckForShutdown()
		{
			if (State == StateType.Shutdown) return true;
			
			// If there runloops remaining...
			if (_runLoops > 0)
			{
				// and there are still actions remaining to run...
				if (ItemQueue.Count == 0)
				{
					// Decrement the run loop counter
					_runLoops--;

					// See if we're done running through the loops 
					if (_runLoops == 0)
					{
						// And if so, then stop the Show
						ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Show stopping after manual start completed");
						var presentation = new Presentation
						{
							Name = Show.Name,
							Info = Show.ID.ToString()
						};
						Broadcast.Publish<Presentation>("StopShow", presentation);

						return true;
					}
				}
			}

			// Else check the date/time window
			else if (InProcessEndTime.CompareTo(DateTime.Now) <= 0)
			{
				return true;
			}

			return false;
		}

		public void Stop(bool graceful)
		{
			if (tokenSourcePreProcess != null && tokenSourcePreProcess.Token.CanBeCanceled)
				tokenSourcePreProcess.Cancel(false);
			if (tokenSourcePreProcessAll != null && tokenSourcePreProcessAll.Token.CanBeCanceled)
				tokenSourcePreProcessAll.Cancel(false);

			if (graceful) {
				State = StateType.Shutdown;
				ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Show stopping gracefully");
			} else {
				State = StateType.Waiting;

				ItemQueue.Clear();

				Action[] actions;
				lock (_actionLock)
				{
					actions = RunningActions.ToArray();
				}
				
				foreach (var action in actions)
				{
					ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Stopping action: " + action.ShowItem.Name);
					action.Stop();
				}
				
				if (Show != null)
				{
					Show.ReleaseAllActions();
					ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Show stopped immediately");
				}
				else
				{
					ScheduleExecutor.AddSchedulerLogEntry("No show selected", "Nothing to stop.");
				}
				
			}
		}

		public void Shutdown()
		{
			ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Shutdown Requested");
			State = StateType.Shutdown;
		}

		public void Start(bool manuallyStarted)
		{
			State = StateType.Running;

			InProcessEndTime = EndTime;

			// If we're manually starting then we'll just do one run-through
			if (manuallyStarted)
			{
				_runLoops = 1;
			}

			ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Show started");

			// Do this in a task so we don't stop Vixen while pre-processing!
			if (tokenSourcePreProcessAll == null || tokenSourcePreProcessAll.IsCancellationRequested)
				tokenSourcePreProcessAll = new CancellationTokenSource();

			var preProcessTask = new Task(a => PreProcessActionTask(),tokenSourcePreProcessAll.Token);

			preProcessTask.ContinueWith(task => BeginStartup(), tokenSourcePreProcessAll.Token);

			preProcessTask.Start();

		}

		private void PreProcessActionTask()
		{
			// Pre-Process all the actions to fill up our memory

			//Show.GetItems(Shows.ShowItemType.All).AsParallel().WithCancellation(tokenSourcePreProcessAll.Token).ForAll(
			//	item => {
				foreach (ShowItem item in Show.GetItems(ShowItemType.All))
				{
					ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Pre-processing: " + item.Name);
					var action = item.GetAction();

					if (!action.PreProcessingCompleted) {
						action.PreProcess(tokenSourcePreProcessAll);
					}
					//if (tokenSourcePreProcessAll != null && tokenSourcePreProcessAll.IsCancellationRequested)
					//	return;
					//};	

				}

		}

		private void ExecuteAction(Shows.Action action)
		{
			LogScheduleInfoEntry(string.Format("ExecuteAction: {0} with state of {1}", action.ShowItem.Name, State));
			
			if (State != StateType.Waiting)
			{
				if (!action.PreProcessingCompleted)
				{
					ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Pre-processing action: " + action.ShowItem.Name);

					// Do this in a task so we don't stop Vixen while pre-processing!
					tokenSourcePreProcess = new CancellationTokenSource();
					Task preProcessTask = new Task(() => action.PreProcess(), tokenSourcePreProcess.Token);
					preProcessTask.ContinueWith(task =>
						{
							ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Starting action: " + action.ShowItem.Name);
							action.Execute();
							lock (_actionLock)
							{
								RunningActions.Add(action);
							}
						}
					);

					preProcessTask.Start();
				}
				else
				{
					ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Starting action: " + action.ShowItem.Name);
					action.Execute();
					lock (_actionLock)
					{
						RunningActions.Add(action);
					}
				}
			}
		}

		public void Pause()
		{
			State = StateType.Paused;
		}

		public void Resume()
		{
			State = StateType.Running;

			if (ItemQueue.Where(x => x.ItemType == ShowItemType.Startup).Count() > 0)
			{
				ExecuteNextStartupItem();
			}

			else if (ItemQueue.Where(x => x.ItemType == ShowItemType.Shutdown).Count() > 0)
			{
				BeginShutdown();
			}

			else
			{
				if (ItemQueue.Where(x => x.ItemType == ShowItemType.Sequential).Count() > 0)
				{
					ExecuteNextSequentialItem();
				}

				if (ItemQueue.Where(x => x.ItemType == ShowItemType.Background).Count() > 0)
				{
					BeginBackground();
				}
			}
		}
		#endregion // Show Control

		#region Startup Items

		public void BeginStartup()
		{
			if (Show != null) {
				// Sort the items
				Show.Items.Sort((item1, item2) => item1.ItemOrder.CompareTo(item2.ItemOrder));
				foreach (Shows.ShowItem item in Show.GetItems(Shows.ShowItemType.Startup)) {
					ItemQueue.Enqueue(item);
				}
				 
				ExecuteNextStartupItem();
			}
		}

		public void ExecuteNextStartupItem() 
		{
			if (State == StateType.Running)
			{
				// Do we have startup items to run?
				if (ItemQueue.Any())
				{
					_currentItem = ItemQueue.Dequeue();
					Shows.Action action = _currentItem.GetAction();
					action.ActionComplete += OnStartupActionComplete;
					ExecuteAction(action);
				}
				// Otherwise, move on to the sequential and background items
				else
				{
					BeginSequential();
					BeginBackground();
				}
			}
		}

		public void OnStartupActionComplete(object sender, EventArgs e)
		{
			var action = (sender as Shows.Action);
			if (action != null) {
				action.ActionComplete -= OnStartupActionComplete;
				lock(_actionLock)
				{
					RunningActions.Remove(action);
				}

				ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Startup action complete: " + action.ShowItem.Name);
				action.Dispose();
			}
			ExecuteNextStartupItem();
		}

		#endregion // Startup Items

		#region Sequential Items

		public void BeginSequential()
		{
			LogScheduleInfoEntry("BeginSequential");
			if (Show != null && State == StateType.Running)
			{
				State = StateType.Running;

				foreach (ShowItem item in Show.GetItems(ShowItemType.Sequential))
				{
					LogScheduleInfoEntry("BeginSequential: Enqueue:" + item.Name);
					ItemQueue.Enqueue(item);
				}

				if (ItemQueue.Any())
				{
					ExecuteNextSequentialItem();
				}
				else
				{
					LogScheduleInfoEntry("BeginSequential: Nothing in queue.");
				}
			}
		}

		public void ExecuteNextSequentialItem()
		{
			LogScheduleInfoEntry("ExecuteNextSequentialItem");
			if (State == StateType.Running)
			{
				if (ItemQueue.Any())
				{
					LogScheduleInfoEntry("ExecuteNextSequentialItem: Dequeue next item");
					_currentItem = ItemQueue.Dequeue();
					Shows.Action action = _currentItem.GetAction();
					action.ActionComplete += OnSequentialActionComplete;
					ExecuteAction(action);
				}
				else
				{
					LogScheduleInfoEntry("ExecuteNextSequentialItem: Nothing left in the queue. Restarting the queue.");
					// Restart the queue 
					BeginSequential();
				}
			}
		}

		public void OnSequentialActionComplete(object sender, EventArgs e)
		{
			Shows.Action action = (sender as Shows.Action);
			action.ActionComplete -= OnSequentialActionComplete;
			lock (_actionLock)
			{
				RunningActions.Remove(action);
			}
			ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Sequential action complete: " + action.ShowItem.Name);
			if (!StartShutdownIfRequested())
			{
				LogScheduleInfoEntry("OnSequentialActionComplete: Shutdown was NOT requested");
				ExecuteNextSequentialItem();
			}
		}

		#endregion // Startup Items

		#region Background Items

		public void BeginBackground()
		{
			if (Show != null)
			{
				foreach (ShowItem item in Show.GetItems(ShowItemType.Background))
				{
					Shows.Action action = item.GetAction();
					BackgroundActions.Add(action);
					ExecuteBackgroundAction(action);
				}
			}
		}

		public void ExecuteBackgroundAction(Shows.Action action)
		{
			if (State == StateType.Running)
			{
				action.ActionComplete += OnBackgroundActionComplete;
				ExecuteAction(action);
			}
		}

		public void OnBackgroundActionComplete(object sender, EventArgs e)
		{
			Shows.Action action = (sender as Shows.Action);
			action.ActionComplete -= OnBackgroundActionComplete;
			lock (_actionLock)
			{
				RunningActions.Remove(action);
			}
			ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Background action complete: " + action.ShowItem.Name);

			// Run it again and again and again and again and again and again and again and...
			if (!CheckForShutdown())
			{
				ExecuteBackgroundAction(action);
			}
		}

		public void StopBackground()
		{
			foreach (Shows.Action action in BackgroundActions)
			{
				action.Stop();
				action.Dispose();
			}
		}

		private void StopSequential()
		{
			foreach (ShowItem item in Show.GetItems(ShowItemType.Sequential))
			{
				Shows.Action action = item.GetAction();
				if (action.IsRunning)
				{
					action.Stop();
				}
				action.Dispose();
			}	
		}

		#endregion // Background Items

		#region Shutdown Items

		public bool StartShutdownIfRequested() {
			if (State == StateType.Shutdown || CheckForShutdown())
			{
				LogScheduleInfoEntry("Shutdown IS requested");
				BeginShutdown();
				return true;
			}
			return false;
		}

		public void BeginShutdown()
		{
			if (Show != null)
			{
				ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Starting shutdown");

				StopBackground();
				StopSequential();
				ItemQueue.Clear();

				State = StateType.Shutdown;
				foreach (ShowItem item in Show.GetItems(Shows.ShowItemType.Shutdown))
				{
					LogScheduleInfoEntry(string.Format("BeginShutdown: Enqueue: {0}", item.Name));
					ItemQueue.Enqueue(item);
				}

				ExecuteNextShutdownItem();
			}
		}

		private void LogScheduleInfoEntry(string message)
		{
			ScheduleExecutor.Logging.Info("({0}) {1}", Show!=null?Show.Name:"Unknown show", message);
		}

		public void ExecuteNextShutdownItem()
		{
			if (ItemQueue.Any() )
			{
				_currentItem = ItemQueue.Dequeue();
				Shows.Action action = _currentItem.GetAction();
				action.ActionComplete += OnShutdownActionComplete;
				ExecuteAction(action);
				// Otherwise, the show is done :(
			}
			else
			{
				State = StateType.Waiting;
				ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Show complete");
				Show.ReleaseAllActions();
			}
		}

		public void OnShutdownActionComplete(object sender, EventArgs e)
		{
			Shows.Action action = (sender as Shows.Action);
			action.ActionComplete -= OnShutdownActionComplete;
			lock (_actionLock)
			{
				RunningActions.Remove(action);
			}
			ScheduleExecutor.AddSchedulerLogEntry(Show.Name, "Shutdown action complete: " + action.ShowItem.Name);
			ExecuteNextShutdownItem();
		}

		#endregion // Shutdown Items

		[OnDeserializing]
		private void OnDeserializing(StreamingContext c)
		{
			if (_actionLock == null)
			{
				_actionLock = new object();
			}
		}

		public void Dispose()
		{
			if (tokenSourcePreProcess != null && tokenSourcePreProcess.Token.CanBeCanceled)
				tokenSourcePreProcess.Cancel();
			if (tokenSourcePreProcessAll != null && tokenSourcePreProcessAll.Token.CanBeCanceled)
				tokenSourcePreProcessAll.Cancel();
		}
	}
}
