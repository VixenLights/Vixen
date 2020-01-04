using System.Collections.Generic;
using System.Linq;

namespace VixenModules.Media.Audio.SampleProviders
{
	public class MaxPeakProvider : PeakProvider
	{
		public override Sample GetNextPeak()
		{
			var samplesRead = Provider.Read(ReadBuffer, 0, ReadBuffer.Length);
			var max = (samplesRead == 0) ? 0 : ReadBuffer.Take(samplesRead).Max();
			var min = (samplesRead == 0) ? 0 : ReadBuffer.Take(samplesRead).Min();
			return new Sample(min, max);
		}
	}
}