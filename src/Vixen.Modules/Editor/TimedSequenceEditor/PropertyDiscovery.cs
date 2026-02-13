#nullable enable

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

		/// <summary>
		/// Retrieves a collection of browsable property metadata for the specified target object, filtered by the provided
		/// list of types.
		/// </summary>
		/// <param name="target">The object whose properties are to be examined. Cannot be null.</param>
		/// <param name="types">A list of types used to filter the properties to include. Only properties matching these types will be considered.
		/// Cannot be null.</param>
		/// <returns>A read-only collection of PropertyMetaData objects representing the browsable properties of the target object that
		/// match the specified types. The collection will be empty if no matching properties are found.</returns>
		public static IEnumerable<PropertyMetaData> GetBrowsableProperties(object target, IList<Type> types)
		{
			return CollectProperties(target, types).AsReadOnly();
		}

		/// <summary>
		/// Retrieves a collection of property metadata for the specified object that are considered browsable.
		/// </summary>
		/// <param name="target">The object whose browsable properties are to be retrieved. Cannot be null.</param>
		/// <returns>An enumerable collection of <see cref="PropertyMetaData"/> objects representing the browsable properties of the
		/// specified object. The collection will be empty if no browsable properties are found.</returns>
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

		/// <summary>
		/// Gets the display name for the specified type, using display-related attributes if available.
		/// </summary>
		/// <remarks>This method checks for a DisplayNameAttribute or DisplayAttribute applied to the type. If neither
		/// attribute is found, the method returns the type's simple name. This is commonly used for UI display or metadata
		/// purposes.</remarks>
		/// <param name="type">The type for which to retrieve the display name. Cannot be null.</param>
		/// <returns>A string containing the display name defined by a DisplayNameAttribute or DisplayAttribute if present; otherwise,
		/// the type's name. Returns null if the display attribute is present but its name is not set.</returns>
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

		/// <summary>
		/// Finds all properties of the specified component that match the given property name and type.
		/// </summary>
		/// <param name="component">The object whose properties are to be searched. Cannot be null.</param>
		/// <param name="propertyType">The type that the property must match. Cannot be null.</param>
		/// <param name="propertyName">The name of the property to search for. The comparison is case-sensitive.</param>
		/// <returns>A list of PropertyMetaData objects representing the properties that match the specified name and type. Returns an
		/// empty list if no matching properties are found.</returns>
		public static List<PropertyMetaData> FindPropertyByNameAndType(object component, Type propertyType, string propertyName)
		{
			List<Type> types = new List<Type>(){propertyType};
			return CollectProperties(component, types).Where(x => x.Name == propertyName).ToList();
		}

		

	}

	public static class PropertyDiscoveryExtensions
	{
		/// <summary>
		/// Attempts to create a new PropertyMetaData instance associated with a specified new owner, preserving the property
		/// context and collection index.
		/// </summary>
		/// <remarks>If the new owner is of the same type as the original owner, the property metadata is transferred
		/// directly. If the property represents a child collection item, the method attempts to locate the corresponding
		/// property in the new owner's collection with the same index. If no matching property is found, the method returns
		/// false and null.</remarks>
		/// <param name="propertyMetaData">The PropertyMetaData instance to transfer to the new owner. Must not be null.</param>
		/// <param name="newOwner">The object that will become the new owner of the property metadata. Cannot be null.</param>
		/// <returns>A tuple containing a boolean indicating success, and the new PropertyMetaData instance if successful; otherwise,
		/// null.</returns>
		/// <exception cref="ArgumentNullException">Thrown if newOwner is null.</exception>
		public static (bool success, PropertyMetaData? propertyMetaData) ToNewOwner(this PropertyMetaData propertyMetaData, object newOwner)
		{
			if (newOwner == null) throw new ArgumentNullException(nameof(newOwner));
			if (newOwner.GetType() == propertyMetaData.Owner.GetType())
			{
				var propertyOwnerMetaData =
					new PropertyOwnerMetaData(newOwner, propertyMetaData.OwnerMetaData.CollectionIndex);
				return (true, new PropertyMetaData(propertyMetaData.Descriptor, propertyOwnerMetaData));
			}
			//Otherwise our property is a child collection type.
			var propertyMetaDataList =
				PropertyDiscovery.FindPropertyByNameAndType(newOwner, propertyMetaData.PropertyType,
					propertyMetaData.Name).Where(x => x.OwnerMetaData.IsCollectionChild && x.OwnerMetaData.CollectionIndex == propertyMetaData.OwnerMetaData.CollectionIndex).ToArray();

			if (!propertyMetaDataList.Any())
			{
				return (false,null);
			}
			//hopefully there is only one.
			return (true, propertyMetaDataList.First());
		}
	}

}