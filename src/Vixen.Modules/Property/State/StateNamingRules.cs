namespace VixenModules.Property.State
{
	public static class StateNamingRules
	{
		/// <summary>
		/// The default State definition name.
		/// </summary>
		public const string DefaultDefinitionName = "State";
		
		/// <summary>
		/// The default State Item name.
		/// </summary>
		public const string DefaultItemName = "State Item";
		
		public static string GetNextStateDefinitionName(IReadOnlyCollection<string> existingNames)
		{
			return GetNextName(DefaultDefinitionName, existingNames);
		}
		
		public static string GetNextStateItemName(IReadOnlyCollection<string> existingNames)
		{
			return GetNextName(DefaultItemName, existingNames);
		}

		/// <summary>
		/// Generates an unused State Item name based on the source name.
		/// </summary>
		/// <param name="baseName">The complete source name, including any existing suffix.</param>
		/// <param name="existingNames">The names already used in the selected State Definition.</param>
		/// <returns>The trimmed base name with the first unused dash-number suffix, starting at 1.</returns>
		/// <remarks>
		/// Uses <see cref="DefaultItemName"/> when the trimmed base is empty.
		/// Compares candidate names case-sensitively and preserves existing suffixes in the base.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="baseName"/> or <paramref name="existingNames"/> is <see langword="null"/>.
		/// </exception>
		public static string GetNextStateItemName(string baseName, IReadOnlyCollection<string> existingNames)
		{
			ArgumentNullException.ThrowIfNull(baseName);
			ArgumentNullException.ThrowIfNull(existingNames);

			var normalizedBaseName = baseName.Trim();
			return GetNextName(normalizedBaseName.Length == 0 ? DefaultItemName : normalizedBaseName, existingNames);
		}

		private static string GetNextName(string baseName, IReadOnlyCollection<string> existingNames)
		{
			var index = 1;
			string candidate;
			do
			{
				candidate = $"{baseName} - {index}";
				index++;
			}
			while (existingNames.Contains(candidate));

			return candidate;
		}

		public static bool TryNormalizeName(string? name, string? currentName, 
			IReadOnlyCollection<string> existingNames, out string normalizedName)
		{
			var candidateName = name?.Trim() ?? string.Empty;
			normalizedName = candidateName;
			return !string.IsNullOrWhiteSpace(candidateName) &&
			       !existingNames.Any(definition =>
				       !definition.Equals(currentName, StringComparison.Ordinal) &&
				       definition.Equals(candidateName, StringComparison.Ordinal));
		}
	}
}
