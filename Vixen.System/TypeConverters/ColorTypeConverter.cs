using System;
using System.ComponentModel;
using System.Drawing;

namespace Vixen.TypeConverters
{
	public class ColorTypeConverter : ColorConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return false;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return false;
			return base.CanConvertFrom(context, sourceType);
		}
	}
}
