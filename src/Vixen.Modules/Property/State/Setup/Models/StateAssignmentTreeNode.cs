using System.Collections.ObjectModel;

namespace VixenModules.Property.State.Setup.Models
{
	/// <summary>
	/// Represents one selectable node in a State property assignment tree.
	/// </summary>
	public sealed class StateAssignmentTreeNode : StateBindableBase
	{
		private bool _isChecked;
		private bool _isEnabled = true;
		private bool _isExpanded;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateAssignmentTreeNode"/> class.
		/// </summary>
		/// <param name="snapshot">The element node snapshot represented by this tree node.</param>
		/// <param name="selectedNodeIds">The stable identifiers selected when the tree opens.</param>
		public StateAssignmentTreeNode(StateElementNodeSnapshot snapshot, ISet<Guid> selectedNodeIds)
		{
			ArgumentNullException.ThrowIfNull(snapshot);
			ArgumentNullException.ThrowIfNull(selectedNodeIds);

			Snapshot = snapshot;
			Children = new ObservableCollection<StateAssignmentTreeNode>(
				snapshot.Children.Select(child => new StateAssignmentTreeNode(child, selectedNodeIds)));
			_isChecked = selectedNodeIds.Contains(snapshot.Id);

			foreach (var child in Children)
			{
				child.AssignmentChanged += ChildAssignmentChanged;
			}

			if (_isChecked)
			{
				ClearDescendantSelections();
			}

			UpdateDescendantEnabledState();
		}

		/// <summary>
		/// Occurs when the effective assignments represented by the tree change.
		/// </summary>
		public event EventHandler? AssignmentChanged;

		/// <summary>
		/// Gets the display name of the element node.
		/// </summary>
		/// <value>The display name of the element node.</value>
		public string Name => Snapshot.Name;

		/// <summary>
		/// Gets the child assignment tree nodes.
		/// </summary>
		/// <value>The child assignment tree nodes.</value>
		public ObservableCollection<StateAssignmentTreeNode> Children { get; }

		/// <summary>
		/// Gets or sets a value that indicates whether this element node is explicitly selected.
		/// </summary>
		/// <value><see langword="true" /> if this element node is explicitly selected; otherwise, <see langword="false" />.</value>
		public bool IsChecked
		{
			get => _isChecked;
			set
			{
				if (!IsEnabled || !SetProperty(ref _isChecked, value))
				{
					return;
				}

				if (value)
				{
					ClearDescendantSelections();
				}

				UpdateDescendantEnabledState();
				AssignmentChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets a value that indicates whether this node can be selected directly.
		/// </summary>
		/// <value><see langword="true" /> if this node can be selected directly; otherwise, <see langword="false" />.</value>
		public bool IsEnabled
		{
			get => _isEnabled;
			private set => SetProperty(ref _isEnabled, value);
		}

		/// <summary>
		/// Gets or sets a value that indicates whether this node displays its children.
		/// </summary>
		/// <value><see langword="true" /> if this node displays its children; otherwise, <see langword="false" />.</value>
		public bool IsExpanded
		{
			get => _isExpanded;
			set => SetProperty(ref _isExpanded, value);
		}

		internal StateElementNodeSnapshot Snapshot { get; }

		/// <summary>
		/// Gets the identifiers of explicitly selected nodes, excluding descendants implied by a selected group.
		/// </summary>
		/// <returns>The identifiers of explicitly selected nodes.</returns>
		public IEnumerable<Guid> GetSelectedNodeIds()
		{
			return IsChecked
				? [Snapshot.Id]
				: Children.SelectMany(child => child.GetSelectedNodeIds());
		}

		/// <summary>
		/// Gets the identifiers of leaf nodes effectively included by the current selection.
		/// </summary>
		/// <returns>The identifiers of effectively included leaf nodes.</returns>
		public IEnumerable<Guid> GetEffectiveLeafNodeIds()
		{
			return IsChecked
				? Snapshot.GetLeafNodeIds()
				: Children.SelectMany(child => child.GetEffectiveLeafNodeIds());
		}

		internal bool ExpandCheckedDescendants()
		{
			var hasCheckedDescendant = false;
			foreach (var child in Children)
			{
				var childHasCheckedDescendant = child.ExpandCheckedDescendants();
				hasCheckedDescendant |= child.IsChecked || childHasCheckedDescendant;
			}

			IsExpanded = hasCheckedDescendant;
			return hasCheckedDescendant;
		}

		private void ChildAssignmentChanged(object? sender, EventArgs e)
		{
			AssignmentChanged?.Invoke(this, EventArgs.Empty);
		}

		private void UpdateDescendantEnabledState()
		{
			foreach (var child in Children)
			{
				child.SetEnabled(!IsChecked);
			}
		}

		private void ClearDescendantSelections()
		{
			foreach (var child in Children)
			{
				child.ClearSelection();
			}
		}

		private void ClearSelection()
		{
			SetProperty(ref _isChecked, false, nameof(IsChecked));

			foreach (var child in Children)
			{
				child.ClearSelection();
			}
		}

		private void SetEnabled(bool enabled)
		{
			IsEnabled = enabled;

			foreach (var child in Children)
			{
				child.SetEnabled(enabled && !IsChecked);
			}
		}
	}
}
