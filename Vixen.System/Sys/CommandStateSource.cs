using Vixen.Commands;

namespace Vixen.Sys {
	class CommandStateSource : IStateSource<Command> {
		public CommandStateSource(Command state = null) {
			Value = state;
		}

		public Command Value { get; set; }
	}
}
