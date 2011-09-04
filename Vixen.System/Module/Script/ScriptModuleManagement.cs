using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Script;

namespace Vixen.Module.Script {
	class ScriptModuleManagement : GenericModuleManagement<IScriptModuleInstance> {
		// Go through the repository so that the repository is the authority on what's available.
		public IScriptSkeletonGenerator GetSkeletonGenerator(string language) {
			ScriptModuleRepository repository = _GetRepository();
			return repository.GetSkeletonGenerator(language);
		}

		public IScriptFrameworkGenerator GetFrameworkGenerator(string language) {
			ScriptModuleRepository repository = _GetRepository();
			return repository.GetFrameworkGenerator(language);
		}

		public IScriptCodeProvider GetCodeProvider(string language) {
			ScriptModuleRepository repository = _GetRepository();
			return repository.GetCodeProvider(language);
		}

		public string GetFileExtension(string language) {
			ScriptModuleRepository repository = _GetRepository();
			return repository.GetFileExtension(language);
		}

		public string[] GetLanguages() {
			ScriptModuleRepository repository = _GetRepository();
			return repository.GetLanguages();
		}

		private ScriptModuleRepository _GetRepository() {
			return Modules.GetRepository<IScriptModuleInstance, ScriptModuleRepository>();
		}
	}
}
