using NAudio.Wave;

namespace VixenModules.Media.Audio.SampleProviders
{
    public interface IPeakProvider
    {
        void Init(ISampleProvider reader, int samplesPerPixel);
        Sample GetNextPeak();
    }
}