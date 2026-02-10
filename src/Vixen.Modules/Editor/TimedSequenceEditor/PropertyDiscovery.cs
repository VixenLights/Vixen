#nullable enable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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

		public static string? GetDisplayName(Type type)
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

		public static List<PropertyMetaData> FindPropertyByNameAndType(object component, Type propertyType, string propertyName)
		{
			List<Type> types = new List<Type>(){propertyType};
			return CollectProperties(component, types).Where(x => x.Name == propertyName).ToList();
		}

		

	}

	public static class PropertyDiscoveryExtensions
	{
		public static PropertyMetaData ToNewOwner(this PropertyMetaData propertyMetaData, object newOwner)
		{
			if (newOwner.GetType() == propertyMetaData.Owner.GetType())
			{
				var propertyOwnerMetaData =
					new PropertyOwnerMetaData(newOwner, propertyMetaData.OwnerMetaData.CollectionIndex);
				return new PropertyMetaData(propertyMetaData.Descriptor, propertyOwnerMetaData);
			}
			//Otherwise our property is a child collection type.
			var propertyMetaDataList =
				PropertyDiscovery.FindPropertyByNameAndType(newOwner, propertyMetaData.PropertyType,
					propertyMetaData.Name).Where(x => x.OwnerMetaData.IsCollectionChild && x.OwnerMetaData.CollectionIndex == propertyMetaData.OwnerMetaData.CollectionIndex);
			//hopefully there is only one.
			return propertyMetaDataList.First();
		}
	}

}