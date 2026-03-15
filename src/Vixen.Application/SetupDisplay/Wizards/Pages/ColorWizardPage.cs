using Catel.Data;
using Common.WPFCommon.Converters;
using Orc.Wizard;
using System.Collections.ObjectModel;
using Vixen.Services;
using Vixen.Sys.Props;
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

			StringType = StringTypes.ColorMixingRGB;
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
		public StringTypes StringType
		{
			get { return GetValue<StringTypes>(StringTypeProperty); }
			set { SetValue(StringTypeProperty, value); }
		}
		private static readonly IPropertyData StringTypeProperty = RegisterProperty<StringTypes>(nameof(StringType));

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
			string colorInfo = $"Light Type: {EnumValueTypeConverter.GetDescription(StringType)}\n";

			if (StringType == StringTypes.SingleColor)
			{
				colorInfo += $"Single Color:\n    Red is {SingleColorOption.R}\n    Green is {SingleColorOption.G}\n    Blue is {SingleColorOption.B}";
			}
			else if (StringType == StringTypes.MultiColor)
			{
				colorInfo += $"Multiple Colors: {SelectedColorSet}";
			}
			if (StringType == StringTypes.ColorMixingRGB)
			{
				colorInfo += $"RGB Colors: {SelectedColorSet}";
			}

			return new SummaryItem
			{
				Title = this.Title,
				Summary = colorInfo
			};
		}

		public override async Task InitializeAsync()
		{
			await base.InitializeAsync();
		}
		#endregion
	}
}
