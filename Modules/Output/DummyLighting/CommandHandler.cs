using System.Drawing;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.DummyLighting {
	class CommandHandler : CommandDispatch {
		public void Reset() {
			ByteValue = 0;
			ColorValue = Color.Transparent;
		}

		public override void Handle(LightingValueCommand obj) {
			ByteValue = (byte)(obj.CommandValue.Intensity * byte.MaxValue);
			ColorValue = obj.GetIntensityAffectedValue();
		}

		public override void Handle(ByteValueCommand obj) {
			ByteValue = obj.CommandValue;
			ColorValue = Color.White;
		}

		public byte ByteValue;
		public Color ColorValue;
	}
}
