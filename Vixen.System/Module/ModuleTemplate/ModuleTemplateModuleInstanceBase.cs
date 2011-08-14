using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.ModuleTemplate {
	abstract public class ModuleTemplateModuleInstanceBase : ModuleInstanceBase, IModuleTemplateModuleInstance, IEqualityComparer<IModuleTemplateModuleInstance>, IEquatable<IModuleTemplateModuleInstance>, IEqualityComparer<ModuleTemplateModuleInstanceBase>, IEquatable<ModuleTemplateModuleInstanceBase> {
		abstract public void Project(IModuleInstance target);

		abstract public void Setup();

		public bool Equals(IModuleTemplateModuleInstance x, IModuleTemplateModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IModuleTemplateModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IModuleTemplateModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(ModuleTemplateModuleInstanceBase x, ModuleTemplateModuleInstanceBase y) {
			return Equals(x as IModuleTemplateModuleInstance, y as IModuleTemplateModuleInstance);
		}

		public int GetHashCode(ModuleTemplateModuleInstanceBase obj) {
			return GetHashCode(obj as IModuleTemplateModuleInstance);
		}

		public bool Equals(ModuleTemplateModuleInstanceBase other) {
			return Equals(other as IModuleTemplateModuleInstance);
		}
	}
}
