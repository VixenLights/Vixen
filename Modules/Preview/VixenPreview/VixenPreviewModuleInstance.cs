using System;
using System.Windows.Forms;
using System.Diagnostics;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview
{
    public partial class VixenPreviewModuleInstance : FormPreviewModuleInstanceBase
    {
        VixenPreviewSetup3 setupForm;
        VixenPreviewDisplay displayForm;

        public VixenPreviewModuleInstance()
        {
        }

        private void VixenPreviewModuleInstance_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Load");
        }

        public override void Stop()
        {
            Console.WriteLine("Stop");
            base.Stop();
        }

        public override void Resume()
        {
            Console.WriteLine("Resume");
            base.Resume();
        }

        public override void Pause()
        {
            Console.WriteLine("Pause");
            base.Pause();
        }

        public override bool IsRunning
        {
            get
            {
                return base.IsRunning;
            }
        }

        public override bool HasSetup
        {
            get
            {
                return base.HasSetup;
            }
        }

        protected override Form Initialize()
        {
            Console.WriteLine("Initialize");
            //Execution.NodesChanged += ExecutionNodesChanged;
            VixenSystem.Contexts.ContextCreated += ProgramContextCreated;
            VixenSystem.Contexts.ContextReleased += ProgramContextReleased;

            displayForm = new VixenPreviewDisplay();
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
            Console.WriteLine("Start");
            var dataModel = GetDataModel();
            base.Start();
        }

        public override bool Setup()
        {
            Console.WriteLine("Setup");
            setupForm = new VixenPreviewSetup3();
            setupForm.Data = GetDataModel();
            //setupForm.Setup();
            if (displayForm != null) 
                displayForm.PreviewControl.Paused = true;
            Console.WriteLine("Paused");
            setupForm.ShowDialog();
            //displayForm.PreviewControl.Reload();
            if (displayForm != null)
                displayForm.PreviewControl.Paused = false;
            Console.WriteLine("Un-Paused");
            if (setupForm.DialogResult == DialogResult.OK)
            {
                if (displayForm != null) 
                    displayForm.PreviewControl.Reload();
            }
            //return setupForm;
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

        private void ProgramContextCreated(object sender, ContextEventArgs contextEventArgs)
        {
            var programContext = contextEventArgs.Context as IProgramContext;
            //
            // This is always null... why does this event get called?
            //
            if (programContext != null)
            {
                //_programContexts.Add(programContext);
                programContext.ProgramStarted += ProgramContextProgramStarted;
                programContext.ProgramEnded += ProgramContextProgramEnded;
                programContext.SequenceStarted += context_SequenceStarted;
                programContext.SequenceEnded += context_SequenceEnded;

                Console.WriteLine("Context Created");
            }
        }

        private void ProgramContextProgramEnded(object sender, ProgramEventArgs e)
        {
            //ResetColors(false);
            Console.WriteLine("Stopped");
            Stop();
        }

        private void ProgramContextProgramStarted(object sender, ProgramEventArgs e)
        {
            Console.WriteLine("ProgramContextProgramStarted");
            Start();
        }

        protected void context_SequenceStarted(object sender, SequenceStartedEventArgs e)
        {
            Console.WriteLine("Squence Started");
            //displayForm.PreviewControl.ResetNodeToPixelDictionary();
        }

        protected void context_SequenceEnded(object sender, SequenceEventArgs e)
        {
            //Nada
        }

        private void ProgramContextReleased(object sender, ContextEventArgs contextEventArgs)
        {
            Console.WriteLine("Context Released");
            var programContext = contextEventArgs.Context as IProgramContext;
            if (programContext != null)
            {
                programContext.ProgramStarted -= ProgramContextProgramStarted;
                programContext.ProgramEnded -= ProgramContextProgramEnded;
                programContext.SequenceStarted -= context_SequenceStarted;
                programContext.SequenceEnded -= context_SequenceEnded;
                //_programContexts.Remove(programContext);
            }
        }

        protected override void Update()
        {
            ProcessUpdate(ElementStates);
        }

        delegate void ProcessUpdateDelegate(ElementIntentStates elementStates);
        private void ProcessUpdate(ElementIntentStates elementStates)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            //displayForm.PreviewControl.ProcessUpdate(elementStates);
            displayForm.PreviewControl.BeginInvoke(new ProcessUpdateDelegate(displayForm.PreviewControl.ProcessUpdate), new object[] {elementStates});

            timer.Stop();

            VixenPreviewControl.updateCount += 1;
            VixenPreviewControl.lastUpdateTime = timer.ElapsedMilliseconds;
            VixenPreviewControl.totalUpdateTime += timer.ElapsedMilliseconds;
            VixenPreviewControl.averageUpdateTime = VixenPreviewControl.totalUpdateTime / VixenPreviewControl.updateCount;        }
    }
}
