using Vixen.Commands;

namespace Vixen.Sys {
	class CommandStateSource : IStateSource<Command> {
		public CommandStateSource(Command state = null) {
			State = state;
		}

		public Command State { get; set; }
	}
}
