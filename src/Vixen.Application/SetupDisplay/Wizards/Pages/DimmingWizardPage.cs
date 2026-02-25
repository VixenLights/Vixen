using System.ComponentModel;
using Catel.Data;
using Orc.Wizard;
using Vixen.Extensions;
using Vixen.Sys.Props;
using VixenModules.App.Curves;
using VixenModules.App.Props.Models;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	public class DimmingWizardPage : WizardPageBase
	{
		#region Constructors
		public DimmingWizardPage()
		{
			// Set with some default parameters
			Title = "Brightness Level";
			Description = "Configure the brightness level";
			
			Curve = null;
			DimmingTypeOption = DimmingType.NoCurve;
			Brightness = 80;
			Gamma = 2.2;
		}
		#endregion

		#region Public Properties
		public DimmingType DimmingTypeOption
		{
			get { return GetValue<DimmingType>(DimmingTypeOptionProperty); }
			set { SetValue(DimmingTypeOptionProperty, value); }
		}
		private static readonly IPropertyData DimmingTypeOptionProperty = RegisterProperty<DimmingType>(nameof(DimmingTypeOption));

		public int Brightness
		{
			get { return GetValue<int>(BrightnessProperty); }
			set 
			{ 
				SetValue(BrightnessProperty, value);
				BrightnessDefault = value;
			}
		}
		private static readonly IPropertyData BrightnessProperty = RegisterProperty<int>(nameof(Brightness));

		public int BrightnessDefault
		{
			get { return GetValue<int>(BrightnessDefaultProperty); }
			set { SetValue(BrightnessDefaultProperty, value); }
		}
		private static readonly IPropertyData BrightnessDefaultProperty = RegisterProperty<int>(nameof(BrightnessDefault));

		public double Gamma
		{
			get { return GetValue<double>(GammaProperty); }
			set 
			{ 
				SetValue(GammaProperty, value);
				GammaDefault = value;
			}
		}
		private static readonly IPropertyData GammaProperty = RegisterProperty<double>(nameof(Gamma));

		public double GammaDefault
		{
			get { return GetValue<double>(GammaDefaultProperty); }
			set { SetValue(GammaDefaultProperty, value); }
		}
		private static readonly IPropertyData GammaDefaultProperty = RegisterProperty<double>(nameof(GammaDefault));

		public Curve Curve
		{
			get { return GetValue<Curve>(CurveProperty); }
			set { SetValue(CurveProperty, value); }
		}
		private static readonly IPropertyData CurveProperty = RegisterProperty<Curve>(nameof(Curve));

		private PropType _propType;
		public PropType PropType
		{
			set { 
				_propType = value;
				Description += $" for {_propType.GetEnumDescription()}";
			}
		}
		#endregion

		#region Base class overrides
		public override ISummaryItem GetSummary()
		{
			string curveName = "Full brightness";

			if (DimmingTypeOption == DimmingType.Simple)
			{
				curveName = $"Maximum Brightness: {Brightness}%\nBrightness Speed: {Gamma:0.0}";
			}
			else if (DimmingTypeOption == DimmingType.Library && Curve != null)
			{
				if (Curve.LibraryReferenceName != string.Empty)
				{
					curveName = $"Library Curve: {Curve.LibraryReferenceName}";
				}
				else if (Curve.Points.Count > 0)
				{
					curveName = "Custom Curve";
				}
			}

			return new SummaryItem
			{
				Title = this.Title,
				Summary = curveName
			};
		}
		#endregion
	}
}
