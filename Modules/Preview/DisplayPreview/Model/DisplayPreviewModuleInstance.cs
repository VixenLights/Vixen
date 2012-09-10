namespace VixenModules.Preview.DisplayPreview.Model
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Vixen.Execution.Context;
    using Vixen.Module.Preview;
    using Vixen.Sys;
    using Vixen.Sys.Managers;
    using VixenModules.Preview.DisplayPreview.Views;

    public class DisplayPreviewModuleInstance : FormPreviewModuleInstanceBase
    {
        private const string DISPLAY_PREVIEW_MENU = "DisplayPreview.Menu";

        private const string EDIT_PREVIEW_SETUP_MENU = "Edit Preview Setup";

        private const string ENABLE_DISPLAY_PREVIEW_MENU = "Preferences window";

        private readonly List<IProgramContext> _programContexts;

        private AppCommand _setupCommand;

        public DisplayPreviewModuleInstance()
        {
            _programContexts = new List<IProgramContext>();
        }

        public override void Dispose()
        {
            EnsureVisualizerIsClosed();
            base.Dispose();
        }
        
        public void Unloading()
        {
            Execution.NodesChanged -= ExecutionNodesChanged;
//            Execution.ValuesChanged -= ExecutionValuesChanged;
//            Execution.ProgramContextCreated -= ProgramContextCreated;
//            Execution.ProgramContextReleased -= ProgramContextReleased;
//            _application.AppCommands.Remove(DISPLAY_PREVIEW_MENU);
        }

        private static void EnsureVisualizerIsClosed()
        {
            ViewManager.EnsureVisualizerIsClosed();
        }

        private static void ExecutionNodesChanged(object sender, EventArgs e)
        {
            // TODO: Remove any channels that are no longer valid.
        }

        private static void ExecutionValuesChanged(ExecutionState stateValues)
        {
            ViewManager.UpdatePreviewExecutionStateValues(stateValues);
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

        private DisplayPreviewModuleDataModel GetDisplayPreviewModuleDataModel()
        {
            return (DisplayPreviewModuleDataModel)StaticModuleData;
        }

        private void InjectAppCommands()
        {
//            if (_application == null
//                || _application.AppCommands == null)
//            {
//                return;
//            }

            var isEnabled = GetDisplayPreviewModuleDataModel().IsEnabled;
            var rootCommand = new AppCommand(DISPLAY_PREVIEW_MENU, "Display Preview")
                              { Enabled = true, Visible = true, Style = AppCommand.AppCommandStyle.Menu };
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
            //_application.AppCommands.Add(rootCommand);
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
            ResetColors(false);
            Stop();
        }

        private void ProgramContextProgramStarted(object sender, ProgramEventArgs e)
        {
            ResetColors(true);
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

        private void ResetColors(bool isRunning)
        {
            var dataModel = GetDisplayPreviewModuleDataModel();
            foreach (var displayItem in dataModel.DisplayItems)
            {
                displayItem.ResetColor(isRunning);
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

        protected override void Update()
        {
            throw new NotImplementedException();
        }

        protected override Form Initialize()
        {
            //Execution.ValuesChanged += ExecutionValuesChanged;
            Execution.NodesChanged += ExecutionNodesChanged;
            //Execution.ProgramContextCreated += ProgramContextCreated;
            //Execution.ProgramContextReleased += ProgramContextReleased;
            Preferences.CurrentPreferences = GetDisplayPreviewModuleDataModel().Preferences;
            ResetColors(false);
            InjectAppCommands();

            return null;
        }
    }
}
