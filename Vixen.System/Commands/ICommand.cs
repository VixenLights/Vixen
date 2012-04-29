using Vixen.Sys;

namespace Vixen.Commands {
	public interface ICommand : IDispatchable {
		object CommandValue { get; set; }
	}

	public interface ICommand<T> : ICommand {
		T CommandValue { get; set; }
	}
}
