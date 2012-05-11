using Vixen.Commands;

namespace Vixen.Sys.Dispatch {
	abstract public class CommandDispatch : IAnyCommandHandler {
		virtual public void Handle(ByteValueCommand obj) { }

		virtual public void Handle(SignedShortValueCommand obj) { }

		virtual public void Handle(UnsignedShortValueCommand obj) { }

		virtual public void Handle(SignedIntValueCommand obj) { }

		virtual public void Handle(UnsignedIntValueCommand obj) { }

		virtual public void Handle(SignedLongValueCommand obj) { }

		virtual public void Handle(UnsignedLongValueCommand obj) { }

		virtual public void Handle(ColorValueCommand obj) { }

		virtual public void Handle(LightingValueCommand obj) { }

		virtual public void Handle(DoubleValueCommand obj) { }
	}
}
