using System;

namespace VixenModules.EffectEditor.TypeConverters
{
	[AttributeUsage(AttributeTargets.All)]
	public class RangeAttribute : Attribute
	{
		public static readonly RangeAttribute Default = new RangeAttribute();

		public virtual int Upper
		{
			get
			{
				return UpperValue;
			}
		}

		public virtual int Lower
		{
			get
			{
				return LowerValue;
			}
		}

		protected int LowerValue { get; set; }
		protected int UpperValue { get; set; }

		public RangeAttribute()
			: this(0,100)
		{
		}

		public RangeAttribute(int lower, int upper)
		{
			LowerValue = lower;
			UpperValue = upper;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;
			RangeAttribute rangeAttribute = obj as RangeAttribute;
			if (rangeAttribute != null)
				return rangeAttribute.Lower == Lower && rangeAttribute.Upper == Upper;
			return false;
		}

		public override int GetHashCode()
		{
			int hash = 23;
			hash = hash * 31 + Lower.GetHashCode();
			hash = hash * 31 + Upper.GetHashCode();
			return hash;
		}

		public override bool IsDefaultAttribute()
		{
			return Equals(Default);
		}
	}
}
