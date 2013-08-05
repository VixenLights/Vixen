using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	internal class CacheHitPercentValue : PercentValue
	{
		public CacheHitPercentValue(string contextName)
			: base("Cache hit (%) [" + contextName + "]")
		{
		}
	}
}