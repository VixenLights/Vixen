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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Vixen.Attributes;
using Vixen.Module.Effect;
using VixenModules.Editor.EffectEditor.Controls;
using VixenModules.Editor.EffectEditor.Design;
using VixenModules.Editor.EffectEditor.Editors;
using VixenModules.Editor.EffectEditor.PropertyEditing;
using VixenModules.Editor.EffectEditor.PropertyEditing.Filters;

namespace VixenModules.Editor.EffectEditor
{
	/// <summary>
	///     EffectPropertyEditorGrid control.
	/// </summary>
	public partial class EffectPropertyEditorGrid : Control, INotifyPropertyChanged
	{
		public delegate void PreviewStateChangedEventHandler(object sender, PreviewStateEventArgs e);

		private const string InformationMessage = "Select an Effect to edit.";
		private const string InformationLinkUrl = "http://www.vixenlights.com/vixen-3-documentation/sequencer/effect-editor";
		private static readonly Type ThisType = typeof (EffectPropertyEditorGrid);

		/// <summary>
		///     CurrentDescription Dependency Property
		/// </summary>
		public static readonly DependencyProperty CurrentDescriptionProperty =
			DependencyProperty.Register("CurrentDescription", typeof (string), typeof (EffectPropertyEditorGrid),
				new FrameworkPropertyMetadata("",
					OnCurrentDescriptionChanged));

		/// <summary>
		///     Identifies the <see cref="PropertyEditingStarted" /> routed event.
		/// </summary>
		public static readonly RoutedEvent PropertyEditingStartedEvent =
			EventManager.RegisterRoutedEvent("PropertyEditingStarted", RoutingStrategy.Bubble,
				typeof (PropertyEditingEventHandler), ThisType);

		/// <summary>
		///     Identifies the <see cref="PropertyEditingFinished" /> routed event.
		/// </summary>
		public static readonly RoutedEvent PropertyEditingFinishedEvent =
			EventManager.RegisterRoutedEvent("PropertyEditingFinished", RoutingStrategy.Bubble,
				typeof (PropertyEditingEventHandler), ThisType);

		/// <summary>
		///     Identifies the <see cref="PropertyValueChanged" /> routed event.
		/// </summary>
		public static readonly RoutedEvent PropertyValueChangedEvent =
			EventManager.RegisterRoutedEvent("PropertyValueChanged", RoutingStrategy.Bubble,
				typeof (PropertyValueChangedEventHandler), ThisType);

		/// <summary>
		///     Identifies the <see cref="ItemsBackground" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty ItemsBackgroundProperty =
			DependencyProperty.Register("ItemsBackground", typeof (Brush), ThisType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

		/// <summary>
		///     Identifies the <see cref="ItemsForeground" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty ItemsForegroundProperty =
			DependencyProperty.Register("ItemsForeground", typeof (Brush), ThisType,
				new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.None));

		/// <summary>
		///     Identifies the <see cref="Layout" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayoutProperty =
			DependencyProperty.Register("Layout", typeof (Control), ThisType,
				new FrameworkPropertyMetadata(default(AlphabeticalLayout),
					FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure |
					FrameworkPropertyMetadataOptions.AffectsRender, OnLayoutChanged));

		/// <summary>
		///     Identifies the <see cref="ShowReadOnlyProperties" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty ShowReadOnlyPropertiesProperty =
			DependencyProperty.Register("ShowReadOnlyProperties", typeof (bool), ThisType,
				new PropertyMetadata(false, OnShowReadOnlyPropertiesChanged));

		/// <summary>
		///     Identifies the <see cref="ShowAttachedProperties" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty ShowAttachedPropertiesProperty =
			DependencyProperty.Register("ShowAttachedProperties", typeof (bool), ThisType,
				new PropertyMetadata(false, OnShowAttachedPropertiesChanged));

		/// <summary>
		///     Identifies the <see cref="PropertyFilter" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty PropertyFilterProperty =
			DependencyProperty.Register("PropertyFilter", typeof (string), ThisType,
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnPropertyFilterChanged));

		/// <summary>
		///     Identifies the <see cref="PropertyFilterVisibility" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty PropertyFilterVisibilityProperty =
			DependencyProperty.Register("PropertyFilterVisibility", typeof (Visibility), ThisType,
				new FrameworkPropertyMetadata(Visibility.Visible));

		/// <summary>
		///     Identifies the <see cref="PropertyDisplayMode" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty PropertyDisplayModeProperty =
			DependencyProperty.Register("PropertyDisplayMode", typeof (PropertyDisplayMode), ThisType,
				new FrameworkPropertyMetadata(PropertyDisplayMode.All, OnPropertyDisplayModePropertyChanged));

