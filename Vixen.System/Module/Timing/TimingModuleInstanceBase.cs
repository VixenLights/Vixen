using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Timing {
	abstract public class TimingModuleInstanceBase : ModuleInstanceBase, ITimingModuleInstance, IEqualityComparer<ITimingModuleInstance> {
		abstract public long Position { get; set; }

		abstract public void Start();

		abstract public void Stop();

		abstract public void Pause();

		abstract public void Resume();

		public bool Equals(ITimingModuleInstance x, ITimingModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ITimingModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}
