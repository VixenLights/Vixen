using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.EffectEditor {
	abstract public class EffectEditorModuleDescriptorBase : ModuleDescriptorBase, IEffectEditorModuleDescriptor, IEqualityComparer<IEffectEditorModuleDescriptor> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public Guid EffectTypeId { get; }

		abstract public CommandStandard.CommandParameterSpecification[] CommandSignature { get; }

		public bool Equals(IEffectEditorModuleDescriptor x, IEffectEditorModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectEditorModuleDescriptor obj) {
			return base.GetHashCode();
		}
	}
}
