using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Module.RuntimeBehavior {
	abstract public class RuntimeBehaviorModuleInstanceBase : ModuleInstanceBase, IRuntimeBehaviorModuleInstance, IEqualityComparer<IRuntimeBehaviorModuleInstance> {
		abstract public void Startup(ISequence sequence, ITiming timingSource);

		abstract public void Shutdown();

		abstract public void Handle(CommandNode commandNode);

		virtual public bool Enabled { get; set; }

		abstract public Tuple<string, Action>[] BehaviorActions { get; }

		public bool Equals(IRuntimeBehaviorModuleInstance x, IRuntimeBehaviorModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IRuntimeBehaviorModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}
