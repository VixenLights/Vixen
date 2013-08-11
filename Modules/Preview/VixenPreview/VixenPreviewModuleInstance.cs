using System;
using System.Windows.Forms;
using System.Diagnostics;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Direct2D;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewModuleInstance : FormPreviewModuleInstanceBase
	{
		private VixenPreviewSetup3 setupForm;
		private IDisplayForm displayForm;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public VixenPreviewModuleInstance()
		{
		}

		private void VixenPreviewModuleInstance_Load(object sender, EventArgs e)
		{
		}

		public override void Stop()
		{
			base.Stop();
		}

		public override void Resume()
		{
			base.Resume();
		}

		public override void Pause()
		{
			base.Pause();
		}

		public override bool IsRunning
		{
			get { return base.IsRunning; }
		}

		public override bool HasSetup
		{
			get { return base.HasSetup; }
		}

		public override Vixen.Module.IModuleDataModel ModuleData
		{
			get
			{
				if (base.ModuleData == null) {
					base.ModuleData = new VixenPreviewData();
					Logging.Warn("VixenPreview: access of null ModuleData. Creating new one. (Thread ID: " +
					                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				}
				return base.ModuleData;
			}
			set
			{
				base.ModuleData = value;
			}
		}

		public bool UseGDIPreviewRendering
		{
			get {
				 
				if (new Properties.Settings().UseGDIRendering)
					return true;

				return !Vixen.Sys.VixenSystem.VersionBeyondWindowsXP;
			}
		}

		protected override Form Initialize()
		{
			Execution.NodesChanged += ExecutionNodesChanged;
			VixenSystem.Contexts.ContextCreated += ProgramContextCreated;
			VixenSystem.Contexts.ContextReleased += ProgramContextReleased;
 
			SetupPreviewForm();

			return (Form)displayForm;
		}

		private object formLock = new object();
		private void SetupPreviewForm()
		{
			lock (formLock) {
			
				if (UseGDIPreviewRendering)
					displayForm = new VixenPreviewDisplay();
				else
					displayForm = new VixenPreviewDisplayD2D();

				displayForm.Data = GetDataModel();
				displayForm.Setup();
			}
		}

		private VixenPreviewData GetDataModel()
		{
			return ModuleData as VixenPreviewData;
		}

		public override void Start()
		{
			//System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;
			base.Start();
		}

		public override bool Setup()
		{
			setupForm = new VixenPreviewSetup3();
			setupForm.Data = GetDataModel();

			setupForm.ShowDialog();

			if (displayForm != null)
				displayForm.Data = GetDataModel();

			return base.Setup();
		}

		public override void Dispose()
		{
			if (displayForm != null)
				displayForm.Close();
			VixenSystem.Contexts.ContextCreated -= ProgramContextCreated;
			VixenSystem.Contexts.ContextReleased -= ProgramContextReleased;
			base.Dispose();
		}

		private void ExecutionNodesChanged(object sender, EventArgs e)
		{
			//Console.WriteLine("hanged");
			//if (setupForm != null)
			//{
			//    setupForm.elementsForm.PopulateElementTree();
			//}
		}

		private void ProgramContextCreated(object sender, ContextEventArgs contextEventArgs)
		{
			var programContext = contextEventArgs.Context as IProgramContext;
			//
			// This is always null... why does this event get called?
			//
			if (programContext != null) {
				//_programContexts.Add(programContext);
				programContext.ProgramStarted += ProgramContextProgramStarted;
				programContext.ProgramEnded += ProgramContextProgramEnded;
				programContext.SequenceStarted += context_SequenceStarted;
				programContext.SequenceEnded += context_SequenceEnded;
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

		protected void context_SequenceStarted(object sender, SequenceStartedEventArgs e)
		{
		}

		protected void context_SequenceEnded(object sender, SequenceEventArgs e)
		{
		}

		private void ProgramContextReleased(object sender, ContextEventArgs contextEventArgs)
		{
			var programContext = contextEventArgs.Context as IProgramContext;
			if (programContext != null) {
				programContext.ProgramStarted -= ProgramContextProgramStarted;
				programContext.ProgramEnded -= ProgramContextProgramEnded;
				programContext.SequenceStarted -= context_SequenceStarted;
				programContext.SequenceEnded -= context_SequenceEnded;
			}
		}

		bool isGdiVersion = false;
		protected override void Update()
		{
			try {
				// displayForm.Scene.ElementStates = ElementStates;
				//if the Preview form style changes re-setup the form
				if ((UseGDIPreviewRendering && !isGdiVersion) || (!UseGDIPreviewRendering && isGdiVersion) || displayForm == null) {
					SetupPreviewForm();
					isGdiVersion = UseGDIPreviewRendering;
					Stop();
					Start();
				}

				if (!UseGDIPreviewRendering) {
					((VixenPreviewDisplayD2D)displayForm).Scene.Update(ElementStates);
				}
				else {
					((VixenPreviewDisplay)displayForm).PreviewControl.ProcessUpdateParallel(ElementStates);
				}
			}
			catch (Exception e) {

				Console.WriteLine(e.ToString());
			}

		}
	}
}