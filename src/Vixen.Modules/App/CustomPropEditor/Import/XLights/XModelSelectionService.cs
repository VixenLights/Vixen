namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	using Catel.IoC;
	using Catel.Services;
	using VixenModules.App.CustomPropEditor.ViewModels;

	internal sealed class XModelSelectionService : IXModelSelectionService
	{
		public async Task<XModelSelectionItem> SelectModelAsync(IReadOnlyList<XModelSelectionItem> models)
		{
			var viewModel = new XModelSelectionViewModel(models);
			var dependencyResolver = this.GetDependencyResolver();
			var uiVisualizerService = dependencyResolver.Resolve<IUIVisualizerService>();
			var busyIndicatorService = dependencyResolver.Resolve<IBusyIndicatorService>();
			busyIndicatorService.Hide();
			try
			{
				var result = await uiVisualizerService.ShowDialogAsync(viewModel);

				return result.DialogResult == true
					? viewModel.SelectedModel
					: null;
			}
			finally
			{
				busyIndicatorService.Show();
			}
		}
	}
}
