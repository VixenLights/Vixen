// NOTE:
// This module type is not even necessary.  The core framework has no dependency on it and doesn't
// make use of it at all.
// These are only used by script sequences, which are now externally defined.
// So why keep it in here?
// Solely as a favor to script sequences; to provide the benefit of the module framework
// we have as a nice and easy way to define and get access to different languages.  And
// to provide a unified interface for such things.

using Vixen.Sys.Attribute;

namespace Vixen.Module.Script {
	[TypeOfModule("Script")]
	class ScriptModuleImplementation : ModuleImplementation<IScriptModuleInstance> {
		public ScriptModuleImplementation()
			: base(new ScriptModuleManagement(), new ScriptModuleRepository()) {
		}
	}
}
