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
            this._programContexts = new List<ProgramContext>();
        }

        public override IApplication Application
        {
            set
            {
                this._application = value;
                this.InjectAppCommands();
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
            Execution.ProgramContextCreated += this.ProgramContextCreated;
            Execution.ProgramContextReleased += this.ProgramContextReleased;
            Preferences.CurrentPreferences = GetDisplayPreviewModuleDataModel().Preferences;
            this.ResetColors(false);
        }

        public override void Unloading()
        {
            Execution.NodesChanged -= ExecutionNodesChanged;
            Execution.ValuesChanged -= ExecutionValuesChanged;
            Execution.ProgramContextCreated -= this.ProgramContextCreated;
            Execution.ProgramContextReleased -= this.ProgramContextReleased;
            this._application.AppCommands.Remove(DISPLAY_PREVIEW_MENU);
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

        private void EnabledCommandChecked(object sender, LatchedEventArgs e)
        {
            var isEnabled = e.CheckedState;
            this.GetDisplayPreviewModuleDataModel().IsEnabled = isEnabled;
            if (this._setupCommand != null)
            {
                this._setupCommand.Enabled = isEnabled;
            }
        }

        private void ResetColors(bool isRunning)
        {
            var dataModel = this.GetDisplayPreviewModuleDataModel();
            foreach (var displayItem in dataModel.DisplayItems)
            {
                displayItem.ResetColor(isRunning);
            }
        }
      
        private DisplayPreviewModuleDataModel GetDisplayPreviewModuleDataModel()
        {
            return (DisplayPreviewModuleDataModel)this.StaticModuleData;
        }

        private void InjectAppCommands()
        {
            if (this._application == null || this._application.AppCommands == null)
            {
                return;
            }

            var isEnabled = this.GetDisplayPreviewModuleDataModel().IsEnabled;
            var rootCommand = new AppCommand(DISPLAY_PREVIEW_MENU, "Display Preview")
                {
                   Enabled = true, Visible = true, Style = AppCommand.AppCommandStyle.Menu 
                };
            this._setupCommand = new AppCommand(EDIT_PREVIEW_SETUP_MENU, "Edit Preview Setup");
            this._setupCommand.Click += this.SetupAppCommandClick;
            this._setupCommand.Enabled = isEnabled;
            this._setupCommand.Visible = true;
            rootCommand.Add(this._setupCommand);

            var enabledCommand = new LatchedAppCommand(ENABLE_DISPLAY_PREVIEW_MENU, "Preview Enabled");
            enabledCommand.Checked += this.EnabledCommandChecked;
            enabledCommand.Enabled = true;
            enabledCommand.Visible = true;
            enabledCommand.IsChecked = isEnabled;
            rootCommand.Add(enabledCommand);
            this._application.AppCommands.Add(rootCommand);
        }

        private void ProgramContextCreated(object sender, ProgramContextEventArgs e)
        {
            var programContext = e.ProgramContext;
            if (programContext != null)
            {
                this._programContexts.Add(programContext);
                programContext.ProgramStarted += this.ProgramContextProgramStarted;
                programContext.ProgramEnded += this.ProgramContextProgramEnded;
            }
        }

        private void ProgramContextProgramEnded(object sender, ProgramEventArgs e)
        {
            this.ResetColors(false);
            this.Stop();
        }

        private void ProgramContextProgramStarted(object sender, ProgramEventArgs e)
        {
            this.ResetColors(true);
            this.Start();
        }

        private void ProgramContextReleased(object sender, ProgramContextEventArgs e)
        {
            var programContext = e.ProgramContext;
            if (programContext != null)
            {
                programContext.ProgramStarted -= this.ProgramContextProgramStarted;
                programContext.ProgramEnded -= this.ProgramContextProgramEnded;
                this._programContexts.Remove(programContext);
            }
        }

        private void Setup()
        {
            ViewManager.DisplaySetupView(this.GetDisplayPreviewModuleDataModel());
        }

        private void SetupAppCommandClick(object sender, EventArgs e)
        {
            this.Setup();
        }

        private void Start()
        {
            var dataModel = this.GetDisplayPreviewModuleDataModel();
            if (dataModel.IsEnabled)
            {
                ViewManager.StartVisualizer(dataModel);
            }
        }

        private void Stop()
        {
            if (!this.GetDisplayPreviewModuleDataModel().Preferences.KeepVisualizerWindowOpen)
            {
                EnsureVisualizerIsClosed();
            }
        }
    }
}