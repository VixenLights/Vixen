using System;
using System.Collections.ObjectModel;
using System.Linq;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Converters
{
	class RootNodeToCollectionMapping : DefaultViewModelToModelMappingConverter
	{
		public RootNodeToCollectionMapping(string[] propertyNames) : base(propertyNames)
		{
		}

		#region Methods
		public override bool CanConvert(Type[] types, Type outType, Type viewModelType)
		{
			//check that all input and output values are of the proper type
			return types.All(x => x == typeof(ElementModel)) && outType == typeof(ObservableCollection<ElementModel>);
		}

		public override object Convert(object[] values, IViewModel viewModel)
		{
			var c = new ObservableCollection<ElementModel>();
			if (values.Length == 1)
			{
				var em = values[0] as ElementModel;
				if (em != null)
				{
					c.Add(em);
				}
			}

			return c;

		}

		public override bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType)
		{
			//check that all input and output values are of the proper type
			return outTypes.All(x => x == typeof(ElementModel)) && inType == typeof(ObservableCollection<ElementModel>);
		}

		public override object[] ConvertBack(object value, IViewModel viewModel)
		{
			var collection = value as ObservableCollection<ElementModel>;
			if (collection.Count > 0)
			{
				return new[] { collection.First() };
			}

			return new object[] { null };
		}

		#endregion
	}
}
