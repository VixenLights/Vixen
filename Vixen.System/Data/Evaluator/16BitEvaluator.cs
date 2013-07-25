using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	public class _16BitEvaluator : Evaluator
	{
		public override void Handle(IIntentState<ColorValue> obj)
		{
			byte byteLevel = ColorValue.GetGrayscaleLevel(obj.GetValue().Color);
			EvaluatorValue = new _16BitCommand(byteLevel);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			EvaluatorValue = new _16BitCommand((ushort)(ushort.MaxValue * obj.GetValue().Intensity));
		}

		public override void Handle(IIntentState<PositionValue> obj)
		{
			EvaluatorValue = new _16BitCommand((ushort)(ushort.MaxValue * obj.GetValue().Position));
		}
	}
}