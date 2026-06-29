using System.Drawing;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Provides discrete color choices for an editor component.
	/// </summary>
	public interface IDiscreteColorProvider
	{
		/// <summary>
		/// Gets the discrete colors that should constrain color editing for this component.
		/// </summary>
		/// <returns>The valid discrete colors, or an empty set when full color editing is allowed.</returns>
		HashSet<Color> GetDiscreteColors();
	}
}
