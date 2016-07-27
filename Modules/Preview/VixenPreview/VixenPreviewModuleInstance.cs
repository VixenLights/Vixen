using System;
using System.Windows.Forms;
using System.Diagnostics;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;
using VixenModules.Preview.VixenPreview.Direct2D;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewModuleInstance
	{
		private VixenPreviewSetup3 setupForm;
		private IDisplayForm displayForm;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private MillisecondsValue _updateTimeValue = new MillisecondsValue("Update time for preview");

		public VixenPreviewModuleInstance()
		{
			VixenSystem.Instrumentation.AddValue(_updateTimeValue);
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
				 
				// if (new Properties.Settings().UseGDIRendering)
					return true;

				// return !Vixen.Sys.VixenSystem.VersionBeyondWindowsXP;
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
				{
					displayForm = new GDIPreviewForm(GetDataModel());
				}
				else
				{
					displayForm = new VixenPreviewDisplayD2D();
					displayForm.Data = GetDataModel();
				}

				displayForm.Setup();
			}
		}

		private VixenPreviewData GetDataModel()
		{
			return ModuleData as VixenPreviewData;
		}

		public override bool Setup()
		{
			setupForm = new VixenPreviewSetup3();
			setupForm.Data = GetDataModel();

			setupForm.ShowDialog();

			if (displayForm != null)
			{
				displayForm.Data = GetDataModel();
				displayForm.Setup();
			}

			return base.Setup();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (displayForm != null)
					displayForm.Close();
				VixenSystem.Contexts.ContextCreated -= ProgramContextCreated;
				VixenSystem.Contexts.ContextReleased -= ProgramContextReleased;	
			}
			
			base.Dispose(disposing);
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

		//bool isGdiVersion = false;
		protected override void Update()
		{
			var sw = Stopwatch.StartNew();
			try {
				// displayForm.Scene.ElementStates = ElementStates;
				//if the Preview form style changes re-setup the form
				//if ((UseGDIPreviewRendering && !isGdiVersion) || (!UseGDIPreviewRendering && isGdiVersion) || displayForm == null) {
				//	SetupPreviewForm();
				//	isGdiVersion = UseGDIPreviewRendering;
				//	Stop();
				//	Start();
				//}

				//if (!UseGDIPreviewRendering) {
				//	((VixenPreviewDisplayD2D)displayForm).Scene.Update(/*ElementStates*/);
				//}
				//else {
				//	if (UseOldPreview)
				//		((VixenPreviewDisplay)displayForm).PreviewControl.ProcessUpdateParallel(/*ElementStates*/);
				//	else
					displayForm.UpdatePreview();
				//}
			}
			catch (Exception e) {
				Logging.Error("Exception in preview update {0} - {1}", e.Message, e.StackTrace);
				//Console.WriteLine(e.ToString());
			}
			_updateTimeValue.Set(sw.ElapsedMilliseconds);
		}
	}
}