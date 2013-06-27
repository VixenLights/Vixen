using System.Drawing;
using Vixen.Data.Value;

namespace FadeOut
{
	internal class Reducer
	{
		public ColorValue Reduce(ColorValue value, double reductionPercent)
		{
			return new ColorValue(Reduce(value.Color, reductionPercent));
		}

		public LightingValue Reduce(LightingValue value, double reductionPercent)
		{
			return new LightingValue(value.Color, Reduce(value.Intensity, reductionPercent));
		}

		public byte Reduce(byte value, double reductionPercent)
		{
			return (byte) (value - (value*reductionPercent));
		}

		public ushort Reduce(ushort value, double reductionPercent)
		{
			return (ushort) (value - (value*reductionPercent));
		}

		public uint Reduce(uint value, double reductionPercent)
		{
			return value - (uint) (value*reductionPercent);
		}

		public ulong Reduce(ulong value, double reductionPercent)
		{
			return value - (ulong) (value*reductionPercent);
		}

		public Color Reduce(Color value, double reductionPercent)
		{
			double remainingPercent = 1d - reductionPercent;
			return Color.FromArgb((int) (value.R*remainingPercent), (int) (value.G*remainingPercent),
			                      (int) (value.B*remainingPercent));
		}

		public float Reduce(float value, double reductionPercent)
		{
			return value - (float) (value*reductionPercent);
		}
	}
}