using Vixen.Model;

namespace Vixen.Sys.Props.Model
{
	/// <summary>
	/// Abstract base class for prop model.
	/// </summary>
	public abstract class BasePropModel : BindableBase
	{
		#region IPropModel
		
		/// <inheritdoc/>		
		public Guid Id { get; init; } = Guid.NewGuid();

		#endregion
	}
}
