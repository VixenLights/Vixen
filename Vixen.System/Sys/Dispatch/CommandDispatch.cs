using Vixen.Commands;

namespace Vixen.Sys.Dispatch {
	abstract public class CommandDispatch : IHandler<ByteValue>, IHandler<SignedShortValue>, IHandler<UnsignedShortValue>, IHandler<SignedIntValue>, IHandler<UnsignedIntValue>, IHandler<SignedLongValue>, IHandler<UnsignedLongValue>, IHandler<ColorValue> {
		virtual public void Handle(ByteValue c) { }

		virtual public void Handle(SignedShortValue c) { }

		virtual public void Handle(UnsignedShortValue c) { }

		virtual public void Handle(SignedIntValue c) { }

		virtual public void Handle(UnsignedIntValue c) { }

		virtual public void Handle(SignedLongValue c) { }

		virtual public void Handle(UnsignedLongValue c) { }

		virtual public void Handle(ColorValue c) { }
	}
}
