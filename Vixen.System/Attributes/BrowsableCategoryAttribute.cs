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

namespace Vixen.Attributes
{
	/// <summary>
	///     Controls the Browsable state of the category with corresponding properties.
	///     Supports the "*" (All) wildcard determining whether all the categories within the given class should be Browsable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class BrowsableCategoryAttribute : Attribute
	{
		/// <summary>
		///     Determines a wildcard for all categories to be affected.
		/// </summary>
		public const string All = "*";

		/// <summary>
		///     Initializes a new instance of the <see cref="BrowsableCategoryAttribute" /> class.
		/// </summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="browsable">if set to <c>true</c> the category is browsable.</param>
		/// <param name="expanded">if set to <c>true</c> the category is collapsed.</param>
		public BrowsableCategoryAttribute(string categoryName, bool browsable, bool expanded)
		{
			CategoryName = string.IsNullOrEmpty(categoryName) ? All : categoryName;
			Browsable = browsable;
			Expanded = expanded;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="BrowsableCategoryAttribute" /> class.
		/// </summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="browsable">if set to <c>true</c> the category is browsable.</param>
		public BrowsableCategoryAttribute(string categoryName, bool browsable)
		{
			CategoryName = string.IsNullOrEmpty(categoryName) ? All : categoryName;
			Browsable = browsable;
			Expanded = true;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="BrowsableCategoryAttribute" /> class.
		/// </summary>
		/// <param name="categoryName">Name of the category.</param>
		public BrowsableCategoryAttribute(string categoryName) : this(categoryName, true)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="BrowsableCategoryAttribute" /> class.
		/// </summary>
		/// <param name="browsable">if set to <c>true</c> all categories are browsable; otherwise hidden</param>
		public BrowsableCategoryAttribute(bool browsable) : this(All, browsable)
		{
		}

		/// <summary>
		///     Gets the name of the category.
		/// </summary>
		/// <value>The name of the category.</value>
		public string CategoryName { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether category is browsable.
		/// </summary>
		/// <value><c>true</c> if category should be displayed at run time; otherwise, <c>false</c>.</value>
		public bool Browsable { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether category is Collapsed.
		/// </summary>
		/// <value><c>true</c> if category should be displayed at run time; otherwise, <c>false</c>.</value>
		public bool Expanded { get; private set; }
	}
}