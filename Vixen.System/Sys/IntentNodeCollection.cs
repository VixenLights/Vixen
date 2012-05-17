using System.Collections.Generic;

namespace Vixen.Sys {
	public class IntentNodeCollection : StrategyList<IIntentNode> {
		internal IntentNodeCollection(IIntentAddStrategy intentAddStrategy)
			: base(addStrategy: intentAddStrategy.AddStrategy) {
		}

		internal IntentNodeCollection(IEnumerable<IIntentNode> intentNodes, IIntentAddStrategy intentAddStrategy)
			: this(intentAddStrategy) {
			AddRange(intentNodes);
		}

		static internal IntentNodeCollection Create() {
			IIntentAddStrategy addStrategy = new IntentReplaceWithOtherStrategy();
			return new IntentNodeCollection(addStrategy);
		}

		static internal IntentNodeCollection Create(IEnumerable<IIntentNode> intentNodes) {
			IIntentAddStrategy addStrategy = new IntentReplaceWithOtherStrategy();
			return new IntentNodeCollection(intentNodes, addStrategy);
		}

		public void AddUnsafe(IIntentNode intentNode) {
			StrategyBypassAdd(intentNode);
		}
	}
}
