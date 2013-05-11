using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Common.BasicInputManagement {
	public class InputsChangedEventArgs : EventArgs {
		public InputsChangedEventArgs(IEnumerable<EffectNode> effectNodes) {
			EffectNodes = effectNodes.ToArray();
		}

		public EffectNode[] EffectNodes { get; private set; }
	}
}
