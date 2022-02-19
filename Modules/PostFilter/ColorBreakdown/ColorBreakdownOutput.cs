using System;
using Vixen.Data.Evaluator;
using Vixen.Data.Flow;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	internal class ColorBreakdownOutput : IDataFlowOutput<CommandDataFlowData>
	{
		private readonly IBreakdownFilter _filter;
		private readonly ColorBreakdownItem _breakdownItem;

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="breakdownItem">Breakdown item associated with the output</param>
		/// <param name="mixColors">True when this output is using color mixing</param>
		/// <param name="rgbToRGBWConverter">Optional RGB to RGBW converter</param>
		public ColorBreakdownOutput(ColorBreakdownItem breakdownItem, bool mixColors, RGBToRGBWConverter rgbToRGBWConverter)
		{
			Data = new CommandDataFlowData(CommandLookup8BitEvaluator.CommandLookup[0]);
			if (mixColors)
			{
				// If an RGB to RGBW converter was specified then...
				if (rgbToRGBWConverter != null)
				{
					// An RGBW mixing filter is needed
					_filter = new RGBWColorBreakdownMixingFilter(breakdownItem, rgbToRGBWConverter);
				}
				else
				{
					// Otherwise a normal RGB mixing filter is needed
					_filter = new RGBColorBreakdownMixingFilter(breakdownItem);
				}
			}
			else
			{
				_filter = new ColorBreakdownFilter(breakdownItem);
			}

			_breakdownItem = breakdownItem;
		}

		#endregion

		public void ProcessInputData(IntentsDataFlowData data)
		{
			//Because we are combining at the layer above us, we should really only have one
			//intent that matches this outputs color setting. 
			//Everything else will have a zero intensity and should be thrown away when it does not match our outputs color.
			double intensity = 0;
			if (data.Value?.Count > 0)
			{
				foreach (var intentState in data.Value)
				{
					var i = _filter.GetIntensityForState(intentState);
					intensity = Math.Max(i, intensity);
				}
			}

			Data.Value = CommandLookup8BitEvaluator.CommandLookup[(byte)(intensity * Byte.MaxValue)];
		}

		IDataFlowData IDataFlowOutput.Data => Data;

		public string Name
		{
			get { return _breakdownItem.Name; }
		}

		/// <inheritdoc />
		public CommandDataFlowData Data { get; }
	}
}
