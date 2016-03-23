using System;
using System.Drawing;

namespace Vixen.Data.Value
{
	public struct DiscreteValue : IIntentDataType
	{
		private readonly Color _color;
		private readonly double _intensity;

		public DiscreteValue(Color color):this(color, 100)
		{
			
		}

		public DiscreteValue(Color color, double intensity)
		{
			_color = color;
			_intensity = intensity;
		}

		public Color Color
		{
			get { return _color; }
		}

		public Color ColorWithAplha
		{
			get
			{
				return Color.FromArgb((byte)(Intensity * Byte.MaxValue), _color.R, _color.G, _color.B);
			}
		}

		public double Intensity
		{
			get { return _intensity; }
		}
	}
}
