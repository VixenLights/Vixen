using System.Collections.Generic;
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
		IEnumerable<ElementNode> GenerateElements(IEnumerable<ElementNode> selectedNodes = null);
	}
}
