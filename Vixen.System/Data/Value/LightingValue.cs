using System.Diagnostics;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace Vixen.Data.Value
{
	public struct LightingValue : IIntentDataType
	{
		public LightingValue(Color color)
		{
			hsv = HSV.FromRGB(color);
		}

		public LightingValue(Color color, float intensity)
			: this(color)
		{
			Intensity = intensity;
		}

		public LightingValue(float h, float s, float i)
		{
			hsv = new HSV(h, s, i);
		}

		// TODO: make a new color class and use that, instead of these color models.
		public HSV hsv;

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public float Hue
		{
			get { return hsv.H; }
			set { hsv.H = value; }
		}

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public float Saturation
		{
			get { return hsv.S; }
			set { hsv.S = value; }
		}

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public float Intensity
		{
			get { return hsv.V; }
			set { hsv.V = value; }
		}

		/// <summary>
		/// The lighting value as a color with the intensity value applied. Results in an opaque color ranging from black
		/// (0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColor
		{
			get { return hsv.ToRGB().ToArgb(); }
			set { hsv = HSV.FromRGB(value); }
		}

		/// <summary>
		/// Gets the lighting value as a color with the intensity value applied to the alpha channel.
		/// Results in a color of variable transparancy.
		/// </summary>
		public Color FullColorWithAplha
		{
			get
			{
				Color c = FullColor;
				return Color.FromArgb((int)(Intensity * byte.MaxValue), c.R, c.G, c.B);
			}
		}

		/// <summary>
		/// The 'color' portion of the lighting value; ie. only the Hue and Saturation.
		/// This is equivalent to the full color that would have an intensity of 1 (or 100%).
		/// </summary>
		public Color HueSaturationOnlyColor
		{
			get
			{
				float i = Intensity;
				Intensity = 1;
				Color rv = hsv.ToRGB().ToArgb();
				Intensity = i;
				return rv;
			}
			set
			{
				HSV newValue = HSV.FromRGB(value);
				hsv.H = newValue.H;
				hsv.S = newValue.S;
			}
		}
	}
}