using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.WPFCommon.Utils;
using Microsoft.Xaml.Behaviors;

namespace Common.WPFCommon.Behaviors
{
	/// <summary>
	/// Adds conventional range-selection gestures to a <see cref="TreeView" />.
	/// </summary>
	public sealed class TreeViewRangeSelectionBehavior : Behavior<TreeView>
	{
		/// <summary>
		/// Identifies the <see cref="SelectionController" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty SelectionControllerProperty =
			DependencyProperty.Register(
				nameof(SelectionController),
				typeof(ITreeViewRangeSelectionController),
				typeof(TreeViewRangeSelectionBehavior));

		/// <summary>
		/// Gets or sets the controller that receives tree selection gestures.
		/// </summary>
		/// <value>The controller that receives tree selection gestures.</value>
		public ITreeViewRangeSelectionController SelectionController
		{
			get => (ITreeViewRangeSelectionController)GetValue(SelectionControllerProperty);
			set => SetValue(SelectionControllerProperty, value);
		}

		/// <inheritdoc />
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.AddHandler(
				UIElement.PreviewMouseLeftButtonDownEvent,
				new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown),
				true);
			AssociatedObject.AddHandler(
				UIElement.KeyDownEvent,
				new KeyEventHandler(OnKeyDown),
				true);
		}

		/// <inheritdoc />
		protected override void OnDetaching()
		{
			AssociatedObject.RemoveHandler(
				UIElement.PreviewMouseLeftButtonDownEvent,
				new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown));
			AssociatedObject.RemoveHandler(
				UIElement.KeyDownEvent,
				new KeyEventHandler(OnKeyDown));
			base.OnDetaching();
		}

		private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (SelectionController == null ||
				FindItem(e.OriginalSource) is not { } item)
			{
				return;
			}

			switch (Keyboard.Modifiers)
			{
				case ModifierKeys.Control:
					SelectionController.ToggleSelection(item);
					e.Handled = true;
					break;
				case ModifierKeys.Shift:
					SelectionController.SelectRange(item);
					e.Handled = true;
					break;
				case ModifierKeys.None:
					SelectionController.SelectSingle(item);
					break;
			}
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (SelectionController == null ||
				e.Key != Key.Space)
			{
				return;
			}

			SelectionController.ToggleCheckedStateForSelectedItems();
			e.Handled = true;
		}

		private static object FindItem(object eventSource)
		{
			if (eventSource is not DependencyObject source)
			{
				return null;
			}

			var treeViewItem = source.FindVisualAncestor<TreeViewItem>();
			return treeViewItem?.DataContext;
		}
	}
}
