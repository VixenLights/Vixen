using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys;
using Vixen.Sys.Managers;

namespace VixenApplication.SetupDisplay.ViewModels
{
    public class PropNodeTreePropViewModel : ViewModelBase
    {
        public PropNodeTreePropViewModel()
        {
            PropManager = VixenSystem.Props;
			PropNodeViewModel vm = new(PropManager.RootNode, null);
            RootNodeViewModel = [vm];
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

        #region RootNodeViewModels property

        /// <summary>
        /// Gets or sets the RootNodesViewModels value.
        /// </summary>
        public ObservableCollection<PropNodeViewModel> RootNodeViewModel
        {
            get { return GetValue<ObservableCollection<PropNodeViewModel>>(RootNodeViewModelProperty); }
            set { SetValue(RootNodeViewModelProperty, value); }
        }

        /// <summary>
        /// RootNodesViewModels property data.
        /// </summary>
        public static readonly IPropertyData RootNodeViewModelProperty = RegisterProperty<ObservableCollection<PropNodeViewModel>>(nameof(RootNodeViewModel));

        #endregion

        #region LeafNodes property

        /// <summary>
        /// Gets or sets the LeafNodes value.
        /// </summary>
        public ObservableCollection<PropNodeViewModel> LeafNodes
        {
            get
            {
                return new(RootNodeViewModel.SelectMany(x => x.GetLeafEnumerator()));
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
            foreach (var elementModelViewModel in propNodeModels)
            {
                elementModelViewModel.IsSelected = true;
                if (elementModelViewModel.ParentViewModel is PropNodeViewModel parent)
                {
                    parent.IsExpanded = true;
                }
            }
        }

        public void DeselectModels(IEnumerable<PropNodeViewModel> propNodeModels)
        {
            foreach (var elementModelViewModel in propNodeModels)
            {
                elementModelViewModel.IsSelected = false;
            }
        }

        #endregion
	}
}