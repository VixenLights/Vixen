using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator {
	//public class _64BitEvaluator : Evaluator<_64BitEvaluator, ulong> {
	public class _64BitEvaluator : Evaluator {
		public override void Handle(IIntentState<ColorValue> obj) {
			byte byteLevel = ColorValue.GetGrayscaleLevel(obj.GetValue().Color);
			EvaluatorValue = new _64BitCommand(byteLevel);
		}

		public override void Handle(IIntentState<LightingValue> obj) {
			EvaluatorValue = new _64BitCommand(obj.GetValue().Intensity);
		}

		public override void Handle(IIntentState<PositionValue> obj) {
			EvaluatorValue = new _64BitCommand((ulong)(ulong.MaxValue * obj.GetValue().Position));
		}

		public override void Handle(IIntentState<CommandValue> obj) {
			obj.GetValue().Command.Dispatch(this);
		}

		public void Handle(_8BitCommand obj) {
			EvaluatorValue = new _64BitCommand(obj.CommandValue);
		}

		public void Handle(_16BitCommand obj) {
			EvaluatorValue = new _64BitCommand(obj.CommandValue);
		}

		public void Handle(_32BitCommand obj) {
			EvaluatorValue = new _64BitCommand(obj.CommandValue);
		}

		public void Handle(_64BitCommand obj) {
			EvaluatorValue = obj;
		}

		public void Handle(ColorCommand obj) {
			EvaluatorValue = new _64BitCommand(ColorValue.GetGrayscaleLevel(obj.CommandValue));
		}
	}
}
