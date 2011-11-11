using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Timing {
	abstract public class TimingModuleInstanceBase : ModuleInstanceBase, ITimingModuleInstance, IEqualityComparer<ITimingModuleInstance>, IEquatable<ITimingModuleInstance>, IEqualityComparer<TimingModuleInstanceBase>, IEquatable<TimingModuleInstanceBase> {
		abstract public TimeSpan Position { get; set; }

		abstract public void Start();

		abstract public void Stop();

		abstract public void Pause();

		abstract public void Resume();

		virtual public bool SupportsVariableSpeeds {
			get { return false; }
		}

		virtual public float Speed {
			get { return 1; } // 1 = 100%
			set { throw new NotSupportedException(); }
		}

		public bool Equals(ITimingModuleInstance x, ITimingModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ITimingModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(ITimingModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(TimingModuleInstanceBase x, TimingModuleInstanceBase y) {
			return Equals(x as ITimingModuleInstance, y as ITimingModuleInstance);
		}

		public int GetHashCode(TimingModuleInstanceBase obj) {
			return GetHashCode(obj as ITimingModuleInstance);
		}

		public bool Equals(TimingModuleInstanceBase other) {
			return Equals(other as ITimingModuleInstance);
		}
	}
}
