using Vixen.Sys;

namespace Vixen.Rule
{
	public interface IElementTemplate
	{
		/// <summary>
		/// The descriptive name for the template that should be displayed to the user.
		/// </summary>
		string TemplateName { get; }

		/// <summary>
		/// Tells the tempate to 'configure' itself for element generation: presumably this would
		/// involve displaying a setup dialog to the user, or something similar, to prompt them
		/// for parameters needed for name generation, etc.
		/// </summary>
		/// <param name="selectedNodes">The node(s) that have been selected by the user (if any) when called.</param>
		/// <returns>true if the setup was successful and template generation should proceed (ie. the user selected
		/// "OK", or everything is good to go), false if not (eg. the user cancelled the setup).</returns>
		bool SetupTemplate(IEnumerable<ElementNode> selectedNodes = null);

		/// <summary>
		/// Generates and adds elements to the system, according to the template rules and parameters.
		/// </summary>
		/// <param name="selectedNodes">The node(s) that have been selected by the user (if any) when called.
		/// These may be used as the 'parent' for ths generated items, for example.</param>
		/// <returns>The element nodes that were generated and added to the system elements during the prcess.</returns>
		Task<IEnumerable<ElementNode>> GenerateElements(IEnumerable<ElementNode> selectedNodes = null);

		/// <summary>
		/// True when the template was cancelled by the user.
		/// </summary>
		bool Cancelled { get; }

		/// <summary>
		/// Indicates if the associated elements require further configuration of color properties.
		/// </summary>
		bool ConfigureColor { get; }

		/// <summary>
		/// Indicates if the associated elements require further configuration of dimming curves.
		/// </summary>
		bool ConfigureDimming { get; }

		/// <summary>
		/// Returns elements that should be deleted by the caller.
		/// Some templates create individual nodes and a group.  Once the group is created the
		/// individual nodes can be removed from the tree.
		/// </summary>
		/// <returns>Element nodes to delete</returns>
		IEnumerable<ElementNode> GetElementsToDelete();

		/// <summary>
		/// Returns the collection of leaf nodes created by the template.
		/// </summary>
		/// <returns>Leaf nodes created by the template</returns>
		IEnumerable<ElementNode> GetLeafNodes();
	}
}
