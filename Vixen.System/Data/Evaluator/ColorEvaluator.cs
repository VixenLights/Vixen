using System.Drawing;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator {
	//public class ColorEvaluator : Evaluator<ColorEvaluator, Color> {
	public class ColorEvaluator : Evaluator {
		public override void Handle(IIntentState<ColorValue> obj) {
			EvaluatorValue = new ColorCommand(obj.GetValue().Color);
		}

		public override void Handle(IIntentState<LightingValue> obj) {
			LightingValue lightingValue = obj.GetValue();
			int alphaValue = (int)(lightingValue.Intensity * byte.MaxValue);
			EvaluatorValue = new ColorCommand(Color.FromArgb(alphaValue, lightingValue.Color));
		}
	}
}
