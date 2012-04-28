using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class BufferSizeInSeconds : DoubleValue {
		public BufferSizeInSeconds(string contextName)
			: base("Buffer Size (seconds) - " + contextName) {
		}
	}
}
