using Catel.Data;
using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Abstract base class for fixture view models.
	/// </summary>
    public abstract class FixtureViewModelBase : ViewModelBase, IFixtureSaveable
    {
		#region Public Properties

		/// <summary>
		/// Delegate to ensure the command buttons (Ok, or Next Page) are enabled or disabled promptly.
		/// </summary>
		public Action RaiseCanExecuteChanged { get; set; }

		#endregion

		#region Protected Properties

		/// <summary>
		/// Validations results for whether the data can be saved.
		/// </summary>
		protected string CanSaveValidationResults { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refreshes parent command enable/disable status.
		/// </summary>
		protected void RaiseCanExecuteChangedInternal()
		{
			// If the delegate has been set then...
			if (RaiseCanExecuteChanged != null)
			{
				// Refresh the command status
				RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Displays the validation bar if there are messages in the validation results.
		/// </summary>
		/// <param name="validationResults">Validation results to examine</param>
		protected void DisplayValidationBar(List<IFieldValidationResult> validationResults)
		{
			// If there are any validation results then...
			if (validationResults.Any())
			{
				// Make sure the validation bar is displayed
				HideValidationResults = false;
			}
		}

		/// <summary>
		/// Displays the validation bar if there are messages in the validation results.
		/// </summary>
		/// <param name="validationResults">Validation results to examine</param>
		protected void DisplayValidationBar(List<IBusinessRuleValidationResult> validationResults)
		{
			// If there are any validation results then...
			if (validationResults.Any())
			{
				// Make sure the validation bar is displayed
				HideValidationResults = false;
			}
		}

		#endregion

		#region IFixtureSaveable

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		/// <summary>
		/// Returns true if all required data has been populated and it is safe to save the data.
		/// This method is being used to determine if it is safe to either save the property data or
		/// to save the fixture specification to the Vixen repository.
		/// </summary>
		/// <returns>True if all required data has been poopulated</returns>
		public virtual bool CanSave()
		{
			// Default to being valid to save
			bool canSave = true;

			// Clear out the previous save validation results
			CanSaveValidationResults = string.Empty;

			// Validate the top level fields
			List<IFieldValidationResult> fieldValidationResults = new List<IFieldValidationResult>();
			ValidateFields(fieldValidationResults);

			// Loop over the field validation results
			foreach (IFieldValidationResult result in fieldValidationResults)
			{
				// Concatenate the error messages
				CanSaveValidationResults += result.Message + "\n";

				// Indicate the fixture cannot be saved
				canSave = false;
			}
			
			// Validate the business
			List<IBusinessRuleValidationResult> validationResults = new List<IBusinessRuleValidationResult>();
			ValidateBusinessRules(validationResults);

			// Loop over the business rule validation results
			for (int index = 0; index < validationResults.Count; index++)
			{
				// Get the specified validation result
				IBusinessRuleValidationResult validationResult = validationResults[index];

				// Concatenate the error messages
				CanSaveValidationResults += validationResult.Message + "\n";

				// Indicate the fixture cannot be saved
				canSave = false;
			}
			
			return canSave;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public virtual string GetValidationResults()
		{
			// Return the validation results
			return CanSaveValidationResults;
		}

		#endregion
	}
}
