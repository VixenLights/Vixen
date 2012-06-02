using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Commands {
	public class Command<T> : ICommand<T> {
		public Command(T value) {
			Value = value;
		}

		public T Value { get; private set; }

		//would have to be done in a derived class because ICommand<T> doesn't match the
		//signature of any method in the called object
		public void Dispatch(CommandDispatch commandDispatch) {
			if(commandDispatch != null)
				commandDispatch.DispatchCommand(this);
		}
	}

	//public abstract class Command {
	//    abstract public CommandIdentifier Identifier { get; }

	//    //// Required for transforms.
	//    //abstract public ParameterSignature Signature { get; }
	//    //// Required for transforms.
	//    //abstract public object GetParameterValue(int index);
	//    //// Required for transforms.
	//    //abstract public void SetParameterValue(int index, object value);
	//    abstract public Command Clone();

	//    virtual public bool CanCombine(Command other) {
	//        return Identifier.Equals(other.Identifier);
	//    }

	//    virtual public Command Combine(Command other) {
	//        return this;
	//    }

	//    public abstract void Dispatch(CommandDispatch commandDispatch);

	//    static public Command Combine(IEnumerable<Command> commands) {
	//        Command[] commandArray = commands.ToArray();

	//        if(commandArray.Length == 0) return null;
	//        Command firstCommand = commandArray[0];
	//        if(commandArray.Length == 1) return firstCommand;

	//        return commandArray.Aggregate((command1, command2) => {
	//            if (command1 == null)
	//                return command2;

	//            if (command2 == null)
	//                return command1;

	//            return command1.Combine(command2);
	//        });
	//    }
	//}
}
