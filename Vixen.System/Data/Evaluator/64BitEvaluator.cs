using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	//public class _64BitEvaluator : Evaluator<_64BitEvaluator, ulong> {
	public class _64BitEvaluator : Evaluator
	{
		public override void Handle(IIntentState<ColorValue> obj)
		{
			byte byteLevel = ColorValue.GetGrayscaleLevel(obj.GetValue().Color);
			EvaluatorValue = new _64BitCommand(byteLevel);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			EvaluatorValue = new _64BitCommand((ulong)(ulong.MaxValue * obj.GetValue().Intensity));
		}

		public override void Handle(IIntentState<PositionValue> obj)
		{
			EvaluatorValue = new _64BitCommand((ulong)(ulong.MaxValue * obj.GetValue().Position));
		}
	}
}