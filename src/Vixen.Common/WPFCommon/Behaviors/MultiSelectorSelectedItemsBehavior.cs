using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace Common.WPFCommon.Behaviors
{
	/// <summary>
	/// Synchronizes a <see cref="MultiSelector" /> selected-items collection with a view model collection.
	/// </summary>
	public sealed class MultiSelectorSelectedItemsBehavior : Behavior<MultiSelector>
	{
		private bool _isUpdatingSource;
		private bool _isUpdatingTarget;

		/// <summary>
		/// Identifies the <see cref="SelectedItems" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty SelectedItemsProperty =
			DependencyProperty.Register(
				nameof(SelectedItems),
				typeof(IList),
				typeof(MultiSelectorSelectedItemsBehavior),
				new UIPropertyMetadata(null, SelectedItemsChanged));

		/// <summary>
		/// Gets or sets the selected items collection to synchronize.
		/// </summary>
		public IList SelectedItems
		{
			get => (IList)GetValue(SelectedItemsProperty);
			set => SetValue(SelectedItemsProperty, value);
		}

		/// <inheritdoc />
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
			RefreshTargetSelection();
			SubscribeToSourceCollection(SelectedItems as INotifyCollectionChanged);
		}

		/// <inheritdoc />
		protected override void OnDetaching()
		{
			AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
			UnsubscribeFromSourceCollection(SelectedItems as INotifyCollectionChanged);
			base.OnDetaching();
		}

		private static void SelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (dependencyObject is not MultiSelectorSelectedItemsBehavior behavior)
			{
				return;
			}

			behavior.UnsubscribeFromSourceCollection(e.OldValue as INotifyCollectionChanged);
			behavior.RefreshTargetSelection();
			behavior.SubscribeToSourceCollection(e.NewValue as INotifyCollectionChanged);
		}

		private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_isUpdatingSource || AssociatedObject == null)
			{
				return;
			}

			try
			{
				_isUpdatingTarget = true;

				if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					AssociatedObject.SelectedItems.Clear();
					return;
				}

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
						if (!AssociatedObject.SelectedItems.Contains(item))
						{
							AssociatedObject.SelectedItems.Add(item);
						}
					}
				}
			}
			finally
			{
				_isUpdatingTarget = false;
			}
		}

		private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_isUpdatingTarget || SelectedItems == null)
			{
				return;
			}

			try
			{
				_isUpdatingSource = true;

				foreach (var item in e.RemovedItems)
				{
					SelectedItems.Remove(item);
				}

				foreach (var item in e.AddedItems)
				{
					if (!SelectedItems.Contains(item))
					{
						SelectedItems.Add(item);
					}
				}
			}
			finally
			{
				_isUpdatingSource = false;
			}
		}

		private void RefreshTargetSelection()
		{
			if (AssociatedObject == null)
			{
				return;
			}

			try
			{
				_isUpdatingTarget = true;
				AssociatedObject.SelectedItems.Clear();

				if (SelectedItems == null)
				{
					return;
				}

				foreach (var item in SelectedItems)
				{
					AssociatedObject.SelectedItems.Add(item);
				}
			}
			finally
			{
				_isUpdatingTarget = false;
			}
		}

		private void SubscribeToSourceCollection(INotifyCollectionChanged collection)
		{
			if (collection != null)
			{
				collection.CollectionChanged += SourceCollectionChanged;
			}
		}

		private void UnsubscribeFromSourceCollection(INotifyCollectionChanged collection)
		{
			if (collection != null)
			{
				collection.CollectionChanged -= SourceCollectionChanged;
			}
		}
	}
}
