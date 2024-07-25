using System.Collections.Generic;

using Catel.Data;
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
			// Create the animation event and initialize to the signaled state
			Animate = new ManualResetEvent(true);
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
		/// <param name="isPan">True when the active function is the Pan function</param>
		public void InitializeViewModel(FixtureRotationLimits rotationLimits, Action raiseCanExecuteChanged, bool isPan)
		{
			// Resume the animation in the view
			Animate.Set();
			
			// Store off the rotation limits
			_rotationLimits = rotationLimits;

			// Store off whether the pan rotation limits are being configured
			IsPan = isPan;

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
		public static readonly IPropertyData StartPositionProperty = RegisterProperty<string>(nameof(StartPosition));

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
		public static readonly IPropertyData StopPositionProperty = RegisterProperty<string>(nameof(StopPosition));

		/// <summary>
		/// Indicates if the Pan function (true)  being editor vs the Tilt function (false).
		/// </summary>
		public bool IsPan
		{
			get { return GetValue<bool>(IsPanProperty); }
			set
			{
				SetValue(IsPanProperty, value);

				// Refresh command status
				RaiseCanExecuteChangedInternal();
			}
		}

		/// <summary>
		/// IsPan property data.
		/// </summary>
		public static readonly IPropertyData IsPanProperty = RegisterProperty<bool>(nameof(IsPan));

		#endregion

		#region Public Properties

		/// <summary>
		/// Event that determines when the fixture graphic needs to be animated.
		/// Fixture is animated when the Pan or Tilt function are being edited.
		/// </summary>
		public ManualResetEvent Animate { get; set; }

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
			IPropertyData propertyData,
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

		/// <summary>
		/// Validates that the minimum value is less than or equal to the maximum.
		/// </summary>
		/// <param name="validationResults">Results of the validation</param>
		private void ValidateMinimumLessThanMaximum(List<IFieldValidationResult> validationResults)
		{
			// Attempt to parse the angles into an integers
			int startPositionValue = 0;
			int stopPositionValue = 0;
			bool validStartPosition = int.TryParse(StartPosition, out startPositionValue);
			bool validStopPosition = int.TryParse(StopPosition, out stopPositionValue);

			// If both values are valid then...
			if (validStartPosition && validStopPosition)
			{
				// If the minimum is greater than the maximum then...
				if (startPositionValue > stopPositionValue)
				{
					// Add an error to the validation results
					validationResults.Add(FieldValidationResult.CreateError(StartPositionProperty, $"Start Position must be less than or equal to the Stop Position."));
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

				// Validate that the minimum is less or equal to the maximum
				ValidateMinimumLessThanMaximum(validationResults);

				// Display the validation bar
				DisplayValidationBar(validationResults);				
			}
		}

		#endregion		
	}
}
