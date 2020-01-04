using System;

namespace VixenModules.Media.Audio.SampleProviders
{
    public class RmsPeakProvider : PeakProvider
    {
        private readonly int blockSize;

        public RmsPeakProvider(int blockSize)
        {
            this.blockSize = blockSize;
        }

        public override Sample GetNextPeak()
        {
            var samplesRead = Provider.Read(ReadBuffer, 0, ReadBuffer.Length);

            var max = 0.0f;
            for (int x = 0; x < samplesRead; x += blockSize)
            {
                double total = 0.0;
                for (int y = 0; y < blockSize && x + y < samplesRead; y++)
                {
                    total += ReadBuffer[x + y] * ReadBuffer[x + y];
                }
                var rms = (float) Math.Sqrt(total/blockSize);

                max = Math.Max(max, rms);
            }

            return new Sample(0 -max, max);
        }
    }
}