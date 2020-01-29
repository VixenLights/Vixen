using System.Collections.Generic;
using CSCore;

namespace VixenModules.Media.Audio.SampleProviders
{
    public abstract class PeakProvider : IPeakProvider
    {
        protected ISampleSource Provider { get; private set; }
        protected int SamplesPerPeak { get; private set; }
        protected float[] ReadBuffer { get; private set; }

        public void Init(ISampleSource provider, int samplesPerPeak)
        {
            Provider = provider;
            SamplesPerPeak = samplesPerPeak;
            ReadBuffer = new float[samplesPerPeak];
        }

        public abstract Sample GetNextPeak();

    }
}