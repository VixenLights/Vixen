using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class CacheHitPercentValue : PercentValue {
		public CacheHitPercentValue(string contextName)
			: base("Cache hit (%) - " + contextName) {
		}
	}
}
