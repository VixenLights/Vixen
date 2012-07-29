using System;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Combinator._16Bit {
	public class _16BitHighestWinsCombinator : Combinator<_16BitHighestWinsCombinator, ushort> {
		public override void Handle(IEvaluator<byte> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _16BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				ushort value1 = CombinatorValue.CommandValue;
				ushort value2 = obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<System.Drawing.Color> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _16BitCommand(ColorValue.GetGrayscaleLevel(obj.EvaluatorValue.CommandValue));
			} else {
				ushort value1 = CombinatorValue.CommandValue;
				ushort value2 = ColorValue.GetGrayscaleLevel(obj.EvaluatorValue.CommandValue);
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<uint> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _16BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				ushort value1 = CombinatorValue.CommandValue;
				ushort value2 = (ushort)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<ulong> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _16BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				ushort value1 = CombinatorValue.CommandValue;
				ushort value2 = (ushort)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<ushort> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = obj.EvaluatorValue;
			} else {
				ushort value1 = CombinatorValue.CommandValue;
				ushort value2 = obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}
	}
}
