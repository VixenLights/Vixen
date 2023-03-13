using Vixen.Sys.Attribute;

namespace Vixen.Module.MediaRenderer
{
	[TypeOfModule("MediaRenderer")]
	internal class MediaRendererModuleImplementation : ModuleImplementation<IMediaRendererModuleInstance>
	{
		public MediaRendererModuleImplementation()
			: base(new MediaRendererModuleManagement(), new MediaRendererModuleRepository())
		{
		}
	}
}