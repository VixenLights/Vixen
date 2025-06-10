using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys;
using Vixen.Sys.Managers;

namespace VixenApplication.SetupDisplay.ViewModels
{
	public class PropNodeTreePropViewModel : ViewModelBase
	{
		private readonly PropNodeTreeViewModel _propNodeTreeViewModel;

		public PropNodeTreePropViewModel(PropNodeTreeViewModel propNodeTreeViewModel)
		{
			_propNodeTreeViewModel = propNodeTreeViewModel;
			PropManager = VixenSystem.Props;
			SelectedItems = new();
			SelectedItems.CollectionChanged += SelectedItemsCollectionChanged;
		}

		#region PropManager model property

		/// <summary>
		/// Gets or sets the PropManager value.
		/// </summary>
		public PropManager PropManager
		{
			get { return GetValue<PropManager>(PropManagerProperty); }
			private set { SetValue(PropManagerProperty, value); }
		}

		/// <summary>
		/// Prop property data.
		/// </summary>
		public static readonly IPropertyData PropManagerProperty = RegisterProperty<PropManager>(nameof(PropManager));

		#endregion

		#region LeafNodes property

		/// <summary>
		/// Gets or sets the LeafNodes value.
		/// </summary>
		public ObservableCollection<PropNodeViewModel> LeafNodes
		{
			get
			{
				return _propNodeTreeViewModel.LeafNodes;
			}
		}

		#endregion

		#region SelectedItem property

		/// <summary>
		/// Gets or sets the SelectedItem value.
		/// </summary>
		public PropNodeViewModel? SelectedItem
		{
			get { return GetValue<PropNodeViewModel>(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		/// <summary>
		/// SelectedItem property data.
		/// </summary>
		public static readonly IPropertyData SelectedItemProperty = RegisterProperty<PropNodeViewModel>(nameof(SelectedItem));

		#endregion

		#region SelectedItems property

		/// <summary>
		/// Gets or sets the SelectedItems value.
		/// </summary>
		public ObservableCollection<PropNodeViewModel> SelectedItems
		{
			get { return GetValue<ObservableCollection<PropNodeViewModel>>(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}

		/// <summary>
		/// SelectedItems property data.
		/// </summary>
		public static readonly IPropertyData SelectedItemsProperty =
			RegisterProperty<ObservableCollection<PropNodeViewModel>>(nameof(SelectedItems));


		private void SelectedItemsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (SelectedItems.Count == 1)
			{
				SelectedItem = SelectedItems.First();
			}
			else
			{
				SelectedItem = null;
			}
		}

		#endregion

		#region Selection

		public void DeselectAll()
		{
			SelectedItems.ToList().ForEach(x => x.IsSelected = false);
			SelectedItems.Clear();
		}

		public void SelectModels(IEnumerable<PropNodeViewModel> propNodeModels)
		{
			foreach (var propNodeViewModel in propNodeModels)
			{
				if (!propNodeViewModel.IsSelected)
				{
					propNodeViewModel.IsSelected = true;
				}
				if (!SelectedItems.Contains(propNodeViewModel))
				{
					SelectedItems.Add(propNodeViewModel);
				}
			}
		}

		public void DeselectModels(IEnumerable<PropNodeViewModel> propNodeModels)
		{
			foreach (var propNodeViewModel in propNodeModels)
			{
				if (propNodeViewModel.IsSelected)
				{
					propNodeViewModel.IsSelected = false;
					SelectedItems.Remove(propNodeViewModel);
				}
			}
		}

		#endregion
	}
}