using System.Collections.Generic;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Maintains the color wheel data for a fixture function.
    /// </summary>
    public class ColorWheelViewModel : ItemsViewModel<ColorWheelItemViewModel>, IFixtureSaveable
	{
        #region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
        public ColorWheelViewModel()
		{
		}

		#endregion		

		#region Public Methods
				
		/// <summary>
		/// Creates the color wheel item view model items from the model data.
		/// </summary>
		/// <param name="colorWheelData">Color wheel item data</param>
		public void InitializeChildViewModels(List<FixtureColorWheel> colorWheelData)
		{
			// Clear any existing color wheel items
			Items.Clear();

			// Loop over the color wheel model items
			foreach (FixtureColorWheel colorWheel in colorWheelData)
			{
				// Create a color wheel view model item
				ColorWheelItemViewModel colorSlot = CreateNewItem();

				// Assign the color wheel item name
				colorSlot.Name = colorWheel.Name;

				// Assign the start value
				colorSlot.StartValue = colorWheel.StartValue.ToString();
								
				// Assign whether the entry is a half step
				colorSlot.HalfStep = colorWheel.HalfStep;
			
				// Assign the color wheel item colors
				colorSlot.Color1 = colorWheel.Color1;
				colorSlot.Color2 = colorWheel.Color2;		
				
				// Add the color wheel view model item to the collection
				Items.Add(colorSlot);
			}
		}		

		/// <summary>
		/// Converts the color wheel data from view model to model.
		/// </summary>
		/// <returns>Color wheel model data</returns>
		public List<FixtureColorWheel> GetColorWheelData()
		{
			// Create a collection of color wheel model data
			List<FixtureColorWheel> returnCollection = new List<FixtureColorWheel>();
			
			// Loop over the color wheel view model items
			foreach (ColorWheelItemViewModel item in Items)
			{
				// Create a color wheel model object
				FixtureColorWheel colorSlot = new FixtureColorWheel();

				// Assign the name to the color wheel item
				colorSlot.Name = item.Name;

				// Assign the start value 
				colorSlot.StartValue = int.Parse(item.StartValue);
				
				// Assign whether this entry is a half step
				colorSlot.HalfStep = item.HalfStep;
				
				// Assign the item's colors
				colorSlot.Color1 = item.Color1;
				colorSlot.Color2 = item.Color2;

				// Add the color wheel model object to the collection
				returnCollection.Add(colorSlot);
			}

			// Return the collection of color wheel model objects
			return returnCollection;
		}

        #endregion

        #region Protected Methods

		/// <summary>
		/// <inheritdoc/> Refer to base class documentation.
		/// </summary>
        protected override ColorWheelItemViewModel CreateNewItem()
		{
			// Create a color wheel view model item
			ColorWheelItemViewModel item = base.CreateNewItem();

			// Assign the item a delegate to update half steps with the colors of the items before and after this item
			item.UpdateHalfSteps = UpdateHalfSteps;

			// Assign the item a delegate to refresh command status (OK Button)
			item.RaiseCanExecuteChanged = RaiseCanExecuteChanged;

			// Return the color wheel item
			return item;
		}

		/// <summary>
		/// For reasons unknown Catel seems to continue to validate items that have been removed from the collection.
		/// This method ensures they are valid so they don't produce errors.
		/// </summary>
		/// <param name="item">Item to make valid</param>
		protected override void MakeObjectValidBeforeDeleting(ColorWheelItemViewModel item)
		{
			item.Name = "Delete Me";
			item.StartValue = "0";
	  	}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the half step colors based on the colors before and after the half step.
		/// </summary>
		private void UpdateHalfSteps()
        {
			// Loop over the color wheel view model items
			for(int index = 0; index < Items.Count; index++)
            {
				// If the item is a half step then...
				if (Items[index].HalfStep)
                {
					// If there is an item before this item then...
					if (index > 0)
                    {
						// Assign color 1 to the color of the previous item
						Items[index].Color1 = Items[index - 1].Color1;												
					}

					// If there is an item after this item then...
					if (index + 1 < Items.Count)
					{
						// Assign color 2 to the color of the next item
						Items[index].Color2 = Items[index + 1].Color1;
					}
				}
            }
        }

        #endregion		
    }
}
