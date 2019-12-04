using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Data.Value;
using Vixen.Module;
using System.Drawing;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vixen.Sys;
using VixenModules.App.Shows;
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

		public DateTime NextStartDateTime
		{
			get
			{
				var now = DateTime.Now;
				if (_startDate < now && _endDate > now)
				{
					var date = _startTime.TimeOfDay > now.TimeOfDay ? now : now.AddDays(1);
					return new DateTime(date.Year, date.Month, date.Day,
										 _startTime.Hour, _startTime.Minute, _startTime.Second);
				}
				
				return new DateTime(_startDate.Year, _startDate.Month, _startDate.Day,
										 _startTime.Hour, _startTime.Minute, _startTime.Second); 
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

			if (InProcessEndTime.CompareTo(DateTime.Now) <= 0) return true;

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
				if (ItemQueue.Any() )
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
