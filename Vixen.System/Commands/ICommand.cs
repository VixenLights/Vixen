using Vixen.Sys;

namespace Vixen.Commands {
	public interface ICommand : IDispatchable {
	}

	public interface ICommand<T> : ICommand {
		T Value { get; set; }
	}
}
