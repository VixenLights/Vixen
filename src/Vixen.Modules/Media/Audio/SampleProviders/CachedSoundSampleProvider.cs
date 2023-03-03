//using Common.AudioPlayer.SampleProvider;
//using NAudio.Wave;

//namespace VixenModules.Media.Audio.SampleProviders
//{
//	public sealed class CachedSoundSampleProvider : ISampleProvider
//	{
//		private readonly CachedSoundSource _cachedAudioData;

//		public CachedSoundSampleProvider(CachedSoundSource cachedAudioData)
//		{
//			_cachedAudioData = cachedAudioData;
//			Position = 0;
//			Length = _cachedAudioData.AudioData.Length;
//		}

//		public int Read(float[] buffer, int offset, int count)
//		{
//			var availableSamples = Length - Position;
//			var samplesToCopy = Math.Min(availableSamples, count);
//			Array.Copy(_cachedAudioData.AudioData, Position, buffer, offset, samplesToCopy);
//			Position += samplesToCopy;
//			return (int)samplesToCopy;
//		}

//		public long Position { get; set; }

//		public long Length { get; }

//		/// <inheritdoc />
//		public bool CanSeek { get; }
//		public WaveFormat WaveFormat => _cachedAudioData.WaveFormat;

//		#region Implementation of IDisposable

//		/// <inheritdoc />
//		public void Dispose()
//		{

//		}

//		#endregion
//	}
//}