using Vixen.Sys;

namespace VixenModules.Property.State.Setup.Services
{
	/// <summary>
	/// Selects a color for a State property item.
	/// </summary>
	public interface IStateColorPickerService
	{
		/// <summary>
		/// Displays the applicable Vixen color chooser.
		/// </summary>
		/// <param name="nodes">The element nodes whose color restrictions should be honored.</param>
		/// <param name="initialColor">The color selected when the chooser opens.</param>
		/// <returns>The selected color, or <see langword="null" /> when the chooser is canceled.</returns>
		Task<System.Drawing.Color?> ChooseColorAsync(IEnumerable<IElementNode> nodes, System.Drawing.Color initialColor);
	}
}
