using Common.DiscreteColorPicker.ViewModels;
using System.Collections.Generic;
using System.Drawing;

namespace Common.DiscreteColorPicker.Views
{
	/// <summary>
	/// Single discrete color picker view window.
	/// </summary>
	public partial class SingleDiscreteColorPickerView 
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="availableColors">Available discrete colors to pick from</param>
		/// <param name="initiallySelectedColor">Initially selected color</param>
		public SingleDiscreteColorPickerView(HashSet<Color> availableColors, Color initiallySelectedColor) :
			// Call base class constructor
			base(availableColors, new HashSet<Color>() { initiallySelectedColor })
		{
			// Initialize the window
			InitializeComponent();

			// Configure the window size
			ConfigureWindowSize(72, 4); 
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Initializes the associated view model.
		/// </summary>
		protected override void Initialize()
		{
			// Call the base class implementation
			base.Initialize();

			// Give the view model the available fixture specifications
			(ViewModel as SingleDiscreteColorPickerViewModel).InitializeViewModel(AvailableColors, InitiallySelectedColors);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the selected color.
		/// </summary>
		/// <returns>Selected color</returns>
		public Color GetSelectedColor()
		{
			// Returns the selected color
			return (ViewModel as SingleDiscreteColorPickerViewModel).SelectedItem.ItemColor;
		}

		#endregion
	}
}
