using System;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Combinator._64Bit {
	public class _64BitHighestWinsCombinator : Combinator<_64BitHighestWinsCombinator, ulong> {
		public override void Handle(IEvaluator<byte> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _64BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				ulong value1 = CombinatorValue.CommandValue;
				ulong value2 = obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<System.Drawing.Color> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _64BitCommand(ColorValue.GetGrayscaleLevel(obj.EvaluatorValue.CommandValue));
			} else {
				ulong value1 = CombinatorValue.CommandValue;
				ulong value2 = ColorValue.GetGrayscaleLevel(obj.EvaluatorValue.CommandValue);
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<uint> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _64BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				ulong value1 = CombinatorValue.CommandValue;
				ulong value2 = (byte)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<ulong> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = obj.EvaluatorValue;
			} else {
				ulong value1 = CombinatorValue.CommandValue;
				ulong value2 = (byte)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<ushort> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _64BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				ulong value1 = CombinatorValue.CommandValue;
				ulong value2 = (byte)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}
	}
}
