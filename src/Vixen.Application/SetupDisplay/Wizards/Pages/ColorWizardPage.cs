using Catel.Data;
using Orc.Wizard;
using System.Collections.ObjectModel;
using Vixen.Services;
using VixenModules.App.Props.Models;
using VixenModules.Property.Color;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	public class ColorWizardPage : WizardPageBase
	{
		#region Constructors
		public ColorWizardPage()
		{
			// Set with some default parameters
			Title = "Color Configuration";
			Description = "Configure how this Prop handles color";

			ColorTypeOption = ColorType.SingleColor;
			SingleColorOption = Color.RoyalBlue;
			var staticData = ApplicationServices.GetModuleStaticData(ColorDescriptor.ModuleId) as ColorStaticData;
			if (staticData != null)
			{
				ColorSetNames = new ObservableCollection<string>(staticData.GetColorSetNames());
				SelectedColorSet = ColorSetNames[0];
			}
		}
		#endregion

		#region Public Properties
		public ColorType ColorTypeOption
		{
			get { return GetValue<ColorType>(ColorTypeOptionProperty); }
			set { SetValue(ColorTypeOptionProperty, value); }
		}
		private static readonly IPropertyData ColorTypeOptionProperty = RegisterProperty<ColorType>(nameof(ColorTypeOption));

		public Color SingleColorOption
		{
			get { return GetValue<Color>(SingleColorProperty); }
			set { SetValue(SingleColorProperty, value); }
		}
		private static readonly IPropertyData SingleColorProperty = RegisterProperty<Color>(nameof(SingleColorOption));

		public ObservableCollection<string> ColorSetNames
		{
			get => GetValue<ObservableCollection<string>>(ColorSetNamesProperty);
			set => SetValue(ColorSetNamesProperty, value);
		}
		private static readonly IPropertyData ColorSetNamesProperty = RegisterProperty<ObservableCollection<string>>(nameof(ColorSetNames));

		public string SelectedColorSet
		{
			get => GetValue<string>(SelectedColorSetProperty);
			set => SetValue(SelectedColorSetProperty, value);
		}
		private static readonly IPropertyData SelectedColorSetProperty = RegisterProperty<string>(nameof(SelectedColorSet));
		#endregion

		#region Base class overrides
		public override ISummaryItem GetSummary()
		{
			string colorInfo = "None Specified";

			if (ColorTypeOption == ColorType.SingleColor)
			{
				colorInfo = $"Single Color:\n    Red is {SingleColorOption.R}\n    Green is {SingleColorOption.G}\n    Blue is {SingleColorOption.B}";
			}
			else if (ColorTypeOption == ColorType.MultipleColors)
			{
				colorInfo = $"Multiple Colors: {SelectedColorSet}";
			}
			if (ColorTypeOption == ColorType.RGBColors)
			{
				colorInfo = $"RGB Colors: {SelectedColorSet}";
			}

			return new SummaryItem
			{
				Title = this.Title,
				Summary = colorInfo
			};
		}
		#endregion
	}
}
