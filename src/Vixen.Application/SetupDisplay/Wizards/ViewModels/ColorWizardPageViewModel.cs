using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media;
using Vixen.Annotations;
using Vixen.Services;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Props.Models;
using VixenModules.Property.Color;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	public class ColorWizardPageViewModel : WizardPageViewModelBase<ColorWizardPage>
	{
		public ColorWizardPageViewModel(ColorWizardPage wizardPage) : base(wizardPage)
		{
		}

		[ViewModelToModel]
		public ColorType ColorTypeOption
		{
			get { return GetValue<ColorType>(ColorTypeOptionProperty); }
			set { SetValue(ColorTypeOptionProperty, value); }
		}
		private static readonly IPropertyData ColorTypeOptionProperty = RegisterProperty<ColorType>(nameof(ColorTypeOption));

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