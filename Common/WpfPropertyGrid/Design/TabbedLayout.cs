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

using System.Linq;
using System.Windows.Input;
using System.Windows.Data;

namespace System.Windows.Controls.WpfPropertyGrid.Design
{
  /// <summary>
  /// The default tabbed layout.
  /// </summary>
  public class TabbedLayout : TabControl
  {
    private static readonly IValueConverter visibilityConverter = new BooleanToVisibilityConverter();

    /// <summary>
    /// The fallback header for a tab if no header custom is provided.
    /// </summary>
    public const string DefaultHeader = "Unknown";

    /// <summary>
    /// Initializes the <see cref="TabbedLayout"/> class.
    /// </summary>
    static TabbedLayout()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TabbedLayout), new FrameworkPropertyMetadata(typeof(TabbedLayout)));
    }

    /// <summary>
    /// Gets or sets the item header property.
    /// </summary>
    /// <value>The item header property.</value>
    public string ItemHeaderProperty { get; set; }

    #region Attached Properties

    #region CanClose
    /// <summary>
    /// Identifies the CanClose property. This is an attached property.
    /// </summary>
    public static readonly DependencyProperty CanCloseProperty =
     DependencyProperty.RegisterAttached("CanClose", typeof(bool), typeof(TabbedLayout), new PropertyMetadata(false));

    /// <summary>
    /// Gets the value indicating whether the target element can be closed from UI.
    /// </summary>
    /// <param name="obj">The dependency object</param>
    /// <returns>
    /// 	<c>true</c> if tab item can be closed; otherwise, <c>false</c>.
    /// </returns>  
    public static bool GetCanClose(DependencyObject obj)
    {
      if (obj == null) throw new ArgumentNullException("obj");
      return (bool)obj.GetValue(CanCloseProperty);
    }

    /// <summary>
    /// Sets the value indicating whether the target element can be closed from UI.
    /// </summary>
    /// <param name="obj">The dependency object.</param>
    /// <param name="value">if set to <c>true</c> the tab item can be closed from UI.</param>
    public static void SetCanClose(DependencyObject obj, bool value)
    {
      if (obj == null) throw new ArgumentNullException("obj");
      obj.SetValue(CanCloseProperty, value);
    } 
    #endregion

    #region Header
    /// <summary>
    /// Identifies the Header property. This is an attached property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
      DependencyProperty.RegisterAttached("Header", typeof(string), typeof(TabbedLayout), new PropertyMetadata("Unknown"));

    /// <summary>
    /// Gets the header.
    /// </summary>
    /// <param name="obj">The dependency object.</param>
    /// <returns>Tab header</returns>
    public static string GetHeader(DependencyObject obj)
    {
      if (obj == null) throw new ArgumentNullException("obj");
      return (string)obj.GetValue(HeaderProperty);
    }

    /// <summary>
    /// Sets the header.
    /// </summary>
    /// <param name="obj">The dependency object.</param>
    /// <param name="value">The value.</param>
    public static void SetHeader(DependencyObject obj, string value)
    {
      if (obj == null) throw new ArgumentNullException("obj");
      if (string.IsNullOrEmpty(value)) value = DefaultHeader;
      obj.SetValue(HeaderProperty, value);
    } 
    #endregion

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TabbedLayout"/> class.
    /// </summary>
    public TabbedLayout()
    {      
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.ClosePropertyTab, OnClosePropertyTabCommand, CanClosePropertyTab));
      CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowExtendedEditor, OnShowExtendedEditor));      
    }

    private void CanClosePropertyTab(object sender, CanExecuteRoutedEventArgs e)
    {      
      var layoutItem = SelectedItem as TabbedLayoutItem;
      if (layoutItem != null)
      {
        e.CanExecute = layoutItem.CanClose;        
      }
      else
      {
        var obj = SelectedItem as DependencyObject;
        if (obj != null)
        {
          e.CanExecute = GetCanClose(obj);
        }
      }
    }

    // TODO: Optimize implementation
    // TODO: move logic to public api
    // It should be possible creating/opening extended tabs from code
    private void OnShowExtendedEditor(object sender, ExecutedRoutedEventArgs e)
    {
      var value = e.Parameter as PropertyItemValue;
      if (value == null) return;
      var property = value.ParentProperty;
      if (property == null) return;

      // Try getting already opened extended tab
      var extendedTab = FindExtendedEditorTab(property);
      if (extendedTab != null)
      {
        // Activate alreay opened tab
        SelectedItem = extendedTab;
      }
      else
      {
        // TODO: Access the PropertyEditorGrid to get TypeEditor or PropertyEditor ExtendedTemplate here!
        
        // Check whether property value editor is actually an extended one
        if (property.Editor.ExtendedTemplate == null) return;

        // create new extended tab, add to the tabs collection and activate it
        extendedTab = new ExtendedPropertyEditorTab(property);

        // Try inserting extended tab after currently selected one
        if (SelectedItem != null)
          Items.Insert(Items.IndexOf(SelectedItem) + 1, extendedTab);
        else
          Items.Add(extendedTab);

        // Activate extended tab
        SelectedItem = extendedTab;
      }
    }

    /// <summary>
    /// Finds the extended editor tab.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>Tab associated with property.</returns>
    public TabbedLayoutItem FindExtendedEditorTab(PropertyItem property)
    {
      return Items
        .OfType<ExtendedPropertyEditorTab>()
        .FirstOrDefault(tab => tab.Property == property);
    }

    private void OnClosePropertyTabCommand(object sender, ExecutedRoutedEventArgs e)
    {      
      if (SelectedItem != null)
        Items.Remove(SelectedItem);
    }

    #region ItemsControl implementation
    /// <summary>
    /// Prepares the specified element to display the specified item.
    /// </summary>
    /// <param name="element">Element used to display the specified item.</param>
    /// <param name="item">Specified item.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);

      if (element == item) return;
      // Processing items not wrapped with TabbedLayoutItem container

      var tab = element as TabbedLayoutItem;
      if (tab != null)
      {
        //TODO: Assign PG as DataContext here?
        //tab.DataContext = item;

        var layout = item as DependencyObject;
        if (layout != null)
        {          
          tab.Header = GetHeader(layout);
          tab.CanClose = GetCanClose(layout);
        }
        else if (!string.IsNullOrEmpty(ItemHeaderProperty))
        {
          var bHeader = new Binding(ItemHeaderProperty) 
          { 
            Source = item, 
            Mode = BindingMode.OneWay, 
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged 
          };

          tab.SetBinding(HeaderedContentControl.HeaderProperty, bHeader);
        }

        if (item is GridEntry)
        {
          var binding = new Binding("IsVisible")
          {
            Source = item,
            Mode = BindingMode.OneWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Converter = visibilityConverter
          };
          tab.SetBinding(UIElement.VisibilityProperty, binding);
        }

        tab.IsVisibleChanged += OnTabVisibilityChanged;
      }
    }

    private void OnTabVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var tabItem = sender as TabbedLayoutItem;
      if (tabItem == null || tabItem.DataContext == null) return;

      bool isVisible = (bool)e.NewValue;


      if (isVisible && GetVisibleEntryCount() == 1)
      {
        var visibleEntry = GetFirstVisibleEntry();
        if (SelectedItem != visibleEntry)
          SelectedItem = visibleEntry;
      }
      else if (tabItem.IsSelected)
      {
        if (GetVisibleEntryCount() == 0)
        {
          SelectedItem = null;
          return;
        }

        if (Items.IndexOf(tabItem.DataContext) > 0)
          SelectedIndex--;
        else if (Items.Count > 1)
          SelectedIndex++;
      }
    }

    private GridEntry GetFirstVisibleEntry()
    {
      return Items.OfType<GridEntry>().FirstOrDefault(item => item.IsVisible);
    }

    private int GetVisibleEntryCount()
    {
      return Items.OfType<GridEntry>().Count(item => item.IsVisible);
    }

    /// <summary>
    /// Determines if the specified item is (or is eligible to be) its own ItemContainer.
    /// </summary>
    /// <param name="item">Specified item.</param>
    /// <returns>
    /// Returns true if the item is its own ItemContainer; otherwise, false.
    /// </returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is TabbedLayoutItem;
    }

    /// <summary>
    /// Creates or identifies the element used to display the specified item.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Windows.Controls.TabItem"/>.
    /// </returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new TabbedLayoutItem();
    }   
    #endregion
  }
}
