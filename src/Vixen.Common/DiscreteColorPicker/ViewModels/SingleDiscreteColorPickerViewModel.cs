namespace Common.DiscreteColorPicker.ViewModels
{
	/// <summary>
	/// Maintains a view model for selecting a discrete color.
	/// </summary>
	public class SingleDiscreteColorPickerViewModel : DiscreteColorPickerViewModelBase<ColorItem>
	{
		#region Protected Methods

		/// <summary>
		/// Enable or disables the OK button.
		/// </summary>
		/// <returns>True when the OK button is enabled.</returns>
		protected override bool CanExecuteOK()
		{
			// Only enable the OK button when a color is selected
			return SelectedItem != null;	
		}

		#endregion
	}
}
