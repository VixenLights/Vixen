using System;
using Vixen.Commands;
using Vixen.Data.Combinator;
using Vixen.Sys;

namespace VixenModules.Controller.PSC {
	class Combinator : Combinator<Combinator, ushort> {
		public override void Handle(IEvaluator<ushort> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = obj.EvaluatorValue;
			} else {
				CombinatorValue = new _16BitCommand(_GetMidpointBetween(CombinatorValue.CommandValue, obj.EvaluatorValue.CommandValue));
			}
		}

		private ushort _GetMidpointBetween(ushort value1, ushort value2) {
			return (ushort)(Math.Abs(value1 - value2) / 2);
		}
	}
}
