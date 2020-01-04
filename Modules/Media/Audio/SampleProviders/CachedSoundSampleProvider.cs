using System;
using NAudio.Wave;

namespace VixenModules.Media.Audio
{
	public sealed class CachedSoundSampleProvider : ISampleProvider
	{
		private readonly CachedAudioData _cachedAudioData;
		
		public CachedSoundSampleProvider(CachedAudioData cachedAudioData)
		{
			_cachedAudioData = cachedAudioData;
			Position = 0;
			Length = _cachedAudioData.AudioData.Length;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			var availableSamples = Length - Position;
			var samplesToCopy = Math.Min(availableSamples, count);
			Array.Copy(_cachedAudioData.AudioData, Position, buffer, offset, samplesToCopy);
			Position += samplesToCopy;
			return (int)samplesToCopy;
		}

		public long Position { get; set; }

		public int Length { get; }

		public WaveFormat WaveFormat => _cachedAudioData.WaveFormat;
	}
}