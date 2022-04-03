using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	public class _16BitEvaluator : Evaluator
	{
		public override void Handle(IIntentState<RGBValue> obj)
		{
			System.UInt16 level = (System.UInt16)(System.UInt16.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = new _16BitCommand(level);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			EvaluatorValue = new _16BitCommand((ushort)(ushort.MaxValue * obj.GetValue().Intensity));
		}

		public override void Handle(IIntentState<RangeValue<FunctionIdentity>> obj)
		{
			EvaluatorValue = new _16BitCommand((ushort)(ushort.MaxValue * obj.GetValue().Value));
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			EvaluatorValue = new _16BitCommand((ushort)(ushort.MaxValue * obj.GetValue().Intensity));
		}

		public override void Handle(IIntentState<IntensityValue> obj)
		{
			EvaluatorValue = new _16BitCommand((ushort)(ushort.MaxValue * obj.GetValue().Intensity));
		}
	}
}