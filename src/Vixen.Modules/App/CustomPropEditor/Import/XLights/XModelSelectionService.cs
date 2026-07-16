namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal sealed class XModelSelectionService : IXModelSelectionService
	{
		public Task<XModelSelectionItem> SelectModelAsync(IReadOnlyList<XModelSelectionItem> models)
		{
			return Task.FromResult<XModelSelectionItem>(null);
		}
	}
}
