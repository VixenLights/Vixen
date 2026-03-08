using System.ComponentModel;
using System.Windows;
using VixenApplication.SetupDisplay.Wizards.ViewModels;

namespace VixenApplication.SetupDisplay.Wizards.Views
{
	public partial class DimmingWizardPageView : INotifyPropertyChanged
	{
		public DimmingWizardPageView()
		{
			InitializeComponent();
		}

		private void AdvancedButton_Click(object sender, RoutedEventArgs e)
		{
			if (DataContext is DimmingWizardPageViewModel viewModel)
			{
				System.Windows.MessageBox.Show("Advanced option to be implemented at future date.", "Advanced Options", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
	}
}
