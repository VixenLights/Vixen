using System.Windows.Controls;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.ViewModels;
using VixenModules.App.Props.Models.Arch;

namespace VixenApplication.SetupDisplay.Wizards.Views
{
	/// <summary>
	/// Maintains an Arch prop Wizard page.
	/// </summary>
	public partial class ArchPropWizardPageView
	{
		private bool _changeLock = false;

		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public ArchPropWizardPageView()
		{
			// Initialize UI component
			InitializeComponent();

			// Pass the OpenTK WPF control to the base class
			OpenTkCntrl = OpenTkControl;

			// Initialize the OpenTK WPF Control
			Initialize();
		}
		#endregion

		#region Properties
		private StringTypes StringType
		{
			get
			{
				if (ViewModel is ArchPropWizardPageViewModel viewModel)
				{
					return viewModel.StringType;
				}

				return StringTypes.SingleColor;
			}
			set
			{
				if (ViewModel is ArchPropWizardPageViewModel viewModel)
				{
					viewModel.StringType = value;
				}
			}
		}

		private ArchStartLocation ArchWiringStart
		{
			get
			{
				if (ViewModel is ArchPropWizardPageViewModel viewModel)
				{
					return viewModel.ArchWiringStart;
				}

				return ArchStartLocation.Left;
			}
			set
			{
				if (ViewModel is ArchPropWizardPageViewModel viewModel)
				{
					viewModel.ArchWiringStart = value;
				}
			}
		}
		#endregion

		#region Control Events
		private void LightType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var control = sender as System.Windows.Controls.ComboBox;

			if (_changeLock == false && control != null && control.SelectedItem != null)
			{
				StringType = (StringTypes)control.SelectedItem;
			}
		}

		private void ArchWiringStart_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var control = sender as System.Windows.Controls.ComboBox;

			if (_changeLock == false && control != null && control.SelectedItem != null)
			{
				ArchWiringStart = (ArchStartLocation)control.SelectedItem;
			}
		}

		#endregion

		#region Overrides
		protected override void OnViewModelChanged()
		{
			base.OnViewModelChanged();

			if (ViewModel is ArchPropWizardPageViewModel viewModel)
			{
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

				for (int index = 0; index < ArchWiringStartComboBox.Items.Count; index++)
				{
					var item = ArchWiringStartComboBox.Items[index];

					if (item != null && item.ToString() == ArchWiringStart.ToString())
					{
						ArchWiringStartComboBox.SelectedIndex = index;
						break;
					}
				}

				_changeLock = false;
			}
		}
		#endregion
	}
}
