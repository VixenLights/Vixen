using System.Threading;
//using System.Timers;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;
using System;

namespace VixenModules.App.SuperScheduler
{
	public class SuperSchedulerModule : AppModuleInstanceBase
	{
		private IApplication _application;
		private SuperSchedulerData _data;
		//private System.Timers.Timer _scheduleCheckTimer = null;
		//private SynchronizationContext _synchronizationContext;

		private ScheduleExecutor _executor;

		//private StatusForm statusForm = null;

		static public string MENU_ID_ROOT = "SuperSchedulerRoot";
		static public string MENU_ID_NAME = "Scheduler";

		public override void Loading()
		{
			//_synchronizationContext = SynchronizationContext.Current;

			CreateMenu();
			//SetSchedulerEnableState(_data.IsEnabled);
			_executor = new ScheduleExecutor(_data);
			_executor.CheckSchedule();
		}

		public override void Unloading()
		{
			//SetSchedulerEnableState(false);
			_executor.Enabled = false;
			_executor = null;
		}

		public override IApplication Application
		{
			set { _application = value; }
		}

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = (SuperSchedulerData) value; }
		}

		public void Start()
		{
			_executor.CheckSchedule();
		}

		//private System.Timers.Timer Timer
		//{
		//    get
		//    {
		//        if (_scheduleCheckTimer == null) 
		//        {
		//            _scheduleCheckTimer = new System.Timers.Timer();
		//            _SetSchedulerCheckIntervalInSeconds(_data.CheckIntervalInSeconds);
		//            _scheduleCheckTimer.Elapsed += _scheduleCheckTimer_Elapsed;
		//        }
		//        return _scheduleCheckTimer;
		//    }
		//}

		//private void _scheduleCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
		//{
		//    // We're currently in a worker thread and need to delegate back to the UI thread.
		//    _synchronizationContext.Post(o => _CheckSchedule(), null);
		//}

		//private void SetSchedulerEnableState(bool value)
		//{
		//    //Timer.Enabled = value;
		//    ShowStatusForm(value);

		//    // We have some time before the timer expires.
		//    if (value) {
		//        // Check immediately if enabled.
		//        _CheckSchedule();
		//    }
		//    else {
		//        // Otherwise immediately terminate anything that's running.
		//        //_stateMachine.TerminateAll();
		//    }
		//}

		//private void _SetSchedulerCheckIntervalInSeconds(int value)
		//{
		//    Timer.Interval = value*1000;
		//}

		private bool _AppSupportsCommands()
		{
			return _application != null && _application.AppCommands != null;
		}

		public AppCommand GetSchedulerMenu()
		{
			AppCommand menu = _application.AppCommands.Find(MENU_ID_ROOT);
			if (menu == null)
			{
				menu = new AppCommand(MENU_ID_ROOT, MENU_ID_NAME);
				_application.AppCommands.Add(menu);
			}
			return menu;
		}

		public void CreateMenu()
		{
			AppCommand schedulerMenu = GetSchedulerMenu();

			AppCommand showCommand = new AppCommand("SuperSchedulerCreateSchedule", "Schedules");
			showCommand.Click += (sender, e) =>
			{
				using (SetupForm form = new SetupForm(_data))
				{
					if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{

					}
				}
			};

			LatchedAppCommand enabledCommand = new LatchedAppCommand("SuperSchedulerEnabled", "Enabled");
			enabledCommand.IsChecked = _data.IsEnabled;
			enabledCommand.Checked += async (sender, e) =>
			{
				_data.IsEnabled = e.CheckedState;
				_executor.CheckSchedule();
				await VixenSystem.SaveModuleConfigAsync();
			};

			/*if (schedulerMenu.Items.Length > 0)
				schedulerMenu.Add(new AppCommand("s1", "-"));*/
			schedulerMenu.Add(showCommand);
			schedulerMenu.Add(new AppCommand("s1", "-"));
			schedulerMenu.Add(enabledCommand);
		}

		private void _CheckSchedule()
		{
			if (_data.IsEnabled) {
				//_stateMachine.Update();
			}
		}

		//private void ShowStatusForm(bool show) 
		//{
		//    if (statusForm == null)
		//    {
		//        statusForm = new StatusForm(_data);
		//    }
		//    statusForm.Visible = show;
		//}
	}
}