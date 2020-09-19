using Catel.MVVM.Converters;
using System;
using System.Globalization;

namespace VixenModules.Editor.PolygonEditor.Converters
{
	/// <summary>
	/// Converts points from editor coordinates to Vixen display element coordinates.
	/// This converter also changes from zero based coordinates to one based.
	/// </summary>
	public class PolygonPointYConverter : IValueConverter
	{
		#region Static Public Properties

		/// <summary>
		/// Y Axis Scale Factor of the drawing canvas.
		/// </summary>
		public static double YScaleFactor { get; set; }

		/// <summary>
		/// Height of the drawing canvas.
		/// </summary>
		public static double BufferHt { get; set; }

		#endregion

		#region IValueConverter

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
			// Convert the object to a double
			double point = (double)value;

			// Apply the scale factor and then add 1
			point = ((BufferHt - 1) - (point / YScaleFactor)) + 1;

			// Round to the nearest integer
			point = Math.Round(point);

			return point;
		}

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Parse the string into a double
			double dblValue = double.Parse((string)value);

			// Subtract one and then apply the scale factor
			return ((BufferHt - 1) - ((double) dblValue - 1)) * YScaleFactor;
		}

		#endregion
	}
}
