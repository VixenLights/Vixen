using Catel.Data;
using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Vixen.Extensions;
using VixenModules.App.Fixture;
using VixenModules.App.FixtureSpecificationManager;
using VixenModules.Editor.FixturePropertyEditor.Views;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// View model for a fixture specification.
    /// </summary>
    public class FixturePropertyEditorViewModel : ItemsViewModel<ChannelItemViewModel>, IFixtureSaveable
	{
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="fixtureSpecification">Fixture specification being edited</param>
        public FixturePropertyEditorViewModel(Tuple<FixtureSpecification, Action, bool> fixtureTuple)
		{			
			// Create the collection of function names
			Functions = new ObservableCollection<string>();

			// Create button commands
			EditFunctionsCommand = new Command(EditFunctions);
			LoadSpecificationCommand = new Command(LoadSpecification);			

			// Create the collection of allowable channel numbers
			_channelNumbers = new ObservableCollection<string>();

			// Save off the action that refreshes commands
			RaiseCanExecuteChanged = fixtureTuple.Item2;

			// Store off whether to display the profile properties
			// Profile properties are not displayed when the view is used inside the wizard
			ShowProfileProperties = fixtureTuple.Item3;

			// Initialize the channel view models	
			InitializeChildViewModels(fixtureTuple.Item1);

			// Force Catel to validate
			Validate(true);
		}

		#endregion

		#region Public Properties
		
		/// <summary>
		/// Command for editing the functions.
		/// </summary>
		public ICommand EditFunctionsCommand { get; set; }

		/// <summary>
		/// Command for loading an existing fixture specification.
		/// </summary>
		public ICommand LoadSpecificationCommand { get; set; }
		
		/// <summary>
		/// Colletion of the function names defined for the fixture.
		/// </summary>
		public ObservableCollection<string> Functions
		{
			get
			{
				return GetValue<ObservableCollection<string>>(FunctionsProperty);
			}
			set
			{
				SetValue(FunctionsProperty, value);
			}
		}

		/// <summary>
		/// Function names property data.
		/// </summary>
		public static readonly PropertyData FunctionsProperty = RegisterProperty(nameof(Functions), typeof(ObservableCollection<string>), null);

		/// <summary>
		/// Name of the fixture.
		/// </summary>
		public string Name
		{
			get 
			{ 
				return GetValue<string>(NameProperty);				
			}
			set 
			{ 
				SetValue(NameProperty, value);
				if (RaiseCanExecuteChanged != null)
				{
					RaiseCanExecuteChanged();
				}
			}
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly PropertyData NameProperty = RegisterProperty(nameof(Name), typeof(string), null);

		/// <summary>
		/// Manufacturer of the fixture.
		/// </summary>
		public string Manufacturer
		{
			get
			{
				return GetValue<string>(ManufacturerProperty);
			}
			set
			{
				SetValue(ManufacturerProperty, value);		
			}
		}

		/// <summary>
		/// Manufacturer property data.
		/// </summary>
		public static readonly PropertyData ManufacturerProperty = RegisterProperty(nameof(Manufacturer), typeof(string), null);

		/// <summary>
		/// Name of the user that created the fixture profile.
		/// </summary>
		public string CreatedBy
		{
			get
			{
				return GetValue<string>(CreatedByProperty);
			}
			set
			{
				SetValue(CreatedByProperty, value);
			}
		}

		/// <summary>
		/// Created By property data.
		/// </summary>
		public static readonly PropertyData CreatedByProperty = RegisterProperty(nameof(CreatedBy), typeof(string), null);

		/// <summary>
		/// Revision information about the fixture profile.
		/// </summary>
		public string Revision
		{
			get
			{
				return GetValue<string>(RevisionProperty);
			}
			set
			{
				SetValue(RevisionProperty, value);
			}
		}

		/// <summary>
		/// Revision property data.
		/// </summary>
		public static readonly PropertyData RevisionProperty = RegisterProperty(nameof(Revision), typeof(string), null);

		/// <summary>
		/// Determines if the profile properties are displayed.
		/// </summary>
		public bool ShowProfileProperties
		{
			get
			{
				return GetValue<bool>(ShowProfilePropertiesProperty);
			}
			set
			{
				SetValue(ShowProfilePropertiesProperty, value);
			}
		}

		/// <summary>
		/// ShowProfileProperties property data.
		/// </summary>
		public static readonly PropertyData ShowProfilePropertiesProperty = RegisterProperty(nameof(ShowProfileProperties), typeof(bool), null);

		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes the channel view model items.
		/// </summary>
		/// <param name="fixtureSpecification">Fixture specification being edited.</param>
		public void InitializeChildViewModels(FixtureSpecification fixtureSpecification)
		{
			// Save off a copy of the fixture specification
			_fixtureSpecification = fixtureSpecification.CreateInstanceForClone();
					
			// Set the name of the fixture
			Name = fixtureSpecification.Name;

			// Set the name of the fixture manufacturer
			Manufacturer = fixtureSpecification.Manufacturer;

			// Set the name of the user that created the profile
			CreatedBy = fixtureSpecification.CreatedBy;

			// Set revision information about the fixture profile
			Revision = fixtureSpecification.Revision;

			// Update the function name collection
			UpdateFunctionNames(_fixtureSpecification.FunctionDefinitions);

			// Clear the channel items view model collection
			Items.Clear();

			// Loop over the model channel definitions		
			foreach (FixtureChannel fixtureChannel in fixtureSpecification.ChannelDefinitions)
			{
				// Create a new channel view model item
				ChannelItemViewModel channelItem = CreateNewItem();

				// Set the channel number
				channelItem.ChannelNumber = fixtureChannel.ChannelNumber.ToString(); 

				// Set the channel name
				channelItem.Name = fixtureChannel.Name;

				// If the function is a valid function then...
				if (Functions.Contains(fixtureChannel.Function))
				{
					// Set the function associated with the channel
					channelItem.Function = fixtureChannel.Function;
				}
				else
				{
					// Otherwise set the function to None
					channelItem.Function = FixtureFunctionType.None.GetEnumDescription();
				}

				// Add the view model item to the Items collection
				Items.Add(channelItem);
			}
		}

		/// <summary>
		/// Gets the fixture specification including any edits.
		/// </summary>
		/// <returns>Fixture specification</returns>
		public FixtureSpecification GetFixtureSpecification()
		{
			// Transfer the name back to the model
			_fixtureSpecification.Name = Name;

			// Transfer the Manufacturer name
			_fixtureSpecification.Manufacturer = Manufacturer;

			// Transfer the CreatedBy name
			_fixtureSpecification.CreatedBy = CreatedBy;	

			// Transfer the revision information
			_fixtureSpecification.Revision = Revision;	

			// Clear the channel defintions in the model
			_fixtureSpecification.ChannelDefinitions.Clear();

			// Loop over the channel definitions in the view model
			foreach (ChannelItemViewModel channelVM in Items)
			{
				// Create a new fixture channel
				FixtureChannel fixtureChannel = new FixtureChannel();

				// Add the channel definition to the fixture
				_fixtureSpecification.ChannelDefinitions.Add(fixtureChannel);

				// Copy of the name of the channel from the view model to the model
				fixtureChannel.Name = channelVM.Name;

				// Copy the function associated with the channel
				fixtureChannel.Function = channelVM.Function;

				// Copy the channel number
				fixtureChannel.ChannelNumber = int.Parse(channelVM.ChannelNumber);
			}

			return _fixtureSpecification;
		}

		/// <summary>
		/// Save Specification command handler, saves the fixture specification to the Vixen repository.
		/// </summary>
		public void SaveSpecification()
		{
			// Make sure the model is up to date with all edits in the view model
			_fixtureSpecification = GetFixtureSpecification();

			// Save the fixture to the Vixen respository
			FixtureSpecificationManager.Instance().Save(_fixtureSpecification);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Validates the editable fields.
		/// </summary>
		/// <param name="validationResults">Results of the validation</param>
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			// If the name is empty then...
			if (string.IsNullOrEmpty(Name))
            {
				// Add an error
				validationResults.Add(FieldValidationResult.CreateError(NameProperty, "Profile name is empty.  Profile is a required field."));
			}

			// Display the validation bar
			DisplayValidationBar(validationResults);			
		}

		/// <summary>
		/// Validates the business rules of the view model.
		/// </summary>
		/// <param name="validationResults">Results of the validation</param>
		protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
		{
			// For a fixture to be valid it must have at least one channel
			if (Items.Count == 0)
			{
				validationResults.Add(BusinessRuleValidationResult.CreateError("At least one channel is required to define a fixture."));
			}			

			// Loop over all the channels
			foreach(ChannelItemViewModel channel in Items)
            {
				// Validate the channel view models
				channel.UpdateValidateBusinessRules(validationResults);
			}

			// Display the validation bar
			DisplayValidationBar(validationResults);
		}
		
		/// <summary>
		/// <inheritdoc/>  Refer to base class documentation.
		/// </summary>		
		protected override ChannelItemViewModel CreateNewItem()
		{
			// Create a new channel view model
			ChannelItemViewModel channel = new ChannelItemViewModel(Functions, _fixtureSpecification.FunctionDefinitions);

			// Give the new channel the delegate to refresh the command enable status
			channel.RaiseCanExecuteChanged = RaiseCanExecuteChanged;

			// Give the channel access to the edit functions command
			channel.EditFunctionsCommand = EditFunctionsCommand;

			// Give the channel the allowable channel numbers
			channel.ChannelNumbers = _channelNumbers;

			// Default the channel number to the next channel number to the allowable channel numbers
			string nextChannelNumber = (Items.Count() + 1).ToString();

			// Don't insert duplicates into the channel numbers
			if (!_channelNumbers.Contains(nextChannelNumber))
			{
				// Add the channel number to the allowable channel numbers					
				_channelNumbers.Add(nextChannelNumber);
			}

			// Initialize the channel number 
			channel.ChannelNumber = nextChannelNumber;

			// Return the new channel VM
			return channel;
		}

		/// <summary>
		/// For reasons unknown Catel seems to continue to validate items that have been removed from the collection.
		/// This method ensures they are valid so they don't produce errors.
		/// </summary>
		/// <param name="item">Item to make valid</param>
		protected override void MakeObjectValidBeforeDeleting(ChannelItemViewModel item)
		{
			item.Name = "Zombie";
			item.Function = FixtureFunctionType.None.GetEnumDescription();
			item.CloseViewModelAsync(null);			
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override void DeleteItem()
		{
			// Call the base class implementation
			base.DeleteItem();

			// Loop over the remaining channel items
			for(int index = 0; index < Items.Count; index++)
			{
				// Retrieve the specified channel
				ChannelItemViewModel channel = Items[index];	

				// Update the channel number to remove any gaps
				channel.ChannelNumber = (index + 1).ToString();	
			}
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override void AddItem()
		{
			// Call the base class implementation
			base.AddItem();

			// Default the Function to the None function
			SelectedItem.Function = FixtureFunctionType.None.GetEnumDescription();

			// Set the channel name to "Ignore"
			SelectedItem.Name = ChannelItemViewModel.IgnoreFunctionName;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Collection of allowable channel numbers.
		/// </summary>
		private ObservableCollection<string> _channelNumbers;

        /// <summary>
		/// Fixture specification being edited.
		/// </summary>
		private FixtureSpecification _fixtureSpecification;
		
		#endregion

		#region Private Methods
		
		/// <summary>
		/// Updates the function name collection from the collection of fixture functions.
		/// </summary>
		/// <param name="functionData">Collection of fixture functions</param>
		private void UpdateFunctionNames(List<FixtureFunction> functionData)
        {			
			// Create a new observable collection for the function names
			ObservableCollection<string> updatedFunctions = new ObservableCollection<string>();

			// Populate the function name collection from the model data 
			foreach (FixtureFunction function in functionData.OrderBy(fnc => fnc.Name))
			{
				// Add the function name to the collection
				updatedFunctions.Add(function.Name);
			}

			// Replace the collection of functions
			Functions = updatedFunctions;
		}

		/// <summary>
		/// Edits the functions associated with this fixture.
		/// </summary>
		private void EditFunctions()
		{	
			// Create the function type editor window indicating which function to select
			FunctionTypeWindowView view = new FunctionTypeWindowView(_fixtureSpecification.FunctionDefinitions, SelectedItem.Function);

			// Display the function type editor window
			bool? result = view.ShowDialog();

			// If the user selected to commit the function changes then...
			if (result.Value)
			{
				// Retrieve the function data
				_fixtureSpecification.FunctionDefinitions = view.GetFunctionData();
				
				// Update the function name collection
				UpdateFunctionNames(_fixtureSpecification.FunctionDefinitions);

				// Pass the actual functions to the channel item to support validation
				foreach (ChannelItemViewModel channel in Items)
				{
					channel.Functions = Functions;
					channel.FunctionObjects = _fixtureSpecification.FunctionDefinitions;
				}

				// Loop over the channel items looking for deleted functions
				foreach (ChannelItemViewModel channel in Items)
				{
					// If the function associated with channel is no longer found then...
					if (Functions.SingleOrDefault(x => x == channel.Function) == null)
					{
						// Set the channel function to 'None'
						channel.Function = FixtureFunctionType.None.GetEnumDescription();
					}
				}

				// Have Catel re-validate
				Validate(true);

				// Update the buttons enabled status
				RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Load Specification command handler, replaces the properties fixture specification from the copy in the Vixen fixture repository.
		/// </summary>
		private void LoadSpecification()
		{		
			// Create the view for selecting a fixture specification
			SelectFixtureSpecificationView view = new SelectFixtureSpecificationView(FixtureSpecificationManager.Instance().FixtureSpecifications);

			// Display the selection dialog
			bool? result = view.ShowDialog();

			// If the user picked a fixture specification then...
			if (result.Value)
			{
				// Retrieve the selected fixture specification
				FixtureSpecification fixtureSpecification = view.GetSelectedFixtureSpecification();
			
				// Update the view model with the new fixture specification
				InitializeChildViewModels(fixtureSpecification);
			}

			// Check to see if this control is being used in the context of the Fixture Property Editor 
			// (It is also being used in the Fixture Wizard where this logic is not necessary)
			if (ParentViewModel is FixturePropertyEditorWindowViewModel)
			{		
				// Refresh the enabled status of the OK button
				((Command)(ParentViewModel as FixturePropertyEditorWindowViewModel).OkCommand).RaiseCanExecuteChanged();
			}

			// Experienced weird validation behavior where the info pane would show phantom errors
			// This should force Catel to revalidate after loading the specification
			Validate(true);
		}
		
		#endregion		
	}
}
