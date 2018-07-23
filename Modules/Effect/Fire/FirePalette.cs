using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Controls.ColorManagement.ColorModels;

namespace VixenModules.Effect.Fire
{
	public static class FirePalette
	{
		private static readonly List<HSV> Palette = new List<HSV>();

		static FirePalette()
		{
			InitPalette();
		}

		private static void InitPalette()
		{
			HSV hsv = new HSV();

			Palette.Clear();
			int i;
			// calc 100 reds, black to bright red
			hsv.H = 0.0f;
			hsv.S = 1.0f;
			for (i = 0; i < 100; i++)
			{
				hsv.V = i / 100.0f;
				Palette.Add(hsv);

			}

			// gives 100 hues red to yellow
			hsv.V = 1.0f;
			for (i = 0; i < 100; i++)
			{
				Palette.Add(hsv);
				hsv.H += 0.00166666f;
			}
		}


		public static HSV GetColor(int index)
		{
			if (index >= 0 && index >= Palette.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return Palette[index];	
		}

		public static int Count()
		{
			return Palette.Count();
		}
	}
}
