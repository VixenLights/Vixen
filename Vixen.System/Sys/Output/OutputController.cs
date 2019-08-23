using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
	public class OutputController : IControllerDevice
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		//Because of bad design, this needs to be created before the base class is instantiated.
		private readonly CommandOutputDataFlowAdapterFactory _adapterFactory = new CommandOutputDataFlowAdapterFactory();
		private readonly IOutputMediator<CommandOutput> _outputMediator;
		private readonly IHardware _executionControl;
		private readonly IOutputModuleConsumer<IControllerModuleInstance> _outputModuleConsumer;
		private int? _updateInterval;
		private readonly Stopwatch _updateStopwatch = new Stopwatch();
		private IDataPolicy _dataPolicy;
		private MillisecondsValue _updateTimeValue;
		private ICommand[] commands = new ICommand[0];
		
		private ParallelOptions _parallelOptions = new ParallelOptions()
		{
			MaxDegreeOfParallelism = Environment.ProcessorCount
		};
		

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

			_dataPolicy = ControllerModule.DataPolicyFactory.CreateDataPolicy();

			ControllerModule.DataPolicyFactoryChanged += DataPolicyFactoryChanged;
		}

		private void CreatePerformanceValues()
		{
			_updateTimeValue = new MillisecondsValue(string.Format("{0} Output Total Update", Name));
			VixenSystem.Instrumentation.AddValue(_updateTimeValue);
		}

		private void RemovePerformanceValues()
		{
			if (_updateTimeValue != null)
				VixenSystem.Instrumentation.RemoveValue(_updateTimeValue);
		}

		private void DataPolicyFactoryChanged(object sender, EventArgs eventArgs)
		{
			_dataPolicy = ControllerModule.DataPolicyFactory.CreateDataPolicy();
		}

		public IDataFlowComponent GetDataFlowComponentForOutput(CommandOutput output)
		{
			return _adapterFactory.GetAdapter(output);
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

		/// <summary>
		/// Just update the commands and don't send them out
		/// </summary>
		public void UpdateCommands()
		{

			_outputMediator.LockOutputs();
			try
			{

				if (OutputCount > 15000)
				{
					Parallel.For(0, OutputCount, _parallelOptions, () => ControllerModule.DataPolicyFactory.CreateDataPolicy(), (x, loopState, dataPolicy) =>
					{
						var o = Outputs[x].State;
						Outputs[x].Command = o?.Value != null ? dataPolicy.GenerateCommand(o) : null;
						return dataPolicy;
					}, (x) => { }); //nothing to do but let the datapolicy expire
				}
				else
				{
					for (int x = 0; x < OutputCount; x++)
					{
						var o = Outputs[x].State;
						Outputs[x].Command = o?.Value != null ? _dataPolicy.GenerateCommand(o) : null;
					}
				}

				
			}
			finally
			{
				_outputMediator.UnlockOutputs();
			}
		}

		public void Update()
		{
			_updateStopwatch.Restart();

			try
			{
				if (commands == null)
				{
					commands = new ICommand[OutputCount];
				}
				_outputMediator.LockOutputs();

				if (OutputCount > 15000)
				{
					Parallel.For(0, OutputCount, _parallelOptions, () => ControllerModule.DataPolicyFactory.CreateDataPolicy(), (x, loopState, dataPolicy) =>
				   {
					   var o = Outputs[x].State;
					   commands[x] = o?.Value != null ? dataPolicy.GenerateCommand(o) : null;
					   return dataPolicy;
				   }, x => { }); //nothing to do but let the datapolicy expire
				}
				else
				{
					for (int x = 0; x < OutputCount; x++)
					{
						var o = Outputs[x].State;
						commands[x] = o?.Value != null ? _dataPolicy.GenerateCommand(o) : null;
					}
				}


				ControllerModule.UpdateState(0, commands);

			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occurred outputting data for controller {0}", Name);
			}
			finally
			{
				_outputMediator.UnlockOutputs();
			}

			_updateTimeValue.Set(_updateStopwatch.ElapsedMilliseconds);
			_updateStopwatch.Stop();

		}

		public IOutputDeviceUpdateSignaler UpdateSignaler
		{
			get { return _outputModuleConsumer.UpdateSignaler; }
		}

		public void Start()
		{
			_executionControl.Start();
			CreatePerformanceValues();
		}

		public void Stop()
		{
			_executionControl.Stop();
			RemovePerformanceValues();
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
				while (OutputCount < value)
				{
					AddOutput(outputFactory.CreateOutput(string.Format("Output {0}", (OutputCount + 1).ToString()), OutputCount));
				}
				while (OutputCount > value)
				{
					RemoveOutput(Outputs[OutputCount - 1]);
				}
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
			AddOutput((CommandOutput)output);
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
			RemoveOutput((CommandOutput)output);
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

		private ICommand GenerateOutputCommand(CommandOutput output)
		{
			if (output.State != null)
			{
				return _dataPolicy.GenerateCommand(output.State);
			}
			return null;
		}

		internal IControllerModuleInstance ControllerModule
		{
			get { return _outputModuleConsumer.Module; }
		}

	}
}