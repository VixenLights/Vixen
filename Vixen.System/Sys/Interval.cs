using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Sys {
	public class Interval {
		public Interval(long time) {
			Time = time;
		}

		// Has to be settable so that it can be adjusted.
		public long Time { get; set; }

		public override string ToString() {
			return Time.ToString();
		}

		public class EqualityComparer : IEqualityComparer<Interval> {
			public bool Equals(Interval x, Interval y) {
				return x.Time == y.Time;
			}

			public int GetHashCode(Interval obj) {
				return obj.Time.GetHashCode();
			}
		}
	}
}
