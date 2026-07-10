using Vixen.Sys.Attribute;

namespace Vixen.Module.App
{
	[TypeOfModule("App")]
	internal class AppModuleImplementation : ModuleImplementation<IAppModuleInstance>
	{
		public AppModuleImplementation()
			: base(new AppModuleManagement(), new AppModuleRepository())
		{
		}
	}
}