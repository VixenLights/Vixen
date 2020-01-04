using System;
using NAudio.Wave;

namespace VixenModules.Media.Audio.SampleProviders
{
    class DecibelPeakProvider : IPeakProvider
    {
        private readonly IPeakProvider sourceProvider;
        private readonly double dynamicRange;

        public DecibelPeakProvider(IPeakProvider sourceProvider, double dynamicRange)
        {
            this.sourceProvider = sourceProvider;
            this.dynamicRange = dynamicRange;
        }

        public void Init(ISampleProvider reader, int samplesPerPixel)
        {
            throw new NotImplementedException();
        }

        public Sample GetNextPeak()
        {
            var peak = sourceProvider.GetNextPeak();
            var decibelMax = 20 * Math.Log10(peak.Low);
            if (decibelMax < 0 - dynamicRange) decibelMax = 0 - dynamicRange;
            var linear = (float)((dynamicRange + decibelMax) / dynamicRange);
            return new Sample(0 - linear, linear);
        }
    }
}