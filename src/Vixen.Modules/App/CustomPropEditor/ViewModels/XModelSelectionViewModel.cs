using Catel.Data;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Import.XLights;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	internal sealed class XModelSelectionViewModel : ViewModelBase
	{
		public XModelSelectionViewModel(IReadOnlyList<XModelSelectionItem> models)
		{
			ArgumentNullException.ThrowIfNull(models);

			Models = models;
			SelectedModel = Models.FirstOrDefault(model => model.IsSupported) ?? Models.FirstOrDefault();
		}

		public override string Title => "Select xModel";

		public IReadOnlyList<XModelSelectionItem> Models
		{
			get { return GetValue<IReadOnlyList<XModelSelectionItem>>(ModelsProperty); }
			private set { SetValue(ModelsProperty, value); }
		}

		public static readonly IPropertyData ModelsProperty = RegisterProperty<IReadOnlyList<XModelSelectionItem>>(nameof(Models));

		public XModelSelectionItem SelectedModel
		{
			get { return GetValue<XModelSelectionItem>(SelectedModelProperty); }
			set { SetValue(SelectedModelProperty, value); }
		}

		public static readonly IPropertyData SelectedModelProperty = RegisterProperty<XModelSelectionItem>(nameof(SelectedModel));

		public Command OkCommand
		{
			get { return field ??= new Command(Ok, CanOk); }
		}

		public Command CancelCommand
		{
			get { return field ??= new Command(Cancel); }
		}

		private bool CanOk()
		{
			return SelectedModel != null;
		}

		private void Ok()
		{
			this.SaveAndCloseViewModelAsync();
		}

		private void Cancel()
		{
			this.CancelAndCloseViewModelAsync();
		}
	}
}
