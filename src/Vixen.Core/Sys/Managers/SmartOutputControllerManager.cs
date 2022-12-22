﻿using System.Collections;
using Vixen.Data.Flow;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	public class SmartOutputControllerManager : IControllerManager<SmartOutputController>, IControllerFacadeParticipant
	{
		private IControllerLinkManager<SmartOutputController> _linkManager;
		private OutputDeviceCollectionExecutionMediator<SmartOutputController> _mediator;

		internal SmartOutputControllerManager(IControllerLinkManager<SmartOutputController> linkManager,
		                                      IOutputDeviceCollection<SmartOutputController> deviceCollection,
		                                      IOutputDeviceExecution<SmartOutputController> deviceExecution)
		{
			_linkManager = linkManager;
			_mediator = new OutputDeviceCollectionExecutionMediator<SmartOutputController>(deviceCollection, deviceExecution);
		}

		public IOutputDevice GetDevice(Guid id)
		{
			return GetController(id);
		}

		public SmartOutputController GetController(Guid id)
		{
			return _mediator.Get(id);
		}

		public SmartOutputController GetNext(SmartOutputController controller)
		{
			return _linkManager.GetNext(controller);
		}

		public SmartOutputController GetPrior(SmartOutputController controller)
		{
			return _linkManager.GetPrior(controller);
		}

		public IDataFlowComponent GetDataFlowComponentForOutput(IOutputDevice controller, int outputIndex)
		{
			return GetDataFlowComponentForOutput(controller as SmartOutputController, outputIndex);
		}

		public IDataFlowComponent GetDataFlowComponentForOutput(SmartOutputController controller, int outputIndex)
		{
			if (controller != null && outputIndex > 0 && outputIndex < controller.OutputCount) {
				return controller.GetDataFlowComponentForOutput(controller.Outputs[outputIndex]);
			}
			return null;
		}

		public IEnumerable<IOutputDevice> Devices
		{
			get { return this; }
		}

		public void Start(SmartOutputController outputDevice)
		{
			_mediator.Start(outputDevice);
		}

		public void Stop(SmartOutputController outputDevice)
		{
			_mediator.Stop(outputDevice);
		}

		public void Pause(SmartOutputController outputDevice)
		{
			_mediator.Pause(outputDevice);
		}

		public void Resume(SmartOutputController outputDevice)
		{
			_mediator.Resume(outputDevice);
		}

		public void StartOnly(IEnumerable<IOutputDevice> outputDevices)
		{
			StartAll(outputDevices.OfType<SmartOutputController>());
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

		public void StartAll(IEnumerable<SmartOutputController> outputDevices)
		{
			_mediator.StartAll(outputDevices);
		}

		public void StopAll(IEnumerable<SmartOutputController> outputDevices)
		{
			_mediator.StopAll(outputDevices);
		}

		public void PauseAll(IEnumerable<SmartOutputController> outputDevices)
		{
			_mediator.PauseAll(outputDevices);
		}

		public void ResumeAll(IEnumerable<SmartOutputController> outputDevices)
		{
			_mediator.ResumeAll(outputDevices);
		}

		public void Add(SmartOutputController outputDevice)
		{
			_mediator.Add(outputDevice);
		}

		public void AddRange(IEnumerable<SmartOutputController> outputDevices)
		{
			foreach (SmartOutputController outputDevice in outputDevices) {
				Add(outputDevice);
			}
		}

		public bool Remove(SmartOutputController outputDevice)
		{
			return _mediator.Remove(outputDevice);
		}

		public SmartOutputController Get(Guid id)
		{
			return _mediator.Get(id);
		}

		public IEnumerable<SmartOutputController> GetAll()
		{
			return _mediator.GetAll();
		}

		public ExecutionState ExecutionState
		{
			get { return _mediator.ExecutionState; }
		}

		public IEnumerator<SmartOutputController> GetEnumerator()
		{
			return _mediator.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}