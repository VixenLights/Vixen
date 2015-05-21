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

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Markup;

namespace System.Windows.Controls.WpfPropertyGrid
{
	/// <summary>
	///     Defines a helper object for binding to CLR properties.
	/// </summary>
	public class DataResource : Freezable
	{
		/// <summary>
		///     Identifies the <see cref="BindingTarget" /> dependency property.
		/// </summary>
		/// <value>
		///     The identifier for the <see cref="BindingTarget" /> dependency property.
		/// </value>
		public static readonly DependencyProperty BindingTargetProperty = DependencyProperty.Register("BindingTarget",
			typeof (object), typeof (DataResource), new UIPropertyMetadata(null));

		/// <summary>
		///     Gets or sets the binding target.
		/// </summary>
		/// <value>The binding target.</value>
		public object BindingTarget
		{
			get { return GetValue(BindingTargetProperty); }
			set { SetValue(BindingTargetProperty, value); }
		}

		/// <summary>
		///     Creates an instance of the specified type using that type's default constructor.
		/// </summary>
		/// <returns>
		///     A reference to the newly created object.
		/// </returns>
		protected override Freezable CreateInstanceCore()
		{
			return (Freezable) Activator.CreateInstance(GetType());
		}

		/// <summary>
		///     Makes the instance a clone (deep copy) of the specified <see cref="Freezable" />
		///     using base (non-animated) property values.
		/// </summary>
		/// <param name="sourceFreezable">
		///     The object to clone.
		/// </param>
		protected override sealed void CloneCore(Freezable sourceFreezable)
		{
			base.CloneCore(sourceFreezable);
		}
	}

	/// <summary>
	///     Markup extension for establishing bindings to CLR properties from XAML.
	/// </summary>
	public class DataResourceBindingExtension : MarkupExtension
	{
		private DataResource _dataResouce;
		private object _targetObject;
		private object _targetProperty;

		/// <summary>
		///     Gets or sets the data resource.
		/// </summary>
		/// <value>The data resource.</value>
		public DataResource DataResource
		{
			get { return _dataResouce; }
			set
			{
				if (_dataResouce != value)
				{
					if (_dataResouce != null)
					{
						_dataResouce.Changed -= DataResourceChanged;
					}
					_dataResouce = value;

					if (_dataResouce != null)
					{
						_dataResouce.Changed += DataResourceChanged;
					}
				}
			}
		}

		/// <summary>
		///     When implemented in a derived class, returns an object that is set as the value of the target property for this
		///     markup extension.
		/// </summary>
		/// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
		/// <returns>
		///     The object value to set on the property where the extension is applied.
		/// </returns>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var target = (IProvideValueTarget) serviceProvider.GetService(typeof (IProvideValueTarget));

			_targetObject = target.TargetObject;
			_targetProperty = target.TargetProperty;

			// mTargetProperty can be null when this is called in the Designer.
			Debug.Assert(_targetProperty != null || DesignerProperties.GetIsInDesignMode(new DependencyObject()));

			if (DataResource.BindingTarget == null && _targetProperty != null)
			{
				var propInfo = _targetProperty as PropertyInfo;
				if (propInfo != null)
				{
					try
					{
						return Activator.CreateInstance(propInfo.PropertyType);
					}
					catch (MissingMethodException)
					{
						// there isn't a default constructor
					}
				}

				var depProp = _targetProperty as DependencyProperty;
				if (depProp != null)
				{
					var depObj = (DependencyObject) _targetObject;
					return depObj.GetValue(depProp);
				}
			}

			return DataResource.BindingTarget;
		}

		private void DataResourceChanged(object sender, EventArgs e)
		{
			// Ensure that the bound object is updated when DataResource changes.
			var dataResource = (DataResource) sender;
			var depProp = _targetProperty as DependencyProperty;

			if (depProp != null)
			{
				var depObj = (DependencyObject) _targetObject;
				var value = Convert(dataResource.BindingTarget, depProp.PropertyType);
				depObj.SetValue(depProp, value);
			}
			else
			{
				var propInfo = _targetProperty as PropertyInfo;
				if (propInfo != null)
				{
					var value = Convert(dataResource.BindingTarget, propInfo.PropertyType);
					propInfo.SetValue(_targetObject, value, new object[0]);
				}
			}
		}

		private static object Convert(object obj, Type toType)
		{
			try
			{
				return System.Convert.ChangeType(obj, toType);
			}
			catch (InvalidCastException)
			{
				return obj;
			}
		}
	}
}