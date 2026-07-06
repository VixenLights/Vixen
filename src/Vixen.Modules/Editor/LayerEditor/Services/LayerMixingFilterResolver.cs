using Vixen.Module.MixingFilter;
using Vixen.Services;

namespace VixenModules.Editor.LayerEditor.Services
{
	/// <summary>
	/// Resolves layer mixing filter instances through <see cref="LayerMixingFilterService"/>.
	/// </summary>
	public sealed class LayerMixingFilterResolver : ILayerMixingFilterResolver
	{
		/// <inheritdoc />
		public ILayerMixingFilterInstance Resolve(Guid filterTypeId)
		{
			return LayerMixingFilterService.Instance.GetInstance(filterTypeId);
		}
	}
}
