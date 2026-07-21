using System.ComponentModel;
using System.Globalization;
using Vixen.Marks;

namespace VixenModules.Effect.LipSync
{
	/// <summary>
	/// Converts LipSync Mark Collection selections into the collection names shown by the effect editor.
	/// </summary>
	public sealed class LipSyncMarkCollectionNameConverter : TypeConverter
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
			return value?.ToString();
		}

		/// <inheritdoc />
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return value?.ToString();
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
			var effect = GetLipSyncEffect(context?.Instance);
			var selectedMarkCollectionId = (effect?.ModuleData as LipSyncData)?.MarkCollectionId ?? Guid.Empty;
			var values = GetAllowedMarkCollectionNames(effect?.MarkCollections, selectedMarkCollectionId);

			return new StandardValuesCollection(values.ToArray());
		}

		/// <summary>
		/// Gets the Mark Collection names allowed in the LipSync effect selector.
		/// </summary>
		/// <param name="markCollections">The available Mark Collections.</param>
		/// <param name="selectedMarkCollectionId">The currently selected Mark Collection identifier.</param>
		/// <returns>The allowed Mark Collection names in sequence order.</returns>
		/// <remarks>
		/// Collections tagged as <see cref="MarkCollectionType.Phoneme" /> are always included. The currently selected
		/// collection is also included when it exists so legacy untagged selections remain visible until the user changes them.
		/// </remarks>
		public static IReadOnlyList<string> GetAllowedMarkCollectionNames(IEnumerable<IMarkCollection> markCollections, Guid selectedMarkCollectionId)
		{
			if (markCollections == null)
			{
				return [];
			}

			var values = new List<string>();

			foreach (var markCollection in markCollections)
			{
				if (markCollection.CollectionType == MarkCollectionType.Phoneme || markCollection.Id == selectedMarkCollectionId)
				{
					values.Add(markCollection.Name);
				}
			}

			return values;
		}

		private static LipSync GetLipSyncEffect(object instance)
		{
			if (instance is LipSync lipSyncEffect)
			{
				return lipSyncEffect;
			}

			if (instance is Array effects)
			{
				return effects.OfType<LipSync>().FirstOrDefault(effect => effect.SupportsMarks);
			}

			return null;
		}
	}
}
