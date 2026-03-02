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

		[ViewModelToModel]
		public double GammaDefault
		{
			get { return GetValue<double>(GammaDefaultProperty); }
			set { SetValue(GammaDefaultProperty, value); }
		}
		private static readonly IPropertyData GammaDefaultProperty = RegisterProperty<double>(nameof(GammaDefault));

		[ViewModelToModel]
		public Curve Curve
		{
			get { return GetValue<Curve>(CurveProperty); }
			set { SetValue(CurveProperty, value); }
		}
		private static readonly IPropertyData CurveProperty = RegisterProperty<Curve>(nameof(Curve));

		private void OnGammaChanged()
		{
			GammaDefault = Gamma;
		}

		/// <summary>
		/// Called when leaving the page
		/// </summary>
		/// <returns>True: if navigation proceeds<br/>False: if navigation is cancelled</returns>
		[UsedImplicitly]
		protected override Task OnClosingAsync()
		{
			// If the simple curve is set, then generate a gamma curve
			//if (DimmingTypeOption == DimmingType.Simple)
			//{
			//	Curve = new Curve().SetGamma(Gamma, Brightness);
			//}
			return base.OnClosingAsync();
		}
	}
}