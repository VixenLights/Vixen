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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VixenModules.Editor.EffectEditor.Design
{
	/// <summary>
	/// Defines a basement for GridEntry UI layouts (panels, lists, etc)
	/// </summary>
	/// <typeparam name="T">The type of elements in the control.</typeparam>
	public abstract class GridEntryLayout<T> : ItemsControl where T : GridEntryContainer, new()
	{
		/// <summary>
		/// Creates or identifies the element that is used to display the given item.
		/// </summary>
		/// <returns>
		/// The element that is used to display the given item.
		/// </returns>
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new T();
		}

		/// <summary>
		/// Prepares the specified element to display the specified item.
		/// </summary>
		/// <param name="element">Element used to display the specified item.</param>
		/// <param name="item">Specified item.</param>
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			var container = element as T;
			if (container != null)
			{
				container.DataContext = item;
				container.SetBinding(GridEntryContainer.EntryProperty, new Binding());
			}
			base.PrepareContainerForItemOverride(element, item);
		}
	}
}