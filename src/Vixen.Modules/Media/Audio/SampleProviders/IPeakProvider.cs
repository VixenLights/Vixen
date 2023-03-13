using Common.AudioPlayer.SampleProvider;
using NAudio.Wave;

namespace VixenModules.Media.Audio.SampleProviders
{
    public interface IPeakProvider
    {
        void Init(CachedSoundSampleProvider reader, int samplesPerPixel);
        Sample GetNextPeak();
    }
}