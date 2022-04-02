using CSCore;

namespace VixenModules.Media.Audio.SampleProviders
{
    public interface IPeakProvider
    {
        void Init(ISampleSource reader, int samplesPerPixel);
        Sample GetNextPeak();
    }
}