using Catel.Data;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Abstract base class for an item view model.  This view model is used with <c>ItemsViewModel</c>.
	/// </summary>
	public abstract class ItemViewModel : FixtureViewModelBase, IFixtureSaveable
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ItemViewModel()
		{
			// Default the name to empty
			Name = String.Empty;

			// Configure Catel to validate immediately
			DeferValidationUntilFirstSaveCall = false;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Event for when the Name property changes.
		/// </summary>
		public event EventHandler NameChanged;

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Name of the item.
		/// </summary>
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set
			{
				SetValue(NameProperty, value);

				// Fire the Name changed event
				EventHandler handler = NameChanged;
				handler?.Invoke(this, EventArgs.Empty);

				// Refresh command enabled status
				RefreshCanExecuteChanged();
			}
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

		#endregion
				
		#region Protected Methods

		/// <summary>
		/// Refreshes the command button enable status.
		/// </summary>
		protected void RefreshCanExecuteChanged()
		{
			// If the delegate to refresh the comand enable status has been set then...
			if (RaiseCanExecuteChanged != null)
			{
				// Refresh the command enable status
				RaiseCanExecuteChanged();
			}
		}
		
		/// <summary>
		/// Validates the Name property.
		/// </summary>
		/// <param name="validationResults">Validation results to update</param>
		protected void ValidateName(List<IFieldValidationResult> validationResults)
		{
			// If the name is empty then...
			if (string.IsNullOrEmpty(Name))
			{				
				validationResults.Add(FieldValidationResult.CreateError(NameProperty, "Name is empty.  Name is a required field."));
			}
		}

		/// <summary>
		/// Validates a DMX number (0-255).
		/// </summary>
		/// <param name="validationResults">Validation results to update</param>
		/// <param name="propertyData">Property data associated with the field</param>
		/// <param name="fieldName">Name of the field</param>
		/// <param name="value">Value of the field</param>
		protected void ValidateDMXNumber(
			List<IFieldValidationResult> validationResults,
			IPropertyData propertyData,
			string fieldName,
			string value)
		{
			// If the field is empty then...
			if (string.IsNullOrEmpty(value))
			{
				validationResults.Add(FieldValidationResult.CreateError(propertyData, $"{fieldName} is empty. {fieldName} is a required field."));
			}
			else
			{
				// Attempt to parse the angle into an integer
				int intValue = 0;
				bool valid = int.TryParse(value, out intValue);

				// If the string did not parse into an int then...
				if (!valid)
				{
					// Indicate the value is invalid
					validationResults.Add(FieldValidationResult.CreateError(propertyData, $"Not a valid {fieldName} value.  Valid range is 0 to 255."));
				}
				// Otherwise check to see if the value is within range
				else if (intValue < 0 || intValue > 255)
				{
					// Indicate the value is invalid
					validationResults.Add(FieldValidationResult.CreateError(propertyData, $"Not a valid {fieldName} value.  Valid range is 0 to 255."));
				}
			}
		}
		
		#endregion						
	}
}
