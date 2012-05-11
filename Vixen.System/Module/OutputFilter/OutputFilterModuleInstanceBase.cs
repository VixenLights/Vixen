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

		public void Handle(ByteValue obj) {
			Affect(obj);
		}

		public void Handle(SignedShortValue obj) {
			Affect(obj);
		}

		public void Handle(UnsignedShortValue obj) {
			Affect(obj);
		}

		public void Handle(SignedIntValue obj) {
			Affect(obj);
		}

		public void Handle(UnsignedIntValue obj) {
			Affect(obj);
		}

		public void Handle(SignedLongValue obj) {
			Affect(obj);
		}

		public void Handle(UnsignedLongValue obj) {
			Affect(obj);
		}

		public void Handle(ColorValue obj) {
			Affect(obj);
		}

		public void Handle(LightingValueCommand obj) {
			Affect(obj);
		}

		public void Handle(DoubleValue obj) {
			Affect(obj);
		}
	}
}
