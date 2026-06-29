using System.ComponentModel;
using System.Globalization;

namespace VixenModules.Effect.State
{
	/// <summary>
	/// Provides State item options for a custom State item row.
	/// </summary>
	public sealed class CustomStateItemNameConverter: TypeConverter
	{
		/// <inheritdoc />
		public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
		{
			return true;
		}

		/// <inheritdoc />
		public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
		{
			return true;
		}

		/// <inheritdoc />
		public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
		{
			return value.ToString();
		}

		/// <inheritdoc />
		public override object? ConvertTo(
			ITypeDescriptorContext? context,
			CultureInfo? culture,
			object? value,
			Type destinationType)
		{
			return value?.ToString();
		}

		/// <inheritdoc />
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
		{
			return true;
		}

		/// <inheritdoc />
		public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
		{
			return true;
		}

		/// <inheritdoc />
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
		{
			var values = context?.Instance is CustomStateItem customStateItem
				? customStateItem.Parent?.GetCustomStateItemOptions(customStateItem) ?? []
				: [];

			return new StandardValuesCollection(values.ToArray());
		}
	}
}
