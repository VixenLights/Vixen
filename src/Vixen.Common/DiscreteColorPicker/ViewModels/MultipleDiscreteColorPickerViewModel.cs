using System.Drawing;

namespace Common.DiscreteColorPicker.ViewModels
{
	/// <summary>
	/// Maintains a view model for selecting multiple discrete colors.
	/// </summary>
	public class MultipleDiscreteColorPickerViewModel : DiscreteColorPickerViewModelBase<MultiSelectColorItem>
	{
		#region Protected Methods

		/// <Inheritdoc/>
		protected override void ProcessSelectedItem(MultiSelectColorItem selectedItem)
		{
			// Select check box associated with the color item
			selectedItem.CheckBoxSelected = true;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the collection of selected colors.
		/// </summary>
		/// <returns>Collection of selected colors</returns>
		public IEnumerable<Color> GetSelectedColors()
		{
			// Return the collection of selected colors
			return Colors.Where(item => item.CheckBoxSelected).Select(item => item.ItemColor).ToList();
		}

		#endregion
	}
}
