using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Rule
{
	public interface IElementSetupHelper
	{
		/// <summary>
		/// The descriptive name for the setup 'helper' or 'wizard' that should be displayed to the user.
		/// </summary>
		string HelperName { get; }

		/// <summary>
		/// Tells the helper to do its work.
		/// </summary>
		/// <param name="selectedNodes">The node(s) that have been selected by the user when called.</param>
		/// <returns>true if the process was successful, false if not.</returns>
		bool Perform(IEnumerable<IElementNode> selectedNodes);
	}
}
