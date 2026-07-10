using Vixen.Sys;

namespace VixenModules.Property.State.Setup.Models
{
	/// <summary>
	/// Stores the stable identity and hierarchy needed to edit State property element assignments.
	/// </summary>
	public sealed class StateElementNodeSnapshot
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateElementNodeSnapshot"/> class.
		/// </summary>
		/// <param name="id">The stable identifier for the element node.</param>
		/// <param name="name">The display name for the element node.</param>
		/// <param name="children">The child element node snapshots.</param>
		public StateElementNodeSnapshot(Guid id, string name, IReadOnlyList<StateElementNodeSnapshot> children)
		{
			Id = id;
			Name = name;
			Children = children;
		}

		/// <summary>
		/// Gets the stable identifier for the element node.
		/// </summary>
		/// <value>The stable identifier for the element node.</value>
		public Guid Id { get; }

		/// <summary>
		/// Gets the display name for the element node.
		/// </summary>
		/// <value>The display name for the element node.</value>
		public string Name { get; }

		/// <summary>
		/// Gets the child element node snapshots.
		/// </summary>
		/// <value>The child element node snapshots.</value>
		public IReadOnlyList<StateElementNodeSnapshot> Children { get; }

		/// <summary>
		/// Gets a value that indicates whether this snapshot has no children.
		/// </summary>
		/// <value><see langword="true" /> if this snapshot has no children; otherwise, <see langword="false" />.</value>
		public bool IsLeaf => Children.Count == 0;

		/// <summary>
		/// Creates a snapshot tree from an element node hierarchy.
		/// </summary>
		/// <param name="node">The root element node to snapshot.</param>
		/// <returns>A snapshot of the element node hierarchy.</returns>
		public static StateElementNodeSnapshot FromElementNode(IElementNode node)
		{
			ArgumentNullException.ThrowIfNull(node);

			return new StateElementNodeSnapshot(
				node.Id,
				node.Name,
				node.Children.Select(FromElementNode).ToList());
		}

		internal IEnumerable<Guid> GetLeafNodeIds()
		{
			return IsLeaf
				? [Id]
				: Children.SelectMany(child => child.GetLeafNodeIds());
		}
	}
}
