using Common.AudioPlayer.SampleProvider;
using NAudio.Wave;

namespace VixenModules.Media.Audio.SampleProviders
{
    public abstract class PeakProvider : IPeakProvider
    {
        protected CachedSoundSampleProvider Provider { get; private set; }
        protected int SamplesPerPeak { get; private set; }
        protected float[] ReadBuffer { get; private set; }

        public void Init(CachedSoundSampleProvider provider, int samplesPerPeak)
        {
            Provider = provider;
            SamplesPerPeak = samplesPerPeak;
            ReadBuffer = new float[samplesPerPeak];
        }

        public abstract Sample GetNextPeak();

    }
}