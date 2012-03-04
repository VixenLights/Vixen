namespace Vixen.Sys {
	public class RenardDataPolicy : StandardDataPolicy {
		protected override IEvaluator GetEvaluator() {
			//modified to accommodate color, revert later
			return new NumericEvaluator();
		}

		protected override ICombinator GetCombinator() {
			//modified to accommodate color, revert later
			return new NumericHighestWinsCombinator();
		}

		protected override IGenerator GetGenerator() {
			return new ByteCommandGenerator();
		}
	}
}
