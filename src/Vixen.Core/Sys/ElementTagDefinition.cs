#nullable enable

namespace Vixen.Sys
{
	/// <summary>
	/// Represents a single entry in the profile-wide element tag catalog.
	/// </summary>
	/// <remarks>
	/// A tag definition is the profile-level record for a tag &#8212; its stable identity (<see cref="Id"/>),
	/// its stable semantic key for built-in tags (<see cref="Key"/>), and editable display metadata.
	/// <see cref="IElementNode.Tags"/> stores only references to a definition's <see cref="Id"/>, never a copy
	/// of this data, so renaming or re-describing a tag here is automatically reflected everywhere it is assigned.
	/// </remarks>
	public sealed class ElementTagDefinition
	{
		/// <summary>
		/// Gets the stable identifier used for persistence and node tag assignments.
		/// </summary>
		/// <value>A <see cref="Guid"/> that never changes for the lifetime of this tag definition.</value>
		public Guid Id { get; }

		/// <summary>
		/// Gets the stable semantic key used by application code to recognize a built-in tag.
		/// </summary>
		/// <value>
		/// A lowercase, hyphenated identifier such as <c>"deprecated"</c> for a built-in tag, or an empty string
		/// for a user-defined tag.
		/// </value>
		public string Key { get; }

		/// <summary>
		/// Gets or sets the user-facing display name of the tag.
		/// </summary>
		/// <value>The trimmed, non-blank display name shown in tag pickers and lists.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets a value that indicates whether this tag is supplied by Vixen rather than created by the user.
		/// </summary>
		/// <value><see langword="true"/> if the tag is built-in and cannot be renamed or deleted; otherwise, <see langword="false"/>.</value>
		public bool IsBuiltIn { get; }

		/// <summary>
		/// Gets or sets optional explanatory text for the tag.
		/// </summary>
		/// <value>Free-form descriptive text, or <see langword="null"/> if none has been provided.</value>
		public string? Description { get; set; }

		/// <summary>
		/// Gets or sets the optional display color used when presenting the tag in the UI.
		/// </summary>
		/// <value>A color value (for example a hex string such as <c>"#FFA500"</c>), or <see langword="null"/> if unset.</value>
		public string? DisplayColor { get; set; }

		/// <summary>
		/// Gets or sets the ordering value used when presenting tags in pickers and filter lists.
		/// </summary>
		/// <value>Lower values sort first. The default is <c>0</c>.</value>
		public int SortOrder { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ElementTagDefinition"/> class.
		/// </summary>
		/// <param name="id">The stable identifier for this tag.</param>
		/// <param name="key">The stable semantic key for a built-in tag, or <see langword="null"/>/empty for a user-defined tag.</param>
		/// <param name="name">The initial display name of the tag.</param>
		/// <param name="isBuiltIn"><see langword="true"/> if this tag is supplied by Vixen; otherwise, <see langword="false"/>.</param>
		public ElementTagDefinition(Guid id, string? key, string name, bool isBuiltIn)
		{
			Id = id;
			Key = key ?? string.Empty;
			Name = name;
			IsBuiltIn = isBuiltIn;
		}
	}
}
