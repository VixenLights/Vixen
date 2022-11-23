using Catel.Data;
using Catel.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using VixenModules.App.Fixture;
using VixenModules.App.FixtureSpecificationManager;

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
			// Create the import Gobo images command
			ImportGoboImagesCommand = new Command(ImportGoboImages);
		}

		#endregion

		#region Private Constants

		/// <summary>
		/// Numerical value to represent an unpopulated index end value.
		/// </summary>
		private const int UnusedIndexValue = -1;

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Determines whether the image column is displayed.
		/// </summary>
		public bool DisplayImage
		{
			get { return GetValue<bool>(DisplayImageProperty); }
			set
			{
				SetValue(DisplayImageProperty, value);
			}
		}

		/// <summary>
		/// Display image property data.
		/// </summary>
		public static readonly PropertyData DisplayImageProperty = RegisterProperty(nameof(DisplayImage), typeof(bool), null);

		/// <summary>
		/// Determines whether the tag column is displayed.
		/// </summary>
		public bool DisplayTag
		{
			get { return GetValue<bool>(DisplayTagProperty); }
			set
			{
				SetValue(DisplayTagProperty, value);
			}
		}

		/// <summary>
		/// Display tag property data.
		/// </summary>
		public static readonly PropertyData DisplayTagProperty = RegisterProperty(nameof(DisplayTag), typeof(bool), null);

		#endregion

		#region Public Methods

		/// <summary>
		/// Initialize the index view model items from the model data.
		/// </summary>
		/// <param name="indexData">Fixture index model data</param>
		/// <param name="displayImage">Flag determinees if the image columns are displayed</param>
		/// <param name="raiseCanExecuteChanged">Delegate to refresh the command status</param>
		public void InitializeChildViewModels(List<FixtureIndex> indexData, bool displayImage, Action raiseCanExecuteChanged)
		{
			// Store of the delegate to refresh the command status
			RaiseCanExecuteChanged = raiseCanExecuteChanged;

			// Store off if the image column should be displayed
			DisplayImage = displayImage;

			// Store off if the tag column should be displayed
			DisplayTag = !displayImage;

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

				// Assign the image associated with the item
				indexNameValuePair.Image = fixtureIndex.Image;

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

				// Assign the image associated with the index
				indexValuePair.Image = item.Image;

				// Add the index object to the model collection
				returnCollection.Add(indexValuePair);
			}

			// Return the model index collection
			return returnCollection;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Command for importing gobo images.
		/// </summary>
		public ICommand ImportGoboImagesCommand { get; private set; }

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
			item.CloseViewModelAsync(null);
		}
		
		/// <summary>
		/// Validates the business rules of the view model.
		/// </summary>
		/// <param name="validationResults">Results of the validation</param>
		protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
		{
			// Default to NOT overlapping ranges
			bool overlaps = false;

			// Loop over all the index ranges
			foreach (IndexedItemViewModel itemLeft in Items)
			{
				// Check to see if the current range overlaps with any other range
				overlaps |= Items.Any(itemRight => itemLeft != itemRight && Overlaps(itemLeft, itemRight));
			}

			// If any ranges overlap then...
			if (overlaps)
			{
				// Add an error message to the validation results
				validationResults.Add(BusinessRuleValidationResult.CreateError("Index ranges cannot overlap."));
			}

			// If the index item names are NOT unique then...
			if (!AreIndexNamesUnique())
			{
				// Add an error that there are duplicate index names
				validationResults.Add(BusinessRuleValidationResult.CreateError("Cannot have duplicate index names."));
			}

			// Display the validation bar
			DisplayValidationBar(validationResults);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Imports (copies) png image files into the fixture image directory.
		/// </summary>
		private void ImportGoboImages()
		{
			// Create the Win32 OpenFileDialog
			OpenFileDialog openFileDialog = new OpenFileDialog();

			// Configure the Open Dialog to look for png files
			openFileDialog.Filter = "Image files (*.png)|*.png";

			// Enable multi-select
			openFileDialog.Multiselect = true;

			// Display the file selection dialog.
			// If the user selects OK button then...
			if (openFileDialog.ShowDialog() == true)
			{
				// Loop over the selected file names
				foreach (string fileName in openFileDialog.FileNames)
				{
					try
					{
						// Copy the files to the fixture image directory
						File.Copy(fileName, Path.Combine(FixtureSpecificationManager.Instance().GetGoboImageDirectory(), Path.GetFileName(fileName)), true);
					}
					catch (Exception)
					{
						// Ignore any exceptions copying the files
					}
				}
			}

			// Loop over the index items
			foreach (IndexedItemViewModel item in Items)
			{
				// Refresh the available images associated with the items
				item.UpdateAvailableImages();
			}
		}

		/// <summary>
		/// Returns true if the specified index ranges overlap.
		/// </summary>
		/// <param name="left">Left range to analyze</param>
		/// <param name="right">Right range to analyze</param>
		/// <returns></returns>
		private bool Overlaps(IndexedItemViewModel left, IndexedItemViewModel right)
		{
			// Default to NOT overlapped
			bool overlaps = false;

			// Define integers for the range end points
			int leftStart = -1;
			int leftEnd = -1;
			int rightStart = -1;
			int rightEnd = -1;

			// If the left start value is populated then...
			if (!string.IsNullOrEmpty(left.StartValue))
			{
				// Convert the value to an integer
				leftStart = int.Parse(left.StartValue);
			}

			// If the left end value is populated then...
			if (!string.IsNullOrEmpty(left.EndValue))
			{
				// Convert the value to an integer
				leftEnd = int.Parse(left.EndValue);
			}

			// If the right start value is populated then...
			if (!string.IsNullOrEmpty(right.StartValue))
			{
				// Convert the value to an integer
				rightStart = int.Parse(right.StartValue);
			}

			// If the right end value is populated then...
			if (!string.IsNullOrEmpty(right.EndValue))
			{
				// Convert the value to an integer
				rightEnd = int.Parse(right.EndValue);
			}

			// If both ranges are fully defined then...
			if (leftStart != -1 &&
				leftEnd != -1 &&
				rightStart != -1 &&
				rightEnd != -1)
			{
				// Check for overlap between the ranges
				overlaps = (leftStart <= rightEnd &&
						   leftEnd >= rightStart);
			}
			// If the right range is a single value then...
			else if (leftStart != -1 &&
					 leftEnd != -1 &&
					 rightStart != -1)
			{
				// Check to see if the right point is inside the left range
				overlaps = (rightStart >= leftStart && rightStart <= leftEnd);
			}
			// If the left range is a single value then...
			else if (leftStart != -1 &&
					rightStart != -1 &&
					rightEnd != -1)
			{
				// Check to see if the left point is inside the right range
				overlaps = (leftStart >= rightStart && leftStart <= rightEnd);
			}
			// If both the left and right ranges are a single value then...
			else if (leftStart != -1 &&
					 rightStart != -1)
			{
				// Check to see if the values are identical
				overlaps = (leftStart == rightStart);
			}

			// Return whether the ranges overlap
			return overlaps;
		}

		/// <summary>
		/// Returns true if all index names are unique.
		/// </summary>
		/// <returns>True if all index names are unique</returns>
		private bool AreIndexNamesUnique()
		{
			// Default to index names being valid
			bool valid = true;

			// Loop over all the index item VM's
			foreach (IndexedItemViewModel index in Items)
			{
				// If more than one index item view model has the same name then...
				if (Items.Count(item => item.Name == index.Name) > 1)
				{
					// Indicate a duplicate index was found!
					valid = false;
				}
			}

			return valid;
		}

		#endregion
	}
}
