using System;

namespace Vixen.Data.Value
{
	public struct PositionValue : IIntentDataType
	{
		public PositionValue(double percentage)
		{
			if (percentage < 0 || percentage > 1) throw new ArgumentOutOfRangeException("percentage");

			Position = percentage;
		}

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public double Position;
	}
}