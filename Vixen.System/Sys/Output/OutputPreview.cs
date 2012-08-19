using System;
using System.Linq;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Module.Preview;

namespace Vixen.Sys.Output {
	/// <summary>
	/// In-memory preview device.
	/// </summary>
	public class OutputPreview : IPreviewDevice {
		private IHardware _executionControl;
		private IOutputModuleConsumer _outputModuleConsumer;
		private int? _updateInterval;

		internal OutputPreview(Guid id, string name, IHardware executionControl, IOutputModuleConsumer outputModuleConsumer) {
			if(executionControl == null) throw new ArgumentNullException("executionControl");
			if(outputModuleConsumer == null) throw new ArgumentNullException("outputModuleConsumer");

			Id = id;
			Name = name;
			_executionControl = executionControl;
			_outputModuleConsumer = outputModuleConsumer;
		}

		public void Update() {
			ChannelCommands channelCommands = new ChannelCommands(VixenSystem.Channels.ToDictionary(x => x.Id, x => DataPolicy.GenerateCommand(new IntentsDataFlowData(x.State))));
			_UpdateModuleState(channelCommands);
		}

		public IDataPolicy DataPolicy {
			get { return _PreviewModule.DataPolicy; }
		}

		public Guid Id { get; private set; }

		public string Name { get; set; }

		public Guid ModuleId {
			get { return _outputModuleConsumer.ModuleId; }
		}

		public int UpdateInterval {
			get { return (_updateInterval.HasValue) ? _updateInterval.Value : _outputModuleConsumer.UpdateInterval; }
			set { _updateInterval = value; }
		}

		public IOutputDeviceUpdateSignaler UpdateSignaler {
			get { return _outputModuleConsumer.UpdateSignaler; }
		}

		public void Start() {
			_executionControl.Start();
		}

		public void Stop() {
			_executionControl.Stop();
		}

		public void Pause() {
			_executionControl.Pause();
		}

		public void Resume() {
			_executionControl.Resume();
		}

		public bool IsRunning {
			get { return _executionControl.IsRunning; }
		}

		public bool IsPaused {
			get { return _executionControl.IsPaused; }
		}

		public bool HasSetup {
			get { return _outputModuleConsumer.HasSetup; }
		}

		public bool Setup() {
			return _outputModuleConsumer.Setup();
		}

		private IPreview _PreviewModule {
			get { return (IPreview)_outputModuleConsumer.Module; }
		}

		private void _UpdateModuleState(ChannelCommands channelCommands) {
			_PreviewModule.UpdateState(channelCommands);
		}
	}
}
