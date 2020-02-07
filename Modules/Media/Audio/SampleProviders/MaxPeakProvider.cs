using System.Linq;

namespace VixenModules.Media.Audio.SampleProviders
{
	public class MaxPeakProvider : PeakProvider
	{
		public override Sample GetNextPeak()
		{
			var samplesRead = Provider.Read(ReadBuffer, 0, ReadBuffer.Length);

			if (samplesRead > 0)
			{
				var samples = ReadBuffer.Take(samplesRead);
				var max = samples.Max();
				var min = samples.Min();
				return new Sample(min, max);
			}
			
			return new Sample(0,0);
		}
	}
}