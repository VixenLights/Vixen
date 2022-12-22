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