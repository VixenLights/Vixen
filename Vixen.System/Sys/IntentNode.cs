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

		//contextAbsoluteEffectStartTime = effectNode.StartTime
		public void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteEffectStartTime)
		{
			// Pre-filters have context-absolute timing, so the intent needs to be told
			// where in context-absolute time it is.
			Intent.ApplyFilter(sequenceFilterNode, contextAbsoluteEffectStartTime + StartTime);
		}

		public IIntentNode[] DivideAt(TimeSpan effectRelativeTime)
		{
			if (effectRelativeTime > StartTime && effectRelativeTime < EndTime) {
				TimeSpan intentRelativeTime = Helper.GetIntentRelativeTime(effectRelativeTime, this);
				IIntent[] intents = Intent.DivideAt(intentRelativeTime);
				return new[]
				       	{
				       		new IntentNode(intents[0], StartTime),
				       		new IntentNode(intents[1], effectRelativeTime)
				       	};
			}
			return null;
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
		void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteEffectStartTime);
		IIntentNode[] DivideAt(TimeSpan effectRelativeTime);
		void OffSetTime(TimeSpan offset);
	}
}