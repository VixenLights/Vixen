using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Catel.MVVM;

namespace VixenModules.App.CustomPropEditor.Converters
{
	public class PointMappingConverter : DefaultViewModelToModelMappingConverter
	{
		public PointMappingConverter(string[] propertyNames) : base(propertyNames)
		{
		}

		#region Methods
		public override bool CanConvert(Type[] types, Type outType, Type viewModelType)
		{
			return types.All(x => x == typeof(double)) && outType == typeof(Point); //check that all input and output values are strings
		}

		public override object Convert(object[] values, IViewModel viewModel)
		{
			var x = System.Convert.ToDouble(values[0]);
			var y = System.Convert.ToDouble(values[1]);
			return new Point(x, y);
			//return string.Join(Separator.ToString(), values.Where(x => !string.IsNullOrWhiteSpace((string)x)));
		}

		public override bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType)
		{
			return outTypes.All(x => x == typeof(double)) && inType == typeof(Point); //check that all input and output values are strings
		}

		public override object[] ConvertBack(object value, IViewModel viewModel)
		{
			var p = value as Point?;
			return new object[] { p.Value.X, p.Value.Y };

		}
		#endregion
	}
}
