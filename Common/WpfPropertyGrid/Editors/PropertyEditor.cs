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

namespace System.Windows.Controls.WpfPropertyGrid.Editors
{
	/// <summary>
	///     Provides value editing service for a property value.
	/// </summary>
	public class PropertyEditor : Editor
	{
		private static readonly Type _ThisType = typeof (PropertyEditor);

		/// <summary>
		///     Identifies the <see cref="DeclaringType" /> property. This is a dependency property.
		/// </summary>
		public static readonly DependencyProperty DeclaringTypeProperty =
			DependencyProperty.Register("DeclaringType", typeof (Type), _ThisType, new PropertyMetadata(null));

		/// <summary>
		///     Identifies the <see cref="PropertyName" /> property. This is a dependency property.
		/// </summary>
		public static readonly DependencyProperty PropertyNameProperty =
			DependencyProperty.Register("PropertyName", typeof (string), _ThisType, new PropertyMetadata(null));

		/// <summary>
		///     Initializes a new instance of the <see cref="PropertyEditor" /> class.
		/// </summary>
		public PropertyEditor()
		{
			// Default empty constructor
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="PropertyEditor" /> class.
		/// </summary>
		/// <param name="declaringType">Type that contains targeted property.</param>
		/// <param name="propertyName">Name of the property to bind the editor to.</param>
		public PropertyEditor(Type declaringType, string propertyName)
		{
			if (declaringType == null) throw new ArgumentNullException("declaringType");
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			DeclaringType = declaringType;
			PropertyName = propertyName;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="PropertyEditor" /> class.
		/// </summary>
		/// <param name="declaringType">Type that contains targeted property.</param>
		/// <param name="propertyName">Name of the property to bind the editor to.</param>
		/// <param name="inlineTemplate">
		///     The inline template for UI presentation. Can be either a DataTemplate or
		///     ComponentResourceKey object.
		/// </param>
		public PropertyEditor(Type declaringType, string propertyName, object inlineTemplate)
			: this(declaringType, propertyName)
		{
			InlineTemplate = inlineTemplate;
		}

		/// <summary>
		///     Gets or sets type of the object containing edited property.
		/// </summary>
		/// <value>The type of the edited property </value>
		public Type DeclaringType
		{
			get { return (Type) GetValue(DeclaringTypeProperty); }
			set { SetValue(DeclaringTypeProperty, value); }
		}

		/// <summary>
		///     Gets or sets the name of the property the editor is bound to.
		/// </summary>
		/// <value>The name of the property editor is bound to.</value>
		public string PropertyName
		{
			get { return (string) GetValue(PropertyNameProperty); }
			set { SetValue(PropertyNameProperty, value); }
		}
	}
}