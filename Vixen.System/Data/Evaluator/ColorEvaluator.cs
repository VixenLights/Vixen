using System.Drawing;
using Vixen.Sys;

namespace Vixen.Data.Evaluator {
	public class ColorEvaluator : Evaluator<ColorEvaluator, Color> {
		override public void Handle(IIntentState<float> obj) {
			float value = Evaluator.Default(obj);
			EvaluatorValue = Helper.ConvertToGrayscale(value);
		}

		override public void Handle(IIntentState<Color> obj) {
			EvaluatorValue = Evaluator.Default(obj);
		}

		override public void Handle(IIntentState<long> obj) {
			long value = Evaluator.Default(obj);
			EvaluatorValue = Helper.ConvertToGrayscale(value);
		}

		override public void Handle(IIntentState<double> obj) {
			double value = Evaluator.Default(obj);
			EvaluatorValue = Helper.ConvertToGrayscale(value);
		}

		public override void Handle(IIntentState<LightingValue> obj) {
			LightingValue value = Evaluator.Default(obj);
			EvaluatorValue = value.Color;
		}
	}
}
