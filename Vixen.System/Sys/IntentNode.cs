using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public class IntentNode : IIntentNode, IComparer<IntentNode> {
		public IntentNode(IIntent intent, TimeSpan startTime) {
			Intent = intent;
			StartTime = startTime;
			//Subordinates may be supported later, but not yet.
			//SubordinateIntents = new List<SubordinateIntent>();
		}

		public IIntent Intent { get; private set; }

		public TimeSpan StartTime { get; private set; }

		public TimeSpan TimeSpan {
			get { return Intent.TimeSpan; }
		}

		public TimeSpan EndTime {
			get { return StartTime + TimeSpan; }
		}

		//contextAbsoluteEffectStartTime = effectNode.StartTime
		public void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteEffectStartTime) {
			// Pre-filters have context-absolute timing, so the intent needs to be told
			// where in context-absolute time it is.
			Intent.ApplyFilter(sequenceFilterNode, contextAbsoluteEffectStartTime + StartTime);
		}

		virtual public IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			IIntentState intentState = Intent.CreateIntentState(intentRelativeTime);

			//intentState.SubordinateIntentStates.AddRange(_GetSubordinateIntentStates(intentRelativeTime));

			return intentState;
		}

		#region Subordinate support
		//private IEnumerable<SubordinateIntentState> _GetSubordinateIntentStates(TimeSpan intentRelativeTime) {
		//    return SubordinateIntents.Select(x => _GetSubordinateIntentState(intentRelativeTime, x));
		//}

		//private SubordinateIntentState _GetSubordinateIntentState(TimeSpan intentRelativeTime, SubordinateIntent subordinateIntent) {
		//    TimeSpan otherIntentRelativeTime = Helper.TranslateIntentRelativeTime(intentRelativeTime, this, subordinateIntent.IntentNode);
		//    IIntentState otherIntentState = subordinateIntent.IntentNode.CreateIntentState(otherIntentRelativeTime);
		//    SubordinateIntentState subordinateIntentState = new SubordinateIntentState(otherIntentState, subordinateIntent.CombinationOperation);
		//    return subordinateIntentState;
		//}

		//public List<SubordinateIntent> SubordinateIntents { get; private set; }
		#endregion

		#region IComparable<IIntentNode>
		public int CompareTo(IIntentNode other) {
			return DataNode.Compare(this, other);
		}
		#endregion

		#region IComparer<IntentNode>
		public int Compare(IntentNode x, IntentNode y) {
			return DataNode.Compare(x, y);
		}
		#endregion
	}

	public interface IIntentNode : IDataNode, IComparable<IIntentNode> {
		IIntent Intent { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime);
		//List<SubordinateIntent> SubordinateIntents { get; }
		void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteEffectStartTime);
	}
}
