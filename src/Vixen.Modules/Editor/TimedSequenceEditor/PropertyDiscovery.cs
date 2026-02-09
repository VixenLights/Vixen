using ExCSS;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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

		private static List<PropertyMetaData> CollectProperties(object target, IList<Type> types, bool filterBrowsable = true, int collectionIndex = -1)
		{
			var result = new List<PropertyMetaData>();
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(target, PropertyFilter))
			{
				if (descriptor.IsBrowsable == filterBrowsable && types.Contains(descriptor.PropertyType))
				{
					result.Add(new PropertyMetaData(descriptor, new PropertyOwnerMetaData(target, collectionIndex)));
					continue;
				}

				if (descriptor.IsBrowsable == filterBrowsable && PropertyMetaData.IsCollectionType(descriptor.PropertyType))
				{
					if (descriptor.GetValue(target) is ICollection collectionObject)
					{
						int index = 0;
						foreach (object o in collectionObject)
						{
							result.AddRange(CollectProperties(o, types, filterBrowsable, index));
							index++;
						}
					}
				}
			}

			return result;
		}

		private static List<PropertyMetaData> CollectProperties(object target, bool filterBrowsable = true, int collectionIndex = -1)
		{
			var result = new List<PropertyMetaData>();
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(target, PropertyFilter))
			{
				if (descriptor.IsBrowsable == filterBrowsable)
				{
					result.Add(new PropertyMetaData(descriptor, new PropertyOwnerMetaData(target, collectionIndex)));
					
					if (descriptor.IsBrowsable == filterBrowsable && PropertyMetaData.IsCollectionType(descriptor.PropertyType))
					{
						if (descriptor.GetValue(target) is ICollection collectionObject)
						{
							int index = 0;
							foreach (object o in collectionObject)
							{
								result.AddRange(CollectProperties(o, filterBrowsable, index));
								index++;
							}
						}
					}
				}
			}

			return result;
		}

		public static string GetDisplayName(Type type)
		{
			// Try to get DisplayNameAttribute
			var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
			if (displayNameAttr != null)
			{
				return displayNameAttr.DisplayName;
			}

			// Try to get DisplayAttribute
			var displayAttr = type.GetCustomAttribute<DisplayAttribute>();
			if (displayAttr != null)
			{
				return displayAttr.Name;
			}

			// If neither is found, return the class name itself
			return type.Name;
		}
	}

}