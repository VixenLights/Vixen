using System;

namespace Vixen.Attributes
{
	/// <summary>
	///     Specifies a range.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class BoolDescriptionAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="BoolDescriptionAttribute" /> class.
		/// </summary>
		/// <param name="trueValue"></param>
		/// <param name="falseValue"></param>
		public BoolDescriptionAttribute(string trueValue, string falseValue)
		{
			TrueValue = trueValue;
			FalseValue = falseValue;
		}

		/// <summary>
		///     Gets or sets the true value string.
		/// </summary>
		/// <value>The true string.</value>
		public string TrueValue { get; private set; }

		/// <summary>
		///     Gets or sets the false value string.
		/// </summary>
		/// <value>The false string.</value>
		public string FalseValue { get; private set; }
	}
}