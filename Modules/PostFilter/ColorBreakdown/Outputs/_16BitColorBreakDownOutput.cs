using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Flow;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	/// <summary>
	/// Maintains a 16 bit color breakdown output.  
	/// </summary>
	internal class _16BitColorBreakDownOutput : ColorBreakdownOutputBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="breakdownItem">Breakdown item associated with the output</param>
		/// <param name="mixColors">True when this output is using color mixing</param>
		/// <param name="rgbToRGBWConverter">Optional RGB to RGBW converter</param>
		public _16BitColorBreakDownOutput(ColorBreakdownItem breakdownItem, bool mixColors, RGBToRGBWConverter rgbToRGBWConverter) : 
			base(breakdownItem, mixColors, rgbToRGBWConverter)
		{
			// Default the output
			Data = new CommandDataFlowData(CommandLookup16BitEvaluator.CommandLookup[0]);
		}

		#endregion

		#region Protected Methods

		/// <InheritDoc/>
		protected override void ProcessInputDataInternal(double intensity)
		{
			// Convert the intensity into a 16-bit command
			Data.Value = CommandLookup16BitEvaluator.CommandLookup[(ushort)(intensity * ushort.MaxValue)];
		}

		#endregion
	}
}
