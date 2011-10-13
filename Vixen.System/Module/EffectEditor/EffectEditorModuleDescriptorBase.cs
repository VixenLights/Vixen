using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Module.EffectEditor {
	abstract public class EffectEditorModuleDescriptorBase : ModuleDescriptorBase, IEffectEditorModuleDescriptor, IEqualityComparer<IEffectEditorModuleDescriptor>, IEquatable<IEffectEditorModuleDescriptor>, IEqualityComparer<EffectEditorModuleDescriptorBase>, IEquatable<EffectEditorModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public Guid EffectTypeId { get; }

		abstract public CommandParameterSignature ParameterSignature { get; }

		public bool Equals(IEffectEditorModuleDescriptor x, IEffectEditorModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectEditorModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IEffectEditorModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(EffectEditorModuleDescriptorBase x, EffectEditorModuleDescriptorBase y) {
			return Equals(x as IEffectEditorModuleDescriptor, y as IEffectEditorModuleDescriptor);
		}

		public int GetHashCode(EffectEditorModuleDescriptorBase obj) {
			return GetHashCode(obj as IEffectEditorModuleDescriptor);
		}

		public bool Equals(EffectEditorModuleDescriptorBase other) {
			return Equals(other as IEffectEditorModuleDescriptor);
		}
	}
}
