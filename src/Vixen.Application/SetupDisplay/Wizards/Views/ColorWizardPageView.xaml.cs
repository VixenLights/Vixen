using Catel.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Vixen.Services;
using VixenApplication.SetupDisplay.Wizards.ViewModels;
using VixenModules.App.Props.Models;
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

		private ColorType ColorTypeOption
		{
			get
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					return viewModel.ColorTypeOption;
				}

				return ColorType.SingleColor;
			}
			set
			{
				if (ViewModel is ColorWizardPageViewModel viewModel)
				{
					viewModel.ColorTypeOption = value;
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
			ColorTypeOption = ColorType.SingleColor;
			OnViewModelChanged();
		}

		private void MultipleColorButtonClick(object sender, RoutedEventArgs e)
		{
			SingleColor.Visibility = Visibility.Collapsed;
			MultipleColor.Visibility = Visibility.Visible;
			RGBColor.Visibility = Visibility.Collapsed;
			ColorTypeOption = ColorType.MultipleColors;
			OnViewModelChanged();
		}

		private void RGBColorButtonClick(object sender, RoutedEventArgs e)
		{
			SingleColor.Visibility = Visibility.Collapsed;
			MultipleColor.Visibility = Visibility.Collapsed;
			RGBColor.Visibility = Visibility.Visible;
			ColorTypeOption = ColorType.RGBColors;
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

			if (ColorTypeOption == ColorType.SingleColor)
			{
				SingleColor.Visibility = Visibility.Visible;
				SingleColorButton.IsChecked = true;
				MultipleColor.Visibility = Visibility.Collapsed;
				RGBColor.Visibility = Visibility.Collapsed;

				if (ColorPanelControl.Color != SingleColorOption)
				{
					ColorPanelControl.Color = SingleColorOption;
				}
			}

			else if (ColorTypeOption == ColorType.MultipleColors)
			{
				SingleColor.Visibility = Visibility.Collapsed;
				MultipleColor.Visibility = Visibility.Visible;
				MultipleColorButton.IsChecked = true;
				RGBColor.Visibility = Visibility.Collapsed;
			}

			else if (ColorTypeOption == ColorType.RGBColors)
			{
				SingleColor.Visibility = Visibility.Collapsed;
				MultipleColor.Visibility = Visibility.Collapsed;
				RGBColor.Visibility = Visibility.Visible;
				RGBColorButton.IsChecked = true;
			}
		}
	}
}
