using Catel.Data;
using System;
using System.Collections.Generic;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Pan/Tilt view model.  This view model maintains the rotations limits of a pan or tilt function.
	/// </summary>
	public class PanTiltViewModel : FixtureViewModelBase, IFixtureSaveable
	{
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PanTiltViewModel()
		{			
		}

		#endregion

		#region Fields

		/// <summary>
		/// Rotation limits associated with the function.
		/// </summary>
		private FixtureRotationLimits _rotationLimits;

		#endregion
		
		#region Public Methods

		/// <summary>
		/// Initializes the view model with index model data.
		/// </summary>		
		/// <param name="rotationLimits">Rotation limits to edit</param>
		/// <param name="raiseCanExecuteChanged">Action to refresh the command status of the parent</param>
		public void InitializeViewModel(FixtureRotationLimits rotationLimits, Action raiseCanExecuteChanged)
		{
			// Store off the rotation limits
			_rotationLimits = rotationLimits;

			// If the rotation limits are not null then...
			if (rotationLimits != null)
			{
				// Convert the start poisition angle to a string
				StartPosition = rotationLimits.StartPosition.ToString();

				// Convert the end poisition angle to a string
				StopPosition = rotationLimits.StopPosition.ToString();
			}

			// Give the index VM the ability to refresh the command enable / disable status
			RaiseCanExecuteChanged = raiseCanExecuteChanged;
		}

		/// <summary>
		/// Retrieves the rotation limits from the view model.
		/// </summary>
		/// <returns></returns>
		public FixtureRotationLimits GetRotationLimitsData()
		{
			// Create the fixture rotation limits model
			FixtureRotationLimits limits = new FixtureRotationLimits();

			// Assign the start position converting from string to integer
			limits.StartPosition = int.Parse(StartPosition);

			// Assign the end position converting from string to integer
			limits.StopPosition = int.Parse(StopPosition);

			return limits;
		}

		/// <summary>
		/// Gets the rotation limits associated with the pan or tilt function.
		/// </summary>
		/// <returns>Rotation limits of the function</returns>
		public FixtureRotationLimits GetRotationLimits()
		{
			// Create a new rotation limits model object
			FixtureRotationLimits rotationLimits = new FixtureRotationLimits();
						
			// Retrieve the start angle from the view model
			rotationLimits.StartPosition = int.Parse(StartPosition);

			// Retrieve teh stop angle from the view model
			rotationLimits.StopPosition = int.Parse(StopPosition);
			
			return rotationLimits;
		}

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Maximum rotation of the function (pan or tilt) in degrees.
		/// </summary>
		public string StartPosition
		{
			get { return GetValue<string>(StartPositionProperty); }
			set
			{
				SetValue(StartPositionProperty, value);

				// Refresh command status
				RaiseCanExecuteChangedInternal();
			}
		}

		/// <summary>
		/// Maximum rotation property data.
		/// </summary>
		public static readonly PropertyData StartPositionProperty = RegisterProperty(nameof(StartPosition), typeof(string), null);

		/// <summary>
		/// Maximum rotation of the function (pan or tilt) in degrees.
		/// </summary>
		public string StopPosition
		{
			get { return GetValue<string>(StopPositionProperty); }
			set
			{
				SetValue(StopPositionProperty, value);

				// Refresh command status
				RaiseCanExecuteChangedInternal();				
			}
		}

		/// <summary>
		/// Maximum rotation property data.
		/// </summary>
		public static readonly PropertyData StopPositionProperty = RegisterProperty(nameof(StopPosition), typeof(string), null);

        #endregion

        #region Private Methods

		/// <summary>
		/// Validates a rotation angle.
		/// </summary>
		/// <param name="validationResults">Validation results to update</param>
		/// <param name="propertyData">Property data associated with the field</param>
		/// <param name="fieldName">Name of the field</param>
		/// <param name="value">Value to validate</param>
        private void ValidatesAngle(
			List<IFieldValidationResult> validationResults,
			PropertyData propertyData,
			string fieldName,
			string value)
		{
			// If the angle is empty then...
			if (string.IsNullOrEmpty(value))
			{
				validationResults.Add(FieldValidationResult.CreateError(propertyData, $"{fieldName} is empty.  Valid range is 0-999 degrees.  "));
			}
			else
			{
				// Attempt to parse the angle into an integer
				int intValue = 0;
				bool valid = int.TryParse(value, out intValue);

				// If the string did not parse into a int then...
				if (!valid)
				{
					// Indicate the value is invalid
					validationResults.Add(FieldValidationResult.CreateError(propertyData, $"Not a valid {fieldName} value.  Valid values are 0-999.  "));
				}
				// Otherwise check to see if the value is within range
				else if (intValue < 0 || intValue > 999)
				{
					// Indicate the value is invalid
					validationResults.Add(FieldValidationResult.CreateError(propertyData, $"Not a valid {fieldName} value.  Valid values are 0-999.  "));
				}
			}
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Validates the editable fields.
        /// </summary>
        /// <param name="validationResults">Results of the validation</param>

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			// If the rotation limits have been populated then...
			if (_rotationLimits != null)
			{
				// Validate the start position
				ValidatesAngle(validationResults, StartPositionProperty, "Start Position", StartPosition);
				
				// Validate the end position
				ValidatesAngle(validationResults, StopPositionProperty, "Stop Position", StopPosition);

				// Display the validation bar
				DisplayValidationBar(validationResults);				
			}
		}

		#endregion		
	}
}
