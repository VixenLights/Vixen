using System;
using System.Drawing;
using Vixen.Interpolator;
using Vixen.Module.PreFilter;

namespace Vixen.Sys {
	class PreFilterState : IFilterState {
		private FloatInterpolator _interpolator;
		public PreFilterState(IPreFilter filter, TimeSpan relativeTime) {
			Filter = filter;
			RelativeTime = relativeTime;
			_interpolator = new FloatInterpolator();
		}

		public IPreFilter Filter { get; private set; }

		public TimeSpan RelativeTime { get; private set; }

		public float Affect(float value) {
			return Filter.Affect(value, _GetPercentIntoFilter());
		}

		public Color Affect(Color value) {
			return Filter.Affect(value, _GetPercentIntoFilter());
		}

		public DateTime Affect(DateTime value) {
			return Filter.Affect(value, _GetPercentIntoFilter());
		}

		public long Affect(long value) {
			return Filter.Affect(value, _GetPercentIntoFilter());
		}

		public double Affect(double value) {
			return Filter.Affect(value, _GetPercentIntoFilter());
		}

		private float _GetPercentIntoFilter() {
			float value;
			_interpolator.Interpolate(RelativeTime, Filter.TimeSpan, 0, 1, out value);
			return value;
			//return (float)((RelativeTime - Filter.StartTime).TotalMilliseconds / Filter.TimeSpan.TotalMilliseconds);
		}
	}
}
