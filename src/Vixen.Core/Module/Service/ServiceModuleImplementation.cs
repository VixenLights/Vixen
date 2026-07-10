using Vixen.Sys.Attribute;

namespace Vixen.Module.Service
{
	[TypeOfModule("Service")]
	internal class ServiceModuleImplementation : ModuleImplementation<IServiceModuleInstance>
	{
		public ServiceModuleImplementation()
			: base(new ServiceModuleManagement(), new ServiceModuleRepository())
		{
		}
	}
}