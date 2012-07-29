using System;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Combinator._32Bit {
	public class _32BitHighestWinsCombinator : Combinator<_32BitHighestWinsCombinator, uint> {
		public override void Handle(IEvaluator<byte> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _32BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				uint value1 = CombinatorValue.CommandValue;
				uint value2 = obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<System.Drawing.Color> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _32BitCommand(ColorValue.GetGrayscaleLevel(obj.EvaluatorValue.CommandValue));
			} else {
				uint value1 = CombinatorValue.CommandValue;
				uint value2 = ColorValue.GetGrayscaleLevel(obj.EvaluatorValue.CommandValue);
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<uint> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = obj.EvaluatorValue;
			} else {
				uint value1 = CombinatorValue.CommandValue;
				uint value2 = (byte)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<ulong> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _32BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				uint value1 = CombinatorValue.CommandValue;
				uint value2 = (byte)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<ushort> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _32BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				uint value1 = CombinatorValue.CommandValue;
				uint value2 = (byte)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}
	}
}
