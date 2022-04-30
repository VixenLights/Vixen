using System.Collections.Generic;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Maintains the indexes (enumerations) for a fixture function.
    /// </summary>
    public class IndexedViewModel : ItemsViewModel<IndexedItemViewModel>
	{
        #region Constructor 
		
		/// <summary>
		/// Constructor
		/// </summary>
        public IndexedViewModel()
		{					
		}

		#endregion

		#region Constants

		/// <summary>
		/// Numerical value to represent an unpopulated index end value.
		/// </summary>
		private const int UnusedIndexValue = -1;

        #endregion 

        #region Public Methods

        /// <summary>
        /// Initialize the index view model items from the model data.
        /// </summary>
        /// <param name="indexData">Fixture index model data</param>
        public void InitializeChildViewModels(List<FixtureIndex> indexData)
		{
			// Clear any existing items
			Items.Clear();

			// Loop over the model index data
			foreach (FixtureIndex fixtureIndex in indexData)
			{
				// Create the index name value pair
				IndexedItemViewModel indexNameValuePair = CreateNewItem();

				// Assign the name to the index item
				indexNameValuePair.Name = fixtureIndex.Name;

				// Assign the start value to the index item
				indexNameValuePair.StartValue = fixtureIndex.StartValue.ToString();

				// If the index has an end value then...
				if (fixtureIndex.EndValue != UnusedIndexValue)
				{
					// Assign the end value to the item
					indexNameValuePair.EndValue = fixtureIndex.EndValue.ToString();
				}

				// Assign the use curve to the item
				indexNameValuePair.UseCurve = fixtureIndex.UseCurve;

				// Assign the index type to the item
				indexNameValuePair.IndexType = fixtureIndex.IndexType;
								
				// Add the item to the item collection
				Items.Add(indexNameValuePair);
			}
		}

		/// <summary>
		/// Converts the index view model data back to model data.
		/// </summary>
		/// <returns>Index data associated with the view model</returns>
		public List<FixtureIndex> GetIndexData()
		{
			// Create a collection of model index data
			List<FixtureIndex> returnCollection = new List<FixtureIndex>();

			// Loop over the view model index items
			foreach (IndexedItemViewModel item in Items)
			{
				// Create a new model fixture index 
				FixtureIndex indexValuePair = new FixtureIndex();

				// Assign the index name
				indexValuePair.Name = item.Name;

				// Assign whether the index should be edited by a curve
				indexValuePair.UseCurve = item.UseCurve;

				// Assign the start value of the index
				indexValuePair.StartValue = int.Parse(item.StartValue);

				// If the end value has been defined then...
				if (!string.IsNullOrEmpty(item.EndValue))
				{
					// Assign the index end value
					indexValuePair.EndValue = int.Parse(item.EndValue);
				}
				else
                {
					// Otherwise set the end value to -1 to represent unused
					indexValuePair.EndValue = UnusedIndexValue;
				}
				
				// Assign the index type
				indexValuePair.IndexType = item.IndexType;
										
				// Add the index object to the model collection
				returnCollection.Add(indexValuePair);
			}

			// Return the model index collection
			return returnCollection;
		}
				
		#endregion

		#region Protected Methods

		/// <summary>
		/// For reasons unknown Catel seems to continue to validate items that have been removed from the collection.
		/// This method ensures they are valid so they don't produce errors.
		/// </summary>
		/// <param name="item">Item to make valid</param>
		protected override void MakeObjectValidBeforeDeleting(IndexedItemViewModel item)
		{
			item.Name = "Zombie";
			item.StartValue = "0";
			item.EndValue = "0";
		}

		#endregion
	}
}
