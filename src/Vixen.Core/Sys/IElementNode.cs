namespace Vixen.Sys
{
	/// <summary>
	/// Represents a logical node &#8212; a single element or a branch/group of other nodes &#8212; in a display's element tree.
	/// </summary>
	public interface IElementNode : IGroupNode
	{
		Element Element { get; set; }

		Guid Id { get; }

		string Name { get; set; }

		IEnumerable<IElementNode> Children { get; }

		IEnumerable<IElementNode> Parents { get; }

		bool IsLeaf { get; }

		int GetMaxChildDepth();

		IEnumerable<IElementNode> GetLeafEnumerator();

		IEnumerable<Element> GetElementEnumerator();

		IEnumerable<IElementNode> GetNodeEnumerator();

		IEnumerable<IElementNode> GetNonLeafEnumerator();

		//IEnumerable<IElementNode> GetAllParentNodes();

		PropertyManager Properties { get; }

		bool IsProxy { get; }

		/// <summary>
		/// Gets this node's direct element tag assignments.
		/// </summary>
		/// <value>
		/// A collection of stable tag-definition identifiers assigned directly to this node. This is not the
		/// profile-wide tag catalog &#8212; it holds only this node's own assignments, resolved against the catalog
		/// managed by <c>Vixen.Services.ElementTagService</c>.
		/// </value>
		ElementTagCollection Tags { get; }
	}
}