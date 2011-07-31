using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.FileTemplate {
	abstract public class FileTemplateModuleInstanceBase : ModuleInstanceBase, IFileTemplateModuleInstance, IEqualityComparer<IFileTemplateModuleInstance>, IEquatable<IFileTemplateModuleInstance>, IEqualityComparer<FileTemplateModuleInstanceBase>, IEquatable<FileTemplateModuleInstanceBase> {
		abstract public void Project(object target);

		abstract public void Setup();

		public bool Equals(IFileTemplateModuleInstance x, IFileTemplateModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IFileTemplateModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IFileTemplateModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(FileTemplateModuleInstanceBase x, FileTemplateModuleInstanceBase y) {
			return Equals(x as IFileTemplateModuleInstance, y as IFileTemplateModuleInstance);
		}

		public int GetHashCode(FileTemplateModuleInstanceBase obj) {
			return GetHashCode(obj as IFileTemplateModuleInstance);
		}

		public bool Equals(FileTemplateModuleInstanceBase other) {
			return Equals(other as IFileTemplateModuleInstance);
		}
	}
}
