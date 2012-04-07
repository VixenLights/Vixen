using System.Drawing;
using Vixen.Sys;

namespace Vixen.Data.Evaluator {
	public class ColorEvaluator : Evaluator<ColorEvaluator, Color> {
		override public void Handle(IIntentState<float> obj) {
			float value = Evaluator.Default(obj);
			Value = Helper.ConvertToGrayscale(value);
		}

		override public void Handle(IIntentState<Color> obj) {
			Value = Evaluator.Default(obj);
		}

		override public void Handle(IIntentState<long> obj) {
			long value = Evaluator.Default(obj);
			Value = Helper.ConvertToGrayscale(value);
		}

		override public void Handle(IIntentState<double> obj) {
			double value = Evaluator.Default(obj);
			Value = Helper.ConvertToGrayscale(value);
		}
	}
	//public class ColorEvaluator : Dispatchable<ColorEvaluator>, IEvaluator<Color>, IAnyIntentStateHandler {
	//    public Color Value { get; private set; }

	//    public void Evaluate(IIntentState intentState) {
	//        intentState.Dispatch(this);
	//    }

	//    public void Handle(IIntentState<float> obj) {
	//        float value = Evaluator.Default(obj);
	//        Value = Helper.ConvertToGrayscale(value);
	//    }

	//    public void Handle(IIntentState<DateTime> obj) {
	//        // Ignored
	//    }

	//    public void Handle(IIntentState<Color> obj) {
	//        Value = Evaluator.Default(obj);
	//    }

	//    public void Handle(IIntentState<long> obj) {
	//        long value = Evaluator.Default(obj);
	//        Value = Helper.ConvertToGrayscale(value);
	//    }

	//    public void Handle(IIntentState<double> obj) {
	//        double value = Evaluator.Default(obj);
	//        Value = Helper.ConvertToGrayscale(value);
	//    }
	//}
}
