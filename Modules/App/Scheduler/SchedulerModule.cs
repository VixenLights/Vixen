using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.App;
using Timer = System.Timers.Timer;

namespace VixenModules.App.Scheduler {
	public class SchedulerModule : AppModuleInstanceBase {
		private IApplication _application;
		private SchedulerData _data;
		private Timer _scheduleCheckTimer;
		private ScheduleService _scheduleService;
		private Dictionary<IProgramContext, ScheduleItem> _contexts;
		private SynchronizationContext _synchronizationContext;

		private const string ID_ROOT = "SchedulerRoot";

		public SchedulerModule() {
			_scheduleService = new ScheduleService();
			_contexts = new Dictionary<IProgramContext, ScheduleItem>();
		}

		public override void Loading() {
			_AddApplicationMenu();
			_SetEnableState(_data.IsEnabled);
			_synchronizationContext = SynchronizationContext.Current;
			VixenSystem.Logs.AddLog(new SchedulerLog());
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
			IEnumerable<ScheduleItem> validItems = _scheduleService.GetQualifiedItems(_data.Items).Cast<ScheduleItem>();

			foreach(ScheduleItem item in validItems) {
				if(_CanExecute(item)) {
					_synchronizationContext.Post((o) => _Execute(o as ScheduleItem), item);
				}
			}
		}

		private bool _CanExecute(ScheduleItem item) {
			return !item.IsExecuting;
		}

		private void _Execute(ScheduleItem item) {
			try {
				_SetEnableState(false);

				Program program = Vixen.Services.ApplicationServices.LoadProgram(item.FilePath);
				IProgramContext context = VixenSystem.Contexts.CreateProgramContext(new ContextFeatures(ContextCaching.ContextLevelCaching), program);
				
				context.ProgramEnded += context_ProgramEnded;
	
				_contexts[context] = item;
				item.IsExecuting = true;
				item.LastExecutedAt = DateTime.Now;

				context.Start();

				_SetEnableState(true);
			} catch(Exception ex) {
				VixenSystem.Logging.Schedule("Could not execute sequence " + item.FilePath + "; " + ex.Message);
			}
		}

		void context_ProgramEnded(object sender, ProgramEventArgs e) {
			ProgramContext context = sender as ProgramContext;
			context.ProgramEnded -= context_ProgramEnded;
			VixenSystem.Contexts.ReleaseContext(context);
			ScheduleItem item;
			if(_contexts.TryGetValue(context, out item)) {
				item.IsExecuting = false;
				_contexts.Remove(context);
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
					_SetEnableState(_data.IsEnabled);
				};

				AppCommand separator1 = new AppCommand("s1", "-");

				AppCommand showCommand = new AppCommand("SchedulerShow", "Show");
				showCommand.Click += (sender, e) => {
					using(SchedulerForm schedulerForm = new SchedulerForm(_data)) {
						if(schedulerForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
							enabledCommand.IsChecked = _data.IsEnabled;
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
