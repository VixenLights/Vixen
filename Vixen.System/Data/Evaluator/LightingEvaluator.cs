using System.Drawing;
using Vixen.Sys;

namespace Vixen.Data.Evaluator {
	public class LightingEvaluator : Evaluator<LightingEvaluator, LightingValue> {
		override public void Handle(IIntentState<float> obj) {
			float value = Evaluator.Default(obj);
			double intensity = (double)Helper.ConvertToByte(value) / byte.MaxValue;
			EvaluatorValue = new LightingValue(Color.White, intensity);
		}

		override public void Handle(IIntentState<Color> obj) {
			EvaluatorValue = new LightingValue(Evaluator.Default(obj), 1);
		}

		override public void Handle(IIntentState<long> obj) {
			long value = Evaluator.Default(obj);
			double intensity = (double)Helper.ConvertToByte(value) / byte.MaxValue;
			EvaluatorValue = new LightingValue(Color.White, intensity);
		}

		override public void Handle(IIntentState<double> obj) {
			double intensity = Evaluator.Default(obj);
			EvaluatorValue = new LightingValue(Color.White, intensity);
		}

		public override void Handle(IIntentState<LightingValue> obj) {
			EvaluatorValue = Evaluator.Default(obj);
		}
	}
}
