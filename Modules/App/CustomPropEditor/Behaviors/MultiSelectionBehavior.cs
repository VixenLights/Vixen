using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using Catel.Linq;
using Common.WPFCommon.Utils;

namespace VixenModules.App.CustomPropEditor.Behaviors
{
	public class MultiSelectionBehavior : Behavior<ListView>
	{

		#region AnchorItem (Private Dependency Property)

		/// <summary>
		/// The dependency property definition for the AnchorItem property.
		/// </summary>
		private static readonly DependencyProperty AnchorItemProperty = DependencyProperty.Register(
			"AnchorItem", typeof(ListViewItem), typeof(MultiSelectionBehavior));

		/// <summary>
		/// Gets or sets the anchor item.
		/// </summary>
		private ListViewItem AnchorItem
		{
			get { return (ListViewItem)GetValue(AnchorItemProperty); }
			set { SetValue(AnchorItemProperty, value); }
		}

		#endregion AnchorItem (Private Dependency Property)


		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnListViewItemKeyDown), true);
			AssociatedObject.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnListViewItemMouseUp), true);

			if (SelectedItems != null)
			{
				AssociatedObject.SelectedItems.Clear();
				foreach (var item in SelectedItems)
				{
					AssociatedObject.SelectedItems.Add(item);
				}
			}
		}

		#region Overrides of Behavior

		/// <inheritdoc />
		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.RemoveHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnListViewItemKeyDown));
			AssociatedObject.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnListViewItemMouseUp));
		}

		#endregion

		private void OnListViewItemKeyDown(object sender, KeyEventArgs e)
		{
			var listViewItem = e.OriginalSource as ListViewItem;
			if (listViewItem == null)
			{
				listViewItem = FindParentTreeViewItem(e.OriginalSource);
			}

			if (listViewItem != null)
			{
				ListViewItem targetItem = null;

				switch (e.Key)
				{
					case Key.Down:
						targetItem = GetRelativeItem(listViewItem, 1);
						break;

					case Key.Space:
						if (Keyboard.Modifiers == ModifierKeys.Control)
						{
							ToggleSingleItem(listViewItem);
						}

						break;

					case Key.Up:
						targetItem = GetRelativeItem(listViewItem, -1);
						break;
				}

				if (targetItem != null)
				{
					switch (Keyboard.Modifiers)
					{
						case ModifierKeys.Control:
							Keyboard.Focus(targetItem);
							break;

						case ModifierKeys.Shift:
							SelectMultipleItemsContinuously(targetItem);
							Keyboard.Focus(targetItem);
							break;

						case ModifierKeys.None:
							SelectSingleItem(targetItem);
							Keyboard.Focus(targetItem);
							break;
					}
				}
			}
		}

		/// <summary>
		/// Selects the specified tree view item, removing any other selections.
		/// </summary>
		/// <param name="listViewItem">The triggering tree view item.</param>
		public void SelectSingleItem(ListViewItem listViewItem)
		{
			AssociatedObject.UnselectAll();
			listViewItem.IsSelected = true;
			AnchorItem = listViewItem;
		}

		/// <summary>
		/// Toggles the selection state of the specified tree view item.
		/// </summary>
		/// <param name="listViewItem">The triggering tree view item.</param>
		public void ToggleSingleItem(ListViewItem listViewItem)
		{

			listViewItem.IsSelected = !listViewItem.IsSelected;
			if (AnchorItem == null)
			{
				if (listViewItem.IsSelected)
				{
					AnchorItem = listViewItem;
				}
			}
			else if (SelectedItems.Count == 0)
			{
				AnchorItem = null;
			}
		}

		/// <summary>
		/// Selects a range of consecutive items from the specified tree view item to the anchor (if exists).
		/// </summary>
		/// <param name="treeViewItem">The triggering tree view item.</param>
		public void SelectMultipleItemsContinuously(ListViewItem treeViewItem)
		{
			if (AnchorItem != null)
			{
				if (ReferenceEquals(AnchorItem, treeViewItem))
				{
					SelectSingleItem(treeViewItem);
					return;
				}

				var isBetweenAnchors = false;

				//var items = new List<ListViewItem>();
				//foreach (var associatedObjectSelectedItem in AssociatedObject.Items)
				//{
				//	items.Add(AssociatedObject.ItemContainerGenerator.ContainerFromItem(associatedObjectSelectedItem) as ListViewItem);
				//}

				var items = GetItemsRecursively<ListViewItem>(AssociatedObject);

				AssociatedObject.UnselectAll();

				foreach (var item in items)
				{
					if (ReferenceEquals(item, treeViewItem) || ReferenceEquals(item, AnchorItem))
					{
						// Toggle isBetweenAnchors when first item is found, and back again when last item is found.
						isBetweenAnchors = !isBetweenAnchors;

						item.IsSelected = true;
					}
					else if (isBetweenAnchors)
					{
						item.IsSelected = true;
					}
				}
			}
		}

		/// <summary>
		/// Called when a TreeViewItem receives a right mouse up event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		/// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the
		/// event data.
		/// </param>
		private void OnListViewItemMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				return;
			}

			var listViewItem = FindParentTreeViewItem(e.OriginalSource);
			if (listViewItem != null && listViewItem.IsSelected)
			{
				AnchorItem = listViewItem;
				//Keyboard.Focus(listViewItem);
			}
		}

		public IList SelectedItems
		{
			get { return (IList)GetValue(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemsProperty =
			DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectionBehavior), new UIPropertyMetadata(null, SelectedItemsChanged));

		private static void SelectedItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var behavior = o as MultiSelectionBehavior;
			if (behavior == null)
				return;

			var oldValue = e.OldValue as INotifyCollectionChanged;
			var newValue = e.NewValue as INotifyCollectionChanged;

			if (oldValue != null)
			{
				oldValue.CollectionChanged -= behavior.SourceCollectionChanged;
				behavior.AssociatedObject.SelectionChanged -= behavior.MultiSelectorSelectionChanged;
			}
			if (newValue != null)
			{
				behavior.AssociatedObject.SelectedItems.Clear();
				foreach (var item in (IEnumerable)newValue)
				{
					behavior.AssociatedObject.SelectedItems.Add(item);
				}

				behavior.AssociatedObject.SelectionChanged += behavior.MultiSelectorSelectionChanged;
				newValue.CollectionChanged += behavior.SourceCollectionChanged;
			}
		}

		private bool _isUpdatingTarget;
		private bool _isUpdatingSource;

		void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_isUpdatingSource)
				return;

			try
			{
				_isUpdatingTarget = true;

				if (e.OldItems != null)
				{
					foreach (var item in e.OldItems)
					{
						AssociatedObject.SelectedItems.Remove(item);
					}
				}

				if (e.NewItems != null)
				{
					foreach (var item in e.NewItems)
					{
						AssociatedObject.SelectedItems.Add(item);
					}
				}

				if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					AssociatedObject.SelectedItems.Clear();
				}
			}
			finally
			{
				_isUpdatingTarget = false;
			}
		}

		private void MultiSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_isUpdatingTarget)
				return;

			var selectedItems = this.SelectedItems;
			if (selectedItems == null)
				return;

			try
			{
				_isUpdatingSource = true;

				foreach (var item in e.RemovedItems)
				{
					selectedItems.Remove(item);
				}

				foreach (var item in e.AddedItems)
				{
					selectedItems.Add(item);
				}
			}
			finally
			{
				_isUpdatingSource = false;
			}
		}

		/// <summary>
		/// Attempts to find the parent TreeViewItem from the specified event source.
		/// </summary>
		/// <param name="eventSource">The event source.</param>
		/// <returns>The parent TreeViewItem, otherwise null.</returns>
		private static ListViewItem FindParentTreeViewItem(object eventSource)
		{
			var source = eventSource as DependencyObject;

			var listViewItem = source?.FindVisualAncestor<ListViewItem>();

			return listViewItem;
		}

		/// <summary>
		/// Gets items of the specified type recursively from the specified parent item.
		/// </summary>
		/// <typeparam name="T">The type of item to retrieve.</typeparam>
		/// <param name="parentItem">The parent item.</param>
		/// <returns>The list of items within the parent item, may be empty.</returns>
		private static IList<T> GetItemsRecursively<T>(ListView parentItem)
			where T : ListBoxItem
		{
			if (parentItem == null)
			{
				throw new ArgumentNullException(nameof(parentItem));
			}

			var items = new List<T>();

			for (int i = 0; i < parentItem.Items.Count; i++)
			{
				var item = parentItem.ItemContainerGenerator.ContainerFromIndex(i) as T;
				if (item != null)
				{
					items.Add(item);
					//items.AddRange(GetItemsRecursively<T>(item));
				}
			}

			return items;
		}

		/// <summary>
		/// Gets an item with a relative position (e.g. +1, -1) to the specified item.
		/// </summary>
		/// <remarks>This deliberately works against a flattened collection (i.e. no hierarchy).</remarks>
		/// <typeparam name="T">The type of item to retrieve.</typeparam>
		/// <param name="item">The item.</param>
		/// <param name="relativePosition">The relative position offset (e.g. +1, -1).</param>
		/// <returns>The item in the relative position, otherwise null.</returns>
		private T GetRelativeItem<T>(T item, int relativePosition)
			where T : ListViewItem
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			var items = GetItemsRecursively<T>(AssociatedObject);
			int index = items.IndexOf(item);
			if (index >= 0)
			{
				var relativeIndex = index + relativePosition;
				if (relativeIndex >= 0 && relativeIndex < items.Count)
				{
					return items[relativeIndex];
				}
			}

			return null;
		}
	}
}