		private GridEntryCollection<CategoryItem> _categories;
		private IComparer<CategoryItem> _categoryComparer;
		private string _effectName = "";
		private string _information = InformationMessage;
		private string _informationLink = InformationLinkUrl;
		private GridEntryCollection<PropertyItem> _properties;
		private IComparer<PropertyItem> _propertyComparer;

		/// <summary>
		///     Gets or sets the brush for items background. This is a dependency property.
		/// </summary>
		/// <value>The items background brush.</value>
		public Brush ItemsBackground
		{
			get { return (Brush) GetValue(ItemsBackgroundProperty); }
			set { SetValue(ItemsBackgroundProperty, value); }
		}

		/// <summary>
		///     Gets or sets the items foreground brush. This is a dependency property.
		/// </summary>
		/// <value>The items foreground brush.</value>
		public Brush ItemsForeground
		{
			get { return (Brush) GetValue(ItemsForegroundProperty); }
			set { SetValue(ItemsForegroundProperty, value); }
		}

		/// <summary>
		///     Gets or sets the layout to be used to display properties.
		/// </summary>
		/// <value>The layout to be used to display properties.</value>
		public Control Layout
		{
			get { return (Control) GetValue(LayoutProperty); }
			set { SetValue(LayoutProperty, value); }
		}

		/// <summary>
		///     Gets or sets the selected object.
		/// </summary>
		/// <value>The selected object.</value>
		public IEffect SelectedObject
		{
			get { return (currentObjects != null && currentObjects.Length != 0) ? currentObjects[0] : null; }
			set { SelectedObjects = (value == null) ? new IEffect[0] : new[] {value}; }
		}

		/// <summary>
		///     Gets or sets the selected objects.
		/// </summary>
		/// <value>The selected objects.</value>
		public IEffect[] SelectedObjects
		{
			get { return (currentObjects == null) ? new IEffect[0] : (IEffect[]) currentObjects.Clone(); }
			set
			{
				// Ensure there are no nulls in the array
				VerifySelectedObjects(value);

				var sameSelection = false;

				// Check whether new selection is the same as was previously defined
				if (currentObjects != null && value != null && currentObjects.Length == value.Length)
				{
					sameSelection = true;

					for (var i = 0; i < value.Length && sameSelection; i++)
					{
						if (currentObjects[i] != value[i])
							sameSelection = false;
					}
				}

				if (!sameSelection)
				{
					// Assign new objects and reload
					if (value == null)
					{
						currentObjects = new IEffect[0];
						DoReload();
					}
					else
					{
						// process single selection
						if (value.Length == 1 && currentObjects != null && currentObjects.Length == 1)
						{
							var oldValue = (currentObjects != null && currentObjects.Length > 0) ? currentObjects[0] : null;
							var newValue = (value.Length > 0) ? value[0] : null;

							currentObjects = (IEffect[]) value.Clone();

							if (oldValue != null && newValue != null && oldValue.GetType().Equals(newValue.GetType()))
								SwapSelectedObject(newValue);
							else
							{
								DoReload();
							}
						}
						// process multiple selection
						else
						{
							currentObjects = (IEffect[]) value.Clone();
							DoReload();
						}
					}

					OnPropertyChanged("SelectedObjects");
					OnPropertyChanged("SelectedObject");
					OnSelectedObjectsChanged();
				}
			}
		}

		/// <summary>
		///     Gets or sets the properties of the selected object(s).
		/// </summary>
		/// <value>The properties of the selected object(s).</value>
		public GridEntryCollection<PropertyItem> Properties
		{
			get { return _properties; }
			private set
			{
				if (_properties == value) return;

				if (_properties != null)
				{
					foreach (var item in _properties)
					{
						UnhookPropertyChanged(item);
						item.Dispose();
					}
				}
				Information = InformationMessage;
				InformationLink = InformationLinkUrl;
				if (value != null)
				{
					_properties = value;
					EffectName = "";
					if (PropertyComparer != null)
					{
						_properties.Sort(PropertyComparer);
					}

					foreach (var item in _properties)
					{
						HookPropertyChanged(item);
						if (item.Name.Equals("EffectName"))
						{
							EffectName = item.PropertyValue.StringValue;
						}
						if (item.Name.Equals("Information"))
						{
							Information = item.PropertyValue.StringValue;
						}
						if (item.Name.Equals("InformationLink"))
						{
							InformationLink = item.PropertyValue.StringValue;
						}
					}
				}
				OnPropertyChanged("Properties");
				OnPropertyChanged("HasProperties");
				OnPropertyChanged("BrowsableProperties");
			}
		}

