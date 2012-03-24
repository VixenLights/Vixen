using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	public class IntentNode : IIntentNode {
		public IntentNode(IIntent intent, TimeSpan startTime) {
			Intent = intent;
			StartTime = startTime;
			SubordinateIntents = new List<SubordinateIntent>();
		}

		public IIntent Intent { get; private set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan TimeSpan {
			get { return (Intent != null) ? Intent.TimeSpan : TimeSpan.Zero; }
		}

		public TimeSpan EndTime { 
			get { return (Intent != null) ? StartTime + TimeSpan : StartTime; }
		}

		virtual public IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			IIntentState intentState = Intent.CreateIntentState(intentRelativeTime);

			intentState.SubordinateIntentStates.AddRange(_GetSubordinateIntentStates(intentRelativeTime));

			return intentState;
		}

		private IEnumerable<SubordinateIntentState> _GetSubordinateIntentStates(TimeSpan intentRelativeTime) {
			return SubordinateIntents.Select(x => _GetSubordinateIntentState(intentRelativeTime, x));
		}

		private SubordinateIntentState _GetSubordinateIntentState(TimeSpan intentRelativeTime, SubordinateIntent subordinateIntent) {
			TimeSpan otherIntentRelativeTime = Helper.TranslateIntentRelativeTime(intentRelativeTime, this, subordinateIntent.IntentNode);
			IIntentState otherIntentState = subordinateIntent.IntentNode.CreateIntentState(otherIntentRelativeTime);
			SubordinateIntentState subordinateIntentState = new SubordinateIntentState(otherIntentState, subordinateIntent.CombinationOperation);
			return subordinateIntentState;
		}

		public List<SubordinateIntent> SubordinateIntents { get; private set; }

		#region IComparer<IntentNode>
		public class Comparer : IComparer<IIntentNode> {
			public int Compare(IIntentNode x, IIntentNode y) {
				return x.StartTime.CompareTo(y.StartTime);
			}
		}
		#endregion

		#region IComparable<IntentNode>
		public int CompareTo(IIntentNode other) {
			return StartTime.CompareTo(other.StartTime);
		}
		#endregion
	}

	public interface IIntentNode : IDataNode, IComparable<IIntentNode> {
		IIntent Intent { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime);
		List<SubordinateIntent> SubordinateIntents { get; }
	}

	//public interface IIntentNode<T> : IIntentNode
	//    where T : IIntent {
	//    new T Intent { get; }
	//}

}
