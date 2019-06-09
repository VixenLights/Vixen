using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VixenModules.App.CustomPropEditor.Converters
{
	public class CompositeCollectionConverter : IMultiValueConverter
	{

		public object Convert(object[] values
			, Type targetType
			, object parameter
			, System.Globalization.CultureInfo culture)
		{
			var res = new CompositeCollection();
			foreach (var item in values)
				if (item is IEnumerable)
					res.Add(new CollectionContainer()
					{
						Collection = item as IEnumerable
					});
				else res.Add(item);
			return res;
		}

		public object[] ConvertBack(object value
			, Type[] targetTypes
			, object parameter
			, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
