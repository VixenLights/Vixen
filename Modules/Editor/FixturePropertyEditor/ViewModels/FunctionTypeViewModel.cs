using Catel.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Data.Value;
using Vixen.Extensions;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Maintains and edits a collection of fixture functions.
	/// </summary>
	public class FunctionTypeViewModel : ItemsViewModel<FunctionItemViewModel>, IFixtureSaveable
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="functions">Functions associated with the fixture and the function to select initially</param>
		public FunctionTypeViewModel(Tuple<List<FixtureFunction>, string, Action> functions)
		{
			// Create the function item view models and select the specified function
			InitialSelectedFunction = InitializeChildViewModels(functions.Item1, functions.Item2);
			SelectedItem = InitialSelectedFunction;

			// Store off the delegate to refresh the command enable / disable status
			RaiseCanExecuteChanged = functions.Item3;

			// Initialize validation thread lock
			_validationLock = new object();
		}

		#endregion

		#region Fields

		/// <summary>
		/// Lock to prevent the Catel validation from producing duplicate validation messages.
		/// </summary>
		private object _validationLock; 

		#endregion

		#region Constants

		/// <summary>
		/// Postfix for the group box label.
		/// </summary>
		private const string DetailsGroupBoxPostfix = " Details";

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Controls whether the Color Wheel user control is visible.
		/// This control is only visible when the selected function is a color wheel.
		/// </summary>
		public bool ColorWheelVisible
		{
			get
			{
				return GetValue<bool>(ColorWheelVisibleProperty);
			}
			set
			{
				SetValue(ColorWheelVisibleProperty, value);
			}
		}

		/// <summary>
		/// Color Wheel Visible property data.
		/// </summary>
		public static readonly PropertyData ColorWheelVisibleProperty = RegisterProperty(nameof(ColorWheelVisible), typeof(bool), null);

		/// <summary>
		/// Controls whether the Indexed or Enumerated values user control is visible.
		/// This control is only visible when the selected function is an indexed function.
		/// </summary>
		public bool IndexedVisible
		{
			get
			{
				return GetValue<bool>(IndexedVisibleProperty);
			}
			set
			{
				SetValue(IndexedVisibleProperty, value);
			}
		}
		/// <summary>
		/// Indexed visible property data.
		/// </summary>
		public static readonly PropertyData IndexedVisibleProperty = RegisterProperty(nameof(IndexedVisible), typeof(bool), null);

		/// <summary>
		/// Controls whether the Pan/Tilt user control is visible.
		/// This control is only visible when the selected function is either the pan or tilt function.
		/// </summary>
		public bool PanTiltVisible
		{
			get
			{
				return GetValue<bool>(PanAndTiltVisibleProperty);
			}
			set
			{
				SetValue(PanAndTiltVisibleProperty, value);
			}
		}
		/// <summary>
		/// Pan tilt visible property data.
		/// </summary>
		public static readonly PropertyData PanAndTiltVisibleProperty = RegisterProperty(nameof(PanTiltVisible), typeof(bool), null);

		/// <summary>
		/// Controls whether the zoom user control is visible.
		/// This control is only visible when the selected function is a zoom function.
		/// </summary>
		public bool ZoomVisible
		{
			get
			{
				return GetValue<bool>(ZoomVisibleProperty);
			}
			set
			{
				SetValue(ZoomVisibleProperty, value);
			}
		}
		/// <summary>
		/// Zoom visible property data.
		/// </summary>
		public static readonly PropertyData ZoomVisibleProperty = RegisterProperty(nameof(ZoomVisible), typeof(bool), null);

		/// <summary>
		/// Maintains the previously selected function from the list of fixture functions.
		/// When the selected funtion is changed the previously selected function needs to be saved off.		
		/// This property makes that possible.
		/// </summary>
		public FunctionItemViewModel PreviouslySelectedItem
		{
			get
			{
				return GetValue<FunctionItemViewModel>(PrevoiuslySelectedItemProperty);
			}
			set
			{
				// If the previously selected item is not NULL then...
				if (PreviouslySelectedItem != null)
				{
					// Unregister for the function name changed event
					PreviouslySelectedItem.NameChanged -= PreviouslySelectedItem_NameChanged;
				}

				// Perform the normal Catel property processing
				SetValue(PrevoiuslySelectedItemProperty, value);

				// Register for the function name chagned event
				value.NameChanged += PreviouslySelectedItem_NameChanged;
			}
		}

		/// <summary>
		/// Selected function property data.
		/// </summary>
		public static readonly PropertyData PrevoiuslySelectedItemProperty = RegisterProperty(nameof(PreviouslySelectedItem), typeof(FunctionItemViewModel), null);

		/// <summary>
		/// Title for the detailed function group box.  The title on this group box changes as the user selects different functions.
		/// </summary>
		public string GroupBoxTitle
		{
			get
			{
				return GetValue<string>(GroupBoxTitleProperty);
			}
			set
			{
				SetValue(GroupBoxTitleProperty, value);
			}
		}

		/// <summary>
		/// Group box title property data.
		/// </summary>
		public static readonly PropertyData GroupBoxTitleProperty = RegisterProperty(nameof(GroupBoxTitle), typeof(string), null);

		#endregion

		#region Public Properties

		/// <summary>
		/// Color Wheel child view model.
		/// </summary>
		public ColorWheelViewModel ColorWheelVM
		{
			get
			{
				// Find the color wheel view model
				return FindChildViewModel<ColorWheelViewModel>();
			}
		}

		/// <summary>
		/// Indexed child view model.
		/// </summary>
		public IndexedViewModel IndexedVM
		{
			get
			{
				// Find the indexed child view model
				return FindChildViewModel<IndexedViewModel>();
			}
		}

		/// <summary>
		/// Pan tilt child view model.
		/// </summary>
		public PanTiltViewModel PanTiltVM
		{
			get
			{
				// Find the pan / tilt child view model
				return FindChildViewModel<PanTiltViewModel>();
			}
		}

		/// <summary>
		/// Zoom child view model.
		/// </summary>
		public ZoomViewModel ZoomVM
		{
			get
			{
				// Find the zoom child view model
				return FindChildViewModel<ZoomViewModel>();
			}
		}

		/// <summary>
		/// The function to initially select on view initialization.
		/// </summary>
		public FunctionItemViewModel InitialSelectedFunction
		{
			get;
			set;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates the function item view model objects based on the specified functions.  Returns the function item 
		/// view model object selected initially.
		/// </summary>
		/// <param name="functions">Function model objects to create the view model items from</param>
		/// <param name="functionToSelect">Name of the function to select initially</param>
		/// <returns>Selected function view model object selected initially</returns>
		public FunctionItemViewModel InitializeChildViewModels(List<FixtureFunction> functions, string functionToSelect)
		{
			// Initialize the selected view model object to null
			FunctionItemViewModel itemToSelect = null;

			// Loop over the fixture function model objects
			foreach (FixtureFunction functionType in functions)
			{
				// Exclude the None function from the editor
				if (functionType.FunctionType != FixtureFunctionType.None)
				{
					// Create a function view model object passing it the index and color data from the model
					FunctionItemViewModel functionVM = new FunctionItemViewModel(
						functionType.IndexData,
						functionType.ColorWheelData,
						functionType.RotationLimits,
						functionType.ZoomType == FixtureZoomType.NarrowToWide);

					// Set the function name on the view model object
					functionVM.Name = functionType.Name;

					// If the function is the function to be selected initially then...
					if (functionVM.Name == functionToSelect)
					{
						// Save off this function view model object
						itemToSelect = functionVM;
					}

					// Set the function type ( Ranged, Indexed, Color Wheel etc)
					functionVM.FunctionTypeEnum = functionVM.ConvertFixtureFunctionType(functionType.FunctionType);

					// Set the function identity so that it can be utilized by the preview
					functionVM.FunctionIdentity = functionType.FunctionIdentity;

					// Set the function legend so that it can be displayed in the preview
					functionVM.Legend = functionType.Label;

					// Set the function timeline color
					functionVM.TimelineColor = functionType.TimelineColor;

					// Add the function view model to the collection
					Items.Add(functionVM);
				}
			}

			// Return the function view model object to select initially
			return itemToSelect;
		}

		/// <summary>
		/// Converts the view model function data back into model data.
		/// </summary>
		/// <returns>Function model data</returns>
		public List<FixtureFunction> GetFunctionData()
		{
			// Create the return collection
			List<FixtureFunction> returnCollection = new List<FixtureFunction>();

			// Save the active function (retrieve data from child VM)
			SaveActiveFunction();

			// Loop over all the function view model objects
			foreach (FunctionItemViewModel item in Items)
			{
				// Create a new model function object
				FixtureFunction function = new FixtureFunction();

				// Set the function name
				function.Name = item.Name;

				// Set the index data
				function.IndexData = item.IndexData;

				// Set the color wheel data
				function.ColorWheelData = item.ColorWheelData;

				// Set the preview legend string
				function.Label = item.Legend;

				// Set the function type
				function.FunctionType = ConvertFixtureFunctionType(item.FunctionTypeEnum);

				// Set the function identity for use by the preview
				function.FunctionIdentity = item.FunctionIdentity;

				// Set the function rotation limits
				function.RotationLimits = item.RotationLimits;

				// Set the function zoom type
				function.ZoomType = item.ZoomNarrowToWide ? FixtureZoomType.NarrowToWide : FixtureZoomType.WideToNarrow;

				// Set the timeline color
				function.TimelineColor = item.TimelineColor;

				// Add the model function to the return collection
				returnCollection.Add(function);
			}

			// Attempt to find the None function
			FixtureFunction noneFunction = returnCollection.SingleOrDefault(item => item.FunctionType == FixtureFunctionType.None);

			// If the None function has been removed then...
			if (noneFunction == null)
			{
				// Add the None function back
				returnCollection.Add(
					CreateFunctionType(
						FixtureFunctionType.None.GetEnumDescription(),
							FixtureFunctionType.None,
							FunctionIdentity.Custom,
							Color.Transparent));
			}


			// Return the collection of model functions
			return returnCollection;
		}

		/// <summary>
		/// Hides all the details user controls.
		/// </summary>
		public void HideDetails()
		{
			// Hide all the details user controls
			ColorWheelVisible = false;
			IndexedVisible = false;
			PanTiltVisible = false;
			PanTiltVM.Animate.Reset();
			ZoomVisible = false;
		}
	
		/// <summary>
		/// Selects the specified function and displays the details in the details area.
		/// </summary>
		/// <param name="item"></param>
		public void SelectFunctionItem(FunctionItemViewModel item)
        {
			// Clear out initial selected item as if we made it this far we
			// have already processed the initial selected item or are processing it.
			InitialSelectedFunction = null;

			// If the previously selected function has changed then...
			if (PreviouslySelectedItem != item)
			{
				// If the previously selected function is not NULL then...
				if (PreviouslySelectedItem != null)
				{
					// Unregister for events from previous function
					PreviouslySelectedItem.FunctionTypeChanged -= PreviouslySelectedItem_FunctionTypeChanged;
				}

				// Store off the selected function
				PreviouslySelectedItem = item;

				// Register for events from the selected function							
				PreviouslySelectedItem.FunctionTypeChanged += PreviouslySelectedItem_FunctionTypeChanged;

				// Update the function details in the details area
				UpdatesFunctionDetails(item);
			}
		}
		
		/// <summary>
		/// Returns true if selected function is complete and it is safe to select a different function.
		/// </summary>
		/// <returns>True if the selected function is complete and it is safe to select a different function</returns>
		public bool AllowSelectionToChange()
        {
			// Default to allowing the selected row to change
			bool allowSelectionToChange = true;

			// If the indexed enumeration data is displayed then...
			if (IndexedVisible)
            {
				// Attempt to save the indexed data
				allowSelectionToChange = CanSaveData(IndexedVM, SaveIndexData, IndexedVisible);
			}
			//  If the color wheel data is displayed then...
			else if (ColorWheelVisible)
            {
				// Attempt to save the color wheel data
				allowSelectionToChange = CanSaveData(ColorWheelVM, SaveColorWheelData, ColorWheelVisible);
			}
			// If the pan / tilt data is displayed then...
			else if (PanTiltVisible)
            {
				// Attempt to save the pan/tilt data
				allowSelectionToChange = CanSaveData(PanTiltVM, SavePanTiltData, PanTiltVisible);
			}
			// If the zoom data is displayed then...
			else if (ZoomVisible)
			{
				// Attempt to save the zoom data
				allowSelectionToChange = CanSaveData(ZoomVM, SaveZoomData, ZoomVisible);
			}
						
			return allowSelectionToChange;
		}

		#endregion

		#region IFixtureSaveable
	
		/// <summary>
		/// Returns true if all required function data has been populated.
		/// </summary>
		/// <returns>True if all required function data has been populated</returns>
		public override bool CanSave()
		{
			// Default to being able to save
			bool canSave = true;

			// If the color wheel VM exists then...
			if (ColorWheelVM != null)
			{
				// Validate the color wheel data
				ColorWheelVM.Validate(true);
			}

			// If the indexed VM exists then...
			if (IndexedVM != null)
			{
				// Validate the index data
				IndexedVM.Validate(true);
			}

			// If the pan/tilt VM exists then...
			if (PanTiltVM != null)
			{
				// Validate the pan/tilt data
				PanTiltVM.Validate(true);
			}

			// To prevent duplicate validation messaages do not allow more than one thread 
			// in this portion of the method.
			lock (_validationLock)
			{
				// Clear out the previous save validation results
				CanSaveValidationResults = string.Empty;

				// Call the base class implementation
				canSave = base.CanSave();
											
				// If the color wheel VM exists then check to see if all color wheel data is complete
				if (ColorWheelVM != null && !ColorWheelVM.CanSave())
				{
					// Indicate the function types cannot be saved
					canSave = false;

					// Retrieve the errors from the color wheel VM
					CanSaveValidationResults += ColorWheelVM.GetValidationResults();
				}

				// If the indexed VM exists then check to see if all indexed data is complete
				if (IndexedVM != null && !IndexedVM.CanSave())
				{
					// Indicate the function types cannot be saved
					canSave = false;

					// Retrieve the errors from the indexed VM
					CanSaveValidationResults += IndexedVM.GetValidationResults();
				}

				// If the pan/tilt VM exists then check to see if all pan/tilt data is complete
				if (PanTiltVM != null && !PanTiltVM.CanSave())
				{
					// Indicate the function types cannot be saved
					canSave = false;

					// Retrieve the errors from teh pan/tilt VM
					CanSaveValidationResults += PanTiltVM.GetValidationResults();
				}
			}

			return canSave;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Converts from the view model function type to the model function type.
		/// The view model flavor does not include the None enumeration.
		/// </summary>
		/// <param name="functionTypeVM">View model function type to convert to the model enumeration</param>
		/// <returns></returns>
		private FixtureFunctionType ConvertFixtureFunctionType(FixtureFunctionTypeVM functionTypeVM)
		{
			// Default to the range function type
			FixtureFunctionType functionType = FixtureFunctionType.Range;

			switch (functionTypeVM)
			{
				case FixtureFunctionTypeVM.Range:
					functionType = FixtureFunctionType.Range;
					break;
				case FixtureFunctionTypeVM.Indexed:
					functionType = FixtureFunctionType.Indexed;
					break;
				case FixtureFunctionTypeVM.ColorWheel:
					functionType = FixtureFunctionType.ColorWheel;
					break;
				case FixtureFunctionTypeVM.RGBColor:
					functionType = FixtureFunctionType.RGBColor;
					break;
				case FixtureFunctionTypeVM.RGBWColor:
					functionType = FixtureFunctionType.RGBWColor;
					break;
				default:
					Debug.Assert(false, "Unsupported FixtureFunctionType");
					break;
			}

			return functionType;
		}


		/// <summary>
		/// Returns true if all function names are unique.
		/// </summary>
		/// <returns>True if all function names are unique</returns>
		private bool AreFunctionNamesUnique()
		{
			// Default to function names being valid
			bool valid = true;

			// Loop over all the function item VM's
			foreach (FunctionItemViewModel function in Items)
			{
				// If more than one function view model has the same name then...
				if (Items.Count(item => item.Name == function.Name) > 1)
				{
					// Indicate a duplicate function was found!
					valid = false;
				}
			}

			return valid;
		}

		/// <summary>
		/// Returns true if there is more than one of the specified function identity.
		/// </summary>
		/// <returns>True if there is more than one of the specified function identity</returns>
		private bool AreMoreThanOneFunction(FunctionIdentity functionIdentity)
		{
			// Return whether there are more than one of the specified function identity
			return Items.Count(item => item.FunctionIdentity == functionIdentity) > 1;			
		}

		/// <summary>
		/// Updates the group box title based on the function name changing.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void PreviouslySelectedItem_NameChanged(object sender, EventArgs e)
		{
			// Update the group box title based on the selected function's name changing
			GroupBoxTitle = PreviouslySelectedItem.Name + DetailsGroupBoxPostfix;
		}

		/// <summary>
		/// Finds the child view model of the specified type. 
		/// </summary>
		/// <typeparam name="T">Type of child view model to find</typeparam>
		/// <returns>Child view model of the specified type</returns>
		private T FindChildViewModel<T>() where T : class
		{
			// Initialize the view model to null
			T returnVM = null;

			// Loop over the child view models being maintained by Catel
			foreach (object vm in GetChildViewModels())
			{
				// If the child VM is of the desired type then...
				if (vm is T)
				{
					// Cast the child view model to the desired type
					returnVM = (T)vm;
				}
			}

			return returnVM;
		}

		/// <summary>
		/// Saves the active function's details.
		/// </summary>
		private void SaveActiveFunction()
		{
			// If the color wheel user control is visible then...
			if (ColorWheelVisible)
			{
				// Save off the color wheel data
				PreviouslySelectedItem.ColorWheelData = ColorWheelVM.GetColorWheelData();
			}
			// If the index user control is visible then...
			else if (IndexedVisible)
			{
				// Save off the index data
				PreviouslySelectedItem.IndexData = IndexedVM.GetIndexData();
			}
			// Otherwise if the pan/tilt user control is visble then... 
			else if (PanTiltVisible)
			{
				// Save off the pan/tilt rotation limits
				PreviouslySelectedItem.RotationLimits = PanTiltVM.GetRotationLimitsData();
			}
		}

		/// <summary>
		/// Display the color wheel data in the details area.
		/// </summary>
		/// <param name="colorWheelData">Color wheel data associated with the function</param>		
		private void DisplayColorWheel(List<FixtureColorWheel> colorWheelData)
		{
			// Give the  model data to the color wheel view model
			ColorWheelVM.InitializeChildViewModels(colorWheelData, RaiseCanExecuteChanged);
			
			// Make the color wheel user control visible
			ColorWheelVisible = true;

			// Hide the index user control
			IndexedVisible = false;

			// Hide the pan / tilt user control
			PanTiltVisible = false;
			PanTiltVM.Animate.Reset();

			// Hide the zoom user control
			PanTiltVisible = false;
			PanTiltVM.Animate.Reset();

			// Hide the zoom user control
			ZoomVisible = false;
		}

		/// <summary>
		/// Display the pan tilt rotation limits in the details area.
		/// </summary>
		/// <param name="rotationLimits">Rotation limits associated with the function</param>
		/// <param name="isPan">True when the active function is the Pan function</param>
		private void DisplayTiltPan(FixtureRotationLimits rotationLimits, Action raiseCanExecuteChanged, bool isPan)
		{
			// VIX-3248
			// This null check exists because during testing it seemed like after
			// creating new build (branch) of the source code that Catel child view model
			// collection was empty.  This defensive coding avoids a crash but
			// will require the user to select or re-select the function they want to edit.
			if (PanTiltVM != null)
			{
				// Give the rotation limits model data to the pan tilt view model
				PanTiltVM.InitializeViewModel(rotationLimits, RaiseCanExecuteChanged, isPan);

				// Make the pan tilt user control visible
				PanTiltVisible = true;
			}

			// Hide the color wheel user control visible
			ColorWheelVisible = false;

			// Hide the index user control
			IndexedVisible = false;

			// Hide the zoom user control
			ZoomVisible = false;
		}

		/// <summary>
		/// Display the zoom data in the details area.
		/// </summary>
		/// <param name="narrowToWide">Whether the zoom expands from narrow to wide</param>		
		private void DisplayZoom(bool narrowToWide, Action raiseCanExecuteChanged)
		{
			// Give the rotation limits model data to the pan tilt view model
			ZoomVM.InitializeViewModel(narrowToWide, RaiseCanExecuteChanged);

			// Make the zoom user control visible
			ZoomVisible = true;

			// Hide the pan tilt user control visible
			PanTiltVisible = false;
			PanTiltVM.Animate.Reset();

			// Hide the color wheel user control visible
			ColorWheelVisible = false;

			// Hide the index user control
			IndexedVisible = false;			
		}

		/// <summary>
		/// Display the index enumerations in the details area.
		/// </summary>
		/// <param name="indexData">Index data associated with the function</param>
		/// <param name="displayImage">Determine if the image columns are displayed</param>
		private void DisplayIndex(List<FixtureIndex> indexData, bool displayImage)
		{
			// Give the index model data to the indexed view model
			IndexedVM.InitializeChildViewModels(indexData, displayImage, RaiseCanExecuteChanged);
			
			// Make the index user control visible
			IndexedVisible = true;

			// Hide the color wheel control
			ColorWheelVisible = false;

			// Hide the pan / tilt user control
			PanTiltVisible = false;
			PanTiltVM.Animate.Reset();

			// Hide the zoom user control
			ZoomVisible = false;
		}

		/// <summary>
		/// Displays the specified function in the details area.
		/// </summary>
		/// <param name="selectedItem">Selected function to display</param>
		private void UpdatesFunctionDetails(FunctionItemViewModel selectedItem)
		{
			// Determine which details user control to display
			switch (ConvertFixtureFunctionType(PreviouslySelectedItem.FunctionTypeEnum))
			{
				case FixtureFunctionType.Indexed:
					// Display the index user control
					DisplayIndex(
						PreviouslySelectedItem.IndexData, 
						PreviouslySelectedItem.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.Gobo); // Display Image Column for Gobo functions
					break;
				case FixtureFunctionType.ColorWheel:
					// Display the color wheel user control
					DisplayColorWheel(PreviouslySelectedItem.ColorWheelData);
					break;
				case FixtureFunctionType.Range:

					// If the function is the Pan or Tilt then...
					if (PreviouslySelectedItem.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.Pan ||
						PreviouslySelectedItem.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.Tilt)
					{
						// Display the pan/tilt rotation limits
						DisplayTiltPan(PreviouslySelectedItem.RotationLimits, RaiseCanExecuteChanged, PreviouslySelectedItem.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.Pan);
					}
					// If the function is Zoom then...
					else if (PreviouslySelectedItem.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.Zoom)
					{
						// Display the zoom data
						DisplayZoom(PreviouslySelectedItem.ZoomNarrowToWide, RaiseCanExecuteChanged);
					}
					else
					{
						// Hide all the user controls
						HideDetails();
					}
					break;
				default:
					// Hide all the user controls
					HideDetails();
					break;
			}
		}

		/// <summary>
		/// Event handler that updates the details user control when the function type is changed.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void PreviouslySelectedItem_FunctionTypeChanged(object sender, EventArgs e)
		{
			// Update the details area with the selected function
			UpdatesFunctionDetails(PreviouslySelectedItem);		
		}

		/// <summary>
		/// Attempts to save (retrieve) the current function data if it is valid and complete.
		/// </summary>
		/// <param name="control">User control to check</param>
		/// <param name="SaveData">Method to commit the function data</param>
		/// <param name="associatedDataIsDisplayed">Flag indicating if the data for the associated user control is displayed</param>
		/// <returns></returns>
		private bool CanSaveData(IFixtureSaveable childVM, Action saveData, bool associatedDataIsDisplayed)
		{
			// Default to allowing the selected row to change
			bool allowSelectionToChange = true;

			// If the data associated with the control is being displayed in the details area then...
			if (PreviouslySelectedItem != null && associatedDataIsDisplayed)
			{
				// Only allow the selection to change if the control's data is complete 
				allowSelectionToChange = childVM.CanSave();

				// If the selection is changing then...
				if (allowSelectionToChange)
				{
					// Save off the view model data to the model
					saveData();
				}
			}

			// Returns whether the data was valid and complete
			return allowSelectionToChange;
		}

		/// <summary>
		/// Saves the current function index data.
		/// </summary>
		private void SaveIndexData()
		{
			// Retrieve the index data from the user control
			PreviouslySelectedItem.IndexData = IndexedVM.GetIndexData();
		}

		/// <summary>
		/// Saves the current color wheel data.
		/// </summary>
		private void SaveColorWheelData()
		{
			// Retrieve the color wheel data from the user control
			PreviouslySelectedItem.ColorWheelData = ColorWheelVM.GetColorWheelData();
		}

		/// <summary>
		/// Saves the current pan/tilt rotation limit data.
		/// </summary>
		private void SavePanTiltData()
		{
			// Retrieve the rotation limit from the user control
			PreviouslySelectedItem.RotationLimits = PanTiltVM.GetRotationLimitsData();
		}

		/// <summary>
		/// Saves the current zoom data.
		/// </summary>
		private void SaveZoomData()
		{
			// Retrieve the zoom data from the user control
			PreviouslySelectedItem.ZoomNarrowToWide = ZoomVM.NarrowToWide;
		}

		/// <summary>
		/// Create a fixture function.
		/// </summary>
		/// <param name="name">Name of the function</param>
		/// <param name="functionType">Type of the function</param>
		/// <param name="identity">Preview identity of the function</param>		
		/// <param name="timelineColor">Color to use on the timeline for some effects</param>
		/// <returns>New fixture function</returns>
		private FixtureFunction CreateFunctionType(
			string name,
			FixtureFunctionType functionType,
			FunctionIdentity identity,
			Color timelineColor)
		{
			// Create the new function
			FixtureFunction function = new FixtureFunction();

			// Configure the name on the function
			function.Name = name;

			// Configure the function type
			function.FunctionType = functionType;

			// Configure the function identity
			function.FunctionIdentity = identity;

			// Configure the timeline color
			function.TimelineColor = timelineColor;

			return function;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Selects the specified function view model item.
		/// </summary>
		/// <param name="item">Function to select</param>
		protected override void OnSelectItem(FunctionItemViewModel item)
		{
			// If the function doesn't have a name yet then...
			if (string.IsNullOrEmpty(item.Name))
			{
				// Update the details group box 
				GroupBoxTitle = "<Function>" + DetailsGroupBoxPostfix;
			}
			// Otherwise update the group box label with the function name
			else
			{
				// Update the group box label with the function name
				GroupBoxTitle = item.Name + DetailsGroupBoxPostfix;
			}
		}

		/// <summary>
		/// For reasons unknown Catel seems to continue to validate items that have been removed from the collection.
		/// This method ensures they are valid so they don't produce zombie errors.
		/// </summary>
		/// <param name="item">Item to make valid</param>
		protected override void MakeObjectValidBeforeDeleting(FunctionItemViewModel item)
		{
			item.Name = "Zombie";			
			item.FunctionTypeEnum = FixtureFunctionTypeVM.Range;						
			item.CloseViewModelAsync(null);
		}

		/// <summary>
		/// Validates the business rules of the view model.
		/// </summary>
		/// <param name="validationResults">Results of the validation</param>
		protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
		{			
			// If the function names are NOT unique then...
			if (!AreFunctionNamesUnique())
			{
				// Add an error that there are duplicate function names
				validationResults.Add(BusinessRuleValidationResult.CreateError("Cannot have duplicate function names."));				
			}

			// If there is more than one Pan function then...
			if (AreMoreThanOneFunction(FunctionIdentity.Pan))
			{
				// Add an error that there are duplicate Pan functions
				validationResults.Add(BusinessRuleValidationResult.CreateError("Cannot have more than one Pan function."));
			}

			// If there is more than one Tilt function then...
			if (AreMoreThanOneFunction(FunctionIdentity.Tilt))
			{
				// Add an error that there are duplicate Tilt functions
				validationResults.Add(BusinessRuleValidationResult.CreateError("Cannot have more than one Tilt function."));
			}

			// If there is more than one Zoom function then...
			if (AreMoreThanOneFunction(FunctionIdentity.Zoom))
			{
				// Add an error that there are duplicate Zoom functions
				validationResults.Add(BusinessRuleValidationResult.CreateError("Cannot have more than one Zoom function."));
			}

			// Display the validation bar
			DisplayValidationBar(validationResults);
		}

		#endregion
	}
}
