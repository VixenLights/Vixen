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

using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls.WpfPropertyGrid.PropertyEditing.Filters
{
	/// <summary>
	/// Specifies a default property filter.
	/// </summary>
	public class PropertyFilter
	{
		private List<PropertyFilterPredicate> _predicates;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyFilter"/> class.
		/// </summary>
		/// <param name="predicates">The predicates.</param>
		public PropertyFilter(IEnumerable<PropertyFilterPredicate> predicates)
		{
			_predicates = new List<PropertyFilterPredicate>();
			SetPredicates(predicates);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyFilter"/> class.
		/// </summary>
		/// <param name="filterText">The filter text.</param>
		public PropertyFilter(string filterText)
		{
			_predicates = new List<PropertyFilterPredicate>();
			SetPredicates(filterText);
		}

		/// <summary>
		/// Matches the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns><c>true</c> if target matches filter; otherwise, <c>false</c>.</returns>
		public bool Match(IPropertyFilterTarget target)
		{
			if (target == null) throw new ArgumentNullException("target");
			if (IsEmpty) return true;

			for (int i = 0; i < _predicates.Count; i++)
				if (target.MatchesPredicate(_predicates[i]))
					return true;

			return false;
		}

		private void SetPredicates(IEnumerable<PropertyFilterPredicate> predicates)
		{
			if (predicates == null) return;
			foreach (var predicate in predicates.Where(predicate => predicate != null))
				_predicates.Add(predicate);
		}

		private void SetPredicates(string filterText)
		{
			if (!string.IsNullOrEmpty(filterText))
			{
				string[] strArray = filterText.Split(new[] {' '});
				for (int i = 0; i < strArray.Length; i++)
					if (!string.IsNullOrEmpty(strArray[i]))
						_predicates.Add(new PropertyFilterPredicate(strArray[i]));
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get { return (_predicates.Count == 0); }
		}
	}
}