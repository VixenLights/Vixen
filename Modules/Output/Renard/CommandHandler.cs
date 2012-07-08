using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.Renard {
	class CommandHandler : CommandDispatch {
		public byte Value { get; private set; }

		public void Reset() {
			Value = 0;
		}

		public override void Handle(LightingValueCommand obj) {
			Value = (byte)(byte.MaxValue * obj.CommandValue.Intensity);
		}
	}
}
