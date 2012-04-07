using System.Drawing;
using System.Linq;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Evaluator {
	public abstract class Evaluator<T, ResultType> : Dispatchable<T>, IEvaluator<ResultType>, IAnyIntentStateHandler
		where T : Evaluator<T, ResultType> {
		public void Evaluate(IIntentState intentState) {
			intentState.Dispatch(this);
		}

		// Opt-in, not opt-out.  Default handlers will not be called
		// from the base class.
		virtual public void Handle(IIntentState<float> obj) { }

		virtual public void Handle(IIntentState<System.DateTime> obj) { }

		virtual public void Handle(IIntentState<Color> obj) { }

		virtual public void Handle(IIntentState<long> obj) { }

		virtual public void Handle(IIntentState<double> obj) { }

		public ResultType Value { get; protected set; }
	}

	static public class Evaluator {
		static public float Default(IIntentState<float> intentState) {
			return intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));
		}

		static public long Default(IIntentState<long> intentState) {
			return intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));
		}

		static public int ColorAsInt(IIntentState<Color> intentState) {
			Color color = intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));

			// Stripping the alpha quantity to keep it from going negative.
			return color.ToArgb() & 0xffffff;
		}

		static public Color Default(IIntentState<Color> intentState) {
			return intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));
		}

		static public double Default(IIntentState<double> intentState) {
			return intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));
		}
	}
}
