using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using NLog;
using Vixen.Attributes;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Editor.EffectEditor.Input;
using VixenModules.Editor.EffectEditor.PropertyEditing;
using Point = System.Windows.Point;

namespace VixenModules.Editor.EffectEditor.Internal
{
	internal class CollectionItemValue : INotifyPropertyChanged, IDropTargetAdvisor, IDragSourceAdvisor, IDisposable
	{
		private readonly PropertyItemValue _propertyItemValue;
		private readonly int _index;
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		
		public CollectionItemValue(PropertyItemValue propertyItemValue, int index)
		{
			_propertyItemValue = propertyItemValue;
			_index = index;

			var expandable = PropertyGridUtils.GetAttributes<ExpandableObjectAttribute>(Value);
			if (expandable.Any())
			{
				var descriptors = MetadataRepository.GetProperties(Value).Select(prop => prop.Descriptor);

				if (descriptors.Any())
				{
					object objectValue;
					if (Value is ICloneable valueToClone)
					{
						objectValue = valueToClone.Clone();
					}
					else
					{
						objectValue = Value;
					}


					HasSubProperties = true;
					
					var properties = new GridEntryCollection<PropertyItem>();
					foreach (PropertyDescriptor d in descriptors)
					{
						var item = new PropertyItem(_propertyItemValue.ParentProperty.Owner, objectValue, d);
						item.IsBrowsable = ShouldDisplayProperty(d);
						item.ValueChanged += ItemOnValueChanged;
						properties.Add(item);
					}

					if (_propertyItemValue.ParentProperty.Owner.PropertyComparer != null)
					{
						properties.Sort(_propertyItemValue.ParentProperty.Owner.PropertyComparer);
					}

					SubProperties = properties;
				}

				MetadataRepository.Remove(Value);
			}
			
		}

		private static bool ShouldDisplayProperty(PropertyDescriptor propertyDescriptor)
		{
			if (propertyDescriptor == null) return false;

			// Return default/standard Browsable settings for the property
			return propertyDescriptor.IsBrowsable;
		}
		/// <summary>
		///     Converts the string to value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Value instance</returns>
		protected object ConvertStringToValue(string value)
		{
			if (_propertyItemValue.CollectionItemType == typeof(string)) return value;
			
			if (string.IsNullOrEmpty(value)) return null;
			if (!_propertyItemValue.CollectionItemConverter.CanConvertFrom(typeof(string)))
				throw new InvalidOperationException("Value to String conversion is not supported!");
			return _propertyItemValue.CollectionItemConverter.ConvertFromString(null, GetSerializationCulture(), value);
		}

		/// <summary>
		///     Gets the parent property.
		/// </summary>
		/// <value>The parent property.</value>
		public PropertyItem ParentProperty
		{
			get { return _propertyItemValue.ParentProperty; }
		}

		public Type ItemType
		{
			get { return _propertyItemValue.CollectionItemType; }
		}

		/// <summary>
		///     Gets the parent property.
		/// </summary>
		/// <value>The parent property.</value>
		public PropertyItem PropertyItem { get; protected set; }

		public GridEntryCollection<PropertyItem> SubProperties { get; }
		

		/// <summary>
		///     Gets a value indicating whether encapsulated value has sub-properties.
		/// </summary>
		/// <remarks>This property is reserved for future implementations.</remarks>
		/// <value>
		///     <c>true</c> if this instance has sub properties; otherwise, <c>false</c>.
		/// </value>
		public bool HasSubProperties { get; }
		


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

			var converter = TypeDescriptor.GetConverter(value);
			if (converter.CanConvertTo(typeof(string)))
				collectionValue = converter.ConvertToString(null, GetSerializationCulture(), value);
			else
				collectionValue = value.ToString();

			// TODO: refer to resources or some constant
			if (string.IsNullOrEmpty(collectionValue) && (value is IEnumerable))
				collectionValue = "(Collection)";

			return collectionValue;
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

