using Vixen.Sys;

namespace VixenModules.Property.State.Setup.Services;

/// <summary>
/// Displays the State mapper for an element node.
/// </summary>
internal interface IStateMapperDialogService
{
	/// <summary>
	/// Displays the mapper for the supplied State property data.
	/// </summary>
	/// <param name="node">The element node being configured.</param>
	/// <param name="data">The State property data to configure.</param>
	/// <returns><see langword="true" /> if the mapper is accepted; otherwise, <see langword="false" />.</returns>
	bool Show(IElementNode node, StateData data);
}
