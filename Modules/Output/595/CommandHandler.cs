using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.Olsen595 {
	class CommandHandler : CommandDispatch {
		public short Value;

		public void Reset() {
			Value = 0;
		}

		public override void Handle(LightingValueCommand obj) {
			Value = (obj.CommandValue.Intensity > 0) ? (short)1 : (short)0;
		}
	}
}
