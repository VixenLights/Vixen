using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	public class _32BitEvaluator : Evaluator
	{
		public override void Handle(IIntentState<RGBValue> obj)
		{
			System.UInt32 level = (System.UInt32)(System.UInt32.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = new _32BitCommand(level);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			EvaluatorValue = new _32BitCommand((uint)(uint.MaxValue * obj.GetValue().Intensity));
		}
		
		public override void Handle(IIntentState<RangeValue<FunctionIdentity>> obj)
		{
			EvaluatorValue = new _32BitCommand((uint)(uint.MaxValue * obj.GetValue().Value));
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			EvaluatorValue = new _32BitCommand((uint)(uint.MaxValue * obj.GetValue().Intensity));
		}

		public override void Handle(IIntentState<IntensityValue> obj)
		{
			EvaluatorValue = new _32BitCommand((uint)(uint.MaxValue * obj.GetValue().Intensity));
		}
	}
}