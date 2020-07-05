using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using VixenModules.Editor.PolygonEditor.ViewModels;

namespace VixenModules.Editor.PolygonEditor.Converters
{
    /// <summary>
    /// Converts from a collection of PolygonPointViewModels to a Microsoft PointCollection.
    /// </summary>
    public class PointCollectionConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Regular point collection.
            PointCollection regPointCollection = new PointCollection();

            // View model PolygonPoint collection
            ObservableCollection<PolygonPointViewModel> inputCollection = (ObservableCollection<PolygonPointViewModel>)value; 

            // If the input collection is not null then...
            if (inputCollection != null)
            {
                // Loop over the view model point collection
                foreach (PolygonPointViewModel point in inputCollection)
                {
                    // Add the point to the regular point collection
                    regPointCollection.Add(point.GetPoint());
                }
            }

            // Return the regular point collection
            return regPointCollection;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Converting back is not required 
            return null;
        }

        #endregion
    }
}
