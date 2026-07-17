namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal interface IXModelSelectionService
	{
		Task<XModelSelectionItem> SelectModelAsync(IReadOnlyList<XModelSelectionItem> models);
	}
}
