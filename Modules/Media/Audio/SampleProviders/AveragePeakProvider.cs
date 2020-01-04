using System;
using System.Linq;

namespace VixenModules.Media.Audio.SampleProviders
{
    public class AveragePeakProvider : PeakProvider
    {
        private readonly float scale;

        public AveragePeakProvider(float scale)
        {
            this.scale = scale;
        }

        public override Sample GetNextPeak()
        {
            var samplesRead = Provider.Read(ReadBuffer, 0, ReadBuffer.Length);
            var sum = (samplesRead == 0) ? 0 : ReadBuffer.Take(samplesRead).Select(s => Math.Abs(s)).Sum();
            var average = sum/samplesRead;
            
            return new Sample(average * (0 - scale), average * scale);
        }
    }
}