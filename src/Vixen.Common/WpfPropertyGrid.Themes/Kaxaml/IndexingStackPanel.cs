﻿/*
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
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls.WpfPropertyGrid.Themes.Kaxaml
{
  public class IndexingStackPanel : StackPanel
  {
    #region Index (Attached Dependency Property)

    public static int GetIndex(DependencyObject obj)
    {
      return (int)obj.GetValue(IndexProperty);
    }

    public static void SetIndex(DependencyObject obj, int value)
    {
      obj.SetValue(IndexProperty, value);
    }

    public static readonly DependencyProperty IndexProperty =
        DependencyProperty.RegisterAttached("Index", typeof(int), typeof(IndexingStackPanel), new UIPropertyMetadata(default(int)));

    #endregion

    #region SelectionLocation

    public static SelectionLocation GetSelectionLocation(DependencyObject obj)
    {
      return (SelectionLocation)obj.GetValue(SelectionLocationProperty);
    }

    public static void SetSelectionLocation(DependencyObject obj, SelectionLocation value)
    {
      obj.SetValue(SelectionLocationProperty, value);
    }

    public static readonly DependencyProperty SelectionLocationProperty =
        DependencyProperty.RegisterAttached("SelectionLocation", typeof(SelectionLocation), typeof(IndexingStackPanel), new UIPropertyMetadata(default(SelectionLocation)));

    #endregion

    #region StackLocation (Attached Dependency Property)

    public static StackLocation GetStackLocation(DependencyObject obj)
    {
      return (StackLocation)obj.GetValue(StackLocationProperty);
    }

    public static void SetStackLocation(DependencyObject obj, StackLocation value)
    {
      obj.SetValue(StackLocationProperty, value);
    }

    public static readonly DependencyProperty StackLocationProperty =
        DependencyProperty.RegisterAttached("StackLocation", typeof(StackLocation), typeof(IndexingStackPanel), new UIPropertyMetadata(default(StackLocation)));

    #endregion

    #region IndexOddEven (Attached DependencyProperty)

    public static IndexOddEven GetIndexOddEven(DependencyObject obj)
    {
      return (IndexOddEven)obj.GetValue(IndexOddEvenProperty);
    }

    public static void SetIndexOddEven(DependencyObject obj, IndexOddEven value)
    {
      obj.SetValue(IndexOddEvenProperty, value);
    }

    public static readonly DependencyProperty IndexOddEvenProperty =
        DependencyProperty.RegisterAttached("IndexOddEven", typeof(IndexOddEven), typeof(IndexingStackPanel), new UIPropertyMetadata(default(IndexOddEven)));


    #endregion

    #region Overrides

    protected override Size MeasureOverride(Size constraint)
    {
      int index = 0;
      bool isEven = true;
      bool foundSelected = false;

      foreach (UIElement element in this.Children)
      {

        if (this.IsItemsHost)
        {
          Selector SelectorParent = this.TemplatedParent as Selector;

          if (SelectorParent != null)
          {
            UIElement selectedElement = (SelectorParent.ItemContainerGenerator.ContainerFromItem(SelectorParent.SelectedItem) as UIElement);

            if (selectedElement != null)
            {
              if (element == selectedElement)
              {
                element.SetValue(SelectionLocationProperty, SelectionLocation.Selected);
                foundSelected = true;
              }
              else if (foundSelected)
              {
                element.SetValue(SelectionLocationProperty, SelectionLocation.After);
              }
              else
              {
                element.SetValue(SelectionLocationProperty, SelectionLocation.Before);
              }
            }
          }
        }

        // StackLocation

        if (Children.Count - 1 == 0)
        {
          element.SetValue(StackLocationProperty, StackLocation.FirstAndLast);
        }
        else if (index == 0)
        {
          element.SetValue(StackLocationProperty, StackLocation.First);
        }
        else if (index == Children.Count - 1)
        {
          element.SetValue(StackLocationProperty, StackLocation.Last);
        }
        else
        {
          element.SetValue(StackLocationProperty, StackLocation.Middle);
        }

        // IndexOddEven

        if (isEven)
        {
          element.SetValue(IndexOddEvenProperty, IndexOddEven.Even);
        }
        else
        {
          element.SetValue(IndexOddEvenProperty, IndexOddEven.Odd);
        }

        element.SetValue(IndexProperty, index);
        index++;

      }

      return base.MeasureOverride(constraint);
    }

    #endregion
  }

  public enum StackLocation
  {
    First,
    Middle,
    Last,
    FirstAndLast
  }

  public enum SelectionLocation
  {
    Before,
    Selected,
    After
  }

  public enum IndexOddEven
  {
    Odd,
    Even
  }
}
