using Vixen.Commands;

namespace Vixen.Sys.Dispatch {
	abstract public class CommandDispatch : IAnyCommandHandler {
		virtual public void Handle(ByteValue obj) { }

		virtual public void Handle(SignedShortValue obj) { }

		virtual public void Handle(UnsignedShortValue obj) { }

		virtual public void Handle(SignedIntValue obj) { }

		virtual public void Handle(UnsignedIntValue obj) { }

		virtual public void Handle(SignedLongValue obj) { }

		virtual public void Handle(UnsignedLongValue obj) { }

		virtual public void Handle(ColorValue obj) { }

		virtual public void Handle(LightingValueCommand obj) { }

		virtual public void Handle(DoubleValue obj) { }
	}
}
