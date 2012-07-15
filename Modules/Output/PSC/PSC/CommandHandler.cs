using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace PSC {
	class CommandHandler : CommandDispatch {
		// PSC channel range = 250-1250
		public const int MinValue = 250;
		public const int MaxValue = 1250;
		private const int _MidValue = MinValue + (MaxValue - MinValue)/2;

		public double Value { get; private set; }

		public void Reset() {
			Value = _MidValue;
		}

		public override void Handle(DoubleValueCommand obj) {
			Value = MinValue + (MaxValue - MinValue) * obj.CommandValue;
		}
	}
}
