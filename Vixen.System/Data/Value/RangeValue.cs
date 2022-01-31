using System;

namespace Vixen.Data.Value
{
	/// <summary>
	/// Maintains a tagged ranged value.
	/// </summary>
	public struct RangeValue : IIntentDataType
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tagType">Type of the tag</param>
		/// <param name="tag">Name of the tag</param>
		/// <param name="rangeValue">Initial value</param>
		public RangeValue(int tagType, string tag, double rangeValue)
		{
			// If the value is outside the normalized range then...
			if (rangeValue < 0 || rangeValue > 1)
			{
				// Throw an out of range exception
				throw new ArgumentOutOfRangeException(nameof(rangeValue));
			}

			// Store off the ranged value
			Value = rangeValue;

			// Store off the tag type
			TagType = tagType;

			// Store off the tag name
			Tag = tag;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Ranged value between 0 and 1.
		/// </summary>
		public double Value { get; set; }

		/// <summary>
		/// Type of the tag.  This property is often an enumerated value.
		/// </summary>
		/// <remarks>This property allows the preview to recognize certain ranged values</remarks>
		public int TagType { get; private set; }

		/// <summary>
		/// Tag value.
		/// </summary>
		/// <remarks>Often this represents a function of a display element.</remarks>
		public string Tag { get; private set; }

		#endregion
	}
}