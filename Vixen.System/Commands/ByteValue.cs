using Vixen.Sys;

namespace Vixen.Commands {
	public class ByteValue : Dispatchable<ByteValue>, ICommand {
		public ByteValue(byte value) {
			Value = value;
		}

		public byte Value { get; private set; }
	}
}
