using System;
using System.Windows;
using System.Windows.Controls;

namespace Common.WPFCommon.Behaviors
{
	public static class ListViewItemBehavior
	{
		#region IsBroughtIntoViewWhenSelected

		/// <summary>
		/// Gets the IsBroughtIntoViewWhenSelected value
		/// </summary>
		/// <param name="listBoxItem"></param>
		/// <returns></returns>
		public static bool GetIsBroughtIntoViewWhenSelected(ListViewItem listBoxItem)
		{
			return (bool)listBoxItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);
		}

		/// <summary>
		/// Sets the IsBroughtIntoViewWhenSelected value
		/// </summary>
		/// <param name="listBoxItem"></param>
		/// <param name="value"></param>
		public static void SetIsBroughtIntoViewWhenSelected(
		  ListViewItem listBoxItem, bool value)
		{
			listBoxItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
		}

		/// <summary>
		/// Determins if the ListViewItem is bought into view when enabled
		/// </summary>
		public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
			DependencyProperty.RegisterAttached(
			"IsBroughtIntoViewWhenSelected",
			typeof(bool),
			typeof(ListViewItemBehavior),
			new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

		/// <summary>
		/// Action to take when item is brought into view
		/// </summary>
		/// <param name="depObj"></param>
		/// <param name="e"></param>
		static void OnIsBroughtIntoViewWhenSelectedChanged(
		  DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			ListViewItem item = depObj as ListViewItem;
			if (item == null)
				return;

			if (e.NewValue is bool == false)
				return;

			if ((bool)e.NewValue)
				item.Selected += OnListViewItemSelected;
			else
				item.Selected -= OnListViewItemSelected;
		}

		static void OnListViewItemSelected(object sender, RoutedEventArgs e)
		{
			// Only react to the Selected event raised by the ListViewItem 
			// whose IsSelected property was modified.  Ignore all ancestors 
			// who are merely reporting that a descendant's Selected fired. 
			if (!Object.ReferenceEquals(sender, e.OriginalSource))
				return;

			if (e.OriginalSource is ListViewItem item)
			{
				ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
				listView?.Focus();
				item.Focus();
				listView?.ScrollIntoView(item);
				item.BringIntoView();
			}
		}

		#endregion // IsBroughtIntoViewWhenSelected
	}
}
