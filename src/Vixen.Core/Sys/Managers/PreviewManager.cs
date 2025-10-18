using System.Collections;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	public class PreviewManager : IOutputDeviceManager<OutputPreview>, IOutputDeviceFacadeParticipant
	{
		private OutputDeviceCollectionExecutionMediator<OutputPreview> _mediator;

		internal PreviewManager(IOutputDeviceCollection<OutputPreview> deviceCollection,
		                        IOutputDeviceExecution<OutputPreview> deviceExecution)
		{
			_mediator = new OutputDeviceCollectionExecutionMediator<OutputPreview>(deviceCollection, deviceExecution);
		}

		public IOutputDevice GetDevice(Guid id)
		{
			return _mediator.Get(id);
		}

		public IEnumerable<IOutputDevice> Devices
		{
			get { return this; }
		}

		public void Start(OutputPreview outputDevice)
		{
			_mediator.Start(outputDevice);
		}

		public void Stop(OutputPreview outputDevice)
		{
			_mediator.Stop(outputDevice);
		}

		public void Pause(OutputPreview outputDevice)
		{
			_mediator.Pause(outputDevice);
		}

		public void Resume(OutputPreview outputDevice)
		{
			_mediator.Resume(outputDevice);
		}

		public void StartAll()
		{
			_mediator.StartAll();
		}

		public void StopAll()
		{
			_mediator.StopAll();
		}

		public void PauseAll()
		{
			_mediator.PauseAll();
		}

		public void ResumeAll()
		{
			_mediator.ResumeAll();
		}

		public void StartOnly(IEnumerable<IOutputDevice> outputDevices)
		{
			StartAll(outputDevices.OfType<OutputPreview>());
		}

		public void StartAll(IEnumerable<OutputPreview> outputDevices)
		{
			_mediator.StartAll(outputDevices);
		}

		public void StopAll(IEnumerable<OutputPreview> outputDevices)
		{
			_mediator.StopAll(outputDevices);
		}

		public void PauseAll(IEnumerable<OutputPreview> outputDevices)
		{
			_mediator.PauseAll(outputDevices);
		}

		public void ResumeAll(IEnumerable<OutputPreview> outputDevices)
		{
			_mediator.ResumeAll(outputDevices);
		}

		public void Add(OutputPreview outputDevice)
		{
			_mediator.Add(outputDevice);
		}

		public void AddRange(IEnumerable<OutputPreview> outputDevices)
		{
			foreach (OutputPreview outputDevice in outputDevices) {
				Add(outputDevice);
			}
		}

		public bool Remove(OutputPreview outputDevice)
		{
			RemoveDataModel(outputDevice);
			return _mediator.Remove(outputDevice);
		}

		public OutputPreview Get(Guid id)
		{
			return _mediator.Get(id);
		}

		public IEnumerable<OutputPreview> GetAll()
		{
			return _mediator.GetAll();
		}

		public ExecutionState ExecutionState
		{
			get { return _mediator.ExecutionState; }
		}

		public IEnumerator<OutputPreview> GetEnumerator()
		{
			return _mediator.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

        private void RemoveDataModel(OutputPreview outputPreview)
        {
            VixenSystem.ModuleStore.InstanceData.RemoveModuleInstanceData(outputPreview.PreviewModule as IModuleInstance);
        }
    }
}