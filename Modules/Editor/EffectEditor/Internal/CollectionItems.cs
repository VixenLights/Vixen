using System;
using System.Collections;
using System.ComponentModel;

namespace VixenModules.Editor.EffectEditor.Internal
{
	public class CollectionItems
	{
		private readonly IList _values;
		
		private readonly PropertyItem _propertyItem;
		public CollectionItems(PropertyItem propertyItem)
		{
			_propertyItem = propertyItem;
			_values = (IList) _propertyItem.GetValue();
		}

		public object GetCollectionValue(int index)
		{
			return _values[index]; 
		} 

		public TypeConverter CollectionItemConverter
		{
			get { return TypeDescriptor.GetConverter(GetCollectionValue(0)); }
		}

		public Type CollectionItemType
		{
			get { return GetCollectionValue(0).GetType(); }
		}


		public void SetCollectionValue(Object value, int index)
		{
			_propertyItem.SetCollectionValue(value, index);
		}

	}
}
