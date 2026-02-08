using System.ComponentModel;

namespace Vixen.Module.Effect
{
	/// <summary>
	/// Contains the details for a property to be displayed in the effect editor.
	/// </summary>
	public class PropertyDetail
	{
		public PropertyDetail(PropertyDescriptor propertyDescriptor, string name, object effect)
		{
			PropertyDescriptor = propertyDescriptor;
			Effect = effect;
			Name = name;
		}

		/// <summary>
		/// The property descriptor.
		/// </summary>
		public PropertyDescriptor PropertyDescriptor { get; set; }

		/// <summary>
		/// The effect instance the property belongs to. If the effect contains sub-effects (i.e. Wave), this is the sub-effect instance.
		/// </summary>
		public Object Effect { get; set; }

		/// <summary>
		/// The display name of the property.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// Contains a collection of properties for an effect or sub-effect.
	/// </summary>
	public class EffectProperties
	{
		private List<PropertyDetail> _properties = new List<PropertyDetail>();

		public EffectProperties(string title = "")
		{
			Title = title;
		}

		/// <summary>
		/// Initializes an instance of the EffectProperties class.
		/// </summary>
		/// <param name="effect">Specifies the effect instance the list of properties belong to. If the effect contains sub-effects (i.e. Wave), this is the sub-effect instance.</param>
		/// <param name="properties">Specifies a list of properties</param>
		/// <param name="title">Specifies the display name of the group of properties</param>
		public EffectProperties(object effect, IEnumerable<PropertyDescriptor> properties, string title)
		{
			Title = title;
			foreach (var pd in properties)
			{
				_properties.Add(new PropertyDetail(pd, pd.DisplayName, effect));
			}
		}

		/// <summary>
		/// Adds a list of properties to the collection.
		/// </summary>
		/// <param name="effect">Specifies the effect instance the list of properties belong to. If the effect contains sub-effects (i.e. Wave), this is the sub-effect instance.</param>
		/// <param name="properties">Specifies a list of properties</param>
		public void Add(IEffectModuleInstance effect, IEnumerable<PropertyDescriptor> properties)
		{
			foreach (var pd in properties)
			{
				_properties.Add(new PropertyDetail(pd, pd.DisplayName, effect));
			}
		}

		/// <summary>
		/// Adds a single property to the collection.
		/// </summary>
		/// <param name="effect">Specifies the effect instance the property belongs to. If the effect contains sub-effects (i.e. Wave), this is the sub-effect instance.</param>
		/// <param name="property">Specifies the property</param>
		/// <param name="displayName">Specifies the display name of the property</param>
		public void Add(object effect, PropertyDescriptor property, string displayName)
		{
			_properties.Add(new PropertyDetail(property, displayName, effect));
		}

		public List<PropertyDetail> PropertyDetail { get { return _properties; } }

		public string Title { get; }
	}
}