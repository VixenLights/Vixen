using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Effect;
using Vixen.Sys.CombinationOperation;

namespace Vixen.Sys {
	public class SubordinateEffect {
		public SubordinateEffect(IEffectModuleInstance effect, ICombinationOperation combinationOperation) {
			Effect = effect;
			CombinationOperation = combinationOperation;
		}

		public IEffectModuleInstance Effect { get; private set; }
		
		public ICombinationOperation CombinationOperation { get; private set; }
	}
}
