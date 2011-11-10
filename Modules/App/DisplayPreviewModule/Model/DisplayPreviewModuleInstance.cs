namespace Vixen.Modules.DisplayPreviewModule.Model
{
    using System;
    using System.Collections.Generic;
    using Vixen.Execution;
    using Vixen.Module.App;
    using Vixen.Modules.DisplayPreviewModule.Views;
    using Vixen.Sys;

    public class DisplayPreviewModuleInstance : AppModuleInstanceBase
    {
        private readonly List<ProgramContext> _programContexts;
        private IApplication _application;

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

        private static void ProgramContextProgramEnded(object sender, ProgramEventArgs e)
        {
            Stop();
        }

        private static void Stop()
        {
            EnsureVisualizerIsClosed();
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

            var rootCommand = new AppCommand("Setup Preview Module", "Display Preview")
                              {
                                  Enabled = true, 
                                  Visible = true, 
                                  Style = AppCommand.AppCommandStyle.Menu
                              };            
            var command = new AppCommand("Edit Preview Setup", "Edit Preview Setup");
            command.Click += SetupAppCommandClick;
            command.Enabled = true;
            command.Visible = true;
            rootCommand.Add(command);
            command = new AppCommand("Toggle preview window", "Toggle preview window");
            command.Click += TogglePreviewWindowCommandClick;
            command.Enabled = true;
            command.Visible = true;
            rootCommand.Add(command);
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
            ViewManager.StartVisualizer(GetDisplayPreviewModuleDataModel());
        }

        private void TogglePreviewWindowCommandClick(object sender, EventArgs e)
        {
            if (ViewManager.IsVisualizerRunning)
            {
                ViewManager.EnsureVisualizerIsClosed();
            }
            else
            {
                ViewManager.StartVisualizer(GetDisplayPreviewModuleDataModel());
            }
        }
    }
}
