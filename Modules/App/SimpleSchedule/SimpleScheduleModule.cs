using System.Threading;
using System.Timers;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Services;
using Vixen.Sys;
using Timer = System.Timers.Timer;
using VixenModules.App.SimpleSchedule.Forms;
using System;

namespace VixenModules.App.SimpleSchedule
{
    public class SimpleScheduleModule : AppModuleInstanceBase
    {
        private IApplication _application;
        private SimpleSchedulerData _data;
        private ScheduleStates _stateMachine;
        private Timer _scheduleCheckTimer;
        private SynchronizationContext _synchronizationContext;
        private LatchedAppCommand _enabledCommand;
        private AppCommand _showCommand;

        private const string MENU_ID_ROOT = "SchedulerRoot";

        public override void Loading()
        {
            _stateMachine = new ScheduleStates();

            try
            {
                foreach (ScheduledItem scheduledItem in _data.ScheduledItems)
                {
                    _AddScheduledItemToStateMachine(scheduledItem);
                }
            }
            catch (Exception ex)
            {
                int x = 0;
            }

            _AddApplicationMenu();
            _SetSchedulerEnableState(_data.IsEnabled);
            _synchronizationContext = SynchronizationContext.Current;
            VixenSystem.Logs.AddLog(new SchedulerLog());
        }

        public override void Unloading()
        {
            _RemoveApplicationMenu();
            _SetSchedulerEnableState(false);
        }

        public override IApplication Application
        {
            set { _application = value; }
        }

        public override IModuleDataModel StaticModuleData
        {
            get { return _data; }
            set { _data = (SimpleSchedulerData)value; }
        }

        private void _AddScheduledItemToStateMachine(IScheduledItem scheduledItem)
        {
            if (SequenceTypeService.Instance.IsValidSequenceFileType(scheduledItem.ItemFilePath))
            {
                _stateMachine.AddSequence(scheduledItem);
            }
            else if (scheduledItem.ItemFilePath.EndsWith(Program.Extension))
            {
                _stateMachine.AddProgram(scheduledItem);
            }
            // Otherwise don't add it.
        }

        private Timer _Timer
        {
            get
            {
                if (_scheduleCheckTimer == null)
                {
                    _scheduleCheckTimer = new Timer();
                    _SetSchedulerCheckIntervalInSeconds(_data.CheckIntervalInSeconds);
                    _scheduleCheckTimer.Elapsed += _scheduleCheckTimer_Elapsed;
                }
                return _scheduleCheckTimer;
            }
        }

        private void _scheduleCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // We're currently in a worker thread and need to delegate back to the UI thread.
            _synchronizationContext.Post(o => _stateMachine.Update(), null);
        }

        private void _SetSchedulerEnableState(bool value)
        {
            _Timer.Enabled = value;
        }

        private void _SetSchedulerCheckIntervalInSeconds(int value)
        {
            _Timer.Interval = value * 1000;
        }

        private void _AddApplicationMenu()
        {
            if (_AppSupportsCommands())
            {
                AppCommand toolsMenu = _GetToolsMenu();
                AppCommand rootCommand = new AppCommand(MENU_ID_ROOT, "Simple Scheduler");

                rootCommand.Add(_enabledCommand ?? (_enabledCommand = _CreateEnabledCommand()));
                rootCommand.Add(new AppCommand("s1", "-"));
                rootCommand.Add(_showCommand ?? (_showCommand = _CreateShowCommand()));

                toolsMenu.Add(rootCommand);
            }
        }

        private LatchedAppCommand _CreateEnabledCommand()
        {
            LatchedAppCommand enabledCommand = new LatchedAppCommand("SimpleSchedulerEnabled", "Enabled");
            enabledCommand.IsChecked = _data.IsEnabled;
            enabledCommand.Checked += (sender, e) =>
            {
                // Not setting the data member in _SetSchedulerEnableState because we want to be
                // able to call _SetSchedulerEnableState without affecting the data (to stop
                // the scheduler upon shutdown).
                _data.IsEnabled = e.CheckedState;
                _SetSchedulerEnableState(_data.IsEnabled);
            };

            return enabledCommand;
        }

        private AppCommand _CreateShowCommand()
        {
            AppCommand showCommand = new AppCommand("SimpleSchedulerShow", "Show");
            showCommand.Click += (sender, e) =>
            {
                using (ConfigureSchedule cs = new ConfigureSchedule(_data))
                {
                    if (cs.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _enabledCommand.IsChecked = _data.IsEnabled;
                    }
                }
            };

            return showCommand;
        }

        private void _RemoveApplicationMenu()
        {
            if (_AppSupportsCommands())
            {
                AppCommand toolsMenu = _GetToolsMenu();
                toolsMenu.Remove(MENU_ID_ROOT);
            }
        }

        private bool _AppSupportsCommands()
        {
            return _application != null && _application.AppCommands != null;
        }

        private AppCommand _GetToolsMenu()
        {
            AppCommand toolsMenu = _application.AppCommands.Find("Tools");
            if (toolsMenu == null)
            {
                toolsMenu = new AppCommand("Tools", "Tools");
                _application.AppCommands.Add(toolsMenu);
            }
            return toolsMenu;
        }
    }
}
