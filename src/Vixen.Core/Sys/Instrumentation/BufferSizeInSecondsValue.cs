using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	internal class BufferSizeInSecondsValue : DoubleValue
	{
		public BufferSizeInSecondsValue(string contextName)
			: base("Buffer Size (seconds) [" + contextName + "]")
		{
		}
	}
}