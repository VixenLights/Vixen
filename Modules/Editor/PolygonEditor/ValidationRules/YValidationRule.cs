using System.Globalization;
using System.Windows.Controls;
using VixenModules.Editor.PolygonEditor.Converters;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
    /// <summary>
    /// Validates the Y coordinates in the polygon editor point data grid.
    /// </summary>
	public class YValidationRule : ValidationRule
	{
        #region Public Static Methods

        /// <summary>
        /// Height of the polygon editor drawing canvas.
        /// </summary>
        static public int Height { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The cuture to use in this rule.</param>
        /// <returns>Validation results</returns>
        public override ValidationResult Validate(
            object value,
            CultureInfo cultureInfo)
        {
            // Default the result to Valid
            ValidationResult result = ValidationResult.ValidResult;

            // Attempt to parse the coordinate into a double
            double doubleValue;
            bool valid = double.TryParse((string)value, out doubleValue);

            // If the string did not parse into a double then...
            if (!valid)
            {
                // Indicate the value is invalid
                result = new ValidationResult(false, "Not valid Y value.");
            }
            // Otherwise check to see if the value is off the canvas to the top
            else if (doubleValue > (Height - 1) / PolygonPointYConverter.YScaleFactor + 1)
            {
                result = new ValidationResult(false, "Y value is larger than display element.");
            }
            // Otherwise check to see if the value is off the bottom of the canvas
            else if (doubleValue < 1.0)
            {
                result = new ValidationResult(false, "Y value must be greater or equal to 1.");
            }

            // Return the results of the validation
            return result;
        }
        
        #endregion
    }
}
