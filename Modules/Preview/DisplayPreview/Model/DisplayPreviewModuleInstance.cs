namespace VixenModules.Preview.DisplayPreview.Model
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using Vixen.Execution.Context;
	using Vixen.Module.Preview;
	using Vixen.Sys;
	using VixenModules.Preview.DisplayPreview.Views;

	public class DisplayPreviewModuleInstance : FormPreviewModuleInstanceBase
	{
		private readonly List<IProgramContext> _programContexts;
		private ViewManager _viewManager;

		public DisplayPreviewModuleInstance()
		{
			_programContexts = new List<IProgramContext>();
			_viewManager = new ViewManager();
		}

		private ViewManager ViewManager
		{
			get
			{
				if (_viewManager == null) {
					_viewManager = new ViewManager();
				}
				return _viewManager;
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override void Dispose()
		{
			EnsureVisualizerIsClosed();
			_viewManager = null;
			Execution.NodesChanged -= ExecutionNodesChanged;
			VixenSystem.Contexts.ContextCreated -= ProgramContextCreated;
			VixenSystem.Contexts.ContextReleased -= ProgramContextReleased;
			base.Dispose();
		}

		public override bool Setup()
		{
			ViewManager.DisplaySetupView(GetDisplayPreviewModuleDataModel());
			GetDisplayPreviewModuleDataModel().IsEnabled = true;
			if (IsRunning) {
				ViewManager.EnsureVisualizerIsOpen(GetDisplayPreviewModuleDataModel());
			}
			return base.Setup();
		}

		public override void Start()
		{
			var dataModel = GetDisplayPreviewModuleDataModel();
			if (dataModel.IsEnabled) {
				ViewManager.StartVisualizer(dataModel);
			}
			base.Start();
		}

		public override void Stop()
		{
			EnsureVisualizerIsClosed();
			_viewManager = null;
			base.Stop();
		}

		protected override Form Initialize()
		{
			Execution.NodesChanged += ExecutionNodesChanged;
			VixenSystem.Contexts.ContextCreated += ProgramContextCreated;
			VixenSystem.Contexts.ContextReleased += ProgramContextReleased;
			Preferences.CurrentPreferences = GetDisplayPreviewModuleDataModel().Preferences;
			ResetColors(false);
			InjectAppCommands();
			//A dummy form to satisfy the contract. Need to do something with this.
			return new PreviewForm();
		}

		protected override void Update()
		{
			ViewManager.UpdateVisualizerExecutionStateValues(ElementStates);
		}

		private void EnsureVisualizerIsClosed()
		{
			ViewManager.EnsureVisualizerIsClosed();
		}

		private static void ExecutionNodesChanged(object sender, EventArgs e)
		{
			// TODO: Remove any elements that are no longer valid.
		}

		private DisplayPreviewModuleDataModel GetDisplayPreviewModuleDataModel()
		{
			return (DisplayPreviewModuleDataModel) ModuleData;
		}

		private void InjectAppCommands()
		{
			// TODO: This needs to be moved to the form contruction
			//        private const string DISPLAY_PREVIEW_MENU = "DisplayPreview.Menu";
			//        private const string EDIT_PREVIEW_SETUP_MENU = "Edit Preview Setup";
			//        private const string ENABLE_DISPLAY_PREVIEW_MENU = "Preferences window";
			//        private AppCommand _setupCommand;
			//            var isEnabled = GetDisplayPreviewModuleDataModel().IsEnabled;
			//            var rootCommand = new AppCommand(DISPLAY_PREVIEW_MENU, "Display Preview")
			//                              { Enabled = true, Visible = true, Style = AppCommand.AppCommandStyle.Menu };
			//            _setupCommand = new AppCommand(EDIT_PREVIEW_SETUP_MENU, "Edit Preview Setup");
			//            _setupCommand.Click += SetupAppCommandClick;
			//            _setupCommand.Enabled = isEnabled;
			//            _setupCommand.Visible = true;
			//            rootCommand.Add(_setupCommand);
			//
			//            var enabledCommand = new LatchedAppCommand(ENABLE_DISPLAY_PREVIEW_MENU, "Preview Enabled");
			//            enabledCommand.Checked += EnabledCommandChecked;
			//            enabledCommand.Enabled = true;
			//            enabledCommand.Visible = true;
			//            enabledCommand.IsChecked = isEnabled;
			//            rootCommand.Add(enabledCommand);
			//_application.AppCommands.Add(rootCommand);
		}

		private void ProgramContextCreated(object sender, ContextEventArgs contextEventArgs)
		{
			var programContext = contextEventArgs.Context as IProgramContext;
			if (programContext != null) {
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

		private void ProgramContextReleased(object sender, ContextEventArgs contextEventArgs)
		{
			var programContext = contextEventArgs.Context as IProgramContext;
			if (programContext != null) {
				programContext.ProgramStarted -= ProgramContextProgramStarted;
				programContext.ProgramEnded -= ProgramContextProgramEnded;
				_programContexts.Remove(programContext);
			}
		}

		private void ResetColors(bool isRunning)
		{
			var dataModel = GetDisplayPreviewModuleDataModel();
			foreach (var displayItem in dataModel.DisplayItems) {
				displayItem.ResetColor(isRunning);
			}
		}
	}
}