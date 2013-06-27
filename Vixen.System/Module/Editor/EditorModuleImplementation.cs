using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.Attribute;

namespace Vixen.Module.Editor
{
	[TypeOfModule("Editor")]
	internal class EditorModuleImplementation : ModuleImplementation<IEditorModuleInstance>
	{
		public EditorModuleImplementation()
			: base(new EditorModuleManagement(), new EditorModuleRepository())
		{
		}
	}
}