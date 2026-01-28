using System.Collections;
using System.ComponentModel;
using Vixen.Module;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public class PropertyMetaData(PropertyDescriptor descriptor, object owner) : IEquatable<PropertyMetaData>
	{
		public PropertyDescriptor Descriptor { get; init; } = descriptor;

		public string Name
		{
			get { return Descriptor.Name; }
		}

		public string DisplayName
		{
			get { return Descriptor.DisplayName; }
		}

		public bool IsBrowsable
		{
			get { return Descriptor.IsBrowsable; }
		}

		public bool IsReadOnly
		{
			get { return Descriptor.IsReadOnly; }
		}

		public Type PropertyType
		{
			get { return Descriptor.PropertyType; }
		}

		public bool IsCollection
		{
			get { return IsCollectionType(PropertyType); }
		}

		public static bool IsCollectionType(Type t)
		{
			return typeof(ICollection).IsAssignableFrom(t);
		}

		public object Owner { get; init; } = owner;

		#region IEquatable<PropertyMetaData> Members

		public bool Equals(PropertyMetaData other)
		{
			return other != null && Descriptor.Equals(other.Descriptor);
		}

		#endregion

		#region System.Object overrides

		public override int GetHashCode()
		{
			return Descriptor.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is PropertyMetaData data) && Descriptor.Equals(data.Descriptor);
		}

		#endregion
	}
}