using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PolygonEditor
{
    public class PointCollectionConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var regPtsColl = new PointCollection(); //regular points collection.
            var obsPtsColl = (ObservableCollection<PolygonPointViewModel>)value; //observable which is used to raise INCC event.

            if (obsPtsColl != null)
            {
                foreach (var point in obsPtsColl)
                {
                    regPtsColl.Add(point.GetPoint());
                }
            }

            return regPtsColl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
