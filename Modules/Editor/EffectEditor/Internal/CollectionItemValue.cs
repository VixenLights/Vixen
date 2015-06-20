using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Editor.EffectEditor.Input;
using VixenModules.Editor.EffectEditor.PropertyEditing;
using Point = System.Windows.Point;

namespace VixenModules.Editor.EffectEditor.Internal
{
	internal class CollectionItemValue : INotifyPropertyChanged, IDropTargetAdvisor, IDragSourceAdvisor
	{
		private readonly PropertyItemValue _propertyItemValue;
		private readonly int _index;
		public CollectionItemValue(PropertyItemValue propertyItemValue, int index)
		{
			_propertyItemValue = propertyItemValue;
			_index = index;
		}
		/// <summary>
		///     Converts the string to value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Value instance</returns>
		protected object ConvertStringToValue(string value)
		{
			if (_propertyItemValue.CollectionItemType == typeof(string)) return value;
			//if (value.Length == 0) return null;
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
					//OnPropertyValueException(new ValueExceptionEventArgs("Cannot create value from string", this,
					//	ValueExceptionSource.Set, exception));
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
					//OnPropertyValueException(new ValueExceptionEventArgs("Value Set Failed", this, ValueExceptionSource.Set, exception));
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

		#endregion

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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

		#endregion
		public UIElement TargetUI { get; set; }
		public bool ApplyMouseOffset { get; private set; }
		public bool IsValidDataObject(IDataObject obj)
		{
			if (obj.GetDataPresent(ItemType) ||
				ItemType == typeof(ColorGradient) && obj.GetDataPresent(typeof(Color)))
			{

				var discreteColors = Util.GetDiscreteColors(ParentProperty.Component);
				if (discreteColors.Any())
				{
					if (ItemType == typeof(Color) || obj.GetDataPresent(typeof(Color)))
					{
						var c = (Color)obj.GetData(typeof(Color));
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

			return false;
		}

		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			var data = obj.GetData(ItemType);
			if (data != null && data.GetType() == ItemType)
			{
				Value = data;
			}
			else
			{
				//Check to see if we are trying to assign color to a gradient
				data = obj.GetData(typeof(Color));
				if (data is Color && ItemType == typeof(ColorGradient))
				{
					Value = new ColorGradient((Color)data);
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
			if (ItemType == typeof(Curve) || ItemType == typeof(ColorGradient)
				|| ItemType == typeof(Color))
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
	}
}
