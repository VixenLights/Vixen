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
	/// Contains state information and data related to FilterApplied event.
	/// </summary>
	public sealed class PropertyFilterAppliedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the filter.
		/// </summary>
		/// <value>The filter.</value>
		public PropertyFilter Filter { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyFilterAppliedEventArgs"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public PropertyFilterAppliedEventArgs(PropertyFilter filter)
		{
			Filter = filter;
		}
	}
}