using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Sys;

namespace Vixen.Module.PreFilter {
	abstract public class PreFilterModuleInstanceBase : ModuleInstanceBase, IPreFilterModuleInstance, IEqualityComparer<IPreFilterModuleInstance>, IEquatable<IPreFilterModuleInstance>, IEqualityComparer<PreFilterModuleInstanceBase>, IEquatable<PreFilterModuleInstanceBase> {
		//abstract public Command Affect(Command command, TimeSpan filterRelativeTime);

		virtual public TimeSpan TimeSpan { get; set; }

		virtual public ChannelNode[] TargetNodes { get; set; }

		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() { return false; }

		virtual public float Affect(float value, float percentIntoFilter) {
			return value;
		}

		virtual public Color Affect(Color value, float percentIntoFilter) {
			return value;
		}

		virtual public DateTime Affect(DateTime value, float percentIntoFilter) {
			return value;
		}

		virtual public long Affect(long value, float percentIntoFilter) {
			return value;
		}

		virtual public double Affect(double value, float percentIntoFilter) {
			return value;
		}

		virtual public IFilterState CreateFilterState(TimeSpan filterRelativeTime) {
			return new PreFilterState(this, filterRelativeTime);
		}

		public bool Equals(IPreFilterModuleInstance x, IPreFilterModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPreFilterModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IPreFilterModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(PreFilterModuleInstanceBase x, PreFilterModuleInstanceBase y) {
			return Equals(x as IPreFilterModuleInstance, y as IPreFilterModuleInstance);
		}

		public int GetHashCode(PreFilterModuleInstanceBase obj) {
			return GetHashCode(obj as IPreFilterModuleInstance);
		}

		public bool Equals(PreFilterModuleInstanceBase other) {
			return Equals(other as IPreFilterModuleInstance);
		}
	}
}
