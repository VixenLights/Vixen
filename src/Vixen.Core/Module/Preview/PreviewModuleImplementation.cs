using Vixen.Sys.Attribute;

namespace Vixen.Module.Preview
{
	[TypeOfModule("Preview")]
	internal class PreviewModuleImplementation : ModuleImplementation<IPreviewModuleInstance>
	{
		public PreviewModuleImplementation()
			: base(new PreviewModuleManagement(), new PreviewModuleRepository())
		{
		}
	}
}