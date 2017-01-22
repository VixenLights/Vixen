using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	public class ControllerFacade : IControllerFacadeParticipant
	{
		private OutputDeviceFacade _outputDeviceFacade;
		private List<IControllerFacadeParticipant> _controllerFacadeParticipants;

		public ControllerFacade()
		{
			_outputDeviceFacade = new OutputDeviceFacade();
			_controllerFacadeParticipants = new List<IControllerFacadeParticipant>();
		}

		public void AddParticipant(IControllerFacadeParticipant participant)
		{
			_outputDeviceFacade.AddParticipant(participant);
			_controllerFacadeParticipants.Add(participant);
		}

		public void StartOnly(IEnumerable<IOutputDevice> outputDevices)
		{
			_outputDeviceFacade.StartOnly(outputDevices);
		}

		public void StartAll()
		{
			_outputDeviceFacade.StartAll();
		}

		public void StopAll()
		{
			_outputDeviceFacade.StopAll();
		}

		public void PauseAll()
		{
			_outputDeviceFacade.PauseAll();
		}

		public void ResumeAll()
		{
			_outputDeviceFacade.ResumeAll();
		}

		public IDataFlowComponent GetDataFlowComponentForOutput(IOutputDevice controller, int outputIndex)
		{
			return
				_controllerFacadeParticipants.Select(
					x => x.GetDataFlowComponentForOutput(controller, outputIndex)).FirstOrDefault(x => x != null);
		}

		public IEnumerable<IOutputDevice> Devices
		{
			get { return _outputDeviceFacade.Devices; }
		}

		public IOutputDevice GetDevice(Guid id)
		{
			return _outputDeviceFacade.GetDevice(id);
		}

		public IOutputDevice GetController(Guid id)
		{
			return _outputDeviceFacade.GetDevice(id);
		}
	}
}