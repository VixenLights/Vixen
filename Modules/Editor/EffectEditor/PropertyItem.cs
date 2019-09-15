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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Editor.EffectEditor.Internal;
using VixenModules.Editor.EffectEditor.Metadata;
using VixenModules.Editor.EffectEditor.PropertyEditing.Filters;

namespace VixenModules.Editor.EffectEditor
{
	/// <summary>
	///     Defines a wrapper around object property to be used at presentation level.
	/// </summary>
	public class PropertyItem : GridEntry, IPropertyFilterTarget
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private readonly PropertyItemValue _parentValue;
		private PropertyItemValue _value;

		#region ParentValue

		// TODO: Reserved for future implementations.
		/// <summary>
		///     Gets the parent value.
		///     <remarks>This property is reserved for future implementations</remarks>
		/// </summary>
		/// <value>The parent value.</value>
		public PropertyItemValue ParentValue
		{
			get { return _parentValue; }
		}

		#endregion

		/// <summary>
		///     Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing">
		///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
		///     unmanaged resources.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				if (disposing)
				{
					_descriptor.RemoveValueChanged(_component, ComponentValueChanged);
					_value?.Dispose();
					
				}
				base.Dispose(disposing);
			}
		}

		#region Public helpers

		/// <summary>
		///     Gets the attribute bound to property.
		/// </summary>
		/// <typeparam name="T">Attribute type to look for</typeparam>
		/// <returns>Attribute bound to property or null.</returns>
		public virtual T GetAttribute<T>() where T : Attribute
		{
			if (Attributes == null) return null;
			return Attributes[typeof (T)] as T;
		}

		#endregion

		#region Fields

		private readonly object _component;
		private object _unwrappedComponent;
		private PropertyDescriptor _descriptor;

		#endregion

		#region Filtering API

		/// <summary>
		///     Applies the filter for the entry.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public override void ApplyFilter(PropertyFilter filter)
		{
			MatchesFilter = (filter == null) || filter.Match(this);
			OnFilterApplied(filter);
		}

		/// <summary>
		///     Checks whether the entry matches the filtering predicate.
		/// </summary>
		/// <param name="predicate">The filtering predicate.</param>
		/// <returns>
		///     <c>true</c> if entry matches predicate; otherwise, <c>false</c>.
		/// </returns>
		public override bool MatchesPredicate(PropertyFilterPredicate predicate)
		{
			if (predicate == null) return false;
			if (!predicate.Match(DisplayName))
			{
				return (PropertyType != null)
					? predicate.Match(PropertyType.Name)
					: false;
			}
			return true;
		}

		#endregion

		#region PropertyValue

		/// <summary>
		///     Gets the property value.
		/// </summary>
		/// <value>The property value.</value>
		public PropertyItemValue PropertyValue
		{
			get
			{
				if (_value == null) _value = CreatePropertyValueInstance();
				return _value;
			}
		}

		/// <summary>
		///     Creates the property value instance.
		/// </summary>
		/// <returns>A new instance of <see cref="PropertyItemValue" />.</returns>
		protected PropertyItemValue CreatePropertyValueInstance()
		{
			return new PropertyItemValue(this);
		}

		#endregion

		#region PropertyDescriptor

		/// <summary>
		///     Gets PropertyDescriptor instance for the underlying property.
		/// </summary>
		public override PropertyDescriptor PropertyDescriptor
		{
			get { return _descriptor; }
		}

		/// <summary>
		///     Gets PropertyDescriptor instance for the underlying property.
		/// </summary>
		public PropertyDescriptor UnderLyingPropertyDescriptor(int index)
		{
			if (_descriptor is MergedPropertyDescriptor)
			{
				return ((MergedPropertyDescriptor) _descriptor)[index];
			}
			return _descriptor;
		}

		#endregion

		#region ITypeDescriptorContext

		public override object GetService(Type serviceType)
		{
			if (serviceType == typeof (PropertyItem))
				return this;
			return null;
		}

		public override bool OnComponentChanging()
		{
			throw new NotSupportedException();
		}

		public override void OnComponentChanged()
		{
			//If our parent component changed, our standard values may have changed and we need to refresh them.
			OnPropertyChanged("StandardValues");
		}

		public override IContainer Container
		{
			get { throw new NotSupportedException(); }
		}

		public override object Instance
		{
			get { return UnwrappedComponent; }
		}

		#endregion

		#region ctor/init

		/// <summary>
		///     Initializes a new instance of the <see cref="PropertyItem" /> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="component">The component property belongs to.</param>
		/// <param name="descriptor">The property descriptor</param>
		public PropertyItem(EffectPropertyEditorGrid owner, object component, PropertyDescriptor descriptor)
			: this(null)
		{
			if (owner == null) throw new ArgumentNullException("owner");
			if (component == null) throw new ArgumentNullException("component");
			if (descriptor == null) throw new ArgumentNullException("descriptor");

			Owner = owner;
			Name = descriptor.Name;
			_component = component;
			_descriptor = descriptor;

			IsBrowsable = descriptor.IsBrowsable;
			_isReadOnly = descriptor.IsReadOnly;
			_description = descriptor.Description;
			CategoryName = descriptor.Category;
			IsLocalizable = descriptor.IsLocalizable;

			Metadata = new AttributesContainer(descriptor.Attributes);
			_descriptor.AddValueChanged(component, ComponentValueChanged);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="PropertyItem" /> class.
		/// </summary>
		/// <param name="parentValue">The parent value.</param>
		protected PropertyItem(PropertyItemValue parentValue)
		{
			_parentValue = parentValue;
		}

		private void ComponentValueChanged(object sender, EventArgs e)
		{
			OnPropertyChanged("PropertyValue");
			if (Name.Equals("TargetNodes"))
			{
				Owner.ComponentChanged();
			}
		}

		#endregion

		#region Events

		/// <summary>
		///     Occurs when property value is changed.
		/// </summary>
		public event Action<PropertyItem, object[], object> ValueChanged;

		private void OnValueChanged(object[] oldValue, object newValue)
		{
			var handler = ValueChanged;
			if (handler != null)
				handler(this, oldValue, newValue);
		}

		#endregion

		#region Properties

		#region DisplayName

		private string _displayName;

		/// <summary>
		///     Gets the display name for the property.
		/// </summary>
		/// <value></value>
		/// <returns>
		///     The display name for the property.
		/// </returns>
		public string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(_displayName))
					_displayName = GetDisplayName();

				return _displayName;
			}
			set
			{
				if (_displayName == value) return;
				_displayName = value;
				OnPropertyChanged("DisplayName");
			}
		}

		#endregion

		#region CategoryName

		/// <summary>
		///     Gets the name of the category that this property resides in.
		/// </summary>
		/// <value></value>
		/// <returns>
		///     The name of the category that this property resides in.
		/// </returns>
		public string CategoryName { get; private set; }

		#endregion

		#region Description

		private string _description;

		/// <summary>
		///     Gets the description of the encapsulated property.
		/// </summary>
		/// <value></value>
		/// <returns>
		///     The description of the encapsulated property.
		/// </returns>
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description == value) return;
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		#endregion

		#region IsAdvanced

		/// <summary>
		///     Gets a value indicating whether the encapsulated property is an advanced property.
		/// </summary>
		/// <returns>true if the encapsulated property is an advanced property; otherwise, false.</returns>
		// TODO: move intilialization to ctor
		public bool IsAdvanced
		{
			get
			{
				var browsable = (EditorBrowsableAttribute) Attributes[typeof (EditorBrowsableAttribute)];
				return browsable != null && browsable.State == EditorBrowsableState.Advanced;
			}
		}

		#endregion

		#region IsLocalizable

		/// <summary>
		///     Gets a value indicating whether the encapsulated property is localizable.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is localizable; otherwise, <c>false</c>.
		/// </value>
		public bool IsLocalizable { get; private set; }

		#endregion

		#region IsReadOnly

		private bool _isReadOnly;

		/// <summary>
		///     Gets a value indicating whether the encapsulated property is read-only.
		/// </summary>
		/// <value></value>
		/// <returns>
		///     true if the encapsulated property is read-only; otherwise, false.
		/// </returns>
		public bool IsReadOnly
		{
			get { return _isReadOnly; }
			set
			{
				if (_isReadOnly == value) return;
				_isReadOnly = value;
				OnPropertyChanged("IsReadOnly");
			}
		}

		#endregion

		#region PropertyType

		/// <summary>
		///     Gets the type of the encapsulated property.
		/// </summary>
		/// <value></value>
		/// <returns>
		///     The type of the encapsulated property.
		/// </returns>
		public virtual Type PropertyType
		{
			get
			{
				if (_descriptor == null) return null;
				return _descriptor.PropertyType;
			}
		}

		#endregion

		#region StandardValues

		/// <summary>
		///     Gets the standard values that the encapsulated property supports.
		/// </summary>
		/// <value></value>
		/// <returns>
		///     A <see cref="T:System.Collections.ICollection" /> of standard values that the encapsulated property supports.
		/// </returns>
		public ICollection StandardValues
		{
			get
			{
				if (Converter.GetStandardValuesSupported())
					return Converter.GetStandardValues(this);

				return new ArrayList(0);
			}
		}

		#endregion

		#region Component

		/// <summary>
		///     Gets the component the property belongs to.
		/// </summary>
		/// <value>The component.</value>
		public object Component
		{
			get { return _component; }
		}

		#endregion

		#region UnwrappedComponent

		/// <summary>
		///     Gets the component the property belongs to.
		/// </summary>
		/// <remarks>
		///     This property returns a real unwrapped component even if a custom type descriptor is used.
		/// </remarks>
		public object UnwrappedComponent
		{
			get { return _unwrappedComponent ?? (_unwrappedComponent = ObjectServices.GetUnwrappedObject(_component)); }
		}

		#endregion

		#region ToolTip

		/// <summary>
		///     Gets the tool tip.
		/// </summary>
		/// <value>The tool tip.</value>
		public object ToolTip
		{
			get
			{
				var attribute = GetAttribute<DescriptionAttribute>();
				return (attribute != null && !string.IsNullOrEmpty(attribute.Description))
					? attribute.Description
					: DisplayName;
			}
		}

		#endregion

		#region Attributes

		/// <summary>
		///     Gets the custom attributes bound to property.
		/// </summary>
		/// <value>The attributes.</value>
		public virtual AttributeCollection Attributes
		{
			get
			{
				if (_descriptor == null) return null;
				return _descriptor.Attributes;
			}
		}

		#endregion

		#region Metadata

		/// <summary>
		///     Gets the custom attributes container.
		/// </summary>
		/// <value>The custom attributes container.</value>
		public AttributesContainer Metadata { get; private set; }

		#endregion

		#region Converter

		/// <summary>
		///     Gets the converter.
		/// </summary>
		/// <value>The converter.</value>
		public TypeConverter Converter
		{
			get { return ObjectServices.GetPropertyConverter(_descriptor); }
		}

		#endregion

		#region CanClearValue

		/// <summary>
		///     Gets a value indicating whether this instance can clear value.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance can clear value; otherwise, <c>false</c>.
		/// </value>
		public bool CanClearValue
		{
			get { return _descriptor.CanResetValue(_component); }
		}

		#endregion

		#region IsDefaultValue

		// TODO: support this (UI should also react on it)
		/// <summary>
		///     Gets a value indicating whether this instance is default value.
		///     <remarks>This property is reserved for future implementations.</remarks>
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is default value; otherwise, <c>false</c>.
		/// </value>
		public bool IsDefaultValue
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IsCollection

		/// <summary>
		///     Gets a value indicating whether this instance is collection.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is collection; otherwise, <c>false</c>.
		/// </value>
		public bool IsCollection
		{
			get { return typeof (IList).IsAssignableFrom(PropertyType); }
		}

		#endregion

		#endregion

		#region Methods

		public void RefreshDescriptors(PropertyDescriptor descriptor)
		{
			Name = descriptor.Name;
			_descriptor = descriptor;

			IsBrowsable = descriptor.IsBrowsable;
			_isReadOnly = descriptor.IsReadOnly;
			_description = descriptor.Description;
			CategoryName = descriptor.Category;
			IsLocalizable = descriptor.IsLocalizable;

			Metadata = new AttributesContainer(descriptor.Attributes);
			_descriptor.AddValueChanged(Component, ComponentValueChanged);
		}

		/// <summary>
		///     Clears the value.
		/// </summary>
		public void ClearValue()
		{
			if (!CanClearValue) return;

			var oldValue = GetValue();
			_descriptor.ResetValue(_component);
			OnValueChanged(new[] {oldValue}, GetValue());
			OnPropertyChanged("PropertyValue");
		}

		/// <summary>
		///     Gets the value.
		/// </summary>
		/// <returns>Property value</returns>
		public object GetValue()
		{
			if (_descriptor == null) return null;
			var target = GetViaCustomTypeDescriptor(_component, _descriptor);
			return _descriptor.GetValue(target);
		}

		public object[] GetValues()
		{
			if (!(_descriptor is MergedPropertyDescriptor)) return null;
			var target = GetViaCustomTypeDescriptor(_component, _descriptor);
			return ((MergedPropertyDescriptor) _descriptor).GetValues((Array) target);
		}

		private void SetValueCore(object value)
		{
			if (_descriptor == null) return;

			// Check whether underlying dependency property passes validation
			if (!IsValidDependencyPropertyValue(_descriptor, value))
			{
				OnPropertyChanged("PropertyValue");
				return;
			}

			var target = GetViaCustomTypeDescriptor(_component, _descriptor);

			if (target != null)
				_descriptor.SetValue(target, value);
		}

		internal void SetCollectionValue(object value, int index)
		{
			if (!IsCollection || IsReadOnly) return;
			//TODO Implement some flavor of generic equals on the collection as some point
			var collectionValue = (IList)GetValue();
			var oldValue = CloneValues();
			if (collectionValue != null)
			{
				
				if (collectionValue[index].Equals(value))
				{
					//old and new values are the same
					return;
				}

				collectionValue[index] = value;
				SetValueCore(collectionValue);
				OnValueChanged(oldValue, GetValue());
				OnPropertyChanged("PropertyValue");
			}

		}

		internal object[] CloneValues()
		{
			if (GetValues() == null)
			{
				return new object[] { CreateList(PropertyType, GetValue() as IList)};
			}

			var multiValues = GetValues();
			var cloneValues = new object[multiValues.Length];
			for (int i = 0; i < multiValues.Length; i++)
			{
				cloneValues[i] = CreateList(PropertyType, multiValues[i] as IList);
			}

			return cloneValues;
		}

		internal void AddCollectionValue(object value)
		{
			if (!IsCollection || IsReadOnly) return;
			//TODO Implement some flavor of generic equals on the collection as some point
			var collectionValue = (IList)GetValue();
			if (collectionValue != null)
			{
				var oldValue = CloneValues();
				collectionValue.Add(value);
				SetValueCore(collectionValue);
				OnValueChanged(oldValue, GetValue());
				OnPropertyChanged("PropertyValue");
			}

		}

		internal void RemoveCollectionValue(int index)
		{
			if (!IsCollection || IsReadOnly) return;
			var collectionValue = (IList)GetValue();
			if (collectionValue != null)
			{
				var oldValue = CloneValues();
				collectionValue.RemoveAt(index);
				SetValueCore(collectionValue);
				OnValueChanged(oldValue, GetValue());
				OnPropertyChanged("PropertyValue");
			}

		}

		private IList CreateList(Type t, IList values)
		{
			var instance = (IList)Activator.CreateInstance(t);
			if (values != null)
			{
				foreach (var value in values)
				{
					instance.Add(value);
				}
			}
			return instance;
		}

		/// <summary>
		///     Sets the value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void SetValue(object value)
		{
			// Check whether the property is not readonly
			if (IsReadOnly) return;

			var oldValue = GetValue();
			var oldValues = GetValues();

			try
			{
				if ((value == null && oldValue == null) || value != null && value.Equals(oldValue)) return;

				if (PropertyType == typeof (object) ||
				    value == null && PropertyType.IsClass ||
				    value != null && PropertyType.IsAssignableFrom(value.GetType()))
				{
					SetValueCore(value);
				}
				else
				{
					var convertedValue = Converter.ConvertFrom(value);
					SetValueCore(convertedValue);
				}
				if (oldValue != null && oldValues == null)
				{
					OnValueChanged(new[] {oldValue}, GetValue());
				}
				else
				{
					OnValueChanged(oldValues, GetValue());
				}
			}
			catch(Exception e)
			{
				Logging.Error(e, "An error occurred setting the property value");	
			}
			OnPropertyChanged("PropertyValue");
		}

		#endregion

		#region Private helpers

		private static object GetViaCustomTypeDescriptor(object obj, PropertyDescriptor descriptor)
		{
			var customTypeDescriptor = obj as ICustomTypeDescriptor;
			return customTypeDescriptor != null ? customTypeDescriptor.GetPropertyOwner(descriptor) : obj;
		}

		/// <summary>
		///     Validates the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		///     <c>true</c> if value can be applied for the property; otherwise, <c>false</c>.
		/// </returns>
		public bool Validate(object value)
		{
			return IsValidDependencyPropertyValue(_descriptor, value);
		}

		private static bool IsValidDependencyPropertyValue(PropertyDescriptor descriptor, object value)
		{
			var result = true;

			var dpd = DependencyPropertyDescriptor.FromProperty(descriptor);
			if (dpd != null)
			{
				if (dpd.DependencyProperty != null)
					result = dpd.DependencyProperty.IsValidValue(value);
			}

			return result;
		}

		private string GetDisplayName()
		{
			// TODO: decide what to be returned in the worst case (no descriptor)
			if (_descriptor == null) return null;

			// Try getting Parenthesize attribute
			var attr = GetAttribute<ParenthesizePropertyNameAttribute>();

			// if property needs parenthesizing then apply parenthesis to resulting display name      
			return (attr != null && attr.NeedParenthesis)
				? "(" + _descriptor.DisplayName + ")"
				: _descriptor.DisplayName;
		}

		#endregion
	}
}