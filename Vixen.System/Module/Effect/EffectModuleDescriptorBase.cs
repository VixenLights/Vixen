using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Effect {
	abstract public class EffectModuleDescriptorBase : ModuleDescriptorBase, IEffectModuleDescriptor, IEqualityComparer<IEffectModuleDescriptor> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string EffectName { get; }

		abstract public CommandStandard.CommandParameterSpecification[] Parameters { get; }

		public bool Equals(IEffectModuleDescriptor x, IEffectModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectModuleDescriptor obj) {
			return base.GetHashCode();
		}
	}
}