		/// <summary>
		///     Enumerates the properties that should be visible for user
		/// </summary>
		public IEnumerable<PropertyItem> BrowsableProperties
		{
			get
			{
				if (_properties != null)
				{
					foreach (var property in _properties)
						if (property.IsBrowsable) yield return property;
				}
			}
		}

		/// <summary>
		///     Gets or sets the default property comparer.
		/// </summary>
		/// <value>The default property comparer.</value>
		public IComparer<PropertyItem> PropertyComparer
		{
			get { return _propertyComparer ?? (_propertyComparer = new PropertyItemComparer()); }
			set
			{
				if (_propertyComparer == value) return;
				_propertyComparer = value;

				if (_properties != null)
					_properties.Sort(_propertyComparer);

				OnPropertyChanged("PropertyComparer");
			}
		}

		/// <summary>
		///     Gets or sets the default category comparer.
		/// </summary>
		/// <value>The default category comparer.</value>
		public IComparer<CategoryItem> CategoryComparer
		{
			get { return _categoryComparer ?? (_categoryComparer = new CategoryItemComparer()); }
			set
			{
				if (_categoryComparer == value) return;
				_categoryComparer = value;

				if (_categories != null)
					_categories.Sort(_categoryComparer);

				OnPropertyChanged("Categories");
			}
		}

		public string EffectName
		{
			get { return _effectName; }
			set
			{
				_effectName = value;
				OnPropertyChanged("EffectName");
			}
		}

		public string Information
		{
			get { return _information; }
			set
			{
				_information = value;
				OnPropertyChanged("Information");
			}
		}

		public string InformationLink
		{
			get { return _informationLink; }
			set
			{
				_informationLink = value;
				OnPropertyChanged("InformationLink");
			}
		}

		/// <summary>
		///     Gets or sets the categories of the selected object(s).
		/// </summary>
		/// <value>The categories of the selected object(s).</value>
		public GridEntryCollection<CategoryItem> Categories
		{
			get { return _categories; }
			private set
			{
				if (_categories == value) return;
				if (_categories != null)
				{
					foreach (var item in _categories)
					{
						item.Dispose();
					}
				}
				_categories = value;

				if (CategoryComparer != null)
					_categories.Sort(CategoryComparer);

				OnPropertyChanged("Categories");
				OnPropertyChanged("HasCategories");
				OnPropertyChanged("BrowsableCategories");
			}
		}

		/// <summary>
		///     Enumerates the categories that should be visible for user.
		/// </summary>
		public IEnumerable<CategoryItem> BrowsableCategories
		{
			get
			{
				if (_categories != null)
				{
					foreach (var category in _categories)
						if (category.IsBrowsable) yield return category;
				}
			}
		}

		/// <summary>
		///     Gets or sets a value indicating whether read-only properties should be displayed. This is a dependency property.
		/// </summary>
		/// <value>
		///     <c>true</c> if read-only properties should be displayed; otherwise, <c>false</c>. Default is <c>false</c>.
		/// </value>
		public bool ShowReadOnlyProperties
		{
			get { return (bool) GetValue(ShowReadOnlyPropertiesProperty); }
			set { SetValue(ShowReadOnlyPropertiesProperty, value); }
		}

		/// <summary>
		///     Gets or sets a value indicating whether attached properties should be displayed.
		/// </summary>
		/// <value>
		///     <c>true</c> if attached properties should be displayed; otherwise, <c>false</c>. Default is <c>false</c>.
		/// </value>
		public bool ShowAttachedProperties
		{
			get { return (bool) GetValue(ShowAttachedPropertiesProperty); }
			set { SetValue(ShowAttachedPropertiesProperty, value); }
		}

		/// <summary>
		///     Gets or sets the property filter. This is a dependency property.
		/// </summary>
		/// <value>The property filter.</value>
		public string PropertyFilter
		{
			get { return (string) GetValue(PropertyFilterProperty); }
			set { SetValue(PropertyFilterProperty, value); }
		}

		/// <summary>
		///     Gets or sets the property filter visibility state.
		/// </summary>
		/// <value>The property filter visibility state.</value>
		public Visibility PropertyFilterVisibility
		{
			get { return (Visibility) GetValue(PropertyFilterVisibilityProperty); }
			set { SetValue(PropertyFilterVisibilityProperty, value); }
		}

