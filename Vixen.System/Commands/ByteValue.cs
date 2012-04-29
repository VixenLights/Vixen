using Vixen.Sys;

namespace Vixen.Commands {
	public class ByteValue : Dispatchable<ByteValue>, ICommand<byte> {
		public ByteValue(byte value) {
			CommandValue = value;
		}

		public ByteValue(int value) {
			CommandValue = (byte)value;
		}

		public ByteValue(float value) {
			CommandValue = (byte)value;
		}

		public ByteValue(long value) {
			CommandValue = (byte)value;
		}

		public byte CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (byte)value; }
		}
	}
}
