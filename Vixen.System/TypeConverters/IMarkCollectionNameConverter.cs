using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module.Effect;

namespace Vixen.TypeConverters
{
	public class IMarkCollectionNameConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return value?.ToString();
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
			Type destinationType)
		{
			return value.ToString();
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ObservableCollection<IMarkCollection> markCollections = null;
			if (context.Instance is Array)
			{
				IEffect[] effects = context.Instance as IEffect[];

				if (effects != null)
				{
					markCollections = effects.FirstOrDefault(x => x.SupportsMarks)?.MarkCollections;
				}

			}
			else
			{
				IEffect effect = (IEffect)context.Instance;
				markCollections = effect.MarkCollections;
			}

			List<string> values = new List<string>();

			if (markCollections != null)
			{
				foreach (var markCollection in markCollections)
				{
					values.Add(markCollection.Name);
				}
			}
		
			return new StandardValuesCollection(values.ToArray());
		}
	}
}
