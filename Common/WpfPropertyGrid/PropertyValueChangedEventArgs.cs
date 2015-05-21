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

namespace System.Windows.Controls.WpfPropertyGrid
{
	/// <summary>
	///     Encapsulates a method that contains an object and PropertyValueChanged event arguments.
	/// </summary>
	public delegate void PropertyValueChangedEventHandler(object sender, PropertyValueChangedEventArgs e);

	/// <summary>
	///     Contains state information and data associated with property value change events.
	/// </summary>
	public sealed class PropertyValueChangedEventArgs : RoutedEventArgs
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="PropertyValueChangedEventArgs" /> class.
		/// </summary>
		/// <param name="routedEvent">The routed event.</param>
		/// <param name="property">The property.</param>
		/// <param name="oldValue">The old value.</param>
		public PropertyValueChangedEventArgs(RoutedEvent routedEvent, PropertyItem property, object[] oldValue)
			: base(routedEvent, property)
		{
			Property = property;
			NewValue = property.PropertyValue;
			OldValue = oldValue;
		}

		/// <summary>
		///     Gets the old value.
		/// </summary>
		/// <value>The old value.</value>
		public object[] OldValue { get; private set; }

		/// <summary>
		///     Gets the new value.
		/// </summary>
		/// <value>The new value.</value>
		public object NewValue { get; private set; }

		/// <summary>
		///     Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public PropertyItem Property { get; private set; }
	}
}