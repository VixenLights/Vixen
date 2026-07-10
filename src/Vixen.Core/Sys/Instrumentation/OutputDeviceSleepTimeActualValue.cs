using Vixen.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys.Instrumentation
{
	internal class OutputDeviceSleepTimeActualValue : DoubleValue
	{
		public OutputDeviceSleepTimeActualValue(IOutputDevice outputDevice)
			: base(string.Format("Output device sleep time (actual) [{0}]", outputDevice.Name))
		{
		}

		protected override string _GetFormattedValue()
		{
	
			return string.Format("{0} ms", (int)_GetValue());
		}
	}
}