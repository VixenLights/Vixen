/*
 * Copyright © 2010, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Editor.EffectEditor.Input;
using VixenModules.Editor.EffectEditor.Internal;
using VixenModules.Editor.EffectEditor.PropertyEditing;
using VixenModules.Property.Color;
using Point = System.Windows.Point;

namespace VixenModules.Editor.EffectEditor
{
	/// <summary>
	///     Provides a wrapper around property value to be used at presentation level.
	/// </summary>
	public class PropertyItemValue : INotifyPropertyChanged, IDropTargetAdvisor, IDragSourceAdvisor, IDisposable
	{
		private readonly bool _hasSubProperties;
		private readonly PropertyItem _property;
		private readonly GridEntryCollection<PropertyItem> _subProperties = new GridEntryCollection<PropertyItem>();
		private readonly ObservableCollection<CollectionItemValue> _collectionItemValues = new ObservableCollection<CollectionItemValue>();
		private IList _collectionValues;

		#region ctor

		/// <summary>
		///     Initializes a new instance of the <see cref="PropertyItemValue" /> class.
		/// </summary>
		/// <param name="property">The property.</param>
		public PropertyItemValue(PropertyItem property)
		{
			if (property == null) throw new ArgumentNullException("property");
			_property = property;

			_hasSubProperties = property.Converter.GetPropertiesSupported();

			if (_hasSubProperties)
			{
				var value = property.GetValue();

				var descriptors = property.Converter.GetProperties(value);
				foreach (PropertyDescriptor d in descriptors)
				{
					_subProperties.Add(new PropertyItem(property.Owner, value, d));
					// TODO: Move to PropertyData as a public property
					var notifyParent =
						d.Attributes[KnownTypes.Attributes.NotifyParentPropertyAttribute] as NotifyParentPropertyAttribute;
					if (notifyParent != null && notifyParent.NotifyParent)
					{
						d.AddValueChanged(value, NotifySubPropertyChanged);
					}
				}
			}

			if (property.IsCollection)
			{
				LoadCollectionValues();
			}

			_property.PropertyChanged += ParentPropertyChanged;
		}

		

		#endregion

		private void LoadCollectionValues()
		{
			_collectionValues = _property.GetValue() as IList;
			CleanUpCollectionValues();
			_collectionItemValues.Clear();
			if (_collectionValues != null)
			{
				for (int i = 0; i < _collectionValues.Count; i++)
				{
					var collectionItemValue = new CollectionItemValue(this, i);
					_collectionItemValues.Add(collectionItemValue);
				}
			}
		}

		private void CleanUpCollectionValues()
		{
			foreach (var collectionItemValue in _collectionItemValues)
			{
				collectionItemValue?.Dispose();
			}
		}

		/// <summary>
		///     Gets the parent property.
		/// </summary>
		/// <value>The parent property.</value>
		public PropertyItem ParentProperty
		{
			get { return _property; }
		}

		public GridEntryCollection<PropertyItem> SubProperties
		{
			get { return _subProperties; }
		}

		internal ObservableCollection<CollectionItemValue> CollectionValues
		{
			get
			{
				return _collectionItemValues;
			}
		}

		public void AddItemToCollection()
		{
			if (!IsCollection) return;
			if (_collectionItemValues.Any())
			{
				object item;
				var oType = _collectionItemValues[0].ItemType;
				if (oType == typeof (string))
				{
					item = String.Empty;
				}
				else
				{
					item = Activator.CreateInstance(_collectionItemValues[0].ItemType);	
				}
				
				_property.AddCollectionValue(item);
			}
		}

		public void RemoveItemFromCollection(int index)
		{
			if (!IsCollection) return;
			if (_collectionItemValues.Any())
			{
				_property.RemoveCollectionValue(index);
			}
		}

		/// <summary>
		///     Gets a value indicating whether encapsulated value has sub-properties.
		/// </summary>
		/// <remarks>This property is reserved for future implementations.</remarks>
		/// <value>
		///     <c>true</c> if this instance has sub properties; otherwise, <c>false</c>.
		/// </value>
		public bool HasSubProperties
		{
			get { return _hasSubProperties; }
		}

		public UIElement TargetUI { get; set; }
		public bool ApplyMouseOffset { get; private set; }

		public bool IsValidDataObject(IDataObject obj)
		{
			if ((obj.GetDataPresent(typeof(Color)) || obj.GetDataPresent(typeof(ColorGradient))) && SupportsColor())
			{
				
				var discreteColors = Util.GetDiscreteColors(ParentProperty.Component);
				if (discreteColors.Any())
				{
					if (ParentProperty.PropertyType == typeof (Color) || obj.GetDataPresent(typeof (Color)))
					{
						var c = (Color) obj.GetData(typeof (Color));
						if (!discreteColors.Contains(c))
						{
							return false;
						}
					}
					else
					{
						var c = (ColorGradient)obj.GetData(typeof(ColorGradient));
						var colors = c.Colors.Select(x => x.Color.ToRGB().ToArgb());
						if (!discreteColors.IsSupersetOf(colors))
						{
							return false;
						}	
					}
				}
				
				return true;
			}

			if (obj.GetDataPresent((typeof(Curve))) && SupportsCurve())
			{
				return true;
			}

			return false;
		}

		private bool SupportsColor()
		{
			return SupportsColorGradient() || ParentProperty.PropertyType == typeof (Color);
		}

		private bool SupportsColorGradient()
		{
			return ParentProperty.PropertyType == typeof(ColorGradient);
		}

		private bool SupportsCurve()
		{
			return ParentProperty.PropertyType == typeof(Curve);
		}

		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			var data = obj.GetData(ParentProperty.PropertyType);
			if (data != null && data.GetType() == ParentProperty.PropertyType)
			{
				Value = data;
			}
			else
			{
				//Check to see if we are trying to assign color to a gradient
				data = obj.GetData(typeof (Color));
				if (data is Color && SupportsColorGradient())
				{
					Value = new ColorGradient((Color) data);
				}
			}
		}

		public UIElement GetVisualFeedback(IDataObject obj)
		{
			return null;
		}

		public UIElement SourceUI { get; set; }

		public DragDropEffects SupportedEffects
		{
			get
			{
				return DragDropEffects.Copy;
			} 
			
		}

		public DataObject GetDataObject(UIElement draggedElt)
		{
			return new DataObject(Value);
		}

		public void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
		{
			
		}

		public bool IsDraggable(UIElement dragElt)
		{
			if ( (SupportsCurve() || SupportsColor()) && Value!=null)
			{
				return true;	
			}
			return false;
		}

		public UIElement GetTopContainer()
		{
			return TargetUI;
		}

		private void ParentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "PropertyValue")
			{
				if (IsCollection)
				{
					LoadCollectionValues();
				}
				NotifyRootValueChanged();
			}

			if (e.PropertyName == "IsReadOnly")
			{
				OnPropertyChanged("IsReadOnly");
				OnPropertyChanged("IsEditable");
			}
		}

		/// <summary>
		///     Gets the serialization culture.
		/// </summary>
		/// <returns>Culture to serialize value.</returns>
		protected virtual CultureInfo GetSerializationCulture()
		{
			return ObjectServices.GetSerializationCulture(_property.PropertyType);
		}

		#region Events

		/// <summary>
		///     Occurs when exception is raised at Property Value.
		///     <remarks>This event is reserved for future implementations.</remarks>
		/// </summary>
		public event EventHandler<ValueExceptionEventArgs> PropertyValueException;

		/// <summary>
		///     Occurs when root value is changed.
		///     <remarks>This event is reserved for future implementations.</remarks>
		/// </summary>
		public event EventHandler RootValueChanged;

		/// <summary>
		///     Occurs when sub property changed.
		/// </summary>
		public event EventHandler SubPropertyChanged;

		/// <summary>
		///     Occurs when Collection value changed.
		/// </summary>
		public event EventHandler CollectionValueChanged;

		#endregion

		#region PropertyValue implementation

		/// <summary>
		///     Gets a value indicating whether this instance can convert from string.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance can convert from string; otherwise, <c>false</c>.
		/// </value>
		public bool CanConvertFromString
		{
			get
			{
				return (((_property.Converter != null) && _property.Converter.CanConvertFrom(typeof (string))) &&
				        !_property.IsReadOnly);
			}
		}

		/// <summary>
		///     Clears the value.
		/// </summary>
		public void ClearValue()
		{
			_property.ClearValue();
		}

		/// <summary>
		///     Converts the string to value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Value instance</returns>
		protected object ConvertStringToValue(string value)
		{
			if (_property.PropertyType == typeof (string)) return value;
			//if (value.Length == 0) return null;
			if (string.IsNullOrEmpty(value)) return null;
			if (!_property.Converter.CanConvertFrom(typeof (string)))
				throw new InvalidOperationException("Value to String conversion is not supported!");
			return _property.Converter.ConvertFromString(_property, GetSerializationCulture(), value);
		}

		/// <summary>
		///     Converts the value to string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>String presentation of the value</returns>
		protected string ConvertValueToString(object value)
		{
			var collectionValue = string.Empty;
			if (value == null) return collectionValue;

			collectionValue = value as string;
			if (collectionValue != null) return collectionValue;

			var converter = _property.Converter;
			if (converter.CanConvertTo(typeof (string)))
				collectionValue = converter.ConvertToString(_property, GetSerializationCulture(), value);
			else
				collectionValue = value.ToString();

			// TODO: refer to resources or some constant
			if (string.IsNullOrEmpty(collectionValue) && (value is IEnumerable))
				collectionValue = "(Collection)";

			return collectionValue;
		}

		/// <summary>
		///     Gets the value.
		/// </summary>
		/// <returns>Property value</returns>
		protected object GetValueCore()
		{
			return _property.GetValue();
		}

		/// <summary>
		///     Gets a value indicating whether encapsulated property value is collection.
		/// </summary>
		/// <value>
		///     <c>true</c> if encapsulated property value is collection; otherwise, <c>false</c>.
		/// </value>
		public bool IsCollection
		{
			get { return _property.IsCollection; }
		}

		/// <summary>
		///     Gets a value indicating whether encapsulated property value is default value.
		/// </summary>
		/// <value>
		///     <c>true</c> if encapsulated property value is default value; otherwise, <c>false</c>.
		/// </value>
		public bool IsDefaultValue
		{
			get { return _property.IsDefaultValue; }
		}

		/// <summary>
		///     Sets the value.
		/// </summary>
		/// <param name="value">The value.</param>
		protected void SetValueCore(object value)
		{
			_property.SetValue(value);
		}

		// TODO: DependencyProperty validation should be placed here
		/// <summary>
		///     Validates the value.
		/// </summary>
		/// <param name="valueToValidate">The value to validate.</param>
		protected void ValidateValue(object valueToValidate)
		{
			//throw new NotImplementedException();
			// Do nothing            
		}

		private void SetValueImpl(object value)
		{
			//this.ValidateValue(value);
			if (ParentProperty.Validate(value))
				SetValueCore(value);

			NotifyValueChanged();
			OnRootValueChanged();
		}

		/// <summary>
		///     Raises the <see cref="PropertyValueException" /> event.
		/// </summary>
		/// <param name="e">The <see cref="ValueExceptionEventArgs" /> instance containing the event data.</param>
		protected virtual void OnPropertyValueException(ValueExceptionEventArgs e)
		{
			if (e == null) throw new ArgumentNullException("e");
			if (PropertyValueException != null) PropertyValueException(this, e);
		}

		/// <summary>
		///     Gets a value indicating whether exceptions should be cought.
		/// </summary>
		/// <value><c>true</c> if expceptions should be cought; otherwise, <c>false</c>.</value>
		protected virtual bool CatchExceptions
		{
			get { return (PropertyValueException != null); }
		}

		/// <summary>
		///     Gets or sets the string representation of the value.
		/// </summary>
		/// <value>The string value.</value>
		public string StringValue
		{
			get
			{
				var str = string.Empty;
				if (CatchExceptions)
				{
					try
					{
						str = ConvertValueToString(Value);
					}
					catch (Exception exception)
					{
						OnPropertyValueException(new ValueExceptionEventArgs("Cannot convert value to string", this,
							ValueExceptionSource.Get, exception));
					}
					return str;
				}
				return ConvertValueToString(Value);
			}
			set
			{
				if (CatchExceptions)
				{
					try
					{
						Value = ConvertStringToValue(value);
					}
					catch (Exception exception)
					{
						OnPropertyValueException(new ValueExceptionEventArgs("Cannot create value from string", this,
							ValueExceptionSource.Set, exception));
					}
				}
				else
				{
					Value = ConvertStringToValue(value);
				}
			}
		}

		/// <summary>
		///     Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value
		{
			get
			{
				object valueCore = null;
				if (CatchExceptions)
				{
					try
					{
						valueCore = GetValueCore();
					}
					catch (Exception exception)
					{
						OnPropertyValueException(new ValueExceptionEventArgs("Value Get Failed", this, ValueExceptionSource.Get, exception));
					}
					return valueCore;
				}
				return GetValueCore();
			}
			set
			{
				if (CatchExceptions)
				{
					try
					{
						SetValueImpl(value);
					}
					catch (Exception exception)
					{
						OnPropertyValueException(new ValueExceptionEventArgs("Value Set Failed", this, ValueExceptionSource.Set, exception));
					}
				}
				else
				{
					SetValueImpl(value);
				}
			}
		}

		public object GetCollectionValue(int index)
		{
			return _collectionValues[index];
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
			_property.SetCollectionValue(value, index);
			NotifyCollectionValueChanged(index);
		}

		#endregion

		#region Helper properties

		/// <summary>
		///     Gets a value indicating whether encapsulated property value is read only.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsReadOnly
		{
			get { return _property.IsReadOnly; }
		}

		/// <summary>
		///     Gets a value indicating whether encapsulated property value is editable.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is editable; otherwise, <c>false</c>.
		/// </value>
		public bool IsEditable
		{
			get { return !_property.IsReadOnly; }
		}

		#endregion

		#region INotifyPropertyChanged Members

		/// <summary>
		///     Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///     Called when property value is changed.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		///     Notifies the root value changed.
		/// </summary>
		protected virtual void NotifyRootValueChanged()
		{
			
			OnPropertyChanged("IsDefaultValue");
			OnPropertyChanged("IsMixedValue");
			OnPropertyChanged("IsCollection");
			OnPropertyChanged("Collection");
			OnPropertyChanged("HasSubProperties");
			OnPropertyChanged("SubProperties");
			OnPropertyChanged("Source");
			OnPropertyChanged("CanConvertFromString");
			NotifyValueChanged();
			OnRootValueChanged();
		}

		private void NotifyStringValueChanged()
		{
			OnPropertyChanged("StringValue");
		}

		/// <summary>
		///     Notifies the sub property changed.
		/// </summary>
		protected void NotifySubPropertyChanged(object sender, EventArgs args)
		{
			NotifyValueChanged();
			OnSubPropertyChanged();
		}

		/// <summary>
		///     Notifies a collection value changed.
		/// </summary>
		protected void NotifyCollectionValueChanged(int index)
		{
			NotifyValueChanged();
			OnRootValueChanged();
			OnCollectionValueChanged(index);
		}

		private void NotifyValueChanged()
		{
			OnPropertyChanged("Value");
			NotifyStringValueChanged();
		}

		private void OnRootValueChanged()
		{
			var handler = RootValueChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		private void OnSubPropertyChanged()
		{
			var handler = SubPropertyChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		private void OnCollectionValueChanged(int index)
		{
			var handler = CollectionValueChanged;
			if (handler != null) handler(this, new CollectionChangedEventArgs(index));
		}


		#endregion

		#region IDisposable

		/// <inheritdoc />
		public void Dispose()
		{
			CleanUpCollectionValues();
			if (_property != null)
			{
				_property.PropertyChanged -= ParentPropertyChanged;
			}

			if (_hasSubProperties)
			{
				foreach (var propertyItem in SubProperties)
				{
					propertyItem?.Dispose();
				}
			}
		}

		#endregion
	}
}