				try
				{
					str = ConvertValueToString(Value);
				}
				catch (Exception exception)
				{
					Logging.Error(exception, "Get: Unable to convert property to string.");
				}
				return str;

			}
			set
			{
				try
				{
					Value = ConvertStringToValue(value);
				}
				catch (Exception exception)
				{
					Logging.Error(exception,"Set: Unable to convert property to string.");
				}
			}
		}

		/// <summary>
		///     Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value
		{
			get { return _propertyItemValue.GetCollectionValue(_index); }
			set
			{

				try
				{
					SetValueImpl(value);
				}
				catch (Exception exception)
				{
					Logging.Error(exception, "Unable to set property value.");
				}

			}
		}

		private void SetValueImpl(object value)
		{
			SetValueCore(value);
			NotifyValueChanged();
		}

		/// <summary>
		///     Sets the value.
		/// </summary>
		/// <param name="value">The value.</param>
		protected void SetValueCore(object value)
		{
			_propertyItemValue.SetCollectionValue(value, _index);
		}

		/// <summary>
		///     Gets the serialization culture.
		/// </summary>
		/// <returns>Culture to serialize value.</returns>
		protected virtual CultureInfo GetSerializationCulture()
		{
			return ObjectServices.GetSerializationCulture(_propertyItemValue.CollectionItemType);
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

		public event EventHandler SubPropertyChanged;

		#endregion

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ItemOnValueChanged(PropertyItem arg1, object[] arg2, object arg3)
		{
			_propertyItemValue.SetCollectionValue(arg1.Component, _index);
		}

		private void NotifyStringValueChanged()
		{
			OnPropertyChanged("StringValue");
		}

		private void NotifyValueChanged()
		{
			OnPropertyChanged("Value");
			NotifyStringValueChanged();
		}

		protected void NotifySubPropertyChanged(object sender, EventArgs args)
		{

			NotifyValueChanged();
			
		}

		#endregion

		public UIElement TargetUI { get; set; }
		public bool ApplyMouseOffset { get; private set; }
		public bool IsValidDataObject(IDataObject obj)
		{
			if ((obj.GetDataPresent(typeof (Color)) || obj.GetDataPresent(typeof (ColorGradient))) && SupportsColor())
			{
				var discreteColors = Util.GetDiscreteColors(ParentProperty.Component);
				if (discreteColors.Any())
				{
					if (obj.GetDataPresent(typeof (Color)))
					{
						var c = (Color) obj.GetData(typeof (Color));
						if (!discreteColors.Contains(c))
						{
							return false;
						}
					}
					else
					{
						var c = (ColorGradient) obj.GetData(typeof (ColorGradient));
						var colors = c.Colors.Select(x => x.Color.ToRGB().ToArgb());
						if (!discreteColors.IsSupersetOf(colors))
						{
							return false;
						}
					}
				}

				return true;
			}
			
			if (obj.GetDataPresent((typeof (Curve))) && SupportsCurve())
			{
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
			bool supported = ItemType == typeof(ColorGradient) || ItemType == typeof(Color) || (ItemType == typeof(GradientLevelPair) && DragDropTargetType.GetTargetType(TargetUI).Equals("ColorGradient"));

			return supported;
		}

		private bool SupportsColorGradient()
		{
			bool supported = ItemType == typeof(ColorGradient) || (ItemType == typeof(GradientLevelPair) && DragDropTargetType.GetTargetType(TargetUI).Equals("ColorGradient"));

			return supported;
		}

		private bool SupportsCurve()
		{
			bool supported = ItemType == typeof(Curve) || (ItemType == typeof(GradientLevelPair) && DragDropTargetType.GetTargetType(TargetUI).Equals("Curve"));

			return supported;
		}

		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			var data = obj.GetData(ItemType);
			if (data != null && data.GetType() == ItemType)
			{
				Value = data;
			}
				//Our type does not match, so either we are applying Color to a Gradient or something to a GradientLevelPair
			else if (obj.GetDataPresent(typeof(Color)))
			{
				data = obj.GetData(typeof(Color));
				HandleColorDrop((Color)data);
			}
			else if (obj.GetDataPresent(typeof (ColorGradient)) && ItemType == typeof(GradientLevelPair))
			{
				var cg = obj.GetData(typeof (ColorGradient)) as ColorGradient;
				var glp = (GradientLevelPair) Value;
				var newGradientLevelPair = new GradientLevelPair(cg, glp.Curve);
				Value = newGradientLevelPair;
			}
			else if (obj.GetDataPresent(typeof(Curve)) && ItemType == typeof(GradientLevelPair))
			{
				var c = obj.GetData(typeof(Curve)) as Curve;
				var glp = (GradientLevelPair)Value;
				var newGradientLevelPair = new GradientLevelPair(glp.ColorGradient, c);
				Value = newGradientLevelPair;
			}
		}

		private void HandleColorDrop(Color c)
		{
			if (ItemType == typeof (ColorGradient))
			{
				Value = new ColorGradient(c);
			}
			else if (ItemType == typeof (GradientLevelPair))
			{
				var glp = (GradientLevelPair) Value;
				var newGradientLevelPair = new GradientLevelPair(new ColorGradient(c), glp.Curve);
				Value = newGradientLevelPair;
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
			if (ItemType == typeof (GradientLevelPair))
			{
				var item = (GradientLevelPair) Value;
				if (DragDropTargetType.GetTargetType(SourceUI).Equals("Curve"))
				{
					return new DataObject(item.Curve);
				}
				return new DataObject(item.ColorGradient);
			}
			return new DataObject(Value);
		}

		public void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
		{
			
		}

		public bool IsDraggable(UIElement dragElt)
		{
			if (ItemType == typeof(Curve) || ItemType == typeof(ColorGradient)
				|| ItemType == typeof(Color) || ItemType == typeof(GradientLevelPair))
			{
				return true;
			}
			return false;
		}

		UIElement IDragSourceAdvisor.GetTopContainer()
		{
			return TargetUI;
		}

		UIElement IDropTargetAdvisor.GetTopContainer()
		{
			return TargetUI;
		}

		#region IDisposable

		/// <inheritdoc />
		public void Dispose()
		{
			if (SubProperties != null)
			{
				foreach (var propertyItem in SubProperties)
				{
					propertyItem.ValueChanged -= ItemOnValueChanged;
					propertyItem.Dispose();
				}
			}
		}

		#endregion
	}
}
