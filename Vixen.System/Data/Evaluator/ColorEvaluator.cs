using System.Drawing;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Evaluator
{
	public class ColorEvaluator : Evaluator
	{
		public override void Handle(IIntentState<RGBValue> obj)
		{
			EvaluatorValue = new ColorCommand(obj.GetValue().FullColor);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			EvaluatorValue = new ColorCommand(lightingValue.FullColor);
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			DiscreteValue lightingValue = obj.GetValue();
			EvaluatorValue = new ColorCommand(lightingValue.FullColor);
		}
	}
}