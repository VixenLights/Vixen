using Vixen.Sys;

namespace Vixen.Commands {
	public class ByteValueCommand : Dispatchable<ByteValueCommand>, ICommand<byte> {
		public ByteValueCommand(byte value) {
			CommandValue = value;
		}

		public ByteValueCommand(int value) {
			CommandValue = (byte)value;
		}

		public ByteValueCommand(float value) {
			CommandValue = (byte)value;
		}

		public ByteValueCommand(long value) {
			CommandValue = (byte)value;
		}

		public byte CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (byte)value; }
		}
	}
}
