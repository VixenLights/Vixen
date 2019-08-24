using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	public class _8BitEvaluator : Evaluator
	{
		// Handling intents as an evaluator.
		public override void Handle(IIntentState<RGBValue> obj)
		{
			var i = (byte) (byte.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = CommandLookup8BitEvaluator.CommandLookup[i];
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			var i = (byte)(byte.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = CommandLookup8BitEvaluator.CommandLookup[i];
		}

		public override void Handle(IIntentState<PositionValue> obj)
		{
			var i = (byte)(byte.MaxValue * obj.GetValue().Position);
			EvaluatorValue = CommandLookup8BitEvaluator.CommandLookup[i];
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			var i = (byte)(byte.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = CommandLookup8BitEvaluator.CommandLookup[i];
		}

		public override void Handle(IIntentState<IntensityValue> obj)
		{
			var i = (byte)(byte.MaxValue * obj.GetValue().Intensity);
			EvaluatorValue = CommandLookup8BitEvaluator.CommandLookup[i];
		}

	}
}