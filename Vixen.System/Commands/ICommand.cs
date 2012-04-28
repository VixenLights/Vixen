using Vixen.Sys;

namespace Vixen.Commands {
	public interface ICommand : IDispatchable {
		object Value { get; set; }
	}

	public interface ICommand<T> : ICommand {
		T Value { get; set; }
	}
}
