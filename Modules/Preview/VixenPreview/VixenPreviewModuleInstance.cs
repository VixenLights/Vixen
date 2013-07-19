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
		private VixenPreviewDisplayD2D displayForm;

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

		protected override Form Initialize()
		{
			Execution.NodesChanged += ExecutionNodesChanged;
			VixenSystem.Contexts.ContextCreated += ProgramContextCreated;
			VixenSystem.Contexts.ContextReleased += ProgramContextReleased;

			displayForm = new VixenPreviewDisplayD2D();
			displayForm.Data = GetDataModel();
			displayForm.Setup();

			return displayForm;
		}

		private VixenPreviewData GetDataModel()
		{
			return ModuleData as VixenPreviewData;
		}

		public override void Start()
		{
			//System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;
			var dataModel = GetDataModel();
			base.Start();
		}

		public override bool Setup()
		{
			setupForm = new VixenPreviewSetup3();
			setupForm.Data = GetDataModel();

			if (displayForm != null)
				displayForm.Scene.IsAnimating = false;



			setupForm.ShowDialog();


			if (setupForm.DialogResult == DialogResult.OK) {
				if (displayForm != null && displayForm.Scene != null) {
					displayForm.Scene.Reload();
				}
			}
			if (displayForm != null)
				displayForm.Scene.IsAnimating = true;


			return base.Setup();
		}

		public override void Dispose()
		{
			if (displayForm != null && !displayForm.Disposing)
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

		protected override void Update()
		{
			try {
				Console.WriteLine("Update");
				displayForm.Scene.ElementStates = ElementStates;

			} catch (Exception e) {

				Console.WriteLine(e.ToString());
			}

		}
	}
}