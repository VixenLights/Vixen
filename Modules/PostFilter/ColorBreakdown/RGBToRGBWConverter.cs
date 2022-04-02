using Common.Controls.ColorManagement.ColorModels;
using System.Drawing;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	/// <summary>
	/// Converts RGB Color objects to RGBW values.
	/// </summary>
	/// <remarks>
	/// This class is used by the red, green, blue and white filters.
	/// This class exists for performance reasons so that the math to convert from
	/// RGB to RGBW is only performed once per color.
	/// </remarks>
	public class RGBToRGBWConverter 
	{
		#region Fields

		/// <summary>
		/// Last input color converted.
		/// </summary>
		private Color _lastInputColor;

		/// <summary>
		/// RGBW value of the last input color.
		/// </summary>
		private RGBW _lastConvertedColor;

		#endregion

		#region Public Methods

		/// <summary>
		/// Converts the specified RGB color to an RGBW struct.
		/// </summary>
		/// <param name="color">RGB Color to convert</param>
		/// <returns>RGBW struct for the specified color</returns>
		public RGBW ConvertToRGBW(Color color)
		{
			// If the color does NOT match the previously converted color then...
			if (_lastInputColor != color)
			{
				// Save off the input color
				_lastInputColor = color;

				// Convert the input color to a RGBW struct
				_lastConvertedColor = color.ToRGBWFast();
			}

			// Return the RGBW struct
			return _lastConvertedColor;
		}

		#endregion 
	}
}
