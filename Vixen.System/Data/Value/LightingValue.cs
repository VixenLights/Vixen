using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace Vixen.Data.Value
{
	public struct LightingValue : IIntentDataType
	{
		/// <summary>
		/// Create a lighting value of the specified color with intensity of 1
		/// </summary>
		/// <param name="color"></param>
		public LightingValue(Color color):this(color, 1)
		{
		}

		/// <summary>
		/// Create a lighting value with the specified color and intensity
		/// </summary>
		/// <param name="color"></param>
		/// <param name="intensity">Percentage value in the range 0.0 -> 1.0 (from 0% to 100%).</param>
		public LightingValue(Color color, double intensity)
		{
			Color = color;
			Intensity = HSV.Clamp(intensity,0,1);
		}

		public LightingValue(LightingValue lv, double i)
		{
			Color = lv.Color;
			Intensity = i;
		}

		public LightingValue(LightingValue lv)
		{
			Color = lv.Color;
			Intensity = lv.Intensity;
		}

		/// <summary>
		/// The Intensity or brightness of this color in the range 0.0 -> 1.0 (from 0% to 100%).
		/// </summary>
		public double Intensity { get; }

		public Color Color { get; }


		/// <summary>
		/// The lighting value as a intensity applied color with a 100% alpha channel. Results in an opaque color ranging from black
		/// (0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColor =>
			Color.FromArgb(255, (int)(Color.R * Intensity),
				(int)(Color.G * Intensity), (int)(Color.B * Intensity));

		/// <summary>
		/// Gets the lighting value as a full brightness color with the intensity value applied to the alpha channel. 
		/// Results in an non opaque color ranging from transparent (0,0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColorWithAlpha => Color.FromArgb((int)(Intensity * byte.MaxValue), Color);
	}
}