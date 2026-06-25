using System.Windows;
using System.Windows.Input;
using Common.WPFCommon.Utils;
using Microsoft.Xaml.Behaviors;
using VixenModules.Property.State.Setup.Models;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using TreeView = System.Windows.Controls.TreeView;
using TreeViewItem = System.Windows.Controls.TreeViewItem;

namespace VixenModules.Property.State.Setup.Behaviors
{
	/// <summary>
	/// Adds assignment-tree range selection gestures to a State assignment <see cref="TreeView" />.
	/// </summary>
	public sealed class StateAssignmentTreeSelectionBehavior : Behavior<TreeView>
	{
		/// <summary>
		/// Identifies the <see cref="SelectionController" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty SelectionControllerProperty =
			DependencyProperty.Register(
				nameof(SelectionController),
				typeof(StateAssignmentTreeSelectionController),
				typeof(StateAssignmentTreeSelectionBehavior));

		/// <summary>
		/// Gets or sets the controller that receives assignment-tree selection gestures.
		/// </summary>
		/// <value>The controller that receives assignment-tree selection gestures.</value>
		public StateAssignmentTreeSelectionController? SelectionController
		{
			get => (StateAssignmentTreeSelectionController?)GetValue(SelectionControllerProperty);
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
				FindAssignmentNode(e.OriginalSource) is not { } node)
			{
				return;
			}

			switch (Keyboard.Modifiers)
			{
				case ModifierKeys.Control:
					SelectionController.ToggleSelection(node);
					e.Handled = true;
					break;
				case ModifierKeys.Shift:
					SelectionController.SelectRange(node);
					e.Handled = true;
					break;
				case ModifierKeys.None:
					SelectionController.SelectSingle(node);
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

			SelectionController.ToggleCheckedStateForSelectedNodes();
			e.Handled = true;
		}

		private static StateAssignmentTreeNode? FindAssignmentNode(object eventSource)
		{
			if (eventSource is not DependencyObject source)
			{
				return null;
			}

			var treeViewItem = source.FindVisualAncestor<TreeViewItem>();
			return treeViewItem?.DataContext as StateAssignmentTreeNode;
		}
	}
}
