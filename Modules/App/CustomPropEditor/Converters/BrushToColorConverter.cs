using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Catel.MVVM;

namespace VixenModules.App.CustomPropEditor.Converters
{
	public class BrushToColorConverter : DefaultViewModelToModelMappingConverter
	{
		/// <inheritdoc />
		public BrushToColorConverter(string[] propertyNames) : base(propertyNames)
		{

		}

		#region Methods
		public override bool CanConvert(Type[] types, Type outType, Type viewModelType)
		{
			return types.All(x => x == typeof(Brush)) && outType == typeof(Color); 
		}

		public override object Convert(object[] values, IViewModel viewModel)
		{

			var brush = (SolidColorBrush) values.First();

			return brush.Color;
		}

		public override bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType)
		{
			return outTypes.All(x => x == typeof(Color)) && inType == typeof(Brush); 
		}

		public override object[] ConvertBack(object value, IViewModel viewModel)
		{
			var color = (Color) value;

			return new object[] {new SolidColorBrush(color) };
		}
		#endregion
	}
}
