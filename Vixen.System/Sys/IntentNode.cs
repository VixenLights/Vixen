using System;
using System.Collections.Generic;
using Vixen.Sys.LayerMixing;

namespace Vixen.Sys
{
	public class IntentNode : IIntentNode, IComparer<IntentNode>
	{
		public IntentNode(IIntent intent, TimeSpan startTime)
		{
			Intent = intent;
			StartTime = startTime;
			EndTime = StartTime + TimeSpan;
		}

		public IIntent Intent { get; private set; }

		public TimeSpan StartTime { get; private set; }

		public TimeSpan TimeSpan
		{
			get { return Intent.TimeSpan; }
		}

		public TimeSpan EndTime { get; private set; }

		public void OffSetTime(TimeSpan offset)
		{
			StartTime = StartTime+offset;
			EndTime = StartTime + TimeSpan;
		}

		public virtual IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer)
		{
			IIntentState intentState = Intent.CreateIntentState(intentRelativeTime, layer);

			return intentState;
		}

		#region IComparable<IIntentNode>

		public int CompareTo(IIntentNode other)
		{
			return DataNode.Compare(this, other);
		}

		#endregion

		#region IComparer<IntentNode>

		public int Compare(IntentNode x, IntentNode y)
		{
			return DataNode.Compare(x, y);
		}

		#endregion
	}

	public interface IIntentNode : IDataNode, IComparable<IIntentNode>
	{
		IIntent Intent { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer);
		void OffSetTime(TimeSpan offset);
	}
}