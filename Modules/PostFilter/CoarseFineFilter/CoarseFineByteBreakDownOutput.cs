using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Produces an 8-bit command output from an intent.
	/// </summary>
	internal class CoarseFineBreakdownOutput : IDataFlowOutput<CommandsDataFlowData>
	{
		#region Private Fields
		
		/// <summary>
		/// Collection of output commands.
		/// </summary>
		private readonly List<ICommand> _outputCommands;
		
		/// <summary>
		/// Filter that determines which intents to process.
		/// </summary>
		private readonly CoarseFineBreakdownFilter _filter;

		/// <summary>
		/// Wraps the output commands in the <c>IDataFlowData</c> interface.
		/// </summary>
		private readonly CommandsDataFlowData _commandsData;
		
		/// <summary>
		/// Determines whether the output is outputing the high byte or the low byte of the intent.
		/// </summary>
		private bool _highByte;

		#endregion

		#region Constructor

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="highByte">True when the output is responsible for the high byte, false when the output is responsible for the low byte</param>
		public CoarseFineBreakdownOutput(bool highByte)
		{
			// Save off whether this output is handling the high or low byte
			_highByte = highByte;

			// Create the filter that determines what intents are processed
			_filter = new CoarseFineBreakdownFilter();

			// Create the collection of output commands
			_outputCommands = new List<ICommand>();

			// Create the IDataFlowOutput output wrapping the output commands
			_commandsData = new CommandsDataFlowData(_outputCommands);			
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Processes the specified command data flow.
		/// </summary>
		/// <param name="data">Command data flow to process</param>
		public void ProcessInputData(CommandDataFlowData data)
		{
			/// Test to see if the intent is a tagged intent
			_16BitCommand command = data.Value as _16BitCommand;

			// If the command is an 16 bit command then...
			if (command != null)
			{
				// Get the 16 bit value
				ushort value = command.CommandValue;

				// Normalize the value to (0 - 1)
				double dblValue = (double)value / (double)ushort.MaxValue;

				// Split the value into a high and low byte
				Handle(dblValue);
			}
		}

		/// <summary>
		/// Processes input intent data.
		/// </summary>
		/// <param name="intents">Intents to process</param>
		public void ProcessInputData(IntentsDataFlowData intents)
		{
			// Clear the output commands
			_outputCommands.Clear();

			if (intents.Value != null)
			{
				// Loop over the intent states
				foreach (IIntentState intentState in intents.Value)
				{
					// Determine if the intent state is supported by this breakdown filter
					IIntentState state = _filter.Filter(intentState);

					// If the state is supported (not null) then...
					if (state != null)
					{
						// Handle the intent
						Handle((IIntentState<RangeValue<FunctionIdentity>>)state);
					}
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles a <c>PositionValue</c> intent.
		/// </summary>
		/// <param name="intent">Range intent to handle</param>
		private void Handle(IIntentState<RangeValue<FunctionIdentity>> intent)
		{
			// Get the position value from the intent
			double rangeValue = intent.GetValue().Value;

			// Handle the range value
			Handle(rangeValue);
		}

		/// <summary>
		/// Handles a double range value.
		/// </summary>
		/// <param name="intent">Double range value to handle</param>
		private void Handle(double value)
		{
			// Get the range value from the intent
			double rangeValue = value;

			// Convert the double into a 16-bit integer			
			UInt16 rangeInteger = (UInt16)(rangeValue * UInt16.MaxValue);

			// Declare the return value
			byte commandValue;

			// If this output is responsible for the high byte then...
			if (_highByte)
			{
				// Shift the value to only be the high byte
				commandValue = (byte)(rangeInteger >> 8);
			}
			// Otherise the output is responsible for the low byte
			else
			{
				// Mask the input to only include the low byte
				commandValue = (byte)(rangeInteger & 0xff);
			}

			// Create a new 8-bit command with the return value
			_outputCommands.Add(new _8BitCommand(commandValue));
		}

		#endregion

		#region IDataFlowOutput

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		IDataFlowData IDataFlowOutput.Data => Data;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public string Name => "Coarse-Fine Breakdown";

		#endregion

		#region IDataFlowOutput<T>

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public CommandsDataFlowData Data => _commandsData;

		#endregion
	}
}