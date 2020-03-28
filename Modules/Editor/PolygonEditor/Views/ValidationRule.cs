

using System.Windows.Controls;
using System.Windows.Data;

namespace PolygonEditor
{
    public class XYValidationRule : ValidationRule
    {       
        public override ValidationResult Validate(
            object value,
            System.Globalization.CultureInfo cultureInfo)
        {            
            double doubleValue = double.Parse((string)value);

            if (doubleValue > Width)
            {
                return new ValidationResult(false, "X value is larger than display element.");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }

        public int Width { get; set; }
    }
}
