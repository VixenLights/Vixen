using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.Scheduler {
	public class SchedulerModule : AppModuleInstanceBase {
		private IApplication _application;
		private SchedulerData _data;
		private Timer _scheduleCheckTimer;
		private ScheduleService _scheduleService;

		private const string ID_ROOT = "SchedulerRoot";

		public override void Loading() {
			_AddApplicationMenu();
			_SetEnableState(_data.IsEnabled);
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
			set { _data = (SchedulerData)value; }
		}

		private Timer Timer {
			get {
				if(_scheduleCheckTimer == null) {
					_scheduleCheckTimer = new Timer(_data.CheckIntervalInSeconds * 1000);
					_scheduleCheckTimer.Elapsed += _scheduleCheckTimer_Elapsed;
				}
				return _scheduleCheckTimer;
			}
		}

		void _scheduleCheckTimer_Elapsed(object sender, ElapsedEventArgs e) {
			IEnumerable<ScheduleItem> validItems = _scheduleService.GetValidEvents(TimeSpan.FromSeconds(_data.CheckIntervalInSeconds));
			foreach(ScheduleItem item in validItems) {
				//***
			}
		}

		private void _SetEnableState(bool value) {
			Timer.Enabled = value;
		}

		private void _AddApplicationMenu() {
			if(_AppSupportsCommands()) {
				AppCommand rootCommand = new AppCommand(ID_ROOT, "Scheduler");

				LatchedAppCommand enabledCommand = new LatchedAppCommand("SchedulerEnabled", "Enabled");
				enabledCommand.IsChecked = _data.IsEnabled;
				enabledCommand.Checked += (sender, e) => {
					// Not setting the data member in _SetEnableState because we want to be
					// able to call _SetEnableState without affecting the data (to stop
					// the scheduler upon shutdown).
					_data.IsEnabled = e.CheckedState;
					_SetEnableState(e.CheckedState);
				};

				AppCommand separator1 = new AppCommand("s1", "-");

				AppCommand showCommand = new AppCommand("SchedulerShow", "Show");
				showCommand.Click += (sender, e) => {
					using(SchedulerForm schedulerForm = new SchedulerForm(_data)) {
						if(schedulerForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
							//*** update data
						}
					}
				};

				rootCommand.Add(enabledCommand);
				rootCommand.Add(separator1);
				rootCommand.Add(showCommand);

				_application.AppCommands.Add(rootCommand);
			}
		}

		private void _RemoveApplicationMenu() {
			if(_AppSupportsCommands()) {
				_application.AppCommands.Remove(ID_ROOT);
			}
		}

		private bool _AppSupportsCommands() {
			return _application != null && _application.AppCommands != null;
		}

	}
}
