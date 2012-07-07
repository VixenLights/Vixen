using System.Collections.Generic;

namespace Vixen.Module.Script {
	abstract public class ScriptModuleInstanceBase : ModuleInstanceBase, IScriptModuleInstance, IEqualityComparer<IScriptModuleInstance> {

		public IScriptSkeletonGenerator SkeletonGenerator { get; set; }

		public IScriptFrameworkGenerator FrameworkGenerator { get; set; }

		public IScriptCodeProvider CodeProvider { get; set; }

		public string FileExtension {
			get { return ((IScriptModuleDescriptor)Descriptor).FileExtension; }
		}

		public bool Equals(IScriptModuleInstance x, IScriptModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IScriptModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public override void Dispose() {
			base.Dispose();
			CodeProvider.Dispose();
			CodeProvider = null;
		}
	}
}
