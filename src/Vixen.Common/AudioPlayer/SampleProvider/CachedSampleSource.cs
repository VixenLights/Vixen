#nullable enable
using Common.AudioPlayer.FileReader;
using NAudio.Wave;

namespace Common.AudioPlayer.SampleProvider
{
	public class CachedSampleSource
	{
		public float[] AudioData { get; }
		public WaveFormat WaveFormat { get; }
		public CachedSampleSource(string audioFileName)
		{
			using (var audioFileReader = new AudioFileReader(audioFileName))
			{
				WaveFormat = audioFileReader.WaveFormat;
				var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
				var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
				int samplesRead;
				while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
				{
					wholeFile.AddRange(readBuffer.Take(samplesRead));
				}
				AudioData = wholeFile.ToArray();
			}
		}
	}
}
