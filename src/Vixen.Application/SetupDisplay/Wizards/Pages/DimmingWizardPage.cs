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
			
			Brightness = 100;
			Gamma = 1;
		}
		#endregion

		#region Public Properties
		public int Brightness
		{
			get => GetValue<int>(BrightnessProperty);
			set 
			{ 
				SetValue(BrightnessProperty, value);
				BrightnessDefault = value;
			}
		}
		private static readonly IPropertyData BrightnessProperty = RegisterProperty<int>(nameof(Brightness));

		public int BrightnessDefault
		{
			get => GetValue<int>(BrightnessDefaultProperty);
			set => SetValue(BrightnessDefaultProperty, value);
		}
		private static readonly IPropertyData BrightnessDefaultProperty = RegisterProperty<int>(nameof(BrightnessDefault));

		public double Gamma
		{
			get => GetValue<double>(GammaProperty);
			set => SetValue(GammaProperty, value);
		}
		private static readonly IPropertyData GammaProperty = RegisterProperty<double>(nameof(Gamma));

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
			return new SummaryItem
			{
				Title = this.Title,
				Summary = $"Maximum Brightness: {Brightness}%\nGamma: {Gamma:0.0}"
			};
		}
		#endregion
	}
}
