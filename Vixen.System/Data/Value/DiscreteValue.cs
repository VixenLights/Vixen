using System;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace Vixen.Data.Value
{
	public struct DiscreteValue : IIntentDataType
	{
		private readonly Color _color;
		private double _intensity;

		public DiscreteValue(Color color) : this(color, 1)
		{

		}

		public DiscreteValue(Color color, double intensity)
		{
			_color = color;
			_intensity = XYZ.ClipValue(intensity, 0, 1);
		}

		public DiscreteValue(DiscreteValue dv)
		{
			_color = dv.Color;
			_intensity = dv.Intensity;
		}

		public Color Color
		{
			get { return _color; }
		}

		/// <summary>
		/// The discrete value as a intensity appplied color with a 100% alpha channel. Results in an opaque color ranging from black
		/// (0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColor
		{
			get
			{
				HSV hsv = HSV.FromRGB(_color);
				hsv.V = hsv.V*_intensity;
				return hsv.ToRGB();
			}
		}

		/// <summary>
		/// Gets the discrete value as a full brightness color with the intensity value applied to the alpha channel. 
		/// Results in an non opaque color ranging from transparent (0,0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColorWithAplha
		{
			get
			{
				Color c = Color;
				return Color.FromArgb((byte)(Intensity * Byte.MaxValue), c.R, c.G, c.B);
			}
		}

		/// <summary>
		/// The Intensity or brightness of this color in the range 0.0 -> 1.0 (from 0% to 100%).
		/// </summary>
		public double Intensity
		{
			get { return _intensity; }
			set { _intensity = value; }
		}
	}
}
