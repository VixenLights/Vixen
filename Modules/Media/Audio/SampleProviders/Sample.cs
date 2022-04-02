namespace VixenModules.Media.Audio.SampleProviders
{
    public struct Sample
    {
        public Sample(float low, float high)
        {
	        Low = low;
            High = high;
        }

        public float High { get; private set; }
        public float Low { get; private set; }
    }
}