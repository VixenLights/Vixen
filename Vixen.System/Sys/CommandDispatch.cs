using Vixen.Commands;

namespace Vixen.Sys {
	abstract public class CommandDispatch : IHandler<ByteValue>, IHandler<SignedShortValue>, IHandler<UnsignedShortValue>, IHandler<SignedIntValue>, IHandler<UnsignedIntValue>, IHandler<SignedLongValue>, IHandler<UnsignedLongValue> {
		//Can't differentiate between the different commands doing this
		//public virtual void DispatchCommand<T>(ICommand<T> command) {
		//}

		//public virtual void DispatchCommand(ICommand<byte> command) {
		//}

		//Then the method using this can't handle the commands using a uniform interface.
		//public virtual void Handle(ICommand<byte> c) {
		//}

		virtual public void Handle(ByteValue c) {
		}

		virtual public void Handle(SignedShortValue c) {
		}

		virtual public void Handle(UnsignedShortValue c) {
		}

		virtual public void Handle(SignedIntValue c) {
		}

		virtual public void Handle(UnsignedIntValue c) {
		}

		virtual public void Handle(SignedLongValue c) {
		}

		virtual public void Handle(UnsignedLongValue c) {
		}
	}
}
