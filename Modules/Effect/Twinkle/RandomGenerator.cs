using System;

namespace VixenModules.Effect.Twinkle
{
	/// <summary>
	/// A random number generator.
	/// </summary>
	public class RandomGenerator
	{
		private static uint m_w;
		private static uint m_z;

		static RandomGenerator()
		{
			// These values are not magical, just the default values Marsaglia used.
			// Any pair of unsigned integers should be fine.
			m_w = 521288629;
			m_z = 362436069;
		}

		// The random generator seed can be set three ways:
		// 1) specifying two non-zero unsigned integers
		// 2) specifying one non-zero unsigned integer and taking a default value for the second
		// 3) setting the seed from the system time

		public static void SetSeed(uint u, uint v)
		{
			if (u != 0) m_w = u;
			if (v != 0) m_z = v;
		}

		public static void SetSeed(uint u)
		{
			m_w = u;
		}

		public static void SetSeedFromSystemTime()
		{
			DateTime dt = DateTime.Now;
			long x = dt.ToFileTime();
			SetSeed((uint)(x >> 16), (uint)(x % 4294967296));
		}

		// Produce a uniform random sample from the open interval (0, 1).
		// The method will not return either end point.
		public static double GetUniform()
		{
			// 0 <= u < 2^32
			uint u = GetUint();
			// The magic number below is 1/(2^32 + 2).
			// The result is strictly between 0 and 1.
			return (u + 1.0) * 2.328306435454494e-10;
		}

		// This is the heart of the generator.
		// It uses George Marsaglia's MWC algorithm to produce an unsigned integer.
		// See http://www.bobwheeler.com/statistics/Password/MarsagliaPost.txt
		private static uint GetUint()
		{
			m_z = 36969 * (m_z & 65535) + (m_z >> 16);
			m_w = 18000 * (m_w & 65535) + (m_w >> 16);
			return (m_z << 16) + m_w;
		}

		// Get normal (Gaussian) random sample with mean 0 and standard deviation 1
		public static double GetNormal()
		{
			// Use Box-Muller algorithm
			double u1 = GetUniform();
			double u2 = GetUniform();
			double r = Math.Sqrt(-2.0 * Math.Log(u1));
			double theta = 2.0 * Math.PI * u2;
			return r * Math.Sin(theta);
		}

		// Get normal (Gaussian) random sample with specified mean and standard deviation
		public static double GetNormal(double mean, double standardDeviation)
		{
			if (standardDeviation <= 0.0)
			{
				string msg = string.Format("Deviation must be positive. Received {0}.", standardDeviation);
				throw new ArgumentOutOfRangeException(msg);
			}
			return mean + standardDeviation * GetNormal();
		}

		// Get exponential random sample with mean 1
		public static double GetExponential()
		{
			return -Math.Log(GetUniform());
		}

		// Get exponential random sample with specified mean
		public static double GetExponential(double mean)
		{
			if (mean <= 0.0)
			{
				string msg = string.Format("Mean must be positive. Received {0}.", mean);
				throw new ArgumentOutOfRangeException(msg);
			}
			return mean * GetExponential();
		}

	}
	

}
