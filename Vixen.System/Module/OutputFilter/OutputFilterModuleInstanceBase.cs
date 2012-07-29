using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace Vixen.Module.OutputFilter {
	abstract public class OutputFilterModuleInstanceBase : ModuleInstanceBase, IAnyCommandHandler, IOutputFilterModuleInstance, IEqualityComparer<IOutputFilterModuleInstance>, IEquatable<IOutputFilterModuleInstance>, IEqualityComparer<OutputFilterModuleInstanceBase>, IEquatable<OutputFilterModuleInstanceBase> {
		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() { return false; }

		abstract public ICommand Affect(ICommand command);

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

		public void Handle(_8BitCommand obj) {
			Affect(obj);
		}

		public void Handle(_16BitCommand obj) {
			Affect(obj);
		}

		public void Handle(_32BitCommand obj) {
			Affect(obj);
		}

		public void Handle(_64BitCommand obj) {
			Affect(obj);
		}

		public void Handle(ColorCommand obj) {
			Affect(obj);
		}
	}
}
