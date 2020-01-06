using NAudio.Wave;

namespace VixenModules.Media.Audio.SampleProviders
{
	public class MonoSampleProvider:ISampleProvider
	{
		private readonly ISampleProvider _sourceProvider;
		private float[] _sourceBuffer;
		
		public MonoSampleProvider(ISampleProvider sourceProvider)
		{
			_sourceProvider = sourceProvider;
			WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sourceProvider.WaveFormat.SampleRate, 1);
		}

		/// <inheritdoc />
		public int Read(float[] buffer, int offset, int count)
		{
			var sourceSamplesRequired = count * WaveFormat.Channels;
			if (_sourceBuffer == null || _sourceBuffer.Length < sourceSamplesRequired) _sourceBuffer = new float[sourceSamplesRequired];

			var sourceSamplesRead = _sourceProvider.Read(_sourceBuffer, 0, sourceSamplesRequired);
			var destOffset = offset;
			for (var sourceSample = 0; sourceSample < sourceSamplesRead; sourceSample += WaveFormat.Channels)
			{
				float audioSum = 0;
				for (int i = 0; i < WaveFormat.Channels; i++)
				{
					audioSum += _sourceBuffer[sourceSample + i];
				}
				var outSample = audioSum / WaveFormat.Channels;
				buffer[destOffset++] = outSample;
			}
			return sourceSamplesRead / WaveFormat.Channels;
		}

		/// <summary>
		/// Output Wave Format
		/// </summary>
		public WaveFormat WaveFormat { get; }

		/// <summary>
		/// Reads bytes from this SampleProvider
		/// </summary>
		public int Read(double[] buffer, int offset, int count)
		{
			int numChannels = _sourceProvider.WaveFormat.Channels;
			var sourceSamplesRequired = count * numChannels;
			if (_sourceBuffer == null || _sourceBuffer.Length < sourceSamplesRequired) _sourceBuffer = new float[sourceSamplesRequired];

			var sourceSamplesRead = _sourceProvider.Read(_sourceBuffer, 0, sourceSamplesRequired);
			var destOffset = offset;
			for (var sourceSample = 0; sourceSample < sourceSamplesRead; sourceSample += numChannels)
			{
				double audioSum = 0;
				for (int i = 0; i < numChannels; i++)
				{
					audioSum += _sourceBuffer[sourceSample + i];
				}
				var outSample = audioSum / numChannels;
				buffer[destOffset++] = outSample;
			}
			return sourceSamplesRead / numChannels;
		}
	}
}
