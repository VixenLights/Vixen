#nullable enable

using System.Collections;

namespace Vixen.Sys
{
	/// <summary>
	/// Holds the direct element tag assignments for a single <see cref="IElementNode"/>.
	/// </summary>
	/// <remarks>
	/// This collection stores only stable tag-definition <see cref="Guid"/> references &#8212; never a copy of a
	/// tag's display name or other catalog data. The profile-wide catalog of <see cref="ElementTagDefinition"/>
	/// instances that these references resolve against is managed separately by <c>Vixen.Services.ElementTagService</c>.
	/// </remarks>
	public sealed class ElementTagCollection : IEnumerable<Guid>
	{
		/// <summary>
		/// The single, shared, non-persistent collection used by every proxy node (for example <see cref="ProxyElementNode"/>).
		/// </summary>
		/// <remarks>
		/// <see cref="Add"/> and <see cref="Remove"/> on this instance are silent no-ops: a proxy node does not
		/// participate in the node tree and is never serialized, so it has nothing for a tag assignment to attach to.
		/// </remarks>
		public static readonly ElementTagCollection Empty = new ElementTagCollection(isInert: true);

		private readonly HashSet<Guid> _tagIds;
		private readonly bool _isInert;

		/// <summary>
		/// Initializes a new instance of the <see cref="ElementTagCollection"/> class with no tags assigned.
		/// </summary>
		public ElementTagCollection() : this(isInert: false)
		{
		}

		private ElementTagCollection(bool isInert)
		{
			_tagIds = new HashSet<Guid>();
			_isInert = isInert;
		}

		/// <summary>
		/// Gets the number of tags currently assigned.
		/// </summary>
		/// <value>The count of distinct tag identifiers held by this collection.</value>
		public int Count => _tagIds.Count;

		/// <summary>
		/// Assigns the specified tag to the owning node.
		/// </summary>
		/// <param name="tagId">The identifier of the tag definition to assign.</param>
		/// <returns><see langword="true"/> if the tag was newly assigned; <see langword="false"/> if it was already assigned or this instance is <see cref="Empty"/>.</returns>
		public bool Add(Guid tagId)
		{
			if (_isInert) return false;
			return _tagIds.Add(tagId);
		}

		/// <summary>
		/// Removes the specified tag assignment from the owning node.
		/// </summary>
		/// <param name="tagId">The identifier of the tag definition to unassign.</param>
		/// <returns><see langword="true"/> if the tag was assigned and has been removed; otherwise, <see langword="false"/>.</returns>
		public bool Remove(Guid tagId)
		{
			if (_isInert) return false;
			return _tagIds.Remove(tagId);
		}

		/// <summary>
		/// Determines whether the specified tag is assigned.
		/// </summary>
		/// <param name="tagId">The identifier of the tag definition to look for.</param>
		/// <returns><see langword="true"/> if the tag is assigned; otherwise, <see langword="false"/>.</returns>
		public bool Contains(Guid tagId)
		{
			return _tagIds.Contains(tagId);
		}

		/// <inheritdoc/>
		public IEnumerator<Guid> GetEnumerator()
		{
			return _tagIds.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
