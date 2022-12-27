using Common.Controls.ColorManagement.ColorModels;
using VixenModules.Effect.Effect;

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
		public HSV HSV;

		private int Rand()
		{
			return ThreadSafeRandom.Instance.Next();
		}

		public void Reset(int x, int y, bool active, float velocity, HSV hsv, int start, int colorLocation)
		{
			X = x;
			Y = y;
			Vel = (Rand() - int.MaxValue / 2) * velocity / (int.MaxValue / 2);
			Angle = (float)(2 * Math.PI * Rand() / int.MaxValue);
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
