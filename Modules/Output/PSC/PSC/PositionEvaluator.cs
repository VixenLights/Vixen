using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.Controller.PSC {
	class PositionEvaluator : Evaluator {
		public override void Handle(IIntentState<PositionValue> obj) {
			EvaluatorValue = new _16BitCommand(PSC.RangeLow + obj.GetValue().Position * PSC.RangeWidth);
		}
	}
}
