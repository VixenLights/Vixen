using System.Collections;
using ExCSS;
using System.ComponentModel;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public class PropertyDiscovery
	{
		private static readonly Attribute[] PropertyFilter =
		{
			new PropertyFilterAttribute(PropertyFilterOptions.SetValues | PropertyFilterOptions.UnsetValues |
			                            PropertyFilterOptions.Valid)
		};

		public static IEnumerable<PropertyMetaData> GetBrowsableProperties(object target, IList<Type> types)
		{
			return CollectProperties(target, types).AsReadOnly();
		}

		public static IEnumerable<PropertyMetaData> GetBrowsableProperties(object target)
		{
			return CollectProperties(target).AsReadOnly();
		}

		private static List<PropertyMetaData> CollectProperties(object target, IList<Type> types, bool filterBrowsable = true)
		{
			var result = new List<PropertyMetaData>();
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(target, PropertyFilter))
			{
				if (descriptor.IsBrowsable == filterBrowsable && types.Contains(descriptor.PropertyType))
				{
					result.Add(new PropertyMetaData(descriptor, target));
					continue;
				}

				if (PropertyMetaData.IsCollectionType(descriptor.PropertyType))
				{
					if (descriptor.GetValue(target) is ICollection collectionObject)
					{
						foreach (object o in collectionObject)
						{
							result.AddRange(CollectProperties(o, types));
						}
					}
				}
			}

			return result;
		}

		private static List<PropertyMetaData> CollectProperties(object target, bool filterBrowsable = true)
		{
			var result = new List<PropertyMetaData>();
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(target, PropertyFilter))
			{
				if (descriptor.IsBrowsable == filterBrowsable)
				{
					result.Add(new PropertyMetaData(descriptor, target));
				}
			}

			return result;
		}
	}
}