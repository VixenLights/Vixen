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

namespace System.Windows.Controls.WpfPropertyGrid.Design
{
	/// <summary>
	/// Special Tab used to contain Extended Editors. Used in Tabbed Layout.
	/// </summary>
	public class ExtendedPropertyEditorTab : TabbedLayoutItem
	{
		private readonly ResourceLocator _resourceLocator = new ResourceLocator();

		/// <summary>
		/// Gets or sets the property an extended editor is bound to.
		/// </summary>
		/// <value>The property.</value>
		public PropertyItem Property { get; private set; }

		/// <summary>
		/// Initializes the <see cref="ExtendedPropertyEditorTab"/> class.
		/// </summary>
		static ExtendedPropertyEditorTab()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (ExtendedPropertyEditorTab),
				new FrameworkPropertyMetadata(typeof (ExtendedPropertyEditorTab)));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtendedPropertyEditorTab"/> class.
		/// </summary>
		public ExtendedPropertyEditorTab()
		{
			CanClose = true;
			VerticalContentAlignment = VerticalAlignment.Stretch;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtendedPropertyEditorTab"/> class.
		/// </summary>
		/// <param name="property">The property to display extended editor for.</param>
		public ExtendedPropertyEditorTab(PropertyItem property)
			: this()
		{
			if (property == null) throw new ArgumentNullException("property");

			Property = property;
			Header = property.Name;
			Content = CreateContent(property);
		}

		/// <summary>
		/// Creates the content with extended editor.
		/// </summary>
		/// <param name="propertyItem">The property item.</param>
		/// <returns>ContentControl to place into Tabbed Layout tab.</returns>
		protected virtual object CreateContent(PropertyItem propertyItem)
		{
			Editor editor = propertyItem.Editor;

			if (editor == null || editor.ExtendedTemplate == null)
				throw new ArgumentException("Property Editor does not support Extended templates!");

			if (editor.ExtendedTemplate == null) return null;

			var content = new ContentControl
			{
				VerticalContentAlignment = VerticalAlignment.Stretch,
				ContentTemplate = GetDataTemplate(editor.ExtendedTemplate),
				Content = propertyItem.PropertyValue
			};

			return content;
		}

		private DataTemplate GetDataTemplate(object template)
		{
			if (template == null) return null;

			var dataTemplate = template as DataTemplate;
			if (dataTemplate != null) return dataTemplate;

			var resourceKey = template as ComponentResourceKey;
			if (resourceKey == null) return null;

			return _resourceLocator.GetResource(resourceKey) as DataTemplate;
		}
	}
}