using System.Collections.ObjectModel;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModels.State
{
	/// <summary>
	/// Represents one element in the State assignment tree.
	/// </summary>
	public sealed class CustomPropStateAssignmentTreeNodeViewModel : ViewModelBase
	{
		private readonly StateItemModel _stateItem;
		private readonly Action _assignmentChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomPropStateAssignmentTreeNodeViewModel"/> class.
		/// </summary>
		/// <param name="elementModel">The custom prop element model represented by this node.</param>
		/// <param name="stateItem">The State item being assigned.</param>
		/// <param name="assignmentChanged">The callback invoked when assignments change.</param>
		public CustomPropStateAssignmentTreeNodeViewModel(
			ElementModel elementModel,
			StateItemModel stateItem,
			Action assignmentChanged)
		{
			ElementModel = elementModel ?? throw new ArgumentNullException(nameof(elementModel));
			_stateItem = stateItem ?? throw new ArgumentNullException(nameof(stateItem));
			_assignmentChanged = assignmentChanged ?? throw new ArgumentNullException(nameof(assignmentChanged));
			Children = new ObservableCollection<CustomPropStateAssignmentTreeNodeViewModel>(
				elementModel.Children.Select(child => new CustomPropStateAssignmentTreeNodeViewModel(
					child,
					stateItem,
					assignmentChanged)));
		}

		/// <summary>
		/// Gets the element model represented by this assignment node.
		/// </summary>
		public ElementModel ElementModel { get; }

		/// <summary>
		/// Gets the child assignment nodes.
		/// </summary>
		public ObservableCollection<CustomPropStateAssignmentTreeNodeViewModel> Children { get; }

		/// <summary>
		/// Gets the display name for the assignment node.
		/// </summary>
		public string Name => ElementModel.Name;

		/// <summary>
		/// Gets or sets a value that indicates whether the represented element leaves are assigned.
		/// </summary>
		/// <value><see langword="true" /> for assigned; <see langword="false" /> for unassigned; otherwise partially assigned.</value>
		public bool? IsAssigned
		{
			get
			{
				var leafIds = GetAssignableElementIds().ToList();
				if (!leafIds.Any())
				{
					return false;
				}

				var assignedCount = leafIds.Count(id => _stateItem.ElementModelIds.Contains(id));
				return assignedCount switch
				{
					0 => false,
					var count when count == leafIds.Count => true,
					_ => null
				};
			}
			set
			{
				if (!value.HasValue)
				{
					return;
				}

				var leafIds = GetAssignableElementIds().ToList();
				if (value.Value)
				{
					foreach (var elementModelId in leafIds.Where(id => !_stateItem.ElementModelIds.Contains(id)))
					{
						_stateItem.ElementModelIds.Add(elementModelId);
					}
				}
				else
				{
					foreach (var elementModelId in leafIds)
					{
						_stateItem.ElementModelIds.Remove(elementModelId);
					}
				}

				_assignmentChanged();
				RefreshAssignmentState();
			}
		}

		/// <summary>
		/// Raises assignment state change notifications for this node and all children.
		/// </summary>
		public void RefreshAssignmentState()
		{
			RaisePropertyChanged(nameof(IsAssigned));

			foreach (var child in Children)
			{
				child.RefreshAssignmentState();
			}
		}

		private IEnumerable<Guid> GetAssignableElementIds()
		{
			if (ElementModel.IsLeaf)
			{
				return [ElementModel.Id];
			}

			return ElementModel.GetLeafEnumerator().Select(model => model.Id).Distinct();
		}
	}
}
