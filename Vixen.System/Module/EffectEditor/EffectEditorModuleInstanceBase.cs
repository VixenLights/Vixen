using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Module.EffectEditor {
	abstract public class EffectEditorModuleInstanceBase : ModuleInstanceBase, IEffectEditorModuleInstance, IEqualityComparer<IEffectEditorModuleInstance>, IEquatable<IEffectEditorModuleInstance>, IEqualityComparer<EffectEditorModuleInstanceBase>, IEquatable<EffectEditorModuleInstanceBase> {
		abstract public IEffectEditorControl CreateEditorControl();

		public Guid EffectTypeId {
			get { return ((IEffectEditorModuleDescriptor)Descriptor).EffectTypeId; }
		}

		public Type[] ParameterSignature {
			get { return ((IEffectEditorModuleDescriptor)Descriptor).ParameterSignature; }
		}

		public bool Equals(IEffectEditorModuleInstance x, IEffectEditorModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectEditorModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IEffectEditorModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(EffectEditorModuleInstanceBase x, EffectEditorModuleInstanceBase y) {
			return Equals(x as IEffectEditorModuleInstance, y as IEffectEditorModuleInstance);
		}

		public int GetHashCode(EffectEditorModuleInstanceBase obj) {
			return GetHashCode(obj as IEffectEditorModuleInstance);
		}

		public bool Equals(EffectEditorModuleInstanceBase other) {
			return Equals(other as IEffectEditorModuleInstance);
		}
	}
}
