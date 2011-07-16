using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Editor {
	[TypeOfModule("Editor")]
	class EditorModuleImplementation : ModuleImplementation<IEditorModuleInstance> {
		public EditorModuleImplementation()
			: base(new EditorModuleManagement(), new EditorModuleRepository()) {
		}
	}
}
