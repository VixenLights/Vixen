using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Vixen.Marks;

namespace VixenModules.Effect.Liquid.Emitters
{
	public class EmitterMarkCollectionNameConverter : TypeConverter
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

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ObservableCollection<IMarkCollection> markCollections = null;
			IEmitter emitter = (IEmitter)context.Instance;
			markCollections = emitter.MarkCollections;
			

			List<string> values = new List<string>();

			if (markCollections != null)
			{
				foreach (var markCollection in markCollections)
				{
					values.Add(markCollection.Name);
				}
			}

			return new TypeConverter.StandardValuesCollection(values.ToArray());
		}
	}
}
