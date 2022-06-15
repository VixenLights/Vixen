using Catel.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using VixenModules.App.Fixture;

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
				SetValue(FunctionProperty, value);

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
						validationResults.Add(BusinessRuleValidationResult.CreateError("Function index data is incomplete.  Select the settings button to enter in the index data."));
					
						// Display the error settings cog on the button
						EditFunctionsImageSource = @"/Resources;component/cog_error_red.png";
					}
					// If the function is a color wheel but there are no color wheel entries then...
					else if (fixtureFunction.FunctionType == FixtureFunctionType.ColorWheel &&
						fixtureFunction.ColorWheelData.Count == 0)
					{
						validationResults.Add(BusinessRuleValidationResult.CreateError("Function color wheel data is incomplete.  Select the settings button to enter in the index data."));

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
	}
}