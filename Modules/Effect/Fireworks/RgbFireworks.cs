using System;
using Common.Controls.ColorManagement.ColorModels;

namespace VixenModules.Effect.Fireworks
{
	public class RgbFireworks
	{
		public const int MaxCycle = 4096;
		public const int MaxNewBurstFlakes = 10;
		public float X { get; internal set; }
		public float Y { get; internal set; }
		public float Dx { get; internal set; }
		public float Dy { get; internal set; }
		public float Vel { get; private set; }
		public float Angle { get; private set; }
		public bool Active { get; set; }
		public int Cycles { get; internal set; }
		public int ColorLocation { get; internal set; }
		public int StartPeriod { get; set; }
		public HSV HSV = new HSV();
		private static readonly Random Random = new Random();

		public void Reset(int x, int y, bool active, float velocity, HSV hsv, int start, int colorLocation)
		{
			X = x;
			Y = y;
			Vel = (Random.Next() - int.MaxValue / 2) * velocity / (int.MaxValue / 2);
			Angle = (float)(2 * Math.PI * Random.Next() / int.MaxValue);
			Dx = (float)(Vel * Math.Cos(Angle));
			Dy = (float)(Vel * Math.Sin(Angle));
			Active = active;
			Cycles = 0;
			HSV = hsv;
			ColorLocation = colorLocation;
			StartPeriod = start;
		}
	}
}
