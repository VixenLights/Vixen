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
	///     Specifies the default category editor.
	/// </summary>
	public class CategoryEditor : Editor
	{
		private static readonly Type ThisType = typeof (CategoryEditor);

		/// <summary>
		///     Identifies the <see cref="DeclaringType" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty DeclaringTypeProperty =
			DependencyProperty.Register("DeclaringType", typeof (Type), ThisType);

		/// <summary>
		///     Identifies the <see cref="CategoryName" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty CategoryNameProperty =
			DependencyProperty.Register("CategoryName", typeof (string), ThisType);

		/// <summary>
		///     Initializes a new instance of the <see cref="CategoryEditor" /> class.
		/// </summary>
		public CategoryEditor()
		{
			// Default empty constructor
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CategoryEditor" /> class.
		/// </summary>
		/// <param name="declaringType">Declaring type.</param>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="inlineTemplate">The inline template.</param>
		public CategoryEditor(Type declaringType, string categoryName, object inlineTemplate)
		{
			if (declaringType == null) throw new ArgumentNullException("declaringType");
			if (string.IsNullOrEmpty(categoryName)) throw new ArgumentNullException("categoryName");

			DeclaringType = declaringType;
			CategoryName = categoryName;
			InlineTemplate = inlineTemplate;
		}

		/// <summary>
		///     Gets or sets the declaring type. This is a dependency property.
		/// </summary>
		/// <value>The declaring type.</value>
		public Type DeclaringType
		{
			get { return (Type) GetValue(DeclaringTypeProperty); }
			set { SetValue(DeclaringTypeProperty, value); }
		}

		/// <summary>
		///     Gets or sets the name of the category. This is a dependency property.
		/// </summary>
		/// <value>The name of the category.</value>
		public string CategoryName
		{
			get { return (string) GetValue(CategoryNameProperty); }
			set { SetValue(CategoryNameProperty, value); }
		}
	}
}