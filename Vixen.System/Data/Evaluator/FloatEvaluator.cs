using System.Drawing;
using Vixen.Sys;

namespace Vixen.Data.Evaluator {
	public class FloatEvaluator : Evaluator<FloatEvaluator, float> {
		override public void Handle(IIntentState<float> obj) {
			Value = Evaluator.Default(obj);
		}

		override public void Handle(IIntentState<Color> obj) {
			Value = Evaluator.ColorAsInt(obj);
		}

		override public void Handle(IIntentState<long> obj) {
			Value = Evaluator.Default(obj);
		}
	}
	//public class FloatEvaluator : Dispatchable<FloatEvaluator>, IEvaluator<float>, IAnyIntentStateHandler {
	//    public float Value { get; private set; }

	//    public void Evaluate(IIntentState intentState) {
	//        intentState.Dispatch(this);
	//    }

	//    public void Handle(IIntentState<float> obj) {
	//        //Value = obj.FilterStates.Aggregate(obj.GetValue(), (current, filterState) => filterState.Affect(current));
	//        Value = Evaluator.Default(obj);
	//    }

	//    public void Handle(IIntentState<DateTime> obj) {
	//        // Ignored
	//    }

	//    public void Handle(IIntentState<Color> obj) {
	//        //Color color = obj.GetValue();

	//        //color = obj.FilterStates.Aggregate(color, (current, filterState) => filterState.Affect(current));

	//        //// Stripping the alpha quantity to keep it from going negative.
	//        //Value = color.ToArgb() & 0xffffff;
	//        Value = Evaluator.ColorAsInt(obj);
	//    }

	//    public void Handle(IIntentState<long> obj) {
	//        //Value = obj.FilterStates.Aggregate(obj.GetValue(), (current, filterState) => filterState.Affect(current));
	//        Value = Evaluator.Default(obj);
	//    }

	//    public void Handle(IIntentState<double> obj) {
	//        // Ignored
	//        // It would make the double a float.
	//        // A double is used to represent a % where a float is a float value.
	//        // So, 30% would become a float value of 0.3 and be interpreted as that
	//        // value from here on out.
	//    }
	//}
}
