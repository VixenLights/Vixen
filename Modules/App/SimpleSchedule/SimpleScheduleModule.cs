using System.Threading;
using System.Timers;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Services;
using Vixen.Sys;
using Timer = System.Timers.Timer;

namespace VixenModules.App.SimpleSchedule {
	public class SimpleScheduleModule : AppModuleInstanceBase {
		private IApplication _application;
		private SimpleSchedulerData _data;
		private ScheduleStates _stateMachine;
		private Timer _scheduleCheckTimer;
		private SynchronizationContext _synchronizationContext;

		private const string MENU_ID_ROOT = "SchedulerRoot";

		public override void Loading() {
			_stateMachine = new ScheduleStates();

			foreach(ScheduledItem scheduledItem in _data.ScheduledItems) {
				_AddScheduledItemToStateMachine(scheduledItem);
			}

			_AddApplicationMenu();
			_SetEnableState(_data.IsEnabled);
			_synchronizationContext = SynchronizationContext.Current;
			VixenSystem.Logs.AddLog(new SchedulerLog());
		}

		private void _AddScheduledItemToStateMachine(IScheduledItem scheduledItem) {
			if(SequenceTypeService.Instance.IsValidSequenceFileType(scheduledItem.ItemFilePath)) {
				_stateMachine.AddSequence(scheduledItem);
			} else if(scheduledItem.ItemFilePath.EndsWith(Program.Extension)) {
				_stateMachine.AddProgram(scheduledItem);
			}
			// Otherwise don't add it.
		}

		public override void Unloading() {
			_RemoveApplicationMenu();
			_SetEnableState(false);
		}

		public override IApplication Application {
			set { _application = value; }
		}

		public override IModuleDataModel StaticModuleData {
			get { return _data; }
			set { _data = (SimpleSchedulerData)value; }
		}

		private Timer _Timer {
			get {
				if(_scheduleCheckTimer == null) {
					_scheduleCheckTimer = new Timer(_data.CheckIntervalInSeconds * 1000);
					_scheduleCheckTimer.Elapsed += _scheduleCheckTimer_Elapsed;
				}
				return _scheduleCheckTimer;
			}
		}

		private void _scheduleCheckTimer_Elapsed(object sender, ElapsedEventArgs e) {
			// We're currently in a worker thread and need to delegate back to the UI thread.
			_synchronizationContext.Post(o => _stateMachine.Update(), null);
		}

		private void _SetEnableState(bool value) {
			_Timer.Enabled = value;
		}

		private void _AddApplicationMenu() {
			if(_AppSupportsCommands()) {
				AppCommand toolsMenu = _GetToolsMenu();
				AppCommand rootCommand = new AppCommand(MENU_ID_ROOT, "Simple Scheduler");

				LatchedAppCommand enabledCommand = _CreateEnabledCommand();
				AppCommand separator1 = new AppCommand("s1", "-");
				AppCommand showCommand = _CreateShowCommand();

				rootCommand.Add(enabledCommand);
				rootCommand.Add(separator1);
				rootCommand.Add(showCommand);

				toolsMenu.Add(rootCommand);
			}
		}

		private LatchedAppCommand _CreateEnabledCommand() {
			LatchedAppCommand enabledCommand = new LatchedAppCommand("SimpleSchedulerEnabled", "Enabled");
			enabledCommand.IsChecked = _data.IsEnabled;
			enabledCommand.Checked += (sender, e) => {
				// Not setting the data member in _SetEnableState because we want to be
				// able to call _SetEnableState without affecting the data (to stop
				// the scheduler upon shutdown).
				_data.IsEnabled = e.CheckedState;
				_SetEnableState(_data.IsEnabled);
			};

			return enabledCommand;
		}

		private AppCommand _CreateShowCommand() {
			AppCommand showCommand = new AppCommand("SimpleSchedulerShow", "Show");
			showCommand.Click += (sender, e) => {
				//using(SchedulerForm schedulerForm = new SchedulerForm(_data)) {
				//    if(schedulerForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				//        enabledCommand.IsChecked = _data.IsEnabled;
				//    }
				//}
			};

			return showCommand;
		}

		private void _RemoveApplicationMenu() {
			if(_AppSupportsCommands()) {
				AppCommand toolsMenu = _GetToolsMenu();
				toolsMenu.Remove(MENU_ID_ROOT);
			}
		}

		private bool _AppSupportsCommands() {
			return _application != null && _application.AppCommands != null;
		}

		private AppCommand _GetToolsMenu() {
			AppCommand toolsMenu = _application.AppCommands.Find("Tools");
			if(toolsMenu == null) {
				toolsMenu = new AppCommand("Tools", "Tools");
				_application.AppCommands.Add(toolsMenu);
			}
			return toolsMenu;
		}
	}
}
