using Vixen.Commands;

namespace Vixen.Sys {
	class CommandStateAggregator : StateAggregator<Command> {
		//public CommandStateAggregator() {
		//}

		//public CommandStateAggregator(IEnumerable<IStateSource<Command>> stateSources)
		//    : base(stateSources) {
		//}

		//Not being called because there is only a single state; nothing to combine.
		protected override IStateSource<Command> _Combinator(IStateSource<Command> left, IStateSource<Command> right) {
			if(left == null) return right;
			if(right == null) return left;

			Command value1 = left.Value;
			Command value2 = right.Value;

			if(value1 == null)
				return right;

			if(value2 == null)
				return left;

			return new CommandStateSource(value1.Combine(value2));
		}

		//private class AggregatedState : IStateSource<Command> {
		//    public AggregatedState(Command state) {
		//        Value = state;
		//    }

		//    public Command Value { get; private set; }
		//}
	}
}
