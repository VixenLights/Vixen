using System.Globalization;
using System.Windows.Controls;
using VixenModules.Editor.PolygonEditor.Converters;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
    /// <summary>
    /// Validates the X coordinates in the polygon editor point data grid.
    /// </summary>
    public class XValidationRule : ValidationRule
    {
		#region Static Public Properties

        /// <summary>
        /// Width of the polygon editor drawing canvas.
        /// </summary>
		static public int Width { get; set; }

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
            double doubleValue = 0;
            bool valid = double.TryParse((string)value, out doubleValue);

            // If the string did not parse into a double then...
            if (!valid)
            {
                // Indicate the value is invalid
                result = new ValidationResult(false, "Not valid X value.");
            }
            // Otherwise check to see if the value is off the canvas to the right
            else if (doubleValue > (Width - 1)  / PolygonPointXConverter.XScaleFactor + 1)
            {
                result = new ValidationResult(false, "X value is larger than display element.");
            }
            // Otherwise check to see if the value is off the canvas to the left
            else if (doubleValue < 1.0)
            {
                result = new ValidationResult(false, "X value must be greater or equal to 1.");
            }

            // Return the results of the validation
            return result;            
        }

        #endregion      
    }
}
