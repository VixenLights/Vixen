namespace VixenModules.Property.State.Setup.Models
{
	/// <summary>
	/// Manages temporary range selection for a State item assignment tree.
	/// </summary>
	internal sealed class StateAssignmentTreeSelectionController
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
				return;
			}

			node.IsSelected = true;
			_anchorNode = node;
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
		}

		/// <summary>
		/// Toggles the checked assignment state for every selected enabled node.
		/// </summary>
		public void ToggleCheckedStateForSelectedNodes()
		{
			var selectedNodes = GetVisibleNodes()
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
		}

		/// <summary>
		/// Clears all pending assignment node selection.
		/// </summary>
		public void ClearSelection()
		{
			ClearSelectedNodes();
			_anchorNode = null;
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
