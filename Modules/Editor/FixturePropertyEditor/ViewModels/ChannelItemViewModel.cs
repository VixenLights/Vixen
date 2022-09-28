using Catel.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using VixenModules.App.Fixture;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Maintains a channel (view model) of the fixture.
	/// </summary>
	public class ChannelItemViewModel : ItemViewModel
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ChannelItemViewModel()
        {			
		}

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="functions">Function names defined for the fixture</param>
		/// <param name="functionObjects">Fixture functions</param>
		public ChannelItemViewModel(ObservableCollection<string> functions, List<FixtureFunction> functionObjects)
		{
			// Store off the function names
			Functions = functions;

			// Store off the function objects
			FunctionObjects = functionObjects;
			
			// Initialize the icon for editing the function associated with the channel
			EditFunctionsImageSource = @"/Resources;component/cog_edit.png";
		}

		#endregion

		#region Public

		/// <summary>
		/// Special function name for the 'None' function.
		/// </summary>
		public const string IgnoreFunctionName = "Ignore";

		#endregion

		#region Public Properties

		/// <summary>
		/// Command to display a dialog for editing the functions.
		/// </summary>
		public ICommand EditFunctionsCommand { get; set; }
		
		/// <summary>
		/// Colletion of available function objects.
		/// </summary>
		public List<FixtureFunction> FunctionObjects { get; set; }

		#endregion

		#region Public Catel Properties

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
		/// Gets or sets the allowable channel numbers.
		/// </summary>
		public ObservableCollection<string> ChannelNumbers
		{
			get { return GetValue<ObservableCollection<string>>(ChannelNumbersProperty); }
			set { SetValue(ChannelNumbersProperty, value); }
		}

		/// <summary>
		/// Channel numbers property data.
		/// </summary>
		public static readonly PropertyData ChannelNumbersProperty = RegisterProperty(nameof(ChannelNumbers), typeof(ObservableCollection<string>), null);


		/// <summary>
		/// Gets or sets the channel number.
		/// </summary>
		public string ChannelNumber
		{
			get { return GetValue<string>(ChannelNumberProperty); }
			set { SetValue(ChannelNumberProperty, value); }
		}

		/// <summary>
		/// Channel number property data.
		/// </summary>
		public static readonly PropertyData ChannelNumberProperty = RegisterProperty(nameof(ChannelNumber), typeof(string), null);

		/// <summary>
		/// Function associated with the channel.
		/// </summary>
		public string Function
		{
			get { return GetValue<string>(FunctionProperty); }
			set 
			{
				// If the incoming function is NOT the special "None" function and
				// the incoming function is not empty then...
				if (value != "None" &&
				    !string.IsNullOrEmpty(value))
				{
					// If the selected function is NOT valid then...
					if (!IsFunctionValid(value))
					{
						// Display an error to the user
						MessageBox.Show(
							$"Function '{value}' is incomplete.  Please populate the function before assigning it to a channel.", 
							"Incomplete Function", MessageBoxButton.OK,
							MessageBoxImage.Error);

						// Revert the function selection to 'None'
						Application.Current.Dispatcher.InvokeAsync(() => { Function = "None"; }, DispatcherPriority.ApplicationIdle);
					}
				}

				SetValue(FunctionProperty, value);

				// If the function name is still the special 'Ignore' function name and
				// the function is being set to a function other than 'None' then...
				if (Name == ChannelItemViewModel.IgnoreFunctionName &&
					value != "None")
				{
					// Make the function name match the selected function
					Name = value;
				}

				// Force Catel to re-validate
				Validate(true);
								
				// Refresh the command button enabled status
				RefreshCanExecuteChanged();
			}
		}

		/// <summary>
		/// Function property data.
		/// </summary>
		public static readonly PropertyData FunctionProperty = RegisterProperty(nameof(Function), typeof(string), null);

		/// <summary>
		/// Icon associated with the button to edit the function associated with the channel.
		/// </summary>
		public string EditFunctionsImageSource
		{
			get { return GetValue<string>(EditFunctionsImageSourceProperty); }
			set
			{
				SetValue(EditFunctionsImageSourceProperty, value);				
			}
		}

		/// <summary>
		/// Function property data.
		/// </summary>
		public static readonly PropertyData EditFunctionsImageSourceProperty = RegisterProperty(nameof(EditFunctionsImageSource), typeof(string), null);

		#endregion
		
		#region Protected Properties

		/// <summary>
		/// Validates the editable fields.
		/// </summary>
		/// <param name="validationResults">Results of the validation</param>
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			// Validate the Name property
			ValidateName(validationResults);

			// If the function is empty then...
			if (string.IsNullOrEmpty(Function))
			{
				validationResults.Add(FieldValidationResult.CreateError(FunctionProperty, "Function is empty.  Function is a required field."));
			}	
			
			// Display the validation bar
			DisplayValidationBar(validationResults);			
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Validates the business rules of the view model.
		/// </summary>
		/// <param name="validationResults">Results of the validation</param>
		public void UpdateValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
		{
			// If the function associated channel has been populated then...
			if (!string.IsNullOrEmpty(Function))
			{
				// Retrieve the function asssociated with the channel
				FixtureFunction fixtureFunction = FunctionObjects.SingleOrDefault(fnc => fnc.Name == Function);

				// If the function was found then...
				if (fixtureFunction != null)
				{
					// If the function is indexed but there are no index entries then...
					if (fixtureFunction.FunctionType == FixtureFunctionType.Indexed &&
						fixtureFunction.IndexData.Count == 0)
					{						
						validationResults.Add(BusinessRuleValidationResult.CreateError("Function index data is incomplete."));
					
						// Display the error settings cog on the button
						EditFunctionsImageSource = @"/Resources;component/cog_error_red.png";
					}
					// If the function is a color wheel but there are no color wheel entries then...
					else if (fixtureFunction.FunctionType == FixtureFunctionType.ColorWheel &&
						fixtureFunction.ColorWheelData.Count == 0)
					{
						validationResults.Add(BusinessRuleValidationResult.CreateError("Function color wheel data is incomplete."));

						// Display the error settings cog on the button
						EditFunctionsImageSource = @"/Resources;component/cog_error_red.png";
					}
					// Otherwise the function is complete
					else
					{
						// Display the normal cog 
						EditFunctionsImageSource = @"/Resources;component/cog_edit.png";
					}

					// Display the validation bar
					DisplayValidationBar(validationResults);					
				}
			}
		}

		#endregion

		#region Private Methods
		
		/// <summary>
		/// Returns true if the specified fixture function is valid.
		/// </summary>
		/// <param name="functionName">Name of the fixture function to inspect</param>
		/// <returns>True if the specified fixture function is valid</returns>
		private bool IsFunctionValid(string functionName)
		{
			// Default the function to NOT valid
			bool valid = false;

			// Retrieve the function asssociated with the channel
			FixtureFunction fixtureFunction = FunctionObjects.SingleOrDefault(fnc => fnc.Name == functionName);

			// If the function was found then...
			if (fixtureFunction != null)
			{
				// Indicate the function is valid
				valid = true;

				// If the function is indexed but there are no index entries then...
				if (fixtureFunction.FunctionType == FixtureFunctionType.Indexed &&
				    fixtureFunction.IndexData.Count == 0)
				{
					// Indicate the function is NOT valid
					valid = false;
				}
				// If the function is a color wheel but there are no color wheel entries then...
				else if (fixtureFunction.FunctionType == FixtureFunctionType.ColorWheel &&
				         fixtureFunction.ColorWheelData.Count == 0)
				{
					// Indicate the function is NOT valid
					valid = false;
				}
			}

			// Return whether 
			return valid;
		}

		#endregion
	}
}