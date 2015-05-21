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
	///     Provides value editing service for property value that is of some specific type.
	/// </summary>
	public class TypeEditor : Editor
	{
		private static readonly Type _ThisType = typeof (TypeEditor);

		/// <summary>
		///     Identifies the <see cref="EditedType" /> property. This is a dependency property.
		/// </summary>
		public static readonly DependencyProperty EditedTypeProperty =
			DependencyProperty.Register("EditedType", typeof (Type), _ThisType, new PropertyMetadata(null));

		/// <summary>
		///     Initializes a new instance of the <see cref="TypeEditor" /> class.
		/// </summary>
		public TypeEditor()
		{
			// Default empty constructor
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="TypeEditor" /> class.
		/// </summary>
		/// <param name="editedType">The type of the object editor is bound to.</param>
		public TypeEditor(Type editedType) : this(editedType, null, null)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="TypeEditor" /> class.
		/// </summary>
		/// <param name="editedType">The type of the object editor is bound to.</param>
		/// <param name="inlineTemplate">
		///     The inline template for UI presentation. Can be either a DataTemplate or
		///     ComponentResourceKey object.
		/// </param>
		public TypeEditor(Type editedType, object inlineTemplate) : this(editedType, inlineTemplate, null)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="TypeEditor" /> class.
		/// </summary>
		/// <param name="editedType">Type of the edited.</param>
		/// <param name="inlineTemplate">
		///     The inline template for UI presentation. Can be either a DataTemplate or
		///     ComponentResourceKey object.
		/// </param>
		/// <param name="extendedTemplate">
		///     The extended template for UI presentation. Can be either a DataTemplate or
		///     ComponentResourceKey object.
		/// </param>
		public TypeEditor(Type editedType, object inlineTemplate, object extendedTemplate)
		{
			if (editedType == null) throw new ArgumentNullException("editedType");

			EditedType = editedType;

			InlineTemplate = inlineTemplate;
			ExtendedTemplate = extendedTemplate;
		}

		/// <summary>
		///     Gets or sets the type of the object editor is bound to.
		/// </summary>
		/// <value>The type of the object editor is bound to.</value>
		public Type EditedType
		{
			get { return (Type) GetValue(EditedTypeProperty); }
			set { SetValue(EditedTypeProperty, value); }
		}
	}
}