using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.DummyLighting {
	class CommandHandler : CommandDispatch {
		public void Reset() {
			ByteValue = 0;
		}

		public override void Handle(ByteValue obj) {
			ByteValue = obj.CommandValue;
		}

		public byte ByteValue;
	}
}
