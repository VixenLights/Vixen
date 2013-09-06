using System;
using System.Linq;
using Vixen.Module.Preview;

namespace Vixen.Sys.Output
{
	/// <summary>
	/// In-memory preview device.
	/// </summary>
	public class OutputPreview : IPreviewDevice
	{
		private IHardware _executionControl;
		private IOutputModuleConsumer<IPreviewModuleInstance> _outputModuleConsumer;
		private int? _updateInterval;

		internal OutputPreview(Guid id, string name, IHardware executionControl,
		                       IOutputModuleConsumer<IPreviewModuleInstance> outputModuleConsumer)
		{
			if (executionControl == null) throw new ArgumentNullException("executionControl");
			if (outputModuleConsumer == null) throw new ArgumentNullException("outputModuleConsumer");

			Id = id;
			Name = name;
			_executionControl = executionControl;
			_outputModuleConsumer = outputModuleConsumer;
		}

		public void Update()
		{
			Vixen.Preview.PreviewElementIntentStates elementIntentStates =
				new Vixen.Preview.PreviewElementIntentStates(VixenSystem.Elements.ToDictionary(x => x, x => x.State));
			_UpdateModuleState(elementIntentStates);

			//ElementIntentStates elementIntentStates =
			//	new ElementIntentStates(VixenSystem.Elements.ToDictionary(x => x.Id, x => x.State));
			//_UpdateModuleState(elementIntentStates);
		}

		public Guid Id { get; private set; }

		public string Name { get; set; }

		public Guid ModuleId
		{
			get { return _outputModuleConsumer.ModuleId; }
		}

		public Guid ModuleInstanceId
		{
			get { return _outputModuleConsumer.ModuleInstanceId; }
		}

		public int UpdateInterval
		{
			get { return (_updateInterval.HasValue) ? _updateInterval.Value : _outputModuleConsumer.UpdateInterval; }
			set { _updateInterval = value; }
		}

		public IOutputDeviceUpdateSignaler UpdateSignaler
		{
			get { return _outputModuleConsumer.UpdateSignaler; }
		}

		public void Start()
		{
			_executionControl.Start();
		}

		public void Stop()
		{
			_executionControl.Stop();
		}

		public void Pause()
		{
			_executionControl.Pause();
		}

		public void Resume()
		{
			_executionControl.Resume();
		}

		public bool IsRunning
		{
			get { return _executionControl.IsRunning; }
		}

		public bool IsPaused
		{
			get { return _executionControl.IsPaused; }
		}

		public bool HasSetup
		{
			get { return _outputModuleConsumer.HasSetup; }
		}

		public bool Setup()
		{
			return _outputModuleConsumer.Setup();
		}

		public override string ToString()
		{
			return Name;
		}

		private IPreview _PreviewModule
		{
			get { return _outputModuleConsumer.Module; }
		}

		private void _UpdateModuleState(Vixen.Preview.PreviewElementIntentStates elementIntentStates)
		{
			_PreviewModule.UpdateState(elementIntentStates);
		}
	}
}