using Vixen.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Instrumentation
{
	internal class OutputDeviceUpdateTimeValue : DoubleValue
	{
		public OutputDeviceUpdateTimeValue(IOutputDevice outputDevice)
			: base(string.Format("Output device update time [{0}]" ,outputDevice.Name ))
		{
		}

		protected override string _GetFormattedValue()
		{
	 		return string.Format("{0} ms", (int)_GetValue());
		}
	}
}