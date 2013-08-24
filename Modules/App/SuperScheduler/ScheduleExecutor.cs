using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;
using System.Threading;

namespace VixenModules.App.SuperScheduler
{
	public class ScheduleExecutor
	{
		private System.Timers.Timer _scheduleCheckTimer = null;
		private SynchronizationContext _synchronizationContext;
		static private StatusForm statusForm = null;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public ScheduleExecutor(SuperSchedulerData data)
		{
			Data = data;
			_synchronizationContext = SynchronizationContext.Current;
			Timer.Enabled = true;
		}

		private SuperSchedulerData Data { get; set; }

		static List<string> log = new List<string>();
		static public void AddSchedulerLogEntry(string showName, string logEntry) 
		{
			if (log == null)
				log = new List<string>();
			string logString = DateTime.Now.ToShortDateString() + " @ " + DateTime.Now.ToShortTimeString() + " (" + showName + ") " + logEntry;
			log.Add(logString);
			statusForm.AddLogEntry(logString);
		}

		private List<Shows.Show> ShowList
		{
			get { return Shows.ShowsData.ShowList; }
		}

		private bool _enabled = false;
		public bool Enabled 
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
				ShowStatusForm(_enabled);
				if (_enabled)
					Logging.Info("Schedule Enabled");
				else
					Logging.Info("Schedule Disabled");
			}
		}

		private System.Timers.Timer Timer
		{
			get
			{
				if (_scheduleCheckTimer == null)
				{
					_scheduleCheckTimer = new System.Timers.Timer();
					_SetSchedulerCheckIntervalInSeconds(Data.CheckIntervalInSeconds);
					_scheduleCheckTimer.Elapsed += _scheduleCheckTimer_Elapsed;
				}
				return _scheduleCheckTimer;
			}
		}

		private void _SetSchedulerCheckIntervalInSeconds(int value)
		{
			Timer.Interval = value * 1000;
		}

		private void _scheduleCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			// We're currently in a worker thread and need to delegate back to the UI thread.
			_synchronizationContext.Post(o => ProcessTimerInUIThread(), null);
		}

		private void ProcessTimerInUIThread()
		{
			CheckSchedule();

			string stateStr = "Waiting for next show to start..";
			foreach (ScheduleItem item in Data.Items)
			{
				if (item.State != StateType.Waiting)
				{
					switch (item.State)
					{
						case StateType.Startup:
							stateStr = "Starting: " + item.Show.Name;
							break;
						case StateType.Running:
							stateStr = "Running: " + item.Show.Name;
							break;
						case StateType.Shutdown:
							stateStr = "Shutdown: " + item.Show.Name;
							break;
						case StateType.Paused:
							stateStr = "Paused: " + item.Show.Name;
							break;
					}
					break;
				}
			}
			statusForm.Status = stateStr;
		}

		private void ShowStatusForm(bool show)
		{
			if (statusForm == null)
			{
				statusForm = new StatusForm(Data, this);
				foreach (string logEntry in log)
				{
					statusForm.AddLogEntry(logEntry);
				}
			}
			statusForm.Visible = show;
		}

		private void CheckSchedule()
		{
			// Were we just enabled?
			if (Data.IsEnabled && !Enabled)
			{
				Enabled = Data.IsEnabled;
				foreach (ScheduleItem item in Data.Items)
				{
					item.State = StateType.Waiting;
				}
			}
			// Were we just disabled?
			else if (!Data.IsEnabled && Enabled)
			{
				Enabled = Data.IsEnabled;
			}

			// Now, if the schedule executor is enabled, process it!
			if (Enabled)
			{
				foreach (ScheduleItem item in Data.Items)
				{
					item.ProcessSchedule();
				}
			}
		}

		public void Stop(bool gracefully)
		{
			foreach (ScheduleItem item in Data.Items)
			{
				item.Stop(gracefully);
			}
		}
	}
}
