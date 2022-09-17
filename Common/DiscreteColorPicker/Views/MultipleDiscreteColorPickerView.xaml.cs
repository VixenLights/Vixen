using Common.DiscreteColorPicker.ViewModels;
using System.Collections.Generic;
using System.Drawing;

namespace Common.DiscreteColorPicker.Views
{
	/// <summary>
	/// Multiple discrete color picker view window.
	/// </summary>
	public partial class MultipleDiscreteColorPickerView
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="availableColors">Available discrete colors to pick from</param>
		/// <param name="initiallySelectedColors">Initially selected colors</param>
		public MultipleDiscreteColorPickerView(HashSet<Color> availableColors, HashSet<Color> initiallySelectedColors) :
			// Call the base class constructor
			base(availableColors, initiallySelectedColors)
		{
			// Initialize the window
			InitializeComponent();

			// Configure the window size
			ConfigureWindowSize(92, 3);
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

			// Give the view model the available colors and the initially selected colors
			(ViewModel as MultipleDiscreteColorPickerViewModel).InitializeViewModel(AvailableColors, InitiallySelectedColors);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the collection of selected colors.
		/// </summary>
		/// <returns>Collection of selected colors</returns>
		public IEnumerable<Color> GetSelectedColors()
		{
			// Returns the selected colors
			return (ViewModel as MultipleDiscreteColorPickerViewModel).GetSelectedColors();
		}

		#endregion
	}
}
