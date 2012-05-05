using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class BufferSizeInSecondsValue : DoubleValue {
		public BufferSizeInSecondsValue(string contextName)
			: base("Buffer Size (seconds) [" + contextName + "]") {
		}
	}
}
