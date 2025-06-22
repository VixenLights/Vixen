namespace Vixen.Sys.Props
{
	public interface ITargetNodeProvider
	{
		/// <summary>
		/// Gets the collection of target element nodes associated with this prop.
		/// </summary>
		/// <remarks>
		/// Target nodes represent the elements within the system that this prop is linked to.
		/// These nodes can be used to define the relationship between the prop and its corresponding elements.
		/// </remarks>
		IEnumerable<IElementNode> TargetNodes { get; }
	}
}