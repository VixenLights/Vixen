using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.DummyLighting {
	class CommandHandler : CommandDispatch {
		public void Reset() {
			Value = 0;
		}

		public override void Handle(LightingValueCommand obj) {
			Value = (byte)(obj.CommandValue.Intensity * byte.MaxValue);
		}

		public override void Handle(ByteValueCommand obj) {
			Value = obj.CommandValue;
		}

		public byte Value;
	}
}
