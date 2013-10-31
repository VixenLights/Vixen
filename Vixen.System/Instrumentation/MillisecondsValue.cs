using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	public class MillisecondsValue : InstrumentationValue
	{
		private double tot=0;
		private int cnt=1;
		private double max=0;
		private double min=999;

		public MillisecondsValue( string name)
			: base(name)
		{
		}

		/**/
		public void Set(double value)
		{
			if( value > max)
				max = value;
			if (value < min)
				min = value;
			tot += value;
			cnt++;
		}

		protected override double _GetValue()
		{
			double tmp = tot;
			if (cnt > 0)
				tmp /= cnt;
			tot = 0;
			cnt = 0;
			max = 0;
			min = 999;
			return tmp;
		}
		/**/

		protected override string _GetFormattedValue()
		{
			double mx = max;
			double mn = min;
			return string.Format("min {0,2} ms, avg {1,2} ms, max {2,2} ms", (int)mn, (int)_GetValue(), (int)mx);
		}
	}
}