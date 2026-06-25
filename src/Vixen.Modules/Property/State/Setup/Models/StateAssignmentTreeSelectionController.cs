using Common.WPFCommon.Behaviors;

namespace VixenModules.Property.State.Setup.Models
{
	/// <summary>
	/// Manages temporary range selection for a State item assignment tree.
	/// </summary>
	public sealed class StateAssignmentTreeSelectionController : ITreeViewRangeSelectionController
	{
		private readonly IReadOnlyCollection<StateAssignmentTreeNode> _roots;
		private StateAssignmentTreeNode? _anchorNode;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateAssignmentTreeSelectionController"/> class.
		/// </summary>
		/// <param name="roots">The assignment tree roots to manage.</param>
		public StateAssignmentTreeSelectionController(IEnumerable<StateAssignmentTreeNode> roots)
		{
			ArgumentNullException.ThrowIfNull(roots);

			_roots = roots.ToList();
		}

		/// <summary>
		/// Occurs when the temporary assignment-tree selection changes.
		/// </summary>
		public event EventHandler? SelectionChanged;

		/// <summary>
		/// Gets the number of enabled assignment nodes selected for a pending batch operation.
		/// </summary>
		/// <value>The number of enabled assignment nodes selected for a pending batch operation.</value>
		public int SelectedCount => GetAllNodes()
			.Count(node => node.IsEnabled && node.IsSelected);

		/// <summary>
		/// Selects one assignment node and clears any previous selection.
		/// </summary>
		/// <param name="node">The assignment node to select.</param>
		public void SelectSingle(StateAssignmentTreeNode node)
		{
			ArgumentNullException.ThrowIfNull(node);

			ClearSelectedNodes();
			if (!node.IsEnabled)
			{
				_anchorNode = null;
				SelectionChanged?.Invoke(this, EventArgs.Empty);
				return;
			}

			node.IsSelected = true;
			_anchorNode = node;
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Adds or removes one assignment node from the current selection.
		/// </summary>
		/// <param name="node">The assignment node to toggle.</param>
		public void ToggleSelection(StateAssignmentTreeNode node)
		{
			ArgumentNullException.ThrowIfNull(node);

			if (!node.IsEnabled)
			{
				return;
			}

			node.IsSelected = !node.IsSelected;
			if (node.IsSelected)
			{
				_anchorNode = node;
			}
			else if (ReferenceEquals(_anchorNode, node))
			{
				_anchorNode = GetAllNodes().FirstOrDefault(selectedNode => selectedNode.IsEnabled && selectedNode.IsSelected);
			}

			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Selects the visible enabled assignment node range between the anchor and the specified node.
		/// </summary>
		/// <param name="node">The assignment node that completes the range.</param>
		public void SelectRange(StateAssignmentTreeNode node)
		{
			ArgumentNullException.ThrowIfNull(node);

			if (!node.IsEnabled)
			{
				return;
			}

			var visibleNodes = GetVisibleNodes()
				.Where(visibleNode => visibleNode.IsEnabled)
				.ToList();
			if (_anchorNode == null ||
				!_anchorNode.IsEnabled ||
				!visibleNodes.Contains(_anchorNode) ||
				!visibleNodes.Contains(node))
			{
				SelectSingle(node);
				return;
			}

			var anchorIndex = visibleNodes.IndexOf(_anchorNode);
			var nodeIndex = visibleNodes.IndexOf(node);
			var startIndex = Math.Min(anchorIndex, nodeIndex);
			var endIndex = Math.Max(anchorIndex, nodeIndex);

			ClearSelectedNodes();
			for (var index = startIndex; index <= endIndex; index++)
			{
				visibleNodes[index].IsSelected = true;
			}

			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Toggles the checked assignment state for every selected enabled node.
		/// </summary>
		public void ToggleCheckedStateForSelectedNodes()
		{
			var selectedNodes = GetAllNodes()
				.Where(node => node.IsSelected)
				.ToList();

			foreach (var node in selectedNodes)
			{
				if (!node.IsEnabled)
				{
					node.IsSelected = false;
					continue;
				}

				node.IsChecked = !node.IsChecked;
			}

			ClearDisabledSelections();
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Clears all pending assignment node selection.
		/// </summary>
		public void ClearSelection()
		{
			ClearSelectedNodes();
			_anchorNode = null;
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		void ITreeViewRangeSelectionController.SelectSingle(object item)
		{
			if (item is StateAssignmentTreeNode node)
			{
				SelectSingle(node);
			}
		}

		void ITreeViewRangeSelectionController.ToggleSelection(object item)
		{
			if (item is StateAssignmentTreeNode node)
			{
				ToggleSelection(node);
			}
		}

		void ITreeViewRangeSelectionController.SelectRange(object item)
		{
			if (item is StateAssignmentTreeNode node)
			{
				SelectRange(node);
			}
		}

		void ITreeViewRangeSelectionController.ToggleCheckedStateForSelectedItems()
		{
			ToggleCheckedStateForSelectedNodes();
		}

		private void ClearSelectedNodes()
		{
			foreach (var node in GetAllNodes())
			{
				node.IsSelected = false;
			}
		}

		private IEnumerable<StateAssignmentTreeNode> GetVisibleNodes()
		{
			return _roots.SelectMany(root => root.GetVisibleNodesInDisplayOrder());
		}

		private IEnumerable<StateAssignmentTreeNode> GetAllNodes()
		{
			foreach (var root in _roots)
			{
				yield return root;
				foreach (var child in GetAllChildren(root))
				{
					yield return child;
				}
			}
		}

		private static IEnumerable<StateAssignmentTreeNode> GetAllChildren(StateAssignmentTreeNode node)
		{
			foreach (var child in node.Children)
			{
				yield return child;
				foreach (var descendant in GetAllChildren(child))
				{
					yield return descendant;
				}
			}
		}

		private void ClearDisabledSelections()
		{
			foreach (var node in GetAllNodes().Where(node => !node.IsEnabled && node.IsSelected))
			{
				node.IsSelected = false;
			}
		}
	}
}
