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
using Vixen.Attributes;

namespace VixenModules.Editor.EffectEditor.PropertyEditing
{
	/// <summary>
	/// Default <see cref="PropertyItem"/> comparer.
	/// </summary>
	public class PropertyItemComparer : IComparer<PropertyItem>
	{
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
		public virtual int Compare(PropertyItem x, PropertyItem y)
		{
			if (x == y) return 0;
			if (x == null) return -1;
			if (y == null) return 1;

			int leftOrder = 0;

			var attribute = x.GetAttribute<PropertyOrderAttribute>();
			if (attribute != null)
				leftOrder = attribute.Order;

			int rightOrder = 0;

			attribute = y.GetAttribute<PropertyOrderAttribute>();
			if (attribute != null)
				rightOrder = attribute.Order;

			int num = (leftOrder == rightOrder) ? 0 : leftOrder.CompareTo(rightOrder);
			if (num != 0) return num;

			return string.Compare(x.DisplayName, y.DisplayName, true);
		}
	}
}