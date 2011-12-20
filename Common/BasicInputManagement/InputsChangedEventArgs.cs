using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace BasicInputManagement {
	public class InputsChangedEventArgs : EventArgs {
		public InputsChangedEventArgs(IEnumerable<EffectNode> effectNodes) {
			EffectNodes = effectNodes.ToArray();
		}

		public EffectNode[] EffectNodes { get; private set; }
	}
}
