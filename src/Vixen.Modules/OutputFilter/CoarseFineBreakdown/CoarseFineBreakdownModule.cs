using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using VixenModules.OutputFilter.CoarseFineBreakdown.Setup.ViewModels;
using VixenModules.OutputFilter.CoarseFineBreakdown.Setup.Views;

namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Breaks 16-bit input values into coarse and fine output bytes.
	/// </summary>
	public class CoarseFineBreakdownModule : OutputFilterModuleInstanceBase
	{
		private CoarseFineBreakdownData _data;
		private CoarseFineBreakdownOutput[] _outputs;

		/// <summary>
		/// Breaks down the intents and routes them to the correct output.
		/// </summary>
		/// <param name="intents">Intents to process</param>
		public override void Handle(IntentsDataFlowData intents)
		{
			foreach (var output in _outputs)
			{
				output.ProcessInputData(intents);
			}
		}

		/// <summary>
		/// Breaks down the commands and routes them to the correct output.
		/// </summary>
		/// <param name="obj"></param>
		public override void Handle(CommandDataFlowData commandDataFlow)
		{
			foreach (var output in _outputs)
			{
				output.ProcessInputData(commandDataFlow);
			}
		}

		/// <summary>
		/// Takes as input multiple intents.
		/// </summary>
		public override DataFlowType InputDataType => DataFlowType.MultipleIntents;

		/// <inheritdoc />
		public override DataFlowType OutputDataType => DataFlowType.MultipleCommands;

		/// <inheritdoc />
		public override IDataFlowOutput[] Outputs => _outputs;

		/// <inheritdoc />
		public override IModuleDataModel ModuleData
		{
			get => _data;
			set
			{
				_data = (CoarseFineBreakdownData)value;
				CreateOutputs();
			}
		}

		/// <summary>
		/// Gets a value that indicates whether this module provides a setup dialog.
		/// </summary>
		/// <value><see langword="true" /> because this module has configurable default-value mapping; otherwise, <see langword="false" />.</value>
		public override bool HasSetup => true;

		/// <summary>
		/// Displays the setup dialog and applies its accepted configuration.
		/// </summary>
		/// <returns><see langword="true" /> if the user accepts valid configuration values; otherwise, <see langword="false" />.</returns>
		public override bool Setup()
		{
			var viewModel = new CoarseFineBreakdownSetupViewModel(
				_data.EnableDefaultValueMapping,
				_data.DefaultInputValue,
				_data.RestingCoarseValue,
				_data.RestingFineValue);
			var view = new CoarseFineBreakdownSetupView(viewModel);

			if (view.ShowDialog() != true || viewModel.Result is not { } result)
			{
				return false;
			}

			ApplyConfiguration(new CoarseFineBreakdownData
			{
				EnableDefaultValueMapping = result.EnableDefaultValueMapping,
				DefaultInputValue = result.DefaultInputValue,
				RestingCoarseValue = result.RestingCoarseValue,
				RestingFineValue = result.RestingFineValue
			});
			return true;
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the configured default input value is mapped to the resting output values.
		/// </summary>
		/// <value><see langword="true" /> to map the configured default input value; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
		public bool EnableDefaultValueMapping
		{
			get => _data.EnableDefaultValueMapping;
			set
			{
				_data.EnableDefaultValueMapping = value;
				CreateOutputs();
			}
		}

		/// <summary>
		/// Gets or sets the 16-bit input value that is mapped when default value mapping is enabled.
		/// </summary>
		/// <value>The input value to map. The default is <c>0</c>.</value>
		public ushort DefaultInputValue
		{
			get => _data.DefaultInputValue;
			set
			{
				_data.DefaultInputValue = value;
				CreateOutputs();
			}
		}

		/// <summary>
		/// Gets or sets the high-byte value emitted for a mapped default input value.
		/// </summary>
		/// <value>The coarse output value. The default is <c>0</c>.</value>
		public byte RestingCoarseValue
		{
			get => _data.RestingCoarseValue;
			set
			{
				_data.RestingCoarseValue = value;
				CreateOutputs();
			}
		}

		/// <summary>
		/// Gets or sets the low-byte value emitted for a mapped default input value.
		/// </summary>
		/// <value>The fine output value. The default is <c>0</c>.</value>
		public byte RestingFineValue
		{
			get => _data.RestingFineValue;
			set
			{
				_data.RestingFineValue = value;
				CreateOutputs();
			}
		}

		private void CreateOutputs()
		{
			var configuration = new CoarseFineBreakdownOutputConfiguration(
				_data.EnableDefaultValueMapping,
				_data.DefaultInputValue,
				_data.RestingCoarseValue,
				_data.RestingFineValue);

			_outputs =
			[
				new CoarseFineBreakdownOutput(true, configuration),
				new CoarseFineBreakdownOutput(false, configuration)
			];
		}

		private void ApplyConfiguration(CoarseFineBreakdownData configuration)
		{
			_data.EnableDefaultValueMapping = configuration.EnableDefaultValueMapping;
			_data.DefaultInputValue = configuration.DefaultInputValue;
			_data.RestingCoarseValue = configuration.RestingCoarseValue;
			_data.RestingFineValue = configuration.RestingFineValue;
			CreateOutputs();
		}
	}
}
