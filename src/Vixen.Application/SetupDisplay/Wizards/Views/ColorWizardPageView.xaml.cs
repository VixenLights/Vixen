using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
		private bool _changeLock = false;

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

		private void LightType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var control = sender as System.Windows.Controls.ComboBox;

			if (_changeLock == false && control != null && control.SelectedItem != null)
			{
				StringType = (StringTypes)control.SelectedItem;
				OnViewModelChanged();
			}
		}

		private void ColorPanel_ColorChanged(object sender, EventArgs e)
		{
			SingleColorOption = ColorPanelControl.Color;
		}

		protected override void OnViewModelChanged()
		{
			base.OnViewModelChanged();

			// Set a temporary lock so we don't get into a reentrancy condition when setting the initial values of
			// ComboBoxes
			_changeLock = true;

			// This code replaces directly setting the value in the XAML because
			// WPF sometimes struggles to match a Boxed Enum (from the ViewModel) with
			// the Enum Objects (from the MarkupExtension) if the DataContext isn't
			// fully inherited at the moment of instantiation.
			for (int index = 0; index < LightTypeComboBox.Items.Count; index++)
			{
				var item = LightTypeComboBox.Items[index];

				if (item != null && item.ToString() == StringType.ToString())
				{
					LightTypeComboBox.SelectedIndex = index;
					break;
				}
			}

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

			_changeLock = false;
		}
	}
}
