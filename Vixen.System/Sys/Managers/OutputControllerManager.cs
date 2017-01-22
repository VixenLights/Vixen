using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	public class OutputControllerManager : IControllerManager<OutputController>, IControllerFacadeParticipant
	{
		//Logger Class
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private OutputDeviceCollectionExecutionMediator<OutputController> _mediator;

		private Dictionary<IDataFlowComponent, Tuple<IControllerDevice, int>> _outputDataFlowComponentToController;

		internal OutputControllerManager(IOutputDeviceCollection<OutputController> deviceCollection,
		                                 IOutputDeviceExecution<OutputController> deviceExecution)
		{
			_mediator = new OutputDeviceCollectionExecutionMediator<OutputController>(deviceCollection, deviceExecution);
			_outputDataFlowComponentToController = new Dictionary<IDataFlowComponent, Tuple<IControllerDevice, int>>();
		}

		public IOutputDevice GetDevice(Guid id)
		{
			return GetController(id);
		}

		public OutputController GetController(Guid id)
		{
			return _mediator.Get(id);
		}

		public IDataFlowComponent GetDataFlowComponentForOutput(IOutputDevice controller, int outputIndex)
		{
			return GetDataFlowComponentForOutput(controller as OutputController, outputIndex);
		}

		public IDataFlowComponent GetDataFlowComponentForOutput(OutputController controller, int outputIndex)
		{
			if (controller != null && outputIndex > 0 && outputIndex < controller.OutputCount) {
				return controller.GetDataFlowComponentForOutput(controller.Outputs[outputIndex]);
			}
			return null;
		}

		public void AddControllerOutputForDataFlowComponent(IDataFlowComponent component, IControllerDevice controller, int outputIndex)
		{
			if (_outputDataFlowComponentToController.ContainsKey(component)) {
				Logging.Error("map already contains link for component: " + component.Name);
			}

			_outputDataFlowComponentToController[component] = new Tuple<IControllerDevice, int>(controller, outputIndex);
		}

		public bool RemoveControllerOutputForDataFlowComponent(IDataFlowComponent component)
		{
			if (component == null)
				return false;

			return _outputDataFlowComponentToController.Remove(component);
		}

		public bool getOutputDetailsForDataFlowComponent(IDataFlowComponent component, out IControllerDevice controller, out int outputIndex)
		{
			controller = null;
			outputIndex = 0;

			if (component == null)
				return false;

			Tuple<IControllerDevice, int> data;
			bool result = _outputDataFlowComponentToController.TryGetValue(component, out data);

			if (data != null) {
				controller = data.Item1;
				outputIndex = data.Item2;
			}

			return result;
		}


		public IEnumerable<IOutputDevice> Devices
		{
			get { return this; }
		}

		public void Start(OutputController outputDevice)
		{
			_mediator.Start(outputDevice);
		}

		public void Stop(OutputController outputDevice)
		{
			_mediator.Stop(outputDevice);
		}

		public void Pause(OutputController outputDevice)
		{
			_mediator.Pause(outputDevice);
		}

		public void Resume(OutputController outputDevice)
		{
			_mediator.Resume(outputDevice);
		}

		public void StartOnly(IEnumerable<IOutputDevice> outputDevices)
		{
			StartAll(outputDevices.OfType<OutputController>());
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

		public void StartAll(IEnumerable<OutputController> outputDevices)
		{
			_mediator.StartAll(outputDevices);
		}

		public void StopAll(IEnumerable<OutputController> outputDevices)
		{
			_mediator.StopAll(outputDevices);
		}

		public void PauseAll(IEnumerable<OutputController> outputDevices)
		{
			_mediator.PauseAll(outputDevices);
		}

		public void ResumeAll(IEnumerable<OutputController> outputDevices)
		{
			_mediator.ResumeAll(outputDevices);
		}

		public void Add(OutputController outputDevice)
		{
			_mediator.Add(outputDevice);
		}

		public void AddRange(IEnumerable<OutputController> outputDevices)
		{
			foreach (OutputController outputDevice in outputDevices) {
				Add(outputDevice);
			}
		}

		public bool Remove(OutputController outputDevice)
		{
			Stop(outputDevice);
			outputDevice.OutputCount = 0;		// "delete" all the outputs to ensure data flow links are discarded
			return _mediator.Remove(outputDevice);
		}

		public OutputController Get(Guid id)
		{
			return _mediator.Get(id);
		}

		public IEnumerable<OutputController> GetAll()
		{
			return _mediator.GetAll();
		}

		public ExecutionState ExecutionState
		{
			get { return _mediator.ExecutionState; }
		}

		public IEnumerator<OutputController> GetEnumerator()
		{
			return _mediator.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}