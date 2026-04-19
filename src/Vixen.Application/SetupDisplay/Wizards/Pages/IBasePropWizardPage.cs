using System.Collections.ObjectModel;
using VixenApplication.SetupDisplay.ViewModels;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	/// <summary>
	/// Maintains base prop wizard data.
	/// </summary>
	public interface IBasePropWizardPage
	{
		/// <summary>
		/// Gets or sets the Name of the prop.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Collection of axis rotations.
		/// </summary>
		ObservableCollection<AxisRotationViewModel> Rotations { get; set; }
	}
}
