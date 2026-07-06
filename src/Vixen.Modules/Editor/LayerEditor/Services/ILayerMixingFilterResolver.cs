using Vixen.Module.MixingFilter;

namespace VixenModules.Editor.LayerEditor.Services
{
	/// <summary>
	/// Resolves layer mixing filter instances by module type identifier.
	/// </summary>
	public interface ILayerMixingFilterResolver
	{
		/// <summary>
		/// Creates a new layer mixing filter instance for the specified module type.
		/// </summary>
		/// <param name="filterTypeId">The layer mixing filter module type identifier.</param>
		/// <returns>A new filter instance, or <see langword="null" /> if the module type is not installed.</returns>
		ILayerMixingFilterInstance Resolve(Guid filterTypeId);
	}
}
