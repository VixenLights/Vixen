using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vixen.Factory;
using Vixen.Data.Flow;
using Vixen.Module.Controller;
using Vixen.Commands;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Output
{
	/// <summary>
	/// In-memory controller device.
	/// </summary>
	public class OutputController : IControllerDevice, IEnumerable<OutputController>
	{
		//Because of bad design, this needs to be created before the base class is instantiated.
		private CommandOutputDataFlowAdapterFactory _adapterFactory = new CommandOutputDataFlowAdapterFactory();
		private IOutputMediator<CommandOutput> _outputMediator;
		private IHardware _executionControl;
		private IOutputModuleConsumer<IControllerModuleInstance> _outputModuleConsumer;
		private int? _updateInterval;
		private IOutputDataPolicyProvider _dataPolicyProvider;
		private ICommand[] commands;
		private Stopwatch _updateStopwatch = new Stopwatch();

		internal OutputController(Guid id, string name, IOutputMediator<CommandOutput> outputMediator,
		                          IHardware executionControl,
		                          IOutputModuleConsumer<IControllerModuleInstance> outputModuleConsumer)
		{
			if (outputMediator == null) throw new ArgumentNullException("outputMediator");
			if (executionControl == null) throw new ArgumentNullException("executionControl");
			if (outputModuleConsumer == null) throw new ArgumentNullException("outputModuleConsumer");

			Id = id;
			Name = name;
			_outputMediator = outputMediator;
			_executionControl = executionControl;
			_outputModuleConsumer = outputModuleConsumer;

			_dataPolicyProvider = new OutputDataPolicyCache();
			_dataPolicyProvider.UseFactory(_ControllerModule.DataPolicyFactory);

			_ControllerModule.DataPolicyFactoryChanged += DataPolicyFactoryChanged;
		}

		private void DataPolicyFactoryChanged(object sender, EventArgs eventArgs)
		{
			_dataPolicyProvider.UseFactory(_ControllerModule.DataPolicyFactory);
		}

		public IDataFlowComponent GetDataFlowComponentForOutput(CommandOutput output)
		{
			return _adapterFactory.GetAdapter(output);
		}

		#region IEnumerable<OutputController>

		public IEnumerator<OutputController> GetEnumerator()
		{
			if (VixenSystem.ControllerLinking.IsRootController(this)) {
				return new ChainEnumerator(this);
			}
			return Enumerable.Empty<OutputController>().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region class ChainEnumerator

		private class ChainEnumerator : IEnumerator<OutputController>
		{
			private OutputController _root;
			private OutputController _current;
			private OutputController _next;

			public ChainEnumerator(OutputController root)
			{
				_root = root;
				Reset();
			}

			public OutputController Current
			{
				get { return _current; }
			}

			public void Dispose()
			{
			}

			object System.Collections.IEnumerator.Current
			{
				get { return _current; }
			}

			public bool MoveNext()
			{
				if (_next != null) {
					_current = _next;
					_next = VixenSystem.OutputControllers.GetNext(_current);
					return true;
				}
				return false;
			}

			public void Reset()
			{
				_current = null;
				_next = _root;
			}
		}

		#endregion

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

		// for instrumentation support
		private long _generateMs;
		private long _extractMs;
		private long _deviceMs;
		public void GetLastUpdateMs(out long generateMs, out long extractMs, out long deviceMs)
		{
			generateMs = _generateMs;
			extractMs = _extractMs;
			deviceMs = _deviceMs;
		}

		/// <summary>
		/// Just update the commands and don't send them out
		/// </summary>
		public void UpdateCommands()
		{
			if (VixenSystem.ControllerLinking.IsRootController(this) && _ControllerChainModule != null)
			{
				_outputMediator.LockOutputs();
				try
				{
					foreach (OutputController controller in this)
					{
						foreach (var x in controller.Outputs)
						{
							x.Command = _GenerateOutputCommand(x);
						}
					}
				} finally
				{
					_outputMediator.UnlockOutputs();
				}
			}

		}
	
		public void Update()
		{
			_updateStopwatch.Restart();
			if (VixenSystem.ControllerLinking.IsRootController(this) && _ControllerChainModule != null) {
				_outputMediator.LockOutputs();
				try {
					foreach (OutputController controller in this) {
						foreach( var x in controller.Outputs)
						{
							x.Command = _GenerateOutputCommand(x);
						}
					}

					_generateMs = _updateStopwatch.ElapsedMilliseconds;
					_extractMs = 0;
					_deviceMs = 0;

					// Latch out the new state.
					// This must be done in order of the chain links so that data
					// goes out the port in the correct order.
					foreach (OutputController controller in this) {
						// A single port may be used to service multiple physical controllers,
						// such as daisy-chained Renard controllers.  Tell the module where
						// it is in that chain.
						long t1 = _updateStopwatch.ElapsedMilliseconds;
						int chainIndex = VixenSystem.ControllerLinking.GetChainIndex(controller.Id);
						ICommand[] outputStates = _ExtractCommandsFromOutputs(controller);
						long t2 = _updateStopwatch.ElapsedMilliseconds;
						controller._ControllerChainModule.UpdateState(chainIndex, outputStates);
						long t3 = _updateStopwatch.ElapsedMilliseconds;
						_extractMs += t2 - t1;
						_deviceMs += t3 - t2;
						
					}
				}
				finally {
					_outputMediator.UnlockOutputs();
				}

				_updateStopwatch.Stop();
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

		public int OutputCount
		{
			get { return _outputMediator.OutputCount; }
			// A nicety not enforced by the interface.
			set
			{
				CommandOutputFactory outputFactory = new CommandOutputFactory();
				while (OutputCount < value) {
					AddOutput(outputFactory.CreateOutput(string.Format("Output {0}", (OutputCount + 1).ToString()), OutputCount));
				}
				while (OutputCount > value) {
					RemoveOutput(Outputs[OutputCount - 1]);
				}
				commands = null;
			}
		}

		public void AddOutput(CommandOutput output)
		{
			_outputMediator.AddOutput(output);
			IDataFlowComponent component = _adapterFactory.GetAdapter(output);
			VixenSystem.DataFlow.AddComponent(component);
			VixenSystem.OutputControllers.AddControllerOutputForDataFlowComponent(component, this, output.Index);
			commands = null;
		}

		public void AddOutput(Output output)
		{
			AddOutput((CommandOutput) output);
		}

		public void RemoveOutput(CommandOutput output)
		{
			_outputMediator.RemoveOutput(output);
			IDataFlowComponent component = _adapterFactory.GetAdapter(output);
			VixenSystem.DataFlow.RemoveComponent(component);
			VixenSystem.OutputControllers.RemoveControllerOutputForDataFlowComponent(component);
			commands = null;
		}

		public void RemoveOutput(Output output)
		{
			RemoveOutput((CommandOutput) output);
		}

		public CommandOutput[] Outputs
		{
			get { return _outputMediator.Outputs; }
		}

		Output[] IHasOutputs.Outputs
		{
			get { return Outputs; }
		}

		public override string ToString()
		{
			return Name;
		}

		private ICommand[]_ExtractCommandsFromOutputs(OutputController controller)
		{
			if (commands == null)
			{
				commands = new ICommand[OutputCount];
			}

			for (int i=0; i < controller.Outputs.Length; i++)
			{
				commands[i] = controller.Outputs[i].Command;
			}
			//return controller.Outputs.Select(x => x.Command);

			return commands;
		}

		private ICommand _GenerateOutputCommand(CommandOutput output)
		{
			if (output.State != null) {

				IDataPolicy effectiveDataPolicy = _dataPolicyProvider.GetDataPolicyForOutput(output);
				return effectiveDataPolicy.GenerateCommand(output.State);
			}
			return null;
		}

		private IControllerModuleInstance _ControllerModule
		{
			get { return _outputModuleConsumer.Module; }
		}

		private IControllerModuleInstance _ControllerChainModule
		{
			get
			{
				// When output controllers are linked, only the root controller will be
				// connected to the port, therefore only it will have the output module
				// used during execution.
				OutputController priorController = VixenSystem.OutputControllers.GetPrior(this);
				return (priorController != null) ? priorController._ControllerChainModule : _ControllerModule;
			}
		}
	}
}