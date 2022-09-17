using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Catel.MVVM;
using System.Windows.Input;
using System;

namespace Common.DiscreteColorPicker.ViewModels
{
	/// <summary>
	/// Base view model class for a discrete color picker.
	/// </summary>
	public abstract class DiscreteColorPickerViewModelBase<ColorItemType> : ViewModelBase
		where ColorItemType : ColorItem, new()	
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public DiscreteColorPickerViewModelBase()
		{
			// Create the OK button command 
			OkCommand = new Command(OK, CanExecuteOK);

			// Create the collection of color items
			Colors = new ObservableCollection<ColorItemType>();
		}

		#endregion

		#region Public Prooperties

		/// <summary>
		/// Gets or sets the collection of color items.
		/// </summary>
		public ObservableCollection<ColorItemType> Colors
		{
			get;
			set;
		}

		/// <summary>
		/// Backing field for the SelectedItem property.
		/// </summary>
		private ColorItemType _selectedColorItem;

		/// <summary>
		/// Selected color item.Y
		/// </summary>
		public ColorItemType SelectedItem
		{
			get
			{
				return _selectedColorItem;
			}
			set
			{
				// Store off the selected item
				_selectedColorItem = value;

				// If the selected item is NOT null then...
				if (value != null)
				{
					// Give the derived class an opportunity to process the selected item
					ProcessSelectedItem(_selectedColorItem);
				}

				ViewModelCommandManager.InvalidateCommands(true);
			}
		}

		/// <summary>
		/// Ok button command.
		/// </summary>
		public ICommand OkCommand { get; private set; }

		/// <summary>
		/// Cancel button command.
		/// </summary>
		public ICommand CancelCommand { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes the view model with available colors and the currently selected colors.
		/// </summary>
		/// <param name="availableColors"></param>
		/// <param name="selectedColors"></param>
		public void InitializeViewModel(
			HashSet<Color> availableColors,
			HashSet<Color> selectedColors)
		{
			//  Loop over the available colors
			foreach (Color color in availableColors)
			{
				// Create a color item to repreent the color
				ColorItemType item = new ColorItemType();
				
				// Assign the color to the color item
				item.ItemColor = color;
				
				// Add the color item to the collection
				Colors.Add(item);

				// If the color is one of the selected colors then...
				if (selectedColors.Contains(color))
				{
					// Select the color item by default
					item.IsSelected = true;

					SelectedItem = item;
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Method to invoke when the OK command is executed.
		/// </summary>
		private void OK()
		{
			// Call Catel processing
			this.SaveAndCloseViewModelAsync();
		}

		/// <summary>
		/// Method to invoke when the Cancel command is executed.
		/// </summary>
		private void Cancel()
		{
			// Call Catel processing
			this.CancelAndCloseViewModelAsync();
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Allows derived classes to process the currently selected color item.
		/// </summary>
		/// <param name="selectedItem">Currently selected color item</param>
		protected virtual void ProcessSelectedItem(ColorItemType selectedItem)
		{
			// Default is not to do anything with the selected item
		}

		/// <summary>
		/// Enable or disables the OK button.
		/// </summary>
		/// <returns>True when the OK button is enabled.</returns>
		protected virtual bool CanExecuteOK()
		{
			// By default enable the OK button
			return true;
		}

		#endregion
	}
}
