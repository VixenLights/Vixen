using System;

namespace Vixen.Attributes
{
	/// <summary>
	/// Specifies an offset
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class OffsetAttribute: Attribute
	{
		public OffsetAttribute(int offset)
		{
			Offset = offset;
		}
		/// <summary>
		/// Gets or Sets the offset
		/// </summary>
		public int Offset { get; private set; }
	}
}
