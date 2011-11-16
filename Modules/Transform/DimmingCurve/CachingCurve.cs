using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Transform;
using VixenModules.App.Curves;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.Transform.DimmingCurve
{
	class CachingCurve : Curve
	{
		public CachingCurve()
			: base()
		{
		}

		public CachingCurve(ZedGraph.IPointList points)
			: base(points)
		{
		}

		public CachingCurve(Curve other)
			: base(other)
		{
		}

		private Dictionary<int, double> CachedValues { get; set; }

		public override double GetValue(double x)
		{
			if (CachedValues == null)
				GenerateCachedValues();

			// round to the nearest 0.1
			int rounded = (int)Math.Round(x * 10);

			return CachedValues[rounded];
		}

		private void GenerateCachedValues()
		{
			double step = 0.1;

			CachedValues = new Dictionary<int, double>();

			for (double i = 0; i <= 100.0; i += step) {
				CachedValues[(int)Math.Round(i * 10)] = base.GetValue(i);
			}
		}
	}
}
