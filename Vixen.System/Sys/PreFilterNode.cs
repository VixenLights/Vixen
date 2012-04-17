using System;
using Vixen.Module.PreFilter;

namespace Vixen.Sys {
	public class PreFilterNode : IPreFilterNode {
		public PreFilterNode(IPreFilterModuleInstance preFilter, TimeSpan startTime) {
			PreFilter = preFilter;

			TimeSpan timeSpan = (PreFilter != null) ? PreFilter.TimeSpan : TimeSpan.Zero;
			TimeNode = new TimeNode(startTime, timeSpan);
		}

		public IPreFilterModuleInstance PreFilter { get; private set; }

		public TimeNode TimeNode { get; private set; }
		
		public TimeSpan StartTime {
			get { return TimeNode.StartTime; }
			set { TimeNode = new TimeNode(value, TimeSpan); }
		}

		public TimeSpan TimeSpan {
			get { return TimeNode.TimeSpan; }
		}

		public TimeSpan EndTime {
			get { return TimeNode.EndTime; }
		}

		public IFilterState CreateFilterState(TimeSpan filterRelativeTime) {
			return PreFilter.CreateFilterState(filterRelativeTime);
		}

		#region IComparable<IPreFilterNode>
		public int CompareTo(IPreFilterNode other) {
			return CompareTo((IDataNode)other);
		}
		#endregion

		#region IComparable<IDataNode>
		public int CompareTo(IDataNode other) {
			return TimeNode.CompareTo(other.TimeNode);
		}
		#endregion
	}

	public interface IPreFilterNode : IDataNode, IComparable<IPreFilterNode> {
		IPreFilterModuleInstance PreFilter { get; }
		IFilterState CreateFilterState(TimeSpan filterRelativeTime);
	}
}
