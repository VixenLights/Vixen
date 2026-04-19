using Catel.Data;
using Orc.Wizard;
using System.Collections.ObjectModel;
using Vixen.Sys.Props.Model;
using VixenApplication.SetupDisplay.ViewModels;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	/// <summary>
	/// Maintains base prop wizard page data.
	/// </summary>
	public abstract class BasePropWizardPage : WizardPageBase, IBasePropWizardPage
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		protected BasePropWizardPage()
		{
			// Initialize the Rotation collection
			ObservableCollection<AxisRotationModel> rotations = new ObservableCollection<AxisRotationModel>();
			rotations.Add(new AxisRotationModel() { Axis = Axis.XAxis, RotationAngle = 0 });
			rotations.Add(new AxisRotationModel() { Axis = Axis.YAxis, RotationAngle = 0 });
			rotations.Add(new AxisRotationModel() { Axis = Axis.ZAxis, RotationAngle = 0 });
			Rotations = AxisRotationViewModel.ConvertToViewModel(rotations);
		}

		#endregion

		#region IBasePropWizardPage

		/// <inheritdoc/>		
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		private static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));
		
		/// <inheritdoc/>
		public ObservableCollection<AxisRotationViewModel> Rotations
		{
			get { return GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty); }
			set { SetValue(RotationsProperty, value); }
		}

		private static readonly IPropertyData RotationsProperty = RegisterProperty<ObservableCollection<AxisRotationViewModel>>(nameof(Rotations));

		#endregion
	}
}
