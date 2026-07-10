using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Services
{
	public class TimingService
	{
		private static TimingService _instance;

		private TimingService()
		{
		}

		public static TimingService Instance
		{
			get { return _instance ?? (_instance = new TimingService()); }
		}

		public ITiming GetDefaultSequenceTiming()
		{
			return Modules.GetManager<ITimingModuleInstance, TimingModuleManagement>().GetDefault();
		}
	}
}