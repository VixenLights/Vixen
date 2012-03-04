using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Commands {
	public interface ICommand : IDispatchable {
		//Really not the responsibility of the command.
		//void Dispatch(CommandDispatch commandDispatch);
	}

	//public interface ICommand<T> : ICommand {
	//    T Value { get; }
	//}
}
