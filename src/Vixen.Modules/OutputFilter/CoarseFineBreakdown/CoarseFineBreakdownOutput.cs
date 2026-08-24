using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Produces an 8-bit command output from an input value.
	/// </summary>
	internal class CoarseFineBreakdownOutput : IDataFlowOutput<CommandsDataFlowData>
	{
		private readonly List<ICommand> _outputCommands;
		private readonly CoarseFineBreakdownFilter _filter;
		private readonly CommandsDataFlowData _commandsData;
		private readonly bool _highByte;
		private readonly CoarseFineBreakdownOutputConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of the <see cref="CoarseFineBreakdownOutput"/> class.
		/// </summary>
		/// <param name="highByte"><see langword="true" /> when the output is responsible for the high byte; otherwise, <see langword="false" />.</param>
		/// <param name="configuration">The immutable configuration used to process input values.</param>
		public CoarseFineBreakdownOutput(bool highByte, CoarseFineBreakdownOutputConfiguration configuration)
		{
			_highByte = highByte;
			_configuration = configuration;
			_filter = new CoarseFineBreakdownFilter();
			_outputCommands = new List<ICommand>();
			_commandsData = new CommandsDataFlowData(_outputCommands);
		}

		/// <summary>
		/// Processes the specified command data flow.
		/// </summary>
		/// <param name="data">The command data flow to process.</param>
		public void ProcessInputData(CommandDataFlowData data)
		{
			if (data.Value is _16BitCommand command)
			{
				Handle(command.CommandValue);
			}
		}

		/// <summary>
		/// Processes the specified intent data flow.
		/// </summary>
		/// <param name="intents">The intent data flow to process.</param>
		public void ProcessInputData(IntentsDataFlowData intents)
		{
			_outputCommands.Clear();

			if (intents.Value != null)
			{
				foreach (var intentState in intents.Value)
				{
					var state = _filter.Filter(intentState);
					if (state != null)
					{
						Handle((IIntentState<RangeValue<FunctionIdentity>>)state);
					}
				}
			}
		}

		private void Handle(IIntentState<RangeValue<FunctionIdentity>> intent)
		{
			Handle((ushort)(intent.GetValue().Value * ushort.MaxValue));
		}

		private void Handle(ushort canonicalValue)
		{
			var effectiveValue = _configuration.EffectiveValue(canonicalValue);
			var commandValue = _highByte
				? (byte)(effectiveValue >> 8)
				: (byte)(effectiveValue & 0xFF);

			_outputCommands.Add(new _8BitCommand(commandValue));
		}

		/// <inheritdoc />
		IDataFlowData IDataFlowOutput.Data => Data;

		/// <inheritdoc />
		public string Name => "Coarse-Fine Breakdown";

		/// <inheritdoc />
		public CommandsDataFlowData Data => _commandsData;
	}
}
