using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Common.AudioPlayer
{
	public static class Extensions
	{
		/// <summary>
		/// Checks the length of an array.
		/// </summary>
		/// <typeparam name="T">Type of the array.</typeparam>
		/// <param name="inst">The array to check. This parameter can be null.</param>
		/// <param name="size">The target length of the array.</param>
		/// <param name="exactSize">A value which indicates whether the length of the array has to fit exactly the specified <paramref name="size"/>.</param>
		/// <returns>Array which fits the specified requirements. Note that if a new array got created, the content of the old array won't get copied to the return value.</returns>
		public static T[] CheckBuffer<T>(this T[] inst, long size, bool exactSize = false)
		{
			if (inst == null || (!exactSize && inst.Length < size) || (exactSize && inst.Length != size))
				return new T[size];
			return inst;
		}

		/// <summary>
		///     Converts a SampleSource to either a Pcm (8, 16, or 24 bit) or IeeeFloat (32 bit) WaveSource.
		/// </summary>
		/// <param name="sampleSource">Sample source to convert to a wave source.</param>
		/// <param name="bits">Bits per sample.</param>
		/// <returns>Wave source</returns>
		public static IWaveProvider ToWaveSourceProvider(this ISampleProvider sampleSource, int bits)
		{
			if (sampleSource == null)
				throw new ArgumentNullException("sampleSource");

			switch (bits)
			{
				case 16:
					return new SampleToWaveProvider16(sampleSource);
				case 24:
					return new SampleToWaveProvider24(sampleSource);
				case 32:
					return new SampleToWaveProvider(sampleSource);
				default:
					throw new ArgumentOutOfRangeException("bits", "Must be 8, 16, 24 or 32 bits.");
			}
		}
	}
}
