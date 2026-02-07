using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Curves;
using static VixenApplication.SetupDisplay.Wizards.Pages.DimmingWizardPage;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	public class DimmingWizardPageViewModel : WizardPageViewModelBase<DimmingWizardPage>
	{
		public DimmingWizardPageViewModel(DimmingWizardPage wizardPage) : base(wizardPage)
		{
		}

		[ViewModelToModel]
		public DimmingType DimmingTypeOption
		{
			get { return GetValue<DimmingType>(DimmingTypeOptionProperty); }
			set { SetValue(DimmingTypeOptionProperty, value); }
		}
		private static readonly IPropertyData DimmingTypeOptionProperty = RegisterProperty<DimmingType>(nameof(DimmingTypeOption));

		[ViewModelToModel]
		public int Brightness
		{
			get { return GetValue<int>(BrightnessProperty); }
			set { SetValue(BrightnessProperty, value); }
		}
		private static readonly IPropertyData BrightnessProperty = RegisterProperty<int>(nameof(Brightness));

		[ViewModelToModel]
		public double Gamma
		{
			get { return GetValue<double>(GammaProperty); }
			set { SetValue(GammaProperty, value); }
		}
		private static readonly IPropertyData GammaProperty = RegisterProperty<double>(nameof(Gamma));


		[ViewModelToModel]
		public Curve Curve
		{
			get { return GetValue<Curve>(CurveProperty); }
			set { SetValue(CurveProperty, value); }
		}
		private static readonly IPropertyData CurveProperty = RegisterProperty<Curve>(nameof(Curve));
	}
}