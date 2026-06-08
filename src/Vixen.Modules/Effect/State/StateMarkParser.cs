namespace VixenModules.Effect.State
{
	internal static class StateMarkParser
	{
		internal static IReadOnlyList<string> ParseStateItemNames(string? markText)
		{
			return (markText ?? string.Empty)
				.Split(',')
				.Select(segment => segment.Trim())
				.ToList();
		}
	}
}
