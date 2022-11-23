using Catel.Data;
using Catel.MVVM;
using System.Drawing;

namespace Common.DiscreteColorPicker.ViewModels
{
	/// <summary>
	/// Maintains a color item.
	/// </summary>
	public class ColorItem : ViewModelBase
	{
		#region Public Catel Properties

		/// <summary>
		/// Gets or sets the Color associated with the item.
		/// </summary>
		public Color ItemColor
		{
			get { return GetValue<Color>(ItemColorPropertyData); }
			set { SetValue(ItemColorPropertyData, value); }
		}

		/// <summary>
		/// Item Color property data.
		/// </summary>
		public static readonly PropertyData ItemColorPropertyData = RegisterProperty(nameof(ItemColor), typeof(Color));

		/// <summary>
		/// True when the Item is selected in the ListBox.
		/// </summary>
		public bool IsSelected
		{
			get { return GetValue<bool>(IsSelectedPropertyData); }
			set { SetValue(IsSelectedPropertyData, value); }
		}

		/// <summary>
		/// IsSelected property data.
		/// </summary>
		public static readonly PropertyData IsSelectedPropertyData = RegisterProperty(nameof(IsSelected), typeof(bool));

		#endregion
	}
}