		/// <summary>
		///     Gets or sets the property display mode. This is a dependency property.
		/// </summary>
		/// <value>The property display mode.</value>
		public PropertyDisplayMode PropertyDisplayMode
		{
			get { return (PropertyDisplayMode) GetValue(PropertyDisplayModeProperty); }
			set { SetValue(PropertyDisplayModeProperty, value); }
		}

		/// <summary>
		///     Occurs when a property value changes.
		/// </summary>
		public event PreviewStateChangedEventHandler PreviewChanged;

		/// <summary>
		///     Called when preview state changes.
		/// </summary>
		/// <param name="state">State</param>
		protected virtual void OnPreviewStateChanged(bool state)
		{
			var handler = PreviewChanged;
			if (handler != null) handler(this, new PreviewStateEventArgs(state));
		}

		/// <summary>
		///     Occurs when property editing is started.
		/// </summary>
		/// <remarks>
		///     This event is intended to be used in customization scenarios. It is not used by EffectPropertyEditorGrid control
		///     directly.
		/// </remarks>
		public event RoutedEventHandler PropertyEditingStarted
		{
			add { AddHandler(PropertyEditingStartedEvent, value); }
			remove { RemoveHandler(PropertyEditingStartedEvent, value); }
		}

		/// <summary>
		///     Occurs when property editing is finished.
		/// </summary>
		/// <remarks>
		///     This event is intended to be used in customization scenarios. It is not used by EffectPropertyEditorGrid control
		///     directly.
		/// </remarks>
		public event RoutedEventHandler PropertyEditingFinished
		{
			add { AddHandler(PropertyEditingFinishedEvent, value); }
			remove { RemoveHandler(PropertyEditingFinishedEvent, value); }
		}

		/// <summary>
		///     Occurs when property item value is changed.
		/// </summary>
		public event PropertyValueChangedEventHandler PropertyValueChanged
		{
			add { AddHandler(PropertyValueChangedEvent, value); }
			remove { RemoveHandler(PropertyValueChangedEvent, value); }
		}

		private void RaisePropertyValueChangedEvent(PropertyItem property, object[] oldValue)
		{
			var args = new PropertyValueChangedEventArgs(PropertyValueChangedEvent, property, oldValue);
			RaiseEvent(args);
		}

		/// <summary>
		///     Occurs when selected objects are changed.
		/// </summary>
		public event EventHandler SelectedObjectsChanged;

		/// <summary>
		///     Called when selected objects were changed.
		/// </summary>
		protected virtual void OnSelectedObjectsChanged()
		{
			var handler = SelectedObjectsChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		private static void OnLayoutChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var layoutContainer = e.NewValue as Control;
			if (layoutContainer != null)
				layoutContainer.DataContext = sender;
		}

		private static void OnShowReadOnlyPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var pg = (EffectPropertyEditorGrid) sender;

			// Check whether any object was selected
			if (pg.SelectedObject == null) return;

