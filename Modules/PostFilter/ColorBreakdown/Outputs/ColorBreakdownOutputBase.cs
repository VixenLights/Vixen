using System;
using Vixen.Data.Flow;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	/// <summary>
	/// Abstract base class for a color breakdown output.
	/// </summary>
	internal abstract class ColorBreakdownOutputBase : IDataFlowOutput<CommandDataFlowData>
	{
		/// <param name="breakdownItem">Breakdown item associated with the output</param>
		/// <param name="mixColors">True when this output is using color mixing</param>
		/// <param name="rgbToRGBWConverter">Optional RGB to RGBW converter</param>

		protected ColorBreakdownOutputBase(ColorBreakdownItem breakdownItem, bool mixColors, RGBToRGBWConverter rgbToRGBWConverter)
		{
			if (mixColors)
			{
				// If an RGB to RGBW converter was specified then...
				if (rgbToRGBWConverter != null)
				{
					// An RGBW mixing filter is needed
					Filter = new RGBWColorBreakdownMixingFilter(breakdownItem, rgbToRGBWConverter);
				}
				else
				{
					// Otherwise a normal RGB mixing filter is needed
					Filter = new RGBColorBreakdownMixingFilter(breakdownItem);
				}
			}
			else
			{
				Filter = new ColorBreakdownFilter(breakdownItem);
			}

			// Store off the breakdown item
			BreakdownItem = breakdownItem;
		}

		#region Protected Properties

		protected IBreakdownFilter Filter { get; set; }

		protected ColorBreakdownItem BreakdownItem { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Processes the intensity into an intent or command.
		/// </summary>
		/// <param name="intensity">Intensity of the color channel</param>
		protected abstract void ProcessInputDataInternal(double intensity);

		#endregion

		#region Public Methods

		/// <summary>
		/// Processes intent data.  This method is the main entry point for the output.
		/// </summary>
		/// <param name="data">Intent data to process</param>
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
					double i = Filter.GetIntensityForState(intentState);
					intensity = Math.Max(i, intensity);
				}
			}

			// Allow the derived outputs to convert the intensity into an intent or command
			ProcessInputDataInternal(intensity);
		}

		#endregion

		#region IDataFlowOutput

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public string Name
		{
			get { return BreakdownItem.Name; }
		}

		#endregion

		#region IDataFlowOutput

		/// <inheritdoc />
		IDataFlowData IDataFlowOutput.Data => Data;

		/// <inheritdoc />
		public CommandDataFlowData Data { get; protected set; }

		#endregion
	}
}
