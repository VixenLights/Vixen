using System;
using CSCore;

namespace VixenModules.Media.Audio.SampleProviders
{
	public class MonoSampleProvider:ISampleSource
	{
		private readonly ISampleSource _sourceProvider;
		private float[] _sourceBuffer;
		
		public MonoSampleProvider(ISampleSource sourceProvider)
		{
			_sourceProvider = sourceProvider.ToMono();
			WaveFormat = _sourceProvider.WaveFormat; //.CreateIeeeFloatWaveFormat(sourceProvider.WaveFormat.SampleRate, 1);
		}

		/// <summary>
		/// Provides a multichannel to mono sample set by averaging all channels into a single channel.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public int Read(float[] buffer, int offset, int count)
		{
			return _sourceProvider.Read(buffer, offset, count);
		}

		/// <inheritdoc />
		public bool CanSeek { get; }

		/// <summary>
		/// Output Wave Format
		/// </summary>
		public WaveFormat WaveFormat { get; }

		/// <inheritdoc />
		public long Position { get; set; }

		/// <inheritdoc />
		public long Length { get; }

		/// <summary>
		/// Reads bytes from this SampleProvider
		/// </summary>
		public int Read(double[] buffer, int offset, int count)
		{
			if(offset + count > buffer.Length) throw new ArgumentOutOfRangeException(nameof(offset));
			var temp = new float[count]; 
			_sourceProvider.Read(temp, 0, count);
			for (int i = 0; i < temp.Length; i++)
			{
				buffer[offset++] = temp[i];
			}

			return count;
		}

		#region Implementation of IDisposable

		/// <inheritdoc />
		public void Dispose()
		{
			
		}

		#endregion
	}
}
