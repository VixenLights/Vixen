#nullable enable

namespace Vixen.Sys
{
	/// <summary>
	/// Provides the stable identities and factory for Vixen's built-in element tags.
	/// </summary>
	/// <remarks>
	/// The <see cref="Guid"/> values below are fixed and must never be regenerated: node tag assignments
	/// reference a tag definition's <see cref="ElementTagDefinition.Id"/>, so changing these values would
	/// silently orphan every existing <c>Deprecated</c>, <c>Hidden</c>, or <c>Prop</c> assignment on reseed.
	/// </remarks>
	public static class BuiltInElementTags
	{
		/// <summary>
		/// The stable semantic key for the built-in <c>Deprecated</c> tag.
		/// </summary>
		public const string DeprecatedKey = "deprecated";

		/// <summary>
		/// The stable semantic key for the built-in <c>Hidden</c> tag.
		/// </summary>
		public const string HiddenKey = "hidden";

		/// <summary>
		/// The stable semantic key for the built-in <c>Prop</c> tag.
		/// </summary>
		public const string PropKey = "prop";

		/// <summary>
		/// The fixed, stable identifier for the built-in <c>Deprecated</c> tag definition.
		/// </summary>
		public static readonly Guid DeprecatedId = new Guid("753115da-e27a-4261-a136-8222ccc3f22e");

		/// <summary>
		/// The fixed, stable identifier for the built-in <c>Hidden</c> tag definition.
		/// </summary>
		public static readonly Guid HiddenId = new Guid("2f21bfcb-07a3-4cac-bdc5-4b6e37773c04");

		/// <summary>
		/// The fixed, stable identifier for the built-in <c>Prop</c> tag definition.
		/// </summary>
		public static readonly Guid PropId = new Guid("6db1852d-71d8-449d-afa5-193a6def26a0");

		/// <summary>
		/// Creates fresh instances of all built-in tag definitions.
		/// </summary>
		/// <returns>A list containing the <c>Deprecated</c>, <c>Hidden</c>, and <c>Prop</c> tag definitions, in that order.</returns>
		public static IReadOnlyList<ElementTagDefinition> CreateDefaults()
		{
			return new[]
			{
				new ElementTagDefinition(DeprecatedId, DeprecatedKey, "Deprecated", isBuiltIn: true) { SortOrder = 0 },
				new ElementTagDefinition(HiddenId, HiddenKey, "Hidden", isBuiltIn: true) { SortOrder = 1 },
				new ElementTagDefinition(PropId, PropKey, "Prop", isBuiltIn: true) { SortOrder = 2 },
			};
		}
	}
}
