using Catel.Data;
using Orc.Wizard;
using System.Runtime.CompilerServices;
using Vixen.Extensions;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Model;
using VixenModules.App.Curves;
using VixenModules.App.Props.Models;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	public class DimmingWizardPage : WizardPageBase
	{
		public DimmingWizardPage()
		{
			Title = "Dimming Curve";
			Description = $"Enter dimming information";

			// Set with some default parameters
			Curve = null;
			DimmingTypeOption = DimmingType.NoCurve;
			Brightness = 80;
			Gamma = 2.2;
		}

		public DimmingType DimmingTypeOption
		{
			get { return GetValue<DimmingType>(DimmingTypeOptionProperty); }
			set { SetValue(DimmingTypeOptionProperty, value); }
		}
		private static readonly IPropertyData DimmingTypeOptionProperty = RegisterProperty<DimmingType>(nameof(DimmingTypeOption));

		public int Brightness
		{
			get { return GetValue<int>(BrightnessProperty); }
			set { SetValue(BrightnessProperty, value); }
		}
		private static readonly IPropertyData BrightnessProperty = RegisterProperty<int>(nameof(Brightness));

		public double Gamma
		{
			get { return GetValue<double>(GammaProperty); }
			set { SetValue(GammaProperty, value); }
		}
		private static readonly IPropertyData GammaProperty = RegisterProperty<double>(nameof(Gamma));

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
				Description = $"Enter dimming information for {_propType.GetEnumDescription()}";
			}
		}

		public override ISummaryItem GetSummary()
		{
			string curveName = "None Specified";
			if (DimmingTypeOption == DimmingType.Simple)
			{
				curveName = $"Brightness: {Brightness}%, Gamma: {Gamma:0.0}";
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

		//public IProp GetProp()
		//{
		//	return null;
		//}
	}
}
