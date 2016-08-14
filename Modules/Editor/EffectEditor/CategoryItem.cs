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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using VixenModules.Editor.EffectEditor.PropertyEditing;
using VixenModules.Editor.EffectEditor.PropertyEditing.Filters;

namespace VixenModules.Editor.EffectEditor
{
	/// <summary>
	///     Special grid entry that provides information about property category and gives access to underlying properties.
	/// </summary>
	public class CategoryItem : GridEntry
	{
		private static readonly Func<PropertyItem, bool> IsVisibleProperty = prop => prop.IsBrowsable && prop.MatchesFilter;

		/// <summary>
		///     Adds the property.
		/// </summary>
		/// <param name="property">The property.</param>
		public void AddProperty(PropertyItem property)
		{
			if (property == null) throw new ArgumentNullException("property");
			if (_properties.Contains(property)) throw new ArgumentException("Cannot add a duplicated property " + property.Name);

			var index = _properties.BinarySearch(property, _comparer);
			if (index < 0)
				index = ~index;

			_properties.Insert(index, property);

			if (property.IsBrowsable)
				HasVisibleProperties = true;
			else
				HasVisibleProperties = _properties.Any(IsVisibleProperty);

			property.BrowsableChanged += PropertyBrowsableChanged;
		}

		private void PropertyBrowsableChanged(object sender, EventArgs e)
		{
			HasVisibleProperties = _properties.Any(IsVisibleProperty);
		}

		/// <summary>
		///     Checks whether the entry matches the filtering predicate.
		/// </summary>
		/// <param name="predicate">The filtering predicate.</param>
		/// <returns>
		///     <c>true</c> if entry matches predicate; otherwise, <c>false</c>.
		/// </returns>
		public override bool MatchesPredicate(PropertyFilterPredicate predicate)
		{
			return _properties.All(property => property.MatchesPredicate(predicate));
		}

		/// <summary>
		///     Applies the filter for the entry.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public override void ApplyFilter(PropertyFilter filter)
		{
			var propertiesMatch = false;
			foreach (var entry in Properties)
			{
				if (PropertyMatchesFilter(filter, entry))
					propertiesMatch = true;
			}

			HasVisibleProperties = _properties.Any(IsVisibleProperty);
			MatchesFilter = propertiesMatch;

			if (propertiesMatch && !IsExpanded)
				IsExpanded = true;

			OnFilterApplied(filter);
		}

		private static bool PropertyMatchesFilter(PropertyFilter filter, PropertyItem entry)
		{
			entry.ApplyFilter(filter);
			return entry.MatchesFilter;
		}

		#region Properties

		/// <summary>
		///     Gets or sets the attribute the category was created with.
		/// </summary>
		/// <value>The attribute.</value>
		public Attribute Attribute { get; set; }

		#region Order

		private int _order = -1;

		/// <summary>
		///     Gets or sets the order of the category.
		/// </summary>
		public int Order
		{
			get { return _order; }
			set
			{
				if (_order == value) return;
				_order = value;
				OnPropertyChanged("Order");
			}
		}

		#endregion

		#region IsExpanded

		private bool _isExpanded = true;

		/// <summary>
		///     Gets or sets a value indicating whether this category is expanded.
		/// </summary>
		/// <value>
		///     <c>true</c> if this category is expanded; otherwise, <c>false</c>.
		/// </value>
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if (_isExpanded == value) return;
				_isExpanded = value;
				OnPropertyChanged("IsExpanded");
			}
		}

		#endregion

		#region Properties

		private readonly GridEntryCollection<PropertyItem> _properties = new GridEntryCollection<PropertyItem>();

		/// <summary>
		///     Get all the properties in the category.
		/// </summary>
		/// <value></value>
		/// <returns>
		///     An enumerable collection of all the properties in the category.
		/// </returns>
		public ReadOnlyObservableCollection<PropertyItem> Properties
		{
			get { return new ReadOnlyObservableCollection<PropertyItem>(_properties); }
		}

		#endregion

		/// <summary>
		///     Gets the <see cref="PropertyItem" /> with the specified property name.
		/// </summary>
		/// <value></value>
		public PropertyItem this[string propertyName]
		{
			get { return _properties[propertyName]; }
		}

		private IComparer<PropertyItem> _comparer = new PropertyItemComparer();

		/// <summary>
		///     Gets or sets the comparer used to sort properties.
		/// </summary>
		/// <value>The comparer. </value>
		public IComparer<PropertyItem> Comparer
		{
			get { return _comparer; }
			set
			{
				if (_comparer == value) return;
				_comparer = value;
				_properties.Sort(_comparer);
				OnPropertyChanged("Comparer");
			}
		}

		private bool _hasVisibleProperties;

		/// <summary>
		///     Gets or sets a value indicating whether this instance has visible properties.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance has visible properties; otherwise, <c>false</c>.
		/// </value>
		public bool HasVisibleProperties
		{
			get { return _hasVisibleProperties; }
			private set
			{
				if (_hasVisibleProperties == value) return;
				_hasVisibleProperties = value;
				OnPropertyChanged("HasVisibleProperties");
			}
		}

		/// <summary>
		///     Gets a value indicating whether this instance should be visible.
		/// </summary>
		public override bool IsVisible
		{
			get { return base.IsVisible && HasVisibleProperties; }
		}

		#endregion

		#region ctor

		/// <summary>
		///     Initializes a new instance of the <see cref="CategoryItem" /> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="name">The name.</param>
		public CategoryItem(EffectPropertyEditorGrid owner, string name)
		{
			if (owner == null) throw new ArgumentNullException("owner");
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

			Owner = owner;
			Name = name;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CategoryItem" /> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="category">The category.</param>
		public CategoryItem(EffectPropertyEditorGrid owner, CategoryAttribute category)
			: this(owner, category.Category)
		{
			Attribute = category;
		}

		#endregion

		/// <summary>
		///     Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing">
		///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
		///     unmanaged resources.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				if (disposing)
				{
					if (_properties != null)
					{
						foreach (var propertyItem in _properties)
						{
							if (propertyItem != null)
							{
								propertyItem.BrowsableChanged -= PropertyBrowsableChanged;
							}
						}
					}
				}
				base.Dispose(disposing);
			}
		}

		#region ITypeDescriptorContext

		public override object GetService(Type serviceType)
		{
			if (serviceType == typeof (PropertyItem))
				return this;
			return null;
		}

		public override bool OnComponentChanging()
		{
			throw new NotSupportedException();
		}

		public override void OnComponentChanged()
		{
			throw new NotSupportedException();
		}

		public override IContainer Container
		{
			get { throw new NotSupportedException(); }
		}

		public override object Instance
		{
			get { return Instance; }
		}

		public override PropertyDescriptor PropertyDescriptor
		{
			get { throw new NotSupportedException(); }
		}

		#endregion
	}
}