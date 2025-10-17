using System.Collections.ObjectModel;

namespace Vixen.Sys.Props.Model
{
	/// <summary>
	/// Base interface for Prop model implementations
	/// </summary>
	public interface IPropModel
	{
		/// <summary>
		/// Unique Id of the prop model.
		/// </summary>
		Guid Id { get; init; }

	}
}