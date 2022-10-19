using System;

namespace Vixen.Data.Value
{
	/// <summary>
	/// Maintains a tagged ranged value.
	/// </summary>
	/// <typeparam name="T">Enumeration type to use for the tag</typeparam>
	public struct RangeValue<T> : IIntentDataType
		where T : System.Enum
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tagType">Type of the tag</param>
		/// <param name="tag">Name of the tag</param>
		/// <param name="rangeValue">Initial value</param>
		/// <param name="label">Label associated with the range value</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public RangeValue(T tagType, string tag, double rangeValue, string label)
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

			// Store off the label associated with the range value
			Label = label;
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
		/// <remarks>This property allows the preview and Vixen filtering to recognize certain ranged values</remarks>
		public T TagType { get; private set; }

		/// <summary>
		/// Tag value.
		/// </summary>		
		public string Tag { get; private set; }

		/// <summary>
		/// Provides a customizable descriptive string field that labels or describes the intent.
		/// </summary>		
		public string Label { get; private set; }

		#endregion
	}
}