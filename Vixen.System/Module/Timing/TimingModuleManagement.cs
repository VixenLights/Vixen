using System;

namespace Vixen.Module.Timing
{
	internal class TimingModuleManagement : GenericModuleManagement<ITimingModuleInstance>
	{
		public ITimingModuleInstance GetDefault()
		{
			return Get(Guid.Empty);
		}
	}
}