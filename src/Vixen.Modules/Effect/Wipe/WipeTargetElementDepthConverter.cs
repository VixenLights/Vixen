using System.ComponentModel;
using System.Globalization;
using Vixen.Module.Effect;

namespace VixenModules.Effect.Wipe
{
	/// <summary>
	/// Provides useful target-depth choices for the Wipe effect.
	/// </summary>
	public class WipeTargetElementDepthConverter : TypeConverter
	{
		/// <inheritdoc />
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		/// <inheritdoc />
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}

		/// <inheritdoc />
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return Convert.ToInt32(value);
		}

		/// <inheritdoc />
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			ArgumentNullException.ThrowIfNull(value);
			return value.ToString();
		}

		/// <inheritdoc />
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <inheritdoc />
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <inheritdoc />
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			var depth = GetDepth(context);
			var values = new List<string>();
			for (var i = 1; i < depth - 1; i++)
			{
				values.Add(i.ToString());
			}

			return new StandardValuesCollection(values.ToArray());
		}

		private static int GetDepth(ITypeDescriptorContext context)
		{
			if (context?.Instance is IEffect[] effects)
			{
				var depth = Int32.MaxValue;
				foreach (var effect in effects)
				{
					var node = effect.TargetNodes.FirstOrDefault();
					if (node != null)
					{
						depth = Math.Min(node.GetMaxChildDepth(), depth);
					}
				}

				return depth == Int32.MaxValue ? 0 : depth;
			}

			if (context?.Instance is IEffect effectInstance)
			{
				return effectInstance.TargetNodes.FirstOrDefault()?.GetMaxChildDepth() ?? 0;
			}

			return 0;
		}
	}
}
