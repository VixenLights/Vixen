using Catel.Data;

namespace Common.DiscreteColorPicker.ViewModels
{
	/// <summary>
	/// Maintains a multiple selection color item.
	/// </summary>
	public class MultiSelectColorItem : ColorItem
	{
		#region Public Catel Properties

		/// <summary>
		/// Gets or sets the state of the check box associated with the color item.
		/// (True=Checked, False=Not Checked)
		/// </summary>
		public bool CheckBoxSelected
		{
			get { return GetValue<bool>(CheckBoxSelectedPropertyData); }
			set { SetValue(CheckBoxSelectedPropertyData, value); }
		}

		/// <summary>
		/// Check box selected property data.
		/// </summary>
		public static readonly PropertyData CheckBoxSelectedPropertyData = RegisterProperty(nameof(CheckBoxSelected), typeof(bool));

		#endregion
	}
}
