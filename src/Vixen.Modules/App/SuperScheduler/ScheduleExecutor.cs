using Common.Broadcast;
using System.Timers;
using Vixen.Sys;
using VixenModules.App.WebServer.Model;
using Vixen.Execution;

namespace VixenModules.App.SuperScheduler
{
	public class ScheduleExecutor
	{
		private System.Timers.Timer _scheduleCheckTimer = null;
		private SynchronizationContext _synchronizationContext;
		static private StatusForm statusForm = null;
		//private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public static NLog.Logger Logging = NLog.LogManager.GetLogger("Scheduler");
		//private static NLog.Logger ScheduleLogging = NLog.LogManager.GetLogger("Scheduler");

		public ScheduleExecutor(SuperSchedulerData data)
		{
			Data = data;
			_synchronizationContext = VixenSystem.UIContext;
			if (Enabled)
			{
				Timer.Enabled = true;
			}

			// Setup the subscribers to the RESTful API.
			Broadcast.Subscribe<Presentation>(this, "PlayShow", RunShow);
			Broadcast.Subscribe<Presentation>(this, "StopShow", StopShow);
			Broadcast.Subscribe<Presentation>(this, "PauseShow", PauseShow);
		}

		private SuperSchedulerData Data { get; set; }

		static List<string> log = new List<string>();
		static public void AddSchedulerLogEntry(string showName, string logEntry) 
		{
			if (statusForm != null)
			{
				if (log == null)
					log = new List<string>();
				string logString = string.Format("{0} ( {1} ) {2}", DateTime.Now.ToString("G"), showName, logEntry);
				Logging.Info("(" + showName + ") " + logEntry);
				//Logging.Log(NLog.LogLevel.FromString("Scheduler"), logEntry);
				statusForm.AddLogEntry(logString);
			}
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
				_synchronizationContext.Send(o => ShowStatusForm(_enabled), null);
				
				if (Enabled)
				{
					Timer.Enabled = true;
				}
				else
				{
					Timer.Enabled = false;

				}
				Logging.Info(_enabled  ? "Schedule Enabled" : "Schedule Disabled");

			}
		}

		public bool ManuallyDisabled { get; set; }

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
			// At least that's what the "other" scheduler said
			_synchronizationContext.Post(o => ProcessTimerInUIThread(), null);
		}

		private void ProcessTimerInUIThread()
		{
			CheckSchedule();

			string stateStr = String.Empty;
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

			if (statusForm != null)
			{
				if (ManuallyDisabled)
				{
					statusForm.Status = "All Show Disabled";
				}
				else
				{
					if (stateStr == String.Empty)
					{
						var item = UpNext();
						if (item != null)
						{
							if (item.Show != null)
							{
								stateStr = string.Format("Up Next: {0} on {1}", item.Show.Name, item.NextStartDateTime);
							}
							else
							{
								stateStr = $"Up Next: Show not selected on {item.NextStartDateTime}";
							}
						}
						else
						{
							stateStr = "Nothing scheduled.... ";
						}
						//Determine next show
					}
					statusForm.Status = stateStr;
				}
			}
		}

		private ScheduleItem UpNext()
		{
			return Data.Items.Where(
				x => x.State == StateType.Waiting && 
				x.Enabled && 
				x.NextStartDateTime != null &&
				x.NextStartDateTime > DateTime.Now).OrderBy(s => s.NextStartDateTime).FirstOrDefault();
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

		internal void CheckSchedule()
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
				Stop(false);
				Enabled = Data.IsEnabled;
			}

			// Now, if the schedule executor is enabled, process it!
			if (Enabled && !ManuallyDisabled)
			{
				foreach (ScheduleItem item in Data.Items)
				{
					item.ProcessSchedule();
				}
			}
		}

		/// <summary>
		/// Stops the currently running Show
		/// </summary>
		/// <param name="show">Specifies the name of the Show.</param>
		public void StopShow(Presentation show)
		{
			Stop(false);
		}

		public void Stop(bool gracefully)
		{
			ManuallyDisabled = true;

			for (int index = Data.Items.Count - 1; index >= 0; index--)
			{
				ScheduleItem item = Data.Items[index];
				if (item.State != StateType.Waiting)
				{
					item.Stop(gracefully);
					Data.Items.Remove(item);
				}
			}
		}

		public void Start()
		{
			
			ManuallyDisabled = false;
		}

		/// <summary>
		/// Starts or re-starts a Show.
		/// </summary>
		/// <param name="show">Specifies teh Show name.</param>
		public void RunShow(Presentation show)
		{
			// Build a list of all running shows that match the show parameter name.
			IEnumerable<ScheduleItem> scheduleItems = Data.Items.Where(x => x.Show.Name == show.Name);

			// If there is an existing show, then it is paused...
			if (scheduleItems.Count() > 0)
			{
				// So we'll unpause that show.
				PauseShow(show, false);
			}

			// Else start a new Show.
			else
			{
				ScheduleItem scheduleItem = new ScheduleItem()
				{
					ShowID = Guid.Parse(show.Info)
				};
				Data.Items.Add(scheduleItem);
				scheduleItem.Start(true);
			}
		}

		/// <summary>
		/// Pause a running Show
		/// </summary>
		/// <param name="show">Specifies the Show name.</param>
		public void PauseShow(Presentation show)
		{
			PauseShow(show, true);
		}

		/// <summary>
		/// Pause or restart a Show.
		/// </summary>
		/// <param name="show">Specifies the name of the Show.</param>
		/// <param name="pause">True: pause the show, False: resume the show.</param>
		private void PauseShow(Presentation show, bool pause)
		{
			// Find the running show
			foreach (ScheduleItem item in Data.Items.Where(x => x.Show.Name == show.Name && x.State == StateType.Running))
			{
				// Within that show, get the name of every included sequence
				foreach (var sequence in item.Show.Items)
				{
					// Search for that running sequence in all the running sequences to acquire its Context inorder to pause/resume it
					IEnumerable<IContext> contexts = VixenSystem.Contexts.Where(x => sequence.Name.Contains(x.Name) && x.IsRunning && x.IsPaused != pause);
					foreach (var context in contexts)
						if (pause)
						{
							context.Pause();
						}
						else
						{
							context.Resume();
						}
				}
			}
		}
	}
}
