namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal static class XLightsStateNameNormalizer
	{
		/// <summary>
		/// Normalizes an imported State property name.
		/// </summary>
		/// <param name="name">The imported State property name.</param>
		/// <param name="stateNumber">The one-based State property number used for fallback naming.</param>
		/// <returns>The trimmed imported name or a deterministic numbered fallback.</returns>
		internal static string NormalizeStateName(string name, int stateNumber)
		{
			var normalizedName = name?.Trim();
			return string.IsNullOrEmpty(normalizedName)
				? $"State Name {stateNumber}"
				: normalizedName;
		}

		/// <summary>
		/// Normalizes an imported State item name.
		/// </summary>
		/// <param name="name">The imported State item name.</param>
		/// <param name="xmlTag">The XML attribute tag used for fallback naming.</param>
		/// <returns>The trimmed imported name or the XML attribute tag.</returns>
		internal static string NormalizeStateItemName(string name, string xmlTag)
		{
			var normalizedName = name?.Trim();
			return string.IsNullOrEmpty(normalizedName)
				? xmlTag
				: normalizedName;
		}
	}
}
