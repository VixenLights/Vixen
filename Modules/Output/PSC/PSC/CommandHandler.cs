using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Controller.PSC {
	class CommandHandler : CommandDispatch {
		public ushort Value { get; private set; }

		public void Reset() {
			Value = PSC.RangeMid;
		}

		public override void Handle(_16BitCommand obj) {
			Value = obj.CommandValue;
		}
	}
}
