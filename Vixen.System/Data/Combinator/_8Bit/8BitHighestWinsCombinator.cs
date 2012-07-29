using System;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Data.Combinator._8Bit {
	public class _8BitHighestWinsCombinator : Combinator<_8BitHighestWinsCombinator, byte> {
		public override void Handle(IEvaluator<byte> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = obj.EvaluatorValue;
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<System.Drawing.Color> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(ColorValue.GetGrayscaleLevel(obj.EvaluatorValue.CommandValue));
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = ColorValue.GetGrayscaleLevel(obj.EvaluatorValue.CommandValue);
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<uint> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = (byte)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<ulong> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = (byte)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(IEvaluator<ushort> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(obj.EvaluatorValue.CommandValue);
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = (byte)obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}
	}
}
