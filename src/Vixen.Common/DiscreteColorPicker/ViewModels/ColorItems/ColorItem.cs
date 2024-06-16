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
		public static readonly IPropertyData ItemColorPropertyData = RegisterProperty<Color>(nameof(ItemColor));

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
		public static readonly IPropertyData IsSelectedPropertyData = RegisterProperty<bool>(nameof(IsSelected));

		#endregion
	}
}
