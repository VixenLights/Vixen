using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers
{
	public class OutputDeviceFacade : IOutputDeviceFacadeParticipant
	{
		private List<IOutputDeviceFacadeParticipant> _participants;

		public OutputDeviceFacade()
		{
			_participants = new List<IOutputDeviceFacadeParticipant>();
		}

		public void AddParticipant(IOutputDeviceFacadeParticipant participant)
		{
			_participants.Add(participant);
		}

		public void StartOnly(IEnumerable<IOutputDevice> outputDevices)
		{
			_ForAllParticipants(x => x.StartOnly(outputDevices));
		}

		public void StartAll()
		{
			_ForAllParticipants(x => x.StartAll());
		}

		public void StopAll()
		{
			_ForAllParticipants(x => x.StopAll());
		}

		public void PauseAll()
		{
			_ForAllParticipants(x => x.PauseAll());
		}

		public void ResumeAll()
		{
			_ForAllParticipants(x => x.ResumeAll());
		}

		public IEnumerable<IOutputDevice> Devices
		{
			get { return _participants.SelectMany(x => x.Devices); }
		}

		public IOutputDevice GetDevice(Guid id)
		{
			return _participants.Select(x => x.GetDevice(id)).FirstOrDefault(x => x != null);
		}

		private void _ForAllParticipants(Action<IOutputDeviceFacadeParticipant> action)
		{
			foreach (IOutputDeviceFacadeParticipant participant in _participants) {
				action(participant);
			}
		}
	}
}