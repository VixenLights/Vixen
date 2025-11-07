using System.Diagnostics;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	/// <summary>
	/// This filter gets the intensity percent for a given state in simple RGBW mixing.
	/// </summary>
	internal class RGBWColorBreakdownMixingFilter : ColorBreakdownMixingFilterBase
	{
		#region Fields

		/// <summary>
		/// RGB to RGBW converter.  This object is intended to improve performance
		/// by only requiring the conversion from RGB to RGBW once per color.
		/// </summary>
		private RGBToRGBWConverter _rgbToRGBWConverter;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="breakdownItem">Breakdown item to create filter for</param>
		/// <param name="rgbToRGBWConverter">Optional RGB to RGBW converter</param>
		public RGBWColorBreakdownMixingFilter(ColorBreakdownItem breakdownItem, RGBToRGBWConverter rgbToRGBWConverter)
		{
			// Save off the RGB to RGBW converter
			_rgbToRGBWConverter = rgbToRGBWConverter;

			// Configure the RGBW filter
			ConfigureRGBWFilter(breakdownItem);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Extracts the red component from the RGBW color.
		/// </summary>
		/// <param name="color">Color to process</param>
		/// <returns>Red component of the RGBW</returns>
		private float FilterRGBToRGBW_Red(Color color)
		{
			// Extract the red component
			return (float)_rgbToRGBWConverter.ConvertToRGBW(color).R;
		}

		/// <summary>
		/// Extracts the green component from the RGBW color.
		/// </summary>
		/// <param name="color">Color to process</param>
		/// <returns>Green component of the RGBW</returns>
		private float FilterRGBToRGBW_Green(Color color)
		{
			// Extract the green component
			return (float) _rgbToRGBWConverter.ConvertToRGBW(color).G;
		}

		/// <summary>
		/// Extracts the blue component from the RGBW color.
		/// </summary>
		/// <param name="color">Color to process</param>
		/// <returns>Blue component of the RGBW</returns>
		private float FilterRGBToRGBW_Blue(Color color)
		{
			// Extract the blue component
			return (float)_rgbToRGBWConverter.ConvertToRGBW(color).B;
		}

		/// <summary>
		/// Extracts the white component from the RGBW color.
		/// </summary>
		/// <param name="color">Color to process</param>
		/// <returns>White component of the RGBW</returns>
		private float FilterRGBToRGBW_White(Color color)
		{
			// Extract the white component
			return (float)_rgbToRGBWConverter.ConvertToRGBW(color).W;
		}

		/// <summary>
		/// Configures the RGBW filter for the specified breakdown item.
		/// </summary>
		/// <param name="breakdownItem">Breakdown item to process</param>
		private void ConfigureRGBWFilter(ColorBreakdownItem breakdownItem)
		{
			// If the break down item color is red then...
			if (breakdownItem.Color.ToArgb().Equals(Color.Red.ToArgb()))
			{
				GetMaxProportionFunc = FilterRGBToRGBW_Red;
			}
			else if (breakdownItem.Color.ToArgb().Equals(Color.Lime.ToArgb()))
			{
				GetMaxProportionFunc = FilterRGBToRGBW_Green;
			}
			else if (breakdownItem.Color.ToArgb().Equals(Color.Blue.ToArgb()))
			{
				GetMaxProportionFunc = FilterRGBToRGBW_Blue;
			}
			else if (breakdownItem.Color.ToArgb().Equals(Color.White.ToArgb()))
			{
				GetMaxProportionFunc = FilterRGBToRGBW_White;
			}
			else
			{ 
				Debug.Assert(false, "Unsupported break down item color!");
			}
		}

		#endregion
	}
}
