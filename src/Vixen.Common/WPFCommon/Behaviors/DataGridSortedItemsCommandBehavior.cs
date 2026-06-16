using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;

namespace Common.WPFCommon.Behaviors
{
	/// <summary>
	/// Executes a command with the displayed item order after a <see cref="DataGrid" /> column sort completes.
	/// </summary>
	public sealed class DataGridSortedItemsCommandBehavior : Behavior<DataGrid>
	{
		/// <summary>
		/// Identifies the <see cref="SortedItemsCommand" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty SortedItemsCommandProperty =
			DependencyProperty.Register(
				nameof(SortedItemsCommand),
				typeof(ICommand),
				typeof(DataGridSortedItemsCommandBehavior),
				new PropertyMetadata(null));

		/// <summary>
		/// Identifies the <see cref="ClearSortAfterCommand" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty ClearSortAfterCommandProperty =
			DependencyProperty.Register(
				nameof(ClearSortAfterCommand),
				typeof(bool),
				typeof(DataGridSortedItemsCommandBehavior),
				new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets the command to invoke after the grid sort completes.
		/// </summary>
		public ICommand SortedItemsCommand
		{
			get => (ICommand)GetValue(SortedItemsCommandProperty);
			set => SetValue(SortedItemsCommandProperty, value);
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the active grid sort is cleared after the command executes.
		/// </summary>
		public bool ClearSortAfterCommand
		{
			get => (bool)GetValue(ClearSortAfterCommandProperty);
			set => SetValue(ClearSortAfterCommandProperty, value);
		}

		/// <inheritdoc />
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.Sorting += AssociatedObjectOnSorting;
		}

		/// <inheritdoc />
		protected override void OnDetaching()
		{
			AssociatedObject.Sorting -= AssociatedObjectOnSorting;
			base.OnDetaching();
		}

		private void AssociatedObjectOnSorting(object sender, DataGridSortingEventArgs e)
		{
			AssociatedObject.Dispatcher.BeginInvoke(
				new Action(ExecuteSortedItemsCommand),
				DispatcherPriority.Background);
		}

		private void ExecuteSortedItemsCommand()
		{
			var command = SortedItemsCommand;
			if (command == null || AssociatedObject == null)
			{
				return;
			}

			var sortedItems = AssociatedObject.Items
				.Cast<object>()
				.Where(item => item != CollectionView.NewItemPlaceholder)
				.ToList();

			if (command.CanExecute(sortedItems))
			{
				command.Execute(sortedItems);
			}

			if (ClearSortAfterCommand)
			{
				ClearSort();
			}
		}

		private void ClearSort()
		{
			AssociatedObject.Items.SortDescriptions.Clear();
			foreach (var column in AssociatedObject.Columns)
			{
				column.SortDirection = null;
			}

			AssociatedObject.Items.Refresh();
		}
	}
}
