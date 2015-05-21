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

namespace System.Windows.Controls.WpfPropertyGrid
{
	/// <summary>
	/// Default <see cref="CategoryItem"/> comparer.
	/// </summary>
	public class CategoryItemComparer : IComparer<CategoryItem>
	{
		#region IComparer<CategoryItem> Members

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// Value
		/// Condition
		/// Less than zero
		/// <paramref name="x"/> is less than <paramref name="y"/>.
		/// Zero
		/// <paramref name="x"/> equals <paramref name="y"/>.
		/// Greater than zero
		/// <paramref name="x"/> is greater than <paramref name="y"/>.
		/// </returns>
		public int Compare(CategoryItem x, CategoryItem y)
		{
			if (x == y) return 0;
			if (x == null) return -1;
			if (y == null) return 1;

			int leftOrder = x.Order;
			int rightOrder = y.Order;

			if (leftOrder == rightOrder)
				return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);

			return leftOrder.CompareTo(rightOrder);
		}

		#endregion
	}
}