using NAudio.Wave;

namespace Common.AudioPlayer.SampleProvider
{
	public class CachedSoundSampleProvider : ISampleProvider
	{
		private readonly CachedSampleSource _source;
		private long _position;
		
		public CachedSoundSampleProvider(CachedSampleSource source)
		{
			_source = source;
		}

		private int TimeSpanToSamples(TimeSpan time)
		{
			var samples = (int)(time.TotalSeconds * WaveFormat.SampleRate) * WaveFormat.Channels;
			return samples;
		}

		private TimeSpan SamplesToTimeSpan(long samples)
		{
			// ReSharper disable once PossibleLossOfFraction
			return TimeSpan.FromSeconds(samples / WaveFormat.Channels / (double)WaveFormat.SampleRate);
		}

		public TimeSpan Duration => SamplesToTimeSpan(Length);

		public long SamplePosition
		{
			get => _position;
			set => _position = value;
		}

		public TimeSpan Position
		{
			get => SamplesToTimeSpan(_position);
			set => _position = TimeSpanToSamples(value);
		}

		public long Length => _source.AudioData.Length;

		public int Read(float[] buffer, int offset, int count)
		{
			var availableSamples = Length - _position;
			var samplesToCopy = Math.Min(availableSamples, count);
			Array.Copy(_source.AudioData, _position, buffer, offset, samplesToCopy);
			_position += samplesToCopy;
			return (int)samplesToCopy;
		}

		public WaveFormat WaveFormat => _source.WaveFormat;
	}
}
