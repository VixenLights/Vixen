using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	internal class BufferSizeInSecondsValue : DoubleValue {
		public BufferSizeInSecondsValue(string contextName)
			: base(string.Format("Buffer Size (seconds) [{0}]", contextName)) {
		}
	}
}