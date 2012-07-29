using System;
using System.Collections.Generic;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Module.OutputFilter {
	abstract public class OutputFilterModuleInstanceBase : ModuleInstanceBase, IAnyIntentStateHandler, IOutputFilterModuleInstance, IEqualityComparer<IOutputFilterModuleInstance>, IEquatable<IOutputFilterModuleInstance>, IEqualityComparer<OutputFilterModuleInstanceBase>, IEquatable<OutputFilterModuleInstanceBase> {
		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() { return false; }

		abstract public IIntentState Affect(IIntentState intentValue);

		public bool Equals(IOutputFilterModuleInstance x, IOutputFilterModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IOutputFilterModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IOutputFilterModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(OutputFilterModuleInstanceBase x, OutputFilterModuleInstanceBase y) {
			return Equals(x as IOutputFilterModuleInstance, y as IOutputFilterModuleInstance);
		}

		public int GetHashCode(OutputFilterModuleInstanceBase obj) {
			return GetHashCode(obj as IOutputFilterModuleInstance);
		}

		public bool Equals(OutputFilterModuleInstanceBase other) {
			return Equals(other as IOutputFilterModuleInstance);
		}

		public void Handle(IIntentState<ColorValue> obj) {
			Affect(obj);
		}

		public void Handle(IIntentState<LightingValue> obj) {
			Affect(obj);
		}

		public void Handle(IIntentState<PositionValue> obj) {
			Affect(obj);
		}

		public void Handle(IIntentState<CommandValue> obj) {
			Affect(obj);
		}
	}
}
