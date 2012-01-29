using System;
using Vixen.Module.PreFilter;

namespace Vixen.Sys {
	public class PreFilterNode : IDataNode, IComparable<PreFilterNode> {
		public PreFilterNode(IPreFilterModuleInstance preFilter, TimeSpan startTime) {
			PreFilter = preFilter;
			StartTime = startTime;
		}

		public IPreFilterModuleInstance PreFilter { get; private set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan TimeSpan {
			get { return (PreFilter != null) ? PreFilter.TimeSpan : TimeSpan.Zero; }
		}

		public TimeSpan EndTime {
			get { return (PreFilter != null) ? StartTime + TimeSpan : StartTime; }
		}

		public int CompareTo(PreFilterNode other) {
			return StartTime.CompareTo(other.StartTime);
		}
	}
}
