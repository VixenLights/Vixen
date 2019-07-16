using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Effect.Effect
{
	public class ExpandoObjectBase:ICustomTypeDescriptor, INotifyPropertyChanged
	{
		private readonly Dictionary<string, bool> _browsableState = new Dictionary<string, bool>();

		#region INotifyPropertyChanged

		[Browsable(false)]
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		/// <param name="propertyName"></param>
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion



		#region ICustomTypeDescriptor

		public virtual AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public virtual string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public virtual string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public virtual TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public virtual EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public virtual PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public virtual EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetPropertiesImpl(null);
		}

		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return GetPropertiesImpl(attributes);

		}

		public virtual PropertyDescriptorCollection GetPropertiesImpl(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(this, attributes, true);
			//Enhance the base properties with our updated browsable attributes
			lock (_browsableState)
			{
				var t = propertyDescriptorCollection.Cast<PropertyDescriptor>().Select(prop =>
				{
					if (_browsableState.ContainsKey(prop.Name))
					{
						bool state = _browsableState[prop.Name];
						List<Attribute> newAttributes =
							prop.Attributes.Cast<Attribute>().Where(attribute => !(attribute is BrowsableAttribute)).ToList();
						newAttributes.Add(new BrowsableAttribute(state));
						return TypeDescriptor.CreateProperty(GetType(), prop, newAttributes.ToArray());
					}

					return prop;

				});

				return new PropertyDescriptorCollection(t.ToArray());
			}

		}

		public virtual object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public void SetBrowsable(string property, bool browsable)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1) { { property, browsable } };
			SetBrowsable(propertyStates);
		}

		public void SetBrowsable(Dictionary<string, bool> propertyStates)
		{
			lock (_browsableState)
			{
				foreach (var propertyState in propertyStates)
				{
					if (_browsableState.ContainsKey(propertyState.Key))
					{
						_browsableState[propertyState.Key] = propertyState.Value;
					}
					else
					{
						_browsableState.Add(propertyState.Key, propertyState.Value);
					}
				}

			}

		}


		#endregion
	}
}
