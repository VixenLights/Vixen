using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace FadeOut {
	class CommandHandler : CommandDispatch {
		private Reducer _reducer;

		public CommandHandler() {
			_reducer = new Reducer();
		}

		public double ReductionPercentage;
		public ICommand Command { get; private set; }

		public override void Handle(_8BitCommand obj) {
			Command = new _8BitCommand(_reducer.Reduce(obj.CommandValue, ReductionPercentage));
		}

		public override void Handle(_16BitCommand obj) {
			Command = new _16BitCommand(_reducer.Reduce(obj.CommandValue, ReductionPercentage));
		}

		public override void Handle(_32BitCommand obj) {
			Command = new _32BitCommand(_reducer.Reduce(obj.CommandValue, ReductionPercentage));
		}

		public override void Handle(_64BitCommand obj) {
			Command = new _64BitCommand(_reducer.Reduce(obj.CommandValue, ReductionPercentage));
		}

		public override void Handle(ColorCommand obj) {
			Command = new ColorCommand(_reducer.Reduce(obj.CommandValue, ReductionPercentage));
		}
	}
}
