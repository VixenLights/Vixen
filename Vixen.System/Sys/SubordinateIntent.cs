using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.CombinationOperation;

namespace Vixen.Sys {
	public class SubordinateIntent {
		public SubordinateIntent(IntentNode intentNode, ICombinationOperation combinationOperation) {
			IntentNode = intentNode;
			CombinationOperation = combinationOperation;
		}

		public IntentNode IntentNode { get; private set; }

		public ICombinationOperation CombinationOperation { get; private set; }
	}
}
