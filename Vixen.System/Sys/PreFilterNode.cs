using System;
using Vixen.Module.PreFilter;

namespace Vixen.Sys {
	public class PreFilterNode : IPreFilterNode {
		public PreFilterNode(IPreFilterModuleInstance preFilter, TimeSpan startTime) {
			PreFilter = preFilter;
			StartTime = startTime;
		}

		public IPreFilterModuleInstance PreFilter { get; private set; }

		public TimeSpan StartTime { get; private set; }

		public TimeSpan TimeSpan {
			get { return (PreFilter != null) ? PreFilter.TimeSpan : TimeSpan.Zero; }
		}

		public TimeSpan EndTime {
			get { return StartTime + TimeSpan; }
		}

		public void AffectIntent(IIntentSegment intentSegment, TimeSpan contextAbsoluteStartTime, TimeSpan contextAbsoluteEndTime) {
			PreFilter.AffectIntent(intentSegment, StartTime - contextAbsoluteStartTime, EndTime - contextAbsoluteEndTime);
		}

		#region IComparable<IPreFilterNode>
		public int CompareTo(IPreFilterNode other) {
			return DataNode.Compare(this, other);
		}
		#endregion
	}

	public interface IPreFilterNode : IDataNode, IComparable<IPreFilterNode> {
		IPreFilterModuleInstance PreFilter { get; }
		void AffectIntent(IIntentSegment intentSegment, TimeSpan contextAbsoluteStartTime, TimeSpan contextAbsoluteEndTime);
	}
}
