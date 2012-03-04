using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public class IntentNode : IIntentNode {
		public IntentNode(IIntent intent, TimeSpan startTime) {
			Intent = intent;
			StartTime = startTime;
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
			return Intent.CreateIntentState(intentRelativeTime);
		}

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

	//abstract public class IntentNode<T> : IntentNode, IIntentNode<T> where T : IIntent {
	//    protected IntentNode(T intent, TimeSpan timeSpan)
	//        : base(intent, timeSpan) {
	//        Intent = intent;
	//    }

	//    new public T Intent { get; private set; }

	//    override public IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
	//        return Intent.CreateIntentState(intentRelativeTime);
	//    }
	//}

	//-------

	public interface IIntentNode : IDataNode, IComparable<IIntentNode> {
		IIntent Intent { get; }

		//TimeSpan StartTime { get; set; }

		//TimeSpan TimeSpan { get; }

		//TimeSpan EndTime { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime);
	}

	public interface IIntentNode<out T> : IIntentNode
		where T : IIntent {
		new T Intent { get; }
	}

}
