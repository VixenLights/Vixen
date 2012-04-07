namespace Vixen.Sys.CombinationOperation {
	class DoubleValueCombiner : Combiner<double> {
		override public double Combine(double value, IIntentState intentState) {
			Value = value * 100;
			intentState.Dispatch(this);
			return Value / 100;
		}
	}
}
