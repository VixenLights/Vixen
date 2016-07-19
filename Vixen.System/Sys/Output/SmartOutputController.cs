using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Data.Policy;
using Vixen.Module.SmartController;

namespace Vixen.Sys.Output
{
	/// <summary>
	/// In-memory smart controller device.
	/// </summary>
	public class SmartOutputController : ISmartControllerDevice
	{
		private IntentOutputStates _outputCurrentStates;
		private SmartControllerDataPolicy _dataPolicy;
		private IntentOutputDataFlowAdapterFactory _adapterFactory;

		private IOutputMediator<IntentOutput> _outputMediator;
		private IHardware _executionControl;
		private IOutputModuleConsumer<ISmartControllerModuleInstance> _outputModuleConsumer;
		private int? _updateInterval;

		internal SmartOutputController(Guid id, string name, IOutputMediator<IntentOutput> outputMediator,
		                               IHardware executionControl,
		                               IOutputModuleConsumer<ISmartControllerModuleInstance> outputModuleConsumer)
		{
			if (outputMediator == null) throw new ArgumentNullException("outputMediator");
			if (executionControl == null) throw new ArgumentNullException("executionControl");
			if (outputModuleConsumer == null) throw new ArgumentNullException("outputModuleConsumer");

			Id = id;
			Name = name;
			_outputMediator = outputMediator;
			_executionControl = executionControl;
			_outputModuleConsumer = outputModuleConsumer;

			_outputCurrentStates = new IntentOutputStates();
			_dataPolicy = new SmartControllerDataPolicy();
			_adapterFactory = new IntentOutputDataFlowAdapterFactory();
		}

		public Guid Id { get; private set; }

		public string Name { get; set; }

		public Guid ModuleId
		{
			get { return _outputModuleConsumer.ModuleId; }
		}

		public int UpdateInterval
		{
			get { return (_updateInterval.HasValue) ? _updateInterval.Value : _outputModuleConsumer.UpdateInterval; }
			set { _updateInterval = value; }
		}

		public void UpdateCommands()
		{
			//Needs implementation
		}

		public void Update()
		{
			_outputMediator.LockOutputs();
			try {
				Outputs.AsParallel().ForAll(x =>
				                            	{
				                            		x.IntentChangeCollection = _GenerateChangeCollection(x);
				                            	});
				_UpdateModuleState(_ExtractIntentChangesFromOutputs().ToArray());
			}
			finally {
				_outputMediator.UnlockOutputs();
			}
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

		public void AddOutput(IntentOutput output)
		{
			_outputMediator.AddOutput(output);
			VixenSystem.DataFlow.AddComponent(_adapterFactory.GetAdapter(output));
		}

		public void AddOutput(Output output)
		{
			AddOutput((IntentOutput) output);
		}

		public void RemoveOutput(IntentOutput output)
		{
			_outputMediator.RemoveOutput(output);
			VixenSystem.DataFlow.RemoveComponent(_adapterFactory.GetAdapter(output));
		}

		public void RemoveOutput(Output output)
		{
			RemoveOutput((IntentOutput) output);
		}

		public IntentOutput[] Outputs
		{
			get { return _outputMediator.Outputs; }
		}

		Output[] IHasOutputs.Outputs
		{
			get { return Outputs; }
		}

		public int OutputCount
		{
			get { return _outputMediator.OutputCount; }
		}

		public IDataFlowComponent GetDataFlowComponentForOutput(IntentOutput output)
		{
			return _adapterFactory.GetAdapter(output);
		}

		public override string ToString()
		{
			return Name;
		}

		private IEnumerable<IntentChangeCollection> _ExtractIntentChangesFromOutputs()
		{
			return Outputs.Select(x => x.IntentChangeCollection);
		}

		private IntentChangeCollection _GenerateChangeCollection(IntentOutput output)
		{
			_dataPolicy.OutputCurrentState = _outputCurrentStates.GetOutputCurrentState(output);
			output.State.Dispatch(_dataPolicy);
			_outputCurrentStates.SetOutputCurrentState(output, _dataPolicy.OutputCurrentState);
			return _dataPolicy.Result;
		}

		private ISmartController _SmartControllerModule
		{
			get { return _outputModuleConsumer.Module; }
		}

		private void _UpdateModuleState(IntentChangeCollection[] outputStates)
		{
			_SmartControllerModule.UpdateState(outputStates);
		}
	}
}