using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	public class _8BitEvaluator : Evaluator
	{
		// Handling intents as an evaluator.
		public override void Handle(IIntentState<RGBValue> obj)
		{
			byte byteLevel = (byte)(byte.MaxValue * obj.GetValue().Intensity);
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

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			EvaluatorValue = new _8BitCommand((byte)(byte.MaxValue * obj.GetValue().Intensity));
		}

		public override void Handle(IIntentState<IntensityValue> obj)
		{
			EvaluatorValue = new _8BitCommand((byte)(byte.MaxValue * obj.GetValue().Intensity));
		}

	}
}