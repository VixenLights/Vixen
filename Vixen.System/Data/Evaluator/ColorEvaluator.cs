using System.Drawing;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator {
	public class ColorEvaluator : Evaluator<ColorEvaluator, Color> {
		public override void Handle(IIntentState<ColorValue> obj) {
			EvaluatorValue = new ColorCommand(obj.GetValue().Color);
		}

		public override void Handle(IIntentState<LightingValue> obj) {
			EvaluatorValue = new ColorCommand(obj.GetValue().Color);
		}
	}
}
