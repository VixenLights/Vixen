using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Vixen.Services;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.ViewModels;
using VixenModules.Property.Color;


namespace VixenApplication.SetupDisplay.Wizards.Views
{
	/// <summary>
	/// Interaction logic for ColorWizardPageView.xaml
	/// </summary>
	public partial class ColorWizardPageView : INotifyPropertyChanged
	{
		public ColorWizardPageView()
		{
			InitializeComponent();
		}

		private StringTypes StringType
		{
			get
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					return viewModel.StringType;
				}

				return StringTypes.MultiColor;
			}
			set
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					viewModel.StringType = value;
				}
			}
		}

		private ObservableCollection<string> ColorSetNames
		{
			get
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					return viewModel.ColorSetNames;
				}

				return null;
			}
			set
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					viewModel.ColorSetNames = value;
				}
			}
		}

		private string SelectedColorSet
		{
			get
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					return viewModel.SelectedColorSet;
				}

				return null;
			}
			set
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					viewModel.SelectedColorSet = value;
				}
			}
		}

		private Color SingleColorOption
		{
			get
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					return viewModel.SingleColorOption;
				}

				return Color.White;
			}
			set
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					viewModel.SingleColorOption = value;
				}
			}
		}

		private void SingleColorButtonClick(object sender, RoutedEventArgs e)
		{
			SingleColor.Visibility = Visibility.Visible;
			MultipleColor.Visibility = Visibility.Collapsed;
			RGBColor.Visibility = Visibility.Collapsed;
			OnViewModelChanged();
		}

		private void MultipleColorButtonClick(object sender, RoutedEventArgs e)
		{
			SingleColor.Visibility = Visibility.Collapsed;
			MultipleColor.Visibility = Visibility.Visible;
			RGBColor.Visibility = Visibility.Collapsed;
			OnViewModelChanged();
		}

		private void RGBColorButtonClick(object sender, RoutedEventArgs e)
		{
			SingleColor.Visibility = Visibility.Collapsed;
			MultipleColor.Visibility = Visibility.Collapsed;
			RGBColor.Visibility = Visibility.Visible;
			OnViewModelChanged();
		}

		private void EditColorsButtonClick(object sender, RoutedEventArgs e)
		{
			using (ColorSetsSetupForm cssf = new ColorSetsSetupForm(ApplicationServices.GetModuleStaticData(ColorDescriptor.ModuleId) as ColorStaticData))
			{
				if (cssf.ShowDialog() == DialogResult.OK)
				{
					var staticData = ApplicationServices.GetModuleStaticData(ColorDescriptor.ModuleId) as ColorStaticData;

					if (staticData != null)
					{
						ColorSetNames = new ObservableCollection<string>(staticData.GetColorSetNames());
						SelectedColorSet = ColorSetNames[0];
					}
				}
			}
		}

		private void ColorPanel_ColorChanged(object sender, EventArgs e)
		{
			SingleColorOption = ColorPanelControl.Color;
		}

		protected override void OnViewModelChanged()
		{
			base.OnViewModelChanged();


			if (StringType == StringTypes.SingleColor)
			{
				SingleColor.Visibility = Visibility.Visible;
				MultipleColor.Visibility = Visibility.Collapsed;
				RGBColor.Visibility = Visibility.Collapsed;

				if (ColorPanelControl.Color != SingleColorOption)
				{
					ColorPanelControl.Color = SingleColorOption;
				}
			}

			else if (StringType == StringTypes.MultiColor)
			{
				SingleColor.Visibility = Visibility.Collapsed;
				MultipleColor.Visibility = Visibility.Visible;
				RGBColor.Visibility = Visibility.Collapsed;
			}

			else if (StringType == StringTypes.ColorMixingRGB)
			{
				SingleColor.Visibility = Visibility.Collapsed;
				MultipleColor.Visibility = Visibility.Collapsed;
				RGBColor.Visibility = Visibility.Visible;
			}
		}
	}
}
