using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Vixen.Annotations;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Curves;
using VixenModules.App.Props.Models;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	public class DimmingWizardPageViewModel : WizardPageViewModelBase<DimmingWizardPage>
	{
		public DimmingWizardPageViewModel(DimmingWizardPage wizardPage) : base(wizardPage)
		{
		}

		[ViewModelToModel]
		public int Brightness
		{
			get { return GetValue<int>(BrightnessProperty); }
			set { SetValue(BrightnessProperty, value); }
		}
		private static readonly IPropertyData BrightnessProperty = RegisterProperty<int>(nameof(Brightness));

		[ViewModelToModel]
		public int BrightnessDefault
		{
			get { return GetValue<int>(BrightnessDefaultProperty); }
			set { SetValue(BrightnessDefaultProperty, value); }
		}
		private static readonly IPropertyData BrightnessDefaultProperty = RegisterProperty<int>(nameof(BrightnessDefault));

		[ViewModelToModel]
		public double Gamma
		{
			get { return GetValue<double>(GammaProperty); }
			set { SetValue(GammaProperty, value); }
		}
		private static readonly IPropertyData GammaProperty = RegisterProperty<double>(nameof(Gamma));

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