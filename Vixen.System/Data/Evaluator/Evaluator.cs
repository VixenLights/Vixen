using System.Drawing;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Data.Evaluator {
	class Evaluator {
		//static public T NumericDefault<T>(IIntentState<T> intentState) {
		//    return intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));
		//}
		static public float Default(IIntentState<float> intentState) {
			return intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));
		}

		static public long Default(IIntentState<long> intentState) {
			return intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));
		}

		static public int Default(IIntentState<Color> intentState) {
			Color color = intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));

			// Stripping the alpha quantity to keep it from going negative.
			return color.ToArgb() & 0xffffff;
		}

		static public double Default(IIntentState<double> intentState) {
			return intentState.FilterStates.Aggregate(intentState.GetValue(), (current, filterState) => filterState.Affect(current));
		}
	}
}
