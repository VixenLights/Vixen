using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.Controller.PSC
{
	internal class PositionEvaluator : Evaluator
	{
		public override void Handle(IIntentState<RangeValue<FunctionIdentity>> obj)
		{
			EvaluatorValue = new _16BitCommand(PSC.RangeLow + obj.GetValue().Value * PSC.RangeWidth);
		}
	}
}