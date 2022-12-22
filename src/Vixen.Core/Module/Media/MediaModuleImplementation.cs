using Vixen.Sys.Attribute;

namespace Vixen.Module.Media
{
	[TypeOfModule("Media")]
	internal class MediaModuleImplementation : ModuleImplementation<IMediaModuleInstance>
	{
		public MediaModuleImplementation()
			: base(new MediaModuleManagement(), new MediaModuleRepository())
		{
		}
	}
}