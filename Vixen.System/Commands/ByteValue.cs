using Vixen.Sys;

namespace Vixen.Commands {
	public class ByteValue : Dispatchable<ByteValue>, ICommand<byte> {
		public ByteValue(byte value) {
			Value = value;
		}

		public byte Value { get; set; }

		object ICommand.Value {
			get { return Value; }
			set { Value = (byte)value; }
		}
	}
}
