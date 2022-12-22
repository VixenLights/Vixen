using Vixen.Data.Evaluator;
using Vixen.Data.Flow;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	/// <summary>
	/// Maintains an 8-bit color breakdown output.
	/// </summary>
	internal class _8BitColorBreakdownOutput : ColorBreakdownOutputBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="breakdownItem">Breakdown item associated with the output</param>
		/// <param name="mixColors">True when this output is using color mixing</param>
		/// <param name="rgbToRGBWConverter">Optional RGB to RGBW converter</param>
		public _8BitColorBreakdownOutput(ColorBreakdownItem breakdownItem, bool mixColors, RGBToRGBWConverter rgbToRGBWConverter) :
			base(breakdownItem, mixColors, rgbToRGBWConverter)
		{
			// Default the output
			Data = new CommandDataFlowData(CommandLookup8BitEvaluator.CommandLookup[0]);
		}

		#endregion

		#region Protected Methods
	
		/// <InheritDoc/>
		protected override void ProcessInputDataInternal(double intensity)
		{
			// Convert the intensity into a 8-bit command
			Data.Value = CommandLookup8BitEvaluator.CommandLookup[(byte)(intensity * Byte.MaxValue)];
		}

		#endregion
	}
}
