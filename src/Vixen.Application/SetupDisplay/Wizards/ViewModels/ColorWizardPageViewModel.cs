using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using System.Collections.ObjectModel;
using Vixen.Annotations;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Pages;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	public class ColorWizardPageViewModel : WizardPageViewModelBase<ColorWizardPage>
	{
		public ColorWizardPageViewModel(ColorWizardPage wizardPage) : base(wizardPage)
		{
		}

		[ViewModelToModel]
		public System.Drawing.Color SingleColorOption
		{
			get { return GetValue<System.Drawing.Color>(SingleColorProperty); }
			set { SetValue(SingleColorProperty, value); }
		}
		private static readonly IPropertyData SingleColorProperty = RegisterProperty<System.Drawing.Color>(nameof(SingleColorOption));

		[ViewModelToModel]
		public ObservableCollection<string> ColorSetNames
		{
			get => GetValue<ObservableCollection<string>>(ColorSetNamesProperty);
			set => SetValue(ColorSetNamesProperty, value);
		}
		private static readonly IPropertyData ColorSetNamesProperty = RegisterProperty<ObservableCollection<string>>(nameof(ColorSetNames));

		[ViewModelToModel]
		public string SelectedColorSet
		{
			get => GetValue<string>(SelectedColorSetProperty);
			set => SetValue(SelectedColorSetProperty, value);
		}
		private static readonly IPropertyData SelectedColorSetProperty = RegisterProperty<string>(nameof(SelectedColorSet));

		[ViewModelToModel]
		public StringTypes StringType
		{
			get { return GetValue<StringTypes>(StringTypeProperty); }
			set { SetValue(StringTypeProperty, value); }
		}
		private static readonly IPropertyData StringTypeProperty = RegisterProperty<StringTypes>(nameof(StringType));

		/// <summary>
		/// Called when leaving the page
		/// </summary>
		/// <returns>True: if navigation proceeds<br/>False: if navigation is cancelled</returns>
		[UsedImplicitly]
		protected override Task OnClosingAsync()
		{
			return base.OnClosingAsync();
		}
	}
}