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

namespace VixenModules.Editor.EffectEditor.PropertyEditing.Filters
{
	/// <summary>
	/// Describes an object that supports filtering.
	/// </summary>
	public interface IPropertyFilterTarget
	{
		/// <summary>
		/// Occurs when filter is applied for the entry.
		/// </summary>
		event EventHandler<PropertyFilterAppliedEventArgs> FilterApplied;

		/// <summary>
		/// Applies the filter for the entry.
		/// </summary>
		/// <param name="filter">The filter.</param>
		void ApplyFilter(PropertyFilter filter);

		/// <summary>
		/// Checks whether the entry matches the filtering predicate.
		/// </summary>
		/// <param name="predicate">The filtering predicate.</param>    
		/// <returns><c>true</c> if entry matches filter; otherwise, <c>false</c>.</returns>
		bool MatchesPredicate(PropertyFilterPredicate predicate);

		/// <summary>
		/// Gets or sets a value indicating whether the entry matches filter.
		/// </summary>
		/// <value><c>true</c> if entry matches filter; otherwise, <c>false</c>.</value>
		bool MatchesFilter { get; }
	}
}