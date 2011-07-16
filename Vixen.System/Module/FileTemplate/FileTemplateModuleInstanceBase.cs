using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.FileTemplate {
	abstract public class FileTemplateModuleInstanceBase : ModuleInstanceBase, IFileTemplateModuleInstance, IEqualityComparer<IFileTemplateModuleInstance> {
		abstract public void Project(object target);

		abstract public void Setup();

		public bool Equals(IFileTemplateModuleInstance x, IFileTemplateModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IFileTemplateModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}
