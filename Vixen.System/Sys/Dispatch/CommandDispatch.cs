using Vixen.Commands;

namespace Vixen.Sys.Dispatch {
	abstract public class CommandDispatch : IAnyCommandHandler {
		virtual public void Handle(_8BitCommand obj) { }

		virtual public void Handle(_16BitCommand obj) { }

		virtual public void Handle(_32BitCommand obj) { }

		virtual public void Handle(_64BitCommand obj) { }

		virtual public void Handle(ColorCommand obj) { }
	}
}
