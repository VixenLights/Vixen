#nullable enable

using Vixen.Sys;

namespace Vixen.Services
{
	/// <summary>
	/// Provides catalog management for element tag definitions and lookups against node tag assignments.
	/// </summary>
	/// <remarks>
	/// This service owns the profile-wide tag catalog (<see cref="VixenSystem.TagDefinitions"/>). It does not
	/// own per-node assignments &#8212; those are read and mutated directly through <see cref="IElementNode.Tags"/>.
	/// </remarks>
	public class ElementTagService
	{
		private static ElementTagService? _instance;

		private ElementTagService()
		{
		}

		/// <summary>
		/// Gets the singleton instance of the service.
		/// </summary>
		/// <value>The shared <see cref="ElementTagService"/> instance.</value>
		public static ElementTagService Instance
		{
			get { return _instance ??= new ElementTagService(); }
		}

		/// <summary>
		/// Gets every tag definition in the profile's tag catalog.
		/// </summary>
		/// <returns>The tag definitions, ordered by <see cref="ElementTagDefinition.SortOrder"/> then by <see cref="ElementTagDefinition.Name"/>.</returns>
		public IReadOnlyList<ElementTagDefinition> GetAll()
		{
			return VixenSystem.TagDefinitions
				.OrderBy(tag => tag.SortOrder)
				.ThenBy(tag => tag.Name, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}

		/// <summary>
		/// Gets the tag definition with the specified semantic key.
		/// </summary>
		/// <param name="key">The stable semantic key of a built-in tag, e.g. <see cref="BuiltInElementTags.DeprecatedKey"/>.</param>
		/// <returns>The matching tag definition, or <see langword="null"/> if no tag has that key.</returns>
		public ElementTagDefinition? GetByKey(string key)
		{
			return VixenSystem.TagDefinitions.FirstOrDefault(tag => tag.Key == key);
		}

		/// <summary>
		/// Gets the tag definition with the specified identifier.
		/// </summary>
		/// <param name="id">The stable identifier of the tag definition.</param>
		/// <returns>The matching tag definition, or <see langword="null"/> if no tag has that identifier.</returns>
		public ElementTagDefinition? GetById(Guid id)
		{
			return VixenSystem.TagDefinitions.FirstOrDefault(tag => tag.Id == id);
		}

		/// <summary>
		/// Creates and adds a new user-defined tag to the catalog.
		/// </summary>
		/// <param name="name">The display name of the new tag.</param>
		/// <returns>The newly created tag definition.</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="name"/> is blank after trimming, or an existing tag already has that name (case-insensitive).
		/// </exception>
		public ElementTagDefinition CreateUserTag(string name)
		{
			string trimmedName = ValidateAndNormalizeTagName(name, VixenSystem.TagDefinitions);

			var tag = new ElementTagDefinition(Guid.NewGuid(), key: string.Empty, trimmedName, isBuiltIn: false);
			VixenSystem.TagDefinitions.Add(tag);
			return tag;
		}

		/// <summary>
		/// Renames an existing user-defined tag.
		/// </summary>
		/// <param name="id">The identifier of the tag to rename.</param>
		/// <param name="newName">The new display name for the tag.</param>
		/// <remarks>
		/// Node tag assignments reference a tag by <see cref="ElementTagDefinition.Id"/>, not by name, so renaming
		/// a tag does not touch any node's <see cref="IElementNode.Tags"/> collection.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// No tag with <paramref name="id"/> exists, or the tag is built-in.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="newName"/> is blank after trimming, or another existing tag already has that name (case-insensitive).
		/// </exception>
		public void RenameUserTag(Guid id, string newName)
		{
			ElementTagDefinition tag = GetById(id) ?? throw new InvalidOperationException("No tag exists with the specified id.");
			if (tag.IsBuiltIn)
			{
				throw new InvalidOperationException("Built-in tags cannot be renamed.");
			}

			string trimmedName = ValidateAndNormalizeTagName(newName, VixenSystem.TagDefinitions.Where(existing => existing.Id != id));
			tag.Name = trimmedName;
		}

		/// <summary>
		/// Deletes an existing user-defined tag and removes it from every node that referenced it.
		/// </summary>
		/// <param name="id">The identifier of the tag to delete.</param>
		/// <remarks>
		/// This method does not prompt for confirmation. Confirming a deletion with the user is a UI-layer
		/// concern for the future Display Setup/Sequencer work, not this service.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// No tag with <paramref name="id"/> exists, or the tag is built-in.
		/// </exception>
		public void DeleteUserTag(Guid id)
		{
			ElementTagDefinition tag = GetById(id) ?? throw new InvalidOperationException("No tag exists with the specified id.");
			if (tag.IsBuiltIn)
			{
				throw new InvalidOperationException("Built-in tags cannot be deleted.");
			}

			VixenSystem.TagDefinitions.Remove(tag);

			foreach (ElementNode node in VixenSystem.Nodes.GetAllNodes())
			{
				node.Tags.Remove(id);
			}
		}

		/// <summary>
		/// Determines whether a node has the specified tag assigned.
		/// </summary>
		/// <param name="node">The node to check.</param>
		/// <param name="tagId">The identifier of the tag to look for.</param>
		/// <returns><see langword="true"/> if the tag is assigned to <paramref name="node"/>; otherwise, <see langword="false"/>.</returns>
		public bool HasTag(IElementNode node, Guid tagId)
		{
			return node.Tags.Contains(tagId);
		}

		/// <summary>
		/// Determines whether a node has the tag with the specified semantic key assigned.
		/// </summary>
		/// <param name="node">The node to check.</param>
		/// <param name="key">The stable semantic key of the tag, e.g. <see cref="BuiltInElementTags.DeprecatedKey"/>.</param>
		/// <returns>
		/// <see langword="true"/> if a tag with <paramref name="key"/> exists and is assigned to <paramref name="node"/>;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		public bool HasTag(IElementNode node, string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}

			ElementTagDefinition? tag = GetByKey(key);
			return tag != null && HasTag(node, tag.Id);
		}

		/// <summary>
		/// Trims a candidate tag name and validates it against blank and case-insensitive duplicate-name rules.
		/// </summary>
		/// <param name="name">The candidate name, in any leading/trailing whitespace form.</param>
		/// <param name="existing">The existing tag definitions to check for a duplicate name against.</param>
		/// <returns>The trimmed name, ready for storage.</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="name"/> is blank after trimming, or <paramref name="existing"/> already contains a tag
		/// with the trimmed name (case-insensitive).
		/// </exception>
		internal static string ValidateAndNormalizeTagName(string name, IEnumerable<ElementTagDefinition> existing)
		{
			string trimmedName = (name ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(trimmedName))
			{
				throw new ArgumentException("Tag name cannot be blank.", nameof(name));
			}

			if (existing.Any(tag => string.Equals(tag.Name, trimmedName, StringComparison.OrdinalIgnoreCase)))
			{
				throw new ArgumentException($"A tag named \"{trimmedName}\" already exists.", nameof(name));
			}

			return trimmedName;
		}
	}
}
