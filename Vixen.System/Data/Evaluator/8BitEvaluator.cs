using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Evaluator
{
	public class _8BitEvaluator : Evaluator
	{
		// Handling intents as an evaluator.
		public override void Handle(IIntentState<ColorValue> obj)
		{
			byte byteLevel = ColorValue.GetGrayscaleLevel(obj.GetValue().Color);
			EvaluatorValue = new _8BitCommand(byteLevel);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			EvaluatorValue = new _8BitCommand((byte)(byte.MaxValue * obj.GetValue().Intensity));
		}

		public override void Handle(IIntentState<PositionValue> obj)
		{
			EvaluatorValue = new _8BitCommand((byte) (byte.MaxValue*obj.GetValue().Position));
		}
	}
}