			// Check whether categories or properties were created
			if (pg.HasCategories | pg.HasProperties) pg.DoReload();
		}

		private static void OnShowAttachedPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var pg = (EffectPropertyEditorGrid) sender;
			if (pg.SelectedObject == null) return;
			if (pg.HasCategories | pg.HasProperties) pg.DoReload();
		}

		private static void OnPropertyFilterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var propertyGrid = (EffectPropertyEditorGrid) sender;

			if (propertyGrid.SelectedObject == null || !propertyGrid.HasCategories) return;

			foreach (var category in propertyGrid.Categories)
				category.ApplyFilter(new PropertyFilter(propertyGrid.PropertyFilter));
		}

		private static void OnPropertyDisplayModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var propertyGrid = (EffectPropertyEditorGrid) sender;
			if (propertyGrid.SelectedObject == null) return;
			propertyGrid.DoReload();
		}

		/// <summary>
		///     Gets the editor for a grid entry.
		/// </summary>
		/// <param name="entry">The entry to look the editor for.</param>
		/// <returns>Editor for the entry</returns>
		public virtual Editors.Editor GetEditor(GridEntry entry)
		{
			var property = entry as PropertyItem;
			if (property != null)
				return Editors.GetEditor(property);

			var category = entry as CategoryItem;
			if (category != null)
				return Editors.GetEditor(category);

			return null;
		}

		internal EditorCollection GetEditors()
		{
			return Editors;
		}

		private void SwapSelectedObject(object value)
		{
			//foreach (PropertyItem property in this.Properties)
			//{
			//  property.SetPropertySouce(value);
			//}
			DoReload();
		}

		private IEnumerable<CategoryItem> CollectCategories(IEnumerable<PropertyItem> properties)
		{
			var categories = new Dictionary<string, CategoryItem>();
			var refusedCategories = new HashSet<string>();

			foreach (var property in properties)
			{
				if (refusedCategories.Contains(property.CategoryName)) continue;
				CategoryItem category;

				if (categories.ContainsKey(property.CategoryName))
					category = categories[property.CategoryName];
				else
				{
					category = CreateCategory(property.GetAttribute<CategoryAttribute>());

					if (category == null)
					{
						refusedCategories.Add(property.CategoryName);
						continue;
					}

					categories[category.Name] = category;
				}

				category.AddProperty(property);
			}

			return categories.Values.ToList();
		}

		private IEnumerable<PropertyItem> CollectProperties(IEffect[] components)
		{
			if (components == null || components.Length == 0) throw new ArgumentNullException("components");

			var descriptors = (components.Length == 1)
				? MetadataRepository.GetProperties(components[0]).Select(prop => prop.Descriptor)
				: ObjectServices.GetMergedProperties(components);

			return descriptors.Select(CreatePropertyItem).Where(item => item != null).ToList();
		}

		private void UpdateBrowsable()
		{
			if (SelectedObjects == null || SelectedObjects.Length == 0) throw new ArgumentNullException("components");
			MetadataRepository.Clear();
			var descriptors = (SelectedObjects.Length == 1)
				? MetadataRepository.GetProperties(SelectedObjects[0]).Select(prop => prop.Descriptor)
				: ObjectServices.GetMergedProperties(SelectedObjects);

			foreach (var descriptor in descriptors)
			{
				var item = Properties.FirstOrDefault(x => x.Name.Equals(descriptor.Name));
				if (item != null)
				{
					item.IsBrowsable = ShoudDisplayProperty(descriptor);
				}
			}

			if (PropertyComparer != null)
				_properties.Sort(PropertyComparer);

			OnPropertyChanged("Properties");
			OnPropertyChanged("HasProperties");
			OnPropertyChanged("BrowsableProperties");
		}

		private static void VerifySelectedObjects(object[] value)
		{
			if (value != null && value.Length > 0)
			{
				// Ensure there are no nulls in the array
				for (var i = 0; i < value.Length; i++)
				{
					if (value[i] == null)
					{
						var args = new object[]
						{i.ToString(CultureInfo.CurrentCulture), value.Length.ToString(CultureInfo.CurrentCulture)};
						// TODO: Move exception format to resources/settings!
						throw new ArgumentNullException(
							string.Format("Item {0} in the 'objs' array is null. The array must begin with at least {1} members.", args));
					}
				}
			}
		}

		/// <summary>
		///     Invoked when an unhandled <see cref="UIElement.KeyDown" /> attached event reaches an element in its route that is
		///     derived from this class. Implement this method to add class handling for this event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Tab && e.OriginalSource is DependencyObject) //tabbing over the property editors
			{
				var source = e.OriginalSource as DependencyObject;
				//var element = Keyboard.Modifiers == ModifierKeys.Shift ? GetTabElement(source, -1) : GetTabElement(source, 1);
				//if (element != null)
				//{
				//	element.Focus();
				//	e.Handled = true;
				//	return;
				//}
			}

			base.OnKeyDown(e);
		}

		/// <summary>
		///     Gets the tab element on which the focus can be placed.
		/// </summary>
		/// <remarks>
		///     If an element is not enabled it will not be returned.
		/// </remarks>
		/// <param name="source">The source.</param>
		/// <param name="delta">The delta.</param>
		private UIElement GetTabElement(DependencyObject source, int delta)
		{
			if (source == null) return null;
			PropertyContainer container = null;
			if (source is SearchTextBox && HasCategories)
			{
				var itemspres = FindVisualChild<ItemsPresenter>(this);
				if (itemspres != null)
				{
					var catcontainer = FindVisualChild<CategoryContainer>(itemspres);
					if (catcontainer != null)
					{
						container = FindVisualChild<PropertyContainer>(catcontainer);
					}
				}
			}
			else
			{
				container = FindVisualParent<PropertyContainer>(source);
			}

			var spanel = FindVisualParent<StackPanel>(container);
			if (spanel != null && spanel.Children.Contains(container))
			{
				var index = spanel.Children.IndexOf(container);
				if (delta > 0)
					index = (index == spanel.Children.Count - 1) ? 0 : index + delta; //go back to the first after last
				else
					index = (index == 0) ? spanel.Children.Count - 1 : index + delta; //go to last after first
				//loop inside the list
				if (index < 0)
					index = spanel.Children.Count - 1;
				if (index >= spanel.Children.Count)
					index = 0;


				var next = VisualTreeHelper.GetChild(spanel, index) as PropertyContainer; //this has always a Grid as visual child

				var grid = FindVisualChild<Grid>(next);
				if (grid != null && grid.Children.Count > 1)
				{
					var pecp = grid.Children[1] as PropertyEditorContentPresenter;
					var final = VisualTreeHelper.GetChild(pecp, 0);
					if ((final as UIElement).IsEnabled && (final as UIElement).Focusable &&
					    !(next.DataContext as PropertyItem).IsReadOnly)
						return final as UIElement;
					return GetTabElement(final, delta);
				}
			}
			return null;
		}

		private static T FindVisualParent<T>(DependencyObject element) where T : class
		{
			if (element == null) return default(T);
			object parent = VisualTreeHelper.GetParent(element);
			if (parent is T)
				return parent as T;
			if (parent is DependencyObject)
				return FindVisualParent<T>(parent as DependencyObject);
			return null;
		}

		private static T FindVisualChild<T>(DependencyObject element) where T : class
		{
			if (element == null) return default(T);
			if (element is T) return element as T;
			if (VisualTreeHelper.GetChildrenCount(element) > 0)
			{
				for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
				{
					object child = VisualTreeHelper.GetChild(element, i);
					if (child is SearchTextBox) continue; //speeds up things a bit
					if (child is T)
						return child as T;
					if (child is DependencyObject)
					{
						var res = FindVisualChild<T>(child as DependencyObject);
						if (res == null) continue;
						return res;
					}
				}
			}
			return null;
		}

		#region INotifyPropertyChanged Members

		/// <summary>
		///     Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///     Called when property value changes.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Static fields

		#endregion

		#region Private members

		internal void ComponentChanged()
		{
			foreach (var propertyItem in Properties)
			{
				propertyItem.OnComponentChanged();
			}
		}

		private void DoReload()
		{
			//Clear the metadata
			MetadataRepository.Clear();
			if (SelectedObject == null)
			{
				Categories = new GridEntryCollection<CategoryItem>();
				Properties = new GridEntryCollection<PropertyItem>();
			}
			else
			{
				// Collect BrowsableCategoryAttribute items
				var categoryAttributes = PropertyGridUtils.GetAttributes<BrowsableCategoryAttribute>(SelectedObject);
				browsableCategories = new List<BrowsableCategoryAttribute>(categoryAttributes);

				// Collect BrowsablePropertyAttribute items
				var propertyAttributes = PropertyGridUtils.GetAttributes<BrowsablePropertyAttribute>(SelectedObject);
				browsableProperties = new List<BrowsablePropertyAttribute>(propertyAttributes);

				// Collect categories and properties
				var properties = CollectProperties(currentObjects);

				// TODO: This needs more elegant implementation
				var categories = new GridEntryCollection<CategoryItem>(CollectCategories(properties));
				if (_categories != null && _categories.Count > 0)
					CopyCategoryFrom(_categories, categories);

				// Fetch and apply category orders
				var categoryOrders = PropertyGridUtils.GetAttributes<CategoryOrderAttribute>(SelectedObject);
				foreach (var orderAttribute in categoryOrders)
				{
					var category = categories[orderAttribute.Category];
					// don't apply Order if it was applied before (Order equals zero or more), 
					// so the first discovered Order value for the same category wins
					if (category != null && category.Order < 0)
						category.Order = orderAttribute.Order;
				}

				Categories = categories; //new CategoryCollection(CollectCategories(properties));
				Properties = new GridEntryCollection<PropertyItem>(properties);
				
			}
		}

		private void PropertyGrid_Unloaded(object sender, RoutedEventArgs e)
		{
			TypeDescriptor.Refreshed -= OnTypeDescriptorRefreshed;
		}

		private void PropertyGrid_Loaded(object sender, RoutedEventArgs e)
		{
			TypeDescriptor.Refreshed += OnTypeDescriptorRefreshed;	
		}


		private void OnTypeDescriptorRefreshed(RefreshEventArgs e)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(new RefreshEventHandler(OnTypeDescriptorRefreshedInvoke), e);
			}
			else
			{
				OnTypeDescriptorRefreshedInvoke(e);
			}
		}

		private void OnTypeDescriptorRefreshedInvoke(RefreshEventArgs e)
		{
			if (currentObjects != null)
			{
				for (var i = 0; i < currentObjects.Length; i++)
				{
					var typeChanged = e.TypeChanged;
					if (currentObjects[i] == e.ComponentChanged ||
					    typeChanged != null && typeChanged.IsInstanceOfType(currentObjects[i]))
					{
						// clear our property hashes
						//DoReload();
						UpdateBrowsable();
						return;
					}
				}
			}
		}

		private static void CopyCategoryFrom(GridEntryCollection<CategoryItem> oldValue, IEnumerable<CategoryItem> newValue)
		{
			foreach (var category in newValue)
			{
				var prev = oldValue[category.Name];
				if (prev == null) continue;

				category.IsExpanded = prev.IsExpanded;
			}
		}

		private void OnPropertyItemValueChanged(PropertyItem property, object[] oldValue, object newValue)
		{
			RaisePropertyValueChangedEvent(property, oldValue);
		}

		private void HookPropertyChanged(PropertyItem item)
		{
			if (item == null) return;
			item.ValueChanged += OnPropertyItemValueChanged;
		}

		private void UnhookPropertyChanged(PropertyItem item)
		{
			if (item == null) return;
			item.ValueChanged -= OnPropertyItemValueChanged;
		}

		#endregion

		#region Fields

		private List<BrowsablePropertyAttribute> browsableProperties = new List<BrowsablePropertyAttribute>();
		private List<BrowsableCategoryAttribute> browsableCategories = new List<BrowsableCategoryAttribute>();

		private IEffect[] currentObjects;

		#endregion

		#region Events

		#region PropertyEditingStarted Event (Bubble)

		#endregion PropertyEditingStarted event (Bubble)

		#region PropertyEditingFinished Event (Bubble)

		#endregion PropertyEditingFinished Event (Bubble)

		#region PropertyValueChanged Event (Bubble)

		#endregion PropertyValueChanged event (Bubble)

		#region SelectedObjectsChanged

		#endregion

		#endregion

		#region Properties

		#region ItemsBackground

		#endregion

		#region ItemsForeground

		#endregion

		private readonly EditorCollection _Editors = new EditorCollection();

		/// <summary>
		///     Gets the editors collection.
		/// </summary>
		/// <value>The editors collection.</value>
		public EditorCollection Editors
		{
			get { return _Editors; }
		}

		#region SelectedObject

		#endregion

		#region SelectedObjects

		#endregion

		#region Properties

		#endregion

		/// <summary>
		///     Gets a value indicating whether this instance has properties.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance has properties; otherwise, <c>false</c>.
		/// </value>
		public bool HasProperties
		{
			get { return _properties != null && _properties.Count > 0; }
		}

		#region PropertyComparer

		#endregion

		#region CategoryComparer

		#endregion

		#region Categories

		#endregion

		/// <summary>
		///     Gets a value indicating whether this instance has categories.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance has categories; otherwise, <c>false</c>.
		/// </value>
		public bool HasCategories
		{
			get { return _categories != null && _categories.Count > 0; }
		}

		#region ShowReadOnlyProperties property

		#endregion

		#region ShowAttachedProperties property

		#endregion

		#region PropertyFilter

		#endregion

		#region PropertyFilterVisibility

		#endregion

		#region PropertyDisplayMode

		#endregion

		#endregion

		#region ctor

		static EffectPropertyEditorGrid()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="EffectPropertyEditorGrid" /> class.
		/// </summary>
		public EffectPropertyEditorGrid()
		{
			EventManager.RegisterClassHandler(typeof (EffectPropertyEditorGrid), GotFocusEvent, new RoutedEventHandler(ShowDescription),
				true);

			// Assign Layout to be Categorized by default
			Layout = new CategorizedLayout();
			Loaded += PropertyGrid_Loaded;
			Unloaded += PropertyGrid_Unloaded;
			// Wire command bindings
			InitializeCommandBindings();
		}

		private void ShowDescription(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource == null || !(e.OriginalSource is FrameworkElement) ||
			    (e.OriginalSource as FrameworkElement).DataContext == null ||
			    !((e.OriginalSource as FrameworkElement).DataContext is PropertyItemValue) ||
			    ((e.OriginalSource as FrameworkElement).DataContext as PropertyItemValue).ParentProperty == null)
				return;
			var descri = ((e.OriginalSource as FrameworkElement).DataContext as PropertyItemValue).ParentProperty.ToolTip;
			CurrentDescription = descri == null ? "" : descri.ToString();
		}

		#endregion

		#region CurrentDescription

		/// <summary>
		///     Gets or sets the CurrentDescription property.
		/// </summary>
		public string CurrentDescription
		{
			get { return (string) GetValue(CurrentDescriptionProperty); }
			set { SetValue(CurrentDescriptionProperty, value); }
		}

		/// <summary>
		///     Handles changes to the CurrentDescription property.
		/// </summary>
		private static void OnCurrentDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((EffectPropertyEditorGrid) d).OnCurrentDescriptionChanged(e);
		}

		/// <summary>
		///     Provides derived classes an opportunity to handle changes to the CurrentDescription property.
		/// </summary>
		protected virtual void OnCurrentDescriptionChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		#endregion

		#region Internal API

		internal CategoryItem CreateCategory(CategoryAttribute attribute)
		{
			// Check the attribute argument to be passed
			Debug.Assert(attribute != null);
			if (attribute == null) return null;

			// Check browsable restrictions
			//if (!ShouldDisplayCategory(attribute.Category)) return null;

			// Create a new CategoryItem
			var categoryItem = new CategoryItem(this, attribute);
			categoryItem.IsBrowsable = ShouldDisplayCategory(categoryItem.Name);
			categoryItem.IsExpanded = IsCategoryExpanded(categoryItem.Name);
			if (attribute is IOrderableAttribute)
			{
				var attr = (IOrderableAttribute) attribute;
				categoryItem.Order = attr.Order;
			}

			// Return resulting item
			return categoryItem;
		}

		private PropertyItem CreatePropertyItem(PropertyDescriptor descriptor)
		{
			// Check browsable restrictions
			//if (!ShoudDisplayProperty(descriptor)) return null;

			var dpDescriptor = DependencyPropertyDescriptor.FromProperty(descriptor);
			// Provide additional checks for dependency properties
			if (dpDescriptor != null)
			{
				// Check whether dependency properties are not prohibited
				if (PropertyDisplayMode == PropertyDisplayMode.Native) return null;

				// Check whether attached properties are to be displayed
				if (dpDescriptor.IsAttached && !ShowAttachedProperties) return null;
			}
			else
			{
				if (PropertyDisplayMode == PropertyDisplayMode.Dependency) return null;
			}

			// Check whether readonly properties are to be displayed
			if (descriptor.IsReadOnly && !ShowReadOnlyProperties) return null;

			var item = (currentObjects.Length > 1)
				? new PropertyItem(this, currentObjects, descriptor)
				: new PropertyItem(this, SelectedObject, descriptor);

			item.IsBrowsable = ShoudDisplayProperty(descriptor);

			return item;
		}

		private bool ShoudDisplayProperty(PropertyDescriptor propertyDescriptor)
		{
			Debug.Assert(propertyDescriptor != null);
			if (propertyDescriptor == null) return false;

			// Check whether owning category is not restricted to ouput
			var showWithinCategory = ShouldDisplayCategory(propertyDescriptor.Category);
			if (!showWithinCategory) return false;

			// Check the explicit declaration
			var attribute = browsableProperties.FirstOrDefault(item => item.PropertyName == propertyDescriptor.Name);
			if (attribute != null) return attribute.Browsable;

			// Check the wildcard
			var wildcard = browsableProperties.FirstOrDefault(item => item.PropertyName == BrowsablePropertyAttribute.All);
			if (wildcard != null) return wildcard.Browsable;

			// Return default/standard Browsable settings for the property
			return propertyDescriptor.IsBrowsable;
		}

		private bool ShouldDisplayCategory(string categoryName)
		{
			if (string.IsNullOrEmpty(categoryName)) return false;

			// Check the explicit declaration
			var attribute = browsableCategories.FirstOrDefault(item => item.CategoryName == categoryName);
			if (attribute != null) return attribute.Browsable;

			// Check the wildcard
			var wildcard = browsableCategories.FirstOrDefault(item => item.CategoryName == BrowsableCategoryAttribute.All);
			if (wildcard != null) return wildcard.Browsable;

			// Allow by default if no restrictions were applied
			return true;
		}

		private bool IsCategoryExpanded(string categoryName)
		{
			if (string.IsNullOrEmpty(categoryName)) return false;

			// Check the explicit declaration
			var attribute = browsableCategories.FirstOrDefault(item => item.CategoryName == categoryName);
			if (attribute != null) return attribute.Expanded;

			// Check the wildcard
			var wildcard = browsableCategories.FirstOrDefault(item => item.CategoryName == BrowsableCategoryAttribute.All);
			if (wildcard != null) return wildcard.Expanded;

			// Expanded by default if no restrictions were applied
			return true;
		}

		#endregion
	}
}