using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;

namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Coarse Fine (High Byte / Low Byte) breakdown filter module.
	/// </summary>
	public class CoarseFineBreakdownModule : OutputFilterModuleInstanceBase
	{
		#region Fields

		/// <summary>
		/// Data associated with the module.
		/// </summary>
		private CoarseFineBreakdownData _data;
		
		/// <summary>
		/// Array of outputs associated with the breakdown filter.
		/// </summary>
		private CoarseFineBreakdownOutput[] _outputs;

		#endregion

		#region Public Overrides

		/// <summary>
		/// Breakdowns the intents and routes them to the correct output.
		/// </summary>
		/// <param name="intents">Intents to process</param>
		public override void Handle(IntentsDataFlowData intents)
		{
			// Loop over the outputs
			foreach (CoarseFineBreakdownOutput output in _outputs) 
			{
				// Process the intents for the output
				output.ProcessInputData(intents);
			}
		}
		
		/// <summary>
		/// Takes as input multiple intents.
		/// </summary>
		public override DataFlowType InputDataType => DataFlowType.MultipleIntents;

		/// <summary>
		/// Outputs multiple commands.
		/// </summary>
		public override DataFlowType OutputDataType => DataFlowType.MultipleCommands;

		/// <summary>
		/// Collection of exposed outputs.
		/// </summary>
		public override IDataFlowOutput[] Outputs => _outputs;

		/// <summary>
		/// Data associated with the module.
		/// </summary>
		public override IModuleDataModel ModuleData
		{
			get => _data;
			set
			{
				// Save off the module data
				_data = (CoarseFineBreakdownData) value;

				// Create the outputs associated with the breakdown filter
				CreateOutputs();
			}
		}

		/// <summary>
		/// Does not require a Setup dialog.
		/// </summary>
		public override bool HasSetup => false;

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the outputs associated with the breakdown filter.
		/// </summary>
		private void CreateOutputs()
		{		
			// Create the array of outputs
			_outputs = new[] 
			{ 
				new CoarseFineBreakdownOutput(true), // Coarse (High Byte)
				new CoarseFineBreakdownOutput(false) // Fine (Low Byte)
			};						
		}

		#endregion
	}
}