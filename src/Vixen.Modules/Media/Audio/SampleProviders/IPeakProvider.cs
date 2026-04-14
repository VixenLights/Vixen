using Common.AudioPlayer.SampleProvider;

namespace VixenModules.Media.Audio.SampleProviders
{
    public interface IPeakProvider
    {
        void Init(CachedSoundSampleProvider reader, int samplesPerPixel);
        Sample GetNextPeak();
    }
}