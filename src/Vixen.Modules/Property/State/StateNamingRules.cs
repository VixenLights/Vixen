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
