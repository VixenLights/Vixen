namespace VixenModules.App.DisplayPreview.Model
{
    using System;
    using System.Collections.Generic;
    using Vixen.Execution;
    using Vixen.Module.App;
    using Vixen.Sys;
    using VixenModules.App.DisplayPreview.Views;

    public class DisplayPreviewModuleInstance : AppModuleInstanceBase
    {
        private const string DISPLAY_PREVIEW_MENU = "DisplayPreview.Menu";
        private const string EDIT_PREVIEW_SETUP_MENU = "Edit Preview Setup";
        private const string ENABLE_DISPLAY_PREVIEW_MENU = "Preferences window";
        private readonly List<ProgramContext> _programContexts;
        private IApplication _application;
        private AppCommand _setupCommand;

        public DisplayPreviewModuleInstance()
        {
            _programContexts = new List<ProgramContext>();
        }

        public override IApplication Application
        {
            set
            {
                _application = value;
                InjectAppCommands();
            }
        }

        public override void Dispose()
        {
            EnsureVisualizerIsClosed();
            base.Dispose();
        }

        public override void Loading()
        {
            Execution.ValuesChanged += ExecutionValuesChanged;
            Execution.NodesChanged += ExecutionNodesChanged;
            Execution.ProgramContextCreated += ProgramContextCreated;
            Execution.ProgramContextReleased += ProgramContextReleased;
        }

        public override void Unloading()
        {
            Execution.NodesChanged -= ExecutionNodesChanged;
            Execution.ValuesChanged -= ExecutionValuesChanged;
            Execution.ProgramContextCreated -= ProgramContextCreated;
            Execution.ProgramContextReleased -= ProgramContextReleased;
            _application.AppCommands.Remove(DISPLAY_PREVIEW_MENU);
        }

        private static void EnsureVisualizerIsClosed()
        {
            ViewManager.EnsureVisualizerIsClosed();
        }

        private static void ExecutionNodesChanged(object sender, EventArgs e)
        {
            // TODO: Remove any channels that are no longer valid.
        }

        private static void ExecutionValuesChanged(ExecutionStateValues stateValues)
        {
            ViewManager.UpdatePreviewExecutionStateValues(stateValues);
        }

        private DisplayPreviewModuleDataModel GetDisplayPreviewModuleDataModel()
        {
            return (DisplayPreviewModuleDataModel)StaticModuleData;
        }

        private void InjectAppCommands()
        {
            if (_application == null
                || _application.AppCommands == null)
            {
                return;
            }

            var isEnabled = GetDisplayPreviewModuleDataModel().IsEnabled;
            var rootCommand = new AppCommand(DISPLAY_PREVIEW_MENU, "Display Preview")
                              {
                                 Enabled = true, Visible = true, Style = AppCommand.AppCommandStyle.Menu 
                              };
            _setupCommand = new AppCommand(EDIT_PREVIEW_SETUP_MENU, "Edit Preview Setup");
            _setupCommand.Click += SetupAppCommandClick;
            _setupCommand.Enabled = isEnabled;
            _setupCommand.Visible = true;
            rootCommand.Add(_setupCommand);

            var enabledCommand = new LatchedAppCommand(ENABLE_DISPLAY_PREVIEW_MENU, "Preview Enabled");
            enabledCommand.Checked += EnabledCommandChecked;
            enabledCommand.Enabled = true;
            enabledCommand.Visible = true;            
            enabledCommand.IsChecked = isEnabled;
            rootCommand.Add(enabledCommand);
            _application.AppCommands.Add(rootCommand);
        }

        private void ProgramContextCreated(object sender, ProgramContextEventArgs e)
        {
            var programContext = e.ProgramContext;
            if (programContext != null)
            {
                _programContexts.Add(programContext);
                programContext.ProgramStarted += ProgramContextProgramStarted;
                programContext.ProgramEnded += ProgramContextProgramEnded;
            }
        }

        private void ProgramContextProgramEnded(object sender, ProgramEventArgs e)
        {
            Stop();
        }

        private void ProgramContextProgramStarted(object sender, ProgramEventArgs e)
        {
            Start();
        }

        private void ProgramContextReleased(object sender, ProgramContextEventArgs e)
        {
            var programContext = e.ProgramContext;
            if (programContext != null)
            {
                programContext.ProgramStarted -= ProgramContextProgramStarted;
                programContext.ProgramEnded -= ProgramContextProgramEnded;
                _programContexts.Remove(programContext);
            }
        }

        private void Setup()
        {
            ViewManager.DisplaySetupView(GetDisplayPreviewModuleDataModel());
        }

        private void SetupAppCommandClick(object sender, EventArgs e)
        {
            Setup();
        }

        private void Start()
        {
            var dataModel = GetDisplayPreviewModuleDataModel();
            if (dataModel.IsEnabled)
            {
                ViewManager.StartVisualizer(dataModel);
            }
        }

        private void Stop()
        {
            if (!GetDisplayPreviewModuleDataModel().Preferences.KeepVisualizerWindowOpen)
            {
                EnsureVisualizerIsClosed();
            }
        }

        private void EnabledCommandChecked(object sender, LatchedEventArgs e)
        {
            var isEnabled = e.CheckedState;
            GetDisplayPreviewModuleDataModel().IsEnabled = isEnabled;             
            if (_setupCommand != null)
            {
                _setupCommand.Enabled = isEnabled;
            }
        }
    }
}
