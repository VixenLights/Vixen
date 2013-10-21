<<<<<<< HEAD
﻿using System;

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
=======
﻿using System;

namespace Vixen.Data.Value
{
	public struct PositionValue : IIntentDataType
	{
		public PositionValue(float percentage)
		{
			if (percentage < 0 || percentage > 1) throw new ArgumentOutOfRangeException("percentage");

			Position = percentage;
		}

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public float Position;
	}
>>>>>>> parent of 42f78e6... Git insists these need committing even tho nothing has changed
}